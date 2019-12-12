using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using System.IO;

public class LogInResponse : MonoBehaviour {

	public Text text;
	public Button ok;

	public InputField correoRecupera;

	public bool recuperarSuccessful = false;
	public bool shouldOpenRecuperar = false;

	private LogIn.LogInResp lIR = new LogIn.LogInResp();
	private static string urlSesion = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/soporte/recuperarPass";
	private static string claveApp = "MIARQ";

	private LogIn.TokenClass token;

	public void okPressed() {
		if (transform.GetComponent<LogIn> ().logInSuccesful) {
			transform.GetComponent<ViewsManager> ().hidePopUp (0);
			//transform.GetComponent<ViewsManager> ().hideAll ();
			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (0);
		}
		else if (!recuperarSuccessful && shouldOpenRecuperar) {
			transform.GetComponent<ViewsManager> ().hidePopUp (0);
			transform.GetComponent<ViewsManager> ().showPopUp (3);
			shouldOpenRecuperar = false;
		}
		else if (transform.GetComponent<LogIn> ().showTerminos){
			transform.GetComponent<ViewsManager> ().hidePopUp (0);
			transform.GetComponent<ViewsManager> ().verTerminos ();
			transform.GetComponent<ViewsManager> ().setComesFromLogin(true);
		}
		else {
			transform.GetComponent<ViewsManager> ().hidePopUp (0);
			shouldOpenRecuperar = false;
		}
	}

	public void recuperarPressed(){
		if (correoRecupera.text.Length > 0 && Dobro.Text.RegularExpressions.TestEmail.IsEmail (correoRecupera.text)) {
			transform.GetComponent<ViewsManager> ().hidePopUp (3);

			StartCoroutine (getToken ());
		} else {
			recuperarSuccessful = false;
			transform.GetComponent<LogIn> ().logInSuccesful = false;
			transform.GetComponent<LogInResponse> ().changeText ("Ingrese un correo válido");
			transform.GetComponent<ViewsManager> ().showPopUp (0);
		}
	}
		
	public void changeText(string text) {
		this.text.text = text;
	}

	public void recuperarSuccess(){
		recuperarSuccessful = true;
		transform.GetComponent<LogIn> ().logInSuccesful = false;
		transform.GetComponent<LogInResponse> ().changeText ("Un correo electrónico fue enviado a la dirección proporcionada");
		transform.GetComponent<ViewsManager> ().showPopUp (0);
	}

	public void recuperarNotSuccess(string s){
		recuperarSuccessful = false;
		transform.GetComponent<LogIn>().logInSuccesful = false;
		transform.GetComponent<LogInResponse> ().changeText (s);
		transform.GetComponent<ViewsManager> ().showPopUp (0);
	}

	public void cancelar(){
		transform.GetComponent<ViewsManager> ().hidePopUp (3);
		shouldOpenRecuperar = false;
	}

	IEnumerator recuperar(){

		transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("pEmail", correoRecupera.text);
		headers.Add ("Authorization", "Bearer " + token.Token);

		print ("correoRecupera: " + correoRecupera.text);

		WWW www = new WWW (urlSesion, null, headers);
		yield return www;
		print ("Respuesta recuperar: " + www.text);
		lIR = JsonUtility.FromJson<LogIn.LogInResp> (www.text);

		if (lIR.RESULTADO == "OK") {
			recuperarSuccess ();
		} else if (lIR.ERROR == "EL USUARIO NO EXISTE VERIFIQUE USUARIO") {
			recuperarNotSuccess ("El correo no existe");
		} 

		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
	}

	IEnumerator getToken(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic SU5NRVJTWVM6SU5NRVJTWVNQUzFOVEVSMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token", null ,headers);
		yield return www;
		token = JsonUtility.FromJson<LogIn.TokenClass> (www.text);
		print ("Token recupera: " + token.Token);
		StartCoroutine (recuperar());
	}

}
