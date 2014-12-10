using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using warsofbaraxa;
public class attaque : MonoBehaviour {
    //savoir si l'atttaquant a été clicke ou non
    bool AttaquantClick;
    //son
    public AudioClip Attack;
    //pour les combat
    Permanent Attaquant;
    Permanent Defenseur;
    GameObject carteAttaque;
    GameObject carteDefense;
    //position de l'attaquant dans le tableau de place de combat allié
    int posAllier=-1;
    //même chose mais dans la place de combat ennemis
    int posDefenseur=-1;
    //trouver le num de la carte(GameOBject)pour pouvoir modifier sa couleur
    int taille;
    int numero;
    Color Normal;
	// Use this for initialization
	void Start () {
        AttaquantClick = false;
    }
	
	// Update is called once per frame
	void Update () {
        //a chaque fois on vérifie si on a clicke sur une créature ou non et si la créature peut attaquer/etre attaquer
        Attaquer();    
    }
    //permet de savoir si la carte est dans la zone de combat
    private bool Selectionable(GameObject style,PosZoneCombat[] Zone)
    {
        bool selectionable = false;
        //pour chaque carte
        for (int i = 0; i < Zone.Length;++i)
        {
            //on vérifie si le GameObject(carte) est a la meme position que la zone de combat[i] si oui on dit quel peut etre selectione sinon non
            if (style != null && Zone[i] != null && style.transform.position.Equals(Zone[i].Pos))
                {
                    selectionable = true;
                }
        }
            return selectionable;
    }
    //change la couleur de derriere de la créature pour savoir si elle peut attaquer
    public void peutAttaquer(PosZoneCombat[] tab, GameObject [] style)
    {
        //pour toute le tableau
        for (int i = 0; i < tab.Length; ++i)
        {
            //on vérifie si il y a une carte  
            //si c'est celle qui est choisie pour attaquer
            if (tab[i].carte != null && style[i] != null && tab[i].carte.perm != null && i == posAllier && tab[i].carte.perm.TypePerm == "Creature")
            {
                //on trouve sa position
                taille = style[i].name.Length - 4;
                numero = int.Parse(style[i].name.Substring(4, taille));
                //on change sa couleur de background pour la couleur bleu
                style[i].transform.FindChild("glow" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = true;
                style[i].transform.FindChild("glow" + numero.ToString()).GetComponent<SpriteRenderer>().color = Color.blue;
            }
                //si elle peut attaquer 
            else if (tab[i].carte != null && style[i] != null && !tab[i].carte.perm.aAttaque && tab[i].carte.perm.TypePerm == "Creature" && Selectionable(style[i], tab) && tab[i].carte.perm.estEndormi == 0)
            {
                //on trouve sa position
                taille = style[i].name.Length - 4;
                numero = int.Parse(style[i].name.Substring(4, taille));
                //on change sa couleur de background pour la couleur verte
                style[i].transform.FindChild("glow" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = true;
                style[i].transform.FindChild("glow" + numero.ToString()).GetComponent<SpriteRenderer>().color = Color.green;
                //style[i].renderer.material.color = Color.green;
            }
            //sinon si il y a seulement une créature qui ne peut pas attaquer ou un batiment
            else if (tab[i].carte != null && style[i] != null)
            {
                //on trouver sa position
                taille = style[i].name.Length - 4;
                numero = int.Parse(style[i].name.Substring(4, taille));
                //on enleve le spriteRenderer
                style[i].transform.FindChild("glow" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = false;
                //style[i].renderer.material.color = Color.white;
            }
        }
    }
    //change la couleur de la créature pour savoir si elle peut etre attaquer par les créatures
    public void peutEtreAttaquer(PosZoneCombat[] tab, GameObject [] style)
    {
        //savoir si l'autre joueur a un taunt
        const int taunt = 1;
        int[] peutEtreAttaquer = new int[tab.Length];
        bool esttaunt = foundTaunt(tab,peutEtreAttaquer);
        //si une des créatures est un provocation
        if (esttaunt)
        {
            //pour tout le tableau 
            for (int i = 0;i < tab.Length; ++i)
            {  
                //si la carte est selectionable et que dans le tableau peut etre attaquer (a la meme position que dans le tab normal) est = a provocation(variable taunt) 
                if (peutEtreAttaquer[i] == taunt && Selectionable(style[i], tab))
                {
                    //trouve sa position
                    taille = style[i].name.Length - 11;
                    numero = int.Parse(style[i].name.Substring(11, taille));
                    //change sa couleur de background pour rouge
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = true;
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().color = Color.red;
                    //style[i].renderer.material.color = Color.red;
                }
                    //sinon si il y a une carte présente
                else if (tab[i].carte != null && style[i] != null)
                {
                    //on trouve sa position
                    taille = style[i].name.Length - 11;
                    numero = int.Parse(style[i].name.Substring(11, taille));
                    //on enleve son spriterenderer qui s'occupe de mettre la couleur
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = false;
                    //style[i].renderer.material.color = Color.white;
                }
            }
        }
            //sinon toute les créatures peuvent etre vise(sauf les créature invisible)
        else
        {
            for (int i = 0; i < tab.Length; ++i)
            {
                //si selectionable et pas invisble
                if (tab[i].carte != null && style[i] != null && !tab[i].carte.perm.estInvisible && Selectionable(style[i], tab))
                {
                    //trouve sa position
                    taille = style[i].name.Length - 11;
                    numero = int.Parse(style[i].name.Substring(11, taille));
                    //on change sa couleur de background pour la couleur rouge
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = true;
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().color = Color.red;
                    //style[i].renderer.material.color = Color.red;
                }
                    //sinon si juste présente
                else if (tab[i] != null && style[i] != null)
                {
                    //trouve sa position
                    taille = style[i].name.Length - 11;
                    numero = int.Parse(style[i].name.Substring(11, taille));
                    //on enleve le sprite rendrer pour qu'il n'y est aucune couleur
                    style[i].transform.FindChild("glowEnnemis" + numero.ToString()).GetComponent<SpriteRenderer>().enabled = false;
                    //style[i].renderer.material.color = Color.white;
                }
            }
        }
    }
    //trouve si dans la zone de combat il y a une créature avec provocation
    public bool foundTaunt(PosZoneCombat[] tab,int[] peutEtreAttaquer)
    {
        const int taunt = 1;
        bool esttaunt = false;
        //pour toute la zone
        for (int i = 0; i < tab.Length; ++i)
        {
            //si la carte a taunt 
            if (tab[i].carte != null && tab[i].carte.perm.estTaunt)
            {
                //on dit qu'il y a un taunt et on la met dans le tableau qui sera retourne(car passe en reference )
                esttaunt = true;
                peutEtreAttaquer[i] = taunt;
            }
        }
        //retourn si oui ou non il y a une carte avec provocation
        return esttaunt;
    }
    //meme chause sauf sans tableau de retour on veut seulement sa voir s'il y a des cartes par provocation
    //mais il fait la meme chause que l'autre fonction
    public bool foundTaunt(PosZoneCombat[] tab)
    {
        bool esttaunt = false;
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i].carte != null && tab[i].carte.perm.estTaunt)
                esttaunt = true;
        }
        return esttaunt;
    }
    //s'occupe des click pour savoir si la carte est choisie et fait le dégat
    public void Attaquer()
    {
        //modifie les cartes selon si elle peut attaquer ou si elle peut etre attaquer
        //change le border pour une autre couleur
        peutAttaquer(Jouer.ZoneCombat, Jouer.styleCarteAlliercombat);
        peutEtreAttaquer(Jouer.ZoneCombatEnnemie,Jouer.styleCarteEnnemisCombat);
        //si on click gauche et que c'est mon tour
        if (Input.GetMouseButtonDown(0) && Jouer.MonTour)
        {
            //on get la position de click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit carte;
            //si c'est au tour de choisir un attaquant(donc dans notre place de combat)
            if (Physics.Raycast(ray, out carte) && !AttaquantClick && Selectionable(GameObject.Find(carte.collider.gameObject.name),Jouer.ZoneCombat))
            {
                //on trouve le gameobject choisie sa position 
                carteAttaque = GameObject.Find(carte.collider.gameObject.name);
                posAllier = TrouverEmplacementCarteJoueur(carteAttaque.transform.position, Jouer.ZoneCombat);
                //si c'est une créature et qu'elle peut attaquer
                if (posAllier != -1 && !Jouer.ZoneCombat[posAllier].carte.perm.aAttaque && Jouer.ZoneCombat[posAllier].carte.perm.TypePerm == "Creature" && Jouer.ZoneCombat[posAllier].carte.perm.estEndormi == 0)
                {
                    //alors le click est valide et on crée un nouveau permanent
                    AttaquantClick = true;
                    int[] stat = getStat(Jouer.ZoneCombat[posAllier].carte.perm);
                    Attaquant = new Permanent("Creature", stat[0], stat[1],stat[2]);
                    
                }
                //sinon on dit que le click est invalide
                else
                {
                    posAllier = -1;
                }
            }
            //si c'est le tour de choisir la carte(ou heros) a attaquer
            else if (Physics.Raycast(ray, out carte) && AttaquantClick)
            {
                //on vérifie s'il y a une carte avec provocation
                bool taunt = false;
                //trouve le gameObject
                carteDefense = GameObject.Find(carte.collider.gameObject.name);
                taunt = foundTaunt(Jouer.ZoneCombatEnnemie);
                //si la carte est selectionable
                if (Selectionable(GameObject.Find(carte.collider.gameObject.name), Jouer.ZoneCombatEnnemie))
                {
                    //on trouve sa position
                    int pos = TrouverEmplacementCarteJoueur(carte.transform.position, Jouer.ZoneCombatEnnemie);
                    //s'il y a des provocateur
                    if (taunt)
                    {
                        //si la carte choisie est un provocateur
                        if (pos != -1 && Jouer.ZoneCombatEnnemie[pos].carte.perm.estTaunt)
                        {
                            //la carte est valide
                            attaqueSomething();
                        }
                    }
                        //si il n'y a pas de provocateur et que la carte n'est pas invisble
                    else if(!Jouer.ZoneCombatEnnemie[pos].carte.perm.estInvisible)
                    {
                        //la carte est valide
                        attaqueSomething();
                    }
                }
                //si c'est le héros et qu'il n'y a aps de provocateur
                if (carteDefense!= null && carteDefense.name == "hero ennemis" && !foundTaunt(Jouer.ZoneCombatEnnemie))
                {
                    //aucune action possible (on attend la fin de l'action)
                    waitForActionDone();
                    //on reduit la vie du joueur
                    Jouer.HpEnnemi = CombatJoueur(Jouer.ZoneCombat[posAllier].carte, Jouer.HpEnnemi);
                    //s'il y a déjà attaquer 2 fois ou qu'il n'a pas attaque double alors on dit qu'il ne peut plus attaquer
                    if (!Jouer.ZoneCombat[posAllier].carte.perm.estAttaqueDouble || Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble)
                        Jouer.ZoneCombat[posAllier].carte.perm.aAttaque = true;
                        //sinon il a attaque double donc on lui enleve un premier coup
                    else
                        Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble = true;
                    //on enleve le click
                    AttaquantClick = false;
                    //son d'attaque
                    audio.PlayOneShot(Attack);
                    //on le dit au serveur
                    envoyerMessage("Attaquer Joueur");
                    StartCoroutine(wait(1.5f));
                    //on envoie la carte
                    EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCombat[posAllier].carte);
                    StartCoroutine(waitEnvoyer(0.75f));
                    //si le joueur est mort alors on dit qu'on a ganger
                    if (Jouer.HpEnnemi <= 0)
                    {
                        Jouer.EstGagnant = true;
                        Jouer.gameFini = true;
                    }
                    //on enleve la posAllier
                    posAllier = -1;
                    Attaquant = null;
                } 
            }
        }
            //si c'est un click droite et qu'on a clicke sur une créature pour attaquer
        else if (Input.GetMouseButtonDown(1) && AttaquantClick )
        {
            //on deselect la créature choisie
            AttaquantClick = false;
            posAllier = -1;
        }
    }
    //fait le combat entre 2 créature
    private void attaqueSomething()
    {
        //on trouve la position du défenseur
        posDefenseur = TrouverEmplacementCarteJoueur(carteDefense.transform.position, Jouer.ZoneCombatEnnemie);
        //si elle est valide
        if (posDefenseur != -1 && carteDefense.name != "hero ennemis")
        {
            //on ne laisse aucune action possible
            waitForActionDone();
            //on trouve les stats de la créature et on les donnes au défenseur
            int[] stat = getStat(Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm);
            AttaquantClick = false;
            Defenseur = new Permanent("Creature", stat[0], stat[1], stat[2]);
            //on enleve la vie du défenseur
            CombatCreature(Attaquant, Defenseur);
            //on enleve la vie de l'attaquant
            CombatCreature(Defenseur, Attaquant);
            //on modifie les stats de la carte (donc celle de la classe carte)
            setStat(Jouer.ZoneCombat[posAllier].carte.perm, new int[] { Attaquant.Attaque, Attaquant.Vie, Attaquant.Armure });
            setStat(Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm, new int[] { Defenseur.Attaque, Defenseur.Vie, Defenseur.Armure });
            //on modifie le GameObject de l'attaquant et le defenseur(la vie et l'armure sur la carte donc le graphique)
            recevoirDegat(Jouer.ZoneCombat[posAllier].carte,carteAttaque, true);
            recevoirDegat(Jouer.ZoneCombatEnnemie[posDefenseur].carte,carteDefense, false);
            //on fait des strings avec les cartes, car unity problems
            string attaquant = SetCarteString(Jouer.ZoneCombat[posAllier].carte);
            string ennemis = SetCarteString(Jouer.ZoneCombatEnnemie[posDefenseur].carte);
            //si la carte a déjà attaquer ou qu'il n'y a pas attaque double ou dit qu'il ne peut plus attaquer
            if (!Jouer.ZoneCombat[posAllier].carte.perm.estAttaqueDouble || Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble)
                Jouer.ZoneCombat[posAllier].carte.perm.aAttaque = true;
                //sinon on dit que le joueur rest un coup a attaquer
            else
                Jouer.ZoneCombat[posAllier].carte.perm.aAttaquerDouble = true;
            //si la créature est endormie on la reveille
            if (Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm.estEndormi != 0)
            {
                Jouer.ZoneCombatEnnemie[posDefenseur].carte.perm.estEndormi = 0;
            }
            //si une carte meurt on la détruit 
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
            //on envoye le message et on reset les variables de combat
            envoyerMessage("Attaquer Creature." + posAllier + "." + posDefenseur + "."+carteAttaque.name+"."+carteDefense.name+"." + attaquant + "." + ennemis);
            StartCoroutine(waitEnvoyer(0.75f));
            Attaquant = null;
            carteAttaque = null;
            Defenseur = null;
            carteDefense = null;
            posAllier = -1;
            posDefenseur = -1;
            //on joue le son d'attaque
            audio.PlayOneShot(Attack);
        }      
    }
    //fait un string avec les données de la carte(la carte est une creature pour sur)
    private string SetCarteString(Carte temp)
    {
                  /*0                   1                     2                   3                      4                      5                    6                     7                            8                   9                         10*/
        return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte + "." + temp.perm.Attaque + "." + temp.perm.Vie + "." + temp.perm.Armure + "." + temp.perm.TypePerm;
    }
    //sleep en unity 
    public IEnumerator wait(float i)
    {
        yield return new WaitForSeconds(i);
    }
    //sleep en unity mais celui-ci restart() il va donc laisser le joueur faire des actions apres le wait
    public IEnumerator waitEnvoyer(float i)
    {
        yield return new WaitForSeconds(i);
        restart();
    }
    //trouve la position du Gameobject dans le tableau de la zone de combat choisie(il la trouve selon la position)
    private int TrouverEmplacementCarteJoueur(Vector3 PosCarte, PosZoneCombat[] Zone)
    {
        //pour chaque place de la zone 
        for (int i = 0; i < Zone.Length; ++i)
        {
            //si elle est presente on retourne la position
            if (PosCarte.Equals(Zone[i].Pos))
            {
                return i;
            }
        }
        return -1; // -1 pour savoir qu'il ne trouve aucune position (techniquement il devrais toujours retourner un pos valide)
    }
    //detruit la carte
    private void kill(GameObject carteTemp)
    {
        Destroy(carteTemp/*2ieme argumen est un delais pour */);
    }
    //sdescend la vie des créatures
    private void CombatCreature(Permanent attaquant, Permanent defenseur)
    {
        //si l'attaquant ne réduit pas toute l'armure on enleve une partie de l'armure selon lattaque
        int attaque = attaquant.Attaque;
        if (defenseur.Armure - attaque >= 0)
            defenseur.Armure -= attaque;
            //sinon on enleve l'atttaque selon l'armure et on met l'armure a 0 pour ensuite enleve la vie de la créature selon la différence entre l'attaque et l'armure
        else
        {
            int difference = attaque - defenseur.Armure;
            defenseur.Armure = 0;
            defenseur.Vie -= difference;
        }
    }
    //reduit la vie du joueur
    private int CombatJoueur(Carte carte, int vie)
    {
        //si la carte a attaque puissante on double le dégat
        int attaque = carte.perm.Attaque;
        if (carte.perm.estAttaquePuisante)
            attaque *= 2;
        return vie - attaque;
    }
    //retourne les stats selon un permanent
    int[] getStat(Permanent perm)
    {
        int[] stat = new int[3];
        stat[0] = perm.Attaque;
        stat[1] = perm.Vie;
        stat[2] = perm.Armure;           
        return stat;
    }
    //set les stat d'un permanent selon un tabelau de 3 valeur 0-attaque 1-vie 2-armure
    void setStat(Permanent perm,int [] stat)
    {
        perm.Attaque = stat[0];
        perm.Vie = stat[1];
        perm.Armure = stat[2];
    }
    //change les donnée de la créature (donc modifié les chiffe sur le GameObject)
    public void recevoirDegat(Carte carte,GameObject card,bool allier)
    {
        GameObject t = null;
        if (carte != null)
        {
            //si la carte est un allié
            if (allier)
            {
                //on trouve sa position
                string position = card.name.Substring(card.name.IndexOf("d") + 1, card.name.Length - (card.name.IndexOf("d") + 1));
                //on change le textMesh selon les nouvelles donnée(pour l'armure et la vie)
                t = GameObject.Find("armure" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vie" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
            else
            {
                //on trouve sa position
                string position = card.name.Substring(card.name.IndexOf("s") + 1, card.name.Length - (card.name.IndexOf("s") + 1));
                //on change le textMesh selon les nouvelles donnée(pour l'armure et la vie)
                t = GameObject.Find("armureEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Armure.ToString();
                t = GameObject.Find("vieEnnemis" + position);
                t.GetComponent<TextMesh>().text = carte.perm.Vie.ToString();
            }
        }
    }
    ///////////-----communication serveur-------//////////
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
        StartCoroutine(wait(0.5f));
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
        StartCoroutine(wait(0.5f));
    }
    ///////////-----fin communication serveur-------//////////
    //ne l'aisse le joueur faire aucune action
    public void waitForActionDone()
    {
        Jouer.MonTour = false;
    }
    //laisse le joueur faire des actions
    public void restart()
    {
        Jouer.MonTour = true;
    }
}
