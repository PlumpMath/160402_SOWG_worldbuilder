using UnityEngine;
using System.Collections;

public class archiveMover : MonoBehaviour {

	public GameObject[] cardboardMains;
	public GameObject canvas;
	private GameObject cardboardMain;

	//public GameObject cardboardMain;
	private float velocityScale = 2.5f;

	private Rigidbody rb;

	private Vector3 moveDir;
	private Transform myTransform;
	private Transform cbTransform;
	private Vector3 lastPos;
	private Vector3 vel;



	void Awake(){

		InitializeCamera cameraSetup = canvas.GetComponent<InitializeCamera> ();
		int index = InitializeCamera.index;
		cardboardMain = cardboardMains [index];

		myTransform = GetComponent<Transform> ();
		cbTransform = cardboardMain.GetComponent<Transform> ();
		rb = cardboardMain.GetComponent<Rigidbody> ();

	

		lastPos = cardboardMain.transform.position;
	}


	void Update(){
		vel = (lastPos - cardboardMain.transform.position) / Time.deltaTime;
		//moveDir = new Vector3 (0, 0, -cardboardMain.transform.localPosition.x * velocityScale);
		moveDir = new Vector3 (0, 0, vel.x * velocityScale);


		myTransform.Translate (moveDir * Time.deltaTime);
		lastPos = cardboardMain.transform.position;



	}









}
