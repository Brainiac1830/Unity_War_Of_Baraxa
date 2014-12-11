using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using warsofbaraxa;

public class Script_logIn : MonoBehaviour {
//Variables
    public string alias  = "";
    public string password = "";
    public string nom = "";
    public string prenom  = "";
    public string messageErreur="";
    bool nouveauCompte = false;
    bool showBox = false;
    bool hitReturn = false;
    bool enTrainDeConnecter = false;

//GUIStyle
    public GUIStyle Logo;
    public GUIStyle textArea;
    public GUIStyle buttons;
    public GUIStyle text;
    public GUIStyle warOfBaraxa;
    public GUIStyle Background;
    public GUIStyle GUIBox;
    public GUIStyle GUIButton;
    //si on ferme l'application
    void OnApplicationQuit()
    {
        //on dit au serveur qu'on quitte
        envoyerMessage("deconnection");
    }
    //avant le début du programe
    void Awake()
    {
        //on se connecte au serveur
        if (connexionServeur.sck == null)
        {
            connexionServeur.sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(connexionServeur.IP), 1234);
            try
            {
                connexionServeur.sck.Connect(localEndPoint);
            }
                //si il y a un probleme on reload la scene de base
            catch (SocketException)
            {
                Application.LoadLevel("Acceuil");
            }
        }
    }
    //ONGUI est appelé a chaque frame et s'occupe de l'interface graphique
public void OnGUI() {
    //met l'image du background
	GUI.Box(new Rect(0,0,Screen.width,Screen.height),"",Background);
    //change la grosseur des textes
	warOfBaraxa.fontSize = Screen.width / 10;
	text.fontSize = Screen.width/35;
	textArea.fontSize = Screen.width/42;
	buttons.fontSize = Screen.width/42;
    //si il n'y a pas de message d'erreur
    if (!showBox)
    {
        /*Bonton enter pour log in*/
        if (Event.current.keyCode == KeyCode.Return)
            hitReturn = true;
        //si il a click sur enter et que on essaye de se connecter
        if (hitReturn && !enTrainDeConnecter && !nouveauCompte)
        {
            //on essaye de se connecter
            connect();
            hitReturn = false;
        }
        else if (hitReturn && !enTrainDeConnecter)
        {
            //sinon on veut créer un compte
            Creer();
            hitReturn = false;
        }
        //Titre
        //GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.6f / 2), Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
        //Alias
        GUI.Label(new Rect(Screen.width *0.29f, Screen.height * 0.47f, Screen.width * 0.10f, Screen.height * 0.05f), "Alias", text);
        alias = GUI.TextField(new Rect((Screen.width / 2) - (Screen.width * 0.25f / 2), Screen.height * 0.47f, Screen.width * 0.25f, Screen.height * 0.05f), alias, 20, textArea);
        //Mot de passe
        GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.6f / 2), Screen.height * 0.54f, Screen.width * 0.20f, Screen.height * 0.05f), "Mot de passe", text);
        password = GUI.PasswordField(new Rect((Screen.width / 2) - (Screen.width * 0.25f / 2), Screen.height * 0.54f, Screen.width * 0.25f, Screen.height * 0.05f), password, "*"[0], 20, textArea);

        //Si veut se connecter a un compte
        if (!nouveauCompte)
        {
            //Tente de se connecter avec les informations fournis
            if (GUI.Button(new Rect((Screen.width * 0.435f) - (Screen.width * 0.12f / 2), Screen.height * 0.62f, Screen.width * 0.12f, Screen.height * 0.06f), "Connecter", buttons))
            {
                //essaye de se connecter
                connect();
            }
            //Change l'interface pour pouvoir créer un compte
            if (GUI.Button(new Rect((Screen.width * 0.564f) - (Screen.width * 0.12f / 2), Screen.height * 0.62f, Screen.width * 0.12f, Screen.height * 0.06f), "Créer", buttons))
            {
                nouveauCompte = true;
            }
        }
        //Si on veut créer un nouveau compte (plus de champs a remplir et boutons changent de place)
        if (nouveauCompte)
        {
            //Prenom
            GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.43f / 2), Screen.height * 0.62f, Screen.width * 0.10f, Screen.height * 0.05f), "Prenom", text);
            prenom = GUI.TextField(new Rect((Screen.width / 2) - (Screen.width * 0.25f / 2), Screen.height * 0.62f, Screen.width * 0.25f, Screen.height * 0.05f), prenom, 25, textArea);
            //Nom
            GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.40f / 2), Screen.height * 0.68f, Screen.width * 0.10f, Screen.height * 0.05f), "Nom", text);
            nom = GUI.TextField(new Rect((Screen.width / 2) - (Screen.width * 0.25f / 2), Screen.height * 0.68f, Screen.width * 0.25f, Screen.height * 0.05f), nom, 25, textArea);

            //Si on click sur "RETOUR" on retourne a la connection d'un compte existant
            if (GUI.Button(new Rect((Screen.width * 0.435f) - (Screen.width * 0.12f / 2), Screen.height * 0.75f, Screen.width * 0.12f, Screen.height * 0.05f), "Retour", buttons))
            {
                alias = "";
                password = "";
                nom = "";
                prenom = "";
                nouveauCompte = false;
            }
            //Si on click sur "CRÉER" on tente de créer le compte selon les informations données (vérifications)
            if (GUI.Button(new Rect((Screen.width * 0.564f) - (Screen.width * 0.12f / 2), Screen.height * 0.75f, Screen.width * 0.12f, Screen.height * 0.05f), "Créer", buttons))
            {
                Creer();
            }
        }
    }
    else 
    {
        GUIBox.fontSize = Screen.width / 40;
        GUIButton.fontSize = Screen.width / 50;
        GUI.Box(new Rect(Screen.width * 0.30f, Screen.height * 0.35f, Screen.width * 0.40f, Screen.height * 0.20f),messageErreur, GUIBox);
        if (GUI.Button(new Rect((Screen.width * 0.425f), Screen.height * 0.45f, Screen.width * 0.15f, Screen.height * 0.07f), "ok", GUIButton))
        {
            showBox = false;
        }
    }
}		
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //si on click Return pour se conencter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Conditions d'erreurs
            if (alias == "" || password == "")
            {
                //si il manque des champs
                showBox = true;
                messageErreur = "Un des champs est vide. \n Veuillez entrer tous les champs.";
            }
            else if (!estDansBd(alias, password))
            {
                //si il n'est pas dans la BD
                showBox = true;
                messageErreur = "Votre alias ou votre mot de passe \n est invalide.";
            }
            else
            {
                //si il a réussis a se connecter
                Application.LoadLevel("Menu");
            }
        }
	}
    //créer un compte
    private void Creer()
    {
        //si un des champs n'est pas remplis
        if (alias == "" || password == "" || nom == "" || prenom == "")
        {
            //erreur
            showBox = true;
            messageErreur = "Un des champs est vide. \n Veuillez entrer tous les champs.";
        }
            //si il est deja créer 
        else if (getAliasBd(alias, password, nom, prenom))
        {
            //erreur
            showBox = true;
            messageErreur = "\n Votre alias est deja utiliser.";
        }
        else
        {
            //on créer le compte et on va au menu
            //manque a se connecter
            enTrainDeConnecter = true;
            Application.LoadLevel("Menu");
        }        
    }
    //connect un compte au jeu
    private void connect()
    {
        //si un des champs n'est pas remplis
        if (alias == "" || password == "")
        {
            //erreur
            showBox = true;
            messageErreur = "Un des champs est vide. \n Veuillez entrer tous les champs.";
        }
        //si les données sont incorrect ou qu'il est conencter
        else if (!estDansBd(alias, password))
        {
            //erreur
            showBox = true;
            messageErreur = "Votre alias ou votre mot de passe \n est invalide.";
        }
        else
        {
            //il se connect au jeu
            enTrainDeConnecter = true;
            Application.LoadLevel("Menu");
        }               
    }
    //essaye de créer le compte
    private bool getAliasBd(string alias,string mdp,string nom,string prenom)
    {
        string reponse = "";
        connexionServeur.sck.ReceiveTimeout = 500;
        bool recu = false;
        while (!recu)
        {
            try
            {
                envoyerMessage(alias + "," + mdp + "," + nom + "," + prenom);
                reponse = lire();
                recu = true;
            }
            catch (SocketException)
            {
                recu = false;
            }
        }
        connexionServeur.sck.ReceiveTimeout = 0;
        if (reponse == "oui")
        {
            connexionServeur.nom = alias;
            return true;
        }

        return false;
    }
    //essaye de voir si on peut se connect
    private bool estDansBd(string alias, string mdp)
    {
        string reponse = "";
        connexionServeur.sck.ReceiveTimeout = 500;
        bool recu = false;
        while (!recu)
        {
            try
            {
                envoyerMessage(alias + "," + mdp);
                reponse = lire();
                recu = true;
            }
            catch (SocketException)
            {
                recu = false;
            }
        }
        connexionServeur.sck.ReceiveTimeout = 0;
        if (reponse == "oui")
        {
            connexionServeur.nom = alias;
            return true;
        }

        return false;
    }
    //Sleep en unity
    public IEnumerator wait(float i)
    {
        yield return new WaitForSeconds(i);
    }
    //envoie un message au serveur
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
    }
    //recois un message du serveur
    private string lire()
    {
        string message = null;
        do
        {
            message = recevoirResultat();
            //tant qu'il n'a rien recus
        } while (message == null);
        return message;
    }
    //recois le string du serveur et l'envoie a la fct lire
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
