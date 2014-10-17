using UnityEngine;
using System.Collections;
using warsofbaraxa;
public class attaque : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Attaquer();    
    }
    public void Attaquer()
    {
        for(int i=0;i<Jouer.tabCarteAllier.Length;++i)
        {
            Jouer.tabCarteAllier[i].perm.aAttaque = false;
            if (Jouer.tabCarteAllier[i]!= null &&!Jouer.tabCarteAllier[i].perm.aAttaque && Jouer.tabCarteAllier[i].perm.TypePerm == "creature" )
            {
            //change le border pour une autre couleur
                Jouer.styleCarteAllier[i].renderer.material.color = Color.green;
            //    //if(personne attaquer n'est pas le héros)
            //    CombatCreature(attaquant.perm, Defenseur.perm);
            //    CombatCreature(Defenseur.perm, attaquant.perm);
            //    //else
            //    playerDef.vie = CombatJoueur(attaquant.perm.Attaque, playerDef.vie);

            //    //si l'attaquant ou le defenseur n'on plus de vie on les enleve du board
            //    if (attaquant.perm.Vie <= 0)
            //        attaquant = null;
            //    if (Defenseur.perm.Vie <= 0)
            //        Defenseur = null;
            //    if (playerDef.vie <= 0)
            //        playerDef = null;

            //    attaquant.perm.aAttaque = true;
            }
            if(Jouer.tabCarteEnnemis[i]!= null)
            {
                Jouer.styleCarteEnnemis[i].renderer.material.color=Color.red;
            }
        }
    }
}
