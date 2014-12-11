using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
/// <summary>
/// Menu du jeu
/// fait en unity C#
/// </summary>
public class menu_Script : MonoBehaviour
{

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
    public GUIStyle Logo;
    //au debut
    void Awake()
    {
        //on détruit toute les objets non utiisé
        Resources.UnloadUnusedAssets();
    }
    //quand on quite le jeu
    void OnApplicationQuit()
    {
        //on dit au serveur qu'on se déconnect
        envoyerMessage("deconnection");
    }
    // Use this for initialization
    void Start()
    {
        nom = connexionServeur.nom;
    }

    // Update is called once per frame
    void Update()
    {
        //si on click sur Escape il y a un menu pour quitter
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitter)
            {
                quitter = false;
            }
            else
            {
                quitter = true;
            }
        }
    }
    public void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        GUI.Box(new Rect(Screen.width * 0.2f, 0, Screen.width - 350, Screen.height - 300), "", Logo);
        test.fontSize = Screen.width / 26;
        //warOfBaraxa.fontSize = Screen.width/10;
        //GUI.Label(new Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
        //si c'est le menu de basse
        if (!quitter)
        {
            //amene au menu pour choisir un deck
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width * 0.3f / 2), Screen.height * 0.47f, Screen.width * 0.3f, Screen.height * 0.05f), "Trouver une partie", test))
            {
                Application.LoadLevel("Lobby");
            }
            //amene au profile
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width * 0.3f / 2), Screen.height * 0.57f, Screen.width * 0.3f, Screen.height * 0.05f), "Profile", test))
            {
                Application.LoadLevel("Profile");
            }
            //decconect le joeuur et ramene a la page de log in
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width * 0.3f / 2), Screen.height * 0.67f, Screen.width * 0.3f, Screen.height * 0.05f), "Deconnection", test))
            {

                envoyerMessage("RetourMenu");
                Application.LoadLevel("Connexion");
            }
            //quitte le jeu(ouvre une box de confirmation)
            if (GUI.Button(new Rect((Screen.width / 2) - (Screen.width * 0.3f / 2), Screen.height * 0.77f, Screen.width * 0.3f, Screen.height * 0.05f), "Quitter", test))
            {
                quitter = true;
            }
        }
            //il veut quiter
        else
        {
            //on crée une box avec les 2 boutons
            //on change les grosseurs des lettres
            GUIBox.fontSize = Screen.width / 30;
            GUIButton.fontSize = Screen.width / 40;

            GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), "Voulez-vous \n vraiment quitter?", GUIBox);

            //bouton pour quitter 
            if (GUI.Button(new Rect((Screen.width * 0.36f), Screen.height * 0.55f, Screen.width * 0.135f, Screen.height * 0.07f), "Confirmer", GUIButton))
            {
                Application.Quit();
            }
            //bouton pour annuler
            if (GUI.Button(new Rect((Screen.width * 0.43f) + (Screen.width * 0.15f / 2), Screen.height * 0.55f, Screen.width * 0.135f, Screen.height * 0.07f), "Annuler", GUIButton))
            {
                quitter = false;
            }
        }
    }
    //-------------communication avec le serveur-------------------////
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
        try
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
        catch (Exception)
        { }
        return null;
    }
    ////////////--------------------fin comm-----------------///////////
}