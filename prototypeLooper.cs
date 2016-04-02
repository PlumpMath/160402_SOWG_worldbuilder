using UnityEngine;
using System.Collections;

public class prototypeLooper : MonoBehaviour {



	private float bufferRight = 8.65f;
	private float bufferLeft = -50.45f;

	void Update () {
		checkBorders ();
	}

	public void checkBorders(){
		if (transform.position.x > bufferRight) {
			//Debug.Log ("more than buffer");
			transform.position = new Vector3(bufferLeft , transform.position.y, transform.position.z);
		}
		if (transform.position.x < bufferLeft) {
			//Debug.Log ("less than buffer");
			transform.position = new Vector3(bufferRight , transform.position.y, transform.position.z);
		}
	}
}
