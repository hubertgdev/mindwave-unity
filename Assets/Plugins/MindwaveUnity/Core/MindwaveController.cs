#region Headers

	using UnityEngine;

	using System.Net.Sockets;
	using System.IO;
	using System.Text;
	using System.Collections;

	// The Jayrock library emulates a web server, and in our case, it allows us to get Mindwave data as JSON, using a simple TCP connection.
	using Jayrock.Json;
	using Jayrock.Json.Conversion;

#endregion

///<summary>
/// 
///		How data can be get from Mindwave ?
///		
///		You must have ThinkGear connector installed on your PC. It provides a "server", and send data bytes from Mindwave to
///		127.0.0.1:13854 with a TCP protocol (see http://developer.neurosky.com/docs/doku.php?id=thinkgear_connector_development_guide).
///		This means the data are accessible like a web server. So we can use a TcpClient in C# to get data.
///
///</summary>
[AddComponentMenu("Scripts/MindwaveUnity/Mindwave Controller")]
public class MindwaveController : MonoBehaviour
{

	#region Attributes

		// Constants & Statics
		
		private const string DEFAULT_TCP_HOSTNAME = "127.0.0.1";
		private const int DEFAULT_TCP_PORT = 13854;

		private const int BUFFER_LENGTH = 1024;

		private const string POOR_SIGNAL_LABEL = "poorSignalLevel";
		private const string RAW_EEG_LABEL = "rawEeg";
		private const string BLINK_STRENGTH_LABEL = "blinkStrength";

		// Delegates

		public delegate void VoidDelegate();
		public delegate void MindwaveDataDelegate(MindwaveDataModel _Data);
		public delegate void IntValueDelegate(int _Value);

		// Triggered when the connection to Mindwave is established.
		public event VoidDelegate OnConnectMindwave;
		// Triggered when the connection to Mindwave is lost (or Disconnect() has been called).
		public event VoidDelegate OnDisconnectMindwave;
		// Triggered when a connection try timeouts.
		public event VoidDelegate OnConnectionTimeout;
		// Triggered when data are get from Mindwave.
		public event MindwaveDataDelegate OnUpdateMindwaveData;
		// Triggered when raw EEG (Electroencephalogram) data are get from Mindwave.
		public event IntValueDelegate OnUpdateRawEEG;
		// Triggered when blink data are get from Mindwave.
		public event IntValueDelegate OnUpdateBlink;

		// Settings

		[Header("Connection settings")]

		[SerializeField, Tooltip("By default: 127.0.0.1")]
		private string m_TcpHostname = DEFAULT_TCP_HOSTNAME;

		[SerializeField, Tooltip("By default: 13854")]
		private int m_TcpPort = DEFAULT_TCP_PORT;

		[Header("Data stream settings")]

		[SerializeField, Tooltip("If it's set to false, you must connect to Mindwave manually using Connect(). Else, automatically try to connect to Mindwave at game start.")]
		private bool m_TryConnectAtStart = false;

		[SerializeField, Range(0.0f, 30.0f), Tooltip("Defines the timing before a connection try timeouts.")]
		private float m_ConnectionTimeout = 10.0f;

		[SerializeField, Range(0.0f, 1.0f), Tooltip("Defines the interval between each Mindwave call.")]
		private float m_UpdateStreamRate = 0.02f;

		[Header("Debug settings")]

		[SerializeField]
		private bool m_ShowDataPackets = true;

		[SerializeField]
		private bool m_ShowStreamErrors = false;

		// Flow

		private TcpClient m_TcpClient = null;
		private Stream m_DataStream = null;
		private byte[] m_Buffer = { };

		private Coroutine m_StreamRoutine = null;
		private bool m_ConnectedFlag = false;
		// This flag is here to check if the conection is timed out
		private bool m_PendingConnection = false;

		private float m_TimeoutTimer = 0.0f;

	#endregion

	
	#region Engine Methods

		private void OnEnable()
		{
			if(m_TryConnectAtStart)
			{
				ConnectToMindwave();
			}
		}

		private void OnDisable()
		{
			DisconnectFromMindwave();
		}

		private void Update()
		{
			UpdateTimeoutTimer(Time.deltaTime);
		}

	#endregion

	
	#region Public Methods

		public void Connect()
		{
			ConnectToMindwave();
		}

		public void Disconnect()
		{
			DisconnectFromMindwave();
		}

	#endregion


	#region Protected Methods
	#endregion

	
	#region Private Methods

		/// <summary>
		/// Try to connect to the Mindwave, and start the data stream.
		/// </summary>
		private void ConnectToMindwave()
		{
			if(m_StreamRoutine == null)
			{
				m_TcpClient = new TcpClient(m_TcpHostname, m_TcpPort);
				m_DataStream = m_TcpClient.GetStream();

				InitBuffer();

				m_StreamRoutine = StartCoroutine(ParseData(m_UpdateStreamRate));

				PendingConnection = true;
			}
		}

		/// <summary>
		/// Initializes the data stream buffer.
		/// </summary>
		private void InitBuffer()
		{
			m_Buffer = new byte[BUFFER_LENGTH];
			byte[] writeBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
			m_DataStream.Write(writeBuffer, 0, writeBuffer.Length);
		}

		/// <summary>
		/// Stop the stream coroutine and close the data stream.
		/// </summary>
		private void DisconnectFromMindwave()
		{
			if(m_StreamRoutine != null)
			{
				StopCoroutine(m_StreamRoutine);
				m_StreamRoutine = null;
			}

			PendingConnection = false;
			ConnectedFlag = false;

			if(m_DataStream != null)
			{
				m_DataStream.Close();
			}
		}

		/// <summary>
		/// Read datas from Mindwave, and parse them.
		/// Also call the relative update events.
		/// </summary>
		/// <param name="_UpdateRate">Defines the interval between each stream read operation.</param>
		private IEnumerator ParseData(float _UpdateRate)
		{
			if(m_DataStream.CanRead)
			{
				int streamBytes = m_DataStream.Read(m_Buffer, 0, m_Buffer.Length);
				string[] packets = Encoding.ASCII.GetString(m_Buffer, 0, streamBytes).Split('\r');

				if(m_ShowDataPackets)
				{
					Debug.Log(Encoding.ASCII.GetString(m_Buffer, 0, streamBytes));
				}

				foreach(string packet in packets)
				{
					if(string.IsNullOrEmpty(packet))
					{
						continue;
					}

					try
					{
						// Convert data to JSON
						IDictionary data = (IDictionary)JsonConvert.Import(typeof(IDictionary), packet);

						// This kind of packet contains all the EEG infos such as brain waves and attention/meditation metrics
						if(data.Contains(POOR_SIGNAL_LABEL))
						{
							MindwaveDataModel model = JsonUtility.FromJson<MindwaveDataModel>(packet);

							// If the current packet reveal a connection trouble, call OnDisconnect delegate
							if(model.NoSignal)
							{
								ConnectedFlag = false;
							}

							// Else, data are sent, so emit it
							else
							{
								ConnectedFlag = true;
								PendingConnection = false;

								if (OnUpdateMindwaveData != null)
								{
									OnUpdateMindwaveData(model);
								}
							}
						}
						
						// Check for EEG raw data, and emit it
						else if (data.Contains(RAW_EEG_LABEL))
						{
							if (OnUpdateRawEEG != null)
							{
								OnUpdateRawEEG(int.Parse(data[RAW_EEG_LABEL].ToString()));
							}
						}

						// Check for eye blinking, and emit it
						else if (data.Contains(BLINK_STRENGTH_LABEL))
						{
							if (OnUpdateBlink != null)
							{
								OnUpdateBlink(int.Parse(data[BLINK_STRENGTH_LABEL].ToString()));
							}
						}
					}

					catch(IOException _JsonException)
					{
						if(m_ShowStreamErrors)
						{
							Debug.LogWarning("MindwaveBinding stream Error: " + _JsonException.ToString());
						}
					}

					catch (JsonException _JsonException)
					{
						if (m_ShowStreamErrors)
						{
							Debug.LogWarning("MindwaveBinding stream Error: " + _JsonException.ToString());
						}
					}

					catch(System.Exception _Exception)
					{
						Debug.LogError("MindwaveBinding error: " + _Exception.ToString());
					}
				}
			}

			// Else, if the stream can't be read
			else
			{
				ConnectedFlag = false;
			}

			yield return new WaitForSeconds(_UpdateRate);

			m_StreamRoutine = StartCoroutine(ParseData(_UpdateRate));
		}

		/// <summary>
		/// If there's a pending connection, update the connection timeout timer.
		/// If the connection timeout delay is elapsed, call Disconnect().
		/// </summary>
		private void UpdateTimeoutTimer(float _DeltaTime)
		{
			if(m_PendingConnection)
			{
				m_TimeoutTimer += _DeltaTime;
				if(m_TimeoutTimer >= m_ConnectionTimeout)
				{
					if(OnConnectionTimeout != null)
					{
						OnConnectionTimeout();
					}
					Disconnect();
				}
			}
		}

	#endregion

	
	#region Accessors

		/// <summary>
		/// When setting ConnectedFlag, check if the Mindwave is already in the target state.
		/// If not, emit OnConnectMindwave or OnDisconnectMindwave event, and change the ConnectedFlag value.
		/// </summary>
		private bool ConnectedFlag
		{
			get { return m_ConnectedFlag; }
			set
			{
				if(m_ConnectedFlag != value)
				{
					m_ConnectedFlag = value;
					if(m_ConnectedFlag)
					{
						if (OnConnectMindwave != null)
						{
							OnConnectMindwave();
						}
					}
					else
					{
						if (OnDisconnectMindwave != null)
						{
							OnDisconnectMindwave();
						}
					}
				}
			}
		}

		/// <summary>
		/// If a new pending connection is triggered, reset the connection timeout timer.
		/// </summary>
		private bool PendingConnection
		{
			get { return m_PendingConnection; }
			set
			{
				if(m_PendingConnection != value)
				{
					m_PendingConnection = value;
					if(m_PendingConnection)
					{
						m_TimeoutTimer = 0.0f;
					}
				}
			}
		}

		public bool IsConnecting
		{
			get { return m_PendingConnection; }
		}

		public bool IsConnected
		{
			get { return m_ConnectedFlag; }
		}

		public float TimeoutTimer
		{
			get { return m_TimeoutTimer; }
		}

		public float ConnectionTimeoutDelay
		{
			get { return m_ConnectionTimeout; }
		}

	#endregion

}