var appuyerTouche : GUIStyle;
var warOfBaraxa : GUIStyle;
var Background : GUIStyle;
var Logo : GUIStyle;
var GUIBox : GUIStyle;
var TextArea : GUIStyle;
var displayLabel = false;
var adresseServeur = false;
var ConfirmerAdresse = false;
var Flash = true;
var speed = 1.0;
var IP;

function Update() {
    if(Input.anyKeyDown){
        adresseServeur = true;
        Flash = false;
    }
    if(ConfirmerAdresse)
        Application.LoadLevel("Connexion");
			
}
	
function Start(){
    FlashLabel();
}


function OnGUI(){
    GUI.Box(Rect(0,0,Screen.width,Screen.height),"",Background);
    GUI.Box(Rect(35,0,Screen.width - 125,Screen.height - 125),"",Logo);
    appuyerTouche.fontSize = Screen.width/25;
    //warOfBaraxa.fontSize = Screen.width/10;
    if(displayLabel){
        GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.8f, Screen.width * 0.6f, Screen.height *0.2f), "Appuyer sur une touche pour continuer", appuyerTouche);
    }
    if(adresseServeur){
        GUI.Box(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), "Entrer l'adresse IP du serveur", GUIBox);
        IP = GUI.TextField(new Rect(Screen.width * 0.35f, Screen.height * 0.35f, Screen.width * 0.30f, Screen.height * 0.30f), IP,15,TextArea);
        if (GUI.Button(new Rect((Screen.width * 0.435f) - (Screen.width * 0.12f / 2), Screen.height * 0.62f, Screen.width * 0.12f, Screen.height * 0.06f), "Connecter", GUIBox))
        {
            ConfirmerAdresse = true;
        }
    }
}

function FlashLabel(){
    while (1 && Flash) {
        displayLabel = true;
        yield WaitForSeconds(.5);
        displayLabel = false;
        yield WaitForSeconds(.5); 
    }
}

