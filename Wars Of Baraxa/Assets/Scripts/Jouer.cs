using UnityEngine;
using System.Collections;
using warsofbaraxa;

public class Jouer : MonoBehaviour {
    //variable
    public GUIStyle Background;
    Joueur joueur1;
    public bool placerClick;
    public bool tourFinis;
	public Texture2D Test;
    public Texture2D ble;
    public Texture2D bois;
    public Texture2D gem;
    public Texture2D Worker;
	public Texture2D PlayerChar;
	public Texture2D EnnemiChar;
    public int NbBle;
    public int NbBois;
    public int NbGem;
    public int NbWorkerMax;
    public int NbWorker;
    public int NbBleEnnemis;
    public int NbBoisEnnemis;
    public int NbGemEnnemis;
	public int HpJoueur;
	public int HpEnnemi;
	public GameObject CarteTest;
	//initialization
	void Start () {
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
        tourFinis = false;
        joueur1 = new Joueur("player1");
	}
	
	// Update is called once per frame
	void Update () {
        Attaquer();
	}
    //affichage (refresh per frame)
    public void OnGUI(){
	    Event e;
	    e=Event.current;
	    GUI.Box(new Rect(0,0,Screen.width,Screen.height),"",Background);

		//Test pour deplacement Carte
		//CarteTest = new GameObject ("Carte1");

		//Héro Joueur
		GUI.Label(new Rect(Screen.width*0.02f,Screen.height*0.70f,Screen.width*0.3f, Screen.height*0.3f),PlayerChar);
		GUI.Label(new Rect(Screen.width*0.035f,Screen.height*0.69f,Screen.width*1.0f, Screen.height*1.0f),"Vie: " + HpJoueur.ToString());
		//Héro Ennemi
		GUI.Label(new Rect(Screen.width*0.87f,Screen.height*0.00009f,Screen.width*5.0f, Screen.height*0.305f),EnnemiChar);
		GUI.Label(new Rect(Screen.width*0.89f,Screen.height*0.00009f,Screen.width*1.0f, Screen.height*1.0f),"Vie: " + HpEnnemi.ToString());
        //BTN EndTurn
        if (!tourFinis && placerClick)
        {
            if (GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Finnis"))
            {
                tourFinis = true;
            }
        }
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Finnis");
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
	    GUI.Label(new Rect(Screen.width*0.14f,Screen.height*0.07f,Screen.width*0.09f,Screen.height*0.07f),"gem: " + NbGemEnnemis.ToString());
	    if(!placerClick){
		    //BLE
	        if(GUI.Button(new Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),ble)){
                NbBle = SetManaAjouter(e, NbBle);
		    }
		    GUI.Label(new Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "blé: "+ NbBle.ToString());
		    //BOIS
		    if(GUI.Button(new Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),bois)){
                NbBois = SetManaAjouter(e, NbBois);
		    }
		    GUI.Label(new Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09f, Screen.height*0.07f), "bois: " + NbBois.ToString());
		    //GEM
		    if(GUI.Button(new Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),gem)){
                NbGem = SetManaAjouter(e, NbGem);
		    }
		    GUI.Label(new Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "gem: " + NbGem.ToString());
		    //WORKER
		    GUI.Label(new Rect(Screen.width/1.3f,Screen.height/1.24f,Screen.width*0.25f, Screen.height*0.10f),Worker);
		    GUI.Label(new Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15f, Screen.height*0.1f),"Worker: " + NbWorker.ToString());
		
		    if(GUI.Button(new Rect(Screen.width/1.12f,Screen.height/1.24f,Screen.width*0.1f, Screen.height*0.1f),"Placer") && NbWorker==0){
			    placerClick=true;
                joueur1.nbBle = NbBle;
                joueur1.nbBois = NbBois;
                joueur1.nbGem = NbGem;
		    }
	    }
	    else{
		    GUI.enabled=false;
		    GUI.Button(new Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),ble);
		    GUI.Button(new Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),bois);
		    GUI.Button(new Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05f, Screen.height*0.07f),gem);
		
		    GUI.enabled=true;
		    GUI.Label(new Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "blé: "+ NbBle.ToString());
		    GUI.Label(new Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09f, Screen.height*0.07f), "bois: " + NbBois.ToString());
		    GUI.Label(new Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09f, Screen.height*0.07f), "gem: " + NbGem.ToString());
		    GUI.Label(new Rect(Screen.width/1.3f,Screen.height/1.24f,Screen.width*0.25f, Screen.height*0.10f),Worker);
		    GUI.Label(new Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15f, Screen.height*0.1f),"Worker: " + NbWorker.ToString());		
	    }
    }
    public void Attaquer()
    {
        Carte attaquant = new Carte(1,"test","Permanent",1,0,0);
        attaquant.perm =  new Permanent("Creature",4,2,0);
        Carte Defenseur = new Carte(1,"test2", "Permanent", 1, 0, 0);
        Defenseur.perm = new Permanent("Creature", 1, 1, 0);
        Joueur playerDef = new Joueur("Defenseur");
        if(!attaquant.perm.aAttaque)
        {
            
            //if(personne attaquer n'est pas le héros)
            CombatCreature(attaquant.perm, Defenseur.perm);
            CombatCreature(Defenseur.perm, attaquant.perm);
            //else
            playerDef.vie = CombatJoueur(attaquant.perm.Attaque, playerDef.vie);

            //si l'attaquant ou le defenseur n'on plus de vie on les enleve du board
            if (attaquant.perm.Vie <= 0)
                attaquant = null;
            if (Defenseur.perm.Vie <= 0)
                Defenseur = null;
            if (playerDef.vie <= 0)
                playerDef = null;

            attaquant.perm.aAttaque = true;
        }
    }
    private void CombatCreature(Permanent attaquant, Permanent defenseur)
    {
        if (defenseur.Armure - attaquant.Attaque >= 0)
            defenseur.Armure -= attaquant.Attaque;
        else
        {
            int attaque = attaquant.Attaque - defenseur.Armure;
            defenseur.Armure=0;
            defenseur.Vie -= attaque;
        }        
    }
    private int CombatJoueur(int attaque, int vie)
    {
        return vie - attaque;  
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
}
