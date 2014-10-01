var warOfBaraxa : GUIStyle;
var text : GUIStyle;
var GUIButton : GUIStyle;

function OnGUI(){
	warOfBaraxa.fontSize = Screen.width/10;
	text.fontSize = Screen.width/30;
	GUIButton.fontSize = Screen.width/45;
		GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
		
		//Victoires
		GUI.Label(Rect(Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.10f, Screen.height * 0.05f), "Victoires :", text);
		GUI.TextField(Rect(Screen.width * 0.35f, Screen.height * 0.3f, Screen.width * 0.03f, Screen.height * 0.05f), "10", text);
		
		//Défaites
		GUI.Label(Rect(Screen.width * 0.6f, Screen.height * 0.3f, Screen.width * 0.10f, Screen.height * 0.05f), "Defaites :", text);
		GUI.TextField(Rect(Screen.width * 0.75f, Screen.height * 0.3f, Screen.width * 0.03f, Screen.height * 0.05f), "3", text);
		
		//Rechercher un joueur
		GUI.Button(Rect(Screen.width * 0.3f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Rechercher", GUIButton);
		//Retour
		if(GUI.Button(Rect(Screen.width * 0.55f, Screen.height * 0.9f, Screen.width * 0.15f, Screen.height * 0.07f), "Retour", GUIButton)){
			Application.LoadLevel("Menu");
		}
}