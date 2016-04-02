using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class textDisplay : MonoBehaviour {

	public GameObject[] cameras;
	public GameObject initialCanvas;
	private GameObject camera;

	public bool isOn = false;
	//public GameObject camera;
	public PrototypeStates prototype;
	public float originalScale;

	private bool textHidden;

	private RectTransform canvas;
	private Vector3 hidden;
	private Text text;

	void Start () {
		canvas = transform.GetComponentInChildren<RectTransform> ();
		text = canvas.GetComponentInChildren<Text> ();

		InitializeCamera cameraSetup = initialCanvas.GetComponent<InitializeCamera> ();
		int index = InitializeCamera.index;
		camera = cameras [index];

		//canvas = transform.GetComponentInChildren<RectTransform> ();
		//text = canvas.GetComponentInChildren<Text> ();
		prototype = transform.GetComponentInParent<PrototypeStates> ();
		originalScale = prototype.originalScale;
		hidden = new Vector3 (0.0f, 0.0f, 1.0f);
		canvas.localScale = hidden;
		text.enabled = false;
		isOn = false;
	}

	void Update () {
		if (isOn == true)
			showText ();
		else if (textHidden == false && prototype.archived == true) {
			StartCoroutine (hideText2 (5.0f));
		} else if (isOn == false && prototype.archived == false){
			hideTextTriggered ();
			//StartCoroutine (hideTextTriggered2 ());
		}
	}

	private void showText(){
		transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
		Vector3 start = canvas.localScale;
		Vector3 end = new Vector3 (0.01f, 0.01f, 1f) * originalScale;
		canvas.localScale = Vector3.Lerp (start, end, Time.deltaTime * 5.0f);
		text.enabled = true;
		textHidden = false;
	}

	public void hideTextTriggered(){
		transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
		Vector3 start = canvas.localScale;
		canvas.localScale = Vector3.Lerp (start, hidden, Time.deltaTime * 0.5f);
		textHidden = true;
	}


	private IEnumerator hideText2 (float fadeTime){
		float t = 0.0f;
		Vector3 start = canvas.localScale;
		Vector3 end = new Vector3 (0.0f, 0.0f, 1f);

		while (t < fadeTime) {
			canvas.localScale = Vector3.Lerp (start, end, (t / fadeTime));
			t += Time.deltaTime;
		}
		textHidden = true;
		t = 0.0f;
		yield return null;
	}


	/*private IEnumerator hideTextTriggered2(){
		float timer;
		float cutoffTime = 2.0f;

		Vector3 startScale = canvas.localScale;

		canvas.localScale = Vector3.Lerp (startScale, hidden, Time.deltaTime * 0.4f);

		timer += Time.deltaTime;

		if (timer > cutoffTime) {
			canvas.localScale = Vector3.zero;
			timer = 0.0f;
			//yield break;
		}

		yield return null;


	}*/

}


