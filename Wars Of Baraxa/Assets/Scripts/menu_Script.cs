using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
public class menu_Script : MonoBehaviour {

    //Variables
 public bool profile = false;
 public bool trouverPartie = false;
 public bool quitter = false;
 public string nom;

    //GUI
 public GUIStyle test;
 public GUIStyle warOfBaraxa;
 public GUIStyle GUIBox;
 public GUIStyle GUIButton;
 public GUIStyle Background;
 void OnApplicationQuit()
 {
     envoyerMessage("deconnection");
 }
	// Use this for initialization
	void Start () {
        nom = connexionServeur.nom;
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKeyDown(KeyCode.Escape)){
		if(quitter){
			quitter = false;
		}
		else{
			quitter = true;
		}
	}	
	}
    public void OnGUI(){
	GUI.Box(new Rect(0,0,Screen.width,Screen.height),"",Background);
	test.fontSize = Screen.width/26;
	warOfBaraxa.fontSize = Screen.width/10;
	GUI.Label(new Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);

	if(!quitter){
		if(GUI.Button(new Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.40f,Screen.width * 0.3f, Screen.height * 0.05f), "Trouver une partie", test)){
			Application.LoadLevel("Lobby");
		}
		if(GUI.Button(new Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.50f,Screen.width * 0.3f ,Screen.height * 0.05f), "Profile", test)){
			Application.LoadLevel("Profile");
		}
		if(GUI.Button(new Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.60f,Screen.width * 0.3f, Screen.height * 0.05f), "Quitter", test)){
			quitter = true;
		}
	}
	else{
		GUIBox.fontSize = Screen.width/30;
		GUIButton.fontSize = Screen.width/40;
		GUI.Box (new Rect (Screen.width*0.35f,Screen.height * 0.35f,Screen.width * 0.30f,Screen.height * 0.30f), "\nVoulez-vous \n vraiment quitter?", GUIBox);
		
		if(GUI.Button (new Rect ((Screen.width * 0.36f) ,Screen.height * 0.55f,Screen.width * 0.135f,Screen.height * 0.07f),"Confirmer", GUIButton)){
			Application.Quit();
		}
		
		if(GUI.Button (new Rect ((Screen.width * 0.43f) + (Screen.width * 0.15f/2),Screen.height * 0.55f,Screen.width * 0.135f,Screen.height * 0.07f), "Annuler", GUIButton)){
			quitter = false;
		}
	}
}
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
    }
    private string lire()
    {
        string message = null;
        do
        {
            message = recevoirResultat();
        } while (message == null);
        return message;
    }
    private string recevoirResultat()
    {
        byte[] buff = new byte[connexionServeur.sck.SendBufferSize];
        int bytesRead = connexionServeur.sck.Receive(buff);
        byte[] formatted = new byte[bytesRead];
        for (int i = 0; i < bytesRead; i++)
        {
            formatted[i] = buff[i];
        }
        string strData = Encoding.ASCII.GetString(formatted);
        return strData;
    }
}