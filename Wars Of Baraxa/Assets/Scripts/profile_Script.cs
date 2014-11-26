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
    public GUIStyle GUIBox;
    public GUIStyle textArea;
    public string victoire;
    public string defaite;
    public string aliasRechercher = "";
    public string alias;
    public string nomComplet;
    public string message;
    bool showBox = false;
    bool messageAfficher = false;
    bool hitReturn = false;
    float temps;
	// Use this for initialization
	void Start () {
	    
	}
        void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }
    public void Awake()
    {
        connexionServeur.sck.ReceiveTimeout = 500;
        string message = "";
        bool recu = false;
        while (!recu)
        {
            try
            {
                envoyerMessage("afficher profil," + connexionServeur.nom);
                message = lire();
                recu = true;
            }
            catch (SocketException)
            {
                recu = false;
            }
        }
        connexionServeur.sck.ReceiveTimeout = 0;
        string[] data = message.Split(new char[] { ',' });
        if (data.Length >= 4)
        {
            victoire = data[0];
            defaite = data[1];
            alias = data[2];
            nomComplet = data[3];
        }
    }
	// Update is called once per frame
	void Update () {
        if (messageAfficher)
        {
            if (temps == 0)
                temps = Time.time;
            if (!showBox && Time.time - temps >= 3)
            {
                temps = 0;
                message = "";
                messageAfficher = false;     
            }
        }
	}
    public void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        text.fontSize = Screen.width / 35;
        if (!showBox)
        {
            //GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.6f / 2), Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
            //account name         nom joueur
            GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.47f, Screen.width * 0.005f, Screen.height * 0.05f), "alias :", text);
            GUI.TextField(new Rect(Screen.width * 0.31f, Screen.height * 0.47f, Screen.width * 0.03f, Screen.height * 0.05f), alias, text);

            //Défaites
            GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.47f, Screen.width * 0.03f, Screen.height * 0.05f), "nom :", text);
            GUI.TextField(new Rect(Screen.width * 0.76f, Screen.height * 0.47f, Screen.width * 0.03f, Screen.height * 0.05f), nomComplet, text);
            //Victoires
            GUI.Label(new Rect(Screen.width * 0.23f, Screen.height * 0.60f, Screen.width * 0.005f, Screen.height * 0.05f), "Victoires :", text);
            GUI.TextField(new Rect(Screen.width * 0.315f, Screen.height * 0.60f, Screen.width * 0.03f, Screen.height * 0.05f), victoire, text);

            //Défaites
            GUI.Label(new Rect(Screen.width * 0.593f, Screen.height * 0.60f, Screen.width * 0.10f, Screen.height * 0.05f), "Defaites :", text);
            GUI.TextField(new Rect(Screen.width * 0.705f, Screen.height * 0.60f, Screen.width * 0.03f, Screen.height * 0.05f), defaite, text);

            //Rechercher un joueur
            if (GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Rechercher", GUIButton))
            {
                message = "";
                showBox = true;
            }
            //Retour
            if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour", GUIButton))
            {
                Application.LoadLevel("Menu");
            }
            //message d'erreur
            GUI.Label(new Rect(Screen.width * 0.45f, Screen.height * 0.80f, Screen.width * 0.10f, Screen.height * 0.05f), message, text);
        }
        else 
        {
            if (Event.current.keyCode == KeyCode.Return)
                hitReturn = true;

            if (hitReturn)
            {
                envoyerProfile(aliasRechercher);
                showBox = false;
                aliasRechercher = "";
                hitReturn = false;
            }
            GUIButton.fontSize = Screen.width / 50;
            GUI.Box(new Rect(Screen.width * 0.25f, Screen.height * 0.5f, Screen.width * 0.40f, Screen.height * 0.22f),"", GUIBox);
            GUI.Label(new Rect(Screen.width*0.18f, Screen.height * 0.55f, Screen.width * 0.25f, Screen.height * 0.05f),"Alias:", text);
            aliasRechercher = GUI.TextField(new Rect((Screen.width / 2) - (Screen.width * 0.25f / 2), Screen.height * 0.55f, Screen.width * 0.25f, Screen.height * 0.05f), aliasRechercher, 25, textArea);
            if (GUI.Button(new Rect((Screen.width * 0.3f), Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.07f), "rechercher", GUIButton))
            {
                envoyerProfile(aliasRechercher);
                showBox = false;
                aliasRechercher = "";
            }
            if (GUI.Button(new Rect((Screen.width * 0.48f), Screen.height * 0.63f, Screen.width * 0.15f, Screen.height * 0.07f), "retour", GUIButton))
            {
                showBox = false;
                aliasRechercher = "";
            }
        }
    }
    private void envoyerProfile(string al)
    {
        envoyerMessage("afficher profil Joueur" + "," + al);
        string rep=recevoirResultat();
        string[] data = rep.Split(new char[] { ',' });
        if (data.Length == 4)
        {
            victoire = data[0];
            defaite = data[1];
            alias = data[2];
            nomComplet = data[3];
        }
        else
        {
            message = "l'alias est invalide.";
            messageAfficher = true;
            envoyerProfile(connexionServeur.nom);
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