var alias : String = "";
var password : String = "";
var nom : String = "";
var prenom : String = "";
var test1 : GUIStyle;
var test2 : GUIStyle;
var newAccount = false;
var hasBeenPressed = false;

function OnGUI() {
		// Make a text field that modifies stringToEdit.
			GUI.Label(Rect(Screen.width/2 - 131, 200, 50, 20), "Alias");
			alias = GUI.TextField (Rect (Screen.width/2 - 100, 200, 260, 20), alias, 25, test1);
			GUI.Label(Rect(Screen.width/2 - 182, 230, 100, 20), "Mot de passe");
			password = GUI.TextField (Rect (Screen.width/2 - 100, 230, 260, 20), password, 25, test1);
			
			if(!hasBeenPressed){
				if(GUI.Button(Rect(Screen.width/2 -100, 260, 120, 30), "Connecter", test2)){
					//Code pour vérifier si l'alias et le mot de passe 
					//correspondent a un enregistrement dans la base de donnée
				}
		
				if(GUI.Button(Rect(Screen.width/2 + 40, 260, 120, 30), "Nouvel utilisateur", test2)){
					newAccount = true;
					hasBeenPressed = true;
				}
			}
			
			if(newAccount){
				GUI.Label(Rect(Screen.width/2 - 130, 260, 100, 20), "Nom");
				prenom = GUI.TextField (Rect (Screen.width/2 - 100, 260, 260, 20), prenom, 25, test1);
				GUI.Label(Rect(Screen.width/2 - 148, 290, 100, 20), "Prenom");
				nom = GUI.TextField (Rect (Screen.width/2 - 100, 290, 260, 20), nom, 25, test1);
				
				if(GUI.Button(Rect(Screen.width/2 - 100, 320, 120, 30), "Retour", test2)){
					newAccount = false;
				}
				
				if(GUI.Button(Rect(Screen.width/2 + 40, 320, 120, 30), "Créer", test2)){
					//Code pour ajouter un enregistrement dans la base de donnée 
					//si l'alias n'est pas déja utilisé
				}
			}
}		

