var guiON = true;
var profile = false;
var trouverPartie = false;
var quitter = false;

function OnMouseEnter(){
	renderer.material.color = Color.red;
}

function OnMouseExit(){
	renderer.material.color = Color.white;
}

function OnMouseUp(){
	if(trouverPartie){
		Application.LoadLevel("Loading");
	}
	else if(profile){
		Application.LoadLevel("Profile");
	}
	else {
		guiON = true;
	}
		
}


function OnGUI(){
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