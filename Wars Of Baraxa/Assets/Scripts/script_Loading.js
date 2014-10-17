#pragma strict
//Variable
var Background: GUIStyle;

function Start () {

}

function Update () {

}

function OnGUI(){
	GUI.Box(Rect(0,0,Screen.width,Screen.height),"",Background);
}