using UnityEngine;
using System.Collections;
using warsofbaraxa;

public class Jouer : MonoBehaviour {
    //variable
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
    public Rect posEnnemis;
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
	public Transform PlacementCarte;
	public GameObject card;
    public GameObject cardennemis;
	public int i;
	static public float pos;
    static public Carte[] tabCarteAllier;
    static public GameObject[] styleCarteAllier;
    static public Carte[] tabCarteEnnemis;
    static public GameObject[] styleCarteEnnemis;
	//initialization
	void Start () {
		i = 0;
		//CarteTest1 = GameObject.Find ("Card");
		pos = 0;
		card = null;
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
        tabCarteAllier = new Carte[5];
        styleCarteAllier = new GameObject[5];
        tabCarteEnnemis = new Carte[5];
        styleCarteEnnemis = new GameObject[5];
        joueur1 = new Joueur("player1");
        posEnnemis = new Rect(Screen.width * 0.87f, Screen.height * 0.00009f, Screen.width * 5.0f, Screen.height * 0.305f);
        CarteDepart();
	}

	public void CarteDepart(){
		float pos = 0;
		while (i<5) {
             Transform t = Instantiate(PlacementCarte, new Vector3(-4.0f + pos, -3.1f, 6.0f), Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
             Transform Ennemis = Instantiate(PlacementCarte, new Vector3(-4.0f+pos,-0.0005f,6.0f),Quaternion.Euler(new Vector3(0,0,0))) as Transform;
             card = t.gameObject;
             cardennemis = Ennemis.gameObject;
             card.name = "card" + i.ToString();
             cardennemis.name = "cardennemis" + i.ToString();
             foreach (Transform  child in t)
             {
                 child.name = child.name + i;
                 child.tag = "textStats";
             }
             foreach (Transform child in Ennemis)
             {
                 child.name = child.name+"Ennemis" + i;
                 child.tag = "textStats";
             }
             pos += 1.5f;
             if (i == 2)
             {
                 tabCarteAllier[i] = new Carte(1, "card" + i, "Permanent", 0, 0, 0);
                 tabCarteAllier[i].perm = new Permanent("batiment", 0, 2, 1);
                 setValue(i, t,true);

                 tabCarteEnnemis[i] = new Carte(1, "cardennemis"+i, "Permanent", 0, 0, 0);
                 tabCarteEnnemis[i].perm = new Permanent("batiment", 0, 2, 1);
                 setValue(i, Ennemis,false);
             }
             else
             {
                 tabCarteAllier[i] = new Carte(1,"card"+i, "Permanent", 0, 0, 0);
                 tabCarteAllier[i].perm = new Permanent("creature", 10, 1, 1);
                 setValue(i, t,true);

                 tabCarteEnnemis[i] = new Carte(1, "cardennemis" + i, "Permanent", 0, 0, 0);
                 tabCarteEnnemis[i].perm = new Permanent("creature", 10, 1, 1);
                 setValue(i, Ennemis,false);
             }
             styleCarteAllier[i] = card;
             styleCarteEnnemis[i] = cardennemis;
             ++i;
        }
	}
    private void setValue(int i,Transform t,bool allier)
    {
        if (allier)
        {
            t.Find("coutBois" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].CoutBois.ToString();
            t.Find("coutBle" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].CoutBle.ToString();
            t.Find("coutGem" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].CoutGem.ToString();
            t.Find("attaque" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].perm.Attaque.ToString();
            t.Find("armure" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].perm.Armure.ToString();
            t.Find("vie" + i).GetComponent<TextMesh>().text = tabCarteAllier[i].perm.Vie.ToString();
        }
        else 
        {
            t.Find("coutBoisEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutBois.ToString();
            t.Find("coutBleEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutBle.ToString();
            t.Find("coutGemEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].CoutGem.ToString();
            t.Find("attaqueEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Attaque.ToString();
            t.Find("armureEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Armure.ToString();
            t.Find("vieEnnemis" + i).GetComponent<TextMesh>().text = tabCarteEnnemis[i].perm.Vie.ToString();            
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
        
	    GUI.Label(new Rect(Screen.width*0.02f,Screen.height*0.70f,Screen.width*0.3f, Screen.height*0.3f),PlayerChar);
		GUI.Label(new Rect(Screen.width*0.035f,Screen.height*0.69f,Screen.width*1.0f, Screen.height*1.0f),"Vie: " + HpJoueur.ToString());
		//Héro Ennemi
		GUI.Label(new Rect(Screen.width*0.85f,Screen.height*0.00009f,Screen.width*1.0f, Screen.height*1.0f),"Vie: " + HpEnnemi.ToString());

        //BTN EndTurn
        if (!tourFinis && placerClick)
        {
            if (GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Finis"))
            {
                tourFinis = true;
                resetArmor(tabCarteAllier);
                resetArmor(tabCarteEnnemis);
            }
        }
        else
        {
            GUI.enabled = false;
            GUI.Button(new Rect(Screen.width * 0.067f, Screen.height * 0.47f, Screen.width * 0.07f, Screen.height * 0.05f), "Finis");
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
    private void resetArmor(Carte [] tab)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            tab[i].perm.Armure = tab[i].perm.getBasicArmor();
        }
    }
}
