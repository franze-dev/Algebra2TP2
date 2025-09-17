using UnityEngine;

public class QuaternionExample : MonoBehaviour
{
	public Vector3 angle = Vector3.zero;

	private float sinAngle;
	private float cosAngle;

	private Quaternion qx;
	private Quaternion qy;
	private Quaternion qz;

	private Quaternion result;

	void Start()
	{
	}

	void Update()
	{
		sinAngle = Mathf.Sin(Mathf.Deg2Rad * angle.z * 0.5f);
		cosAngle = Mathf.Cos(Mathf.Deg2Rad * angle.z * 0.5f);
		qz.Set(0, 0, sinAngle, cosAngle);

		sinAngle = Mathf.Sin(Mathf.Deg2Rad * angle.x * 0.5f);
		cosAngle = Mathf.Cos(Mathf.Deg2Rad * angle.x * 0.5f);
		qx.Set(sinAngle, 0, 0, cosAngle);

		sinAngle = Mathf.Sin(Mathf.Deg2Rad * angle.y * 0.5f);
		cosAngle = Mathf.Cos(Mathf.Deg2Rad * angle.y * 0.5f);
		qy.Set(0, sinAngle, 0, cosAngle);

		result = qy * qx * qz;
		transform.rotation = result;
	}
}
