using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.EventSystems;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

public class LogIn : MonoBehaviour {

	public InputField correo;
	public InputField contraseña;

	public InputField lateCorreo;
	public InputField lateContraseña;

	public string sisOperativo;

	public class LogInResp {
		public string RESULTADO;
		public string ERROR;
		public string NOMBRECOMPLETO;
		public string EMAIL;
        public string CELULAR;
		public int TERMINOS_CONDICIONES;
		public int CONTACTO_ID;
		public int PAIS;
		public int ESTADO;
		public int MUNICIPIO;
		public int LOCALIDAD;
		public int ENCUESTA;
	}

	public class TokenClass{
		public string Token;
		public int Expire_in;
	}
	TokenClass token;

	private LogInResp lIR = new LogInResp();
	public bool logInSuccesful = false;

	public bool showTerminos = false;

	// Dev : SU5NRVJTWVM6SU5NRVJTWVNQUzFOVEVSMTIzIw==

	private static string claveApp = "MIARQ";
	private static string urlSesion = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/login/valida";
	private static string urlRegister = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/login/registro";
	private Registrar.registerResp rR;
	private bool updateSuccesful = false;

	// Use this for initialization
	void Start () {

		#if UNITY_IPHONE
		sisOperativo = "iOS";
		#elif UNITY_ANDROID
		sisOperativo = "Android";
		#endif

		if (PlayerPrefs.GetInt ("LoggedIn") == 1) {
			logInSuccesful = true;
		} else {
			logInSuccesful = false;
		}

		token = new TokenClass ();
        //StartCoroutine (getToken ());
        Debug.Log("MAC ADDRESS: " + GetMacAddress());
	}

	public void UpdateUserTerminosInfo(){
		print ("Comes from login");
		StartCoroutine (updateInfo());
	}

	IEnumerator updateInfo() {
		transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();
		string jsonData = 
            "{\n  " +
            "\"contacto_id\" : " + PlayerPrefs.GetInt("UserId") + 
            ",\n \"email\" : \"" + PlayerPrefs.GetString("UserEmail") + 
            "\",\n \"celular\" : \"" + PlayerPrefs.GetString("celular") + 
            "\",\n \"aceptaTermCond\" : 1,\n\"aceptaCondComer\" : \"1\",\n   " +
            " \"claveApp\" : \"" + claveApp + 
            "\",\n\"edicion\" : 1\n " +
            "\n}\n";

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);
        Debug.Log("Token " + token.Token);
        Debug.Log(jsonData);

		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		WWW www = new WWW (urlRegister, pData, headers);
		yield return www;

		try{
			rR = JsonUtility.FromJson<Registrar.registerResp> (www.text);
		}
		catch (System.ArgumentException e){
			logInNotSuccess ("Error de conexion");
			print (e);
		}
		updateSuccesful = rR.RESULTADO == "EDICION REALIZADA CORRECTAMENTE";

		print ("updateResponse: " + www.text);

		if (updateSuccesful) {
			logInSuccess ();
		}

		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
	}

	private void updateSuccess(){
		
	}

	public void PopCerrarSesion(){
		transform.GetComponent<ViewsManager> ().views [0].transform.localPosition = new Vector3 (0, 0, 0);
		PlayerPrefs.SetInt ("LoggedIn", 0);
		PlayerPrefs.SetString ("UserName", "");
		transform.GetComponent<ViewsManager> ().views [transform.GetComponent<ViewsManager> ().activeView].GetComponent<View> ().hideAll ("derecha");
		transform.GetComponent<ViewsManager> ().goToIndex (0, "derecha");
		correo.text = "";
		contraseña.text = "";
		logInSuccesful = false;
		FB.LogOut ();
		reiniciarCuestionario ();
		transform.GetComponent<ViewsManager> ().hidePopUp (5);

		transform.GetComponent<ViewsManager> ().Encuesta.GetComponent <Encuesta> ().ResetEncuesta ();
		transform.GetComponent<ViewsManager> ().Encuesta.GetComponent <Encuesta> ().GetEncuesta ();
		if (PlayerPrefs.GetInt("UserEncuesta") == 0){
			transform.GetComponent<ViewsManager> ().verEncuesta ();
		}
	}

	public void CerrarPopCerrarSesion(){
		transform.GetComponent<ViewsManager> ().hidePopUp (5);
	}

	public void cerrarSesion(){
		GameObject.Find ("CerrarSesionText").GetComponent<Text> ().text = "Cerrar sesión,\nperderás los cambios que has hecho.\n¿Estás seguro?";
		transform.GetComponent<ViewsManager> ().showPopUp (5);
	}

	public void reiniciarCuestionario(){

		bool lastState = transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [3].gameObject.activeSelf;
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [3].SetActive (true);
		transform.GetComponent<Tamano> ().toggleAll ();
		transform.GetComponent<Tamano> ().restartCont ();
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [3].SetActive (lastState);

		lastState = transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [2].gameObject.activeSelf;
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [2].SetActive (true);
		transform.GetComponent<Tipos> ().toggleAll ();
		transform.GetComponent<Tipos> ().restartCont ();
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [2].SetActive (lastState);

		lastState = transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [4].gameObject.activeSelf;
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [4].SetActive (true);
		transform.GetComponent<Lugar> ().toggleAll ();
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [4].SetActive (lastState);

		lastState = transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [6].gameObject.activeSelf;
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [6].SetActive (true);
        FindObjectOfType<ScriptFotosNativo>().EliminarTodasImagenes(); //GameObject.Find("Scripts").GetComponent<ScriptFotosNativo> ().EliminarTodasImagenes ();
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [6].SetActive (lastState);
	}

	public void recuperarContraseña(){
		transform.GetComponent<LogInResponse> ().shouldOpenRecuperar = true;
		transform.GetComponent<ViewsManager> ().showPopUp (3);
	}

	public void logInSuccess(){
		logInSuccesful = true;
		transform.GetComponent<ViewsManager> ().views [0].transform.localPosition = new Vector3 (0, 0, 0);
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (0);
		transform.GetComponent<ViewsManager> ().guestMode = false;
		PlayerPrefs.SetInt ("LoggedIn", 1);
		transform.GetComponent<ViewsManager> ().setNameLabel ();
		StartCoroutine (GetComponent<ViewsManager> ().getPrecio ());
	}

	public void lateLoginSuccess(){
		logInSuccesful = true;
		transform.GetComponent<ViewsManager> ().guestMode = false;
		PlayerPrefs.SetInt ("LoggedIn", 1);
		transform.GetComponent<ViewsManager> ().setNameLabel ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (11);

		transform.GetComponent<Factura> ().AceptarFactura ();
	}

	public void logInSuccesShowTerminos(){
		logInSuccesful = false;
		showTerminos = true;
		transform.GetComponent<LogInResponse> ().changeText ("Debe aceptar los términos y condiciones para poder usar esta aplicación");
		transform.GetComponent<ViewsManager> ().showPopUp (0);
	}

	public void falseLogInSuccess(){
		PlayerPrefs.SetString("UserName", "Invitado");
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (0);
		transform.GetComponent<ViewsManager> ().guestMode = true;
		transform.GetComponent<ViewsManager> ().setNameLabel ();
		StartCoroutine (GetComponent<ViewsManager> ().getPrecio ());
	}

	public void logInNotSuccess(string s) {
		logInSuccesful = false;
		transform.GetComponent<LogInResponse> ().changeText (s);
		transform.GetComponent<ViewsManager> ().showPopUp (0);
	}

	public void iniciarSesion(){
		if (loginInfo ()) {
			StartCoroutine(getToken());
		} else {
			logInSuccesful = false;
			logInNotSuccess ("Ingresa un correo o contraseña válida");
		}
	}

	bool loginInfo (){
		if (!GetComponent<ViewsManager> ().guestMode){
			return correo.text.Length != 0 && contraseña.text.Length != 0;
		}
		else {
			return lateCorreo.text.Length != 0 && lateContraseña.text.Length != 0;
		}
	}

	public void stopPost(){
		StopAllCoroutines ();
	}

	IEnumerator postSesion(){

		string sCorreo;
		string sContraseña;

		if (!GetComponent<ViewsManager> ().guestMode){
			sCorreo = correo.text;
			sContraseña = contraseña.text;
		}
		else {
			sCorreo = lateCorreo.text;
			sContraseña = lateContraseña.text;
		}

		string jsonData = 
            "{\n    \"email\" : \"" + sCorreo + 
            "\",\n    \"password\" : \"" + sContraseña +
            "\",\n    \"claveApp\" : \"" + claveApp + 
            "\",\n    \"macAddress\" : \""+ GetMacAddress() +
            "\",\n    \"dispositivo\" : \"Celular\",\n    " +
            "\"login_facebook\" : 0,\n    " +   
            "\"sistOperativo\" : \""+ sisOperativo +"\",\n" +
            "\"dispclave\" : \"" + SystemInfo.deviceUniqueIdentifier + "\"" +
          "}\n";
        Debug.Log(jsonData);
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);

		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		WWW www = new WWW (urlSesion, pData, headers);

		yield return www;
		print ("Loginresp: " + www.text);
		try {
			lIR = JsonUtility.FromJson<LogInResp> (www.text);
		}
		catch (System.ArgumentException e){
			logInNotSuccess ("Error de conexión");
			print (e);
		}
		logInSuccesful = lIR.RESULTADO == "OK";

		if (logInSuccesful) {
			PlayerPrefs.SetString ("UserName", lIR.NOMBRECOMPLETO);
			PlayerPrefs.SetString ("UserEmail", lIR.EMAIL);
            PlayerPrefs.SetString("celular", lIR.CELULAR);
			PlayerPrefs.SetInt ("UserPais", lIR.PAIS);
			PlayerPrefs.SetInt ("UserEstado", lIR.ESTADO);
			PlayerPrefs.SetInt ("UserMunicipio", lIR.MUNICIPIO);
			PlayerPrefs.SetInt ("UserLocalidad", lIR.LOCALIDAD);
			PlayerPrefs.SetInt ("UserId", lIR.CONTACTO_ID);
			PlayerPrefs.SetInt ("UserEncuesta", lIR.ENCUESTA);

			/*if (lIR.TERMINOS_CONDICIONES == 1 && !GetComponent<ViewsManager> ().guestMode) {
				logInSuccess ();
                Debug.Log("Login Succes");
			}*/
			if (GetComponent<ViewsManager> ().guestMode){
				lateLoginSuccess ();
			}
			else
            {
                logInSuccess();
                if (lIR.TERMINOS_CONDICIONES != 1)
				    logInSuccesShowTerminos ();
			}
		} 
		else if (lIR.ERROR == "EL USUARIO NO EXISTE VERIFIQUE USUARIO Y/O CONTRASEÑA") {
			logInNotSuccess ("El usuario o contraseña son incorrectos");
		}

		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
	}

	IEnumerator getToken(){
		transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();
		// PRODUCTION BEARER SU5NRVJTWVM6SU5NRVJTWVNQUzFOVEVSMTIzIw==
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token", null, headers);

		float maxTime = 10f;

		while (!www.isDone){
			maxTime -= Time.deltaTime;

			if (maxTime <= 0f){
				logInNotSuccess ("Tiempo de conexión agotado\nRevisa tu conexión a internet");

				transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
				transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

				yield break;
			}

			yield return null;
		}

		yield return www;

		if (www.error != null){
			logInNotSuccess ("Tiempo de conexión agotado\nRevisa tu conexión a internet");

			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

			yield break;
		}

		token = JsonUtility.FromJson<TokenClass> (www.text);
		print ("Token: " + token.Token);
		StartCoroutine (postSesion());
	}

	public void logInPressed(){
		transform.GetComponent<ViewsManager> ().views[0].GetComponent<View> ().goToIndexNoMove (0);
	}

	public void registerPressed(){
		transform.GetComponent<ViewsManager> ().goToIndexNoMove (1);
	}

    public string GetMacAddress()
    {
        string address = "";
        //#if UNITY_IOS || UNITY_EDITOR
        NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0; i < networks.Length; i++)
        {
            PhysicalAddress physicalAddress = networks[i].GetPhysicalAddress();
            if (!physicalAddress.ToString().Equals(string.Empty))
            {
                address = physicalAddress.ToString();
                break;
            }
        }

        if (address.Equals(string.Empty))
            address = "02:00:00:00:00:00";
        //#elif UNITY_ANDROID

        //#endif

        return address;
    }

	const string appAccessKeyID = "AKIAYJS2WBVK47Q7ZXVN";
	const string appSecretAccessKey = "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";

	const double requestValidForSeconds = 60.0f;

	string CreateSignatureSHA256(string policy)
	{
		byte[] policyBytes = Encoding.Default.GetBytes(policy);
		byte[] keyBytes = Encoding.Default.GetBytes(appSecretAccessKey);

		HMACSHA256 signHash = new HMACSHA256(keyBytes);
		signHash.ComputeHash(policyBytes);

		string finalSignature = Convert.ToBase64String(signHash.Hash);
		return finalSignature;
	}
}
