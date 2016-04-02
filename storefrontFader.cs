using UnityEngine;
using System.Collections;

public class storefrontFader : MonoBehaviour {

	public GameObject[] heads;
	public GameObject canvas;
	private GameObject head;


	private Material material;
	private Color color;
	private float lookAngle;

	void Start () {
		material = GetComponent<Renderer> ().sharedMaterial;
		color = material.color;
		color.a = 1.0f;

		initializeHead ();
	}

	void Update () {
		DownwardFade ();
		UpwardFade ();

	
	}

	public void initializeHead(){
		/// copy this 
		InitializeCamera cameraSetup = canvas.GetComponent<InitializeCamera> ();
		int index = InitializeCamera.index;
		head = heads [index];

	}

	void DownwardFade(){
		if (head.transform.eulerAngles.x <= 60.0f && head.transform.eulerAngles.x >= 10.0f) { // 60.0f & 5.0f
			float Angle = head.transform.eulerAngles.x;
			color.a = Angle.Remap (10.0f, 25.0f, 1.0f, 0.0f); //(5.0f, 20.0f, 1.0f, 0.0f)
		} 
		material.color = color;
	}

	void UpwardFade(){
		if (head.transform.eulerAngles.x <= 360.0f && head.transform.eulerAngles.x >= 300.0f) { // 360.0f & 300.0f
			float Angle = head.transform.eulerAngles.x;
			color.a = Angle.Remap (360.0f, 340.0f, 1.0f, 0.0f); //(360.0f, 345.0f, 1.0f, 0.0f)
		} 
		material.color = color;
	}





}
