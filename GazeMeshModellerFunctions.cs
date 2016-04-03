using UnityEngine;
using System.Collections;
using System.Linq;


public static class GazeMeshModellerFunctions{


	public static readonly float PI = Mathf.PI;
	public static readonly float TWO_PI = Mathf.PI * 2;

	public static Vector3[] ModelStrikeVs (RaycastHit hit, float strength, float radius){
		Collider c = hit.collider;
		GameObject targ = c.gameObject;
		Transform targTr = targ.GetComponent<Transform>();
		Vector3 hp = targTr.InverseTransformPoint(hit.point);
		Mesh m = targ.GetComponent<MeshFilter>().mesh;
		Vector3[] vs = m.vertices;
		for(int i = 0; i < vs.Length; i++){
			Vector3 p = vs[i];
			Vector3 diff = hp - p;
			if (diff.magnitude < radius){
				vs[i] = 
				p + 
				(-strength *
					(diff.normalized * 
						Mathf.Min(1, 
							Mathf.Pow(
								diff.magnitude * 2, // arbitrarily * 2
								-0.01F))));
			}
		}
		return vs;
	}

	public static Mesh CloneMesh (Mesh m){
		Mesh m2 = new Mesh();
		m2.vertices = m.vertices;
		m2.triangles = m.triangles;
		m2.uv = m.uv;
		m2.normals = m.normals;
		return m2;
	}

	public static void ModelStrike(RaycastHit hit, float strength, float radius){
		MeshCollider c = (MeshCollider)hit.collider;
		GameObject targ = c.gameObject;
		MeshFilter mf = targ.GetComponent<MeshFilter>();
		Mesh m = CloneMesh(c.sharedMesh);
		m.vertices = ModelStrikeVs(hit, strength, radius);
		c.sharedMesh = m;
		mf.sharedMesh = m;
	}

	// from gazer to gazeCandidate (has to have MeshCollider)
	public static object GazeUpdate(GameObject gazer, GameObject gazeCandidate, 
								float strength, float radius){
		Transform t = gazer.transform;
		Collider coll = gazeCandidate.GetComponent<Collider>();
		Ray ray = new Ray(t.position, t.forward);
		RaycastHit hit;
		if (coll.Raycast(ray, out hit, 1000.0F)){
			ModelStrike(hit, strength, radius);
		}
		return (object) hit;
	}

}