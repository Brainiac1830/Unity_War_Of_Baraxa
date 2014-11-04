﻿using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using warsofbaraxa;

public class Jouer : MonoBehaviour {
    //variable
    static public Joueur joueur1;
    ThreadLire ReceiveMessage;
    Thread t;
    bool gameFini = false;
    static public PosZoneCombat[] ZoneCarteJoueur;
    static public PosZoneCombat[] ZoneCombat;
    static public GameObject[] styleCarteAlliercombat;
    static public PosZoneCombat[] ZoneCombatEnnemie;
    static public GameObject[] styleCarteEnnemisCombat;
    static public PosZoneCombat[] ZoneCarteEnnemie;
    public int NbCarteEnMainJoueur;
    public bool placerClick;
    public Texture2D Test;
    public Texture2D ble;
    public Texture2D bois;
    public Texture2D gem;
    public Texture2D Worker;
    public Texture2D PlayerChar;
    public Texture2D EnnemiChar;
    public static int NbBle; //test avec static
    public static int NbBois; // test avec static
    public static int NbGem; // test avec statics
    public int NbWorkerMax;
    public int NbWorker;
    public int NbBleEnnemis;
    public int NbBoisEnnemis;
    public int NbGemEnnemis;
    public int HpJoueur;
    public int HpEnnemi;
    public Transform PlacementCarte;
    public GameObject card;
    public GameObject cardennemis;
    public JouerCarteBoard ScriptEnnemie;
    public int NoCarte;
    static public float pos;
    static public Carte[] tabCarteAllier;
    static public GameObject[] styleCarteAllier;
    static public Carte[] tabCarteEnnemis;
    static public GameObject[] styleCarteEnnemis;
    static public bool MonTour;
	//initialization
	void Start () {
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
        NbBle = 0;
        NbBois = 0;
        NbGem = 0;
        NbBleEnnemis = 0;
        NbBoisEnnemis = 0;
        NbGemEnnemis = 0;
        NbWorkerMax = 2;
        NbWorker = NbWorkerMax;
        placerClick = false;
        //deck
        tabCarteAllier = new Carte[40];
        styleCarteAllier = new GameObject[40];
        //deck ennemis
        tabCarteEnnemis = new Carte[40];
        styleCarteEnnemis = new GameObject[40];
        //board
        styleCarteAlliercombat = new GameObject[7];
        styleCarteEnnemisCombat = new GameObject[7];
        joueur1 = new Joueur("player1");
        CarteDepart();
	}
    void OnDestroy()
    {
        t.Abort();
    }
    void Awake()
    {
        string message = recevoirResultat();
        if (message == "Premier Joueur")
            MonTour = true;
        else
            MonTour = false;
    }
    public void InitZoneJoueur()
    {
        ZoneCarteJoueur = new PosZoneCombat[7];
		float pos = 0;
        for (int i = 0; i < ZoneCarteJoueur.Length; ++i)
        {
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

	public void CarteDepart(){
        int pos = 0;
        float posi = 0;
		while (NoCarte<7) {
             Transform t = Instantiate(PlacementCarte, ZoneCarteJoueur[pos].Pos, Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
             Transform Ennemis = Instantiate(PlacementCarte, ZoneCarteEnnemie[pos].Pos, Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;

             ZoneCarteJoueur[pos].EstOccupee = true;
             ZoneCarteEnnemie[pos].EstOccupee = true;


             card = t.gameObject;
             cardennemis = Ennemis.gameObject;

             ScriptEnnemie = Ennemis.GetComponent<JouerCarteBoard>();
             ScriptEnnemie.EstEnnemie = true;

             card.name = "card" + NoCarte.ToString();
             cardennemis.name = "cardennemis" + NoCarte.ToString();

             foreach (Transform  child in t)
             {
                 child.name = child.name + NoCarte;
                 child.tag = "textStats";
             }
             foreach (Transform child in Ennemis)
             {
                 child.name = child.name+"Ennemis" + NoCarte;
                 child.tag = "textStats";
             }
             if (NoCarte == 2)
             {
                 tabCarteAllier[NoCarte] = new Carte(1, "card" + NoCarte, "Permanent", "attaque puissante", 1, 1, 0);
                 tabCarteAllier[NoCarte].perm = new Permanent("creature", 1, 2, 1);

                 tabCarteEnnemis[NoCarte] = new Carte(1, "cardennemis" + NoCarte, "Permanent", "attaque puissante", 0, 0, 0);
                 tabCarteEnnemis[NoCarte].perm = new Permanent("creature", 0, 2, 1);
             }
             else
             {
                 tabCarteAllier[NoCarte] = new Carte(1,"card"+ NoCarte, "Permanent","", 0, 0, 0);
                 tabCarteAllier[NoCarte].perm = new Permanent("creature", 30, 1, 1);

                 tabCarteEnnemis[NoCarte] = new Carte(1, "cardennemis" + NoCarte, "Permanent","", 0, 0, 0);
                 tabCarteEnnemis[NoCarte].perm = new Permanent("creature", 1, 1, 1);
             }
             tabCarteAllier[NoCarte]=setHabilete(tabCarteAllier[NoCarte]);
             setValue(NoCarte, NoCarte, t, true);
             
             tabCarteEnnemis[NoCarte]=setHabilete(tabCarteEnnemis[NoCarte]);
             setValue(NoCarte, NoCarte, Ennemis, false);

             styleCarteAllier[NoCarte] = card;
             styleCarteEnnemis[NoCarte] = cardennemis;
             ZoneCarteJoueur[pos].carte = tabCarteAllier[pos];
             ZoneCarteEnnemie[pos].carte = tabCarteEnnemis[pos];
             ++pos;
             posi += 1.5f;
             ++NoCarte;
        }
	}
    private void setValue(int i,int pos,Transform t,bool allier)
    {
        if (allier)
        {
            t.Find("coutBois" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].CoutBois.ToString();
            t.Find("coutBle" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].CoutBle.ToString();
            t.Find("coutGem" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].CoutGem.ToString();
            t.Find("attaque" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].perm.Attaque.ToString();
            t.Find("armure" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].perm.Armure.ToString();
            t.Find("vie" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].perm.Vie.ToString();
            t.Find("habilete" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].Habilete;
            t.Find("type" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].perm.TypePerm;
        }
        else 
        {
            t.Find("coutBoisEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].CoutBois.ToString();
            t.Find("coutBleEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].CoutBle.ToString();
            t.Find("coutGemEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].CoutGem.ToString();
            t.Find("attaqueEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].perm.Attaque.ToString();
            t.Find("armureEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].perm.Armure.ToString();
            t.Find("vieEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].perm.Vie.ToString();
            t.Find("habileteEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[pos].Habilete;
            t.Find("typeEnnemis" + i).GetComponent<TextMesh>().text = tabCarteAllier[pos].perm.TypePerm.ToString(); 
        }
    }
	// Update is called once per frame
	void Update () {
	}
    //affichage (refresh per frame)
    public void OnGUI(){
	    Event e;
	    e=Event.current;

		//Héro Joueur
		GUI.Label(new Rect(Screen.width*0.045f,Screen.height*0.72f,Screen.width*1.0f, Screen.height*1.0f),"Vie: " + HpJoueur.ToString());
		//Héro Ennemi
        GUI.Label(new Rect(Screen.width * 0.90f, Screen.height * 0.0001f, Screen.width * 1.0f, Screen.height * 1.0f), "Vie: " + HpEnnemi.ToString());

        //BTN EndTurn
        if (placerClick && MonTour)
        {
            if (GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Fini"))
            {
                envoyerMessage("Fin De Tour");
                MonTour = false;
                resetArmor(ZoneCombat);
                resetArmor(ZoneCombatEnnemie);
            }
        }
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Fini");
            GUI.enabled = true;
        }
	    //blé
	    GUI.Label(new Rect(Screen.width*0.005f,Screen.height*0.005f,Screen.width*0.05f, Screen.height*0.07f),ble);
	    GUI.Label(new Rect(Screen.width*0.005f,Screen.height*0.07f,Screen.width*0.09f, Screen.height*0.07f),"Blé: " + NbBleEnnemis.ToString());
	    //bois
	    GUI.Label(new Rect(Screen.width*0.06f,Screen.height*0.005f,Screen.width*0.05f,Screen.height*0.07f),bois);
	    GUI.Label(new Rect(Screen.width*0.06f,Screen.height*0.07f,Screen.width*0.09f,Screen.height*0.07f),"Bois: " + NbBoisEnnemis.ToString());
	    //gem
	    GUI.Label(new Rect(Screen.width*0.14f,Screen.height*0.005f,Screen.width*0.05f,Screen.height*0.07f),gem);
	    GUI.Label(new Rect(Screen.width*0.14f,Screen.height*0.07f,Screen.width*0.09f,Screen.height*0.07f),"Gem: " + NbGemEnnemis.ToString());
	    if(!placerClick && MonTour){
		    //BLE
	        if(GUI.Button(new Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),ble)){
                NbBle = SetManaAjouter(e, NbBle);
		    }
		    GUI.Label(new Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "Blé: "+ NbBle.ToString());
		    //BOIS
		    if(GUI.Button(new Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),bois)){
                NbBois = SetManaAjouter(e, NbBois);
		    }
		    GUI.Label(new Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09f, Screen.height*0.07f), "Bois: " + NbBois.ToString());
		    //GEM
		    if(GUI.Button(new Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),gem)){
                NbGem = SetManaAjouter(e, NbGem);
		    }
		    GUI.Label(new Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "Gem: " + NbGem.ToString());
		    //WORKER
		    GUI.Label(new Rect(Screen.width/1.3f,Screen.height/1.24f,Screen.width*0.25f, Screen.height*0.10f),Worker);
		    GUI.Label(new Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15f, Screen.height*0.1f),"Worker: " + NbWorker.ToString());
		
		    if(GUI.Button(new Rect(Screen.width/1.12f,Screen.height/1.24f,Screen.width*0.1f, Screen.height*0.1f),"Placer") && NbWorker==0){
			    placerClick=true;
                joueur1.nbBle = NbBle;
                joueur1.nbBois = NbBois;
                joueur1.nbGem = NbGem;
                envoyerMessage("Ajouter Mana." + NbBle + "." + NbBois + "." + NbGem);
		    }
	    }
	    else{
		    GUI.enabled=false;
		    GUI.Button(new Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),ble);
		    GUI.Button(new Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),bois);
		    GUI.Button(new Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),gem);
		
		    GUI.enabled=true;
		    GUI.Label(new Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "Blé: "+ NbBle.ToString());
		    GUI.Label(new Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09f, Screen.height*0.07f), "Bois: " + NbBois.ToString());
		    GUI.Label(new Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "Gem: " + NbGem.ToString());
		    GUI.Label(new Rect(Screen.width/1.3f,Screen.height/1.24f,Screen.width*0.25f, Screen.height*0.10f),Worker);
		    GUI.Label(new Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15f, Screen.height*0.1f),"Worker: " + NbWorker.ToString());		
	    }
        if (!MonTour)
        {
            if (ReceiveMessage.message == "vous avez gagné" || ReceiveMessage.message == "vous avez perdu")
                gameFini = true;
            if (t == null || !t.IsAlive && !gameFini)
            {
                t = new Thread(ReceiveMessage.doWork);
                t.Start();
            }
            attaque s = GetComponent<attaque>();
            s.enabled = false; 
            traiterMessagePartie(ReceiveMessage.message.Split(new char[] { '.' }));           
        }
    }
    private void traiterMessagePartie(string[] data)
    {
        switch(data[0])
        {
            case "AjouterManaEnnemis":
                setManaEnnemis(int.Parse(data[1]),int.Parse(data[2]),int.Parse(data[3]));
                ReceiveMessage.message = "";
            break;
            case "Tour Commencer":
                MonTour = true;
                placerClick = false;
                attaque s = GetComponent<attaque>();
                s.enabled = true; 
                //max mana =5
                if (NbWorkerMax < 5)
                    setWorker(true);
                else
                    setWorker(false);
                setpeutAttaquer();
                PigerCarte();

                ReceiveMessage.message = "";
            break;
            case "AjouterCarteEnnemis":
                Carte temp=createCarte(data,1);
                temp=setHabilete(temp);
                setManaEnnemis(NbBleEnnemis-int.Parse(data[1]),NbBoisEnnemis - int.Parse(data[2]),NbGemEnnemis - int.Parse(data[3]));
                temp.NomCarte=temp.NomCarte.Insert(temp.NomCarte.Length - 1, "ennemis");
                GameObject zeCarteEnnemis = GameObject.Find(temp.NomCarte);
                int pos = TrouverEmplacementCarteJoueur(zeCarteEnnemis.transform.position,ZoneCarteEnnemie);
                int posCombat=placerCarte(zeCarteEnnemis, ZoneCombatEnnemie);
                ZoneCombatEnnemie[posCombat].carte = temp;
                styleCarteEnnemisCombat[posCombat] = zeCarteEnnemis;
                JouerCarteBoard a = zeCarteEnnemis.GetComponent<JouerCarteBoard>();
                a.EstJouer = true;
                a.EstEnnemie = true;
                ReceiveMessage.message = "";
            break;
            case "Joueur attaquer":
                HpJoueur = int.Parse(data[1]);
                ReceiveMessage.message = "";
            break;
            case "Combat Creature":
                Carte attaque = createCarte(data,3);
                Carte defenseur = createCarte(data, 14);
                changeName(attaque, defenseur);
                combat(attaque, defenseur,int.Parse(data[2]),int.Parse(data[1]));
                ReceiveMessage.message = "";
            break;
            //case "Piger":
            //    Carte tempPiger = createCarte(data, 1);
            //    tempPiger.NomCarte = tempPiger.NomCarte.Insert(tempPiger.NomCarte.Length - 1, "ennemis");
            //    GameObject zeCartePiger = GameObject.Find(tempPiger.NomCarte);
            //    placerCarte(zeCartePiger, ZoneCarteEnnemie);
            //    int emplacement = TrouverEmplacementCarteJoueur(zeCartePiger.transform.position, ZoneCarteEnnemie);
            //    ZoneCarteEnnemie[emplacement].EstOccupee = true;
            //    ReceiveMessage.message = "";
            //    //A faire!
            //break;
        }
        if (data[0] == "vous avez gagné")
        {
            Application.LoadLevel("Menu");
        }
        else if(data[0] == "vous avez perdu")
        {
            Application.LoadLevel("Menu");
        }
    }
    private void setpeutAttaquer()
    {
        for (int i = 0; i < ZoneCombat.Length; ++i)
        {
            if (ZoneCombat[i].carte != null && ZoneCombat[i].EstOccupee && ZoneCombat[i].carte.perm.TypePerm == "creature")
            { 
                ZoneCombat[i].carte.perm.aAttaque = false;
                ZoneCombat[i].carte.perm.aAttaquerDouble = false;
            }
        }
    }
    private string SetCarteString(Carte temp)
    {
                   /*0                    1                     2                   3                      4                      5                    6                     7                            8                   9                         10*/
        return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte + "." + temp.perm.Attaque + "." + temp.perm.Vie + "." + temp.perm.Armure + "." + temp.perm.TypePerm;
    }
    private void changeName(Carte attaque, Carte defense)
    {
        attaque.NomCarte = attaque.NomCarte.Insert(attaque.NomCarte.Length - 1, "ennemis");
        defense.NomCarte = defense.NomCarte.Replace("ennemis", "");
    }
    private Carte createCarte(string [] data,int posDepart)
    {
        Carte zeCarte=null;
        zeCarte = new Carte(int.Parse(data[posDepart + 6]), data[posDepart + 5], data[posDepart + 4], data[posDepart + 3], int.Parse(data[posDepart]), int.Parse(data[posDepart + 1]), int.Parse(data[posDepart + 2]));
        if (zeCarte.TypeCarte == "Permanents" || zeCarte.TypeCarte == "creature" || zeCarte.TypeCarte == "batiment" || zeCarte.TypeCarte == "Permanent")
            zeCarte.perm = new Permanent(data[posDepart + 10], int.Parse(data[posDepart + 7]), int.Parse(data[posDepart + 8]), int.Parse(data[posDepart + 9]));
        return zeCarte;
    }
    private void combat(Carte attaquant, Carte ennemi,int posAllier,int posDefenseur)
    {
        recevoirDegat(attaquant, posAllier, true);
        recevoirDegat(ennemi, posDefenseur, false);
        if (attaquant.perm.Vie <= 0)
        {
            GameObject temp = GameObject.Find(attaquant.NomCarte);
            Destroy(temp);
        }
        if (ennemi.perm.Vie <= 0)
        {
            GameObject temp = GameObject.Find(ennemi.NomCarte);
            Destroy(temp);            
        }
    }
    void setStat(Permanent perm, int[] stat)
    {
        perm.Attaque = stat[0];
        perm.Vie = stat[1];
        perm.Armure = stat[2];
    }
    public void recevoirDegat(Carte carte, int pos, bool allier)
    {
        GameObject t = null;
        if (carte != null)
        {
            string position = carte.NomCarte.Substring(carte.NomCarte.Length - 1, 1);
            if (allier)
            {
                t = GameObject.Find("armure" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vie" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
            else
            {
                t = GameObject.Find("armureEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vieEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
        }
    }
    public IEnumerator wait(int i)
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
    private void setManaEnnemis(int ble, int bois,int gem)
    {
        NbBleEnnemis = ble;
        NbBoisEnnemis = bois;
        NbGemEnnemis = gem;
    }
    public int SetManaAjouter(Event events, int ressource)
    {
	    if(events.button==0 && NbWorker>0){
		    ressource++;
		    NbWorker--;
	    }
	    else if(events.button==1 && ressource!=0 && NbWorker< NbWorkerMax){
		    ressource--;
		    NbWorker++;
	    }
	    return ressource;
    }
    private void resetArmor(PosZoneCombat [] tab)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            if(tab[i] !=null && tab[i].carte !=null && tab[i].carte.perm != null)
            tab[i].carte.perm.Armure = tab[i].carte.perm.getBasicArmor();
        }
    }

    //Retourne la premiere position qui est vide (disponible) sur la zone passé en parametre
    public static int TrouverOuPlacerCarte(PosZoneCombat[] Zone)
    {
        int OuPlacerCarte = 0;
        for (int i = 0; i < Zone.Length && Zone[i].EstOccupee == true; ++i)
        {
            OuPlacerCarte++;
        }
        return OuPlacerCarte;
    }

    private void PigerCarte()
    {
        NbCarteEnMainJoueur = 0;
        for(int i=0;i<ZoneCarteJoueur.Length;++i)
        {
            if(ZoneCarteJoueur[i].EstOccupee == true)
            {
                ++NbCarteEnMainJoueur;
            }
        }

        if(NbCarteEnMainJoueur == 7)
        {
            //Faire afficher la carte au joueur mais elle disparait puisque la main est pleine
        }
        else
        {
            int OuPlacerCarte = TrouverOuPlacerCarte(ZoneCarteJoueur);
            Transform t = Instantiate(PlacementCarte, ZoneCarteJoueur[OuPlacerCarte].Pos, Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
            card = t.gameObject;
            card.name = "card" + NoCarte.ToString();
            foreach (Transform child in t)
            {
                child.name = child.name + NoCarte;
                child.tag = "textStats";
            }

            //à modifier (Verifier si la carte est un batiment ou creature ou spell)
            tabCarteAllier[OuPlacerCarte] = new Carte(1, "card" + NoCarte, "Permanent", "", 0, 0, 0);
            tabCarteAllier[OuPlacerCarte].perm = new Permanent("creature", 30, 1, 1);
            /*set habilete*/
            setHabilete(tabCarteAllier[OuPlacerCarte]);
            setValue(NoCarte,OuPlacerCarte, t, true);              
            ZoneCarteJoueur[OuPlacerCarte].EstOccupee = true;
            styleCarteAllier[OuPlacerCarte] = card;
            ++NoCarte;
            envoyerMessage("Piger." + SetCarteString(tabCarteAllier[OuPlacerCarte]));
        }
    }
    private Carte setHabilete(Carte card)
    {
        if (card.Habilete != "" && card.Habilete != null)
        {
            string[] data = card.Habilete.Split(new char[] {','});
            for (int i = 0; i < data.Length; ++i)
            {
                if (card.esthabileteNormal(data[i]))
                    card.setHabileteNormal(data[i]);
                else
                { 
                    /*set habilete special*/
                }
            }
        }
        return card;
    }
    private int placerCarte(GameObject carte,PosZoneCombat[] zone)
    {
        int PlacementZoneCombat = TrouverOuPlacerCarte(zone);
        carte.transform.position = zone[PlacementZoneCombat].Pos;
        zone[PlacementZoneCombat].EstOccupee = true;
        return PlacementZoneCombat;
    }
    private int TrouverEmplacementCarteJoueur(Vector3 PosCarte, PosZoneCombat[] Zone)
    {
        int Emplacement = 0;
        for (int i = 0; i < Zone.Length; ++i)
        {
            if (PosCarte.Equals(Zone[i].Pos))
            {
                return Emplacement;
            }
            else
            {
                ++Emplacement;
            }
        }
        return -1; // -1 pour savoir qu'il ne trouve aucune position (techniquement il devrais toujours retourner un pos valide)
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
            recevoirResultat();
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
        catch(TimeoutException ex) { Console.Write("Erreur de telechargement des données"); }
        return carte;
    }
}
