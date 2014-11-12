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
            if (EstEnnemie)
                Jouer.ZoneCombatEnnemie[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombatEnnemie)].EstOccupee = false;
            else
            {
                int PlacementZoneCombat = TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombat);
                Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = false;
                if (Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
                {
                    string[] zeSpecialHability = Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.habilityspecial.Split(new char[] { ' ' });
                    if (zeSpecialHability[0] == "Donne")
                    {
                        enleverBonusStat(zeSpecialHability);
                    }
                }
            }
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
    void OnMouseDown(){
        if (getNbCarteZone(Jouer.ZoneCombat)<Jouer.ZoneCombat.Length&&Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            if (Cout[8].text != "Sort")
            {
                int PlacementZoneCombat = Jouer.TrouverOuPlacerCarte(Jouer.ZoneCombat);
                Vector3 temp = this.transform.position;
                this.transform.position = Jouer.ZoneCombat[PlacementZoneCombat].Pos;
                EstJouer = true;
                int Emplacement = TrouverEmplacementCarteJoueur(temp, Jouer.ZoneCarteJoueur);
                if (Emplacement != -1)
                {
                    Jouer.ZoneCarteJoueur[Emplacement].EstOccupee = false;
                    Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = true;
                    Jouer.ZoneCombat[PlacementZoneCombat].carte = Jouer.ZoneCarteJoueur[Emplacement].carte;
                    Jouer.ZoneCarteJoueur[Emplacement].carte = null;
                    Jouer.styleCarteAlliercombat[PlacementZoneCombat] = this.gameObject;
                    if (Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
                    {
                        string[] zeSpecialHability = Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.habilityspecial.Split(new char[] { ' ' });
                        if (zeSpecialHability[0] == "Donne")
                        {
                            setStatbonus(zeSpecialHability);
                            setStat(Jouer.ZoneCombat,PlacementZoneCombat);
                        }
                        else if (zeSpecialHability[0] == "Ajoute")
                            setManaBonus(zeSpecialHability);
                    }
                    else
                    {
                        setStat(Jouer.ZoneCombat,PlacementZoneCombat);
                    }
                    envoyerMessage("Jouer Carte." + this.name);
                    wait(1);
                    EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCombat[PlacementZoneCombat].carte);
                }
            }
            else
            {
                /*faire habileté de la carte*/

                /*détruire la carte*/
                Destroy(this);
            }
        }
	}
    private void enleverBonusStat(string [] data)
    {
        int num = getNumBonus(data[1]);
        string sorteAttaque = data[2];
        enevlerStat(Jouer.ZoneCombat,sorteAttaque,num);
    }
    private void enevlerStat(PosZoneCombat[] zone, string stat,int valeur)
    {
        if (stat == "pV")
            Jouer.vieBonus -= valeur;
        else if (stat == "pAtt")
            Jouer.attaqueBonus -= valeur;
        else if (stat == "pArm")
            Jouer.armureBonus -= valeur;        
    }
    private void setStatbonus(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteAttaque = data[2];
        setBonusStat(Jouer.ZoneCombat,sorteAttaque,num);
    }
    private void setBonusStat(PosZoneCombat[] zone, string stat,int valeur)
    {
        if (stat == "pV")
            Jouer.vieBonus += valeur;
        else if (stat == "pAtt")
            Jouer.attaqueBonus += valeur;
        else if (stat == "pArm")
            Jouer.armureBonus += valeur;
    }
    private void setStat(PosZoneCombat[] zone,int pos)
    {
        GameObject temp=null;
        if(Jouer.ZoneCombat[pos].carte != null)
           temp = getGameObjet(Jouer.styleCarteAllier, zone, pos);
        if (temp != null)
            setStats(temp,zone[pos].carte,Jouer.attaqueBonus,Jouer.armureBonus,Jouer.vieBonus);
    }
    private void setStats(GameObject a,Carte c,int attaqueB,int armureB,int vieB)
    {
        TextMesh[] stat = a.GetComponentsInChildren<TextMesh>();
        c.perm.Armure += armureB;
        c.perm.Attaque += +attaqueB;
        c.perm.Vie += vieB;
        /*armure*/
        stat[3].text = c.perm.Armure.ToString();
        /*attaque*/
        stat[4].text = c.perm.Attaque.ToString();
        /*vie*/
        stat[5].text = c.perm.Vie.ToString();

    }
    private GameObject getGameObjet(GameObject[] tab, PosZoneCombat[] carte,int pos)
    { 
        for(int i=0;i<tab.Length;++i)
        {
            if (tab[i] != null && tab[i].transform.position.Equals(carte[pos].Pos))
                return tab[i].gameObject;
        }
        return null;
    }
    private void setManaBonus(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteMana = data[2];
        if (sorteMana == "bois")
            Jouer.NbBois += num;
        else if (sorteMana == "gem")
            Jouer.NbGem += num;
        else if (sorteMana == "ble")
            Jouer.NbBle += num;
    }
    private int getNumBonus(string laplace)
    {
        char[] tab = { ' ', '+' };
        string nombre = laplace.Trim(tab);
        return int.Parse(nombre);
    }
    private bool getHabilete(string mot)
    {
        return mot == "Donne" || mot == "Ajoute";
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
}
