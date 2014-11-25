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
    public float delay;
    public Jouer Script_Jouer;
    public bool EstJouer = false;
    public bool EstEnnemie = false;

    GameObject target;
    GameObject spell;
    Transform clonetransform;
    GameObject cloneCarte;
    bool estClone;
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

    void OnDestroy()
    {
        if (EstJouer && !estClone)
        {
            if (EstEnnemie)
                Jouer.ZoneCombatEnnemie[TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombatEnnemie)].EstOccupee = false;
            else
            {
                int PlacementZoneCombat = TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCombat);

                if (PlacementZoneCombat != -1)
                {
                    Jouer.ZoneCombat[PlacementZoneCombat].EstOccupee = false;
                    if (Jouer.ZoneCombat[PlacementZoneCombat] != null && Jouer.ZoneCombat[PlacementZoneCombat].carte!= null && Jouer.ZoneCombat[PlacementZoneCombat].carte.perm!= null && Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
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

    private bool spellSummon(string target, int nbCreatures, int[] stats, PosZoneCombat[] zone)
    {
        bool valide = false; int nbSummon = nbCreatures; int placeDisponnible = 0;
        for (int i = 0; i < zone.Length; i++)
        {
            if (!zone[i].EstOccupee)
                placeDisponnible++;
        }
        if (nbSummon > placeDisponnible)
            nbSummon = placeDisponnible;

        //Créer les summons
        return !valide;
    }
    private bool spellBurn(string target, int nbDegat, Carte zeTarget, GameObject t, int posTarget)
    {
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemis" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemis")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
            else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
                if (zeTarget.perm.TypePerm == "Batiment")
                    valide = true;

        if (valide)
        {
            if ((zeTarget.perm.Armure - nbDegat) >= 0)
                zeTarget.perm.Armure -= nbDegat;
            else
            {
                int difference = nbDegat - zeTarget.perm.Armure;
                zeTarget.perm.Armure = 0;
                zeTarget.perm.Vie -= difference;
                if (zeTarget.perm.Vie <= 0)
                {
                    Destroy(t, 1);
                    Jouer.ZoneCombatEnnemie[posTarget].EstOccupee = false;
                    Jouer.ZoneCombatEnnemie[posTarget].carte = null;
                    zeTarget = null;
                }
            }

            if (t != null && zeTarget != null)
            {
                TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
                stat[3].text = zeTarget.perm.Armure.ToString();
                stat[5].text = zeTarget.perm.Vie.ToString();
            }
        }
        return !valide;
    }
    private bool spellHeal(string target, int nbVie, Carte zeTarget, GameObject t, int posTarget)
    {
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemis" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemis")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
            else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
                if (zeTarget.perm.TypePerm == "Batiment")
                    valide = true;

        if ((zeTarget.perm.Vie + nbVie) >= zeTarget.perm.basicVie)
            zeTarget.perm.Vie = zeTarget.perm.basicVie;
        else
            zeTarget.perm.Vie += nbVie;

            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[5].text = zeTarget.perm.Vie.ToString();

        return !valide;
    }
    private bool spellSleep(string target, int nbTour, Carte zeTarget, GameObject t, int posTarget)
    {
        bool valide = false;
        if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemis")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;

        if (valide)
        {
            zeTarget.perm.estEndormi = nbTour;

            //EnvoyerCarte(connexionServeur.sck, zeTarget);
        }

        return !valide;
    }
    private bool spellDestroy(string target, Carte zeTarget, GameObject t, int posTarget)
    {
        bool valide = false;
        if (target == "cible" || target == "ennemis" || target == "touteslescartesennemis" || target == "touteslescartes" || target == "placedecombat")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemis")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
            else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
                if (zeTarget.perm.TypePerm == "Batiment")
                    valide = true;
        if (valide)
        {
            Destroy(t, 1);
            Jouer.ZoneCombatEnnemie[posTarget].EstOccupee = false;
            zeTarget = null;
        }
        return !valide;
    }
    private bool spellTransform(string target, int[] stats, Carte zeTarget, GameObject t, int posTarget)
    {
        //Vie attaque armure
        ////////////////////

        bool valide = false;
        if (target == "cible" || target == "touteslescartes")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesennemis")
        {
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        }
        else if (target == "batiment" || target == "touslesbatiments" || target == "touslesbatimentsennemis")
        {
            if (zeTarget.perm.TypePerm == "Batiment")
                valide = true;
        }

        if (valide)
        {
            zeTarget.perm.Vie = stats[2];
            zeTarget.perm.Attaque = stats[1];
            zeTarget.perm.Armure = stats[0];
            zeTarget.perm.basicVie = stats[2];
            zeTarget.perm.basicAttaque = stats[1];
            zeTarget.perm.basicArmor = stats[0];
        }

        if (t != null)
        {
            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();
            stat[3].text = zeTarget.perm.Armure.ToString();
            stat[4].text = zeTarget.perm.Attaque.ToString();
            stat[5].text = zeTarget.perm.Vie.ToString();
        }

        return !valide;
    }
    private bool spellBuff(string target, Carte zeTarget, GameObject t, int posTarget)
    {
        bool valide = false;
        string buffHabilete = "";
        int buffVie = 0; int buffAttaque = 0; int buffArmure = 0;
        if (target == "cible")
            valide = true;
        else if (target == "creature" || target == "touteslescreatures" || target == "touteslescreaturesalliees")
            if (zeTarget.perm.TypePerm == "Creature")
                valide = true;
        if (valide)
        {
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

            zeTarget.perm.Vie += buffVie;
            zeTarget.perm.basicVie += buffVie;
            zeTarget.perm.Attaque += buffAttaque;
            zeTarget.perm.basicAttaque += buffAttaque;
            zeTarget.perm.Armure += buffArmure;
            zeTarget.perm.basicArmor += buffArmure;
            if (buffHabilete == "provocateur")
            {
                if(!zeTarget.perm.estInvisible)
                    zeTarget.perm.estTaunt = true;
            }
            else if (buffHabilete == "attaque puissante")
            {
                zeTarget.perm.estAttaquePuisante = true;
            }
            else if (buffHabilete == "attaque double")
            {
                zeTarget.perm.estAttaqueDouble = true;
            }

            TextMesh[] stat = t.GetComponentsInChildren<TextMesh>();

            stat[3].text = zeTarget.perm.Armure.ToString();
            stat[4].text = zeTarget.perm.Attaque.ToString();
            stat[5].text = zeTarget.perm.Vie.ToString();
            stat[7].text = zeTarget.Habilete.ToString();
        }

        return !valide;
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
        string target = ""; int i = 0;
        bool pasTrouver = true;
        while (pasTrouver || i > motsHabilete.Length)
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
                    if (i+3 < motsHabilete.Length && (motsHabilete[i + 3] == "ennemis" || motsHabilete[i+3] == "alliees"))
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
                    target = motsHabilete[i] + motsHabilete[i + 1] + motsHabilete[i + 2];
                    pasTrouver = false;
                }
            }
            else if (motsHabilete[i] == "heros")
            {
                if (motsHabilete[i + 1] == "ennemi")
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
        int[] stats = { 0, 0, 0 };
        int vie = 0; int attaque = 0; int armure = 0;

        string[] tabStat = motsHabilete[motsHabilete.Length - 1].Split(new char[] { '/' });
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
    private bool isEffet(string mot)
    {
        return mot == "Invoque" || mot == "Inflige" || mot == "Soigne" || mot == "Endort" || mot == "Transforme" || mot == "Donne" || mot == "Detruit";
    }
    private bool isEnnemi(string mot)
    {
        return mot == "Inflige" || mot == "Endort" || mot == "Transforme" || mot == "Detruit";
    }
    private bool isATargetNeeded(string mot)
    {
        return mot == "cible" || mot == "creature" || mot == "batiment";
    }
    private bool isOnBothPlayers(string target)
    {
        return target == "touteslescartes" || target == "touteslescreatures" || target == "touslesbatiments" || target == "placedecombat";
    }
    private bool healHeros(string quelJoueur, int nbVie)
    {
        Script_Jouer = GetComponent<Jouer>();
        if (quelJoueur == "Allier")
        {
            if (Jouer.HpJoueur + nbVie >= 30)
                Jouer.HpJoueur = 30;
            else
                Jouer.HpJoueur += nbVie;
        }
        else
        {
            if (Jouer.HpEnnemi + nbVie >= 30)
                Jouer.HpEnnemi = 30;
            else
                Jouer.HpEnnemi += nbVie;
        }
        return true;
    }
    public bool burnHeros(string quelJoueur, int nbDegat)
    {
        Script_Jouer = GetComponent<Jouer>();
        if (quelJoueur == "Allier")
            Jouer.HpJoueur -= nbDegat;
        else
            Jouer.HpEnnemi -= nbDegat;
        return true;
    }
    public bool isOnHeros(string target)
    {
        return target == "placedecombat" || target == "touslesennemis";
    }
    public bool isOnBothHeros(string target)
    {
        return target == "placedecombat";
    }
    public bool spellCreaturesBothSides(string target)
    {
        return target == "touteslescreatures";
    }

    void OnMouseDown()
    {
        Jouer.target = null;
        int Emplacement = 0;
        int posTarget;
        if(Jouer.enTrainCaster)
        {
            if (Jouer.targetNeeded)
            {
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit carte;
                if (Physics.Raycast(ray, out carte))
                {
                    Jouer.target = GameObject.Find(carte.collider.gameObject.name);
                    
                    if(Jouer.target != null && (Jouer.target.name == "hero ennemis" || Jouer.target.name == "hero"))
                    {
                        if (Jouer.effet == "Inflige")
                        {
                            if (Jouer.target.name == "hero ennemis")
                                Jouer.enTrainCaster = burnHeros("Ennemi", trouverNbDegat(Jouer.texteHabileteSansEspace));
                            else
                                Jouer.enTrainCaster = burnHeros("Allier", trouverNbDegat(Jouer.texteHabileteSansEspace));
                        }
                        else if(Jouer.effet == "Soigne")
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
                        if (! isEnnemi(Jouer.effet))
                        {
                            posTarget = TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombat);
                            Jouer.carteTarget = Jouer.ZoneCombat[posTarget].carte;

                            if (TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombat) != -1)
                            {
                                if (Jouer.effet == "Soigne")
                                    Jouer.enTrainCaster = spellHeal(Jouer.spellTarget, trouverNbVie(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[posTarget].carte, Jouer.target, posTarget);
                                else if (Jouer.effet == "Donne")
                                    Jouer.enTrainCaster = spellBuff(Jouer.spellTarget, Jouer.ZoneCombat[posTarget].carte, Jouer.target, posTarget);
                            }
                        }
                        else
                        {
                            if (TrouverEmplacementCarteJoueur(Jouer.target.transform.position, Jouer.ZoneCombatEnnemie) != -1)
                            {
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
                if (!Jouer.enTrainCaster)
                {
                    Destroy(Jouer.spell,1);
                    envoyerMessage("Jouer spellTarget." + Jouer.spell.name +"." + Jouer.target.name);
                    wait(1);
                    EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCarteJoueur[Jouer.position].carte);
                    wait(1);
                    if (Jouer.target.name != "hero ennemis" || Jouer.target.name != "hero")
                        EnvoyerCarte(connexionServeur.sck, Jouer.carteTarget);

                    Jouer.ZoneCarteJoueur[Jouer.position].carte = null;
                    Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
                    Jouer.spell = null;
                }
            }
        }
        else if ((this.tag != "hero" || this.tag != "hero ennemis") && !Jouer.enTrainCaster && Cout[8].text == "Sort" && Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
        {
            Jouer.targetNeeded = false;
            Jouer.effet = "";
            Jouer.spellTarget = "";
            Jouer.joueur1.nbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.joueur1.nbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.joueur1.nbGem -= System.Int32.Parse(Cout[2].text);

            Jouer.NbBle -= System.Int32.Parse(Cout[1].text);
            Jouer.NbBois -= System.Int32.Parse(Cout[0].text);
            Jouer.NbGem -= System.Int32.Parse(Cout[2].text);
            Jouer.texteHabileteSansNewline = Cout[7].text.Replace('\n', ' ');
            Jouer.texteHabileteSansEspace = Jouer.texteHabileteSansNewline.Split(new char[] { ' ' });
            Jouer.isEnnemi = isEnnemi(Jouer.texteHabileteSansEspace[0]);
            Jouer.spell = this.gameObject;
            Jouer.position = TrouverEmplacementCarteJoueur(this.transform.position, Jouer.ZoneCarteJoueur);
            this.transform.position = new Vector3(-5.4f, 0.0f, 6.0f);
            EstJouer = true;
            Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
            //effet habilité
            Jouer.effet = trouverTypeEffet(Jouer.texteHabileteSansEspace);
            //target qui recoit le spell
            Jouer.spellTarget = trouverTypeTarget(Jouer.texteHabileteSansEspace);

            if (isATargetNeeded(trouverTypeTarget(Jouer.texteHabileteSansEspace)))
                Jouer.targetNeeded = true;
            else
                Jouer.targetNeeded = false;

            if(!Jouer.targetNeeded)
            {
                if (Jouer.effet == "Inflige")
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
                    if (isOnHeros(Jouer.spellTarget))
                    {
                        burnHeros("Ennemi", trouverNbDegat(Jouer.texteHabileteSansEspace));
                        if (isOnBothHeros(Jouer.spellTarget))
                            burnHeros("Allier", trouverNbDegat(Jouer.texteHabileteSansEspace));
                    }
                }
                else if (Jouer.effet == "Invoque")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                        spellSummon(Jouer.spellTarget, trouverNbSummon(Jouer.texteHabileteSansEspace), trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat);
                }
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
                else if (Jouer.effet == "Transforme")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if(Jouer.ZoneCombatEnnemie[i].carte != null)
                            spellTransform(Jouer.spellTarget, trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        if(isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombat[i].carte != null)
                                spellTransform(Jouer.spellTarget, trouverStats(Jouer.texteHabileteSansEspace), Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        }
                        
                    }
                }
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
                else if (Jouer.effet == "Soigne")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombat[i].carte != null)
                            spellHeal(Jouer.spellTarget, trouverNbVie(Jouer.texteHabileteSansEspace) ,Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if(Jouer.ZoneCombatEnnemie[i].carte != null)
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
                else if(Jouer.effet == "Donne")
                {
                    for (int i = 0; i < Jouer.ZoneCombatEnnemie.Length; i++)
                    {
                        if (Jouer.ZoneCombat[i].carte != null)
                            spellBuff(Jouer.spellTarget, Jouer.ZoneCombat[i].carte, Jouer.styleCarteAlliercombat[i], i);
                        if (isOnBothPlayers(Jouer.spellTarget))
                        {
                            if (Jouer.ZoneCombatEnnemie[i].carte != null)
                                spellBuff(Jouer.spellTarget, Jouer.ZoneCombatEnnemie[i].carte, Jouer.styleCarteEnnemisCombat[i], i);
                        }
                    }
                }
            }
            if (Jouer.targetNeeded)
                Jouer.enTrainCaster = true;
            else
            {
                Destroy(Jouer.spell, 1);
                envoyerMessage("Jouer spellnotarget." + Jouer.spell.name);
                wait(1);
                EnvoyerCarte(connexionServeur.sck, Jouer.ZoneCarteJoueur[Jouer.position].carte);
                Jouer.ZoneCarteJoueur[Jouer.position].carte = null;
                Jouer.ZoneCarteJoueur[Jouer.position].EstOccupee = false;
                Jouer.spell = null;
            }
        }
        else if ((this.tag != "hero" || this.tag != "hero ennemis") && !Jouer.enTrainCaster && getNbCarteZone(Jouer.ZoneCombat) < Jouer.ZoneCombat.Length && Jouer.MonTour && !EstJouer && !EstEnnemie && Jouer.joueur1.nbBois >= System.Int32.Parse(Cout[0].text) && Jouer.joueur1.nbBle >= System.Int32.Parse(Cout[1].text) && Jouer.joueur1.nbGem >= System.Int32.Parse(Cout[2].text))
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
                if (Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.specialhability)
                {
                    string[] zeSpecialHability = Jouer.ZoneCombat[PlacementZoneCombat].carte.perm.habilityspecial.Split(new char[] { ' ' });
                    if (zeSpecialHability[0] == "Donne")
                    {
                        setStatbonus(zeSpecialHability);
                        setStat(Jouer.ZoneCombat, PlacementZoneCombat);
                    }
                    else if (zeSpecialHability[0] == "Ajoute")
                        setManaBonus(zeSpecialHability);
                }
                else
                {
                    setStat(Jouer.ZoneCombat, PlacementZoneCombat);
                }
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
    private void enleverBonusStat(string[] data)
    {
        int num = getNumBonus(data[1]);
        string sorteAttaque = data[2];
        enevlerStat(Jouer.ZoneCombat, sorteAttaque, num);
    }
    private void enevlerStat(PosZoneCombat[] zone, string stat, int valeur)
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
        setBonusStat(Jouer.ZoneCombat, sorteAttaque, num);
    }
    private void setBonusStat(PosZoneCombat[] zone, string stat, int valeur)
    {
        if (stat == "pV")
            Jouer.vieBonus += valeur;
        else if (stat == "pAtt")
            Jouer.attaqueBonus += valeur;
        else if (stat == "pArm")
            Jouer.armureBonus += valeur;
    }
    private void setStat(PosZoneCombat[] zone, int pos)
    {
        GameObject temp = null;
        if (Jouer.ZoneCombat[pos].carte != null)
            temp = getGameObjet(Jouer.styleCarteAllier, zone, pos);
        if (temp != null)
            setStats(temp, zone[pos].carte, Jouer.attaqueBonus, Jouer.armureBonus, Jouer.vieBonus);
    }
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
    private GameObject getGameObjet(GameObject[] tab, PosZoneCombat[] carte, int pos)
    {
        for (int i = 0; i < tab.Length; ++i)
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
        delay += Time.deltaTime;
        //// here the 2 is the time that you want before load the bar
        if(delay >=1f && cloneCarte == null &&  !estClone){
           //create clone
            clonetransform = Instantiate(this.gameObject.transform) as Transform;
            clonetransform.tag = "clone";
            cloneCarte = clonetransform.gameObject;
            cloneCarte.GetComponent<JouerCarteBoard>().estClone = true;
           //grossir
            cloneCarte.transform.localScale = new Vector3(cloneCarte.transform.localScale.x * 2, cloneCarte.transform.localScale.y * 2, cloneCarte.transform.localScale.y);
           //changer de position
            cloneCarte.transform.position = new Vector3(6.5f,-0.5f, 1);
         }
	}

	void OnMouseExit(){
        delay = 0;
        if (cloneCarte != null)
        {
            Destroy(cloneCarte);
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
