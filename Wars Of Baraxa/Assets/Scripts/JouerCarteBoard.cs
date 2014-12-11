using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using warsofbaraxa;


public class JouerCarteBoard : MonoBehaviour
{
    //le delay avant d'afficher le zoom de la carte
    public float delay;
    public Jouer Script_Jouer;
    public attaque Script_attaque;
    //savoir si elle est jouée et si elle est ennemis
    public bool EstJouer = false;
    public bool EstEnnemie = false;
    //clone de la carte
    Transform clonetransform;
    GameObject cloneCarte;
    //si la carte est clone
    bool estClone;
    //cout
    TextMesh[] Cout;



    // Use this for initialization
    void Start()
    {
        Cout = GetComponentsInChildren<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //quand une carte est detruite
    void OnDestroy()
    {
        //si la carte est joue et ce n'est pas un clone
        if (EstJouer && !estClone)
        {
            //si c'est un ennemis on dit que la place n'est plus occupe
            if (EstEnnemie)
                Jouer.ZoneCombatEnnemie[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombatEnnemie)].EstOccupee = false;
            //sinon allie
            else
            {
                int PlacementZoneCombat = TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombat);
                //si elle est en jeu
                if (PlacementZoneCombat != -1)
                {
                    //on enleve les bonus quelle donne sinon on di qu'il n'y a plus de carte la
                    Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = false;
                    if (Jouer.ZoneCombat[PlacementZoneCombat] != null && Jouer.ZoneCombat[PlacementZoneCombat].carte != null && Jouer.ZoneCombat[PlacementZoneCombat].carte.perm != null && Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
                    {
                        string[] zeSpecialHability = Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.habilityspecial.Split(new char[] { ' ' });
                        if (zeSpecialHability[0] == "Donne")
                        {
                            enleverBonusStat(zeSpecialHability);
                        }
                    }
                    Jouer.ZoneCombat[PlacementZoneCombat].carte = null;
                }
            }
        }
        //si il y a un clone on detruit le clone
        if (cloneCarte != null)
        {
            //on la met invisible
            Color c = cloneCarte.renderer.material.color;
            c.a = 0f;
            //on la détruit
            Destroy(cloneCarte);
            cloneCarte = null;
        }
    }
    //trouver la position de la carte dans la zone
    private int TrouverEmplacementCarteJoueur(Vector3 PosCarte, PosZoneCombat[] Zone)
    {
        for (int i = 0; i < Zone.Length; ++i)
        {
            if (PosCarte.Equals(Zone[i].Pos))
            {
                return i;
            }
        }
        return -1; // -1 pour savoir qu'il ne trouve aucune position (techniquement il devrais toujours retourner un pos valide)
    }
    //retourne le nombre de carte dans la zone
    private int getNbCarteZone(PosZoneCombat[] zone)
    {
        int nbCarte = 0;
        for (int i = 0; i < zone.Length; ++i)
        {
            if (zone[i] != null && zone[i].EstOccupee)
                ++nbCarte;
        }
        return nbCarte;
    }
    //trouve la cible et inflige le degat à la cible
    private bool spellBurn(string target, int nbDegat, Carte zeTarget, GameObject t, int posTarget)
    {
        //trouve si la cible est balide
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemies" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemies")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        //si la cible est valide
        if (valide)
        {
            //on réduit sa vie/armure
            if ((zeTarget.perm.Armure - nbDegat) >= 0)
                zeTarget.perm.Armure -= nbDegat;
            else
            {
                int difference = nbDegat - zeTarget.perm.Armure;
                zeTarget.perm.Armure = 0;
                zeTarget.perm.Vie -= difference;
                //si elle meurt on détruit la créature
                if (zeTarget.perm.Vie <= 0)
                {
                    Destroy(t, 1);
                    Jouer.ZoneCombatEnnemie[posTarget].EstOccupee = false;
                    Jouer.ZoneCombatEnnemie[posTarget].carte = null;
                    zeTarget = null;
                }
            }
            //on change les stats du gameobject carte
            if (t != null && zeTarget != null)
            {
                TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
                stat[3].text = zeTarget.perm.Armure.ToString();
                stat[5].text = zeTarget.perm.Vie.ToString();
            }
        }
        return !valide;
    }
    //trouve la cible et soigne la cible
    private bool spellHeal(string target, int nbVie, Carte zeTarget, GameObject t, int posTarget)
    {
        //trouve si la cible
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemies" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemies")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        
        //si la cible valide
        if (valide)
        {
            if ((zeTarget.perm.Vie + nbVie) >= zeTarget.perm.basicVie)
                zeTarget.perm.Vie = zeTarget.perm.basicVie;
            else
                zeTarget.perm.Vie += nbVie;

            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[5].text = zeTarget.perm.Vie.ToString();
        }

        return !valide;
    }
    //trouve la cible et l'endort
    private bool spellSleep(string target, int nbTour, Carte zeTarget, GameObject t, int posTarget)
    {
        //trouve si la cible est valide
        bool valide = false;
        if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemies")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        //si elle est valide 
        if (valide)
        {
            //on l'endort pour le nombre de tour
            zeTarget.perm.estEndormi = nbTour;

            //EnvoyerCarte(connexionServeur.sck, zeTarget);
        }

        return !valide;
    }
    //trouve la cible et la détruit
    private bool spellDestroy(string target, Carte zeTarget, GameObject t, int posTarget)
    {
        //trouve si la cible est valide
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemies" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemies")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        //si elle est valide
        if (valide)
        {
            //on la détruit et on enleve de la zone
            Destroy(t, 1);
            Jouer.ZoneCombatEnnemie[posTarget].EstOccupee = false;
            Jouer.ZoneCombatEnnemie[posTarget].carte = null;
            zeTarget = null;
        }
        return !valide;
    }
    //trouve la cible et la transforme
    private bool spellTransform(string target, int[] stats, Carte zeTarget, GameObject t, int posTarget)
    {
        //Vie attaque armure
        ////////////////////
        //vérifie si la cible est valide
        bool valide = false;
        if (target == "cible" || target == "touteslescartes" || target == "touteslescartesennemies" || target == "touteslescartesalliees")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemies" || target == "touteslescreaturesalliees")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis" || target == "touslesbatimentsallies")
        {
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        }
        //si oui
        if (valide)
        {
            //on change ses stats
            zeTarget.perm.Vie = stats[2];
            zeTarget.perm.Attaque = stats[1];
            zeTarget.perm.Armure = stats[0];
            zeTarget.perm.basicVie = stats[2];
            zeTarget.perm.basicAttaque = stats[1];
            zeTarget.perm.basicArmor = stats[0];
        }
        //on cahnge apres son text sur le gameobject
        if (t != null)
        {
            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[3].text = zeTarget.perm.Armure.ToString();
            stat[4].text = zeTarget.perm.Attaque.ToString();
            stat[5].text = zeTarget.perm.Vie.ToString();
        }

        return !valide;
    }
    //trouve si la cible est valide et augmente ses stats
    private bool spellBuff(string target, Carte zeTarget, GameObject t, int posTarget)
    {
        //vérifie si la cible est valide
        bool valide = false;
        string buffHabilete = "";
        int buffVie = 0; int buffAttaque = 0; int buffArmure = 0;
        if (target == "cible" || target == "touteslescartes" || target == "touteslescartesalliees")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesalliees")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsallies")
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        //si oui
        if (valide)
        {
            //on augmente les stats de la cible
            for (int i = 1; i < Jouer.texteHabileteSansEspace.Length; i++)
            {
                if (Jouer.texteHabileteSansEspace[i] == "et")
                {
                    if (Jouer.texteHabileteSansEspace[i + 2] == "double" || Jouer.texteHabileteSansEspace[i + 2] == "puissante")
                        buffHabilete = Jouer.texteHabileteSansEspace[i + 1] + ' ' + Jouer.texteHabileteSansEspace[i + 2];
                    else
                        buffHabilete = Jouer.texteHabileteSansEspace[i + 1];
                }
                else if (Jouer.texteHabileteSansEspace[i][0] == '+')
                {
                    if (Jouer.texteHabileteSansEspace[i + 1] == "pV")
                        buffVie = int.Parse(Jouer.texteHabileteSansEspace[i].Split(new char[] { '+' })[1]);
                    else if (Jouer.texteHabileteSansEspace[i + 1] == "pAtt")
                        buffAttaque = int.Parse(Jouer.texteHabileteSansEspace[i].Split(new char[] { '+' })[1]);
                    else if (Jouer.texteHabileteSansEspace[i + 1] == "pArm")
                        buffArmure = int.Parse(Jouer.texteHabileteSansEspace[i].Split(new char[] { '+' })[1]);
                }
            }
            //stats (vie armure attaque)
            zeTarget.perm.Vie += buffVie;
            zeTarget.perm.basicVie += buffVie;
            zeTarget.perm.Attaque += buffAttaque;
            zeTarget.perm.basicAttaque += buffAttaque;
            zeTarget.perm.Armure += buffArmure;
            zeTarget.perm.basicArmor += buffArmure;
            //s'il donne provocation
            if (buffHabilete == "provocation")
            {
                if (zeTarget.perm.estInvisible)
                    zeTarget.perm.estInvisible = false;
                zeTarget.perm.estTaunt = true;
            }
            //s'il donne attaque puissante
            else if (buffHabilete == "attaque puissante")
            {
                zeTarget.perm.estAttaquePuisante = true;
            }
            //s'il donne attaque double
            else if (buffHabilete == "attaque double")
            {
                zeTarget.perm.estAttaqueDouble = true;
            }
            //s'il donne invisible
            else if (buffHabilete == "invisible")
            {
                if(zeTarget.perm.estTaunt)
                    zeTarget.perm.estTaunt = false;
                zeTarget.perm.estInvisible = true;
            }
            //modifie le gameobject selon les nouvelle stats
            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();

            stat[3].text = zeTarget.perm.Armure.ToString();
            stat[4].text = zeTarget.perm.Attaque.ToString();
            stat[5].text = zeTarget.perm.Vie.ToString();
        }

        return !valide;
    }
    //reoutne si la carte peut etre présente dans la zone
    private bool Selectionable(GameObject style, PosZoneCombat[] Zone)
    {
        bool selectionable = false;
        for (int i = 0; i < Zone.Length; ++i)
        {
            //on vérifie si il y a une carte et si elle est a la position de la zone
            if (style != null && Zone[i]!= null && style.transform.position.Equals(Zone[i].Pos))
            {
                selectionable = true;
            }
        }
        return selectionable;
    }
    //trouve la sorte de target
    private string trouverTypeTarget(string[] motsHabilete)
    {
        //pour chaque mot on vérifie si c'est le mot pour connaitre les cibles
        string target = ""; int i = 0;
        bool pasTrouver = true;
        while (pasTrouver && i < motsHabilete.Length)
        {
            if (motsHabilete[i] == "cible" || motsHabilete[i] == "creature" || motsHabilete[i] == "batiment" || motsHabilete[i] == "ennemis")
            {
                target = motsHabilete[i];
                pasTrouver = false;
            }
            else if (motsHabilete[i] == "vos")
            {
                if (motsHabilete[i + 1] == "creatures" || motsHabilete[i + 1] == "batiments" || motsHabilete[i + 1] == " cartes")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1];
                    pasTrouver = false;
                }
            }
            else if (motsHabilete[i] == "toutes")
            {
                if (motsHabilete[i + 1] == "les" && (motsHabilete[i + 2] == "cartes" || motsHabilete[i + 2] == "creatures"))
                {
                    if (i + 3 < motsHabilete.Length && (motsHabilete[i + 3] == "ennemies" || motsHabilete[i + 3] == "alliees"))
                    {
                        target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2] + motsHabilete[i + 3];
                        pasTrouver = false;
                    }
                    else
                    {
                        target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                        pasTrouver = false;
                    }
                }
            }
            else if (motsHabilete[i] == "tous")
            {
                if (motsHabilete[i + 1] == "les" && motsHabilete[i + 2] == "batiments")
                {
                    if (i + 3 < motsHabilete.Length && (motsHabilete[i + 3] == "ennemies" || motsHabilete[i + 3] == "alliees"))
                    {
                        target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2] + motsHabilete[i + 3];
                        pasTrouver = false;
                    }
                    else
                    {
                        target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                        pasTrouver = false;
                    }
                }
            }
            else if (motsHabilete[i] == "heros")
            {
                if (motsHabilete[i + 1] == "ennemis")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1];
                    pasTrouver = false;
                }
            }
            else if (motsHabilete[i] == "place")
            {
                if (motsHabilete[i + 1] == "de" && motsHabilete[i + 2] == "combat")
                {
                    target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                    pasTrouver = false;
                }
            }
            ++i;
        }
        return target;
    }
    private int trouverNbDegat(string[] motsHabilete)
    {
        int nombre = 0;
        //trouve le nombre de dégat que le sort fait
        nombre = int.Parse(motsHabilete[1]);

        return nombre;
    }
    private int trouverNbVie(string[] motsHabilete)
    {
        
        int nombre = 0;
        //trouve le nombre de vie que le sort soigne
        nombre = int.Parse(motsHabilete[motsHabilete.Length - 2]);

        return nombre;
    }
    private int[] trouverStats(string[] motsHabilete)
    {

        int[] stats = { 0, 0, 0 };
        int vie = 0; int attaque = 0; int armure = 0;
        //on split a chaque / car c'est les 3 chiffre important
        string[] tabStat = motsHabilete[motsHabilete.Length - 1].Split(new char[] { '/' });
                        //vie                               //attaque                   //armure
        vie = int.Parse(tabStat[0]); attaque = int.Parse(tabStat[1]); armure = int.Parse(tabStat[2]);
        stats[0] = vie; stats[1] = attaque; stats[2] = armure;

        return stats;
    }
    private int trouverNbTour(string[] motsHabilete)
    {
        int nombre = 0;
        //trouve le nombre de tour que la cible sera endormi
        nombre = int.Parse(motsHabilete[motsHabilete.Length - 2]);

        return nombre;
    }
    //trouve le type effet entre : Inflige Soigne Endort Transforme Donne Detruit
    private string trouverTypeEffet(string[] motsHabilete)
    {
        string effet = "";

        for (int i = 0; i < motsHabilete.Length; i++)
        {
            //pour chaque mot on vérifie si c'est un habileté
            if (isEffet(motsHabilete[i]))
            {
                effet = motsHabilete[i];
            }
        }
        return effet;
    }
    //liste des habileté
    private bool isEffet(string mot)
    {
        return mot == "Inflige" || mot == "Soigne" || mot == "Endort" || mot == "Transforme" || mot == "Donne" || mot == "Detruit";
    }
    //liste des sort sur les ennemis
    private bool isEnnemi(string mot)
    {
        return mot == "Inflige" || mot == "Endort" || mot == "Transforme" || mot == "Detruit";
    }
    //liste des mots pour savoir si on a besoin d'une cible
    private bool isATargetNeeded(string mot)
    {
        return mot == "cible" || mot == "creature" || mot == "batiment";
    }
    //liste des mot pour savoir si ça touche les 2 joueur
    private bool isOnBothPlayers(string target)
    {
        return target == "touteslescartes" || target == "touteslescreatures" || target == "touslesbatiments" || target == "placedecombat";
    }
    //si le soigne touche le heros
    private bool healHeros(string quelJoueur, int nbVie)
    {
        Script_Jouer = GetComponent<Jouer>();
        //notre heros
        if (quelJoueur == "Allier")
        {
            //on ne dépasse pas la vie maximum donc si il est pour dépasser on augment a la vie max
            if (Jouer.HpJoueur + nbVie >= 30)
                Jouer.HpJoueur = 30;
            else
                Jouer.HpJoueur += nbVie;
        }
            //héros ennemis
        else
        {
            if (Jouer.HpEnnemi + nbVie >= 30)
                Jouer.HpEnnemi = 30;
            else
                Jouer.HpEnnemi += nbVie;
        }
        return false;
    }
    //si le inflige touche le heros
    public bool burnHeros(string quelJoueur, int nbDegat)
    {
        Script_Jouer = GetComponent<Jouer>();
        //si notre heros
        if (quelJoueur == "Allier")
            //on réduit la vie de notre personnage
            Jouer.HpJoueur -= nbDegat;
        else
            Jouer.HpEnnemi -= nbDegat;
            //sinon on réduit la vie du personnage ennemis
        return false;
    }
    //si le sort touche le héros ennemis
    public bool isOnHeros(string target)
    {
        return target == "placedecombat" || target == "ennemis" || target == "herosennemis";
    }
    //si le sort touche les 2 héros
    public bool isOnBothHeros(string target)
    {
        return target == "placedecombat";
    }
    //si le sort touche toute les creature
    public bool spellCreaturesBothSides(string target)
    {
        return target == "touteslescreatures";
    }
    //trouve la position de la carte en main qu'on est en train de jouer(donc si on annul le sort elle va pouvoir retourner a sa position)
    public static int trouverPosEnTrainCaster(GameObject carte)
    {
        int pos = 0;
        for (int i = 0; i < Jouer.ZoneCarteJoueur.Length; i++)
        {
            if (carte.transform.position == Jouer.ZoneCarteJoueur[i].Pos)
                pos = i;
        }
        return pos;
    }

    void OnMouseDown()
    {
        Jouer.target = null;
        int Emplacement = 0;
        int posTarget;
        //si on a besoin d'une cible pour finir le sort
        if (Jouer.enTrainCaster)
        {
            
            if (Jouer.targetNeeded)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit carte;
                //on recois la cible
                if (Physics.Raycast(ray, out carte))
                {
                    Jouer.target = GameObject.Find(carte.collider.gameObject.name);
                    //si la cible est un héros
                    if (Jouer.target != null && (Jouer.target.name == "hero ennemis" || Jouer.target.name == "hero"))
                    {
                        //c'est sois Inflige ou soigne alors on modifie leur vie
                        if (Jouer.effet == "Inflige")
                        {
                            if (Jouer.target.name == "hero ennemis")
                                Jouer.enTrainCaster = burnHeros("Ennemi", trouverNbDegat(Jouer.texteHabileteSansEspace));
                            else
                                Jouer.enTrainCaster = burnHeros("Allier", trouverNbDegat(Jouer.texteHabileteSansEspace));
                        }
                        else if (Jouer.effet == "Soigne")
                        {
                            if (Jouer.target.name == "hero")
                                Jouer.enTrainCaster = healHeros("Allier", trouverNbVie(Jouer.texteHabileteSansEspace));
                            else
                                Jouer.enTrainCaster = healHeros("Ennemi", trouverNbVie(Jouer.texteHabileteSansEspace));
                        }
                    }
                    /*permanents car est sur le board*/

                    else
                    {
                        //notre zone
                        if (!isEnnemi(Jouer.effet))
                        {
                            //on trouve la position la carte dans la zone
                            posTarget = TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombat);
                            Jouer.carteTarget = Jouer.ZoneCombat[posTarget].carte;

                            if (TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombat) != -1)
                            {
                                //si c'est dans notre zone on veut soit soigner ou donner
                                if (Jouer.effet == "Soigne")
                                    Jouer.enTrainCaster = spellHeal(Jouer.spellTarget, trouverNbVie(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[posTarget].carte, Jouer.target, posTarget);
                                else if (Jouer.effet == "Donne")
                                    Jouer.enTrainCaster = spellBuff(Jouer.spellTarget, Jouer.ZoneCombat[posTarget].carte, Jouer.target, posTarget);
                            }
                        }
                        else
                        {
                            //sinon zone ennemis
                            if (TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombatEnnemie) != -1)
                            {
                                //on veut soit infliger endormir transformer ou detruire
                                posTarget = TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombatEnnemie);
                                Jouer.carteTarget = Jouer.ZoneCombatEnnemie[posTarget].carte;

                                if (Jouer.effet == "Inflige")
                                    Jouer.enTrainCaster = spellBurn(Jouer.spellTarget, trouverNbDegat(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte, Jouer.target, posTarget);
                                else if (Jouer.effet == "Endort")
                                    Jouer.enTrainCaster = spellSleep(Jouer.spellTarget, trouverNbTour(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte, Jouer.target, posTarget);
                                else if (Jouer.effet == "Transforme")
                                    Jouer.enTrainCaster = spellTransform(Jouer.spellTarget, trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[posTarget].carte, Jouer.target, posTarget);
                                else if (Jouer.effet == "Detruit")
                                    Jouer.enTrainCaster = spellDestroy(Jouer.spellTarget, Jouer.ZoneCombatEnnemie[posTarget].carte, Jouer.target, posTarget);
                            }
                        }
                    }
                }
                //si il n'est pas entrain de caster
                if (!Jouer.enTrainCaster)
                {
                    //on ne peut pas faire d'autre action(joueur une autre carte ajouter la mana etc)
                    waitForActionDone();
                    EstJouer = true;
                    //apres une seconde on détruit le sort
                    Destroy(Jouer.spell, 1);
                    string spellString = SetCarteString(Jouer.ZoneCarteJoueur[Jouer.position].carte);
                    //si on joue un spell qui cible le heros
                    if (Jouer.target.name != "hero ennemis" && Jouer.target.name != "hero")
                    {
                        //le message envoyer au serveur est different(on envoie aucune créature, car c'est le heros)
                        string targerString = SetCarteString(Jouer.carteTarget);
                        envoyerMessage("Jouer spellTarget." + Jouer.spell.name + "." + Jouer.target.name + "." + spellString+"." +targerString);
                        StartCoroutine(waitEnvoyer(0.75f));
                    }
                    //sinon créature ou batiment
                    else
                        envoyerMessage("Jouer spellTarget." + Jouer.spell.name + "." + Jouer.target.name +"."+spellString);
                    StartCoroutine(waitEnvoyer(0.75f));
                    //on met la carte a cette position a vide
                    Jouer.ZoneCarteJoueur[Jouer.position].carte = null;
                    Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
                    Jouer.spell = null;
                    //si on tue le heros on dit que lon gagne
                    //la c'est null
                    if (Jouer.HpEnnemi <= 0 && Jouer.HpJoueur <= 0)
                    {
                        Jouer.gameFini = true;
                        Jouer.EstNul = true;
                    }
                        //ici on a perdu
                    else if (Jouer.HpJoueur <= 0)
                    {
                        Jouer.gameFini = true;
                        Jouer.EstPerdant = true;
                    }
                        //et ici on a gagner
                    else if (Jouer.HpEnnemi <= 0)
                    {
                        Jouer.gameFini = true;
                        Jouer.EstGagnant = true;
                    }
                }
            }
        }
            //si on joue un sort
        else if ((this.name != "hero" || this.name != "hero ennemis") && Cout.Length != 0 && !Jouer.enTrainCaster && Cout[8].text == "Sort" && Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            //on réduit la mana 
            Jouer.targetNeeded = false;
            Jouer.effet = "";
            Jouer.spellTarget = "";
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            //on vérifie le texte sans saut de ligne et sans espace
            Jouer.texteHabileteSansNewline = Cout[7].text.Replace('\n', ' ');
            Jouer.texteHabileteSansEspace = Jouer.texteHabileteSansNewline.Split(new char[] { ' ' });
            //on vérifie si c'est un ennemis
            Jouer.isEnnemi = isEnnemi(Jouer.texteHabileteSansEspace[0]);
            Jouer.spell = this.gameObject;
            //on trouve la position du sort
            Jouer.position = TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCarteJoueur);
            //on set la position du sort que nous somme en train de jouer
            Jouer.posCarteEnTrainCaster = trouverPosEnTrainCaster(this.gameObject);
            //on bouge le sort de palce
            this.transform.position = new Vector3(-5.4f, 0.0f, 6.0f);
            Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
            //effet habilité
            Jouer.effet = trouverTypeEffet(Jouer.texteHabileteSansEspace);
            //target qui recoit le spell
            Jouer.spellTarget = trouverTypeTarget(Jouer.texteHabileteSansEspace);

            //on trouve si le sort a besoin d'une cible
            if (isATargetNeeded(trouverTypeTarget(Jouer.texteHabileteSansEspace)))
                Jouer.targetNeeded = true;
            else
                Jouer.targetNeeded = false;
            //si il n'y a pas de cible alors le sort ne touche pas seulement une cible
            if (!Jouer.targetNeeded)
            {
                //on ne laise pas le joueur faire d'autre action
                waitForActionDone();
                //on fait l'action du sort
                //inflige
                if (Jouer.effet == "Inflige")
                {
                    if (Jouer.spellTarget != "herosennemis")
                    {
                        for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        {
                            if (Jouer.ZoneCombatEnnemie[i].carte != null)
                                spellBurn(Jouer.spellTarget, trouverNbDegat(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                            if (isOnBothPlayers(Jouer.spellTarget))
                            {
                                if (Jouer.ZoneCombat[i].carte != null)
                                    spellBurn(Jouer.spellTarget, trouverNbDegat(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                            }
                        }
                    }
                    if (isOnHeros(Jouer.spellTarget))
                    {
                        burnHeros("Ennemi", trouverNbDegat(Jouer.texteHabileteSansEspace));
                        if (isOnBothHeros(Jouer.spellTarget))
                            burnHeros("Allier", trouverNbDegat(Jouer.texteHabileteSansEspace));
                    }
                }
                    //endort
                else if (Jouer.effet == "Endort")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombatEnnemie[i].carte != null)
                            spellSleep(Jouer.spellTarget, trouverNbTour(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        if (spellCreaturesBothSides(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombat[i].carte != null)
                                spellSleep(Jouer.spellTarget, trouverNbTour(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        }

                    }
                }
                    //transforme
                else if (Jouer.effet == "Transforme")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombatEnnemie[i].carte != null)
                            spellTransform(Jouer.spellTarget, trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombat[i].carte != null)
                                spellTransform(Jouer.spellTarget, trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        }

                    }
                }
                    //detruit
                else if (Jouer.effet == "Detruit")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombatEnnemie[i].carte != null)
                            spellDestroy(Jouer.spellTarget, Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombat[i].carte != null)
                                spellDestroy(Jouer.spellTarget, Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        }
                    }
                }
                    //soigne
                else if (Jouer.effet == "Soigne")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombat[i].carte != null)
                            spellHeal(Jouer.spellTarget, trouverNbVie(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombatEnnemie[i].carte != null)
                                spellHeal(Jouer.spellTarget, trouverNbVie(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);

                            if (isOnHeros(Jouer.spellTarget))
                            {
                                healHeros("Allier", trouverNbVie(Jouer.texteHabileteSansEspace));
                                if (isOnBothHeros(Jouer.spellTarget))
                                    healHeros("Ennemi", trouverNbVie(Jouer.texteHabileteSansEspace));
                            }
                        }
                    }
                }
                    //donne
                else if (Jouer.effet == "Donne")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombat[i].carte != null && Jouer.ZoneCombat[i].EstOccupee)
                            spellBuff(Jouer.spellTarget, Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombatEnnemie[i].carte != null && Jouer.ZoneCombatEnnemie[i].EstOccupee)
                                spellBuff(Jouer.spellTarget, Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        }
                    }
                }
            }
            //si on a besoin d'une cible on dit que nous sommes en train de jouer le sort
            if (Jouer.targetNeeded)
                Jouer.enTrainCaster = true;
            else
            {
                //on détruit le sort
                EstJouer = true;
                Destroy(Jouer.spell, 1);
                //on crée un string de la carte
                string spellCarteString = SetCarteString(Jouer.ZoneCarteJoueur[Jouer.position].carte);
                //on envoie le message au serveur
                envoyerMessage("Jouer spellnotarget." + Jouer.spell.name +"." + spellCarteString);
                StartCoroutine(waitEnvoyer(0.75f));
                //on laisse le joeuur faire des actions
                //on dit que la carte n'est plus presente dans la main et le jeu
                Jouer.ZoneCarteJoueur[Jouer.position].carte = null;
                Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
                Jouer.spell = null;
                //si un joueur meurt on dit le bon message selon le résultat 
                if (Jouer.HpEnnemi <= 0 && Jouer.HpJoueur <= 0)
                {
                    Jouer.gameFini = true;
                    Jouer.EstNul = true;
                }
                else if (Jouer.HpJoueur <= 0)
                {
                    Jouer.gameFini = true;
                    Jouer.EstPerdant = true;
                }
                else if (Jouer.HpEnnemi <= 0)
                {
                    Jouer.gameFini = true;
                    Jouer.EstGagnant = true;
                }
            }
        }
        else if ((this.tag != "hero" || this.tag != "hero ennemis") && Cout.Length != 0 && !Jouer.enTrainCaster && getNbCarteZone(Jouer.ZoneCombat) < Jouer.ZoneCombat.Length && Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            //quand on veut jouer une créature ou un batiment
            //on ne laisse pas le joeuur faire d'autre action
            waitForActionDone();
            //on réduit la mana du joueur
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            //trouve ou on peut placer la carte sur la zone de combat
            int PlacementZoneCombat = Jouer.TrouverOuPlacerCarte(Jouer.ZoneCombat);
            Vector3 temp = this.transform.position;
            this.transform.position = Jouer.ZoneCombat[PlacementZoneCombat].Pos;
            EstJouer = true;
            //position de la carte dans les mains du joueur
            Emplacement = TrouverEmplacementCarteJoueur(temp, Jouer.ZoneCarteJoueur);
            if (Emplacement != -1)
            {
                //on l'enleve des mains et on la met dans la place de combat
                Jouer.ZoneCarteJoueur[Emplacement].EstOccupee = false;
                Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = true;
                Jouer.ZoneCombat[PlacementZoneCombat].carte = Jouer.ZoneCarteJoueur[Emplacement].carte;
                Jouer.ZoneCarteJoueur[Emplacement].carte = null;
                Jouer.styleCarteAlliercombat[PlacementZoneCombat] = this.gameObject;
                //si la carte a un habilete special
                if (Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
                {
                    //on trouve quelle est l'habilete
                    string[] lesHabiletes = Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.habilityspecial.Split(new char[] { ',' });
                    for (int i = 0; i < lesHabiletes.Length; ++i)
                    {
                        string[] zeSpecialHability = lesHabiletes[i].Split(new char[] { ' ' });
                        //si donne
                        if (zeSpecialHability[0] == "Donne")
                        {
                            //on set les bonus
                            setStatbonus(zeSpecialHability);
                            setStat(Jouer.ZoneCombat, PlacementZoneCombat);
                        }
                            //si ajoute
                        else if (zeSpecialHability[0] == "Ajoute")
                            setManaBonus(zeSpecialHability);
                        //on ajoute la mana bonus
                    }
                }
                else
                {
                    //on modifie les stats selon ce que les créature donnes en bonus
                    setStat(Jouer.ZoneCombat, PlacementZoneCombat);
                }
                //on le dit au serveur
                envoyerMessage("Jouer Carte." + this.name);
                EnvoyerCarte(connexionServeur.sck,Jouer.ZoneCombat[PlacementZoneCombat].carte);
                //on laisse le joueur faire des actions
                StartCoroutine(waitEnvoyer(0.75f));
            }
        }
    }
    //meme chose
    private void enleverBonusStat(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteAttaque = data[2];
        enevlerStat(Jouer.ZoneCombat, sorteAttaque, num);
    }
    //quand le batiment  meurt on enleve les stat bonus
    private void enevlerStat(PosZoneCombat[] zone, string stat, int valeur)
    {
        if (stat == "pV")
            Jouer.vieBonus -= valeur;
        else if (stat == "pAtt")
            Jouer.attaqueBonus -= valeur;
        else if (stat == "pArm")
            Jouer.armureBonus -= valeur;
    }
    //meme chose
    private void setStatbonus(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteAttaque = data[2];
        setBonusStat(Jouer.ZoneCombat, sorteAttaque, num);
    }
    //ajoute les stats bonus
    private void setBonusStat(PosZoneCombat[] zone, string stat, int valeur)
    {
        if (stat == "pV")
            Jouer.vieBonus += valeur;
        else if (stat == "pAtt")
            Jouer.attaqueBonus += valeur;
        else if (stat == "pArm")
            Jouer.armureBonus += valeur;
    }
    //change les stats des créatures
    private void setStat(PosZoneCombat[] zone, int pos)
    {
        GameObject temp = null;
        if (Jouer.ZoneCombat[pos].carte != null)
            temp = getGameObjet(Jouer.styleCarteAllier, zone, pos);
        if (temp != null)
            setStats(temp, zone[pos].carte, Jouer.attaqueBonus, Jouer.armureBonus, Jouer.vieBonus);
    }
    //modifie l'attaque l'armure et la vie
    private void setStats(GameObject a, Carte c, int attaqueB, int armureB, int vieB)
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
    //crée un string selon une carte
    private string SetCarteString(Carte temp)
    {
        //si c'est une créature ou un baitment
        if (temp.TypeCarte == "Permanents")
        {
            /*0                    1                     2                   3                      4                      5                    6                     7                            8                   9                         10*/
            return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte + "." + temp.perm.Attaque + "." + temp.perm.Vie + "." + temp.perm.Armure + "." + temp.perm.TypePerm;
        }

        //sinon sort
        else
        {
            return temp.CoutBle + "." + temp.CoutBois + "." + temp.CoutGem + "." + temp.Habilete + "." + temp.TypeCarte + "." + temp.NomCarte + "." + temp.NoCarte;
        }
    }
    //retourne le gameObject selon une zone et le tableau de gameobject
    private GameObject getGameObjet(GameObject[] tab, PosZoneCombat[] carte, int pos)
    {
        for (int i = 0; i < tab.Length; ++i)
        {
            if (tab[i] != null && tab[i].transform.position.Equals(carte[pos].Pos))
                return tab[i].gameObject;
        }
        return null;
    }
    //ajoute la mana au joueur
    private void setManaBonus(string[] data)
    {
        //nombre de ressource donnée
        int num = getNumBonus(data[1]);
        //type de ressource
        string sorteMana = data[2];
        if (sorteMana == "bois")
        {
            Jouer.NbBois += num;
            Jouer.joueur1.nbBois += num;
        }
        else if (sorteMana == "gem")
        {
            Jouer.NbGem += num;
            Jouer.joueur1.nbGem += num;
        }
        else if (sorteMana == "ble")
        {
            Jouer.NbBle += num;
            Jouer.joueur1.nbBle += num;
        }
    }
    //retourne le chiffre que l'habilete va donner en bonus
    private int getNumBonus(string laplace)
    {
        char[] tab = { ' ', '+' };
        string nombre = laplace.Trim(tab);
        return int.Parse(nombre);
    }
    //retourne si le mot est une habilete de créature
    private bool getHabilete(string mot)
    {
        return mot == "Donne" || mot == "Ajoute";
    }
    //slepp unity mais restart apres
    public IEnumerator waitEnvoyer(float i)
    {
        yield return new WaitForSeconds(i);
        restart();
    }
    //sleep unity
    public IEnumerator wait(float i)
    {
        yield return new WaitForSeconds(i);
    }
    //quand on reste assez longtemps on met un clone
    void OnMouseOver()
    {
        delay += Time.deltaTime;
        //// on augmente le temps du delay apres 1f on fait un clone
        //si ce n'est pas le heros
        if (this.name != "hero" && this.name != "hero ennemis")
        {
            //si ce n'est pas le clone et qu'il n'y a pas de clone
            if (delay >= 1f && cloneCarte == null && !estClone)
            {
                //create clone
                clonetransform = Instantiate(this.gameObject.transform) as Transform;
                clonetransform.tag = "clone";
                cloneCarte = clonetransform.gameObject;
                cloneCarte.GetComponent<JouerCarteBoard>().estClone = true;
                //grossir
                cloneCarte.transform.localScale = new Vector3(cloneCarte.transform.localScale.x * 2, cloneCarte.transform.localScale.y * 2, cloneCarte.transform.localScale.y);
                //changer de position
                cloneCarte.transform.position = new Vector3(6.5f, -0.5f, 1);
            }
        }
    }
    //quand qu'on quite la carte on détruit le clone
    void OnMouseExit()
    {
        delay = 0;
        //si il y a un clone
        if (cloneCarte != null)
        {
            //on le met invisible et apres on le détruit
            Color c = cloneCarte.renderer.material.color;
            c.a = 0f;
            Destroy(cloneCarte);
            cloneCarte = null;
        }
    }
    //////////---------Communication serveur---------/////////
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
        //StartCoroutine(wait(0.5f));
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
        //StartCoroutine(wait(0.5f));
    }
    //////////---------fin Communication serveur---------/////////
    //fait que le joueur ne peut pas faire d'autre action pendant que le serveur s'occupe de son action
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
