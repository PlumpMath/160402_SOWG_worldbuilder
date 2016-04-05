using UnityEngine;
using System.Collections;
using System.Linq;


public static class GazeMeshModellerFunctions{

	public static string[] modelPaths = 
		new string[]{
			"1929-Cunningham", "1931_fnrs", "1956_Thermoheliodon",
			"1972_Ecological House", "2000_Eden"};

	public static readonly float PI = Mathf.PI;
	public static readonly float TWO_PI = Mathf.PI * 2;

	public static float smootherStep (float x){
		float a = Mathf.Clamp01(x);
		return 6 * Mathf.Pow(a, 5) - 15 * Mathf.Pow(a, 4) + 10 * Mathf.Pow(a, 3);
	}

	public static Mesh ImportMesh(string s){
		return Resources.Load(s, typeof(Mesh)) as Mesh;
	}

	public static Mesh ImportModelMesh(string s){
		return ImportMesh("models/" + s);
	}

	public static GameObject PopulateWithMesh(GameObject obj, Mesh m){
		// add mesh to meshfilter
		MeshFilter mf = obj.GetComponent<MeshFilter>();
		if (mf == null){
			mf = obj.AddComponent<MeshFilter>();
		}
		mf.sharedMesh = m;
		// add collider
		MeshCollider mc = obj.GetComponent<MeshCollider>();
		if (mc == null){
			mc = obj.AddComponent<MeshCollider>();
		}
		mc.sharedMesh = m;
		// add MeshRenderer
		MeshRenderer mr = obj.GetComponent<MeshRenderer>();
		if (mr == null){
			mr = obj.AddComponent<MeshRenderer>();
		}
		// don't know if this will work as well as one would hope:
		mr.material = new Material(Shader.Find("Standard"));

		return obj;
	}

	public static GameObject MeshObject(string s){
		GameObject obj = new GameObject(s);
		PopulateWithMesh(obj, ImportModelMesh(s));
		return obj;
	}

	// thoroughly hacky named-based lookup:
	public static GameObject ResetMesh(GameObject obj){
		PopulateWithMesh(obj, ImportModelMesh(obj.name));
		return obj;
	}

	// transform mesh of object struck by RaycastHit hit
	public static Vector3[] ModelStrikeVs (RaycastHit hit, float strength, float radius){
		Collider c = hit.collider;
		GameObject targ = c.gameObject;
		Transform targTr = targ.GetComponent<Transform>();
		// Move hit point into space of impacted target:
		Vector3 hp = targTr.InverseTransformPoint(hit.point);
		Mesh m = targ.GetComponent<MeshFilter>().mesh;
		// scale radius, grossly:
		float radius2 = (targTr.InverseTransformPoint(new Vector3(radius, 0, 0))).magnitude;
		Vector3[] vs = m.vertices;
		// Loop over all vertices, moving towards or away 
		// from hit point (hp) according to their proximity:
		for(int i = 0; i < vs.Length; i++){
			Vector3 p = vs[i];
			Vector3 diff = p - hp;
			float mag = diff.magnitude;
			if (mag < radius2){
				float delta = strength * (1 - smootherStep(mag / radius2));
				Vector3 deltaV = diff.normalized * delta;
				Vector3 p2 = p + deltaV;
				// Ensure we don't shift through the hit point if strength is negative:
				if (delta < 0 && mag < (p - p2).magnitude){
					vs[i] = hp;
				}else{
					vs[i] = p2;
				}
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
	// better way of doing this thing is to just return a new RaycastHit no matter what.
	// It looks like RaycastHit has a null collider field iff it didn't hit anything, so
	// that stands in nicely for returning false or nil in case of no hit.
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