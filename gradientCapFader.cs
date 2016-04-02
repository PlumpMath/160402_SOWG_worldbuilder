using UnityEngine;
using System.Collections;

public class gradientCapFader : MonoBehaviour {

	public GameObject head;

	private Material material;
	private Color color;
	private float lookAngle;

	void Start () {
		material = GetComponent<Renderer> ().material;
		color = material.color;
		color.a = 0.0f;

	}

	void Update () {
		DownwardFade ();
		UpwardFade ();
	}

	void DownwardFade(){
		if (head.transform.eulerAngles.x <= 15.0f && head.transform.eulerAngles.x >= 0.0f) {
			float Angle = head.transform.eulerAngles.x;
			color.a = Angle.Remap (15.0f, 5.0f, 1.0f, 0.0f);
		} 
		material.color = color;
	}

	void UpwardFade(){
		if (head.transform.eulerAngles.x <= 360.0f && head.transform.eulerAngles.x >= 300.0f) {
			float Angle = head.transform.eulerAngles.x;
			color.a = Angle.Remap (345.0f, 360.0f, 1.0f, 0.0f);
		} 
		material.color = color;
	}





}