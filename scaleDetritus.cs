using UnityEngine;
using System.Collections;

public class scaleDetritus : MonoBehaviour {

	private Transform myTransform;

	private float globalScale = 0.3f;
	private float originalScale;

	void Awake(){
		myTransform = GetComponent<Transform> ();
		AcquireScale ();
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

}
