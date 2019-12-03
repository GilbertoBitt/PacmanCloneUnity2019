using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PacMan : MonoBehaviour {

	public float speed = 4.0f;

	private Vector2Int _direction = Vector2Int.zero;
	public Vector2 nextPosition = Vector2.zero;
	public bool isMoving = false;
	public Orientation currentOrientation = Orientation.None;
	public Orientation nextOrientation = Orientation.None;
	public LayerMask mazeLayer;
	private bool IsOnNextPosition => Math.Abs(Vector2.Distance(transform.localPosition, nextPosition)) <= 0.1f;

	private void Start()
	{
		nextPosition = transform.localPosition;
	}

	// Update is called once per frame
	void Update () {
		MoveTo();
		UpdateOrientation();
		InputUpdate ();
		ValidateDirection();
	}

	void InputUpdate () {
		
		if (Input.GetKeyDown (KeyCode.LeftArrow)){
			nextOrientation = Orientation.Left;
		} else if (Input.GetKeyDown (KeyCode.RightArrow))
		{
			nextOrientation = Orientation.Right;
		} else if (Input.GetKeyDown (KeyCode.UpArrow))
		{
			nextOrientation = Orientation.Up;
		} else if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			nextOrientation = Orientation.Down;
		}
	}

	Vector2Int GetDirectionBasedOnOrientation(Orientation orientation)
	{
		switch (orientation)
		{
			case Orientation.Down:
				return Vector2Int.down;
			case Orientation.Up:
				return Vector2Int.up;
			case Orientation.Left:
				return Vector2Int.left;
			case Orientation.Right:
				return Vector2Int.right;
			case Orientation.None:
			default:
				return Vector2Int.zero;
		}
	}

	Vector2Int NextPosition(Vector2Int direction, Vector2 currentPosition)
	{
		Vector2Int currentPos = Vector2Int.RoundToInt(currentPosition);
		return currentPos + direction;
	}

	void ValidateDirection()
	{
		if (IsOnNextPosition)
		{
			transform.position = nextPosition;
			nextPosition = NextPosition(GetDirectionBasedOnOrientation(currentOrientation), transform.position);
		}
		isMoving = IsValidDirection(currentOrientation, transform.position, nextPosition);
		if (!IsOnNextPosition && !IsValidDirection(nextOrientation, transform.position, NextPosition(GetDirectionBasedOnOrientation(nextOrientation), transform.position))) return;
		currentOrientation = nextOrientation;
		nextPosition = NextPosition(GetDirectionBasedOnOrientation(currentOrientation), transform.position);
		isMoving = IsValidDirection(currentOrientation, transform.position, nextPosition);
	}

	bool IsValidDirection(Orientation orientation, Vector2 position, Vector2 toPos)
	{
		Debug.DrawLine(position, toPos);
		return !Physics2D.Linecast(position,toPos, mazeLayer);
	}

	void MoveTo()
	{
		if (isMoving && currentOrientation != Orientation.None)
		{
			transform.position += (Vector3)((Vector2)(GetDirectionBasedOnOrientation(currentOrientation)) * speed) * Time.deltaTime;
		}
	}

	void UpdateOrientation () {
		switch (currentOrientation)
		{
			case Orientation.Down:
				transform.localScale = new Vector3 (1, 1, 1);
				transform.localRotation = Quaternion.Euler (0, 0, 270);
				break;
			case Orientation.Up:
				transform.localScale = new Vector3 (1, 1, 1);
				transform.localRotation = Quaternion.Euler (0, 0, 90);
				break;
			case Orientation.Left:
				transform.localScale = new Vector3 (-1, 1, 1);
				transform.localRotation = Quaternion.Euler (0, 0, 0);
				break;
			case Orientation.Right:
				transform.localScale = new Vector3 (1, 1, 1);
				transform.localRotation = Quaternion.Euler (0, 0, 0);
				break;
			case Orientation.None:
			default:
				break;
		}
	}
}
