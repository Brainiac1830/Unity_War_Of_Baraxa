#pragma strict
var Lobby : GUIStyle;
var warOfBaraxa : GUIStyle;


function OnGUI () {
	warOfBaraxa.fontSize = Screen.width/10;

	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
	GUI.Box(Rect(Screen.width/4.2, Screen.height/3,400, 300),"Liste des parties");
	
	if(GUI.Button(Rect(Screen.width/3.6, Screen.height/1.1,100, 30),"Créer")){
		Application.LoadLevel("Loading");
	};
	
	GUI.Button(Rect(Screen.width/2.3, Screen.height/1.1,100, 30),"Rejoindre");
	
	if(GUI.Button(Rect(Screen.width/1.7, Screen.height/1.1,100, 30),"Retour")){
		Application.LoadLevel("Menu");
	};
}
