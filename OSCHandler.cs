//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine	  
//
//	  Copyright (c) 2012 Jorge Garcia Martin
//
// 	  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// 	  documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// 	  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// 	  and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// 	  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// 	  of the Software.
//
// 	  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// 	  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// 	  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// 	  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// 	  IN THE SOFTWARE.
//
//	  Inspired by http://www.unifycommunity.com/wiki/index.php?title=AManagerClass

using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityOSC;

/// <summary>
/// Models a log of a server composed by an OSCServer, a List of OSCPacket and a List of
/// strings that represent the current messages in the log.
/// </summary>
public struct ServerLog
{
	public OSCServer server;
	public List<OSCPacket> packets;
	public List<string> log;
}

/// <summary>
/// Models a log of a client composed by an OSCClient, a List of OSCMessage and a List of
/// strings that represent the current messages in the log.
/// </summary>
public struct ClientLog
{
	public OSCClient client;
	public List<OSCMessage> messages;
	public List<string> log;
}

/// <summary>
/// Handles all the OSC servers and clients of the current Unity game/application.
/// Tracks incoming and outgoing messages.
/// </summary>
public class OSCHandler : MonoBehaviour
{
	#region Singleton Constructors
	static OSCHandler()
	{
	}

	OSCHandler()
	{
	}
	
	public static OSCHandler Instance 
	{
	    get 
		{
	        if (_instance == null) 
			{
				_instance = new GameObject ("OSCHandler").AddComponent<OSCHandler>();
	        }
	       
	        return _instance;
	    }
	}
	#endregion
	
	#region Member Variables
	private static OSCHandler _instance = null;
	private Dictionary<string, ClientLog> _clients = new Dictionary<string, ClientLog>();
	private Dictionary<string, ServerLog> _servers = new Dictionary<string, ServerLog>();
	
	private const int _loglength = 25;


	public static GameObject theirCameraObject = null;
	public static float myStrength = 0.0f; 
	public static float myRadius = 0.0f;
	public static float theirStrength = 0.0f; 
	public static float theirRadius = 0.0f;
	public static string candidateName;


	#endregion

	/*************************************
	 * CUSTOM CODE HERE FROM DAN TAEYOUNG
	 ****************************************/
	/// <summary>
	/// Initializes the OSC Handler.
	/// Here you can create the OSC servers and clientes.
	/// </summary>
	public void Init()
	{
        //Initialize OSC clients (transmitters)
        //Example:		
        //CreateClient("SuperCollider", IPAddress.Parse("127.0.0.1"), 5555);
		// static IPs, ish
		var computerListenHost = "192.168.253.200";
		var computerListenPort = 5555;
		var phoneAListenHost = "192.168.253.201";
		var phoneAListenPort = 5556;
		var phoneBListenHost = "192.168.253.202";
		var phoneBListenPort = 5557;




		// EDIT THIS
		// ALSO we have to set index in InitializeCamera.cs

	
		//var IAm = "phoneB"; var TheyAre = "computer"; 
		var IAm = "computer"; var TheyAre = "phoneB";

		//var IAm = "computer"; var TheyAre = "phoneA";
		//var IAm = "phoneA"; var TheyAre = "phoneB";
		//var IAm = "phoneB"; var TheyAre = "phoneA";


		//candidateName = "targetCandidate";
		candidateName = "thermoCandidate";

		// THE BELOW STUFF JUST MAKES OUR LIVES EASIER. DON'T EDIT.
		var sendToHost = ""; var sendToPort = 0; var listenFromPort = 0;
		float growStrength = 40.2f; 
		float growRadius = 10.2f;
		float shrinkStrength = -7.2f;
		float shrinkRadius = 10.2f;

		if (IAm == "computer") {	listenFromPort = computerListenPort; myRadius = growRadius; myStrength = growStrength;	}
		if (IAm == "phoneA") {	listenFromPort = phoneAListenPort; myRadius = shrinkRadius; myStrength = shrinkStrength;	}
		if (IAm == "phoneB") {	listenFromPort = phoneBListenPort;	myRadius = growRadius; myStrength = growStrength;	}
		if (TheyAre == "computer") {	sendToHost = computerListenHost;	sendToPort = computerListenPort; theirRadius = shrinkRadius; theirStrength = shrinkStrength;	}
		if (TheyAre == "phoneA") {		sendToHost = phoneAListenHost;		sendToPort = phoneAListenPort; theirRadius = shrinkRadius; theirStrength = shrinkStrength;	}
		if (TheyAre == "phoneB") {		sendToHost = phoneBListenHost;		sendToPort = phoneBListenPort; theirRadius = growRadius; theirStrength = growStrength;	}




		// client = sending to 
		CreateClient("testServer", IPAddress.Parse(sendToHost), sendToPort);

		// server = Listening From
		CreateServer("thisListener", listenFromPort);
	

		print ("listening from port: " + listenFromPort);
		print ("Sending to: " + sendToHost + " on port " + sendToPort);
	}

	void Start() {
		OSCHandler.Instance.Init();

		InvokeRepeating("sendCameraMessage", 0, 0.1F);
		InvokeRepeating("receiveCameraMessage", 0, 0.1F);
		InvokeRepeating ("handleGazesHit", 1, 0.25F);


	}

	void Update() {
		//receiveCameraMessage ();
	}


	void sendCameraMessage() {
		DateTime time = DateTime.Now;

		//float orientationAngle = 30.0f;

		string cameraPosition = Camera.main.gameObject.transform.position.ToString("F5");
		string cameraEuler = Camera.main.gameObject.transform.eulerAngles.ToString("F5");

		string OSCAddress = "/SWG/camera/" + InitializeCamera.index + "/positionorientation";
		string OSCMessage = cameraPosition + "/" + cameraEuler;

		print ("YOOO, sending message ::: " + OSCMessage + " ::: to" + OSCAddress);

		OSCHandler.Instance.SendMessageToClient("testServer", OSCAddress, OSCMessage);


	}

	public Vector3 getVector3(string rString){
		string[] temp = rString.Substring(1,rString.Length-2).Split(',');
		float x = float.Parse(temp[0]);
		float y = float.Parse(temp[1]);
		float z = float.Parse(temp[2]);
		Vector3 rValue = new Vector3(x,y,z);
		return rValue;
	}


	void receiveCameraMessage() {
		OSCHandler.Instance.UpdateLogs();

		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;

		try {
	

			string oscMessage = servers["thisListener"].server.LastReceivedPacket.Data[0].ToString();
			string oscAddress = servers["thisListener"].server.LastReceivedPacket.Address;

			if(oscAddress == "/init") {
				// if we receive an initialization message, then reload level
				//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				//print("HEY! WE GOTTA INITIALIZE!!"); 
				//return;
			}

			print(">>>>");

			print (oscMessage);
			print (oscAddress);
		
		
			Regex regex = new Regex(@"/camera/(?<cameraID>\w+?)/positionorientation");
			var match = regex.Match(oscAddress);
			var cameraID = match.Groups["cameraID"].Value;
		

			GameObject cameraToSet = GameObject.Find ("CAMERA" + cameraID);
			string[] positionorientation = oscMessage.Split('/');

			print(getVector3(positionorientation[0]));
			print(positionorientation[1]);

			cameraToSet.gameObject.transform.position = getVector3(positionorientation[0]);
			cameraToSet.gameObject.transform.eulerAngles = getVector3(positionorientation[1]);


			// we store the Cameraindex in a static Var so that handleGazesHit() can handle it
			print("camera>>>");
			print(cameraID);
			theirCameraObject = cameraToSet;


		

		} catch(Exception e) {
			print ("Error happened: " + e);
		}


	}

	void handleGazesHit() {

		GameObject candidate = GameObject.Find (candidateName);


		print("handleGazesHit()");

		//handle our gaze
		object ourRaycastHit = GazeMeshModellerFunctions.GazeUpdate (Camera.main.gameObject, candidate, myStrength, myRadius);

		if (ourRaycastHit != null) {
			try {
			RaycastHit thisr = (RaycastHit)ourRaycastHit;
			print ("gazehit: WE hit something!");
			print (thisr.ToString ());
			} catch {
			}
		}

		//handle their gaze
		if (theirCameraObject) {

			object theirRaycastHit = GazeMeshModellerFunctions.GazeUpdate (theirCameraObject, candidate, theirStrength, theirRadius);

			if(theirRaycastHit != null) {


				print ("gazehit: THEY hit something!");
			}
		} else {
			print ("theirCameraObject doesn't exist!!!!");
		}



	}

	/*************************************
	 * CUSTOM CODE ENDING
	 ****************************************/

	#region Properties
	public Dictionary<string, ClientLog> Clients
	{
		get
		{
			return _clients;
		}
	}
	
	public Dictionary<string, ServerLog> Servers
	{
		get
		{
			return _servers;
		}
	}
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Ensure that the instance is destroyed when the game is stopped in the Unity editor
	/// Close all the OSC clients and servers
	/// </summary>
	void OnApplicationQuit() 
	{
		foreach(KeyValuePair<string,ClientLog> pair in _clients)
		{
			pair.Value.client.Close();
		}
		
		foreach(KeyValuePair<string,ServerLog> pair in _servers)
		{
			pair.Value.server.Close();
		}
			
		_instance = null;
	}
	
	/// <summary>
	/// Creates an OSC Client (sends OSC messages) given an outgoing port and address.
	/// </summary>
	/// <param name="clientId">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="destination">
	/// A <see cref="IPAddress"/>
	/// </param>
	/// <param name="port">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void CreateClient(string clientId, IPAddress destination, int port)
	{
		ClientLog clientitem = new ClientLog();
		clientitem.client = new OSCClient(destination, port);
		clientitem.log = new List<string>();
		clientitem.messages = new List<OSCMessage>();
		
		_clients.Add(clientId, clientitem);
		
		// Send test message
		string testaddress = "/init";
		OSCMessage message = new OSCMessage(testaddress, destination.ToString());
		message.Append(port); message.Append("OK");
		
		_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
		                                         FormatMilliseconds(DateTime.Now.Millisecond), " : ",
		                                         testaddress," ", DataToString(message.Data)));
		_clients[clientId].messages.Add(message);
		
		_clients[clientId].client.Send(message);
	}
	
	/// <summary>
	/// Creates an OSC Server (listens to upcoming OSC messages) given an incoming port.
	/// </summary>
	/// <param name="serverId">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="port">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void CreateServer(string serverId, int port)
	{
        OSCServer server = new OSCServer(port);
        server.PacketReceivedEvent += OnPacketReceived;

        ServerLog serveritem = new ServerLog();
        serveritem.server = server;
		serveritem.log = new List<string>();
		serveritem.packets = new List<OSCPacket>();
		
		_servers.Add(serverId, serveritem);
	}

    void OnPacketReceived(OSCServer server, OSCPacket packet)
    {
    }
	
	/// <summary>
	/// Sends an OSC message to a specified client, given its clientId (defined at the OSC client construction),
	/// OSC address and a single value. Also updates the client log.
	/// </summary>
	/// <param name="clientId">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="address">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="value">
	/// A <see cref="T"/>
	/// </param>
	public void SendMessageToClient<T>(string clientId, string address, T value)
	{
		List<object> temp = new List<object>();
		temp.Add(value);
		
		SendMessageToClient(clientId, address, temp);
	}
	
	/// <summary>
	/// Sends an OSC message to a specified client, given its clientId (defined at the OSC client construction),
	/// OSC address and a list of values. Also updates the client log.
	/// </summary>
	/// <param name="clientId">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="address">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="values">
	/// A <see cref="List<T>"/>
	/// </param>
	public void SendMessageToClient<T>(string clientId, string address, List<T> values)
	{	
		if(_clients.ContainsKey(clientId))
		{
			OSCMessage message = new OSCMessage(address);
		
			foreach(T msgvalue in values)
			{
				message.Append(msgvalue);
			}
			
			if(_clients[clientId].log.Count < _loglength)
			{
				_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
				                                         FormatMilliseconds(DateTime.Now.Millisecond),
				                                         " : ", address, " ", DataToString(message.Data)));
				_clients[clientId].messages.Add(message);
			}
			else
			{
				_clients[clientId].log.RemoveAt(0);
				_clients[clientId].messages.RemoveAt(0);
				
				_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
				                                         FormatMilliseconds(DateTime.Now.Millisecond),
				                                         " : ", address, " ", DataToString(message.Data)));
				_clients[clientId].messages.Add(message);
			}
			
			_clients[clientId].client.Send(message);
		}
		else
		{
			Debug.LogError(string.Format("Can't send OSC messages to {0}. Client doesn't exist.", clientId));
		}
	}
	
	/// <summary>
	/// Updates clients and servers logs.
	/// </summary>
	public void UpdateLogs()
	{
		foreach(KeyValuePair<string,ServerLog> pair in _servers)
		{
			if(_servers[pair.Key].server.LastReceivedPacket != null)
			{
				//Initialization for the first packet received
				if(_servers[pair.Key].log.Count == 0)
				{	
					_servers[pair.Key].packets.Add(_servers[pair.Key].server.LastReceivedPacket);
						
					_servers[pair.Key].log.Add(String.Concat(DateTime.UtcNow.ToString(), ".",
					                                         FormatMilliseconds(DateTime.Now.Millisecond)," : ",
					                                         _servers[pair.Key].server.LastReceivedPacket.Address," ",
					                                         DataToString(_servers[pair.Key].server.LastReceivedPacket.Data)));
					break;
				}
						
				if(_servers[pair.Key].server.LastReceivedPacket.TimeStamp
				   != _servers[pair.Key].packets[_servers[pair.Key].packets.Count - 1].TimeStamp)
				{	
					if(_servers[pair.Key].log.Count > _loglength - 1)
					{
						_servers[pair.Key].log.RemoveAt(0);
						_servers[pair.Key].packets.RemoveAt(0);
					}
		
					_servers[pair.Key].packets.Add(_servers[pair.Key].server.LastReceivedPacket);
						
					_servers[pair.Key].log.Add(String.Concat(DateTime.UtcNow.ToString(), ".",
					                                         FormatMilliseconds(DateTime.Now.Millisecond)," : ",
					                                         _servers[pair.Key].server.LastReceivedPacket.Address," ",
					                                         DataToString(_servers[pair.Key].server.LastReceivedPacket.Data)));
				}
			}
		}
	}
	
	/// <summary>
	/// Converts a collection of object values to a concatenated string.
	/// </summary>
	/// <param name="data">
	/// A <see cref="List<System.Object>"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}
	
	/// <summary>
	/// Formats a milliseconds number to a 000 format. E.g. given 50, it outputs 050. Given 5, it outputs 005
	/// </summary>
	/// <param name="milliseconds">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	private string FormatMilliseconds(int milliseconds)
	{	
		if(milliseconds < 100)
		{
			if(milliseconds < 10)
				return String.Concat("00",milliseconds.ToString());
			
			return String.Concat("0",milliseconds.ToString());
		}
		
		return milliseconds.ToString();
	}
			
	#endregion
}	

