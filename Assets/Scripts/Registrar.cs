using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class Registrar : MonoBehaviour {

	public bool isRegistering;
	public bool registerSuccesful = false;

	public InputField Nombre;
	public InputField Apellido;
	public InputField Correo;
	public InputField Contraseña;
	public Dropdown Pais;
	public Dropdown Estado;
	public Dropdown Ciudad;
	public InputField Telefono;
	public Dropdown TipoCliente;
	public Toggle Promociones;
	public Toggle Privacidad;
	public Toggle Terminos;
	//public Button Aceptar;

	public int Facebook = 0;
	private string fechaFinal;
	private Rect keyboardArea;

	public void nombreEnded(){
//		Apellido.Select ();
//		Apellido.ActivateInputField ();
	}

	public void apellidoEnded(){
//		Correo.Select ();
//		Correo.ActivateInputField ();
	}

	public void correoEnded(){
//		Contraseña.Select ();
//		Contraseña.ActivateInputField ();
	}

	public void contraseñaEnded(){
//		Telefono.Select ();
//		Telefono.ActivateInputField ();
	}

	public void telefonoEnded(){
//		Año.Select ();
//		Año.ActivateInputField ();
	}

	public void añoEnded(){
//		Mes.Select ();
//		Mes.ActivateInputField ();
	}

	public void mesEnded(){
//		Dia.Select ();
//		Dia.ActivateInputField ();
	}

	public void diaEnded(){

	}

	public class registerResp{
		public string RESULTADO;
		public string ERROR;
	}

	[Serializable]
	public class PaisClass{
		public string PAIS;
		public string NOMBRE;
	}

	[Serializable]
	public class PaisesClass{
		public PaisClass[] PAISES;
	}

	[Serializable]
	public class EstadoClass{
		public int ESTADO;
		public string NOMBRE;
	}

	[Serializable]
	public class EstadosClass{
		public EstadoClass[] ESTADOS;
	}

	[Serializable]
	public class CiudadClass{
		public int CIUDAD;
		public string NOMBRE;
	}

	[Serializable]
	public class CiudadesClass{
		public CiudadClass[] CIUDADES;
	}

	PaisesClass paises;
	EstadosClass estados;
	CiudadesClass ciudades;

	public class TokenClass{
		public string Token;
		public string Expire_in;
	}
	TokenClass token;

	registerResp rR = new registerResp();

	private string urlRegister = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/login/registro";
	private static string claveApp = "MIARQ";

	// Use this for initialization
	void Start () {
//		StartCoroutine (bajaInfoPais());
		StartCoroutine (getTokenAndGetPais());
		token = new TokenClass ();
	}

	IEnumerator bajaInfoPais(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/consulta/paises", null, headers);
		yield return www;
		print ("respuesta Pais: " + www.text);
		paises = JsonUtility.FromJson<PaisesClass> (www.text);

		List<Dropdown.OptionData> lista = new List<Dropdown.OptionData> ();
		lista.Add (new Dropdown.OptionData("País", null));
		for (int i = 0; i < paises.PAISES.Length; i++) {
			lista.Add (new Dropdown.OptionData (paises.PAISES[i].NOMBRE, null));
		}
		Pais.options = lista;
		Pais.Hide ();
	}

	IEnumerator bajaInfoEstado(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("pPaisClave", paises.PAISES[Pais.value-1].PAIS);
		headers.Add ("Authorization", "Bearer " + token.Token);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/consulta/estados", null, headers);
		yield return www;
		print ("respuesta: " + www.text);
		estados = JsonUtility.FromJson<EstadosClass> (www.text);

		List<Dropdown.OptionData> lista = new List<Dropdown.OptionData> ();
		lista.Add (new Dropdown.OptionData("Estado", null));
		for (int i = 0; i < estados.ESTADOS.Length; i++) {
			lista.Add (new Dropdown.OptionData (estados.ESTADOS[i].NOMBRE, null));
		}
		Estado.options = lista;
		Estado.Hide ();
	}

	IEnumerator bajaInfoCiudad(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("pPaisClave", paises.PAISES[Pais.value-1].PAIS);
		headers.Add ("pEntFedNum", estados.ESTADOS [Estado.value - 1].ESTADO.ToString());
		headers.Add ("Authorization", "Bearer " + token.Token);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/consulta/municipios", null, headers);
		yield return www;
		print ("respuesta: " + www.text);
		ciudades = JsonUtility.FromJson<CiudadesClass> (www.text);

		List<Dropdown.OptionData> lista = new List<Dropdown.OptionData> ();
		lista.Add (new Dropdown.OptionData("Ciudad", null));
		for (int i = 0; i < ciudades.CIUDADES.Length; i++) {
			lista.Add (new Dropdown.OptionData (ciudades.CIUDADES[i].NOMBRE, null));
		}
		Ciudad.options = lista;
		Ciudad.Hide ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown("-")) {
//			Nombre.text = "Andre";
//			Apellido.text = "Peres";
//			Correo.text = "andre.peres@inmersys.com";
//			Contraseña.text = "123456";
//			Contraseña.interactable = true;
//			Pais.value = 1;
//			Estado.value = 8;
//			Ciudad.value = 17;
//			Telefono.text = "555555555";
//			TipoCliente.value = 1;
//			Masculino.isOn = true;
//			Año.text = "1992";
//			Mes.text = "12";
//			Dia.text = "15";
//			Privacidad.isOn = true;
//			Terminos.isOn = true;
//		}

//		if (Telefono.isFocused) {
//			keyboardArea = TouchScreenKeyboard.area;
//			print (Telefono.transform.position + " " + keyboardArea.center.y);
////			if (Telefono.transform.position.y < keyboardArea.height) {
////				transform.GetComponent<ViewsManager> ().showPopUp (2);
////			}
//		}
	}

	public void paisValueChange(){
		if (Pais.value > 0)
			StartCoroutine (bajaInfoEstado());
	}

	public void estadoValueChange(){
		if (Estado.value > 0)
			StartCoroutine (bajaInfoCiudad());
	}

	public void register() {
		transform.GetComponent<ViewsManager> ().goToIndexNoMove (1);
		Nombre.text = "";
		Apellido.text = "";
		Correo.text = "";
		Contraseña.text = "";
		Contraseña.interactable = true;
		Pais.value = 0;
		Estado.value = 0;
		Ciudad.value = 0;
		Telefono.text = "";
		TipoCliente.value = 0;
		Privacidad.isOn = false;
		Terminos.isOn = false;
	}

	public void back() {
		transform.GetComponent<ViewsManager> ().goToIndex (0, "derecha");
	}

	public void registerSuccess() {
		if (Facebook == 0) {
			transform.GetComponent<RegisterResponse> ().changeText ("Registro aprobado, un correo electrónico se envió para verificar tu dirección de correo");
		}
		else {
			transform.GetComponent<RegisterResponse> ().changeText ("Registro aprobado, tu cuenta de Facebook quedó asociada con Interceramic");
		}
		transform.GetComponent<ViewsManager> ().showPopUp (1);
		PlayerPrefs.SetString ("UserName", Nombre.text + " " + Apellido.text);
	}

	public void registerNotSuccess(string m) {
		transform.GetComponent<RegisterResponse> ().changeText (m);
		transform.GetComponent<ViewsManager> ().showPopUp (1);
	}

	public void aceptar() {
		if (Nombre.text.Length > 0
		    && Apellido.text.Length > 0
		    && Correo.text.Length > 0
			&& (Contraseña.text.Length > 0 || Facebook == 1)
		    && Pais.value > 0
		    && Estado.value > 0
		    && Ciudad.value > 0
		    && Telefono.text.Length > 0
		    && Privacidad.isOn
		    && Terminos.isOn) {
			if (Dobro.Text.RegularExpressions.TestEmail.IsEmail (Correo.text)) {
				if (Contraseña.text.Length >= 6 || Facebook == 1) {
					StartCoroutine (getTokenAndPost ());
				}
				else {
					registerNotSuccess ("La contraseña debe tener más de 6 digitos");
				}
			}
			else {
				registerNotSuccess ("Ingresa una dirección de correo válida");
			}
		} else {
			//print ("Rellena todos los campos");
			registerNotSuccess ("Rellena todos los campos");
		}
	}

	public void stopPost(){
		StopAllCoroutines ();
	}

	IEnumerator registrar() {
		string jsonData = "{\n    \"email\" : \"" + Correo.text + "\",\n    \"celular\" : \"" + Telefono.text +"\",\n    \"nombreCompleto\" : \"" + Nombre.text + " " + Apellido.text + "\",\n    \"fechaNacimiento\" : \"" + fechaFinal + "\",\n    \"pais\" : \"" + paises.PAISES[Pais.value-1].PAIS + "\",\n    \"estado\" : " + estados.ESTADOS [Estado.value - 1].ESTADO + ",\n    \"municipio\" : " + ciudades.CIUDADES [Ciudad.value - 1].CIUDAD + ",\n    \"localidad\" : 1,\n    \"publicidadMail\" : " + (Promociones.isOn ? 1:0) + ",\n    \"publicidadSMS\" : 0,\n    \"contacto\" : \"M\",\n    \"password\" : \"" + Contraseña.text + "\",\n    \"aceptaTermCond\" : \"1\",\n    \"aceptaCondComer\" : \"1\",\n    \"claveApp\" : \"" + claveApp + "\",\n    \"edicion\" : 0,\n    \"registro_facebook\" : \"" + Facebook + "\"\n}\n";

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);

		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		print ("Register JSON: " + jsonData);

		WWW www = new WWW (urlRegister, pData, headers);
		yield return www;

		print ("Register resp: " + www.text);

		if (www.error != null){
			registerNotSuccess ("Tiempo de conexión agotado\nRevisa tu conexión a internet");
		}
		else {
			try{
				rR = JsonUtility.FromJson<registerResp> (www.text);
			}
			catch (System.ArgumentException e){
				registerNotSuccess ("Tiempo de conexión agotado\nRevisa tu conexión a internet");
				print (e);
			}
			registerSuccesful = rR.RESULTADO == "REGISTRO REALIZADO CORRECTAMENTE";

			print (rR.ERROR);

			if (registerSuccesful) {
				print ("register success");
				registerSuccess ();
				isRegistering = false;
			}
			else if (rR.ERROR == "EL EMAIL YA SE ENCUENTRA REGISTRADO") {
				registerNotSuccess ("El correo ya se encuentra registrado");
			}
		}

		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
	}

	IEnumerator getTokenAndPost(){
		transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token", null ,headers);
		yield return www;
		token = JsonUtility.FromJson<TokenClass> (www.text);
//		print ("Token: " + token.Token);
		StartCoroutine (registrar());
	}

	IEnumerator getTokenAndGetPais(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNEM1YxMjMj");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token", null ,headers);
		yield return www;
		print ("Token: " + www.text);
		token = JsonUtility.FromJson<TokenClass> (www.text);
//		print ("Token: " + token.Token);
		StartCoroutine (bajaInfoPais());
	}

}
