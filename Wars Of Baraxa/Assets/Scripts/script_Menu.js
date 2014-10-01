var guiON = false;
var profile = false;
var trouverPartie = false;
var quitter = false;
var test : GUIStyle;
var warOfBaraxa : GUIStyle;

function OnGUI(){
	test.fontSize = Screen.width/30;
	warOfBaraxa.fontSize = Screen.width/10;
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
	
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.40f,Screen.width * 0.3f, Screen.height * 0.05f), "Trouver une partie", test)){
		Application.LoadLevel("Loading");
	}
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.50f,Screen.width * 0.3f ,Screen.height * 0.05f), "Profile", test)){
		Application.LoadLevel("Connexion");
	}
	if(GUI.Button(Rect((Screen.width/2) - (Screen.width * 0.3f/2), Screen.height * 0.60f,Screen.width * 0.3f, Screen.height * 0.05f), "Quitter", test)){
		guiON = true;
	}


	if(guiON){
		GUI.Box (Rect (Screen.width*1.35/8,Screen.height*1.5/8,370,270), "You sure?");
		
		if(GUI.Button (Rect (Screen.width*1.5/8,Screen.height*2/4,150,60),"Confirmer")){
			Application.Quit();
		}
		
		if(GUI.Button (Rect (Screen.width*4.5/8,Screen.height*2/4,150,60), "Annuler")){
			guiON = false;
		}
	}
}