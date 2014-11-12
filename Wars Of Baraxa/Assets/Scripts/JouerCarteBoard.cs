using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using warsofbaraxa;


public class JouerCarteBoard : MonoBehaviour {
	public float delay;
    public Jouer Script_Jouer;
    Permanent Target;
    public bool EstJouer = false;
    public bool EstEnnemie = false;
    GameObject target;
    GameObject spell;
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
            if(EstEnnemie)
                Jouer.ZoneCombatEnnemie[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombatEnnemie)].EstOccupee = false;
            else
                Jouer.ZoneCombat[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombat)].EstOccupee = false;
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
    private int getNbCarteZone(PosZoneCombat[] zone)
    {
        int nbCarte=0;
        for (int i = 0; i < zone.Length; ++i)
        {
            if (zone[i] != null && zone[i].EstOccupee)
                ++nbCarte;
        }
        return nbCarte;
    }

    private void spellSummon(string target, int nbCreatures, int[] stats)
    {

    }
    private void spellBurn(string target, int nbDegat, Carte zeTarget)
    {
    }
    private void spellHeal(string target, int nbVie, Carte zeTarget)
    {

    }
    private void spellSleep(string target, int nbTour, Carte zeTarget)
    {

    }
    private void spellDestroy(string target, Carte zeTarget)
    {

    }
    private void spellTransform(string target, int[] stats, Carte zeTarget)
    {
    }
    private void spellBuff(string target, string habilete, Carte zeTarget)
    {
        int buffVie = 0; int buffAttaque = 0; int buffArmure = 0;
        char[] pasDeLettres = { '+', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        char[] pasDeChiffres = { '+', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        string[] sansEspace = habilete.Split(new char[] { ' ' });
        for (int i = 1; i < sansEspace.Length; i++)
        {
            if (sansEspace[i] == "et")
            {
                string buffHabilete = sansEspace[i + 1];
            }
            else if (sansEspace[i][0] == '+')
            {
                string nombre = sansEspace[i].Trim(pasDeLettres);
                string attribut = sansEspace[i].Trim(pasDeChiffres);
                if (attribut == "pV")
                    buffVie = int.Parse(nombre);
                else if (attribut == "pAtt")
                    buffAttaque = int.Parse(nombre);
                else if (attribut == "pArm")
                    buffArmure = int.Parse(nombre);
            }
        }
    }


    private bool Selectionable(GameObject style, PosZoneCombat[] Zone)
    {
        bool selectionable = false;
        for (int i = 0; i < Zone.Length; ++i)
        {
            if (style.transform.position.Equals(Zone[i].Pos))
            {
                selectionable = true;
            }
        }
        return selectionable;
    }
    private string trouverTypeTarget(string[] motsHabilete)
    {
        string target = "";
        for (int i = 0; i < motsHabilete.Length; i++)
        {
            if (isTarget(motsHabilete[i]))
            {
                if (motsHabilete[i] == "toutes" && motsHabilete[i + 1] == "les" && motsHabilete[i + 2] == "créatures")
                    target = "toutes les créatures";
                else if (motsHabilete[i] == "tous" && motsHabilete[i + 1] == "les" && motsHabilete[i + 2] == "ennemis")
                    target = "tous les ennemis";
                else if (motsHabilete[i] == "toutes" && motsHabilete[i + 1] == "vos" && motsHabilete[i + 2] == "créatures")
                    target = "toutes vos créatures";
                else
                    target = motsHabilete[i];
            }
        }
            return target;
    }
    private int trouverNbDegat(string[] motsHabilete)
    {
        //Inflige 2 points de dégat à votre cible
        int nombre = 0;

        nombre = int.Parse(motsHabilete[1]);

        return nombre;
    }
    private int trouverNbSummon(string[] motsHabilete)
    {
        //Invoque 2 singes 1/0/1
        int nombre = 0;

        nombre = int.Parse(motsHabilete[1]);

        return nombre;
    }
    private int trouverNbVie(string[] motsHabilete)
    {
        // Soigne votre cible de 2 pV
        int nombre = 0;

        nombre = int.Parse(motsHabilete[motsHabilete.Length - 2]);

        return nombre;
    }
    private int[] trouverStats(string[] motsHabilete)
    {
        //Transforme votre cible en 0/1/0
        //Invoque 2 singes 1/0/0
        int[] stats = {0,0,0};
        int vie = 0; int attaque = 0; int armure = 0;

        string[] tabStat = motsHabilete[motsHabilete.Length - 1].Split(new char[] {'/'});
        vie = int.Parse(tabStat[0]); attaque = int.Parse(tabStat[1]); armure = int.Parse(tabStat[2]);
        stats[0] = vie; stats[1] = attaque; stats[2] = armure;

        return stats;
    }
    private int trouverNbTour(string[] motsHabilete)
    {
        int nombre = 0;

        nombre = int.Parse(motsHabilete[motsHabilete.Length - 2]);

        return nombre;
    }
    private string trouverTypeEffet(string[] motsHabilete)
    {
        string effet = "";
        
        for (int i = 0; i < motsHabilete.Length; i++)
        {
            if (isEffet(motsHabilete[i]))
            {
                effet = motsHabilete[i];
            }
        }
        return effet;
    }
    private void trouverHabilite(string zHabilete)
    {
        string[] motsHabilete = zHabilete.Split(new char[] { ' ' });
        string effet ="";
        string[] stats = zHabilete.Split(new char[] { '+' });
        effet = trouverTypeEffet(motsHabilete);
    }
    private bool isEffet(string mot)
    {
        return mot == "invoque" || mot == "dégat" || mot == "dégats" || mot == "soigne" || mot == "endort" || mot == "transforme" || mot == "donne";
    }
    private bool isTarget(string mot)
    {        // une seul target ----- Héros ---- Créatures et batiments--------Créatures---------Bâtiments--------T
        return mot == "cible" || mot == "héros" || mot == "cartes" || mot == "créature" || mot == "bâtiment" || mot == "toutes" ;
    }
    private bool isEnnemis(string mot)
    {
        return mot == "dégat" || mot == "endort" || mot == "transforme";
    }
    private bool isATargetNeeded(string mot)
    {
        return mot == "cible" || mot == "créature" || mot == "bâtiment";
    }

    void OnMouseDown(){
        int Emplacement = 0;
        if (Cout[8].text == "Sort" && Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            bool targetNeeded = false;
            string effet = "";
            string spellTarget = "";
            string[] texteHabiliteSansEspace;
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            spell = this.gameObject;
            int pos = TrouverEmplacementCarteJoueur(this.transform.position,Jouer.ZoneCarteJoueur);
            this.transform.position = new Vector3(-5.4f, 0.0f, 6.0f);
            EstJouer=true;
            Jouer.ZoneCarteJoueur[pos].EstOccupee=false;
            //Split
            texteHabiliteSansEspace = Cout[7].text.Split(new char[] { ' ' });
            //effet habilité
            effet = trouverTypeEffet(Cout[7].text.Split(new char[] { ' ' }));
            //target qui recoit le spell
            spellTarget = trouverTypeTarget(Cout[7].text.Split(new char[] { ' ' }));

            if (isATargetNeeded(trouverTypeTarget(texteHabiliteSansEspace)))
                targetNeeded = true;
            else
                targetNeeded = false;

            if(targetNeeded)
            {
                bool targetTrouve = false;
                int posTarget;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit carte;
                while(!targetTrouve)
                {
                    if (Physics.Raycast(ray, out carte) && Selectionable(GameObject.Find(carte.collider.gameObject.name), Jouer.ZoneCombat))
                    {
                        target = GameObject.Find(carte.collider.gameObject.name);
                        if (isEnnemis(texteHabiliteSansEspace[0]))
                            posTarget = TrouverEmplacementCarteJoueur(target.transform.position, Jouer.ZoneCombatEnnemie);
                        else
                            posTarget = TrouverEmplacementCarteJoueur(target.transform.position, Jouer.ZoneCombat);
                        /*permanents car est sur le board*/
                        if (posTarget != -1)
                        {

                            if (effet == "Infliger")
                                spellBurn(spellTarget, trouverNbDegat(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                            else if (effet == "Endort")
                                spellSleep(spellTarget, trouverNbTour(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                            else if (effet == "Transforme")
                                spellTransform(spellTarget, trouverStats(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                            else if (effet == "Détruit")
                                spellDestroy(spellTarget, Jouer.ZoneCombatEnnemie[posTarget].carte);
                            else if (effet == "Soigne")
                                spellHeal(spellTarget, trouverNbVie(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                            targetTrouve = true;
                        }
                        //burn
                        else if (gameObject.name == "hero ennemis")
                        {
                            if (effet == "Inflige")
                            {
                                spellBurn(spellTarget, trouverNbDegat(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                                targetTrouve = true;
                            }
                        }
                        //heal
                        else if (gameObject.name == "hero") 
                        {
                            if (effet == "Soigne")
                            {
                                spellHeal(spellTarget, trouverNbVie(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte);
                                targetTrouve = true;
                            }
                        }
                    }
                }
            }
            else
            {
                
                if (effet == "Infliger")
                {
                    for(int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellBurn(spellTarget, trouverNbDegat(texteHabiliteSansEspace), Jouer.ZoneCombatEnnemie[i].carte);
                }
                    
                else if (effet == "Invoque")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellSummon(spellTarget, trouverNbSummon(texteHabiliteSansEspace), trouverStats(texteHabiliteSansEspace));
                }
                    
                else if (effet == "Endort")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellSleep(spellTarget, trouverNbTour(texteHabiliteSansEspace), Jouer.ZoneCombat[i].carte);
                }
                    
                else if (effet == "Transforme")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellTransform(spellTarget, trouverStats(texteHabiliteSansEspace), Jouer.ZoneCombat[i].carte);
                }
                    
                else if (effet == "Détruit")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellDestroy(spellTarget, Jouer.ZoneCombat[i].carte);
                }
                    
                else if (effet == "Soigne")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellHeal(spellTarget, trouverNbVie(texteHabiliteSansEspace), Jouer.ZoneCombat[i].carte);
                }
                    

            }

            envoyerMessage("Jouer Carte." + this.name);
            wait(1);
            EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCarteJoueur[pos].carte);
            Jouer.ZoneCarteJoueur[pos].carte = null;


        }
        else if (getNbCarteZone(Jouer.ZoneCombat)<Jouer.ZoneCombat.Length&&Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            //if (Cout[8].text != "Sort")
            //{
                int PlacementZoneCombat = Jouer.TrouverOuPlacerCarte(Jouer.ZoneCombat);
                Vector3 temp = this.transform.position;
                this.transform.position = Jouer.ZoneCombat[PlacementZoneCombat].Pos;
                EstJouer = true;
                Emplacement = TrouverEmplacementCarteJoueur(temp, Jouer.ZoneCarteJoueur);
                if (Emplacement != -1)
                {
                    Jouer.ZoneCarteJoueur[Emplacement].EstOccupee = false;
                    Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = true;
                    Jouer.ZoneCombat[PlacementZoneCombat].carte = Jouer.ZoneCarteJoueur[Emplacement].carte;
                    Jouer.ZoneCarteJoueur[Emplacement].carte = null;
                    Jouer.styleCarteAlliercombat[PlacementZoneCombat] = this.gameObject;
                    envoyerMessage("Jouer Carte." + this.name);
                    wait(1);
                    EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCombat[PlacementZoneCombat].carte);
                }
            //}
            //else
            //{
                /*faire habileté de la carte*/

                /*détruire la carte*/
                //Destroy(this);
            //}
        }
	}

    public IEnumerator wait(int i)
    {
        yield return new WaitForSeconds(i);
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
    }

    int[] getStat(Permanent perm)
    {
        int[] stat = new int[3];
        stat[0] = perm.Attaque;
        stat[1] = perm.Vie;
        stat[2] = perm.Armure;
        return stat;
    }
}
