using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class rayGazeTrigger : MonoBehaviour {

	public Image reticle;
	private Color retcolor;
	private RaycastHit hit;
	private textDisplay text;
	private PrototypeMover moving;
	private float timer;
	private float triggerTime = 3.0f;
	private bool triggered = false;

	void Start(){
		reticle.fillAmount = 0.0f;
		retcolor = reticle.material.color;
		retcolor.a = 1.0f;
		triggered = false;
	}

	void Update () {
			gazeTrigger2 ();
	}


	private void gazeTrigger2(){
		Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 500.0f) && hit.collider.gameObject.layer == LayerMask.NameToLayer ("Text Container")) {

			moving = hit.collider.gameObject.GetComponentInParent<PrototypeMover> ();

			if (moving.isMoving == false) {
				text = hit.collider.gameObject.GetComponentInParent<textDisplay> ();
				turnTextOn ();
			}

			if (triggered == false) {
				timer += Time.deltaTime;
				reticle.fillAmount += 1.0f / triggerTime * Time.deltaTime;
			}

			if (timer >= triggerTime) {
				triggered = true;
				timer = 0.0f;
				StartCoroutine (FadeReticle (0.0f, 0.7f));
				turnTextOff ();

				//retcolor.a = Mathf.Lerp (retcolor.a, 0.0f, Time.deltaTime * 5.0f);
				//Debug.Log (retcolor.a);
				//reticle.material.color.a = retcolor;
				moving.isMoving = true;
			}	
			
		} else if (text != null){
			triggered = false;
			reticle.fillAmount = 0;
			timer = 0.0f;
			turnTextOff ();
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