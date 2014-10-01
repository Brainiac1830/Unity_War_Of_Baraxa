#pragma strict
var warOfBaraxa : GUIStyle;
var GUIBox : GUIStyle;
var GUIButton : GUIStyle;


function OnGUI () {
	warOfBaraxa.fontSize = Screen.width/10;
	GUIBox.fontSize = Screen.width/30;
	GUIButton.fontSize = Screen.width/30;

	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
	GUI.Box(Rect((Screen.width/2) - (Screen.width * 0.8f/2),Screen.height * 0.3f,Screen.width * 0.8f, Screen.height * 0.6f),"\nListe des parties", GUIBox);
	
	if(GUI.Button(Rect(Screen.width * 0.25, Screen.height * 0.91f,Screen.width * 0.15f, Screen.height * 0.07f),"Créer", GUIButton)){
		Application.LoadLevel("Loading");
	};
	
	GUI.Button(Rect(Screen.width * 0.45f, Screen.height * 0.91f,Screen.width * 0.15f, Screen.height * 0.07f),"Rejoindre", GUIButton);
	
	if(GUI.Button(Rect(Screen.width * 0.65f, Screen.height * 0.91f,Screen.width * 0.15f, Screen.height * 0.07f),"Retour", GUIButton)){
		Application.LoadLevel("Menu");
	};
}