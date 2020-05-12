using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{

	public Player_Controller player;
	public Vector2 focusAreaSize;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

	// Start is called before the first frame update
	void Start()
	{
		focusArea = new FocusArea(player.characterController.bounds, focusAreaSize);
	}

	private void LateUpdate()
	{

		//Debug.Log(Camera.main.scaledPixelWidth + ":" + Camera.main.scaledPixelHeight);
		focusArea.Update(player.characterController.bounds);

		Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

		if (focusArea.Velocity.x != 0)
		{
			lookAheadDirX = Mathf.Sign(focusArea.Velocity.x);
			if (Mathf.Sign(player.moveDirection.x) == Mathf.Sign(focusArea.Velocity.x) && player.moveDirection.x != 0)
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else
			{
				if (!lookAheadStopped)
				{
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
				}
			}
		}


		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}


	struct FocusArea
	{
		public Vector2 center;
		float left, right;
		float top, bottom;
		public Vector2 Velocity;

		public FocusArea(Bounds targetsBounds, Vector2 size)
		{
			left = targetsBounds.center.x - size.x / 2;
			right = targetsBounds.center.x + size.x / 2;
			top = targetsBounds.center.y + size.y / 2;
			bottom = targetsBounds.center.y - size.y / 2;

			Velocity = Vector2.zero;
			center = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			center = new Vector2((left + right) / 2, (top + bottom) / 2);
			Velocity = new Vector2(shiftX, shiftY);
		}
	}
}
