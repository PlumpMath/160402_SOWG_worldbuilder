using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gazeTrigger : MonoBehaviour {

	public GameObject detritusGO;

	public LayerMask myLayerMask;
	public Image reticle;
	private Color retcolor;
	private RaycastHit hit;
	private textDisplay text;
	private PrototypeStates prototype;
	private float timer;
	private float triggerTime = 1.25f;

	void Start(){
		reticle.fillAmount = 0.0f;
		retcolor = reticle.material.color;
		retcolor.a = 1.0f;
	}

	void Update () {
		gazeTrigger2 ();
	}


	private void gazeTrigger2(){
		Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 18.0f, myLayerMask)) {
			prototype = hit.collider.gameObject.GetComponentInParent<PrototypeStates> ();
			prototype.focusSound = true;

			if (prototype.archived == true) {
				text = hit.collider.gameObject.GetComponentInParent<textDisplay> ();
				turnTextOn ();
				timer += Time.deltaTime;
				reticle.fillAmount += 1.0f / triggerTime * Time.deltaTime;
			}

			if (timer > triggerTime) {
				detritusGO.GetComponent<DetritusManager> ().spawnDetritus ();
				timer = 0.0f;
				StartCoroutine (FadeReticle (0.0f, 0.7f));
				turnTextOff ();
				prototype.archived = false;
				prototype.movingUp = true;
			}

		} else if (text != null) {
			prototype.focusSound = false;
			reticle.fillAmount = 0;
			timer = 0.0f;
			turnTextOff ();

		} else if (prototype != null) {
			prototype.focusSound = false;
			prototype = null;
		}
	}
		
	private void turnTextOn(){
		text.isOn = true;
	}

	private void turnTextOff(){
		text.isOn = false;
		text = null;
	}

	IEnumerator FadeReticle(float aValue, float aTime)
	{
		float alpha = reticle.material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = reticle.material.color;
			newColor.a = Mathf.Lerp (alpha, aValue, t);
			reticle.material.color = newColor;
			yield return null;
		}
		retcolor = reticle.material.color;
		retcolor.a = 1.0f;
		reticle.material.color = retcolor;
		reticle.fillAmount = 0.0f;
	}

}