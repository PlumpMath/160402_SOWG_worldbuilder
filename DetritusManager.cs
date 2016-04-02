using UnityEngine;
using System.Collections;

public class DetritusManager : MonoBehaviour {
	
	public Transform[] Detritus;
	public Transform[] spawnPoints;
	public float launchForce = 2.0f;

	private int index = 0;
	private int spawnIndex = 0;

	public void spawnDetritus () {
		if (index < Detritus.Length) {
			Transform det = Detritus [index];
			spawnIndex = Random.Range (0, spawnPoints.Length);
			det.localPosition = spawnPoints [spawnIndex].transform.position;
			Rigidbody detRb = det.gameObject.GetComponent<Rigidbody> ();
			det.gameObject.SetActive (true);
			det.gameObject.transform.rotation = Random.rotation;
			Vector3 launchDir = new Vector3 (Random.Range (0.0f, 0.5f), 1.0f, Random.Range (0.0f, 0.5f));
			detRb.AddForce (launchDir * launchForce, ForceMode.Impulse);

			index++;
		}else
			index = 0;
	}
		
	public void iterateSpawn () {
		int size = spawnPoints.Length-1; 
		if (spawnIndex < size) { 
			spawnIndex++;
		} else {
			spawnIndex = 0;
		}
	}
}

