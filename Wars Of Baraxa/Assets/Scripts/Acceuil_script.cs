using UnityEngine;
using System.Collections;

public class Acceuil_script : MonoBehaviour
{
    public GUIStyle appuyerTouche;
    public GUIStyle warOfBaraxa;
    public GUIStyle Background;
    public GUIStyle Logo;
    public GUIStyle GUIBox;
    public GUIStyle TextArea;
    public GUIStyle LabelIP;
    public GUIStyle GUIButton;
    public bool displayLabel;
    public bool adresseServeur = false;
    public bool ConfirmerAdresse = false;
    public string IP;

    // Use this for initialization
    void Start() {
        displayLabel = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            adresseServeur = true;
            displayLabel = false;
        }
    }



    public void OnGUI()
    {
        //Affiche le Background
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        //Affiche le Logo en avant du Background
        GUI.Box(new Rect(35, 0, Screen.width - 125, Screen.height - 125), "", Logo);
        //Change la taille du texte Appuyez sur une touche
        appuyerTouche.fontSize = Screen.width / 25;

        //Affiche le texte si un button n'a pas été appuyé
        if (displayLabel)
        {
            GUI.Label(new Rect((Screen.width / 2) - (Screen.width * 0.6f / 2), Screen.height * 0.8f, Screen.width * 0.6f, Screen.height * 0.2f), "Appuyez sur une touche pour continuer", appuyerTouche);
        }

        //Affiche la boite de l'adresse IP si une touche a été appuyé
        if (adresseServeur)
        {
            GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f),"Entrer l'adresse IP du serveur:", GUIBox);
            IP = GUI.TextField(new Rect(((Screen.width / 2) - (Screen.width * 0.25f / 2)), Screen.height * 0.47f, Screen.width * 0.25f, Screen.height * 0.05f),IP,15,TextArea);
            if (GUI.Button(new Rect((Screen.width * 0.499f) - (Screen.width * 0.12f / 2), Screen.height * 0.57f, Screen.width * 0.12f, Screen.height * 0.06f), "Connecter", GUIButton))
            {
                //Affecte l'adresse du TextField pour la connection dans le Script_logIn
                connexionServeur.IP = IP;
                //Change la scène
                Application.LoadLevel("Connexion");
            }
        }
    }
}
