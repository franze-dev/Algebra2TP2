using UnityEngine;

public class TwoDRotationExample : MonoBehaviour
{
	public float angle;
	void Start()
	{

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Vector3 rotation = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
			transform.position = new Vector3(transform.position.x * rotation.x - transform.position.y * rotation.y,
											 transform.position.x * rotation.y + transform.position.y * rotation.x,
											 0.0f);
		}
	}
}
