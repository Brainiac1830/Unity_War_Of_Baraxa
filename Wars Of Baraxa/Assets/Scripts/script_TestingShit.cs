using UnityEngine;
using System.Collections;

public class script_TestingShit : MonoBehaviour {
	//cartes
	public GameObject card;
	public GameObject card1;
	public GameObject cardback;
	public GameObject cardback1;
	public GameObject[] newCardText;
	//color
	public Color color;
	//texte
	public GameObject coutGem;
	public GameObject coutBois;
	public GameObject coutBle;
	public GameObject attaque;
	public GameObject armure;
	public GameObject vie;
    public SpriteRenderer Image;
	//variables bool
	public bool tourne;
	public bool standBy;
	public bool front;
	public bool on;
	public bool onSwitch;
	public bool newCard;
	//variables floats
	public float totalRotation;
	public float rotationAmt;

	// Use this for initialization
	void Start () {
		//cartes
		card = GameObject.Find ("Card");
		cardback = GameObject.Find ("Cardback");
		//textes
		coutGem = GameObject.Find ("coutGem");
		coutBois = GameObject.Find ("coutBois");
		coutBle = GameObject.Find ("coutBle");
		attaque = GameObject.Find ("attaque");
		armure = GameObject.Find ("armure");
		vie = GameObject.Find ("vie");
		totalRotation = 0;
		rotationAmt = 30;
		//variables
		newCard = false;
		onSwitch = false;
		on = true;
		front = true;
		standBy = false;
		tourne = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (newCard) 
		{
			card1 = GameObject.Find("Card1");
			cardback1 = GameObject.Find("Cardback1");
			turnOffNewCardText();
			card1.transform.Translate(1 * Time.deltaTime, 0, 0);
			cardback1.transform.Translate(-1* Time.deltaTime,0,0);
		}

		if (newCard) 
		{
			card1.transform.Translate(1 * Time.deltaTime, 0, 0);
			cardback1.transform.Translate(-1* Time.deltaTime,0,0);
		}
			

		if (tourne) 
		{
			if (totalRotation < 180) 
			{
					card.transform.Rotate (0, 0, rotationAmt);
					cardback.transform.Rotate (0, 0, rotationAmt);
					totalRotation += rotationAmt;

				if (!onSwitch && totalRotation > 90) 
				{
					on = !on;
					onSwitch = true;
				}
			} 
			else
			{
				standBy = true;
				tourne = false;
				onSwitch = false;
			}
		}

		if(onSwitch)
		{
			if(on)
			{
				turnOnText();
			}
			else
			{
				turnOffText();
			}
		}

		if(!front && !tourne && standBy)
		{
			card.transform.rotation = Quaternion.Euler (new Vector3(90,180,0));
			cardback.transform.rotation = Quaternion.Euler (new Vector3(90,0,0));
			standBy = false;
			front = true;
			totalRotation = 0;
		}
		else if(front && !tourne && standBy)
		{
			card.transform.rotation = Quaternion.Euler (new Vector3(90,0,0));
			cardback.transform.rotation = Quaternion.Euler (new Vector3(90,180,0));
			standBy = false;
			front = false;
			totalRotation = 0;
		}

		/*if (Input.GetKeyDown(KeyCode)){
			tourne = true;
		}*/
	}

	public void turnOffText()
	{
		color = renderer.material.color;
		color.a = 0;

		coutGem.renderer.material.color = color;
		coutBois.renderer.material.color = color;
		coutBle.renderer.material.color = color;
		attaque.renderer.material.color = color;
		armure.renderer.material.color = color;
		vie.renderer.material.color = color;

	}

	public void turnOnText()
	{
		color = renderer.material.color;
		color.a = 1;
		
		coutGem.renderer.material.color = color;
		coutBois.renderer.material.color = color;
		coutBle.renderer.material.color = color;
		attaque.renderer.material.color = color;
		armure.renderer.material.color = color;
		vie.renderer.material.color = color;
	}

	public void turnOffNewCardText()
	{
		newCardText = GameObject.FindGameObjectsWithTag ("textStats");

		for(int i = 0; i < newCardText.Length; i++) 
		{
			color = newCardText[i].renderer.material.color;
			color.a = 0;
			newCardText[i].renderer.material.color = color;
		}
	}
}
