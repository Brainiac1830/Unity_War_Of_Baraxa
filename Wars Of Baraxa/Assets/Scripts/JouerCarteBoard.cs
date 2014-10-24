using UnityEngine;
using System.Collections;
using warsofbaraxa;


public class JouerCarteBoard : MonoBehaviour {
	public float delay;
    public Jouer Script_Jouer;
    public bool EstJouer = false;
    public bool EstEnnemie = false;
    TextMesh[] Cout;
    


	// Use this for initialization
	void Start () {

        Cout = GetComponentsInChildren<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnDestroy()
    {
        if (EstJouer)
        {
            Jouer.ZoneCombat[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombat)].EstOccupee = false;
        }
        else
        {
            Jouer.ZoneCarteJoueur[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCarteJoueur)].EstOccupee = false;
        }
    }

    private int TrouverEmplacementCarteJoueur(Vector3 PosCarte,PosZoneCombat[] Zone)
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
	void OnMouseDown(){
        if (!EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            int PlacementZoneCombat = Jouer.TrouverOuPlacerCarte(Jouer.ZoneCombat);
            Vector3 temp = this.transform.position;
            this.transform.position = Jouer.ZoneCombat[PlacementZoneCombat].Pos;
            EstJouer = true;
            int Emplacement = TrouverEmplacementCarteJoueur(temp, Jouer.ZoneCarteJoueur);
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBle -= System.Int32.Parse(Cout[0].text);
            Jouer.NbBois -= System.Int32.Parse(Cout[1].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);

            if (Emplacement != -1)
            {
                Jouer.ZoneCarteJoueur[Emplacement].EstOccupee = false;
                Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = true;
            }
        }
	}


	void OnMouseOver(){
        //delay += Time.deltaTime;
        //// here the 2 is the time that you want before load the bar
        //if(delay >=0.5f){
        //    //this.transform.position = new Vector3 (0,-1.50f, 6.0f);
        //    this.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        //}
	}

	void OnMouseExit(){
        //delay = 0;
        //this.transform.localScale = new Vector3 (0.1f, 1.0f, 0.13f);
	}
}
