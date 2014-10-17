using UnityEngine;
using System.Collections;

public class script_Card : MonoBehaviour {

	/*string nom_ = "";
	int vie_ = 0;
	int armure_ = 0;
	int attaque_ = 0;
	int coutBle_ = 0;
	int coutBois_ = 0;
	int coutGem_ = 0;*/
	public float x_ = 0;
	public float y_ = 0;
	public Transform cardFront;
	public Transform cardBack;
    public GameObject cardF;
	public GameObject cardB;
	public float layer;
	int i = 0;




	// Use this for initialization
	void Start () {
		cardF = null;
		cardB = null;
		layer = 0.15f;
	}

	void OnMouseDown()
	{
		Transform F_cardFront = Instantiate(cardFront, new Vector3 (-3, -2, 0), Quaternion.Euler (new Vector3 (90, 0, 0))) as Transform;
		Transform F_cardBack = Instantiate(cardBack, new Vector3 (-3, -2, layer), Quaternion.Euler (new Vector3 (90, 180, 0))) as Transform;
		cardF = cardFront.gameObject;
		cardB = cardBack.gameObject;
		i++;
		F_cardFront.name = "Card" + i.ToString();
		F_cardBack.name = "Cardback" + i.ToString ();

		foreach (Transform child in F_cardFront) 
		{
			child.name = child.name + i;
			child.tag = "textStats";
		}


	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{

	}

	/*public void setCarte()
	{
		nom_ = "Troll méchant";
		vie_ = 5;
		armure_ = 1;
		attaque_ = 2;
		coutBle_ = 2;
		coutBois_ = 3;
		coutGem_ = 0;
	}*/
}
