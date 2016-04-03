using UnityEngine;
using System.Collections;
using System.Linq;

public class GazeSoundFunctions{

	public static string[] hitPaths = 
	new string[]{
		"hit1", "hit2", "hit3", "hit4",
		"hit5", "hit6", "hit7", "hit8",
		"hit9", "hit10", "hit11"};

	public static void playAtPoint(string s, Vector3 pt){
		AudioClip a = (AudioClip) Resources.Load("sounds/" + s) as AudioClip;
		AudioSource.PlayClipAtPoint(a, pt, 1F);
	}

	public static void playBackground(Vector3 pt){
		playAtPoint("bg1", pt);
	}

	public static void playHit(Vector3 hitPosition){
		string hit = hitPaths[Random.Range(0, hitPaths.Length)];
		playAtPoint(hit, hitPosition);
	}

}