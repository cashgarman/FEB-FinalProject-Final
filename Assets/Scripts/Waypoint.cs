using UnityEngine;

internal class Waypoint : MonoBehaviour
{
	[SerializeField] private float _waitTime = 3f;
	public float WaitTime => _waitTime;
	private Vector3 _position;
	public Vector3 Position => _position;

	private void Awake()
	{
		_position = transform.position;
	}
}