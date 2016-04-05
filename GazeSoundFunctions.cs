using UnityEngine;
using System.Collections;
using System.Linq;

public class GazeSoundFunctions{

	// Entry points are PlayRandomHitAtHit and PlayBackgroundAtObject.
	// To work properly, this file assumes the following:
	//  - GameObjects that want to host a sound (ie, some part of which a sound should play from)
	//    have a child object named "AudioSource," attached to which is a single AudioSource.
	//  - Daniel Perlin's sound files are reachable under some Resources directory at the following path:
	//       ...Resources/sounds/<the files>

	public static string[] hitPaths = 
		new string[]{
			"hit1", "hit2", "hit3", "hit4",
			"hit5", "hit6", "hit7", "hit8",
			"hit9", "hit10", "hit11"};

	public static string backgroundPath = "bg1";

	public static AudioClip LoadClip(string name){
		return (AudioClip) Resources.Load("sounds/" + name) as AudioClip;
	}

	public static AudioClip RandomHitClip(){
		return LoadClip(hitPaths[Random.Range(0, hitPaths.Length)]);
	}

	public static AudioClip BackgroundClip(){
		return LoadClip(backgroundPath);
	}

	public static GameObject FindAudioSourceObj (GameObject obj){
		return obj.transform.Find("AudioSource").gameObject;
	}

	public static AudioSource AudioSourcePlayClip (AudioSource asource, AudioClip clip,
													 bool loop){
		asource.clip = clip;
		asource.loop = loop;
		asource.spatialBlend = 1F; // might not be the place to do this
		asource.Play();
		return asource;
	}

	public static GameObject PlayClipAtPoint(GameObject obj, Vector3 pt, 
											AudioClip clip, bool loop){
		GameObject audioSourceObj = FindAudioSourceObj(obj);
		audioSourceObj.transform.position = pt;
		AudioSourcePlayClip(
			audioSourceObj.GetComponent<AudioSource>(),
			clip,
			loop);
		return obj;
	}

	public static void PlayClipAtHit (RaycastHit hit, AudioClip clip, bool loop){
		PlayClipAtPoint(
			hit.transform.gameObject,
			hit.point,
			clip,
			loop);
	}

	// top-level API

	public static void PlayRandomHitAtHit(RaycastHit hit){
		PlayClipAtHit(hit, RandomHitClip(), false);
	}

	public static void PlayBackgroundAtObject(GameObject obj){
		PlayClipAtPoint(obj, obj.transform.position, BackgroundClip(), true);
		// hacky:
		FindAudioSourceObj(obj).GetComponent<AudioSource>().spatialBlend = 0F;
	}

}