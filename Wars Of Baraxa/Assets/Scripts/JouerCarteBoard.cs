using UnityEngine;
using System.Collections;

public class JouerCarteBoard : MonoBehaviour {
	public float delay;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown(){
	    this.transform.position = new Vector3 (-4+Jouer.pos, -1.50f, 6.0f);
		Jouer.pos+=1.5f;
	}

	void OnMouseOver(){
		delay += Time.deltaTime;
		// here the 2 is the time that you want before load the bar
		if(delay >=0.5f){
			//this.transform.position = new Vector3 (0,-1.50f, 6.0f);
			this.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
		}
	}

	void OnMouseExit(){
		delay = 0;
		this.transform.localScale = new Vector3 (0.1f, 1.0f, 0.13f);
	}
}
