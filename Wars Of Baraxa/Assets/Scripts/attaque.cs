using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using warsofbaraxa;
public class attaque : MonoBehaviour {
    bool AttaquantClick;
    public Texture2D taunt;
    public AudioClip Attack;
    Permanent Attaquant;
    Permanent Defenseur;
    GameObject carteAttaque;
    GameObject carteDefense;
    int posAllier=-1;
    int posDefenseur=-1;
    Color Normal;
	// Use this for initialization
	void Start () {
        AttaquantClick = false;
    }
	
	// Update is called once per frame
	void Update () {
        Attaquer();    
    }

    private bool Selectionable(GameObject style,PosZoneCombat[] Zone)
    {
        bool selectionable = false;
        for (int i = 0; i < Zone.Length;++i)
        {
                if (style.transform.position.Equals(Zone[i].Pos))
                {
                    selectionable = true;
                }
        }
            return selectionable;
    }
    public void peutAttaquer(PosZoneCombat[] tab, GameObject [] style)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i].carte != null && style[i] != null && i == posAllier)
            {
                style[i].renderer.material.color = Color.blue;
            }
            else if (tab[i].carte != null && style[i] != null && !tab[i].carte.perm.aAttaque && tab[i].carte.perm.TypePerm == "Creature" && Selectionable(style[i], tab) && tab[i].carte.perm.estEndormi == 0)
            {
                style[i].renderer.material.color = Color.green;
            }

            else if (tab[i].carte != null && style[i] != null)
                style[i].renderer.material.color = Color.white;
        }
    }
    public void peutEtreAttaquer(PosZoneCombat[] tab, GameObject [] style)
    {
        const int taunt = 1;
        int[] peutEtreAttaquer = new int[tab.Length];
        bool esttaunt = foundTaunt(tab,peutEtreAttaquer);
        if (esttaunt)
        {
            for (int i = 0;i < tab.Length; ++i)
            {
                if (peutEtreAttaquer[i] == taunt && Selectionable(style[i], tab))
                    style[i].renderer.material.color = Color.red;
                else if (tab[i].carte != null && style[i] != null)
                    style[i].renderer.material.color = Color.white;
            }
        }
        else
        {
            for (int i = 0; i < tab.Length; ++i)
            {
                if (tab[i].carte != null && style[i] != null && !tab[i].carte.perm.estInvisible && Selectionable(style[i], tab))
                    style[i].renderer.material.color = Color.red;
                else if (tab[i] != null && style[i] != null)
                    style[i].renderer.material.color = Color.white;
            }
        }
    }
    public bool foundTaunt(PosZoneCombat[] tab,int[] peutEtreAttaquer)
    {
        const int taunt = 1;
        bool esttaunt = false;
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i].carte != null && tab[i].carte.perm.estTaunt)
            {
                esttaunt = true;
                peutEtreAttaquer[i] = taunt;
            }
        }
        return esttaunt;
    }
    public bool foundTaunt(PosZoneCombat[] tab)
    {
        const int taunt = 1;
        bool esttaunt = false;
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i].carte != null && tab[i].carte.perm.estTaunt)
                esttaunt = true;
        }
        return esttaunt;
    }
    public void Attaquer()
    {
            //change le border pour une autre couleur
        peutAttaquer(Jouer.ZoneCombat, Jouer.styleCarteAlliercombat);
        peutEtreAttaquer(Jouer.ZoneCombatEnnemie,Jouer.styleCarteEnnemisCombat);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit carte;
            if (Physics.Raycast(ray, out carte) && !AttaquantClick && Selectionable(GameObject.Find(carte.collider.gameObject.name),Jouer.ZoneCombat))
            {
                carteAttaque = GameObject.Find(carte.collider.gameObject.name);
                posAllier = TrouverEmplacementCarteJoueur(carteAttaque.transform.position, Jouer.ZoneCombat);
                if (posAllier != -1 && !Jouer.ZoneCombat[posAllier].carte.perm.aAttaque && Jouer.ZoneCombat[posAllier].carte.perm.TypePerm == "Creature")
                {
                    AttaquantClick = true;
                    Jouer.styleCarteAlliercombat[posAllier].renderer.material.color = Color.blue;
                    int[] stat = getStat(Jouer.ZoneCombat[posAllier].carte.perm);
                    Attaquant = new Permanent("Creature", stat[0], stat[1],stat[2]);
                    
                }
            }
            else if (Physics.Raycast(ray, out carte) && AttaquantClick)
            {
                bool taunt = false;
                carteDefense = GameObject.Find(carte.collider.gameObject.name);
                if (foundTaunt(Jouer.ZoneCombatEnnemie,new int[7]))
                    taunt = true;
                if (Selectionable(GameObject.Find(carte.collider.gameObject.name), Jouer.ZoneCombatEnnemie))
                {
                    int pos = TrouverEmplacementCarteJoueur(carte.transform.position, Jouer.ZoneCombatEnnemie);
                    if (taunt)
                    {
                        if(pos != -1 &&Jouer.ZoneCombatEnnemie[pos].carte.perm.estTaunt)
                            attaqueSomething();
                    }
                    else if(!Jouer.ZoneCombatEnnemie[pos].carte.perm.estInvisible)
                    {
                        attaqueSomething();
                    }
                }
                if (carteDefense!= null && carteDefense.name == "hero ennemis" && !foundTaunt(Jouer.ZoneCombatEnnemie))
                {
                    Jouer.HpEnnemi = CombatJoueur(Jouer.ZoneCombat[posAllier].carte, Jouer.HpEnnemi);
                    if (!Jouer.ZoneCombat[posAllier].carte.perm.estAttaqueDouble || Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble)
                        Jouer.ZoneCombat[posAllier].carte.perm.aAttaque = true;
                    else
                        Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble = true;
                    AttaquantClick = false;
                    audio.PlayOneShot(Attack);
                    envoyerMessage("Attaquer Joueur");
                    wait(1);
                    EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCombat[posAllier].carte);

                    if (Jouer.HpJoueur <= 0 || Jouer.HpEnnemi <= 0)
                    {
                        Jouer.EstGagnant = true;
                        Jouer.gameFini = true;
                    }
                    posAllier = -1;
                    Attaquant = null;
                } 
            }
        }
        else if (Input.GetMouseButtonDown(1) && AttaquantClick )
        {
            AttaquantClick = false;
        }
    }
    private void attaqueSomething()
    {
        posDefenseur = TrouverEmplacementCarteJoueur(carteDefense.transform.position, Jouer.ZoneCombatEnnemie);
        if (posDefenseur != -1 && carteDefense.name != "hero ennemis")
        {
            int[] stat = getStat(Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm);
            AttaquantClick = false;
            Defenseur = new Permanent("Creature", stat[0], stat[1], stat[2]);
            CombatCreature(Attaquant, Defenseur);
            CombatCreature(Defenseur, Attaquant);
            setStat(Jouer.ZoneCombat[posAllier].carte.perm, new int[] { Attaquant.Attaque, Attaquant.Vie, Attaquant.Armure });
            setStat(Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm, new int[] { Defenseur.Attaque, Defenseur.Vie, Defenseur.Armure });
            recevoirDegat(Jouer.ZoneCombat[posAllier].carte,carteAttaque, true);
            recevoirDegat(Jouer.ZoneCombatEnnemie[posDefenseur].carte,carteDefense, false);
            string attaquant = SetCarteString(Jouer.ZoneCombat[posAllier].carte);
            string ennemis = SetCarteString(Jouer.ZoneCombatEnnemie[posDefenseur].carte);
            if (!Jouer.ZoneCombat[posAllier].carte.perm.estAttaqueDouble || Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble)
                Jouer.ZoneCombat[posAllier].carte.perm.aAttaque = true;
            else
                Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble = true;

            if (Attaquant.Vie <= 0)
            {
                kill(carteAttaque);
                Jouer.ZoneCombat[posAllier].carte = null;
            }
            if (Defenseur.Vie <= 0)
            {
                kill(carteDefense);
                Jouer.ZoneCombatEnnemie[posDefenseur].carte = null;
            }
            envoyerMessage("Attaquer Creature." + posAllier + "." + posDefenseur + "."+carteAttaque.name+"."+carteDefense.name+"." + attaquant + "." + ennemis);
            Attaquant = null;
            carteAttaque = null;
            Defenseur = null;
            carteDefense = null;
            posAllier = -1;
            posDefenseur = -1;
            audio.PlayOneShot(Attack);
        }      
    }
    private string SetCarteString(Carte temp)
    {
                  /*0                   1                     2                   3                      4                      5                    6                     7                            8                   9                         10*/
        return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte + "." + temp.perm.Attaque + "." + temp.perm.Vie + "." + temp.perm.Armure + "." + temp.perm.TypePerm;
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
    public IEnumerator wait(int i)
    {
        yield return new WaitForSeconds(i);
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
    private void kill(GameObject carteTemp)
    {
        Destroy(carteTemp/*2ieme argumen est un delais pour */);
    }
    private void CombatCreature(Permanent attaquant, Permanent defenseur)
    {
        int attaque = attaquant.Attaque;
        if (defenseur.Armure - attaque >= 0)
            defenseur.Armure -= attaque;
        else
        {
            int difference = attaque - defenseur.Armure;
            defenseur.Armure = 0;
            defenseur.Vie -= difference;
        }
    }
    private int CombatJoueur(Carte carte, int vie)
    {
        int attaque = carte.perm.Attaque;
        if (carte.perm.estAttaquePuisante)
            attaque *= 2;
        return vie - attaque;
    }
    int[] getStat(Permanent perm)
    {
        int[] stat = new int[3];
        stat[0] = perm.Attaque;
        stat[1] = perm.Vie;
        stat[2] = perm.Armure;           
        return stat;
    }
    void setStat(Permanent perm,int [] stat)
    {
        perm.Attaque = stat[0];
        perm.Vie = stat[1];
        perm.Armure = stat[2];
    }
    public void recevoirDegat(Carte carte,GameObject card,bool allier)
    {
        GameObject t = null;
        if (carte != null)
        {
            if (allier)
            {
                string position = card.name.Substring(card.name.IndexOf("d") + 1, card.name.Length - (card.name.IndexOf("d") + 1));
                t = GameObject.Find("armure" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vie" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
            else
            {
                string position = card.name.Substring(card.name.IndexOf("s") + 1, card.name.Length - (card.name.IndexOf("s") + 1));
                t = GameObject.Find("armureEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vieEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
        }
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
}
