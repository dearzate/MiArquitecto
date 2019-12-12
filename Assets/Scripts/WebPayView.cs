using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AndreScripts;
using UnityEngine.Purchasing;

public class WebPayView : MonoBehaviour, IStoreListener
{

	public static string claveApp = "MIARQ";
	public RectTransform loader;
	private float rotateSpeed = 200f;
	public GameObject loaderView;
	public GameObject payAppleButton;

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

	public class TokenClass{
		public string Token;
		public string Expire_in;
	}
	TokenClass token, tokenProp;

	public class PropRespClass{
		public string RESULTADO;
		public int PROP_ID;
	}
	public PropRespClass propResp;
	public GameObject processingiOS;
	public UniWebView webView;
	public GameObject fotografias;

	[HideInInspector]
	public int attempts = 0;
	[HideInInspector]
	public int failAttempts = 0;

	public Respuesta resp;
	public PrecioResp precio;

	public class PrecioResp{
		public string RESULTADO;
		public int COSTO;
	}

	[XmlRoot("xml")]
	public class Respuesta{
		[XmlAttribute("referencia")]
		public string referencia {get; set;}

		[XmlAttribute("response")]
		public string response {get; set;}

		[XmlAttribute("aut")]
		public string aut {get; set;}

		[XmlAttribute("error")]
		public string error {get; set;}

		[XmlAttribute("ccName")]
		public string ccName {get; set;}

		[XmlAttribute("ccNum")]
		public string ccNum {get; set;}

		[XmlAttribute("amount")]
		public string amount {get; set;}

		[XmlAttribute("type")]
		public string type {get; set;}
	}

	private string XMLA = "<xml>";
	private string XMLE = "<xml>";
	private string encryptedXMLA = "";
	private string encryptedXMLE = "";

	private static string inProcess = "<transacciones></transacciones><transaccionesCautM></transaccionesCautM>";
	private static float deltaTrans = 30f;

	//Development
//	private string XMLM = "5647586E6A376D4C4A384452524F43744E4241523632374C7132753955443270305554677254515145657574623445783768725A3774336373306D6D76742B2F5A7A53314A4A626F364853680A4D77454F53325346586D703942632B313252304C396B7A507A554C334679715950442F79365A376B50527A3041684D5346476D53364E625A36534F6F6B652B6747697079426A5A69776968790A6D5A304D64334E73495271567644534F4B495A446C6346625879474F35313956666D496A374A792B51355842573138686A756451716D6F696753626F32413D3D";
//	private string seed = "1A03EE6B14C442413723727449ADE550";
//	private string seed128 = "E218DB31582E5DC8B3C98A624AC5C162";
//	private string url = "https://qa3.mitec.com.mx/pgs/WebPay";
//	private string companyId = "F500";
//	private string branch = "0005";
//	private string user = "F500SCUS0";
//	private string password = "E83U83DRR2";
//	private string XMLTransacciones = "https://qa3.mitec.com.mx/pgs/services/xmltransacciones";

//	Production
	private string XMLM = "5647586E6A376D4C4A384452524F43744E42415236355459627A58736D744138305554677254515145657574623445783768725A37755453787732583645335656556B6F44362F7848634D410A363562674B5079414C7643776F2F6C714C314A6F70326F36784E66644D2F6B70745A38326168736D696E6D3567576E657971414554306A6B4673787030716A727241666C38583978365635780A654B306B65395564574371326A3041505536364C78326838342F762F356756592F6B61562B564A796938646F664F50372F2B5967325532483064616F53513D3D";
	private string seed = "5CDF40731CE015906D07C6F81260478F";
	private string seed128 = "0A5677A4359EBAC485AE141A5E5ECD40";
	private string url = "https://vip.e-pago.com.mx/pgs/WebPay";
	private string companyId = "C187";
	private string branch = "035";
	private string user = "C187CAUS0";
	private string password = "C6V1YP07FF";
	private string XMLTransacciones = "https://vip.e-pago.com.mx/pgs/services/xmltransacciones?wsdl";

	private MITEncrypt.clsAESWebPay AES;
	private MITEncrypt.clsAES128 AES128;

	//XMLA static info
	#if UNITY_IPHONE
	private string urlResponse = "https://interceramic.com/application/mi-arquitecto/index.php";
	#elif UNITY_ANDROID
	private string urlResponse = "https://interceramic.com/application/mi-arquitecto/Androidindex.php";
	#endif
	private string referencia = "3";
	private string moneda = "MXN";
	private string date_hour = "";
	private string cantidad = "";

	private bool isActive = false;
	private bool shouldCheckTrans = false;
	private double attemptResetTime = 5;


    [Header("Enlace a otros scripts")]
    public SampleWebView sampleWebView;
    public NewWebPay nuevoWebPay;
    public ViewsManager viewsManager;

    public ConsultasAPI consultaStatusBanco;

	void Start(){
		resp = new Respuesta();
		token = new TokenClass ();

		AES = new MITEncrypt.clsAESWebPay ();



#if UNITY_ANDROID
		webView.insets = new UniWebViewEdgeInsets ((int)(Screen.height * 0.1), 0, 0, 0);
		webView.zoomEnable = false;
		webView.backButtonEnable = false;
#else
		GameObject.Find("UniWebViewObject").SetActive(false);
#endif
	}

	/*void OnEnable (){
		webView.OnReceivedMessage += OnReceivedMessage;
		webView.OnLoadComplete += OnLoadComplete;
	}

	void OnDisable (){
		webView.OnReceivedMessage -= OnReceivedMessage;
		webView.OnLoadComplete -= OnLoadComplete;
	}*/

	void Awake(){
		print ("UserId: " + PlayerPrefs.GetInt("UserId"));
	}

	void Update () {
		#if UNITY_ANDROID
				int KeyboardSize = GetKeyboardSize ();
				if (KeyboardSize != 0 && isActive){
					//TO DO
					webView.insets = new UniWebViewEdgeInsets ((int)(Screen.height * 0.1), 0, KeyboardSize, 0);
				}
				else if (isActive){
					webView.insets = new UniWebViewEdgeInsets ((int)(Screen.height * 0.1), 0, 0, 0);
				}
				showIfActive();
				if(loaderView.gameObject.active) {
					loadingAnimation();
				}
		#endif
		#if UNITY_IPHONE
			//payAppleButton.SetActive(true);
		#endif
	}

	public int GetKeyboardSize (){
		#if !UNITY_EDITOR
        using(AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using(AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect")){
                View.Call("getWindowVisibleDisplayFrame", Rct);
                return Screen.height - Rct.Call<int>("height");
            }
        }
        #endif
        return 0;
    }

	void OnLoadComplete (UniWebView webView, bool success, string error){
		#if UNITY_ANDROID
			print ("Webload Success: " + success);
			print ("Webload Error: " + error);
			webView.Show ();
			if (!success){
				attempts--;
				requestHTML ();
			}
		#endif
	}

	public void requestHTML ()
    {
		XMLA = "<xml>";
		XMLE = "<xml>";
		encryptedXMLA = "";
		encryptedXMLE = "";
        string idIntent = PlayerPrefs.GetInt("UserPropID").ToString() + "-" + attempts++;
		XMLA = addRowToXML (XMLA, "tpPago", "C");
		XMLA = addRowToXML (XMLA, "amount", cantidad);
		XMLA = addRowToXML (XMLA, "urlResponse", urlResponse);
        XMLA = addRowToXML (XMLA, "referencia", idIntent);
		XMLA = addRowToXML (XMLA, "moneda", moneda);
		DateTime dt = DateTime.Now;
		date_hour = DateTime.Now.ToString ("s");
		XMLA = addRowToXML (XMLA, "date_hour", date_hour);

		AES = new MITEncrypt.clsAESWebPay ();
		encryptedXMLA = AES.encrypInAES128 (seed, XMLA);

		Andre.Log ("XMLA: " + XMLA);
		Andre.Log ("Encrypted XMLA: " + encryptedXMLA);

//		shouldCheckTrans = true;
//		checkPaymentStatus ();

		if (!PlayerPrefs.HasKey ("FailTime"))
        {
#if UNITY_ANDROID
            nuevoWebPay.ConstruirXML(viewsManager.precioArqui, idIntent );
#endif
			/*#if UNITY_IPHONE
			processingiOS.SetActive (true);
            //Application.OpenURL (url + "?id_company="+companyId+"&xmlm="+XMLM+"&xmla="+encryptedXMLA+"&xmle="+encryptedXMLE);

            //sampleWebView.AbrirEnlace(url + "?id_company=" + companyId + "&xmlm=" + XMLM + "&xmla=" + encryptedXMLA + "&xmle=" + encryptedXMLE);



#else
			StartCoroutine(getHTML ());
			#endif*/
		}
		else {
			long lastTime = Convert.ToInt64 (PlayerPrefs.GetString ("FailTime"));

			DateTime lastTimePlusThreshold = DateTime.FromBinary (lastTime);
			lastTimePlusThreshold = lastTimePlusThreshold.AddMinutes (attemptResetTime);

			print ("LastTimePlusThreshold: " + lastTimePlusThreshold.ToString ());
			print ("Time Now: " + DateTime.Now.ToString ());

			if (lastTimePlusThreshold <= DateTime.Now) {
				print(">>> Time has passed, you can try again now!!!");
				PlayerPrefs.DeleteKey("FailTime");
			#if UNITY_IPHONE 
				processingiOS.SetActive(true);
				//Application.OpenURL (url + "?id_company="+companyId+"&xmlm="+XMLM+"&xmla="+encryptedXMLA+"&xmle="+encryptedXMLE);
			#else
				StartCoroutine (getHTML ());
			#endif
		}
			else {
				back ();
				GetComponent<RegisterResponse> ().changeText ("Número de intentos para pagar superado, por favor intente más tarde");
				GetComponent<ViewsManager> ().showPopUp (1);
			}
		}

		isActive = true;
	}

	public void checkPaymentStatus (){
		string request= "";
		string requestEncrypted = "";
		string requestBase64 = "";
		string completeRequest = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wst=\"http://wstrans.cpagos\"><soapenv:Header/> <soapenv:Body><wst:transacciones>";

		request = addRowToXML (request, "user", user);
		request = addRowToXML (request, "pwd", password);
		request = addRowToXML (request, "id_company", companyId);
		request = addRowToXML (request, "date", DateTime.Now.ToString ("dd/MM/yyyy"));
		request = addRowToXML (request, "id_branch", branch);
		request = addRowToXML (request, "reference", PlayerPrefs.GetInt ("UserPropID").ToString () + "-" + (attempts-1));

		Andre.Log ("Request: " + request);

		AES128 = new MITEncrypt.clsAES128 ();
		requestEncrypted = AES128.encrypInAES128 (seed128, request);

		completeRequest = addRowToXML (completeRequest, "wst:in0", "9265654618");
		completeRequest = addRowToXML (completeRequest, "wst:in1", requestEncrypted);
		completeRequest = addRowToXML (completeRequest, "wst:in2", "");
		completeRequest = addRowToXML (completeRequest, "wst:in3", "");
		completeRequest = addRowToXML (completeRequest, "wst:in4", "");
		completeRequest = addRowToXML (completeRequest, "wst:in5", "");
		completeRequest += "</wst:transacciones></soapenv:Body></soapenv:Envelope>";

		Andre.Log ("CompleteRequest: " + completeRequest);

		StartCoroutine (getTransaccion (completeRequest));
	}

	public string decryptPaymentStatus (string raw){
		AES128 = new MITEncrypt.clsAES128 ();
		string decrypted = AES128.descrypInAES128 (seed128, raw);
		Andre.Log ("Decrypted: " + decrypted);
		return decrypted;
	}

	private string addRowToXML (string XML, string key, string content){
		return XML += "<" + key + ">" + content + "</" + key + ">";
	}

	private IEnumerator getTransaccion (string s){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "text/html");	
		byte[] b = System.Text.Encoding.UTF8.GetBytes(s);

		WWW www = new WWW (XMLTransacciones, b, headers);
		yield return www;

		Andre.Log ("Transacción: " + www.text);

		try {
			XmlDocument doc = new XmlDocument ();
			doc.InnerXml = www.text;
			XmlNamespaceManager nsmgr = new XmlNamespaceManager (doc.NameTable);
			nsmgr.AddNamespace ("soap", "http://schemas.xmlsoap.org/soap/envelope/");
			nsmgr.AddNamespace ("xsd", "http://www.w3.org/2001/XMLSchema");
			nsmgr.AddNamespace ("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			nsmgr.AddNamespace ("ns1", "http://wstrans.cpagos");

			XmlNodeList Response = doc.DocumentElement.SelectNodes("//ns1:transaccionesResponse", nsmgr);
			string Out = "";
			string decryptedOut = "";
				
			foreach	(XmlNode node in Response){
				Out = node["ns1:out"].InnerText;
			}

			Andre.Log ("out: " + Out);
			decryptedOut = decryptPaymentStatus (Out);
			Andre.Log ("descrypted out: " + decryptedOut);

			if (shouldCheckTrans && decryptedOut == inProcess){
				Andre.Log ("Waiting another: " + deltaTrans);
				StartCoroutine (TransLoop (s));
			}
			else if (shouldCheckTrans){
				if (decryptedOut.Contains ("approved")){
					Andre.Log ("Approved!");
					StartCoroutine (postImg ());
				}
				else {
					Andre.Log ("Declined");
					string failText = 	"Error de conexión";
					failText += 		"\n\nInténtalo de nuevo";
					PayNotSucces (failText);
				}
			}
		}
		catch (XmlException e){
			Andre.Log ("Declined: " + e);
			string failText = 	"Error de conexión";
			failText += 		"\n\nInténtalo de nuevo";
			PayNotSucces (failText);
		}
	}

	IEnumerator TransLoop (string s){
		yield return new WaitForSeconds (deltaTrans);
		StartCoroutine (getTransaccion (s));	
	}

	private IEnumerator getHTML (){

		webView.Show ();

		WWW www = new WWW (url + "?id_company="+companyId+"&xmlm="+XMLM+"&xmla="+encryptedXMLA+"&xmle="+encryptedXMLE);
//		print ("url: " + url + "?id_company="+companyId+"&xmlm="+XMLM+"&xmla="+encryptedXMLA+"&xmle="+encryptedXMLE);
		yield return www;

		//		print ("WebResponse: " + www.text);
		webView.LoadHTMLString (www.text, url);

		webView.AddJavaScript ("jsp/cpagos/styles2/jquery-2.2.3.min.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/bootstrap-3.3.6/js/bootstrap.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/jquery.payment.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/webpaym.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/formvalidation/js/formValidation.min.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/formvalidation/js/framework/bootstrap.min.js");
		webView.AddJavaScript ("jsp/cpagos/styles2/formvalidation/js/language/es_ES.js");
		webView.AddJavaScript ("https://sdos.mitec.com.mx/sdsWebServices/js/user-prefs.js");
		webView.AddJavaScript ("\n\t\t\tvar var_idioma=\"SP\";\n\t      \tvar var_sDosHabilitado = false;\n\t      \tvar var_sDosInstantHabilitado = false;\n\t      \tvar var_isMCI = false;\n\t      \tvar var_opcionDCC = \"\";\n\t\t\tdocument.oncontextmenu = function(){return false;}\n\t\t\tdocument.onkeydown = function(e){\n\t\t\t\tif(e)\n\t\t\t\t\tdocument.onkeypress = function(){return true;}\n\t\t\t\tvar evt = e?e:event;\n\t\t\t\tif(evt.keyCode==116)\n\t\t\t\t{\n\t\t\t\t\tif(e)\n\t\t\t\t\t\tdocument.onkeypress = function(){return false;}\n\t\t\t\t\telse\n\t\t\t\t\t{\n\t\t\t\t\t\tevt.keyCode = 0;\n\t\t\t\t\t\tevt.returnValue = false;\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t};\n\n\t\t\tfunction getMCI(){\n\t\t\t\tif(var_isMCI){\n\t\t\t\t\tvar i;\n\t\t\t\t   \tfor (i=0;i<document.C.op_mci.length;i++){ \n\t\t\t\t      \t if (document.C.op_mci[i].checked) \n\t\t\t\t         \t break; \t\t\t         \t \n\t\t\t\t   \t} \n\t\t\t\t   \tdocument.C.plazo_mci.value = document.C.op_mci[i].value;\n\t\t\t\t}\n\t\t\t};\n\t\n\t\t\tfunction enviaD(){\n\t\t\t\tif(var_opcionDCC==\"true\" || var_isMCI ){\n\t\t\t\t\tdocument.C.lang.value=var_idioma;\n\t\t\t\t\tfortyone.collect('userPrefs');\n\t\t\t\t\tFormEncode(document.C);\n\t\t\t\t}\t\t\t\n\t\t\t};\n\t\t");
		webView.AddJavaScript ("\n//jQuery para móviles\n$(function () { //NO TOCAR es el wrapper de jQuery\n\t//Resticciones de entrada de datos\n\t$('#cdCP').payment('restrictNumeric');\n\t$('#telefono').payment('restrictNumeric');\n\t$('#lada').payment('restrictNumeric');\n\t$('#cardexp').payment('formatCardExpiry');\n\t$('#csc1').payment('formatCardCVC');\n\n\t$('#cardexp').on('input', function(e){\n\t\tif($('#cardexp').val().length==7){\n\t\t\tvar expiry = $('#cardexp').val();\n\t        var parts = expiry.match(/^\\D*(\\d{1,2})(\\D+)?(\\d{1,4})?/);\n\t        month = parts[1] || ''\n\t        year = parts[3] || ''\n\t        $('#M1').val(month);\n\t        $('#A1').val(year);\n\t\t}\n\t})\n\n/*\n* NUEVA REGLA DE VALIDACION: NOMBRE para navegdores móviles con teclado Swift\n*\n*/\n\nFormValidation.Validator.wpName = {\n        validate: function(validator, $field, options) {\n            var value = $field.val();\n            if (value === '') {\n                return true;\n            }\n\t\t\t\n            // El nombre no puede llevar números\n            if (value.search(/[0-9]/) > 0) {\n                return {\n                    valid: false,\n                    message: 'Por favor no use números'\n                }\n            }\n            \n            // El nombre no puede llevar caracteres raros\n            if  (/^[a-zñ ]+$/i.test(value)===false) {\n                return {\n                    valid: false,\n                    message: 'Por favor use sólo letras y espacios'\n                }\n            }\n\n            return true;\n        }\n    };\n\nFormValidation.Validator.wpAlphanum = {\n        validate: function(validator, $field, options) {\n            var value = $field.val();\n            if (value === '') {\n                return true;\n            }\n\t\t\t\n            // El campo no puede llevar caracteres raros\n            if  (/^[a-z0-9ñ ]+$/i.test(value)===false) {\n                return {\n                    valid: false,\n                    message: 'Por favor use sólo letras, números y espacios'\n                }\n            }\n\n            return true;\n        }\n    };\n\n/*\n* NUEVA validación de la forma usando formvalidation\n*/\n\n\t$('#webpay').formValidation({\n\t\tframework: 'bootstrap',\n\t\tverbose: false,\n\t\tlocale: 'es_ES',\n\t\tfields: {\n\t\t\tcc_name1: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El nombre es requerido'\n\t\t\t\t\t},\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tpaterno1: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: 'El apellido paterno es requerido',\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tmaterno1: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tlada: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: 'La lada es requerida',\n\t\t\t\t\tdigits: {},\n\t\t\t\t\tstringLength: {\n\t\t\t\t\t\tmax: 5,\n\t\t\t\t\t\tmessage: 'La Lada debe tener 5 dìgitos o menos'\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t},\n\t\t\ttelefono: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: 'El teléfono es requerido',\n\t\t\t\t\tdigits: {},\n\t\t\t\t\tstringLength: {\n\t\t\t\t\t\tmax: 20,\n\t\t\t\t\t\tmessage: 'El teléfono debe tener 20 dígitos o menos'\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t},\n\t\t\temail: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El correo electrónico es requerido '\n\t\t\t\t\t},\n\t\t\t\t\temailAddress: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\temail2: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'Confirmar el correo electrónico es requerido '\n\t\t\t\t\t},\n\t\t\t\t\temailAddress: {},\n\t\t\t\t\tidentical: {\n\t\t\t\t\t\tfield: 'email',\n\t\t\t\t\t\tmessage: 'Los correos no coinciden, favor de verificarlos.'\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t},\n\t\t\tcc_num1: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El número de tarjeta es requerido.'\n\t\t\t\t\t},\n\t\t\t\t\tcreditCard: {\n                        message: 'El número de tarjeta es inválido.'\n                    }\n\t\t\t\t}\n\t\t\t},\n\t\t\tcardexp: {\n\t\t\t\tverbose: false,\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'La fecha de expiracíon es requerida.'\n\t\t\t\t\t},\n\t\t\t\t\tregexp: {\n\t\t\t\t\t\tmessage: 'La fecha de expiracíon de ser MM/AA',\n\t\t\t\t\t\tregexp: /^\\d{1,2}\\s?\\/\\s?\\d{2,4}$/\n\t\t\t\t\t},\n\t\t\t\t\tcallback: {\n\t\t\t\t\t\tmessage: 'Favor de validar la fecha de expiración',\n\t\t\t\t\t\tcallback: function(value, validator, $field) {\n\t\t\t\t\t\treturn $.payment.validateCardExpiry($('#cardexp').payment('cardExpiryVal'));\n\t\t\t\t\t\t}\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t},\n\t\t\tcsc1: {\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El código de seguridad es requerido.'\n\t\t\t\t\t},\n\t\t\t\t\tcvv: {\n\t\t\t\t\t\tmessage: 'El CVV no es válido.',\n\t\t\t\t\t\tcreditCardField: 'cc_num1'\n\t\t\t\t\t}\n\t\t\t\t}\n\t\t\t},\n\t\t\tnombre: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El nombre es requerido'\n\t\t\t\t\t},\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tpaterno: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El apellido paterno es requerido'\n\t\t\t\t\t},\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tmaterno: {\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators: {\n\t\t\t\t\twpName: {}\n\t\t\t\t}\n\t\t\t},\n\t\t\tcalle:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'La calle es requerida'\n\t\t\t\t\t},\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\text:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\tnotEmpty: {\n\t\t\t\t\t\tmessage: 'El número exterior es requerido'\n\t\t\t\t\t},\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tint2:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tnbColonia:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tnbDelegacion:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tnbCiudad:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tnbEstado:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\twpAlphanum:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\tcdCP:{\n\t\t\t\tthreshold: 3,\n\t\t\t\tvalidators:{\n\t\t\t\t\tnotEmpty: 'El código postal es requerido.',\n\t\t\t\t\tnumeric:{}\n\t\t\t\t}\n\t\t\t},\n\t\t\top_mci:{\n\t\t\t\tvalidators:{\n\t\t\t\t\tnotEmpty: 'Seleccione forma de pago.',\t\t\t\t\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}).on('success.form.fv', function(e) {\n            // Prevent form submission\n            e.preventDefault();\n\n            var $form = $(e.target),\n                fv    = $(e.target).data('formValidation');\n            \n            document.C.lang.value=var_idioma;\n            fortyone.collect('userPrefs');\n            FormEncode(document.C)\n            //fv.defaultSubmit();\n        });\n\n}); //NO TOCAR es el wrapper de jQuery\n");

		webView.Show ();

	}

	void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
    {
		webView.Hide ();

		shouldCheckTrans = false;

		print ("In message");
		if (message.path == "resp"){
			transform.GetComponent<ViewsManager> ().showPopUpNoAnim (2);
			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();

			print ("UniwebView message: " + message.args["respuesta"]);
			print ("UniwebView decrypted: " + AES.descrypInAES128 (seed, message.args["respuesta"]));

			string respXML = AES.descrypInAES128 (seed, message.args["respuesta"]);
			respXML += "</xml>";

			XmlDocument doc = new XmlDocument ();
			doc.InnerXml = respXML;
			XmlNodeList nodes = doc.DocumentElement.SelectNodes("/xml");

			foreach (XmlNode node in nodes) {
				resp.referencia = node.SelectSingleNode ("referencia").InnerText;
				resp.response = node.SelectSingleNode ("response").InnerText;
				resp.aut = node.SelectSingleNode ("aut").InnerText;
				resp.error = node.SelectSingleNode ("error").InnerText;
				resp.ccName = node.SelectSingleNode ("ccName").InnerText;
				resp.ccNum = node.SelectSingleNode ("ccNum").InnerText;
				resp.amount = node.SelectSingleNode ("amount").InnerText;
				resp.type = node.SelectSingleNode ("type").InnerText;
			}

			if (resp.response == "approved") {
				failAttempts = 0;
				PlayerPrefs.DeleteKey ("FailTime");
				StartCoroutine (postImg ());
			}
			else if (resp.response == "error") {
				failAttempts ++;
				transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
				transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

				string failText = 	"Pago declinado";
				failText +=			"\n\nMotivo: " + resp.response;
				failText += 		"\nReferencia: " + resp.referencia;
				failText +=			"\nTarjeta habiente: " + resp.ccName;
				failText += 		"\n\nInténtalo de nuevo";
				PayNotSucces (failText);
				webView.Hide ();
			}
			else if (resp.response == "unauthenticated"){
				failAttempts ++;
				transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
				transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

				string failText = 	"Pago no autorizado";
				failText +=			"\n\nMotivo: " + resp.response;
				failText += 		"\nReferencia: " + resp.referencia;
				failText +=			"\nTarjeta habiente: " + resp.ccName;
				failText += 		"\n\nInténtalo de nuevo";
				PayNotSucces (failText);
				webView.Hide ();
			}
			else if (resp.response == "denied"){
				failAttempts ++;
				transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
				transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

				string failText = 	"Pago declinado";
				failText +=			"\n\nMotivo: " + resp.response;
				failText += 		"\nReferencia: " + resp.referencia;
				failText +=			"\nTarjeta habiente: " + resp.ccName;
				failText += 		"\n\nInténtalo de nuevo";
				PayNotSucces (failText);
			}

			if (failAttempts >= 3){
				PlayerPrefs.SetString ("FailTime", System.DateTime.Now.ToBinary ().ToString ());
			}
			
			isActive = false;
			webView.Hide ();
		}

	}

	public void ReceivedMessage (string message)
    {

		processingiOS.SetActive (false);
		print ("Decrypted: " + AES.descrypInAES128 (seed, message));

		string respXML = AES.descrypInAES128 (seed, message);
		respXML += "</xml>";

		XmlDocument doc = new XmlDocument ();
		doc.InnerXml = respXML;
		XmlNodeList nodes = doc.DocumentElement.SelectNodes("/xml");

		foreach (XmlNode node in nodes) 
        {
			resp.referencia = node.SelectSingleNode ("referencia").InnerText;
			resp.response = node.SelectSingleNode ("response").InnerText;
			resp.aut = node.SelectSingleNode ("aut").InnerText;
			resp.error = node.SelectSingleNode ("error").InnerText;
			resp.ccName = node.SelectSingleNode ("ccName").InnerText;
			resp.ccNum = node.SelectSingleNode ("ccNum").InnerText;
			resp.amount = node.SelectSingleNode ("amount").InnerText;
			resp.type = node.SelectSingleNode ("type").InnerText;
		}

		if (resp.response == "approved") 
        {
			failAttempts = 0;
			PlayerPrefs.DeleteKey ("FailTime");
			StartCoroutine (postImg ());
		}
		else if (resp.response == "error")
        {
			failAttempts ++;
			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

			string failText = 	"Pago declinado";
			failText +=			"\n\nMotivo: " + resp.response;
			failText += 		"\nReferencia: " + resp.referencia;
			failText +=			"\nTarjeta habiente: " + resp.ccName;
			failText += 		"\n\nInténtalo de nuevo";
			PayNotSucces (failText);
		}
		else if (resp.response == "unauthenticated")
        {
			failAttempts ++;
			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

			string failText = 	"Pago no autorizado";
			failText +=			"\n\nMotivo: " + resp.response;
			failText += 		"\nReferencia: " + resp.referencia;
			failText +=			"\nTarjeta habiente: " + resp.ccName;
			failText += 		"\n\nInténtalo de nuevo";
			PayNotSucces (failText);
		}
		else if (resp.response == "denied"){
			failAttempts ++;
			transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
			transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);

			string failText = 	"Pago declinado";
			failText +=			"\n\nMotivo: " + resp.response;
			failText += 		"\nReferencia: " + resp.referencia;
			failText +=			"\nTarjeta habiente: " + resp.ccName;
			failText += 		"\n\nInténtalo de nuevo";
			PayNotSucces (failText);
		}

		if (failAttempts >= 3){
			PlayerPrefs.SetString ("FailTime", System.DateTime.Now.ToBinary ().ToString ());
		}
	}


    /// <summary>
    /// Pago aprobado
    /// </summary>
    public void ApprovedPayment()
    {
		#if UNITY_ANDROID
	        WebViewObject webViewObject = GameObject.FindObjectOfType<WebViewObject>();
	        if (webViewObject != null)
	            Destroy(webViewObject.gameObject);

	        failAttempts = 0;
	        PlayerPrefs.DeleteKey("FailTime");
		#endif
        StartCoroutine(postImg());
    }

    /// <summary>
    /// Pago rechazado
    /// </summary>
    public void DeniedPayment()
    {
        WebViewObject webViewObject = GameObject.FindObjectOfType<WebViewObject>();
        if (webViewObject != null)
            Destroy(webViewObject.gameObject);

        failAttempts++;
        transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().stopSpinning();
        transform.GetComponent<ViewsManager>().hidePopUpNoAnim(2);

        string failText = "Pago declinado";
        failText += "\n\nMotivo: " + resp.response;
        failText += "\nReferencia: " + resp.referencia;
        failText += "\nTarjeta habiente: " + resp.ccName;
        failText += "\n\nInténtalo de nuevo";
        PayNotSucces(failText);

        if (failAttempts >= 3)
        {
            PlayerPrefs.SetString("FailTime", System.DateTime.Now.ToBinary().ToString());
        }
    }

    /// <summary>
    /// Error en la solicitud de pago
    /// </summary>
    public void ErrorPayment()
    {
        WebViewObject webViewObject = GameObject.FindObjectOfType<WebViewObject>();
        if (webViewObject != null)
            Destroy(webViewObject.gameObject);

        failAttempts++;
        transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().stopSpinning();
        transform.GetComponent<ViewsManager>().hidePopUpNoAnim(2);

        string failText = "Pago declinado";
        failText += "\n\nMotivo: " + resp.response;
        failText += "\nReferencia: " + resp.referencia;
        failText += "\nTarjeta habiente: " + resp.ccName;
        failText += "\n\nInténtalo de nuevo";
        PayNotSucces(failText);

        if (failAttempts >= 3)
        {
            PlayerPrefs.SetString("FailTime", System.DateTime.Now.ToBinary().ToString());
        }
    }





	private void PayNotSucces (string s){
		GameObject.Find ("PayRespFailedText").GetComponent<Text> ().text = s; 
		GetComponent<ViewsManager> ().showPopUp (8);
	}

	public void loadWebPayUrl(){
		webView.url = "https://inmersys.com";
		webView.Load ();
	}

	public void back(){
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (8);
		webView.Hide ();

		//fotografias.GetComponent<ScriptFotosNativo>().IniciarCamara();

        WebViewObject webViewObject = GameObject.FindObjectOfType<WebViewObject>();
        if (webViewObject != null)
            Destroy(webViewObject.gameObject);

		StartCoroutine (getTokenCancelProp ());
	}

	public void closePop(){
		GameObject.Find ("DeNuevoText").GetComponent<Text> ().text = "¿Deseas solicitar otro diseño?";
		transform.GetComponent<ViewsManager> ().showPopUp (6);
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (4);
	}

	public void goToFactura(){
		webView.Hide ();
		// transform.GetComponent<Factura> ().setFacturaValues ();
		transform.GetComponent<ViewsManager> ().hidePopUp (4);
		StartCoroutine (transform.GetComponent<Factura> ().postInfoFactura ());
	}

	public void aceptarDiseno(){
		webView.Hide ();
		transform.GetComponent<ViewsManager> ().hidePopUp (6);
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().Awake ();
//		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (0);
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [0].transform.localPosition = new Vector3 (0, 0, 0);
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [0].SetActive (true);
		transform.GetComponent<LogIn> ().reiniciarCuestionario ();
	}

	public void cancelarDiseno(){
		webView.Hide ();
		transform.GetComponent<ViewsManager> ().hidePopUp (6);
		transform.GetComponent<LogIn> ().PopCerrarSesion ();
		transform.GetComponent<LogIn> ().reiniciarCuestionario ();
	}

	public void showIfActive (){
		if (GetComponent<ViewsManager> ().popIsShown) {
			webView.Hide ();
		}
	}

	private IEnumerator getTokenProp()
    {
		transform.GetComponent<ViewsManager> ().showPopUp (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		print ("TokenProp: " + www.text);
		tokenProp = JsonUtility.FromJson<TokenClass> (www.text);
        consultaStatusBanco.ReemplazarCabecera("Authorization", "Bearer " + tokenProp.Token);
		StartCoroutine (postProp ());
	}

	private IEnumerator postProp()
    {
#if UNITY_IPHONE
		transform.GetComponent<ViewsManager>().showPopUp(2);
		transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().startSpinning();
#endif
		GetComponent<Factura> ().facturaInfoFilled = false;

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + tokenProp.Token);

		string p1 = "", p2 = "", p3 = "", p4 = "", p5 = "";
		CuestionarioData.Data data = transform.GetComponent<CuestionarioData> ().respData;
		p1 = data.respEspacios.ToString ();
		p2 = data.respEstilos.ToString ();

		print ("p1: " + p1);
		print ("p2: " + p2);

		for (int i=0; i<10; i++) {
			if (data.respTipo[i]) {
				CuestionarioData.Tipo tipo = (CuestionarioData.Tipo)i+1;
				p3 += tipo.ToString () + ", ";
			}
		}
		p3 = p3.Substring (0, p3.Length-2);
		print ("p3: " + p3);

		for (int i=0; i<5; i++) {
			if (data.respTamano[i]) {
				CuestionarioData.Tamano tamano = (CuestionarioData.Tamano)i+1;
				p4 += tamano.ToString () + ", ";
			}
		}
		p4 = p4.Substring (0, p4.Length-2);
		print ("p4: " + p4);

		p5 = data.respProducto.ToString ();
		print ("p5: " + p5);

		string jsonData = "" +
		                  "{" +
		                  "\n\"contacto_id\":" + PlayerPrefs.GetInt ("UserId") + "," +
		                  "\n\"cuestionario\":[" +
		                  
		                  "\n{" +
		                  "\n\"pregunta\":\"Que tipo de espacio es el que se desea rediseñar\"," +
		                  "\n\"respuesta\":\"" + p1 + "\"" +
		                  "\n}," +

		                  "\n{" +
		                  "\n\"pregunta\":\"Cual es su estilo\"," +
		                  "\n\"respuesta\":\"" + p2 + "\"" +
		                  "\n}," +

		                  "\n{" +
		                  "\n\"pregunta\":\"Que tipo de producto le gustaria\"," +
		                  "\n\"respuesta\":\"" + p3 + "\"" +
		                  "\n}," +

		                  "\n{" +
		                  "\n\"pregunta\":\"Tamaño\"," +
		                  "\n\"respuesta\":\"" + p4 + "\"" +
		                  "\n}," +

		                  "\n{" +
		                  "\n\"pregunta\":\"Donde quiere el producto\"," +
		                  "\n\"respuesta\":\"" + p5 + "\"" +
		                  "\n}" +

		                  "\n]" +
			"}";
		byte[] pData = Encoding.ASCII.GetBytes (jsonData.ToCharArray());

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/propuesta", pData, headers);
		yield return www;
		propResp = JsonUtility.FromJson<PropRespClass> (www.text);
		PlayerPrefs.SetInt ("UserPropID", propResp.PROP_ID);
		print ("Info prop: " + propResp.RESULTADO + ", " + propResp.PROP_ID);

        consultaStatusBanco.ReemplazarCabecera("pProp_id", propResp.PROP_ID.ToString());
        consultaStatusBanco.ConsultaConHeaders();

		//Precio
		Dictionary <string, string> headersPrecio = new Dictionary<string, string> ();
		headersPrecio.Add ("Content-Type", "application/json");
		headersPrecio.Add ("pClaveApp", claveApp);
		headersPrecio.Add ("Authorization", "Bearer " + tokenProp.Token);

		WWW wwwPrecio = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/consulta/costo", null, headersPrecio);
		yield return wwwPrecio;
		print ("precioResp: " + wwwPrecio.text);
		precio = JsonUtility.FromJson<PrecioResp> (wwwPrecio.text);
		cantidad = precio.COSTO.ToString ();

		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
#if UNITY_ANDROID
			requestHTML ();
#else
		inAppPurchaseApple();
#endif
	}

  
	public void PostPropCall (){
//#if UNITY_ANDROID
			if (GetComponent<Factura> ().facturaInfoFilled){
				StartCoroutine (getTokenProp ());
			}
//#endif
	}

	private IEnumerator postImg()
    {
		transform.GetComponent<ViewsManager> ().showPopUp (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().childrenViews [6].SetActive (true);
        ScriptFotosNativo camaraScript = FindObjectOfType<ScriptFotosNativo>();

		List<Texture2D> fotos = new List<Texture2D> ();
		List<string> descs = new List<string> ();

		foreach (string s in camaraScript.AlmacenadorFotos)
        {
			Texture2D t = new Texture2D (1, 1);
			t.LoadImage (File.ReadAllBytes (s));
			fotos.Add (t);
		}
		descs = camaraScript.AlmacenadorDescripciones;

		for (int i=0; i<fotos.Count; i++)
        {
			Dictionary <string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Content-Type", "application/json");
			headers.Add ("Cookie", "Interceramic");
			headers.Add ("Authorization", "Bearer " + tokenProp.Token);
			headers.Add ("pProp_id", PlayerPrefs.GetInt ("UserPropID").ToString());
			headers.Add ("pDesc", descs[i]);
            Debug.Log(tokenProp.Token);
			byte[] pData = fotos [i].EncodeToPNG ();

			WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/carga/imagen", pData, headers);

			float currentProgress = 0f;
			float lastProgress = 0f;
			float realProgress = i/fotos.Count;

			while (www.uploadProgress < 1)
            {
				lastProgress = currentProgress;
				currentProgress = www.uploadProgress;

				realProgress += (currentProgress - lastProgress);
				transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().background.fillAmount = realProgress;
				yield return new WaitForEndOfFrame ();
			}

			yield return www;

			print ("Img resp: " + www.text);
		}

		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
		transform.GetComponent<ViewsManager> ().hidePopUpNoAnim (2);
		transform.GetComponent<ViewsManager> ().popUps [2].GetComponent<LoadingManager> ().background.fillAmount = 0f;
#if UNITY_ANDROID
		string responseText = 		"Pago aprobado";
		responseText += 			"\n\nReferencia: " + resp.referencia;
		responseText +=				"\nAutorización: " + resp.aut;
		responseText +=				"\nTarjeta habiente: " + resp.ccName;
		responseText +=				"\nNúmero de tarjeta: " + resp.ccNum;
		responseText +=				"\nMonto: " + resp.amount;
		responseText +=				"\nDivisa: MXN";
		responseText +=				"\nHora: " + date_hour;
		responseText +=				"\n\nUn correo electrónico será enviado con la confirmación";
		GameObject.Find ("PayRespText").GetComponent<Text> ().text = responseText;
		transform.GetComponent<ViewsManager> ().showPopUp (4);
#else
		stopLoadingAnimation();
		string responseText = "Pago aprobado";
		responseText += "\n\nEn minutos le llegara la confirmación de compra\n\n";
		transform.GetComponent<LogIn>().reiniciarCuestionario();
		transform.GetComponent<Registrar>().registerNotSuccess(responseText);
		transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().goBack(2);
#endif


	}

	private IEnumerator getTokenCancelProp (){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		print ("TokenProp: " + www.text);
		tokenProp = JsonUtility.FromJson<TokenClass> (www.text);

		StartCoroutine (CancelProp ());
	}

	private IEnumerator CancelProp (){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + tokenProp.Token);
		headers.Add ("pProp_id", PlayerPrefs.GetInt ("UserPropID").ToString());

		byte[] pData = new byte[3];

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/soporte/cancelaProp", pData, headers);
		yield return www;

		print ("CancelResp: " + www.text);
	}

	void OnApplicationQuit (){
		StartCoroutine (getTokenCancelProp ());
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		stopLoadingAnimation();
		Debug.Log("No pude comprar porque: initialize " + error.ToString());
		throw new NotImplementedException();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		stopLoadingAnimation();
		PaymentTicket ticket = new PaymentTicket();
		ticket.Prop_id = ""+propResp.PROP_ID;
		ticket.Store = "AppleAppStore";
		ticket.Status = "PaymentAccepted";
		ticket.TransactionID = JsonUtility.FromJson<UnityPaymentReceipt>(e.purchasedProduct.receipt.ToString()).TransactionID;
		ticket.Payload = JsonUtility.FromJson<UnityPaymentReceipt>(e.purchasedProduct.receipt.ToString()).Payload;
		ticket.calle = Factura.invoiceData.calle;
		ticket.municipio = Factura.invoiceData.municipio;
		ticket.cliente = Factura.invoiceData.cliente;
		ticket.colonia = ""+Factura.invoiceData.colonia;
		ticket.cp = Factura.invoiceData.cp;
		ticket.localidad = Factura.invoiceData.localidad;
		ticket.estado = Factura.invoiceData.estado;
		ticket.numext = Factura.invoiceData.numext;
		ticket.numint = ""+Factura.invoiceData.numint;
		ticket.pais = Factura.invoiceData.pais;
		ticket.rfc = Factura.invoiceData.rfc;
		ticket.telefono = ""+Factura.invoiceData.telefono;
		Debug.Log("Lo que se envia: " + ticket.calle);
		Debug.Log("Lo que se envia: " + ticket.municipio);
		Debug.Log("Lo que se envia: " + ticket.cliente);
		Debug.Log("Lo que se envia: " + ticket.rfc);
		Debug.Log("Lo que se envia: " + ticket.localidad);
		GetComponent<AmazonRequest>().type = InterceramicPetitionType.SEND_PAYMENT_RESULT;
		GetComponent<AmazonRequest>().sendPaymentResponse(JsonUtility.FromJson<UnityPaymentReceipt>(e.purchasedProduct.receipt.ToString()).TransactionID, ticket);//+PlayerPrefs.GetInt("UserId"));

		ApprovedPayment();
		Debug.Log("Purchase price :3" + e.purchasedProduct.metadata.localizedPriceString);
		Debug.Log("Purchase receipt :3" + e.purchasedProduct.receipt.ToString());
		Debug.Log("Purchase transaction id :3" + e.purchasedProduct.transactionID);
		Debug.Log("Purchase availalble :3" + e.purchasedProduct.availableToPurchase);
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.Log("No pude comprar porque: " + p.ToString());
		stopLoadingAnimation();
		PaymentTicket ticket = new PaymentTicket();
		ticket.Prop_id = "" + propResp.PROP_ID;
		ticket.Store = "AppleAppStore";
		ticket.Status = p.ToString();
		ticket.TransactionID = i.transactionID;
		ticket.Payload = "error";

		//GetComponent<AmazonRequest>().type = InterceramicPetitionType.SEND_PAYMENT_RESULT;
		//GetComponent<AmazonRequest>().sendPaymentResponse(i.transactionID, ticket);//+PlayerPrefs.GetInt("UserId"));

		Debug.Log("No pude comprar porque: " + p.ToString());

		string failText = "Pago declinado";
		failText += "\n\nMotivo: " + p.ToString();
		failText += "\nReferencia: " + i.transactionID;
		failText += "\n\nInténtalo de nuevo\n\n";
		transform.GetComponent<Registrar>().registerNotSuccess(failText);
		transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().goBack(8);
		throw new NotImplementedException();
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("Initialize :3" + controller.products.all[0].metadata.localizedPriceString);
		controller.InitiatePurchase(controller.products.all[0]);
		//throw new NotImplementedException();
	}

	public void inAppPurchaseApple() {

		var module = StandardPurchasingModule.Instance();
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
		builder.AddProduct("com_interceramic_miarq_propuesta", ProductType.Consumable);
		UnityPurchasing.Initialize(this, builder);
		//transform.GetComponent<ViewsManager>().showPopUpNoAnim(2);
		//transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().startSpinning();
	}

	void loadingAnimation() {
		loader.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
	}

	void stopLoadingAnimation() {
		transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().stopSpinning();
		transform.GetComponent<ViewsManager>().hidePopUpNoAnim(2);
		transform.GetComponent<ViewsManager>().hideAll();
	}
}
