var guiON = false;
var profile = false;
var trouverPartie = false;
var quitter = false;
var test : GUIStyle;
var warOfBaraxa : GUIStyle;
var GUIBox : GUIStyle;
var GUIButton : GUIStyle;

function OnGUI(){
	test.fontSize = Screen.width/30;
	warOfBaraxa.fontSize = Screen.width/10;
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
	
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.40f,Screen.width * 0.3f, Screen.height * 0.05f), "Trouver une partie", test)){
		Application.LoadLevel("Lobby");
	}
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.50f,Screen.width * 0.3f ,Screen.height * 0.05f), "Profile", test)){
		Application.LoadLevel("Connexion");
	}
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.60f,Screen.width * 0.3f, Screen.height * 0.05f), "Quitter", test)){
		guiON = true;
	}


	if(guiON){
		GUIBox.fontSize = Screen.width/30;
		GUIButton.fontSize = Screen.width/40;
		GUI.Box (Rect (Screen.width*0.35f,Screen.height * 0.35f,Screen.width * 0.30f,Screen.height * 0.30f), "\nVoulez-vous \n vraiment quitter?", GUIBox);
		
		if(GUI.Button (Rect ((Screen.width * 0.36f) ,Screen.height * 0.55f,Screen.width * 0.135f,Screen.height * 0.07f),"Confirmer", GUIButton)){
			Application.Quit();
		}
		
		if(GUI.Button (Rect ((Screen.width * 0.43f) + (Screen.width * 0.15f/2),Screen.height * 0.55f,Screen.width * 0.135f,Screen.height * 0.07f), "Annuler", GUIButton)){
			guiON = false;
		}
	}
}