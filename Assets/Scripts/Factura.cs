using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System;

public class Factura : MonoBehaviour {

	private static string claveApp = "MIARQ";
	public static InvoiceData invoiceData;
	private string fillError = "";
	public string linkFactura = "";

	//Info país
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

	[Serializable]
	public class ColoniaClass{
		public int COLONIA;
		public string NOMBRE;
	}

	[Serializable]
	public class ColoniasClass{
		public ColoniaClass[] COLONIAS;
	}

	PaisesClass paises;
	EstadosClass estados;
	CiudadesClass ciudades;
	ColoniasClass colonias;

	public GameObject fotografias;

	public InputField RazonSocial;
	public InputField RFC;
	public InputField Calle;
	public InputField NoExt;
	public InputField NoInt;
	public Dropdown Pais;
	public Dropdown Estado;
	public Dropdown Ciudad;
	public InputField CP;
	public Dropdown Colonia;
	public InputField Delegacion;
	public InputField Telefono;

	public class TokenClass{
		public string Token;
		public string Expire_in;
	}
	TokenClass token, tokenProp;

	public class FacturaRespClass{
		public string RESULTADO;
		public string FOLIO;
		public string URLPDF;
		public string URLXML;
	}
	FacturaRespClass facturaResp;

	[HideInInspector]
	public bool facturaInfoFilled = false;

	void Awake(){
		StartCoroutine (getTokenProp());
		StartCoroutine (getTokenAndGetPais());
		invoiceData = new InvoiceData();
	}

	public void Back(){
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (7);
		fotografias.GetComponent<ScriptFotosNativo>().IniciarCamara();
		facturaInfoFilled = false;
	}

	private bool allFilled(){
		// if (	RazonSocial.text.Length > 1
		// 	&& 	Calle.text.Length > 3
		// 	&& 	NoExt.text.Length > 0
		// 	&&	Pais.value != 0
		// 	&&	Estado.value != 0
		// 	&&	Ciudad.value != 0
		// 	&&	CP.text.Length > 4
		// 	&& 	Colonia.value != 0
		// 	&& 	Delegacion.text.Length > 3
		// ) {
		// 	return true;
		// }
		// return false;

		if (RazonSocial.text.Length <= 1){
			fillError = "El nombre es muy corto";
			return false;
		}
		else {
			if (Calle.text.Length <= 3){
				fillError = "Ingresa una calle válida";
				return false;
			}
			else {
				if (NoExt.text.Length == 0){
					fillError = "Ingresa un número exterior válido";
					return false;
				}
				else {
					if (Pais.value == 0){
						fillError = "Ingresa un país";
						return false;
					}
					else {
						if (Estado.value == 0){
							fillError = "Ingresa un estado";
							return false;
						}
						else {
							if (Ciudad.value == 0){
								fillError = "Ingresa una ciudad";
								return false;
							}
							else {
								if (CP.text.Length <= 4){
									fillError = "Ingresa un código postal válido";
									return false;
								}
								else {
									if (Colonia.value == 0){
										fillError = "Ingresa una colonia";
										return false;
									}
									else {
										if (Delegacion.text.Length <= 3){
											fillError = "Ingresa una localidad válida";
											return false;
										}

										return true;
									}
								}
							}
						}
					}
				}
			}  
		}

		return true;
	}

	public void AceptarFactura(){
		if (allFilled () && !transform.GetComponent<ViewsManager> ().guestMode){
			#if UNITY_IPHONE
				invoiceData.calle = Calle.text;
				invoiceData.cp = CP.text;
				invoiceData.numext = NoExt.text;
				invoiceData.numint = NoInt.text;
				invoiceData.cliente = ""+PlayerPrefs.GetInt("UserId");
				invoiceData.rfc = RFC.text;
				invoiceData.telefono = Telefono.text;
				invoiceData.pais = paises.PAISES[Pais.value - 1].PAIS;
				invoiceData.estado = ""+estados.ESTADOS[Estado.value - 1].ESTADO;
				invoiceData.municipio = ""+ciudades.CIUDADES[Ciudad.value - 1].CIUDAD;
				invoiceData.localidad = Delegacion.text;
				invoiceData.colonia = ""+colonias.COLONIAS[Colonia.value - 1].COLONIA;
			#endif
			transform.GetComponent<ViewsManager> ().views[0].GetComponent<View>().goToIndexNoMove (8);

			facturaInfoFilled = true;

			GetComponent<ViewsManager> ().changeAvisoTxt (0);
			GetComponent<ViewsManager> ().showPopUp (9);
		}
		else if (transform.GetComponent<ViewsManager> ().guestMode){
			// Mostrar login tardío
			GetComponent<ViewsManager> ().showPopUp (11);
		}
		else {
			transform.GetComponent<Registrar> ().registerNotSuccess (fillError);
		}
	}

	private IEnumerator getTokenProp(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		print ("TokenProp: " + www.text);
		tokenProp = JsonUtility.FromJson<TokenClass> (www.text);
		//		print ("Token: " + token.Token);
	}

	public IEnumerator postInfoFactura(){
		transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + tokenProp.Token);

		WebPayView.Respuesta resp = transform.GetComponent<WebPayView> ().resp;
		WebPayView.PropRespClass propResp = transform.GetComponent<WebPayView> ().propResp;

		print ("jsonData vars: " + paises.PAISES[Pais.value-1].PAIS);

		string jsonData = "{" +
			"\n\"contacto_id\":" + PlayerPrefs.GetInt ("UserId") + "," +
			"\n\"cliente\":\"" + RazonSocial.text + "\"," +
			"\n\"calle\":\"" + Calle.text + "\"," +
			"\n\"numext\":\"" + NoExt.text + "\"," +
			"\n\"numint\":\"" + NoInt.text + "\"," +
			"\n\"pais\":" + paises.PAISES[Pais.value-1].PAIS + "," +
			"\n\"estado\":" + estados.ESTADOS [Estado.value - 1].ESTADO + "," +
			"\n\"municipio\":" + ciudades.CIUDADES [Ciudad.value - 1].CIUDAD + "," +
			"\n\"localidad\":\"" + Delegacion.text + "\"," +
			"\n\"colonia\":" + colonias.COLONIAS [Colonia.value - 1].COLONIA + "," +
			"\n\"cp\":\"" + CP.text + "\"," +
			"\n\"telefono\":\"" + Telefono.text + "\"," +
			"\n\"rfc\":\"" + RFC.text + "\"," +
			"\n\"procesadorPago\":\"" + /*resp.type*/ "" + "\"," +
			"\n\"tipotarjeta\":\"" + resp.type + "\"," +
			"\n\"autorizacion_banco\":\"" + resp.aut + "\"," +
			"\n\"compra_id\":\"" + resp.referencia + "\"," +
			"\n\"claveApp\":\"" + claveApp + "\"," +
			"\n\"prop_id\":" + propResp.PROP_ID +
			"}";

		print ("json factura: " + jsonData);

		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/pago", pData, headers);
		yield return www;

		print ("Factura resp: " + www.text);

		try{
			facturaResp = JsonUtility.FromJson<FacturaRespClass> (www.text);
		}
		catch (System.ArgumentException e){
			GetComponent<LogIn>().logInNotSuccess ("Error de conexion");
			print (e);
		}

		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

		if (facturaResp.RESULTADO == "OK") {
			yield return new WaitForSeconds (3f);
			GameObject.Find ("FacturaRespText").GetComponent<Text> ().text = "Creación de factura exitosa\nUn correo electrónico será mandado con tu factura\n¿Deseas verla ahora?";
			transform.GetComponent<ViewsManager> ().showPopUp (7);
			cleanLinkFactura (facturaResp.URLPDF);
		}
		else if (facturaResp.RESULTADO == "El folio se generara mas adelante, procesando rutinas contables") {
			transform.GetComponent<Registrar> ().registerNotSuccess ("El folio se generará más adelante, procesando rutinas contables");
		}
		else if (facturaResp.RESULTADO == "El folio de su compra le llegar\u00E1 en unos minutos") {
			transform.GetComponent<Registrar> ().registerNotSuccess ("El folio de tu compra te llegará en unos minutos");
		}
	}

	private string ReplaceAll(string input, char target) {
		StringBuilder sb = new StringBuilder(input.Length);
		for(int i = 0; i < input.Length; i++){
			if (input [i] == target) {

			}
			else {
				sb.Append (input [i]);
			}
		}

		return sb.ToString();
	}

	private void cleanLinkFactura(string f){
		linkFactura = ReplaceAll (f, '\\');
	}

	public void verFactura(){
		if (linkFactura.Length > 0) {
			Application.OpenURL (linkFactura);
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (7);
			GameObject.Find ("DeNuevoText").GetComponent<Text> ().text = "Gracias por utilizar Interceramic Mi Arquitecto.\nPronto nos pondremos en contacto contigo";
			transform.GetComponent<ViewsManager> ().showPopUp (6);
		}
	}

	public void cerrarFacturaResp(){
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (7);
		GameObject.Find ("DeNuevoText").GetComponent<Text> ().text = "Gracias por utilizar Interceramic Mi Arquitecto.\nPronto nos pondremos en contacto contigo";
		transform.GetComponent<ViewsManager> ().showPopUp (6);
	}

	private IEnumerator getTokenAndGetPais(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token", null ,headers);
		yield return www;
		print ("Token: " + www.text);
		token = JsonUtility.FromJson<TokenClass> (www.text);
		//		print ("Token: " + token.Token);
		StartCoroutine (bajaInfoPais());
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
		if (estados != null) {
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
	}

	IEnumerator bajaInfoColonia(){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("pPaisClave", paises.PAISES[Pais.value-1].PAIS);
		headers.Add ("pEntFedNum", estados.ESTADOS [Estado.value - 1].ESTADO.ToString());
		headers.Add ("pMpoNum", ciudades.CIUDADES [Ciudad.value - 1].CIUDAD.ToString());
		headers.Add ("pCp", CP.text);
		headers.Add ("Authorization", "Bearer " + token.Token);

		print ("pPaisClave: " + paises.PAISES[Pais.value-1].PAIS);
		print ("pEntFedNum: " + estados.ESTADOS [Estado.value - 1].ESTADO.ToString());
		print ("pMoNum: " + ciudades.CIUDADES [Ciudad.value - 1].CIUDAD.ToString());
		print ("pCp: " + CP.text);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/mastercontactossecws_sa/consulta/colonias", null, headers);
		yield return www;
		print ("respuesta: " + www.text);
		colonias = JsonUtility.FromJson<ColoniasClass> (www.text);

		List<Dropdown.OptionData> lista = new List<Dropdown.OptionData> ();
		lista.Add (new Dropdown.OptionData("Colonia", null));
		if (colonias.COLONIAS.Length == 0)
			CPError ("El código postal está incorrecto");
		else
			CPError ("");
		for (int i = 0; i < colonias.COLONIAS.Length; i++) {
			lista.Add (new Dropdown.OptionData (colonias.COLONIAS[i].NOMBRE, null));
		}
		Colonia.options = lista;
		Colonia.Hide ();
	}

	private void CPError (string s) {
		GameObject.Find ("ErrorCPFac").GetComponent<Text> ().text = s;
	}

	public void paisValueChange(){
		if (Pais.value > 0){
			Estado.gameObject.SetActive (true);
			StartCoroutine (bajaInfoEstado());
		}
		else {
			Estado.gameObject.SetActive (false);
		}
	}

	public void estadoValueChange(){
		if (Estado.value > 0){
			Ciudad.gameObject.SetActive (true);
			StartCoroutine (bajaInfoCiudad());
		}
		else {
			Ciudad.gameObject.SetActive (false);
		}
	}

	public void ciudadValueChange(){
		if (Ciudad.value > 0){
			CP.gameObject.SetActive (true);
		}
		else {
			CP.gameObject.SetActive (false);
		}
	}

	public void cpValueChange(){
		if (CP.text.Length > 4) {
			Colonia.gameObject.SetActive (true);
			CPError ("");
			StartCoroutine (bajaInfoColonia ());
		}
		else{
			Colonia.gameObject.SetActive (false);
			CPError ("El código postal debe tener 5 dígitos");
		}
	}

}
