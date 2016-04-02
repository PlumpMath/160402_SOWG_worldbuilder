using UnityEngine;
using System.Collections;

public class InitializePathArrows : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		CameraPathAnimator[] cpas = GetComponents<CameraPathAnimator>();

		foreach (CameraPathAnimator cpa in cpas) {
			//cpa.pathSpeed = 0.4f;
			//cpa.Stop ();
		}
	
	}

}
