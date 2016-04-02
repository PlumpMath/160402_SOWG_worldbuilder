using UnityEngine;
using System.Collections;

public class FloorFader : MonoBehaviour {

	public GameObject head;

	private Material material;
	private Color color;
	private float lookAngle;

	void Start () {
		material = GetComponent<Renderer> ().material;
		color = material.color;
		color.a = 1.0f;

	}
		
	void Update () {
		if (head.transform.eulerAngles.x <= 60.0f && head.transform.eulerAngles.x >= 5.0f) {
			float Angle = head.transform.eulerAngles.x;

			color.a = Angle.Remap (5.0f, 20.0f, 0.5f, 0.0f);
		} 

		material.color = color;

	}
		



}