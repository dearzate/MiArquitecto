using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ViewsManager : MonoBehaviour {

	public GameObject[] views;
	public int MainView = 0;
	public int activeView = 0;
	public int lastView = 0;

	public GameObject[] popUps;

	public Text[] nombresUsuario;
	public Text[] costos;

	public GameObject terminos;
	public GameObject aviso;
	public GameObject Expand;
	public GameObject Tutorial;
	public GameObject Encuesta;

	public bool guestMode = false;

	private static float speed = 2f;
	private bool comesFromLogin = false;
	private bool expandIsShown = false;
	private bool tutorialIsShown = false;
	public bool popIsShown = false;
	private bool encuestaIsShown = false;

	private int sWidth;

	public class TokenClass{
		public string Token;
		public string Expire_in;
	}
	TokenClass tokenProp;

	public class PrecioResp{
		public string RESULTADO;
		public int MONTO;
	}

	private PrecioResp bonificacion;
	private string cantidad;
	public string precioArqui;

	void Awake(){
		Application.targetFrameRate = 60;
		StartCoroutine (getBonificacion ());
	}

	void Start () {
		bool showpop = false;
		sWidth = Screen.width;
		if (PlayerPrefs.GetInt ("LoggedIn") == 0) {
			views [0].transform.localPosition = new Vector3 (0, 0, 0);
			goToIndexNoAnim (MainView, "izquierda");
		}
		else {
			views [0].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			views [0].GetComponent<View> ().childrenViews [0].transform.localPosition = Vector3.zero;
			// Show new popup
			showpop = true;
		}
		initPopUps ();
		initTerminos ();
		views[1].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		if (showpop){
			StartCoroutine (getPrecio ());
		}
		foreach (Text t in nombresUsuario) {
			t.text = PlayerPrefs.GetString ("UserName");
		}
	}

	public void setNameLabel(){
		foreach (Text t in nombresUsuario) {
			t.text = PlayerPrefs.GetString ("UserName");
		}
	}

	public void verAviso (){
		StartCoroutine (showAviso ());
	}

	public void esconderAvisoNoAcepta (){
		StartCoroutine (hideAviso ());
	}

	public void changeAvisoImg (Sprite i){
		GameObject.Find ("Aviso_Image").GetComponent<Image> ().sprite = i;
		GameObject.Find ("Aviso_Txt").GetComponent<Text> ().text = "";
	}

	public void changeAvisoTxt (int o){
		string s;

		if (o == 0){
			s = "Si compras en nuestras sucursales los productos de alguna de las propuestas, y el monto de la compra es mayor al importe requerido para bonificación $*, se te bonificara el costo del servicio de Mi Arquitecto en tu compra.";
			s = s.Replace ("*", cantidad);
		}
		else {
			s = "Si tienes alguna duda sobre el uso del App, necesitas localizar una sucursal y/o el proceso para la bonificación del costo del Servicio de Mi Arquitecto, contáctanos en el Centro de Atención Interceramic, desde nuestra página web www.interceramic.com o llama al 01-800-725-1010.";
		}
		GameObject.Find ("Aviso_Image").GetComponent<Image> ().sprite = null;
		GameObject.Find ("Aviso_Txt").GetComponent<Text> ().text = s;
	}

	private IEnumerator getBonificacion (){
		//Token
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", "MIARQ");

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		tokenProp = JsonUtility.FromJson<TokenClass> (www.text);

		//Bonificacion
		Dictionary <string, string> headersPrecio = new Dictionary<string, string> ();
		headersPrecio.Add ("Content-Type", "application/json");
		headersPrecio.Add ("pClaveApp", "MIARQ");
		headersPrecio.Add ("Authorization", "Bearer " + tokenProp.Token);

		WWW wwwPrecio = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/consulta/bonificacion", null, headersPrecio);
		yield return wwwPrecio;
		print ("bonificacionResp: " + wwwPrecio.text);
		bonificacion = JsonUtility.FromJson<PrecioResp> (wwwPrecio.text);
		cantidad = bonificacion.MONTO.ToString ();
	}

	public IEnumerator getPrecio (){
		//Token
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", "MIARQ");

		WWW www = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/genera/token", null ,headers);
		yield return www;
		tokenProp = JsonUtility.FromJson<TokenClass> (www.text);

		//Precio
		Dictionary <string, string> headersPrecio = new Dictionary<string, string> ();
		headersPrecio.Add ("Content-Type", "application/json");
		headersPrecio.Add ("pClaveApp", "MIARQ");
  		headersPrecio.Add ("Authorization", "Bearer " + tokenProp.Token);

		WWW wwwPrecio = new WWW ("https://swsp.interceramic.com/ords/miarqws_sa/consulta/costo", null, headersPrecio);
		yield return wwwPrecio;
		print ("precioResp: " + wwwPrecio.text);
		WebPayView.PrecioResp precio = JsonUtility.FromJson<WebPayView.PrecioResp> (wwwPrecio.text);
		precioArqui = precio.COSTO.ToString ();

		foreach (Text t in costos){
			t.text = "$"+precioArqui;
		}

		string s = "Mi arquitecto es un App que te brinda el servicio de propuestas de diseño, hechas por nuestros profesionales y  usando los productos para piso y pared de Interceramic simulados en tu espacio.\n\nEl costo por servicio es de $*,  cada servicio corresponde únicamente a la propuesta de diseño de un espacio (Baños, Cocina, Comedores, Pasillos, Recamaras, Salas, Exteriores o Comerciales).";
		s = s.Replace ("*", precioArqui);

		GameObject.Find ("Aviso_Init").GetComponent<Text> ().text = s;

		showPopUp (10);
	}
		
	public void esconderAvisoAcepta (){
		if (comesFromLogin){
			transform.GetComponent<LogIn> ().UpdateUserTerminosInfo();
		}
		else {
			transform.GetComponent<Registrar> ().Privacidad.isOn = true;
		}
		StartCoroutine (hideAviso ());
	}

	public void verTerminos(){
		setComesFromLogin(false);
		StartCoroutine (showTerminos ());
	}

	public void esconderTerminosNoAcepta (){
		StartCoroutine (hideTerminos ());
	}

	public void esconderTerminos(){
		if (comesFromLogin) {
//			transform.GetComponent<LogIn> ().UpdateUserTerminosInfo();
			StartCoroutine (showAviso ());
		}
		else {
			transform.GetComponent<Registrar> ().Terminos.isOn = true;
		}
		StartCoroutine (hideTerminos ());
	}

	public void verExpand(int n){
		Expand.GetComponent<ExpandView> ().resetPos ();
		Expand.GetComponent<ExpandView> ().changeImagesAndText (n);
		StartCoroutine (showExpand());
	}

	public void esconderExpand(){
		StartCoroutine (hideExpand());
	}

	public void verEncuesta (){
		StartCoroutine (showEncuesta ());
	}

	public void esconderEncuesta (){
		StartCoroutine (hideEncuesta ());
	}

	public void verTutorial(){
		GameObject.Find ("Tutorial").GetComponent<TutorialView> ().resetPos ();
		GameObject.Find ("Tutorial").GetComponent<TutorialView> ().changeImagesAndText ();
		StartCoroutine (showTutorial ());
	}

	public void esconderTutorial(){
		StartCoroutine (hideTutorial ());
	}

	public bool getExpandIsShown(){
		return expandIsShown;
	}

	public bool getTutorialIsShown(){
		return tutorialIsShown;
	}

	public void setComesFromLogin(bool login){
		comesFromLogin = login;
	}

	public void initTerminos(){
		aviso.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		terminos.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		Expand.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		Tutorial.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		Encuesta.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
	}

	public void initPopUps() {
		foreach (GameObject pop in popUps) {
			pop.transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
		}
	}

	public void showPopUp(int index) {
		StartCoroutine (showPop (index));
	}

	public void hidePopUp(int index) {
		StartCoroutine (hidePop (index));
	}

	public void showPopUpNoAnim(int index){
		popIsShown = true;
		popUps [index].transform.localPosition = new Vector3 (0, 0, 0);
	}

	public void hidePopUpNoAnim(int index){
		popIsShown = false;
		popUps [index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
	}

	public void goToIndexNoMove(int index){
		StartCoroutine (moveThisCenter (index));
		activeView = index;
	}
		
	public void goToIndex(int index, string animation = "izquierda") {
		for (int i = 0; i < views.Length; i++) {
			if (i != index && animation == "derecha") {
				StartCoroutine (moveThisRight (i));
			} 
			else if (i != index && animation == "izquierda") {
				StartCoroutine (moveThisLeft (i));
			} 
			else {
				StartCoroutine (moveThisCenter (index));
			}
		}
		activeView = index;
	}

	public void goToIndexNoAnim(int index, string animation = "izquierda") {
		for (int i = 0; i < views.Length; i++) {
			if (i != index && animation == "derecha")
				views [i].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			else if (i != index && animation == "izquierda")
				views [i].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
			else
				views [index].transform.localPosition = new Vector3 (0, 0, 0);
		}
		activeView = index;
	}

	public void hideAll(string animation = "izquierda") {
		int i = 0;
		foreach (GameObject view in views) {
			if (animation == "izquierda")
				StartCoroutine (moveThisLeft (i));
			else if (animation == "derecha")
				StartCoroutine (moveThisRight (i));
			i++;
		}
		lastView = activeView;
	}

	public void setActiveAll(bool active){
		foreach (GameObject view in views) {
			view.SetActive (active);
		}
		foreach (GameObject pop in popUps){
			pop.SetActive (active);
		}
		terminos.SetActive (active);
		views [0].GetComponent<View> ().childrenViews [0].SetActive (active);
	}

	public void hideAllNoAnim(string animation =  "izquierda"){
		int i = 0;
		foreach (GameObject view in views) {
			if (animation == "izquierda")
				views [i].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
			else if (animation == "derecha")
				views [i].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			i++;
		}
		lastView = activeView;
	}

	private IEnumerator showEncuesta(){
		#if UNITY_ANDROID
		Encuesta.transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (Encuesta.transform.localPosition.x < 0.1f) {
				Encuesta.transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				Encuesta.transform.localPosition = Vector3.Lerp (Encuesta.transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
		encuestaIsShown = true;
	}

	private IEnumerator hideEncuesta(){
		if (encuestaIsShown) {
			#if UNITY_ANDROID
			Encuesta.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			yield return null;
			#else
			float elapsedTime = 0f;
			while (elapsedTime < speed) {
				if (Encuesta.transform.localPosition.x > sWidth * 1.9f) {
					Encuesta.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
					elapsedTime = speed;
				}
				else {
					Encuesta.transform.localPosition = Vector3.Lerp (Encuesta.transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime / speed);
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			#endif
			encuestaIsShown = false;
		}
	}

	private IEnumerator showTutorial(){
		#if UNITY_ANDROID
		Tutorial.transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		float alpha = 0f;
		Color c = Tutorial.GetComponent<Image> ().color;
		Tutorial.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0f);
		while (elapsedTime < speed) {
			if (Tutorial.transform.localPosition.x < 0.1f) {
				Tutorial.transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				Tutorial.transform.localPosition = Vector3.Lerp (Tutorial.transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		while (alpha < 0.7f){
			alpha += Time.deltaTime*2;
			Tutorial.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, alpha);
			yield return null;
		}
		#endif
		tutorialIsShown = true;
	}

	private IEnumerator hideTutorial(){
		if (tutorialIsShown) {
			#if UNITY_ANDROID
			Tutorial.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			yield return null;
			#else
			float elapsedTime = 0f;
			Color c = Tutorial.GetComponent<Image> ().color;
			Tutorial.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0);
			while (elapsedTime < speed) {
				if (Tutorial.transform.localPosition.x > sWidth * 1.9f) {
					Tutorial.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
					elapsedTime = speed;
				}
				else {
					Tutorial.transform.localPosition = Vector3.Lerp (Tutorial.transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime / speed);
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			#endif
			tutorialIsShown = false;
		}
	}

	private IEnumerator showExpand(){
		#if UNITY_ANDROID
		Expand.transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		float alpha = 0f;
		Color c = Expand.GetComponent<Image> ().color;
		Expand.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0f);
		while (elapsedTime < speed) {
			if (Expand.transform.localPosition.x < 0.1f) {
				Expand.transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				Expand.transform.localPosition = Vector3.Lerp (Expand.transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		while (alpha < 0.7f){
			alpha += Time.deltaTime*2;
			Expand.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, alpha);
			yield return null;
		}
		#endif
		expandIsShown = true;
	}
	
	private IEnumerator hideExpand(){
		#if UNITY_ANDROID
		Expand.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		yield return null;
		#else
		if (expandIsShown) {
			float elapsedTime = 0f;
			Color c = Expand.GetComponent<Image> ().color;
			Expand.GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0);
			while (elapsedTime < speed) {
				if (Expand.transform.localPosition.x > sWidth * 1.9f) {
					Expand.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
					elapsedTime = speed;
				}
				else {
					Expand.transform.localPosition = Vector3.Lerp (Expand.transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime / speed);
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
		#endif
		expandIsShown = false;
	}

	private IEnumerator showTerminos(){
		#if UNITY_ANDROID
		terminos.transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (terminos.transform.localPosition.x < 0.1) {
				terminos.transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				terminos.transform.localPosition = Vector3.Lerp (terminos.transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator hideTerminos(){
		#if UNITY_ANDROID
		terminos.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (terminos.transform.localPosition.x > sWidth * 1.9f) {
				terminos.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
				elapsedTime = speed;
			}
			else {
				terminos.transform.localPosition = Vector3.Lerp (terminos.transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator showAviso (){
		#if UNITY_ANDROID
		aviso.transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (aviso.transform.localPosition.x < 0.1) {
				aviso.transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				aviso.transform.localPosition = Vector3.Lerp (aviso.transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator hideAviso (){
		#if UNITY_ANDROID
		aviso.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (aviso.transform.localPosition.x > sWidth * 1.9f) {
				aviso.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
				elapsedTime = speed;
			}
			else {
				aviso.transform.localPosition = Vector3.Lerp (aviso.transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator moveThisRight (int index) {
		#if UNITY_ANDROID
		views [index].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (views [index].transform.localPosition.x > sWidth * 1.9f) {
				views [index].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
				elapsedTime = speed;
			}
			else {
				views [index].transform.localPosition = Vector3.Lerp (views [index].transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator moveThisLeft (int index) {
		#if UNITY_ANDROID
		views [index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (views [index].transform.localPosition.x < sWidth *-1.9f) {
				views [index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
				elapsedTime = speed;
			}
			else {
				views [index].transform.localPosition = Vector3.Lerp (views [index].transform.localPosition, new Vector3 (sWidth *-2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator moveThisCenter (int index) {
		#if UNITY_ANDROID
		views [index].transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (views [index].transform.localPosition.x < 0.1) {
				views [index].transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				views [index].transform.localPosition = Vector3.Lerp (views [index].transform.localPosition, new Vector3 (0, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

	private IEnumerator showPop (int index){
		#if UNITY_ANDROID
		popUps[index].transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		float alpha = 0f;
		float speed2 = speed / 2;
		Color c = popUps[index].GetComponent<Image> ().color;
		popUps[index].GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0f);
		while (elapsedTime < speed) {
			if (popUps[index].transform.localPosition.x > -0.1f) {
				popUps[index].transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				popUps[index].transform.localPosition = Vector3.Lerp (popUps[index].transform.localPosition, new Vector3 (0, 0, 0), elapsedTime / speed2);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		while (alpha < 0.7f){
			alpha += Time.deltaTime*2;
			popUps[index].GetComponent<Image> ().color = new Color(c.r, c.g, c.b, alpha);
			yield return null;
		}
		#endif
		popIsShown = true;
	}

	private IEnumerator hidePop (int index){
		#if UNITY_ANDROID
		popUps[index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
		yield return null;
		#else
		float elapsedTime = 0f;
		float speed2 = speed / 2;
		Color c = popUps[index].GetComponent<Image> ().color;
		popUps[index].GetComponent<Image> ().color = new Color(c.r, c.g, c.b, 0);
		while (elapsedTime < speed) {
			if (popUps[index].transform.localPosition.x < sWidth *-1.9f) {
				popUps[index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
				elapsedTime = speed;
			}
			else {
				popUps[index].transform.localPosition = Vector3.Lerp (popUps[index].transform.localPosition, new Vector3 (sWidth *-2, 0, 0), elapsedTime / speed2);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
		popIsShown = false;
	}
}
