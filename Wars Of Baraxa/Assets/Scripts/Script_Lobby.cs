using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class Script_Lobby : MonoBehaviour
{
    //Pour le ONGUI
    public GUIStyle Logo;
    public GUIStyle warOfBaraxa;
    public GUIStyle GUIBox;
    public GUIStyle GUIButton;
    public GUIStyle Background;
    public GUIStyle Text;

    string[] NomDeck;
    string selected;
    bool DeckChoisis;
    //au début
    void Awake()
    {
        DeckChoisis = false;
        int tour = 0;
        connexionServeur.sck.ReceiveTimeout = 500;
        string message = "";
        bool recu = false;
        //on demande au serveur si on peut avoir notre profil apres on recois un message
        //si on ne recois pas le message apres une demie seconde on recommence apres 5 seconde on retourne au menu de départ
        while (!recu && tour <10)
        {
            try
            {
                envoyerMessage("recevoir Deck");
                message = lire();
                recu = true;
            }
            catch (SocketException)
            {
                recu = false;
                ++tour;
            }
        }
        if (tour == 10)
        {
            Application.LoadLevel("menu");
        }
        //au reset le timeout
        connexionServeur.sck.ReceiveTimeout = 0;
        NomDeck = message.Split(new char[] { ',' });
    }
    //a la fermeture on envoie un message au serveur pour lui dire qu'on quitte
    void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }
    //initialisation
    void Start()
    {
        selected = "";
    }

    // Update is called once per frame
    void Update()
    {

    }
    //ONGUI est appelé a chaque frame et s'occupe de l'interface graphique
    void OnGUI()
    {
        //image de background
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        //change l'écriture
        warOfBaraxa.fontSize = Screen.width / 10;
        GUIBox.fontSize = Screen.width / 30;
        GUIButton.fontSize = Screen.width / 45;

       //montre la liste de deck
        GUI.Box(new Rect((Screen.width / 2) - (Screen.width * 0.4f), Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.6f), "Liste des decks", GUIBox);
        int y = 0;
        //met un bouton pour chaque deck quand qu'on click on change le deck choisis
        for (int i = 0; i < NomDeck.Length; ++i)
        {
            if (GUI.Button(new Rect((Screen.width * 0.15f) + (Screen.width * (i - y * 5) * 0.15f), (Screen.height * 0.4f) + (Screen.height * y * 0.2f), Screen.width * 0.08f, Screen.height * 0.08f), NomDeck[i],GUIButton))
            {
                selected = NomDeck[i];
                DeckChoisis = true;
            }
            if (i + 1 == (y + 1) * 5)
                y++;
        }
        //montre quel deck est choisis
        GUI.Label(new Rect((Screen.width * 0.55f), (Screen.height * 0.85f), Screen.width * 0.4f, Screen.height * 0.05f), "Deck choissis: " + selected, Text);
        //si il y a un deck choisis on peut rejoindre et annuler le choix de deck
        if (DeckChoisis)
        {
            if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.91f, Screen.width * 0.15f, Screen.height * 0.07f), "Rejoindre", GUIButton))
            {
                envoyerMessage("trouver partie,"+ selected);
                Application.LoadLevel("Loading");
            }
            if (GUI.Button(new Rect((Screen.width * 0.75f), (Screen.height * 0.8f), Screen.width * 0.1f, Screen.height * 0.07f), "annuler",GUIButton))
            {
                selected = "";
                DeckChoisis = false;
            }
        }
            //sinon les boutons sont disabled
        else
        {
            //en unity il faut dire que enabled est false et endesoous mettre les choses quon ne veut pas enabled
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.91f, Screen.width * 0.15f, Screen.height * 0.07f), "Rejoindre", GUIButton);
            GUI.Button(new Rect((Screen.width * 0.75f), (Screen.height * 0.8f), Screen.width * 0.1f, Screen.height * 0.07f), "annuler", GUIButton);
            //on remet enabled pour les futur gestionaires 
            GUI.enabled = true;
        }
        //si on click retour on revient au menu
        if (GUI.Button(new Rect(Screen.width * 0.65f, Screen.height * 0.91f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour", GUIButton))
        {
            Application.LoadLevel("Menu");
        }
    }
    //-------------communication serveur----------/////////////////
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
    //-------------Fin communication serveur----------/////////////////
}

