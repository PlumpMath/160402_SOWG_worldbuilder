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

	public static GameObject AddAudioSourceObject(GameObject obj){
		GameObject g = new GameObject("AudioSource");
		g.transform.parent = obj.transform;
		g.AddComponent<AudioSource>();
		return g;
	}

	public static GameObject NextAudioSourceObject(GameObject obj){
		GameObject[] audioSourceObjs = obj.transform.Cast<Transform>(
			).Select((Transform x) => x.gameObject
			).Where(x => x.name == "AudioSource"
			).ToArray();
		GameObject asource = audioSourceObjs.Where(x => !(x.GetComponent<AudioSource>().isPlaying)
			).FirstOrDefault();
		if(asource != null){
			return asource;
		}else if (audioSourceObjs.Length <= 5){
			return AddAudioSourceObject(obj);
		}else{
			return null;
		}
	}

	public static AudioSource AudioSourcePlayClip (AudioSource asource, AudioClip clip,
													 bool loop, bool threeD){
		asource.clip = clip;
		asource.loop = loop;
		if(threeD){
			asource.spatialBlend = 1F; // might not be the place to do this
		}else{
			asource.spatialBlend = 0F;
		}
		asource.Play();
		return asource;
	}

	public static GameObject PlayClipAtPoint(GameObject obj, Vector3 pt, 
											AudioClip clip, bool loop, bool threeD){
		GameObject audioSourceObj = NextAudioSourceObject(obj);
		if (audioSourceObj != null){
		audioSourceObj.transform.position = pt;
		AudioSourcePlayClip(
			audioSourceObj.GetComponent<AudioSource>(),
			clip,
			loop,
			threeD);
		}
		return obj;
	}

	public static void PlayClipAtHit (RaycastHit hit, AudioClip clip, bool loop, bool threeD){
		PlayClipAtPoint(
			hit.transform.gameObject,
			hit.point,
			clip,
			loop,
			threeD);
	}

	// top-level API

	public static void PlayRandomHitAtHit(RaycastHit hit){
		PlayClipAtHit(hit, RandomHitClip(), false, true);
	}

	public static void PlayBackgroundAtObject(GameObject obj){
		PlayClipAtPoint(obj, obj.transform.position, BackgroundClip(), true, false);
	}

}