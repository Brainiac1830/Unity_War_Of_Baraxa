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
	public Transform bitch;
    GameObject card;
	int i = 0;




	// Use this for initialization
	void Start () {
        card = null;
	}

	void OnMouseDown()
	{
		Transform t = Instantiate(bitch, new Vector3 (0, 1, 0), Quaternion.Euler (new Vector3 (90, 180, 0))) as Transform;
		card = t.gameObject;
		i++;
		card.name = "card" + i.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (card != null)
        {
            x_ = card.renderer.bounds.size.x;
            y_ = card.renderer.bounds.size.y;
        }
	}

	void OnGUI()
	{
		GUI.Label (new Rect (x_, y_,100,100), "2");
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
