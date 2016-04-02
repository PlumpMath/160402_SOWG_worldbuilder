using UnityEngine;
using System.Collections;
using System;

public class TimeStart : MonoBehaviour {

	void Update(){
		DateTime time = DateTime.Now;
		String hour = time.Hour.ToString ().PadLeft (2, '0');
		String minute = time.Minute.ToString ().PadLeft (2, '0');
		string second = time.Second.ToString ().PadLeft (2, '0');

		//GUILayout.Label (hour + ":" + minute + ":" + second);
		Debug.Log (hour + ":" + minute + ":" + second);
	}
}
