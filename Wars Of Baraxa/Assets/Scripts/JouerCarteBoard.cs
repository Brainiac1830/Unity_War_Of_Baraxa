using UnityEngine;
using System.Collections;
using warsofbaraxa;

public class JouerCarteBoard : MonoBehaviour {
	public float delay;
    public Jouer Script_Jouer;
    public bool EstJouer = false;

	// Use this for initialization
	void Start () {
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
        if (!EstJouer)
        {
            int PlacementZoneCombat = Jouer.TrouverOuPlacerCarte(Jouer.ZoneCombat);
            Vector3 temp = this.transform.position;
            this.transform.position = Jouer.ZoneCombat[PlacementZoneCombat].Pos;
            EstJouer = true;
            //JouerCarteBoard script = GetComponent<JouerCarteBoard>();
            //script.enabled = false;
            Jouer.ZoneCarteJoueur[TrouverEmplacementCarteJoueur(temp,Jouer.ZoneCarteJoueur)].EstOccupee = false;
            Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = true;
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
