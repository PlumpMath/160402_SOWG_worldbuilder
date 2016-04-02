using UnityEngine;
using System.Collections;
using System;

public class InitializeCamera : MonoBehaviour {
	public CameraPath camerapath;
	public Transform[] cameras;
	public bool checkTime = false;

	private float[] startPositions = new float[7];
	public  int goHour;
	public  int  goMinute;
	private float pathSpeed;

	// set starting position (0-6)
	public static int index = 4;

	public GameObject[] thingsToActivate;


	void Awake(){
		// set initial camera position positions
		float step = 1.0f / 7.0f;
		for (int i = 0; i < 7; i++) {
			startPositions[i] = (float) i*step;
		}
		// explicitly set two positions
		startPositions [3] = 0.376f;
		startPositions [6] = 0.82f;

		// get time
		DateTime time = DateTime.Now;
		int hour = time.Hour;
		int minute = time.Minute;

		goHour = hour;
		goMinute = ((int)Math.Ceiling(minute / 10.0)) * 10;
		if (goMinute > 51) {
			goHour++;
			goMinute = 00;
		}
		if (goHour > 24)
			goHour = 01;
	}

	void Start(){
		// enable touch in vr
		//Cardboard.SDK.TapIsTrigger = false;

		//Initialize OSC clients (transmitters)
		CreateClient("otherCamera", IPAddress.Parse("127.0.0.1"), 5555);
		//Initialize OSC servers (listeners)
		CreateServer("thisCamera", 6666);

	}

	void Update(){
		
		DateTime time = DateTime.Now;
		int hour = time.Hour;
		int minute = time.Minute;
		// start on next 10th minute
		if (checkTime) {
			if ((hour.CompareTo (goHour) == 0) && (minute.CompareTo (goMinute) == 0)) {
				startCamera ();
			}
		} else {
			startCamera ();
		}
	}
		
	
	public void startCamera(){
		cameras [index].gameObject.SetActive (true);
		// enable touch in vr
		//Cardboard.SDK.TapIsTrigger = false;
		CameraPathAnimator cpa = camerapath.GetComponent<CameraPathAnimator> ();
		cpa.animationObject = cameras [index];
		cpa.Seek (startPositions[index]);
		cpa.Play ();

		// switch on vr, switch off canvas
		//Cardboard.SDK.TapIsTrigger = true;
		gameObject.SetActive (false);

		//initialize rest
		initializeGameObject();

	}

	void initializeGameObject(){
		for (int i = 0; i < thingsToActivate.Length; i++) {
			thingsToActivate [i].SetActive (true);
		}
	}
	

}
