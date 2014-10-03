#pragma strict
var ble :Texture;
var bois : Texture;
var gem : Texture;
var Worker :Texture;
var NbBle : int;
var NbBois :int;
var NbGem :int;
var NbWorkerMax :int;
var NbWorker :int;

function Start () {
 NbBle=0;
 NbBois=0;
 NbGem=0;
 NbWorkerMax=2;
 NbWorker= NbWorkerMax;
}

function Update () {

}
function OnGUI(){
	var e:Event;
	e=Event.current;
	var BtnBle = GUI.Button(Rect(Screen.width/1.30f, Screen.height/1.3f, 30, 30),ble);
	if(BtnBle){
		NbBle=SetMana(e,NbBle);
	}
	GUI.Label(Rect(Screen.width/1.30f, Screen.height/1.2f,100,100), "blé: "+ NbBle.ToString());
	
	var BtnBois = GUI.Button(Rect((Screen.width/1.20f), Screen.height/1.3f, 30, 30),bois);
	if(BtnBois){
		NbBois=SetMana(e,NbBois);
	}
	GUI.Label(Rect(Screen.width/1.20f, Screen.height/1.2f, 100, 100), "bois: " + NbBois.ToString());
	
	var BtnGem=GUI.Button(Rect((Screen.width/1.10f), Screen.height/1.3f, 30, 30),gem);
	if(BtnGem){
		NbGem=SetMana(e,NbGem);
	}
	GUI.Label(Rect(Screen.width/1.10f, Screen.height/1.2f, 100, 100), "gem: " + NbGem.ToString());
	
	GUI.Label(Rect(Screen.width/1.25f,Screen.height/1.15,100,75),Worker);
	GUI.Label(Rect(Screen.width/1.22f,Screen.height/1.04f,100,100),"Worker: " + NbWorker.ToString());
}
function SetMana(event:Event,ressource:int):int{
	if(event.button==0 && NbWorker>0){
		ressource++;
		NbWorker--;
	}
	else if(event.button==1 && ressource!=0 && NbWorker< NbWorkerMax){
		ressource--;
		NbWorker++;
	}
	return ressource;
}