using UnityEngine;

namespace MuffinTools
{

[AddComponentMenu("Muffin Tools/UI/Show FPS")]
public class ShowFPS : MonoBehaviour
{

	#region Enums & Subclasses

		private enum HorizontalPosition
		{
			Left,
			Center,
			Right
		}

		private enum VerticalPosition
		{
			Top,
			Center,
			Bottom
		}

	#endregion

	#region Attributes

		// Settings

		[Header("Settings")]

		[SerializeField]
		private HorizontalPosition m_HorizontalPosition = HorizontalPosition.Right;

		[SerializeField]
		private VerticalPosition m_VerticalPosition = VerticalPosition.Top;

		[SerializeField]
		private Vector2 m_BoxSize = new Vector2(140.0f, 24.0f);

		// Flow

		private float m_DeltaTime = 0.0f;

	#endregion

	
	#region Engine Methods

		private void Update()
		{
			m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
		}

	#endregion

	
	#region Private Methods

		private void OnGUI()
		{
			float msec = m_DeltaTime * 1000.0f;
			float fps = 1.0f / m_DeltaTime;

			Rect box = new Rect(CalculatePosition(), m_BoxSize);
			GUI.Box(box, string.Format("{0:0.0}ms ({1:0.}fps)", msec, fps));
		}

		private Vector2 CalculatePosition()
		{
			return new Vector2
			(
				CalculateHorizontalPosition(),
				CalculateVerticalPosition()
			);
		}

		private float CalculateHorizontalPosition()
		{
			float xPos = 0.0f;

			switch (m_HorizontalPosition)
			{
				case HorizontalPosition.Left:
					xPos = 0.0f;
					break;

				case HorizontalPosition.Center:
					xPos = Screen.width / 2 - m_BoxSize.x / 2;
					break;

				case HorizontalPosition.Right:
					xPos = Screen.width - m_BoxSize.x;
					break;

				default:
					break;
			}

			return xPos;
		}

		private float CalculateVerticalPosition()
		{
			float yPos = 0.0f;

			switch (m_VerticalPosition)
			{
				case VerticalPosition.Top:
					yPos = 0.0f;
					break;

				case VerticalPosition.Center:
					yPos = Screen.height / 2 - m_BoxSize.y / 2;
					break;

				case VerticalPosition.Bottom:
					yPos = Screen.height - m_BoxSize.y;
					break;

				default:
					break;
			}

			return yPos;
		}

	#endregion

}

}