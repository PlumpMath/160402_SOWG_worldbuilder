using UnityEngine;
using System.Collections;

// @robotduck 2011
// set the object's rigidbody angular drag to a high value, like 10

public class TorqueLookRotation : MonoBehaviour {
	
	public Transform target;
	public float force = 0.1f;
	public float range = 10.0f;


	void FixedUpdate () {
		TorqueRotate ();
	}


	void TorqueRotate() {

			float d = Vector3.Distance (target.position, transform.position);
			if (d < range) {
				Vector3 targetDelta = target.position - transform.position;
				//get the angle between transform.forward and target delta
				float angleDiff = Vector3.Angle (transform.forward, targetDelta);
				
				// get its cross product, which is the axis of rotation to
				// get from one vector to the other
				Vector3 cross = Vector3.Cross (transform.forward, targetDelta);
				
				// apply torque along that axis according to the magnitude of the angle.

				GetComponent<Rigidbody> ().AddTorque (cross * angleDiff * force/d);

		}
	}
}