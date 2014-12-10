using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections;

public class connexionServeur : MonoBehaviour {
    public static Socket sck;
    public static string nom = "allo";
	// Use this for initialization
	void Start () {
        sck = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Awake()
    {
        // Do not destroy this game object:donc une variables qui va etre présente tout au long du programme
        DontDestroyOnLoad(this);
    }
}
