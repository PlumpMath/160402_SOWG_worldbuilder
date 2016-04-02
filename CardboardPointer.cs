using UnityEngine;
using System.Collections;

public class CardboardPointer : MonoBehaviour {

	public void OnGazeEnabled (){

	}

	public void OnGazeDisabled(){

	}

	public void OnGazeStart(Camera camera, GameObject targetObject, Vector3 intersectionPosition){
		Debug.Log ("GAZING!");

	}

	public void OnGazeStay(Camera camera, GameObject targetObject, Vector3 intersectionPosition){

	}

	public void OnGazeExit(Camera camera, GameObject targetObject){

	}





	public void OnGazeTriggerStart(Camera camera){
	}
	public void OnGazeTriggerEnd(Camera camera){
	}
}
