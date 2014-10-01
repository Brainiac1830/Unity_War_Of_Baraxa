//Variables
var alias : String = "";
var password : String = "";
var nom : String = "";
var prenom : String = "";
var nouveauCompte = false;
//GUIStyle
var textArea : GUIStyle;
var buttons : GUIStyle;
var text : GUIStyle;
var warOfBaraxa : GUIStyle;



function OnGUI() {
	warOfBaraxa.fontSize = Screen.width / 10;
	text.fontSize = Screen.width/45;
	textArea.fontSize = Screen.width/45;
	buttons.fontSize = Screen.width/45;
	
	//Titre
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.6f/2), Screen.height * 0.1f,Screen.width * 0.6f, Screen.height * 0.1f), "Wars of Baraxa", warOfBaraxa);
	//Alias
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.40f/2), Screen.height * 0.40f, Screen.width * 0.10f, Screen.height * 0.05f), "Alias", text);
	alias = GUI.TextField (Rect ((Screen.width/2) - (Screen.width * 0.25f/2), Screen.height * 0.40f, Screen.width * 0.25f, Screen.height * 0.05f), alias, 20, textArea);
	//Mot de passe
	GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.485f/2), Screen.height * 0.47f, Screen.width * 0.10f, Screen.height * 0.05f), "Mot de passe", text);
	password = GUI.PasswordField (Rect ((Screen.width/2) - (Screen.width * 0.25f/2), Screen.height * 0.47f, Screen.width * 0.25f, Screen.height * 0.05f),password, "*"[0] ,20, textArea);
	
	//Si veut se connecter a un compte
	if(!nouveauCompte){
		//Tente de se connecter avec les informations fournis
		if(GUI.Button(Rect((Screen.width * 0.435f) - (Screen.width * 0.12f/2), Screen.height * 0.54f, Screen.width * 0.12f, Screen.height * 0.06f), "Connecter", buttons)){
			//Conditions d'erreurs
			if(alias == "" || password == ""){
				//Erreur
			}
			else if (false){
				//Code pour vérifier si l'alias et le mot de passe 
				//correspondent a un enregistrement dans la base de donnée
			}
			else{
				//Connection et changement de page
			}
			
		}
		//Change l'interface pour pouvoir créer un compte
		if(GUI.Button(Rect((Screen.width * 0.564) - (Screen.width * 0.12f/2), Screen.height * 0.54f, Screen.width * 0.12f, Screen.height * 0.06f), "Créer", buttons)){
			nouveauCompte = true;
		}
	}
	//Si on veut créer un nouveau compte (plus de champs a remplir et boutons changent de place
	if(nouveauCompte){
		//Prenom
		GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.43f/2), Screen.height * 0.54f, Screen.width * 0.10f, Screen.height * 0.05f), "Prenom", text);
		prenom = GUI.TextField(Rect((Screen.width/2) - (Screen.width * 0.25f/2), Screen.height * 0.54f, Screen.width * 0.25f, Screen.height * 0.05f), prenom, 25, textArea);
		//Nom
		GUI.Label(Rect((Screen.width/2) - (Screen.width * 0.40f/2), Screen.height * 0.61f, Screen.width * 0.10f, Screen.height * 0.05f), "Nom", text);
		nom = GUI.TextField(Rect((Screen.width/2) - (Screen.width * 0.25f/2), Screen.height * 0.61f, Screen.width * 0.25f, Screen.height * 0.05f), nom, 25, textArea);
		
		//Si on click sur "RETOUR" on retourne a la connection d'un compte existant
		if(GUI.Button(Rect((Screen.width * 0.435) - (Screen.width * 0.12f/2), Screen.height * 0.68f, Screen.width * 0.12f, Screen.height * 0.05f), "Retour", buttons)){
			if(alias == "" || password == "" || nom == "" || prenom == ""){
				//Erreur
			}
			else if(false){
				//Alias déja utilisé
				//Erreur
			}
			
			nouveauCompte = false;
		}
		//Si on click sur "CRÉER" on tente de créer le compte selon les informations données (vérifications)
		if(GUI.Button(Rect((Screen.width * 0.564) - (Screen.width * 0.12f/2), Screen.height * 0.68f, Screen.width * 0.12f, Screen.height * 0.05f), "Créer", buttons)){
			//Code pour ajouter un enregistrement dans la base de donnée 
			//si l'alias n'est pas déja utilisé
		}
	}
}		

