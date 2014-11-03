#pragma strict
//Variable
var Background: GUIStyle;

function Start () {

}

function Update () {

}

function OnGUI(){
	GUI.Box(Rect(0,0,Screen.width,Screen.height),"",Background);
	GUI.Label(Rect(Screen.width*0.45f,Screen.height*0.9f,Screen.width*0.07f,Screen.height*0.03f),"Loadin...");
}