var appuyerTouche : GUIStyle;
var warOfBaraxa : GUIStyle;

function Update() {
		if(Input.anyKeyDown)
			Application.LoadLevel("Menu");
	}


function OnGUI(){
	appuyerTouche.fontSize = Screen.width/30;
	warOfBaraxa.fontSize = Screen.width/10;
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.8f, Screen.width * 0.6f, Screen.height *0.2f), "Appuyer sur une touche pour continuer", appuyerTouche);
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
}

