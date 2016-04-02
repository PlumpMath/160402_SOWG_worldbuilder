using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PrototypeMover : MonoBehaviour {
	

	public float globalScale = 1.0f;
	public float originalScale;

	public bool enlarged = false;

	public bool audioPlay = false;
	public bool isMoving = false;
	public Transform[] targets;
	public float speed = 5.0f;
	public float maxVelocity = 1.5f;
	public float dampingThreshold = 0.05f;
	public float timeAbove = 5.0f;

	private Rigidbody rb;
	public Transform target;
	private int targetIndex = 0;
	private Transform myTransform;

	private AudioSource audio;
	

	// Use this for initialization
	void Start () {
		
		myTransform = gameObject.GetComponent<Transform> ();
		AcquireScale ();

		audio = GetComponent<AudioSource> ();
		target = targets[targetIndex];
		rb = gameObject.GetComponent<Rigidbody> ();

		isMoving = false;

		enlarged = false;

	}

	public void iterateTarget(){
		int size = targets.Length-1; 
		if (targetIndex < size) { 
			targetIndex++;
		} else {
			targetIndex = 0;
		}
		target = targets [targetIndex];
		if (myTransform.position.y > 0) {
			isMoving = true;
			GetComponent<TorqueLookRoto> ().isRotating = true;
			StartCoroutine (ScaleDown (5.0f));
		} else {
			isMoving = false;
			enlarged = false;
		}
	}

	void FixedUpdate () {
		if (isMoving == true)
		move ();
	}

	public void move(){
			Vector3 targetDelta = target.position - myTransform.position;
			float dist = targetDelta.magnitude;
			if (dist > dampingThreshold){
				// move towards
				targetDelta.Normalize ();
				float mag = (1 / dist) * speed;
				rb.AddForce (targetDelta * mag);
			}
			rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
	}

	public void ScaleUp(){
		StartCoroutine (ScaleUp (2.0f));
	}

	IEnumerator ScaleUp(float time) 	{
		Vector3 startScale = myTransform.localScale;
		Vector3 endScale = myTransform.localScale * originalScale;
		float currentTime = 0.0f;

		do {
			myTransform.localScale = Vector3.Lerp (startScale, endScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
		enlarged = true;
	}

	public void ScaleDown(){
		StartCoroutine (ScaleDown (2.0f));
	}

	IEnumerator ScaleDown(float time) 	{
		Vector3 startScale = myTransform.localScale;
		Vector3 endScale = myTransform.localScale * (1.0f/originalScale);
		float currentTime = 0.0f;

		do {
			myTransform.localScale = Vector3.Lerp (startScale, endScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
	}

	public void playAudio(){
		Debug.Log ("SOUND");
		audio.Stop ();
		audio.PlayOneShot (audio.clip, 1.0f);
		audioPlay = false;
	}


	public void AcquireScale(){
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

		Bounds bounds = mesh.bounds;
		Debug.Log (bounds);
		// normalize meshx size
		float xsize = bounds.size.x;
		//Debug.Log (xsize);
		float scale = 1/xsize * globalScale;
		myTransform.localScale = myTransform.localScale * scale;
		// store scale up factor of original mesh
		originalScale = xsize/globalScale;
	}

		

}
