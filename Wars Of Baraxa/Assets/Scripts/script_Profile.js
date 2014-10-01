var warOfBaraxa : GUIStyle;

function OnGUI(){
	warOfBaraxa.fontSize = Screen.width/10;
		GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
}