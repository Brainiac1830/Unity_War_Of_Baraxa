var test : GUIStyle;
var speed = 1.0;

function OnGUI(){
	//GUI.Button(Rect(Screen.width/5, Screen.height / 1.1,300, 50), "Appuyer sur une touche pour continuer", test);
	
	GUI.TextField(Rect(Screen.width/5, Screen.height / 1.1,300, 50), "Appuyer sur une touche pour continuer", test);
}

function Update() {
		if(Input.anyKeyDown)
			Application.LoadLevel("Menu");
}
