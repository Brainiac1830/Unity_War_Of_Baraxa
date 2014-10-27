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
    Permanent Attaquant;
    Permanent Defenseur;
    GameObject carteAttaque;
    GameObject carteDefense;
    int posAllier;
    int posDefenseur;
    Color Normal;
	// Use this for initialization
	void Start () {
        AttaquantClick = false;
        Normal = Jouer.styleCarteAllier[0].renderer.material.color;
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
    public void peutAttaquer(Carte[] tab, GameObject [] style,PosZoneCombat[] Zone)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i] != null && style[i] != null && !tab[i].perm.aAttaque && tab[i].perm.TypePerm == "creature" && Selectionable(style[i],Zone))
            {
                style[i].renderer.material.color = Color.green;
            }
            else if (tab[i] != null && style[i] != null)
                style[i].renderer.material.color = Color.white;
        }
    }
    public void peutEtreAttaquer(Carte[] tab, GameObject [] style)
    {
        const int taunt = 1;
        int[] peutEtreAttaquer = new int[tab.Length];
        bool esttaunt = foundTaunt(tab,peutEtreAttaquer);
        if (esttaunt)
        {
            for (int i = 0;i < tab.Length; ++i)
            {
                if (peutEtreAttaquer[i] == taunt)
                    style[i].renderer.material.color = Color.red;
                else if (tab[i] != null && style[i] != null)
                    style[i].renderer.material.color = Color.white;
            }
        }
        else
        {
            for (int i = 0; i < tab.Length; ++i)
            { 
                if (tab[i] != null && style[i] != null)
                    style[i].renderer.material.color = Color.red;
                else if (tab[i] != null && style[i] != null)
                    style[i].renderer.material.color = Color.white;
            }
        }
    }
    public bool foundTaunt(Carte[] tab,int[] peutEtreAttaquer)
    {
        const int taunt = 1;
        bool esttaunt = false;
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i] != null && tab[i].perm.estTaunt)
            {
                esttaunt = true;
                peutEtreAttaquer[i] = taunt;
            }
        }
        return esttaunt;
    }
    public void Attaquer()
    {
            //change le border pour une autre couleur
        peutAttaquer(Jouer.tabCarteAllier, Jouer.styleCarteAllier,Jouer.ZoneCombat);
        peutEtreAttaquer(Jouer.tabCarteEnnemis,Jouer.styleCarteEnnemis);
        Jouer script = GetComponent<Jouer>();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit carte;
            if (Physics.Raycast(ray, out carte) && !AttaquantClick && Selectionable(GameObject.Find(carte.collider.gameObject.name),Jouer.ZoneCombat))
            {
                carteAttaque = GameObject.Find(carte.collider.gameObject.name);
                posAllier = getPosCarte(carteAttaque.name, Jouer.tabCarteAllier);
                if (posAllier != -1 && !Jouer.tabCarteAllier[posAllier].perm.aAttaque && Jouer.tabCarteAllier[posAllier].perm.TypePerm == "creature")
                {
                    AttaquantClick = true;
                    int[] stat = getStat(Jouer.tabCarteAllier[posAllier].perm);
                    Attaquant = new Permanent("creature", stat[0], stat[1],stat[2]);                   
                }
            }
            else if (Physics.Raycast(ray, out carte) && AttaquantClick)
            {
                carteDefense = GameObject.Find(carte.collider.gameObject.name);
                posDefenseur = getPosCarte(carteDefense.name, Jouer.tabCarteEnnemis);
                if (posDefenseur != -1 && carteDefense.name != "hero ennemis")
                {
                    int[] stat = getStat(Jouer.tabCarteEnnemis[posDefenseur].perm);
                    AttaquantClick = false;
                    Defenseur = new Permanent("creature", stat[0], stat[1], stat[2]);
                    CombatCreature(Attaquant, Defenseur);
                    CombatCreature(Defenseur, Attaquant);
                    setStat(Jouer.tabCarteAllier[posAllier].perm, new int[] { Attaquant.Attaque, Attaquant.Vie, Attaquant.Armure });
                    setStat(Jouer.tabCarteEnnemis[posDefenseur].perm, new int[] { Defenseur.Attaque, Defenseur.Vie, Defenseur.Armure });
                    recevoirDegat(Jouer.tabCarteAllier[posAllier], posAllier,true);
                    recevoirDegat(Jouer.tabCarteEnnemis[posDefenseur],posDefenseur,false);
                    if (Attaquant.Vie <= 0)
                        kill(carteAttaque);
                    if (Defenseur.Vie <= 0)
                        kill(carteDefense);

                    Attaquant = null;
                    carteAttaque = null;
                    Defenseur = null;
                    carteDefense = null;
                    Jouer.tabCarteAllier[posAllier].perm.aAttaque = true;
                }
                else if (carteDefense.name == "hero ennemis")
                {
                    script.HpEnnemi = CombatJoueur(Attaquant.Attaque, script.HpEnnemi);
                    Jouer.tabCarteAllier[posAllier].perm.aAttaque = true;
                    AttaquantClick = false;
                    envoyerMessage("Attaquer Joueur");
                    wait(1);
                    EnvoyerCarte(connexionServeur.sck, Jouer.tabCarteAllier[posAllier]);

                    Attaquant = null;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) && AttaquantClick )
        {
            AttaquantClick = false;
        }
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
    private int getPosCarte(string nom,Carte[] tab)
    { 
        for (int i =0; i<tab.Length;++i)
        {
            if(tab[i].NomCarte == nom)
                return i;
        }
        return -1;
    }
    private void kill(GameObject carteTemp)
    {
        Destroy(carteTemp/*2ieme argumen est un delais pour */);
    }
    private void CombatCreature(Permanent attaquant, Permanent defenseur)
    {
        if (defenseur.Armure - attaquant.Attaque >= 0)
            defenseur.Armure -= attaquant.Attaque;
        else
        {
            int attaque = attaquant.Attaque - defenseur.Armure;
            defenseur.Armure = 0;
            defenseur.Vie -= attaque;
        }
    }
    private int CombatJoueur(int attaque, int vie)
    {
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
    void recevoirDegat(Carte carte,int pos,bool allier)
    {
        GameObject t = null;
        if (allier)
        {
            t = GameObject.Find("armure" + pos);
            t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
            t = GameObject.Find("vie" + pos);
            t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
        }
        else 
        {
            t = GameObject.Find("armureEnnemis" + pos);
            t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
            t = GameObject.Find("vieEnnemis" + pos);
            t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();            
        }
    }
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
    }
}
