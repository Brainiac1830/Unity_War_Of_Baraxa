var Background : GUIStyle;
var placerClick: boolean;
var ble :Texture2D;
var bois : Texture2D;
var gem : Texture2D;
var Worker :Texture2D;
var PlayerChar:Texture2D;
var EnnemieChar:Texture2D;
var NbBle : int;
var NbBois :int;
var NbGem :int;
var NbWorkerMax :int;
var NbWorker :int;
var NbBleEnnemis :int;
var NbBoisEnnemis :int;
var NbGemEnnemis :int;
function Update() {
}
	
function Start(){
 NbBle=0;
 NbBois=0;
 NbGem=0;
 NbBleEnnemis=0;
 NbBoisEnnemis=0;
 NbGemEnnemis=0;
 NbWorkerMax=2;
 NbWorker= NbWorkerMax;
 placerClick=false;
}


function OnGUI(){
	var e:Event;
	e=Event.current;
	GUI.Box(Rect(0,0,Screen.width,Screen.height),"",Background);
	//Héro Joueur
	GUI.Label(Rect(Screen.width*0.005,Screen.height*0.005,Screen.width*0.05, Screen.height*0.07),PlayerChar);
	//Héro Ennemie
	GUI.Label(Rect(Screen.width*0.005,Screen.height*0.005,Screen.width*0.05, Screen.height*0.07),EnnemieChar);
	//blé
	GUI.Label(Rect(Screen.width*0.005,Screen.height*0.005,Screen.width*0.05, Screen.height*0.07),ble);
	GUI.Label(Rect(Screen.width*0.005,Screen.height*0.07,Screen.width*0.09, Screen.height*0.07),"Blé: " + NbBleEnnemis.ToString());
	//bois
	GUI.Label(Rect(Screen.width*0.06,Screen.height*0.005,Screen.width*0.05,Screen.height*0.07),bois);
	GUI.Label(Rect(Screen.width*0.06,Screen.height*0.07,Screen.width*0.09,Screen.height*0.07),"Bois: " + NbBoisEnnemis.ToString());
	//gem
	GUI.Label(Rect(Screen.width*0.14,Screen.height*0.005,Screen.width*0.05,Screen.height*0.07),gem);
	GUI.Label(Rect(Screen.width*0.14,Screen.height*0.07,Screen.width*0.09,Screen.height*0.07),"gem: " + NbGemEnnemis.ToString());
	if(!placerClick){
		//BLE
	    if(GUI.Button(Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),ble)){
			NbBle=SetMana(e,NbBle);
		}
		GUI.Label(Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09, Screen.height*0.07), "blé: "+ NbBle.ToString());
		//BOIS
		if(GUI.Button(Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),bois)){
			NbBois=SetMana(e,NbBois);
		}
		GUI.Label(Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09, Screen.height*0.07), "bois: " + NbBois.ToString());
		//GEM
		if(GUI.Button(Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),gem)){
			NbGem=SetMana(e,NbGem);
		}
		GUI.Label(Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09, Screen.height*0.07), "gem: " + NbGem.ToString());
		//WORKER
		GUI.Label(Rect(Screen.width/1.3f,Screen.height/1.24,Screen.width*0.25, Screen.height*0.10),Worker);
		GUI.Label(Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15, Screen.height*0.1),"Worker: " + NbWorker.ToString());
		
		if(GUI.Button(Rect(Screen.width/1.12f,Screen.height/1.24,Screen.width*0.1, Screen.height*0.1),"Placer") && NbWorker==0){
			placerClick=true;
		}
	}
	else{
	
		GUI.enabled=false;
		GUI.Button(Rect(Screen.width/1.27f, Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),ble);
		GUI.Button(Rect((Screen.width/1.17f), Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),bois);
		GUI.Button(Rect((Screen.width/1.08f), Screen.height/1.10f, Screen.width*0.05, Screen.height*0.07),gem);
		
		GUI.enabled=true;
		GUI.Label(Rect(Screen.width/1.27f, Screen.height/1.02f,Screen.width*0.09, Screen.height*0.07), "blé: "+ NbBle.ToString());
		GUI.Label(Rect(Screen.width/1.17f, Screen.height/1.02f, Screen.width*0.09, Screen.height*0.07), "bois: " + NbBois.ToString());
		GUI.Label(Rect(Screen.width/1.08f, Screen.height/1.02f,Screen.width*0.09, Screen.height*0.07), "gem: " + NbGem.ToString());
		GUI.Label(Rect(Screen.width/1.3f,Screen.height/1.24,Screen.width*0.25, Screen.height*0.10),Worker);
		GUI.Label(Rect(Screen.width/1.23f,Screen.height/1.15f,Screen.width*0.15, Screen.height*0.1),"Worker: " + NbWorker.ToString());		
	}
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