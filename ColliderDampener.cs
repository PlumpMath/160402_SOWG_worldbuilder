using UnityEngine;
using System.Collections;

public class ColliderDampener : MonoBehaviour {

	// We will multiply our sphere velocity by this number with each frame, thus dumping it
	public float dampingFactor = 0.98f;
	// After our velocity will reach this threshold, we will simply set it to zero and stop damping
	public float dampingThreshold = 0.2f;
	public float waitAtTarget = 3.0f;
	private PrototypeMover prototype;
	
	void OnTriggerEnter(Collider other) {
		//other.gameObject.tag == "Player"
		// Transfer rigidbody of the sphere to the damping coroutine
		if(other.gameObject.tag == "Player"){
			prototype = other.gameObject.GetComponent<PrototypeMover> ();
			// only slow down if in the target collider
			if (prototype.target == transform){
			StartCoroutine (DampVelocity (other.GetComponent<Rigidbody> (), other));
			}
		}

	}
	
	IEnumerator DampVelocity(Rigidbody target, Collider other) 	{
		Debug.Log ("Slowing down!");
		do {
			// Here we are damping (simply multiplying) velocity of the sphere whith each frame, until it reaches our threshold 
			target.velocity *= dampingFactor;
			yield return new WaitForEndOfFrame();
		} while (target.velocity.magnitude > dampingThreshold);		

		// Completely stop momentum
		target.velocity = Vector3.zero;
		prototype.isMoving = false;
		Debug.Log ("Landed!");
		yield return new WaitForSeconds(waitAtTarget);
		prototype.iterateTarget ();
	}


}





