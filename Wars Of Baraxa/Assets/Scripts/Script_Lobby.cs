using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class Script_Lobby : MonoBehaviour
{
    public GUIStyle Logo;
    public GUIStyle warOfBaraxa;
    public GUIStyle GUIBox;
    public GUIStyle GUIButton;
    public GUIStyle Background;
    public GUIStyle Text;
    string[] NomDeck;
    string selected;
    bool DeckChoisis;
    // Use this for initialization
    void Awake()
    {
        DeckChoisis = false;
        int tour = 0;
        connexionServeur.sck.ReceiveTimeout = 500;
        string message = "";
        bool recu = false;
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
        connexionServeur.sck.ReceiveTimeout = 0;
        NomDeck = message.Split(new char[] { ',' });
    }
    void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }
    void Start()
    {
        selected = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        warOfBaraxa.fontSize = Screen.width / 10;
        GUIBox.fontSize = Screen.width / 30;
        GUIButton.fontSize = Screen.width / 45;

        //GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.3f), Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
        GUI.Box(new Rect((Screen.width / 2) - (Screen.width * 0.4f), Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.6f), "Liste des decks", GUIBox);
        int y = 0;
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
        GUI.Label(new Rect((Screen.width * 0.55f), (Screen.height * 0.85f), Screen.width * 0.4f, Screen.height * 0.05f), "Deck choissis: " + selected, Text);
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
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.91f, Screen.width * 0.15f, Screen.height * 0.07f), "Rejoindre", GUIButton);
            GUI.Button(new Rect((Screen.width * 0.75f), (Screen.height * 0.8f), Screen.width * 0.1f, Screen.height * 0.07f), "annuler", GUIButton);
            GUI.enabled = true;
        }
        if (GUI.Button(new Rect(Screen.width * 0.65f, Screen.height * 0.91f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour", GUIButton))
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

