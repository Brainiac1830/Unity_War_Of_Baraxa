using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class profile_Script : MonoBehaviour {
    public GUIStyle Logo;
    public GUIStyle warOfBaraxa;
    public GUIStyle text;
    public GUIStyle GUIButton;
    public GUIStyle Background;
    public string victoire;
    public string defaite;
	// Use this for initialization
	void Start () {
	    
	}
    public void Awake()
    {
        envoyerMessage("afficher profil,"+ connexionServeur.nom);
        string message = lire();
        string[] data = message.Split(new char[] { ',' });
        victoire = data[0];
        defaite = data[1];
    }	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        warOfBaraxa.fontSize = Screen.width / 10;
        text.fontSize = Screen.width / 25;
        GUIButton.fontSize = Screen.width / 38;
        //GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.6f / 2), Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
        //Victoires
        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.47f, Screen.width * 0.10f, Screen.height * 0.05f), "Victoires :", text);
        GUI.TextField(new Rect(Screen.width * 0.35f, Screen.height * 0.47f, Screen.width * 0.03f, Screen.height * 0.05f), victoire, text);

        //Défaites
        GUI.Label(new Rect(Screen.width * 0.6f, Screen.height * 0.47f, Screen.width * 0.10f, Screen.height * 0.05f), "Defaites :", text);
        GUI.TextField(new Rect(Screen.width * 0.75f, Screen.height * 0.47f, Screen.width * 0.03f, Screen.height * 0.05f), defaite, text);

        //Rechercher un joueur
        GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Rechercher", GUIButton);
        //Retour
        if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour", GUIButton))
        {
            Application.LoadLevel("Menu");
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
        message = recevoirResultat();
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