using UnityEngine;
using System.Collections;
using warsofbaraxa;

public class script_Attaquer : MonoBehaviour {
	Carte  attaquant =  new Carte("test","permanent",1,1,1,null,3,1,1,"creature");
    Carte defenseur = new Carte("test2", "permanent", 1, 1, 1, null, 1, 1, 1, "creature");
    joueur joueurDef = new joueur();
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) /*&& location creature*/ && !attaquant.aAttaque)
		{
            /*if(enemis creature a taunt)
             {
               creature possible a attaquer = creature avec taunt
               heros possible a attaquer = false
             }
             else
             {
                creature possible a attaquer =  toute les creature
                heros possible a attaquer =true
             }
             afficher en une couleur differente les créatues possible a attaquer et heros aussi*/
		}
        if(Input.GetMouseButtonDown(0) /*&& creature de la couleur*/)
        {
            /*if(est creature)*/
            CombatCreatureToCreature(attaquant, defenseur);
            CombatCreatureToCreature(defenseur, attaquant);
            /*else heros*/
            joueurDef.vie-= attaquant.perm_.attaque_;

            attaquant.aAttaque=true;
        }
        else if(Input.GetMouseButtonDown(1))
        {
            /*enleve le click de la creature donc les creture reviennent normal*/        
        }
	}
    private void CombatCreatureToCreature(Carte attaque,Carte defense)
    {
        int defarmure = defenseur.perm_.armure_;
        int defvie = defenseur.perm_.vie_;
        int attaquantAttaque = attaquant.perm_.attaque_;
        if (attaquantAttaque >= defarmure)
        {
            attaquantAttaque -= defarmure;
            defenseur.perm_.armure_ = 0;
            defenseur.perm_.vie_ -= attaquant.perm_.attaque_;
        }
        else
        {
            defenseur.perm_.armure_ -= attaquantAttaque;
        }
    }
}
//3.si joueur la créature ne perd aucun point de vie
//sinon la créature qui attaque perd des points de vie et son armure ainsi que la créature qui recoit l'attaque
//4.si créature qui attaque meurt on la sort du board ainsi que la créature qui se fait attaquer
//sinon si le heros meurt la partie est finis et on revient au menu de départ
//sinon la créature ne peut plus attaquer a moins qu'elle a plus qu'un attaque