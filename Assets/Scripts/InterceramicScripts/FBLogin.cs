using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Facebook.Unity;

public class FBLogin : MonoBehaviour {

	private static string urlSesion = "https://swsp.interceramic.com/ords/mastercontactosws_sa/login/valida";
	private static string claveApp = "MIARQ";

	LogIn.TokenClass token;

	void Awake(){
		FB.Init (SetInit, OnHideUnity);
	}

	void SetInit(){
		if (FB.IsLoggedIn) {
			print ("FB logged in");
//			transform.GetComponent<ViewsManager> ().hideAll ();
//			transform.GetComponent<ViewsManager> ().views [transform.GetComponent<ViewsManager> ().activeView].GetComponent<View> ().goToIndex (transform.GetComponent<ViewsManager> ().views [transform.GetComponent<ViewsManager> ().activeView].GetComponent<View> ().mainView);
		}
		else {
			print ("FB not logged in");
		}
	}

	void OnHideUnity(bool isAppShown){
		if (!isAppShown){
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}
	}

	public void FBLoginButton(){
		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");
		permissions.Add ("email");

		StartCoroutine (getToken ());
		FB.LogInWithReadPermissions (permissions, AuthCallBack);
	}

	void AuthCallBack(IResult result){
		if (result.Error != null) {
			print (result.Error);
		}
		else {
			if (FB.IsLoggedIn) {
				print ("FB logged in");
				FB.API ("/me?fields=first_name,last_name,email", HttpMethod.GET, APICallback);
				//transform.GetComponent<LogIn> ().logInSuccess ();
			}
			else {
				print ("FB not logged in");
			}
		}
	}

	void APICallback(IResult result) {
		StartCoroutine (postAPICallback(result));
	}

	IEnumerator postAPICallback(IResult result) {
		print (result.ResultDictionary["email"]);
		print (result.ResultDictionary["first_name"]);
		print (result.ResultDictionary["last_name"]);

		PlayerPrefs.SetString ("UserName", result.ResultDictionary["first_name"].ToString() + " " + (result.ResultDictionary["last_name"]).ToString());

		LogIn.LogInResp resp = new LogIn.LogInResp ();

		string jsonData = "{\n    \"email\" : \"" + result.ResultDictionary["email"] + "\",\n    \"password\" : \"\",\n    \"claveApp\" : \"" + claveApp + "\",\n    \"macAddress\" : \"123456\",\n    \"dispositivo\" : \"celular\",\n    \"login_facebook\" : 1,\n    \"sistOperativo\" : \"IOS\"\n}\n";
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);

		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		print ("JSON FB: " + jsonData);

		WWW www = new WWW (urlSesion, pData, headers);
		yield return www;
		print ("respuesta FB: " + www.text);
		resp = JsonUtility.FromJson<LogIn.LogInResp> (www.text);

		if (resp.RESULTADO == "OK") {
			transform.GetComponent<LogIn> ().logInSuccess ();
		}
		else {
			transform.GetComponent<ViewsManager> ().goToIndex (1, "izquierda");

			transform.GetComponent<Registrar> ().Nombre.text = result.ResultDictionary["first_name"] as string;
			transform.GetComponent<Registrar> ().Apellido.text = result.ResultDictionary["last_name"] as string;
			transform.GetComponent<Registrar> ().Correo.text = result.ResultDictionary["email"] as string;
			transform.GetComponent<Registrar> ().Contraseña.interactable = false;
			transform.GetComponent<Registrar> ().Contraseña.text = "";

			transform.GetComponent<Registrar> ().Facebook = 1;
		}
	}

	IEnumerator getToken(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic SU5NRVJTWVM6SU5NRVJTWVNQUzFOVEVSMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		token = JsonUtility.FromJson<LogIn.TokenClass> (www.text);
		print ("Token: " + token.Token);
//		StartCoroutine (postSesion());
	}
}
