var guiON = false;
var profile = false;
var trouverPartie = false;
var quitter = false;
var test : GUIStyle;

function OnGUI(){
	if(GUI.Button(Rect(Screen.width/3, Screen.height / 3,120, 30), "Trouver une partie", test)){
		Application.LoadLevel("Lobby");
	}
	if(GUI.Button(Rect(Screen.width/3, Screen.height / 2.4,120, 30), "Profile", test)){
		Application.LoadLevel("Profile");
	}
	if(GUI.Button(Rect(Screen.width/3, Screen.height / 2,120, 30), "Quitter", test)){
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