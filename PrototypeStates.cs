using UnityEngine;
using System.Collections;

public class PrototypeStates : MonoBehaviour {

	public GameObject[] heads;
	public GameObject canvas;
	private GameObject head;

	public bool archived;
	public bool movingUp;
	public bool scalingUp;
	public bool suspended;
	public bool movingDown;
	private bool stopped;
	private bool scaledDown;
	public bool focusSound;

	public float speed = 1.5f;
	public float maxVelocity = 1.5f;
	public Transform upTarget;
	public Transform homeTarget;
	public float dampingFactor = 0.98f;
	public float velocityThreshold = 0.2f;
	public float dampingThreshold = 7.0f;
	public float waitAtTarget = 20.0f;
	public AudioSource audio;
	public float globalScale = 1.5f;
	public float originalScale;
	public float snapDistance = 0.2f;

	private Quaternion homeRotation;
	private Transform myTransform;
	private Rigidbody rb;

	void Awake(){
		
		InitializeCamera cameraSetup = canvas.GetComponent<InitializeCamera> ();
		int index = InitializeCamera.index;
		head = heads [index];

		myTransform = gameObject.transform;
		AcquireScale ();
		homeRotation = myTransform.rotation;
		audio = GetComponent<AudioSource> ();
		rb = gameObject.GetComponent<Rigidbody> ();
		archived = true;
		movingUp = false;
		scalingUp = false;
		suspended = false;
		stopped = false;
		movingDown = false;
		scaledDown = false;
		focusSound = false;
	}

	void AcquireScale(){
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		Bounds bounds = mesh.bounds;
		// normalize meshx size
		float xsize = bounds.size.x;
		float scale = 1/xsize * globalScale;
		myTransform.localScale = myTransform.localScale * scale;
		// store scale up factor of original mesh
		originalScale = xsize/globalScale;
	}

	void FixedUpdate(){
		if (archived == true)
			archivePrototype ();
		if (movingUp == true)
			moveUp ();
		if (scalingUp == true)
			scaleUp ();
		if (suspended == true)
			suspendRotate ();
		if (movingDown == true) {
			moveDown ();
			rotatingHome ();
		}
	}

	public void archivePrototype (){
		if (rb.isKinematic == false)
			rb.isKinematic = true;
	}

	public void moveUp(){
		rb.isKinematic = false;
		Vector3 targetDelta = upTarget.position - myTransform.position;
		float dist = targetDelta.magnitude;
		targetDelta.Normalize ();
		float mag = (1 / dist) * speed;
		rb.AddForce (targetDelta * mag);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

		if (myTransform.position.y > dampingThreshold) {
			movingUp = false;
			scalingUp = true;
		}
	}

	public void moveDown(){
		if (scaledDown == false)
			StartCoroutine (ScaleDown (5.0f));
		
		Vector3 targetDelta = homeTarget.position - myTransform.position;
		float dist = targetDelta.magnitude;
		targetDelta.Normalize ();
		float mag = (1 / dist) * speed;
		rb.AddForce (targetDelta * mag);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

		if (dist <= snapDistance) {
			movingDown = false;
			archived = true;
			scaledDown = false;
		}
	}

	public void scaleUp(){
		StartCoroutine (ScaleUp (3.0f));
		StartCoroutine (DampVelocity ());
		playAudio ();
		scalingUp = false;
		suspended = true;
	}

	IEnumerator ScaleUp(float time) {
		Vector3 startScale = myTransform.localScale;
		Vector3 endScale = myTransform.localScale * originalScale;
		float currentTime = 0.0f;

		do {
			myTransform.localScale = Vector3.Lerp (startScale, endScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
	}

	IEnumerator DampVelocity() {
		do { 
			rb.velocity *= dampingFactor;
			yield return new WaitForEndOfFrame();
		} while (rb.velocity.magnitude > velocityThreshold);		

		rb.velocity = Vector3.zero;
		float random = (float) Random.Range (80, 120) / 100;
		yield return new WaitForSeconds(waitAtTarget*random);
		stopped = true;
	}

	public void playAudio(){
		audio.Stop ();
		audio.Play ();
		StartCoroutine (AdjustVolume (0.1f, 0.5f, 1.0f));
	}

	private IEnumerator AdjustVolume(float minVolume, float midVolume, float maxVolume){
		float t = Time.deltaTime;
		float clipLength = audio.clip.length;

		while (t <= clipLength) {
			if (focusSound == true) {
				audio.volume = Mathf.Lerp (audio.volume, maxVolume, Time.deltaTime * 2.0f); 
			}
			else {
				focusSound = false;
				if (head.transform.eulerAngles.x <= 360.0f && head.transform.eulerAngles.x >= 300.0f) {
					float Angle = head.transform.eulerAngles.x;
					Angle = Mathf.Clamp (Angle, 300.0f, 360.0f);
					audio.volume = Angle.Remap (360.0f, 345.0f, minVolume, midVolume);
				} else
					audio.volume = Mathf.Lerp (audio.volume, minVolume, Time.deltaTime * 2.0f);
			}
			yield return null;
		} 
	}

	public void suspendRotate (){
		Vector3 targetDelta = head.transform.position - myTransform.position;
		float angleDiff = Vector3.Angle(myTransform.forward, targetDelta);
		Vector3 cross = Vector3.Cross(myTransform.forward, targetDelta);
		rb.AddTorque(cross * angleDiff * 0.005f);

		if (stopped == true) {
			suspended = false;
			movingDown = true;
			stopped = false;
		}
	}

	IEnumerator ScaleDown(float time) {
		scaledDown = true;
		Vector3 startScale = myTransform.localScale;
		Vector3 endScale = myTransform.localScale * (1.0f/originalScale);
		float currentTime = 0.0f;

		do {
			myTransform.localScale = Vector3.Lerp (startScale, endScale, currentTime / time);
			currentTime += Time.deltaTime;
			yield return null;
		} while (currentTime <= time);
	}

	public void rotatingHome () {
		float alignmentSpeed = 0.015f;
		float alignmentDamping = 0.2f;

		Quaternion targetRotation = homeRotation;
		Quaternion deltaRotation = Quaternion.Inverse(myTransform.rotation) * targetRotation;
		Vector3 deltaAngles = GetRelativeAngles(deltaRotation.eulerAngles);
		Vector3 worldDeltaAngles = myTransform.TransformDirection(deltaAngles);

		// alignmentSpeed controls how fast you rotate the body towards the target rotation
		// alignmentDamping prevents overshooting the target rotation
		// Values used: alignmentSpeed = 0.025, alignmentDamping = 0.2
		rb.AddTorque(alignmentSpeed * worldDeltaAngles - alignmentDamping * rb.angularVelocity);
		// Convert angles above 180 degrees into negative/relative angles
	}

	Vector3 GetRelativeAngles(Vector3 angles)
	{
		Vector3 relativeAngles = angles;
		if (relativeAngles.x > 180f)
			relativeAngles.x -= 360f;
		if (relativeAngles.y > 180f)
			relativeAngles.y -= 360f;
		if (relativeAngles.z > 180f)
			relativeAngles.z -= 360f;

		return relativeAngles;
	}


		


}
