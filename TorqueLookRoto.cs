using UnityEngine;
using System.Collections;

public class TorqueLookRoto : MonoBehaviour {

	public bool isRotating = false;
	public Transform target;
	public float force = 0.1f;

	private Rigidbody rb;
	private Quaternion homeRotation;

	void Start (){
		rb = GetComponent<Rigidbody> ();
		isRotating = false;
		homeRotation = transform.rotation;
	}


	void FixedUpdate () {
		if (isRotating == true) {
			//TorqueRotate ();
			TorqueRoto2 ();
		}
		//TorqueRoto();
	}


	void TorqueRotate(){

		Vector3 targetDelta = target.transform.forward - transform.position;

		//get the angle between transform.forward and target delta
		float angleDiff = Vector3.Angle(transform.up, targetDelta);

		// get its cross product, which is the axis of rotation to
		// get from one vector to the other
		Vector3 cross = Vector3.Cross(transform.up, targetDelta);

		// apply torque along that axis according to the magnitude of the angle.
		rb.AddTorque(cross * angleDiff * force);

		float checkAngle = Quaternion.Angle(transform.rotation, homeRotation);

		if (checkAngle < 5.0f) {
			transform.rotation = homeRotation;
			isRotating = false;
		}
	}


	void TorqueRoto2(){
		float alignmentSpeed = 0.025f;
		float alignmentDamping = 0.2f;

		Quaternion targetRotation = homeRotation;

		Quaternion deltaRotation = Quaternion.Inverse(transform.rotation) * targetRotation;
		Vector3 deltaAngles = GetRelativeAngles(deltaRotation.eulerAngles);
		Vector3 worldDeltaAngles = transform.TransformDirection(deltaAngles);

		// alignmentSpeed controls how fast you rotate the body towards the target rotation
		// alignmentDamping prevents overshooting the target rotation
		// Values used: alignmentSpeed = 0.025, alignmentDamping = 0.2
		rb.AddTorque(alignmentSpeed * worldDeltaAngles - alignmentDamping * rb.angularVelocity);

		// Convert angles above 180 degrees into negative/relative angles

	}

	Vector3 GetRelativeAngles(Vector3 angles)
	{
		Vector3 relativeAngles = angles;
		if (relativeAngles.x > 180f)
			relativeAngles.x -= 360f;
		if (relativeAngles.y > 180f)
			relativeAngles.y -= 360f;
		if (relativeAngles.z > 180f)
			relativeAngles.z -= 360f;

		return relativeAngles;
	}


}
