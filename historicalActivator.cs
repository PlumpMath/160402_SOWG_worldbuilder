using UnityEngine;
using System.Collections;

public class historicalActivator : MonoBehaviour {

	private PrototypeMover prototype;

	void Start () {
	
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player") {

			prototype = other.gameObject.GetComponentInParent<PrototypeMover> ();
			if (prototype.enlarged == false) {
				Debug.Log ("SCALE UP BITCH!");
				prototype.playAudio();
				prototype.ScaleUp ();
			}
			
		}
	}

}
