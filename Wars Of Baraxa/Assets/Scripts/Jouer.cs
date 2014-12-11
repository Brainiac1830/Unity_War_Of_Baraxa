using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using warsofbaraxa;

public class Jouer : MonoBehaviour
{
    //variable
    bool menu = false;
    static public Joueur joueur1;
    ThreadLire ReceiveMessage;
    Thread t;
    static public bool EstNul = false;
    static public bool gameFini = false;
    static public bool EstGagnant = false;
    static public bool EstPerdant = false;
    public attaque Script_attaque;
    //les zone utilisé
    static public PosZoneCombat[] ZoneCarteJoueur;
    static public PosZoneCombat[] ZoneCombat;
    static public GameObject[] styleCarteAlliercombat;
    static public PosZoneCombat[] ZoneCombatEnnemie;
    static public GameObject[] styleCarteEnnemisCombat;
    static public PosZoneCombat[] ZoneCarteEnnemie;

    public int NbCarteEnMainJoueur;
    static public bool placerClick;
    //les textute2D
    public Texture2D Test;
    public Texture2D ble;
    public Texture2D bois;
    public Texture2D gem;
    public Texture2D bleAjouter;
    public Texture2D boisAjouter;
    public Texture2D gemAjouter;
    public Texture2D Worker;
    public Texture2D PlayerChar;
    public Texture2D EnnemiChar;
    public Texture2D imgTaunt;
    public Texture2D imgSleep;
    public Texture2D imgInvisible;
    public Texture2D imgAttaquePuissante;
    public Texture2D imgAttaqueDouble;
    public SpriteRenderer ImageCarte;

    public static int NbBle; //test avec static
    public static int NbBois; // test avec static
    public static int NbGem; // test avec statics
    //son
    public AudioClip AttackSound;
    public GUIStyle GUIBox;
    public GUIStyle GUIButton;
    //GameObject pour les habileté de créature
    Color color;
    public static GameObject taunt = null;
    public static GameObject invis = null;
    public static GameObject attaqueDouble = null;
    public static GameObject attaquePuissante = null;
    public static GameObject sleep = null;
    public static int numero = 0;
    public static int taille = 0;

    public static int attaqueBonus;
    public static int armureBonus;
    public static int vieBonus;
    ////////////////////////////////////////////// Spellz
    public static bool enTrainCaster = false;
    public static bool enTrainAttaquer = false;
    public static int posCarteEnTrainCaster = 0; //
    public static bool targetNeeded = false;    //
    public static bool isEnnemi = false;        //
    public static string effet;                 //
    public static string spellTarget;
    public static GameObject spell;             //
    public static string[] texteHabileteSansEspace;
    public static string texteHabileteSansNewline;
    public static GameObject target;
    public static Carte carteTarget;
    public static int position;
    ////////////////////////////////////////////// Spellz
    //pour les ressources
    public int NbWorkerMax;
    public int NbWorker;
    public int NbBleEnnemis;
    public int NbBoisEnnemis;
    public int NbGemEnnemis;
    //Hp des joueur
    public static int HpJoueur;
    public static int HpEnnemi;
    //carte des joueur
    public int nbCarteAllier;
    public int nbCarteEnnemis;
    //pour les prefabs
    public Transform PlacementCarte;
    public Transform carteBack;
    public Transform carteBatiment;
    public Transform carteCreature;
    public GameObject card;
    public GameObject cardennemis;
    public JouerCarteBoard ScriptEnnemie;

    public int NoCarte;
    public int noCarteEnnemis;
    static public float pos;
    //deck + carte(objet unity)
    static public Deck tabCarteAllier;
    static public GameObject[] styleCarteAllier;
    static int[] ordrePige;
    static int dernierePige;
    //inutile i guess
    static public Carte[] tabCarteEnnemis;
    static public GameObject[] styleCarteEnnemis;

    static public bool MonTour;
    //au debut de la scene(avant)
    void Awake()
    {
        //on recois le deck et qui est le premier joueur
        tabCarteAllier = ReceiveDeck(connexionServeur.sck);
        string message = recevoirResultat();
        if (message == "Premier Joueur")
            MonTour = true;
        else
            MonTour = false;
    }
    //quand on quitte on le dit au serveur
    void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }

    //initialization
    void Start()
    {
        NoCarte = 0;
        ReceiveMessage = new ThreadLire();
        ReceiveMessage.workSocket = connexionServeur.sck;
        InitZoneJoueur();
        InitZoneEnnemie();
        InitZoneCombatEnnemie();
        InitZoneCombatJoueur();
        pos = 0;
        card = null;
        NbCarteEnMainJoueur = 0;
        HpJoueur = 30;
        HpEnnemi = 30;
        nbCarteAllier = 40;
        nbCarteEnnemis = 40;
        NbBle = 0;
        NbBois = 0;
        NbGem = 0;
        NbBleEnnemis = 0;
        NbBoisEnnemis = 0;
        NbGemEnnemis = 0;
        NbWorkerMax = 2;
        NbWorker = NbWorkerMax;
        placerClick = false;
        //stat bonus
        attaqueBonus = 0;
        armureBonus = 0;
        vieBonus = 0;
        //deck
        styleCarteAllier = new GameObject[40];
        //deck ennemis
        tabCarteEnnemis = new Carte[40];
        styleCarteEnnemis = new GameObject[40];
        ordrePige = new int[40];
        //board
        styleCarteAlliercombat = new GameObject[7];
        styleCarteEnnemisCombat = new GameObject[7];
        joueur1 = new Joueur("player1");
        instantiateCardAllies();
        instantiateCardEnnemis();
        initOrdrePige(ordrePige);
        CarteDepart();
    }
    //modifie le text pour que se soit plus beau pour les cartes
    private string formaterHabilete(string texte)
    {
        //maxCaractere
        string temp = "";
        const int maxcaractere = 21;
        //on parse a chaque habilete
        string[] tab = texte.Split(new char[] { ',' });
        //pour chaque habilete
        for (int i = 0; i < tab.Length; ++i)
        {
            //si la grousseur du texte est plus gros que la grosseur maximal
            if (tab[i].Length > maxcaractere)
            {
                //on parse a chaque mot
                int longueur = 0;
                string[] mot = tab[i].Split(new char[] { ' ' });
                for (int y = 0; y < mot.Length; ++y)
                {
                    //on cherche quel mot dépasse de la carte et on met un \n au mot précedant
                    if (longueur + mot[y].Length + 1 > maxcaractere)
                    {
                        mot[y - 1] += "\n";
                        longueur = mot[y].Length + 1;
                    }
                        //sinon on rajoute a la longeuur
                    else
                        longueur += mot[y].Length + 1;

                }
                for (int y = 0; y < mot.Length; ++y)
                {
                    //on le remet dans le texte maintenant formater
                    if (y != 0 && mot[y - 1].IndexOf('\n') != -1)
                    {
                        temp += mot[y];
                    }
                    else if (y == 0)
                        temp += mot[y];
                    else
                        temp += " " + mot[y];
                }
            }
                //si le texte est corret on rajoute un \n et on le met dans le temp
            else
            {
                temp += tab[i] + "\n";
            }
        }

        return temp;
    }
    //quand on finis on ferme le thread
    void OnDestroy()
    {
        if (t != null)
            t.Abort();
    }
    //crée les 40 cartes du joueur allié
    public void instantiateCardAllies()
    {
        //on instantie le prefab
        Transform cards;
        for (int i = 0; i < styleCarteAllier.Length; ++i)
        {
            cards = null;
            cards = Instantiate(PlacementCarte, new Vector3(0, 0, -5), Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;

            foreach (Transform child in cards)
            {
                child.name = child.name + i;
                child.tag = "textStats";
            }


            //on leur donne leur habileté on leur met leur icon selon leur habileté et on change leur value
            styleCarteAllier[i] = cards.gameObject;
            styleCarteAllier[i].name = "card" + i;
            setValue(i, cards, true);
            tabCarteAllier.CarteDeck[i] = setHabilete(tabCarteAllier.CarteDeck[i]);
            takeOutIconsFriendly(i);
        }
    }
    //meme chose mais pour les cartes ennmeis(on ne peut pas set car on na pas le deck ennemis)
    public void instantiateCardEnnemis()
    {
        Transform cards;
        for (int i = 0; i < styleCarteEnnemis.Length; ++i)
        {
            cards = null;
            cards = Instantiate(PlacementCarte, new Vector3(0, 0, -10), Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
            foreach (Transform child in cards)
            {
                child.name = child.name + "Ennemis" + i;
                child.tag = "textStats";
            }



            styleCarteEnnemis[i] = cards.gameObject;
            styleCarteEnnemis[i].name = "cardennemis" + i;
            takeOutIconsEnnemy(i);
        }
    }
    //fait une pige random pour le joueur
    public void initOrdrePige(int[] tab)
    {
        List<int> tabNombre = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
        for (int i = 0; i < tab.Length; ++i)
        {
            /*besoin du unity engine car il ne sest pas quel prendre entre celle de unityengine et celle de sysytem c#*/
            int temp = UnityEngine.Random.Range(0, tab.Length - (1 + i));
            tab[i] = tabNombre[temp];
            tabNombre.RemoveAt(temp);
        }
    }
    ///////////-----crée les zones----////////////
    public void InitZoneJoueur()
    {
        //on fait une nouvelle zone avec le nombre de position voulue
        ZoneCarteJoueur = new PosZoneCombat[7];
        float pos = 0;
        for (int i = 0; i < ZoneCarteJoueur.Length; ++i)
        {
            //pour chaque on leur donne une nouvelle position
            ZoneCarteJoueur[i] = new PosZoneCombat();
            ZoneCarteJoueur[i].Pos = new Vector3(-5.0f + pos, -3.9f, 6.0f);
            pos += 1.4f;
        }
    }
    public void InitZoneEnnemie()
    {
        ZoneCarteEnnemie = new PosZoneCombat[7];
        float pos = 0;
        for (int i = 0; i < ZoneCarteEnnemie.Length; ++i)
        {
            ZoneCarteEnnemie[i] = new PosZoneCombat();
            ZoneCarteEnnemie[i].Pos = new Vector3(-4.0f + pos, 3.95f, 6.0f);
            pos += 1.4f;
        }
    }

    public void InitZoneCombatEnnemie()
    {
        ZoneCombatEnnemie = new PosZoneCombat[7];
        float pos = 0;
        for (int i = 0; i < ZoneCombatEnnemie.Length; ++i)
        {
            ZoneCombatEnnemie[i] = new PosZoneCombat();
            ZoneCombatEnnemie[i].Pos = new Vector3(-4.0f + pos, 1.5f, 6.0f);
            pos += 1.4f;
        }
    }

    public void InitZoneCombatJoueur()
    {
        ZoneCombat = new PosZoneCombat[7];
        float pos = 0;
        for (int i = 0; i < ZoneCombat.Length; ++i)
        {
            ZoneCombat[i] = new PosZoneCombat();
            ZoneCombat[i].Pos = new Vector3(-4.0f + pos, -1.5f, 6.0f);
            pos += 1.4f;
        }
    }
    ///////////----- fin crée les zones----////////////
    //met 4 carte dans cahque main
    public void CarteDepart()
    {
        int pos = 0;
        float posi = 0;
        while (NoCarte < 4)
        {
            //crée les carte ennemis(de derriere) et met les places de la zone a estoccupe
            Transform Ennemis = Instantiate(carteBack, ZoneCarteEnnemie[pos].Pos, Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
            ZoneCarteJoueur[pos].EstOccupee = true;
            ZoneCarteEnnemie[pos].EstOccupee = true;

            cardennemis = Ennemis.gameObject;
            //on dit que l'ennemie est ennemis (no shit sherlock)
            ScriptEnnemie = Ennemis.GetComponent<JouerCarteBoard>();
            ScriptEnnemie.EstEnnemie = true;
            cardennemis.name = "cardbackennemis" + NoCarte.ToString();
            //on donne au pos leur carte
            ZoneCarteJoueur[NoCarte].carte = tabCarteAllier.CarteDeck[ordrePige[NoCarte]];
            styleCarteAllier[ordrePige[NoCarte]].gameObject.transform.position = ZoneCarteJoueur[NoCarte].Pos;
            //on change le style de la zone
            styleCarteEnnemis[NoCarte] = cardennemis;
            ZoneCarteEnnemie[pos].carte = tabCarteEnnemis[pos];
            //incrément la pige et décremente le nombre de carte restante
            ++pos;
            --nbCarteAllier;
            --nbCarteEnnemis;
            posi += 1.5f;
            ++NoCarte;
        }
        noCarteEnnemis = NoCarte;
    }
    /// <summary>
    /// permet de changer les stats de la carte(prefab) donc pour que l'utilisateur puisse le voir.
    /// plusieur fonction pareil selon ce que nous avons besoin
    /// on change le cout(ble, bois, gem), habilete, Nom, image, si c'est un permanent on change aussi attaque, armure, vie, type
    /// </summary>
    /// <param name="i">la position de la carte pour savoir le num du game object</param>
    /// <param name="t">le transform de la carte pour trouver les textmesh</param>
    /// <param name="card">la carte ou que l'on prend les vrai données</param>
    /// <param name="allier">si c'est une de nos carte ou non</param>
    private void setValueFromCard(int i, Transform t, Carte card, bool allier)
    {
        if (allier)
        {
            t.Find("coutBois" + i).GetComponent<TextMesh>().text = card.CoutBois.ToString();
            t.Find("coutBle" + i).GetComponent<TextMesh>().text = card.CoutBle.ToString();
            t.Find("coutGem" + i).GetComponent<TextMesh>().text = card.CoutGem.ToString();
            t.Find("habilete" + i).GetComponent<TextMesh>().text = formaterHabilete(card.Habilete);
            t.Find("Nom" + i).GetComponent<TextMesh>().text = card.NomCarte;
            t.Find("Image" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(card.NomCarte, typeof(Sprite)) as Sprite;
            if (card.perm != null)
            {
                t.Find("attaque" + i).GetComponent<TextMesh>().text = card.perm.Attaque.ToString();
                t.Find("armure" + i).GetComponent<TextMesh>().text = card.perm.Armure.ToString();
                t.Find("vie" + i).GetComponent<TextMesh>().text = card.perm.Vie.ToString();
                t.Find("type" + i).GetComponent<TextMesh>().text = card.perm.TypePerm;
            }
            else if (card.TypeCarte == "Sort")
                t.Find("type" + i).GetComponent<TextMesh>().text = card.TypeCarte;
        }
        else
        {
            t.Find("coutBoisEnnemis" + i).GetComponent<TextMesh>().text = card.CoutBois.ToString();
            t.Find("coutBleEnnemis" + i).GetComponent<TextMesh>().text = card.CoutBle.ToString();
            t.Find("coutGemEnnemis" + i).GetComponent<TextMesh>().text = card.CoutGem.ToString();
            t.Find("habileteEnnemis" + i).GetComponent<TextMesh>().text = formaterHabilete(card.Habilete);
            t.Find("NomEnnemis" + i).GetComponent<TextMesh>().text = card.NomCarte;
            t.Find("ImageEnnemis" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(card.NomCarte, typeof(Sprite)) as Sprite;
            if (card.perm != null)
            {
                t.Find("attaqueEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Attaque.ToString();
                t.Find("armureEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Armure.ToString();
                t.Find("vieEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Vie.ToString();
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = card.perm.TypePerm;
            }
            else if (card.TypeCarte == "Sort")
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = card.TypeCarte;
        }
    }
    //meme chose seulement il ne prend aucune carte il va la chercher dans le tableau de carte(pour le début)
    private void setValue(int i, Transform t, bool allier)
    {
        if (allier)
        {
            t.Find("coutBois" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].CoutBois.ToString();
            t.Find("coutBle" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].CoutBle.ToString();
            t.Find("coutGem" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].CoutGem.ToString();
            t.Find("habilete" + i).GetComponent<TextMesh>().text = formaterHabilete(tabCarteAllier.CarteDeck[i].Habilete);
            t.Find("Nom" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].NomCarte;
            t.Find("Image" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(tabCarteAllier.CarteDeck[i].NomCarte, typeof(Sprite)) as Sprite;

            if (tabCarteAllier.CarteDeck[i].perm != null)
            {
                t.Find("attaque" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].perm.Attaque.ToString();
                t.Find("armure" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].perm.Armure.ToString();
                t.Find("vie" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].perm.Vie.ToString();
                t.Find("type" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].perm.TypePerm;
            }
            else if (tabCarteAllier.CarteDeck[i].TypeCarte == "Sort")
                t.Find("type" + i).GetComponent<TextMesh>().text = tabCarteAllier.CarteDeck[i].TypeCarte;
        }
        else
        {
            t.Find("coutBoisEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutBois.ToString();
            t.Find("coutBleEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutBle.ToString();
            t.Find("coutGemEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutGem.ToString();
            t.Find("habileteEnnemis" + i).GetComponent<TextMesh>().text = formaterHabilete(tabCarteEnnemis[i].Habilete);
            t.Find("NomEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].NomCarte;
            t.Find("ImageEnnemis" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(tabCarteEnnemis[i].NomCarte, typeof(Sprite)) as Sprite;
            if (tabCarteAllier.CarteDeck[i].perm != null)
            {
                t.Find("attaqueEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Attaque.ToString();
                t.Find("armureEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Armure.ToString();
                t.Find("vieEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Vie.ToString();
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.TypePerm;
            }
            else if (tabCarteEnnemis[i].TypeCarte == "Sort")
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].TypeCarte;
        }
    }
    //meme chose qu'avant seulement on doit trouver la position du gameobject
    private void setValueFromCard(Transform t, Carte card, bool allier)
    {
        if (allier)
        {
            int taille = t.name.Length - 4;
            int i = int.Parse(t.name.Substring(4, taille));
            t.Find("coutBois" + i).GetComponent<TextMesh>().text = card.CoutBois.ToString();
            t.Find("coutBle" + i).GetComponent<TextMesh>().text = card.CoutBle.ToString();
            t.Find("coutGem" + i).GetComponent<TextMesh>().text = card.CoutGem.ToString();
            t.Find("habilete" + i).GetComponent<TextMesh>().text = formaterHabilete(card.Habilete);
            t.Find("Nom" + i).GetComponent<TextMesh>().text = card.NomCarte;
            t.Find("Image" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(card.NomCarte, typeof(Sprite)) as Sprite;
            if (card.perm != null)
            {
                t.Find("attaque" + i).GetComponent<TextMesh>().text = card.perm.Attaque.ToString();
                t.Find("armure" + i).GetComponent<TextMesh>().text = card.perm.Armure.ToString();
                t.Find("vie" + i).GetComponent<TextMesh>().text = card.perm.Vie.ToString();
                t.Find("type" + i).GetComponent<TextMesh>().text = card.perm.TypePerm;
            }
            else if (card.TypeCarte == "Sort")
                t.Find("type" + i).GetComponent<TextMesh>().text = card.TypeCarte;
        }
        else
        {
            int taille = t.name.Length - 11;
            int i = int.Parse(t.name.Substring(11, taille));
            t.Find("coutBoisEnnemis" + i).GetComponent<TextMesh>().text = card.CoutBois.ToString();
            t.Find("coutBleEnnemis" + i).GetComponent<TextMesh>().text = card.CoutBle.ToString();
            t.Find("coutGemEnnemis" + i).GetComponent<TextMesh>().text = card.CoutGem.ToString();
            t.Find("habileteEnnemis" + i).GetComponent<TextMesh>().text = formaterHabilete(card.Habilete);
            t.Find("NomEnnemis" + i).GetComponent<TextMesh>().text = card.NomCarte;
            t.Find("ImageEnnemis" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load(card.NomCarte, typeof(Sprite)) as Sprite;
            if (card.perm != null)
            {
                t.Find("attaqueEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Attaque.ToString();
                t.Find("armureEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Armure.ToString();
                t.Find("vieEnnemis" + i).GetComponent<TextMesh>().text = card.perm.Vie.ToString();
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = card.perm.TypePerm;
            }
            else if (card.TypeCarte == "Sort")
                t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = card.TypeCarte;
        }
    }
    //meme chose seulement on utilise que la carte et un transform
    //on va donc aller chercher tout les enfants du prefab et le modifié selon leur position
    private void setValueFromCard(Transform t, Carte card)
    {
        if (card != null && t != null)
        {
            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[0].text = card.CoutBois.ToString();
            stat[1].text = card.CoutBle.ToString();
            stat[2].text = card.CoutGem.ToString();
            stat[6].text = card.NomCarte;
            stat[7].text = card.Habilete;
            if (card.perm != null)
            {
                stat[3].text = card.perm.Armure.ToString();
                stat[4].text = card.perm.Attaque.ToString();
                stat[5].text = card.perm.Vie.ToString();
            }
            if (card.TypeCarte == "Sort")
                stat[8].text = card.TypeCarte;
            else
                stat[8].text = card.perm.TypePerm;
            t.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load(card.NomCarte, typeof(Sprite)) as Sprite;
        }
    }
    //modifie les stat(attaque, armure, vie) d'une créature selon une carte dans le prefab
    private void changestatFromCard(GameObject t, Carte card)
    {
        if (card != null && card.perm != null && t != null)
        {
            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[3].text = card.perm.Armure.ToString();
            stat[4].text = card.perm.Attaque.ToString();
            stat[5].text = card.perm.Vie.ToString();
        }
    }
    //vérifie si la cartes a une attribut et lui donne l'icone selon l'attribut
    private void takeOutIconsFriendly(int i)
    {
        if (styleCarteAllier[i] != null)
        {
            taunt = styleCarteAllier[i].transform.FindChild("zeattributs" + i.ToString()).FindChild("taunt").gameObject;
            invis = styleCarteAllier[i].transform.FindChild("zeattributs" + i.ToString()).FindChild("invis").gameObject;
            attaqueDouble = styleCarteAllier[i].transform.FindChild("zeattributs" + i.ToString()).FindChild("attaqueDouble").gameObject;
            attaquePuissante = styleCarteAllier[i].transform.FindChild("zeattributs" + i.ToString()).FindChild("attaquePuissante").gameObject;
            sleep = styleCarteAllier[i].transform.FindChild("zeattributs" + i.ToString()).FindChild("sleep").gameObject;
            //turn off taunt icon
            color = taunt.renderer.material.color;
            color.a = 0f;
            taunt.renderer.material.color = color;
            //turn off invis icon
            color = invis.renderer.material.color;
            color.a = 0f;
            invis.renderer.material.color = color;
            //turn off attaque double icon
            color = attaqueDouble.renderer.material.color;
            color.a = 0f;
            attaqueDouble.renderer.material.color = color;
            //turn off attaque puissante icon
            color = attaquePuissante.renderer.material.color;
            color.a = 0f;
            attaquePuissante.renderer.material.color = color;
            //turn off sleep icon
            color = sleep.renderer.material.color;
            color.a = 0f;
            sleep.renderer.material.color = color;
        }


    }
    //meme chose, mais pour les cartes ennemis
    private void takeOutIconsEnnemy(int i)
    {

        if (styleCarteEnnemis[i] != null)
        {
            taunt = styleCarteEnnemis[i].transform.FindChild("zeattributsEnnemis" + i.ToString()).FindChild("taunt").gameObject;
            invis = styleCarteEnnemis[i].transform.FindChild("zeattributsEnnemis" + i.ToString()).FindChild("invis").gameObject;
            attaqueDouble = styleCarteEnnemis[i].transform.FindChild("zeattributsEnnemis" + i.ToString()).FindChild("attaqueDouble").gameObject;
            attaquePuissante = styleCarteEnnemis[i].transform.FindChild("zeattributsEnnemis" + i.ToString()).FindChild("attaquePuissante").gameObject;
            sleep = styleCarteEnnemis[i].transform.FindChild("zeattributsEnnemis" + i.ToString()).FindChild("sleep").gameObject;
            //turn off taunt icon
            color = taunt.renderer.material.color;
            color.a = 0f;
            taunt.renderer.material.color = color;
            //turn off invis icon
            color = invis.renderer.material.color;
            color.a = 0f;
            invis.renderer.material.color = color;
            //turn off attaque double icon
            color = attaqueDouble.renderer.material.color;
            color.a = 0f;
            attaqueDouble.renderer.material.color = color;
            //turn off attaque puissante icon
            color = attaquePuissante.renderer.material.color;
            color.a = 0f;
            attaquePuissante.renderer.material.color = color;
            //turn off sleep icon
            color = sleep.renderer.material.color;
            color.a = 0f;
            sleep.renderer.material.color = color;
        }
    }
    //affiche les icones spécial
    private void afficherIconSpecial()
    {
        //pour chaque carte
        for (int i = 0; i < ZoneCombat.Length; i++)
        {
            //carte allier
            if (styleCarteAlliercombat[i] != null)
            {
                //trouve les enfants du prefab selon le gameObject et sa position
                taille = styleCarteAlliercombat[i].name.Length - 4;
                numero = int.Parse(styleCarteAlliercombat[i].name.Substring(4, taille));
                taunt = styleCarteAlliercombat[i].transform.FindChild("zeattributs" + numero.ToString()).FindChild("taunt").gameObject;
                invis = styleCarteAlliercombat[i].transform.FindChild("zeattributs" + numero.ToString()).FindChild("invis").gameObject;
                attaqueDouble = styleCarteAlliercombat[i].transform.FindChild("zeattributs" + numero.ToString()).FindChild("attaqueDouble").gameObject;
                attaquePuissante = styleCarteAlliercombat[i].transform.FindChild("zeattributs" + numero.ToString()).FindChild("attaquePuissante").gameObject;
                sleep = styleCarteAlliercombat[i].transform.FindChild("zeattributs" + numero.ToString()).FindChild("sleep").gameObject;
                //si il y a une des icone on l'affiche selon l'attribut que la carte possède
                //provocation
                if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm != null && ZoneCombat[i].carte.perm.estTaunt)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    taunt.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    taunt.renderer.material.color = color;
                }
                //invisible
                if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm.estInvisible)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    invis.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    invis.renderer.material.color = color;
                }
                //attaque double
                if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm.estAttaqueDouble)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    attaqueDouble.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    attaqueDouble.renderer.material.color = color;
                }
                //attaque puissante
                if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm.estAttaquePuisante)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    attaquePuissante.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    attaquePuissante.renderer.material.color = color;
                }
                //endormi
                if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm.estEndormi != 0)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    sleep.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    sleep.renderer.material.color = color;
                }
            }


            //carte ennemis
            if (styleCarteEnnemisCombat[i] != null)
            {
                //trouve les enfants du prefab selon le gameObject et sa position
                taille = styleCarteEnnemisCombat[i].name.Length - 11;
                numero = int.Parse(styleCarteEnnemisCombat[i].name.Substring(11, taille));
                taunt = styleCarteEnnemisCombat[i].transform.FindChild("zeattributsEnnemis" + numero.ToString()).FindChild("taunt").gameObject;
                invis = styleCarteEnnemisCombat[i].transform.FindChild("zeattributsEnnemis" + numero.ToString()).FindChild("invis").gameObject;
                attaqueDouble = styleCarteEnnemisCombat[i].transform.FindChild("zeattributsEnnemis" + numero.ToString()).FindChild("attaqueDouble").gameObject;
                attaquePuissante = styleCarteEnnemisCombat[i].transform.FindChild("zeattributsEnnemis" + numero.ToString()).FindChild("attaquePuissante").gameObject;
                sleep = styleCarteEnnemisCombat[i].transform.FindChild("zeattributsEnnemis" + numero.ToString()).FindChild("sleep").gameObject;
                //si il y a une des icone on l'affiche selon l'attribut que la carte possède
                //provocation
                if (ZoneCombatEnnemie[i].carte != null && ZoneCombatEnnemie[i].carte.perm.estTaunt)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    taunt.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    taunt.renderer.material.color = color;
                }
                //invisible
                if (ZoneCombatEnnemie[i].carte != null && ZoneCombatEnnemie[i].carte.perm.estInvisible)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    invis.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    invis.renderer.material.color = color;
                }
                //attaque double
                if (ZoneCombatEnnemie[i].carte != null && ZoneCombatEnnemie[i].carte.perm.estAttaqueDouble)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    attaqueDouble.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    attaqueDouble.renderer.material.color = color;
                }
                //attaque puissante
                if (ZoneCombatEnnemie[i].carte != null && ZoneCombatEnnemie[i].carte.perm.estAttaquePuisante)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    attaquePuissante.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    attaquePuissante.renderer.material.color = color;
                }
                //endormi
                if (ZoneCombatEnnemie[i].carte != null && ZoneCombatEnnemie[i].carte.perm.estEndormi != 0)
                {
                    color = taunt.renderer.material.color;
                    color.a = 1f;
                    sleep.renderer.material.color = color;
                }
                else
                {
                    color = taunt.renderer.material.color;
                    color.a = 0f;
                    sleep.renderer.material.color = color;
                }
            }


        }
    }
    // Update is called once per frame
    void Update()
    {
        //si on click sur escape on va ouvrir le menu pour quitter(si le menu est déjà ouvert on va le fermer)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu)
            {
                menu = false;
            }
            else
            {
                menu = true;
            }
        }
            //si c'est un click droit on et que l'on est en train de jouer un sort il sera annulé
        else if (Input.GetMouseButton(1) && (enTrainCaster))
        {
            enTrainCaster = false;
            Jouer.spell.transform.position = ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].Pos;
            ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].EstOccupee = true;
            NbBle += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutBle;
            NbBois += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutBois;
            NbGem += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutGem;
            joueur1.nbBle += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutBle;
            joueur1.nbBois += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutBois;
            joueur1.nbGem += ZoneCarteJoueur[Jouer.posCarteEnTrainCaster].carte.CoutGem;
        }
        Script_attaque = GetComponent<attaque>();
        if (enTrainCaster)
        {
            Script_attaque.enabled = false;
        }
        else
        {
            Script_attaque.enabled = true;
        }
    }
    //affichage (refresh per frame)
    public void OnGUI()
    {
        Event e;
        e = Event.current;

        //Héro Joueur
        GUI.Label(new Rect(Screen.width * 0.045f, Screen.height * 0.76f, Screen.width * 1.0f, Screen.height * 1.0f), "Vie: " + HpJoueur.ToString());
        GUI.Label(new Rect(Screen.width * 0.045f, Screen.height * 0.73f, Screen.width * 1.0f, Screen.height * 1.0f), "Nombre de carte: " + nbCarteAllier.ToString());
        //Héro Ennemi
        GUI.Label(new Rect(Screen.width * 0.90f, Screen.height * 0.001f, Screen.width * 1.0f, Screen.height * 1.0f), "Vie: " + HpEnnemi.ToString());
        GUI.Label(new Rect(Screen.width * 0.90f, Screen.height * 0.03f, Screen.width * 1.0f, Screen.height * 1.0f), "Nombre de carte: " + nbCarteEnnemis.ToString());
        //si on doit afficher le menu
        if (menu)
        {
            //on change la grosseur du texte
            GUIBox.fontSize = Screen.width / 30;
            GUIButton.fontSize = Screen.width / 40;
            //on crée une box qui va contenir un bouton abbandoner et un autre retour au jeu
            GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.17f, Screen.height * 0.30f), "", GUIBox);
            GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.385f, Screen.width * 0.005f, Screen.height * 0.005f), "Menu", GUIButton);
            //si au decide dabbandoner
            if (GUI.Button(new Rect((Screen.width * 0.36f), Screen.height * 0.43f, Screen.width * 0.14f, Screen.height * 0.07f), "Abandonner", GUIButton))
            {
                //la partie est terminer
                gameFini = true;
                EstPerdant = true;
                //on l'envoie au serveur
                envoyerMessage("surrender");
                //StartCoroutine(wait(1.5f));
            }
            //sinon on retourne au jeu
            if (GUI.Button(new Rect(Screen.width * 0.36f, Screen.height * 0.53f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour au jeu", GUIButton))
            {
                menu = false;
            }
        }
        //si la game est fini
        if (gameFini)
        {
            //on met une autre fenetre
            GUIBox.fontSize = Screen.width / 25;
            GUIButton.fontSize = Screen.width / 35;
            //si on gagne on affiche gagne et on retourne au menu
            if (EstGagnant)
            {
                GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), "\n  Vous avez gagné!", GUIBox);
                if (GUI.Button(new Rect((Screen.width * 0.43f), Screen.height * 0.54f, Screen.width * 0.135f, Screen.height * 0.07f), "Menu", GUIButton))
                {
                    //envoye un message car si l'ennemis part le jour va le savoir mais va rester dans la partie il faut donc un message au serveur pour qu'il puisse lui aussi retourner au menu
                    envoyerMessage("asd");
                    t.Abort();
                    //on load le level
                    Application.LoadLevel("Menu");
                    EstGagnant = false;
                    gameFini = false;
                }
            }
                //meme chose seulement on affiche vous avez perdu
            else if (EstPerdant)
            {
                GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), "\n  Vous avez perdu!", GUIBox);
                if (GUI.Button(new Rect((Screen.width * 0.43f), Screen.height * 0.54f, Screen.width * 0.135f, Screen.height * 0.07f), "Menu", GUIButton))
                {
                    envoyerMessage("asd");
                    t.Abort();
                    Application.LoadLevel("Menu");
                    ReceiveMessage.message = "";
                    EstPerdant = false;
                    gameFini = false;
                }
            }
                //meme chose seulement la partie est nul
            else if (EstNul)
            {
                GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), "\n  Partie nul!", GUIBox);
                if (GUI.Button(new Rect((Screen.width * 0.43f), Screen.height * 0.54f, Screen.width * 0.135f, Screen.height * 0.07f), "Menu", GUIButton))
                {
                    envoyerMessage("asd");
                    t.Abort();
                    Application.LoadLevel("Menu");
                }
            }
        }

        //BTN EndTurn
        if (placerClick && MonTour)
        {
            if (GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Fini"))
            {
                //on dit au serveur que note tour est finis
                envoyerMessage("Fin De Tour");
                //StartCoroutine(wait(1.5f));
                MonTour = false;
                //on reset l'armure
                resetArmor(ZoneCombat, styleCarteAlliercombat, true);
                resetArmor(ZoneCombatEnnemie, styleCarteEnnemisCombat, false);
                //l'ennemis pige une carte
                pigerCarteEnnemis();
                //on redeuit le sleep de nos créature
                descendreSleep(ZoneCombat);
                //on ne peut plus attaquer
                Script_attaque = GetComponent<attaque>();
                Script_attaque.enabled = false;
            }
        }
            //le bouton fini est disabled
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Fini");
            GUI.enabled = true;
        }
        //blé
        GUI.Label(new Rect(Screen.width * 0.005f, Screen.height * 0.005f, Screen.width * 0.05f, Screen.height * 0.07f), ble);
        GUI.Label(new Rect(Screen.width * 0.005f, Screen.height * 0.07f, Screen.width * 0.09f, Screen.height * 0.07f), "Blé: " + NbBleEnnemis.ToString());
        //bois
        GUI.Label(new Rect(Screen.width * 0.06f, Screen.height * 0.005f, Screen.width * 0.05f, Screen.height * 0.07f), bois);
        GUI.Label(new Rect(Screen.width * 0.06f, Screen.height * 0.07f, Screen.width * 0.09f, Screen.height * 0.07f), "Bois: " + NbBoisEnnemis.ToString());
        //gem
        GUI.Label(new Rect(Screen.width * 0.14f, Screen.height * 0.005f, Screen.width * 0.05f, Screen.height * 0.07f), gem);
        GUI.Label(new Rect(Screen.width * 0.14f, Screen.height * 0.07f, Screen.width * 0.09f, Screen.height * 0.07f), "Gemmes: " + NbGemEnnemis.ToString());
        if (!placerClick && MonTour)
        {
            //AJOUTER BLE
            if (GUI.Button(new Rect(Screen.width / 1.27f, Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), bleAjouter))
            {
                NbBle = SetManaAjouter(e, NbBle, "ble");
            }
            GUI.Label(new Rect(Screen.width / 1.27f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Blé: " + NbBle.ToString());
            //AJOUTER BOIS
            if (GUI.Button(new Rect((Screen.width / 1.17f), Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), boisAjouter))
            {
                NbBois = SetManaAjouter(e, NbBois, "bois");
            }
            GUI.Label(new Rect(Screen.width / 1.17f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Bois: " + NbBois.ToString());
            //AJOUTER GEM
            if (GUI.Button(new Rect((Screen.width / 1.08f), Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), gemAjouter))
            {
                NbGem = SetManaAjouter(e, NbGem, "gem");
            }
            GUI.Label(new Rect(Screen.width / 1.08f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Gemmes: " + NbGem.ToString());
            //WORKER
            GUI.Label(new Rect(Screen.width / 1.3f, Screen.height / 1.26f, Screen.width * 0.25f, Screen.height * 0.10f), Worker);
            GUI.Label(new Rect(Screen.width / 1.23f, Screen.height / 1.17f, Screen.width * 0.15f, Screen.height * 0.1f), "Artisan(s): " + NbWorker.ToString());
            //si on click sur placer
            if (GUI.Button(new Rect(Screen.width / 1.12f, Screen.height / 1.26f, Screen.width * 0.1f, Screen.height * 0.1f), "Placer") && NbWorker == 0)
            {
                //on ne peut rien faire pendant ce temps
                waitForActionDone();
                placerClick = true;
                //on set la mana
                joueur1.nbBle = NbBle;
                joueur1.nbBois = NbBois;
                joueur1.nbGem = NbGem;
                //on le dit au serveur
                envoyerMessage("Ajouter Mana." + NbBle + "." + NbBois + "." + NbGem);
                //on attend et on laisse le joueur faire des action
                StartCoroutine(waitEnvoyer(0.75f));
            }
        }
            //les boutons pour ajouter la mana sont disabled sinon
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width / 1.27f, Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), bleAjouter);
            GUI.Button(new Rect((Screen.width / 1.17f), Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), boisAjouter);
            GUI.Button(new Rect((Screen.width / 1.08f), Screen.height / 1.12f, Screen.width * 0.05f, Screen.height * 0.07f), gemAjouter);

            GUI.enabled = true;
            GUI.Label(new Rect(Screen.width / 1.27f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Blé: " + NbBle.ToString());
            GUI.Label(new Rect(Screen.width / 1.17f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Bois: " + NbBois.ToString());
            GUI.Label(new Rect(Screen.width / 1.08f, Screen.height / 1.04f, Screen.width * 0.09f, Screen.height * 0.07f), "Gemmes: " + NbGem.ToString());
            GUI.Label(new Rect(Screen.width / 1.3f, Screen.height / 1.26f, Screen.width * 0.25f, Screen.height * 0.10f), Worker);
            GUI.Label(new Rect(Screen.width / 1.23f, Screen.height / 1.17f, Screen.width * 0.15f, Screen.height * 0.1f), "Artisan(s): " + NbWorker.ToString());
        }
        afficherIconSpecial();
        //comunication avec le serveur 
        //si on recois ceci on dit que la aprtie est terminer
        if (ReceiveMessage.message == "vous avez gagné" || ReceiveMessage.message == "vous avez perdu")
            gameFini = true;
        //si on n'a pas de thread on en crée un quand ces notre tours le seul message possible est que l'autre joueur soit partie
        if (!gameFini && ReceiveMessage.message == "")
        {
            if (t == null || !t.IsAlive)
            {
                t = new Thread(ReceiveMessage.doWork);
                t.Start();
            }
        }
        //si le message n'est pas vide
        if (ReceiveMessage.message != "")
        {
            //on le traite
            traiterMessagePartie(ReceiveMessage.message.Split(new char[] { '.' }));
        }
    }
    //réduit de 1 le nombre de tour restant pour chaque carte qu'ils ont sleep
    private void descendreSleep(PosZoneCombat[] Zone)
    {
        //pour chaque carte 
        for (int i = 0; i < Zone.Length; ++i)
        {
            //si le nombre de tour != 0 on réduit de 1
            if (Zone[i] != null && Zone[i].EstOccupee && Zone[i].carte != null && Zone[i].carte.perm.estEndormi != 0)
                Zone[i].carte.perm.estEndormi--;
        }
    }
    //habilete de batiment 
    private void setSpecialHability(string[] data)
    {
        //si c'est ajoute on ajoute la mana
        if (data[0] == "Ajoute")
        {
            setManaBonus(data);
        }
    }
    //augmante la mana du joueur
    private void setManaBonus(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteMana = data[2].Split(new char[] {','})[0];
        if (sorteMana == "bois")
            NbBoisEnnemis += num;
        else if (sorteMana == "gem")
            NbGemEnnemis += num;
        else if (sorteMana == "ble")
            NbBleEnnemis += num;
    }
    //vérifie si un joueur est mort
    public void checkIfHeroIsDead()
    {
        //si les 2 = partie nul
        if (HpEnnemi <= 0 && HpJoueur <= 0)
        {
            gameFini = true;
            EstNul = true;
        }
            //moi = jai perdu
        else if (HpJoueur <= 0)
        {
            gameFini = true;
            EstPerdant = true;
        }
            //ennemis = jai gagner
        else if (HpEnnemi <= 0)
        {
            gameFini = true;
            EstGagnant = true;
        }

    }
    //donne la mana qu'il va resevoir
    private int getNumBonus(string laplace)
    {
        char[] tab = { ' ', '+' };
        string nombre = laplace.Trim(tab);
        return int.Parse(nombre);
    }
    //traite les message de la partie(ceux recu su serveur de l'ennemis)
    private void traiterMessagePartie(string[] data)
    {
        //on doit toujours remettre le message a "" car il va le faire plus d'une fois sinon
        switch (data[0])
        {
                //si l'autre joueur part on gagne
            case "JePart":
                gameFini = true;
                EstGagnant = true;
                ReceiveMessage.message = "";
                break;
                //si le joueur ajoute de la mana
            case "AjouterManaEnnemis":
                //on set sa mana a la nouvelle mana
                setManaEnnemis(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
                ReceiveMessage.message = "";
                break;
            case "Tour Commencer":
                //on reset l'armure on dit que l'on peut attaque et faire des action et on augmente le nombre de worker
                MonTour = true;
                placerClick = false;
                resetArmor(ZoneCombat, styleCarteAlliercombat, true);
                resetArmor(ZoneCombatEnnemie, styleCarteEnnemisCombat, false);
                Script_attaque = GetComponent<attaque>();
                Script_attaque.enabled = true;
                //max mana =5 
                if (NbWorkerMax < 5)
                    setWorker(true);
                else
                    setWorker(false);
                //on reduit le sleep, set les créature qui peuvent attaquer et on pige une cartes
                descendreSleep(ZoneCombatEnnemie);
                setpeutAttaquer();
                PigerCarte();
                ReceiveMessage.message = "";
                break;
            case "AjouterCarteEnnemis":
                //si l'ennemis pige une carte on la crée et on la met sur le board
                Carte temp = createCarte(data, 2);
                temp = setHabilete(temp);
                setManaEnnemis(NbBleEnnemis - int.Parse(data[2]), NbBoisEnnemis - int.Parse(data[3]), NbGemEnnemis - int.Parse(data[4]));
                if (temp.perm.specialhability)
                {
                    setSpecialHability(temp.perm.habilityspecial.Split(new char[] { ' ' }));
                }

                /*trouver le back de carte pour prendre son nom e tla detruire pour contruire un prefab de devant de carte avec les stats de la carte*/
                GameObject temps = trouverBackCard();
                int place = TrouverEmplacementCarteJoueur(temps.transform.position, ZoneCarteEnnemie);
                ZoneCarteEnnemie[place].EstOccupee = false;
                //detruit le prefab  back card(celle qui avait dans ses mains)
                Destroy(temps);

                //crée lobjet carte (celle de face)
                string nomtemp = data[1].Insert(data[1].IndexOf("d") + 1, "ennemis");
                GameObject zeCarteEnnemis = GameObject.Find(nomtemp);
                int index = zeCarteEnnemis.name.IndexOf("s");
                string nombre = zeCarteEnnemis.name.Substring(index + 1, zeCarteEnnemis.name.Length - (index + 1));
                //modifie la carte selon la carte qui a été jouée
                int posCombat = placerCarte(zeCarteEnnemis, ZoneCombatEnnemie);
                tabCarteEnnemis[int.Parse(nombre)] = temp;
                setValueFromCard(int.Parse(nombre), zeCarteEnnemis.transform, temp, false);
                //on la set dans la zone ennemis
                ZoneCombatEnnemie[posCombat].carte = temp;
                styleCarteEnnemisCombat[posCombat] = zeCarteEnnemis;
                JouerCarteBoard a = zeCarteEnnemis.GetComponent<JouerCarteBoard>();
                a.EstJouer = true;
                a.EstEnnemie = true;
                ReceiveMessage.message = "";
                break;
            case "Joueur attaquer":
                //l'ennemis attaque notre héros on réduit la vie du héros
                HpJoueur = int.Parse(data[1]);
                ReceiveMessage.message = "";
                audio.PlayOneShot(AttackSound);
                //si on est mort on dit que l'on perd
                if (HpJoueur <= 0)
                {
                    gameFini = true;
                    EstPerdant = true;
                    if (t.IsAlive)
                        t.Abort();
                    ReceiveMessage.message = "";
                }
                break;
            case "Combat Creature":
                //l'ennemis attaque une de nos créature ces cartes recu par le serveur sont déjà modifié alors il faut seulement changer les vrai cartes et leur prefab
                Carte attaque = createCarte(data, 5);
                Carte defenseur = createCarte(data, 16);
                //on trouve les cartes(il faut les changer car la carte qui attaque est ennemis pour moi et vice versa)
                int num = data[3].IndexOf("d");
                data[3] = data[3].Insert(num + 1, "ennemis");
                data[4] = data[4].Replace("ennemis", "");
                //on fait le combat
                combat(attaque, defenseur, int.Parse(data[2]), int.Parse(data[1]), data[3], data[4]);
                audio.PlayOneShot(AttackSound);
                ReceiveMessage.message = "";
                break;
            case "spellwithtarget":
                //fait un sort qui cible quelquechose
                Carte target = null;
                Carte spell = createCarte(data, 3);
                //si la carte n'est pas le hérps on crée la carte recu par le serveur
                if (data[2] != "hero ennemis")
                {
                    target = createCarte(data, 10);
                    setHabilete(target);
                }
                //on reduit la mana 
                setManaEnnemis(NbBleEnnemis - spell.CoutBle, NbBoisEnnemis - spell.CoutBois, NbGemEnnemis - spell.CoutGem);
                int num3 = data[1].IndexOf("d");
                data[1] = data[1].Insert(num3 + 1, "ennemis");
                //si elle donne c'est une de ses créatures a lui
                if (spell.Habilete.Split(new char[] { ' ' })[0] != "Donne")
                {
                    data[2] = data[2].Replace("ennemis", "");
                }
                    //sinon elle touche une de nos créature
                else
                {
                    int num2 = data[1].IndexOf("d");
                    data[2] = data[2].Insert(num2 + 1, "ennemis");
                }
                //on enleve une carte des mains et on la détruit
                GameObject tempss = trouverBackCard();
                int places = TrouverEmplacementCarteJoueur(tempss.transform.position, ZoneCarteEnnemie);
                if(places != -1)
                    ZoneCarteEnnemie[places].EstOccupee = false;
                Destroy(tempss);
                //on montre le sort
                GameObject gamespell = GameObject.Find(data[1]);
                setValueFromCard(gamespell.transform, spell, false);
                GameObject gametarget = GameObject.Find(data[2]);
                gamespell.transform.position = new Vector3(-5.4f, 0.0f, 6.0f);
                //apres 3 seconde on detrui le spell
                Destroy(gamespell, 3);

                TextMesh[] stat = null;
                if (target != null)
                    stat = gametarget.GetComponentsInChildren<TextMesh>();
                //si c'est detruit on detruit le taget
                if (spell.Habilete.Split(new char[] { ' ' })[0] == "Detruit")
                {
                    //on la trouver et on detruit le gameobject
                    ZoneCombat[TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat)].EstOccupee = false;
                    ZoneCombat[TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat)].carte = null;
                    Destroy(gametarget, 1);
                }
                    //si c'est inflige on lui fait du dégat
                else if (spell.Habilete.Split(new char[] { ' ' })[0] == "Inflige")
                {
                    //si il y a une cible
                    if (target != null && target.perm != null)
                    {
                        //on reduit la vie et si elle est supposer mourir on detruit le game object
                        if (target != null && target.perm != null && target.perm.Vie <= 0)
                        {
                            ZoneCombat[TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat)].EstOccupee = false;
                            ZoneCombat[TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat)].carte = null;
                            Destroy(gametarget, 1);
                        }
                            //sinon on réduit la vie
                        else
                        {

                            stat[3].text = target.perm.Armure.ToString();
                            stat[5].text = target.perm.Vie.ToString();
                        }
                    }
                    else
                    {
                        //si c'est l'autre héros on enleve la vie et on vérifie si il y meurt
                        if (data[2] == "hero")
                        {
                            HpEnnemi -= int.Parse(spell.Habilete.Split(new char[] { ' ' })[1]);
                            checkIfHeroIsDead();
                        }
                            //sinon on enleve notre vie et on vérifie si on meurt
                        else
                        {
                            HpJoueur -= int.Parse(spell.Habilete.Split(new char[] { ' ' })[1]);
                            checkIfHeroIsDead();
                        }

                    }

                }
                    //si on endort il y a une cible et on change son nombre de tour endormi pour le nombre de tour du sort
                else if (spell.Habilete.Split(new char[] { ' ' })[0] == "Endort")
                {
                    string[] textHabilete = spell.Habilete.Split(new char[] { ' ' });
                    ZoneCombat[TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat)].carte.perm.estEndormi = int.Parse(textHabilete[textHabilete.Length - 2]);

                }
                    //si c'est transforme on lui change ses stats
                else if (spell.Habilete.Split(new char[] { ' ' })[0] == "Transforme")
                {
                    //on trouve la carte
                    int positionCarteEnJeu = TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombat);
                    if (positionCarteEnJeu != -1 && ZoneCombat[positionCarteEnJeu] != null && ZoneCombat[positionCarteEnJeu].carte != null && ZoneCombat[positionCarteEnJeu].carte.perm != null)
                    {
                        //on change les basic stat et les autres stats, mais il garde ses habileté
                        ZoneCombat[positionCarteEnJeu].carte.perm.Vie = target.perm.Vie;
                        ZoneCombat[positionCarteEnJeu].carte.perm.basicVie = target.perm.basicVie;
                        ZoneCombat[positionCarteEnJeu].carte.perm.Armure = target.perm.Armure;
                        ZoneCombat[positionCarteEnJeu].carte.perm.basicArmor = target.perm.basicArmor;
                        ZoneCombat[positionCarteEnJeu].carte.perm.Attaque = target.perm.Attaque;
                        ZoneCombat[positionCarteEnJeu].carte.perm.basicAttaque = target.perm.basicAttaque;
                        ZoneCombat[positionCarteEnJeu].carte.perm.estAttaqueDouble = target.perm.estAttaqueDouble;
                        ZoneCombat[positionCarteEnJeu].carte.perm.estAttaquePuisante = target.perm.estAttaquePuisante;
                        ZoneCombat[positionCarteEnJeu].carte.perm.estInvisible = target.perm.estInvisible;
                        ZoneCombat[positionCarteEnJeu].carte.perm.estTaunt = target.perm.estTaunt;

                    }
                    //on change sur le prefab
                    stat[3].text = target.perm.Armure.ToString();
                    stat[4].text = target.perm.Attaque.ToString();
                    stat[5].text = target.perm.Vie.ToString();

                }
                    //si c'est donne on augmente s'est stats selon qu'est que le spell augmente
                else if (spell.Habilete.Split(new char[] { ' ' })[0] == "Donne")
                {
                    //on trouve la carte
                    int positionCarteEnJEu = TrouverEmplacementCarteJoueur(gametarget.transform.position, ZoneCombatEnnemie); string buffHabilete = "";
                    for (int i = 0; i < spell.Habilete.Split(new char[] { ' ' }).Length; i++)
                    {
                        //on regarde chaque mot pour trouver qu'est que la carte donne selon ce quelle donne on l'ajoute
                        if (spell.Habilete.Split(new char[] { ' ' })[i] == "et")
                        {
                            if (spell.Habilete.Split(new char[] { ' ' })[i + 2] == "double" || spell.Habilete.Split(new char[] { ' ' })[i + 2] == "puissante")
                                buffHabilete = spell.Habilete.Split(new char[] { ' ' })[i + 1] + ' ' + spell.Habilete.Split(new char[] { ' ' })[i + 2];
                            else
                                buffHabilete = spell.Habilete.Split(new char[] { ' ' })[i + 1];
                        }
                    }
                    //provocation
                    if (buffHabilete == "provocation")
                    {
                        if (ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estInvisible)
                            ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estInvisible = false;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estTaunt = true;
                    }
                        //attaque puissante
                    else if (buffHabilete == "attaque puissante")
                    {
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estAttaquePuisante = true;
                    }
                        //attaque double
                    else if (buffHabilete == "attaque double")
                    {
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estAttaqueDouble = true;
                    }
                        //invisible
                    else if (buffHabilete == "invisible")
                    {
                        if (ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estTaunt)
                            ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estTaunt = false;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.estInvisible = true;
                    }
                    //si sa change attque armure vie
                    if (positionCarteEnJEu != -1 && ZoneCombatEnnemie[positionCarteEnJEu] != null && ZoneCombatEnnemie[positionCarteEnJEu].carte != null && ZoneCombatEnnemie[positionCarteEnJEu].carte.perm != null)
                    {
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.Vie = target.perm.Vie;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.basicVie = target.perm.basicVie;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.Armure = target.perm.Armure;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.basicArmor = target.perm.basicArmor;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.Attaque = target.perm.Attaque;
                        ZoneCombatEnnemie[positionCarteEnJEu].carte.perm.basicAttaque = target.perm.basicAttaque;
                    }

                    stat[3].text = target.perm.Armure.ToString();
                    stat[4].text = target.perm.Attaque.ToString();
                    stat[5].text = target.perm.Vie.ToString();
                    //afficher un !!!!!!!!  de taunt if there's taunt!
                }

                ReceiveMessage.message = "";
                break;
            case "spellNoTarget":
                //sort qui touche une certaine zone
                Carte spell2 = createCarte(data, 2);

                bool valide = false;
                //donne le type de target
                string typeTarget = trouverTypeTarget(spell2.Habilete.Split(new char[] { ' ' }));
                //réduit le mana du joueur
                setManaEnnemis(NbBleEnnemis - spell2.CoutBle, NbBoisEnnemis - spell2.CoutBois, NbGemEnnemis - spell2.CoutGem);
                //modifie le nom carte
                num = data[1].IndexOf("d");
                data[1] = data[1].Insert(num + 1, "ennemis");
                //trouve la carte ennemis(backcard) dans la main du joueur
                GameObject tempsss = trouverBackCard();
                int placess = TrouverEmplacementCarteJoueur(tempsss.transform.position, ZoneCarteEnnemie);
                ZoneCarteEnnemie[placess].EstOccupee = false;
                //on le détruit
                Destroy(tempsss);
                //montre le sort et le détruit apres 3 seconde
                GameObject gamespell2 = GameObject.Find(data[1]);
                setValueFromCard(gamespell2.transform, spell2, false);
                gamespell2.transform.position = new Vector3(-5.4f, 0.0f, 6.0f);
                Destroy(gamespell2, 3);
                //si inflige
                if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Inflige")
                {
                    string tabHabileteSpellSansNewline = spell2.Habilete.Replace('\n', ' ');
                    int dmg = int.Parse(tabHabileteSpellSansNewline.Split(new char[] { ' ' })[1]);
                    //si ce n'est pas le héros
                    if (typeTarget != "herosennemis")
                    {
                        //pour chaque carte
                        for (int i = 0; i < ZoneCombat.Length; i++)
                        {
                            valide = false;
                            //si c'est toute la zone on le fait sur les ennemis
                            if (typeTarget == "touteslescartes" || typeTarget == "touslesbatiments" || typeTarget == "placedecombat")
                            {
                                if (styleCarteEnnemisCombat[i] != null)
                                {
                                    //pour chaque carte valide
                                    //on fait le degat
                                    valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                                    if (valide)
                                        doTheDmg(dmg, i, ZoneCombatEnnemie, styleCarteEnnemisCombat[i]);
                                }
                            }
                            //sinon seulement sur nos carte
                            if (styleCarteAlliercombat[i] != null)
                            {
                                //pour chaque carte valide
                                //on fait le degat
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                                if (valide)
                                    doTheDmg(dmg, i, ZoneCombat, styleCarteAlliercombat[i]);
                            }
                        }
                    }
                    //si le hros est ciblé
                    if (typeTarget == "placedecombat" || typeTarget == "ennemis" || typeTarget == "herosennemis")
                        if (typeTarget == "placedecombat")
                        {
                            //on réduit la vie du héros allié et ennemis
                            HpEnnemi -= dmg;
                            HpJoueur -= dmg;
                            checkIfHeroIsDead();
                        }
                        else
                        {
                            //on réduit notre vie car on recois le spell
                            HpJoueur -= dmg;
                            checkIfHeroIsDead();
                        }
                }
                //si détruit
                else if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Detruit")
                {
                    for (int i = 0; i < ZoneCombat.Length; i++)
                    {
                        //meme chose que inflige mais on détruit au lieu de faire du dégat
                        valide = false;
                        if (typeTarget == "touteslescartes" || typeTarget == "touslesbatiments" || typeTarget == "touteslescreatures")
                        {
                            if (styleCarteEnnemisCombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                                if (valide)
                                    doDestruction(styleCarteEnnemisCombat[i], i, ZoneCombatEnnemie);
                            }
                        }
                        if (styleCarteAlliercombat[i] != null)
                        {
                            valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                            if (valide)
                                doDestruction(styleCarteAlliercombat[i], i, ZoneCombat);
                        }
                    }
                }
                //si transforme
                else if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Transforme")
                {
                    string tabHabileteSpellSansNewline = spell2.Habilete.Replace('\n', ' ');
                    string[] tabHabileteSpell = tabHabileteSpellSansNewline.Split(new char[] { ' ' });
                    string[] statsTransforme = tabHabileteSpell[tabHabileteSpell.Length - 1].Split(new char[] { '/' });
                    //meme chose que inflige mais avec transforme
                    for (int i = 0; i < ZoneCombat.Length; i++)
                    {
                        valide = false;
                        if (typeTarget == "touteslescartes" || typeTarget == "touslesbatiments" || typeTarget == "touteslescreatures")
                        {
                            if (transformEnnemis(typeTarget) && styleCarteEnnemisCombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                                if (valide)
                                    doTransformation(styleCarteEnnemisCombat[i], i, ZoneCombatEnnemie, statsTransforme);
                            }

                            if (transformAllies(typeTarget) && styleCarteAlliercombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                                if (valide)
                                    doTransformation(styleCarteAlliercombat[i], i, ZoneCombat, statsTransforme);
                            }
                        }
                        else if (typeTarget == "touteslescartesalliees" || typeTarget == "touslesbatimentsallies" || typeTarget == "touteslescreaturesalliees")
                        {
                            if (transformAllies(typeTarget) && styleCarteAlliercombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                                if (valide)
                                    doTransformation(styleCarteAlliercombat[i], i, ZoneCombat, statsTransforme);
                            }
                        }
                        else if (typeTarget == "touteslescartesennemies" || typeTarget == "touslesbatimentsennemis" || typeTarget == "touteslescreaturesennemies")
                        {
                            if (transformEnnemis(typeTarget) && styleCarteEnnemisCombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                                if (valide)
                                    doTransformation(styleCarteEnnemisCombat[i], i, ZoneCombatEnnemie, statsTransforme);
                            }
                        }
                    }
                }
                //si endort
                else if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Endort")
                {
                    //afficher icon ZZzz
                    string tabHabileteSpellSansNewline = spell2.Habilete.Replace('\n', ' ');
                    int nbTours = int.Parse(tabHabileteSpellSansNewline.Split(new char[] { ' ' })[tabHabileteSpellSansNewline.Split(new char[] { ' ' }).Length - 2]);
                    //meme chose que inflige mais avec endort
                    for (int i = 0; i < ZoneCombat.Length; i++)
                    {
                        valide = false;
                        if (typeTarget == "touteslescreatures" || typeTarget == "touteslescartes" || typeTarget == "touslesbatiments")
                        {
                            if (styleCarteEnnemisCombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                                if (valide)
                                    doSleep(i, ZoneCombatEnnemie, nbTours);
                            }
                        }
                        if (styleCarteAlliercombat[i] != null)
                        {
                            valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                            if (valide)
                                doSleep(i, ZoneCombat, nbTours);
                        }
                    }
                }
                //si donne
                else if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Donne")
                {
                    string tabHabileteSpellSansNewline = spell2.Habilete.Replace('\n', ' ');
                    string[] tabHabileteSpell = tabHabileteSpellSansNewline.Split(new char[] { ' ' });
                    for (int i = 0; i < ZoneCombatEnnemie.Length; i++)
                    {
                        valide = false;
                        //presque la meme chose seulement on touche les créature ennemis de base et si ça touche toute la place de combat on le fait sur nos créature
                        if (typeTarget == "touteslescreatures" || typeTarget == "touteslescartes" || typeTarget == "touslesbatiments")
                        {
                            if (styleCarteAlliercombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                                if (valide)
                                    doBuffShitUp(i, ZoneCombat, tabHabileteSpell);
                            }
                        }
                        if (styleCarteEnnemisCombat[i] != null)
                        {
                            valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                            if (valide)
                                doBuffShitUp(i, ZoneCombatEnnemie, tabHabileteSpell);
                        }
                    }
                }
                //si soigne(pour l'instant aucune carte)
                else if (spell2.Habilete.Split(new char[] { ' ' })[0] == "Soigne")
                {
                    string tabHabileteSpellSansNewline = spell2.Habilete.Replace('\n', ' ');
                    int nbHeal = int.Parse(tabHabileteSpellSansNewline.Split(new char[] { ' ' })[1]);
                    //meme chose que donne mais avec soigne
                    for (int i = 0; i < ZoneCombatEnnemie.Length; i++)
                    {
                        valide = false;
                        if (typeTarget == "touteslescreatures" || typeTarget == "touslesbatiments" || typeTarget == "placedecombat" || typeTarget == "touteslescartes")
                        {
                            if (styleCarteAlliercombat[i] != null)
                            {
                                valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombat);
                                if (valide)
                                    doHealingStuff(i, nbHeal, ZoneCombatEnnemie);
                            }
                        }
                        if (styleCarteEnnemisCombat[i] != null)
                        {
                            valide = checkIfValide(typeTarget, i, spell2.Habilete.Split(new char[] { ' ' })[0], ZoneCombatEnnemie);
                            if (valide)
                                doHealingStuff(i, nbHeal, ZoneCombatEnnemie);
                        }
                    }
                }

                ReceiveMessage.message = "";
                break;
            case "Carte manquante":
                //l'ennemis manque de carte
                HpEnnemi = 0;
                gameFini = true;
                break;
        }
        if (data[0] == "Carte manquante")
        {
            EstGagnant = true;
        }
    }
    //vérifie qu'est quele sort cible(pour transformer ennemis)
    private bool transformEnnemis(string typeTarget)
    {
        //si c'est un batiment, une carte ou une créature ennemis
        return typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsallies" || typeTarget == "touteslescartes" ||
            typeTarget == "touteslescartesalliees" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesalliees";
    }
    //vérifie qu'est quele sort cible(pour transformer allie)
    private bool transformAllies(string typeTarget)
    {
        //si c'est un batiment, une carte ou une créature allie
        return typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsennemis" || typeTarget == "touteslescartes" ||
            typeTarget == "touteslescartesennemis" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesennemies";
    }
    //fait du dégat si c'est placedecombat ou ennemis
    private void checkIfDmgHeroes(string typeTarget, int dmg)
    {
        if (typeTarget == "placedecombat" || typeTarget == "ennemis")
        {
            if (typeTarget == "placedecombat")
            {
                HpEnnemi -= dmg;
                HpJoueur -= dmg;
            }
            else
                HpEnnemi -= dmg;
        }
    }
    //fait du dégat a la cible
    private void doTheDmg(int dmg, int i, PosZoneCombat[] zone, GameObject t)
    {
        //réduit l'armure(si le dégat n'enleve pas toute l'armure)
        if ((zone[i].carte.perm.Armure - dmg) >= 0)
            zone[i].carte.perm.Armure -= dmg;
        else
        {
            //sinon on trouve cmb de dégat on va faire sur la vie(attaque - armure) et on réduit la vie selon ce chiffre. on met aussi larmure a 0
            int difference = dmg - zone[i].carte.perm.Armure;
            zone[i].carte.perm.Armure = 0;
            zone[i].carte.perm.Vie -= difference;
            //si elle meurt on detrui le gameobject
            if (zone[i].carte.perm.Vie <= 0)
            {
                Destroy(t, 1);
                zone[i].EstOccupee = false;
            }
        }
        //si il y a uen target (encore) oon change ses stats
        if (t != null && zone[i].carte != null)
        {
            TextMesh[] zeStat = t.GetComponentsInChildren<TextMesh>();
            zeStat[3].text = zone[i].carte.perm.Armure.ToString();
            zeStat[5].text = zone[i].carte.perm.Vie.ToString();
        }
    }
    //endort la créature pour un nombre de tour x
    private void doSleep(int i, PosZoneCombat[] zone, int nbTours)
    {
        zone[i].carte.perm.estEndormi = nbTours;
    }
    //fait que le joeur ne peut faire aucune action(appelé lorsqu'il fait une action pour laisser le temps au serveur de finir l'action)
    public void waitForActionDone()
    {
        Jouer.MonTour = false;
    }
    //laisse le joueur faire ses spells
    public void restart()
    {
        Jouer.MonTour = true;
    }
    //sleep en unity (avec restart)
    public IEnumerator waitEnvoyer(float i)
    {
        yield return new WaitForSeconds(i);
        restart();
    }
    //augment la puissance de la créature
    private void doBuffShitUp(int pos, PosZoneCombat[] zone, string[] tabHabileteSpell)
    {
        //on trouve ce que le spell fait
        TextMesh[] zeStat = null;
        string buffHabilete = ""; int buffVie = 0; int buffArmure = 0; int buffAttaque = 0;
        for (int i = 1; i < tabHabileteSpell.Length; i++)
        {
            //si il y a plus d'un augmentation pour le sort
            if (tabHabileteSpell[i] == "et")
            {
                if (tabHabileteSpell[i + 2] == "double" || tabHabileteSpell[i + 2] == "puissante")
                    buffHabilete = tabHabileteSpell[i + 1] + ' ' + tabHabileteSpell[i + 2];
                else
                    buffHabilete = tabHabileteSpell[i + 1];
            }
                //si il y a un + alors on augment attaque armure vie
            else if (tabHabileteSpell[i][0] == '+')
            {
                if (tabHabileteSpell[i + 1] == "pV")
                    buffVie = int.Parse(tabHabileteSpell[i].Split(new char[] { '+' })[1]);
                else if (tabHabileteSpell[i + 1] == "pAtt")
                    buffAttaque = int.Parse(tabHabileteSpell[i].Split(new char[] { '+' })[1]);
                else if (tabHabileteSpell[i + 1] == "pArm")
                    buffArmure = int.Parse(tabHabileteSpell[i].Split(new char[] { '+' })[1]);
            }
        }
        //on set les nouvelles valeur la carte
        zone[pos].carte.perm.Vie += buffVie;
        zone[pos].carte.perm.basicVie += buffVie;
        zone[pos].carte.perm.Attaque += buffAttaque;
        zone[pos].carte.perm.basicAttaque += buffAttaque;
        zone[pos].carte.perm.Armure += buffArmure;
        zone[pos].carte.perm.basicArmor += buffArmure;
        //on les changes sur la carte(gameobject)
        if (zone == ZoneCombat)
            zeStat = styleCarteAlliercombat[pos].GetComponentsInChildren<TextMesh>();
        else
            zeStat = styleCarteEnnemisCombat[pos].GetComponentsInChildren<TextMesh>();
        zeStat[3].text = zone[pos].carte.perm.Armure.ToString();
        zeStat[4].text = zone[pos].carte.perm.Attaque.ToString();
        zeStat[5].text = zone[pos].carte.perm.Vie.ToString();

        //si on doit lui rajouter un habileté
        //provocation
        if (buffHabilete == "provocation")
        {
            if (zone[pos].carte.perm.estInvisible)
                zone[pos].carte.perm.estInvisible = false;
            zone[pos].carte.perm.estTaunt = true;
        }
            //attaque puissante
        else if (buffHabilete == "attaque puissante")
        {
            zone[pos].carte.perm.estAttaquePuisante = true;
        }
            //attaque double
        else if (buffHabilete == "attaque double")
        {
            zone[pos].carte.perm.estAttaqueDouble = true;
        }
            //invisible
        else if (buffHabilete == "invisible")
        {
            if (zone[pos].carte.perm.estTaunt)
                zone[pos].carte.perm.estTaunt = false;
            zone[pos].carte.perm.estInvisible = true;
        }
    }
    //si il y a du gain de vie(pour l'instant aucun)
    private void doHealingStuff(int i, int nbHeal, PosZoneCombat[] zone)
    {
        //si la carte va pour gagner plus que sa vie de base on lui donne sa vie de base sinon elle gagne x vie
        if ((zone[i].carte.perm.Vie + nbHeal) >= zone[i].carte.perm.basicVie)
            zone[i].carte.perm.Vie = zone[i].carte.perm.basicVie;
        else
            zone[i].carte.perm.Vie += nbHeal;
        //on modifie le gameobject de la carte
        TextMesh[] stat = styleCarteAlliercombat[i].GetComponentsInChildren<TextMesh>();
        stat[5].text = zone[i].carte.perm.Vie.ToString();
    }
    //s'il y a une transformation
    private void doTransformation(GameObject t, int i, PosZoneCombat[] zone, string[] statsTransforme)
    {
        //on verifie les nouvelles valeurs
        int vieTransformation = int.Parse(statsTransforme[2]); int attaqueTransformation = int.Parse(statsTransforme[1]); int armureTransformation = int.Parse(statsTransforme[0]);
        //on les set sur la carte 
        zone[i].carte.perm.Vie = vieTransformation;
        zone[i].carte.perm.Attaque = attaqueTransformation;
        zone[i].carte.perm.Armure = armureTransformation;
        zone[i].carte.perm.basicVie = vieTransformation;
        zone[i].carte.perm.basicAttaque = attaqueTransformation;
        zone[i].carte.perm.basicArmor = armureTransformation;
        //on modifie sur le gameobject de la carte
        TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
        stat[3].text = zone[i].carte.perm.Armure.ToString();
        stat[4].text = zone[i].carte.perm.Attaque.ToString();
        stat[5].text = zone[i].carte.perm.Vie.ToString();
    }
    //si on détruit la carte
    private void doDestruction(GameObject t, int i, PosZoneCombat[] zone)
    {
        //on detrui le game object de la carte
        Destroy(t, 1);
        zone[i].EstOccupee = false;
    }
    //vérifie si la cible est valide selon le sort
    private bool checkIfValide(string typeTarget, int i, string typeSpell, PosZoneCombat[] zone)
    {
        bool valide = false;
        //si inflige cible possible:
        //ennemis créature batiment et heros 
        //allié créature batiement ou héros
        if (typeSpell == "Inflige")
        {
            if (typeTarget == "cible" || typeTarget == "ennemis" || typeTarget == "touteslescartesennemies" || typeTarget == "touteslescartes" || typeTarget == "placedecombat")
                valide = true;
            else if (typeTarget == "creature" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesennemies")
            {
                if (zone[i].carte.perm.TypePerm == "Creature")
                    valide = true;
            }
            else if (typeTarget == "batiment" || typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsennemis")
                if (zone[i].carte.perm.TypePerm == "Batiment")
                    valide = true;
        }
            //si détruit
            //seulement ennemis
        else if (typeSpell == "Detruit" || typeSpell == "Transforme" || typeSpell == "Endort")
        {
            if (typeTarget == "cible" || typeTarget == "touteslescartesennemies" || typeTarget == "touteslescartes" || typeTarget == "touteslescartesalliees")
                valide = true;
            else if (typeTarget == "creature" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesennemies")
            {
                if (zone[i].carte.perm.TypePerm == "Creature")
                    valide = true;
            }
            else if (typeTarget == "batiment" || typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsennemis")
                if (zone[i].carte.perm.TypePerm == "Batiment")
                    valide = true;
        }
            //si soigne
            //comme inflige
        else if (typeSpell == "Soigne")
        {
            if (typeTarget == "cible" || typeTarget == "ennemis" || typeTarget == "touteslescartesalliees" || typeTarget == "touteslescartes" || typeTarget == "placedecombat")
                valide = true;
            else if (typeTarget == "creature" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesalliees")
            {
                if (zone[i].carte.perm.TypePerm == "Creature")
                    valide = true;
            }
            else
                if (typeTarget == "batiment" || typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsalliees")
                    valide = true;
        }
            //si donne
            //seulement allié
        else if (typeSpell == "Donne")
        {
            if (typeTarget == "cible" || typeTarget == "touteslescartesalliees" || typeTarget == "touteslescartes")
                valide = true;
            else if (typeTarget == "creature" || typeTarget == "touteslescreatures" || typeTarget == "touteslescreaturesalliees")
            {
                if (zone[i].carte.perm.TypePerm == "Creature")
                    valide = true;
            }
            else if (typeTarget == "batiment" || typeTarget == "touslesbatiments" || typeTarget == "touslesbatimentsalliees")
                if (zone[i].carte.perm.TypePerm == "Batiment")
                    valide = true;
        }
        return valide;

    }
    //trouve la premiere carte(gameObject dans les cartes de la main adverses
    private GameObject trouverBackCard()
    {
        GameObject game = null;
        for (int i = 0; game == null && i < tabCarteEnnemis.Length; ++i)
        {
            game = GameObject.Find("cardbackennemis" + i);
        }
        return game;
    }
    //reset l'attaque des créatures pour le tour
    private void setpeutAttaquer()
    {
        for (int i = 0; i < ZoneCombat.Length; ++i)
        {
            if (ZoneCombat[i].carte != null && ZoneCombat[i].carte.perm.TypePerm == "Creature")
            {
                ZoneCombat[i].carte.perm.aAttaque = false;
                ZoneCombat[i].carte.perm.aAttaquerDouble = false;
            }
        }
    }
    //crée un string a partir d'une carte
    private string SetCarteString(Carte temp)
    {
        /*0                    1                     2                   3                      4                      5                    6                     7                            8                   9                         10*/
        return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte + "." + temp.perm.Attaque + "." + temp.perm.Vie + "." + temp.perm.Armure + "." + temp.perm.TypePerm;
    }
    //crée une carte a partir d'un string
    private Carte createCarte(string[] data, int posDepart)
    {
        Carte zeCarte = null;
        //si il y a assez de donnée pour crée une carte
        if (data.Length >= posDepart + 6)
        {
            //on fait la carte
            zeCarte = new Carte(int.Parse(data[posDepart + 6]), data[posDepart + 5], data[posDepart + 4], data[posDepart + 3], int.Parse(data[posDepart]), int.Parse(data[posDepart + 1]), int.Parse(data[posDepart + 2]));
            //si elle est permanent on crée le permanent
            if (zeCarte.TypeCarte == "Permanents" || zeCarte.TypeCarte == "creature" || zeCarte.TypeCarte == "batiment" || zeCarte.TypeCarte == "Permanent")
                zeCarte.perm = new Permanent(data[posDepart + 10], int.Parse(data[posDepart + 7]), int.Parse(data[posDepart + 8]), int.Parse(data[posDepart + 9]));
        }
        return zeCarte;
    }
    //chagne vie/armure des cartes qui se battent
    private void combat(Carte attaquant, Carte ennemi, int posAllier, int posDefenseur, string nomAttaquant, string nomDefenseur)
    {
        //change les stats du gameobject et de la carte
        recevoirDegat(attaquant, posAllier, false, nomAttaquant);
        recevoirDegat(ennemi, posDefenseur, true, nomDefenseur);
        GameObject def = GameObject.Find(nomDefenseur);
        int posDef=-1;
        if(def != null)
             posDef = TrouverEmplacementCarteJoueur(def.transform.position, ZoneCombat);
        //on enleve endormi
        if (posDef != -1)
            ZoneCombat[posDef].carte.perm.estEndormi = 0;
        //si une carte meurt on détruit le gameobject de celle-ci
        if (attaquant.perm.Vie <= 0)
        {
            GameObject temp = GameObject.Find(nomAttaquant);
            Destroy(temp);
        }
        if (ennemi.perm.Vie <= 0)
        {
            GameObject temp = GameObject.Find(nomDefenseur);
            Destroy(temp);
        }
    }
    //change attaque armure et vie d'un permanent selon un tableau de stat
    void setStat(Permanent perm, int[] stat)
    {
        perm.Attaque = stat[0];
        perm.Vie = stat[1];
        perm.Armure = stat[2];
    }
    //cahnge les stats du gameobject et celle de la carte
    public void recevoirDegat(Carte carte, int pos, bool allier, string nom)
    {
        GameObject t = null;
        if (carte != null)
        {
            //si c'est allié
            if (allier)
            {
                //on trouve la position de la carte dans le tab de gameobject
                int posi = nom.IndexOf("d");
                string position = nom.Substring(posi + 1, nom.Length - (posi + 1));
                t = GameObject.Find("card" + position);
                //ensuite avec le gameobject on trouve la créacture ou baitment
                int poscreature = TrouverEmplacementCarteJoueur(t.transform.position, ZoneCombat);
                //on réduit sa vie
                if (poscreature != -1 && ZoneCombat[poscreature] != null && ZoneCombat[poscreature].carte != null && ZoneCombat[poscreature].carte.perm != null)
                    ZoneCombat[poscreature].carte.perm.Vie = carte.perm.Vie;
                //on change son armure et sa vie
                t = GameObject.Find("armure" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vie" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
            else
            {
                //meme chose seulement celle-ci est pour l'ennemis
                int posi = nom.IndexOf("s");
                string position = nom.Substring(posi + 1, nom.Length - (posi + 1));
                t = GameObject.Find("cardennemis" + position);
                int poscreature = TrouverEmplacementCarteJoueur(t.transform.position, ZoneCombatEnnemie);
                if (poscreature != -1 && ZoneCombatEnnemie[poscreature] != null && ZoneCombatEnnemie[poscreature].carte != null && ZoneCombatEnnemie[poscreature].carte.perm != null)
                    ZoneCombatEnnemie[poscreature].carte.perm.Vie = carte.perm.Vie;
                t = GameObject.Find("armureEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vieEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
        }
    }
    //sleep en unity
    public IEnumerator wait(float i)
    {
        yield return new WaitForSeconds(i);
    }
    //remet le nombre de worker au debut du tour
    // si newWorker = true on rajouter un worker
    private void setWorker(bool newWorker)
    {
        if (newWorker)
            NbWorkerMax++;
        NbWorker = NbWorkerMax;
    }
    //change le nombre de mana de l'ennemis selon les 3 variables
    private void setManaEnnemis(int ble, int bois, int gem)
    {
        NbBleEnnemis = ble;
        NbBoisEnnemis = bois;
        NbGemEnnemis = gem;
    }
    //change notre mana selon event la ressource et le type
    public int SetManaAjouter(Event events, int ressource, string typeRessource)
    {
        //si click gauche et que nous avons un worker
        if (events.button == 0 && NbWorker > 0)
        {
            ressource++;
            NbWorker--;
        }
            //si click droite
        else if (events.button == 1 && ressource != 0)
        {
            //si ble
            if (typeRessource == "ble" && joueur1.nbBle <= ressource - 1)
            {
                ressource--;
                NbWorker++;
            }
                //si bois
            else if (typeRessource == "bois" && joueur1.nbBois <= ressource - 1)
            {
                ressource--;
                NbWorker++;
            }
                //si gem
            else if (typeRessource == "gem" && joueur1.nbGem <= ressource - 1)
            {
                ressource--;
                NbWorker++;
            }
        }
        return ressource;
    }
    //remet l'armure a celle de départ de chaque créature
    private void resetArmor(PosZoneCombat[] tab, GameObject[] style, bool allier)
    {
        //pour chaque carte de la créature
        for (int i = 0; i < tab.Length; ++i)
        {
            //si il y a une créature
            if (tab[i] != null && tab[i].carte != null && tab[i].carte.perm != null)
            {
                //on set son armure a celle de base et on le change sur le gameobject
                tab[i].carte.perm.Armure = tab[i].carte.perm.getBasicArmor();
                changestatFromCard(style[i], tab[i].carte);
            }
        }
    }

    //Retourne la premiere position qui est vide (disponible) sur la zone passé en parametre
    public static int TrouverOuPlacerCarte(PosZoneCombat[] Zone)
    {
        int OuPlacerCarte = 0;
        for (int i = 0; i < Zone.Length && Zone[i].EstOccupee; ++i)
        {
            OuPlacerCarte++;
        }
        return OuPlacerCarte;
    }
    //trouve quelle carte piger et le met dans la main du joueur
    private void PigerCarte()
    {
        NbCarteEnMainJoueur = 0;
        for (int i = 0; i < ZoneCarteJoueur.Length; ++i)
        {
            if (ZoneCarteJoueur[i].EstOccupee == true)
            {
                ++NbCarteEnMainJoueur;
            }
        }

        /*il n'y a plus de carte*/
        if (NoCarte >= 40)
        {
            envoyerMessage("Carte manquante");
            //StartCoroutine(wait(1.5f));
            //afficher vous avez perdu
            EstPerdant = true;
            //Application.LoadLevel("Menu");
        }
        /*main pleine*/
        else if (NbCarteEnMainJoueur >= 7)
        {
            //on montre la carte
            styleCarteAllier[ordrePige[NoCarte]].transform.position = new Vector3(-7, -1.2f, 1);
            //on la detruit
            Destroy(styleCarteAllier[ordrePige[NoCarte]], 2.0f);
            tabCarteAllier.CarteDeck[ordrePige[NoCarte]] = null;
            ++NoCarte;
        }
        /*on peut piger*/
        else
        {
            //on trouve ou mettre la carte dans la main
            int OuPlacerCarte = TrouverOuPlacerCarte(ZoneCarteJoueur);
            ZoneCarteJoueur[OuPlacerCarte].EstOccupee = true;
            //on la place
            ZoneCarteJoueur[OuPlacerCarte].carte = tabCarteAllier.CarteDeck[ordrePige[NoCarte]];
            styleCarteAllier[ordrePige[NoCarte]].gameObject.transform.position = ZoneCarteJoueur[OuPlacerCarte].Pos;
            ++NoCarte;
            //on envoye au serveur qu'on a piger
            envoyerMessage("Piger");
            //StartCoroutine(wait(1.5f));
        }
        //réduit le nombre de carte total
        --nbCarteAllier;
    }
    //l'ennemis pige
    public void pigerCarteEnnemis()
    {
        int NbCarteEnMainJoueurEnemis = 0;
        //compte le nombre de carte en main
        for (int i = 0; i < ZoneCarteJoueur.Length; ++i)
        {
            if (ZoneCarteJoueur[i].EstOccupee == true)
            {
                ++NbCarteEnMainJoueurEnemis;
            }
        }
        // il y a de la place
        if (NbCarteEnMainJoueur < 7)
        {
            Transform t = Instantiate(carteBack, new Vector3(0, 0, -100), Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
            GameObject zeCartePiger = t.gameObject;
            zeCartePiger.name = "cardbackennemis" + noCarteEnnemis;
            placerCarte(zeCartePiger, ZoneCarteEnnemie);
            JouerCarteBoard pigerScript = zeCartePiger.GetComponent<JouerCarteBoard>();
            pigerScript.EstEnnemie = true;
            ++noCarteEnnemis;
        }
        //réduit le nombre de carte du joueur
        --nbCarteEnnemis;
    }
    //donne les habiletés au cartes
    private Carte setHabilete(Carte card)
    {
        if (card != null && card.Habilete != "" && card.Habilete != null)
        {
            //on regarde chaque habileté
            string[] data = card.Habilete.Split(new char[] { ',' });
            for (int i = 0; i < data.Length; ++i)
            {
                //pour chaque habileté si ses une des habileté(attaque puissante, attaque double, provocation, rapide, invisible) on dit que la Créature là
                string trimmer = data[i].Trim();
                if (card.esthabileteNormal(data[i].Trim()))
                    card.setHabileteNormal(data[i].Trim());
                else
                {
                    //sinon c'st une habiléspécial entre donne(donne des stats au prochaine créature qui rentre)ou ajoute(ajoute des ressources)
                    string[] zeSpecialHability = data[i].Split(new char[] { ' ' });
                    if (getHabilete(zeSpecialHability[0]) && card != null && card.perm != null)
                    {
                        //on split a chaque mot et on modifie selon ce que nous avons
                        card.perm.specialhability = true;
                        if (card.perm.habilityspecial == "" || card.perm.habilityspecial == null)
                            card.perm.habilityspecial += data[i];
                        else
                            card.perm.habilityspecial += "," +data[i];
                    }
                }
            }
        }
        return card;
    }
    private bool getHabilete(string mot)
    {
        return mot == "Donne" || mot == "Ajoute";
    }
    //trouve ou placer la carte dans la zone
    private int placerCarte(GameObject carte, PosZoneCombat[] zone)
    {
        int PlacementZoneCombat = TrouverOuPlacerCarte(zone);
        if (PlacementZoneCombat != -1 && carte != null && PlacementZoneCombat < zone.Length)
        {
            carte.transform.position = zone[PlacementZoneCombat].Pos;
            zone[PlacementZoneCombat].EstOccupee = true;
        }
        return PlacementZoneCombat;
    }
    //trouve la carte dans la zone
    private int TrouverEmplacementCarteJoueur(Vector3 PosCarte, PosZoneCombat[] Zone)
    {
        for (int i = 0; i < Zone.Length; ++i)
        {
            //lorsqu'il trouve que la position soit celle de la zone[i] il retourne la position
            if (PosCarte.Equals(Zone[i].Pos))
            {
                return i;
            }
        }
        return -1; // -1 pour savoir qu'il ne trouve aucune position (techniquement il devrais toujours retourner un pos valide)
    }
    ///////////-------communication serveur--------///////////////
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
        //StartCoroutine(wait(0.5f));
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
    private Carte ReceiveCarte(Socket client)
    {
        Carte carte = null;
        try
        {
            byte[] buffer = new byte[client.SendBufferSize];
            int bytesRead = client.Receive(buffer);
            byte[] formatted = new byte[bytesRead];
            BinaryFormatter receive = new BinaryFormatter();

            for (int i = 0; i < bytesRead; i++)
            {
                formatted[i] = buffer[i];
            }
            using (var recstream = new MemoryStream(formatted))
            {
                carte = receive.Deserialize(recstream) as Carte;
            }

        }
        catch (TimeoutException)
        {
            Console.Write("Erreur de telechargement des données");

        }
        return carte;
    }
    private void EnvoyerCarte(Socket client, Carte carte)
    {
        byte[] data;
        BinaryFormatter b = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            b.Serialize(stream, carte);
            data = stream.ToArray();
        }
        client.Send(data);
        //StartCoroutine(wait(0.5f));
    }
    private Deck ReceiveDeck(Socket client)
    {
        Deck zeDeck = null;
        try
        {
            byte[] buffer = new byte[client.SendBufferSize];
            int bytesRead = client.Receive(buffer);
            byte[] formatted = new byte[bytesRead];
            BinaryFormatter receive = new BinaryFormatter();

            for (int i = 0; i < bytesRead; i++)
            {
                formatted[i] = buffer[i];
            }
            using (var recstream = new MemoryStream(formatted))
            {
                zeDeck = receive.Deserialize(recstream) as Deck;
            }

        }
        catch (TimeoutException) { Console.Write("Erreur de telechargement des données"); }
        return zeDeck;
    }
    ///////////-------fin communication serveur--------///////////////
    //trouve si la cible est une créature un batiment etc
    private string trouverTypeTarget(string[] motsHabilete)
    {
        string target = ""; int i = 0;
        bool pasTrouver = true;
        while (pasTrouver && i < motsHabilete.Length)
        {
            //pour chaque mot
            //si c'est une de celle ci on a trouver la cible et on a la targer
            if (motsHabilete[i] == "cible" || motsHabilete[i] == "creature" || motsHabilete[i] == "batiment" || motsHabilete[i] == "ennemis")
            {
                target = motsHabilete[i];
                pasTrouver = false;
            }
                //plusieur cible
            else if (motsHabilete[i] == "vos")
            {
                if (motsHabilete[i + 1] == "creatures" || motsHabilete[i + 1] == "batiments" || motsHabilete[i + 1] == " cartes")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1];
                    pasTrouver = false;
                }
            }
                //toute les cibles (allié ou ennemis)
            else if (motsHabilete[i] == "toutes")
            {
                if (motsHabilete[i + 1] == "les" && (motsHabilete[i + 2] == "cartes" || motsHabilete[i + 2] == "creatures"))
                {
                    if (i + 3 < motsHabilete.Length)
                    {
                        if (motsHabilete[i + 3] == "ennemies" || motsHabilete[i + 3] == "alliees")
                        {
                            target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2] + motsHabilete[i + 3];
                            pasTrouver = false;
                        }
                        else
                        {
                            target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                            pasTrouver = false;
                        }
                    }
                    else
                    {
                        target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                        pasTrouver = false;
                    }
                }
            }
            //tous (batiment)
            else if (motsHabilete[i] == "tous")
            {
                if (motsHabilete[i + 1] == "les" && motsHabilete[i + 2] == "batiments")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                    pasTrouver = false;
                }
            }
                //héros
            else if (motsHabilete[i] == "heros")
            {
                if (motsHabilete[i + 1] == "ennemis")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1];
                    pasTrouver = false;
                }
            }
                //place de combat donc toute
            else if (motsHabilete[i] == "place")
            {
                if (motsHabilete[i + 1] == "de" && motsHabilete[i + 2] == "combat")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                    pasTrouver = false;
                }
            }
            ++i;
        }
        return target;
    }
}
