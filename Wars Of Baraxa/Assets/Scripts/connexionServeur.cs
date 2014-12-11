using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections;

public class connexionServeur : MonoBehaviour {
    public static Socket sck;
    public static string nom = "allo";
    public static string IP;
	// Use this for initialization
	void Start () {
        sck = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Awake()
    {
        // Do not destroy this game object:
        DontDestroyOnLoad(this);
    }
}
