using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Events;
using Tastybits.NativeGallery;
using UnityEngine.EventSystems;

public class ScriptFotosNativo : MonoBehaviour {

	// Imagen donde se vera el render de la camara
	public RawImage ImagenCamara;
	// Objeto de la texturaWebCam
	[HideInInspector]
	public WebCamTexture TexturaWebCam;
	// Textura pivote donde se guarda la captura activa de la camara
	[HideInInspector]
	public Texture2D TexturaPivote;
	// Imagen para previsualizar foto
	public RawImage ImagenPrevisualizar;
	public Image ImagenPlayVideo;
	// Fondo opcional para colocar imagenes nulas
	public Sprite ImagenNula;
	// Imagen para colocar el ultimo preview
	public RawImage ImagenUltimoPreview;

	// Variables de apoyo a la funcionalidad
	public bool GuardarAlCapturar;
	public bool GuardarEnSprite;
	[HideInInspector]
	public int PivoteFotos;

	// Lista de fotos a almacenar como maximo
	[HideInInspector]
	public List<string> AlmacenadorFotos;
	[HideInInspector]
	public List<string> AlmacenadorDescripciones;
	public RawImage[] ImagenesParaAsignarFotos;

	public GameObject TemplateImagenAsignar;
	public GameObject ImagenesScroll;
	public InputField Descripcion;
	public GameObject BotonTomarFoto;
	public GameObject BotonTerminoFotos;
	public GameObject ActualizarDesc;
	public GameObject EliminarImg;
	public GameObject AcptDesc;
	public GameObject InputFieldDesc;
	public GameObject BotonGaleria;
	public GameObject Pregunta;

	[HideInInspector]
	public List<GameObject> ArregloImagenes;

	public Image[] BotonesVideo;

	[HideInInspector]
	public int IndiceVideo;

	// Evento cuando se previsualiza
	public UnityEvent EventoAlPrevisualizar;
	public UnityEvent EventoAlTomarFoto;	

	private int lastIndexSelected = 0;

	private bool scrollShouldActive = true;
	private Vector2 rescaleFactor = new Vector2 (810, 1290);
	private Vector2 sizeDeltaImagen;

	// Variable que sirve para ver fotos con botones siguiente y anterior
	private int IndexFotoActiva;
	public Button BotonAvanza;
	public Button BotonRetrocede;

	private bool comprobacionHecha = false;

	public void TerminoFotos(){
		foreach (string s in AlmacenadorFotos){
			print (s);
		}
		foreach (string s in AlmacenadorDescripciones){
			print (s);
		}
	}

	public void OnOpenGalleryButtonClicked() { 
		Tastybits.NativeGallery.ImagePicker.OpenGallery( ( Texture2D tex ) => { 
			SaveTextureInArray(tex);
		} );
	}

	private void SaveTextureInArray(Texture2D tex){
		ReescaleImages (tex.width, tex.height);
		print ("Galeria size: " + tex.width + ", " + tex.height);

		string RutaCaptura = Path.Combine (Application.persistentDataPath, "Imagen" +
											DateTime.Now.Day.ToString () +
											DateTime.Now.Month.ToString () +
											DateTime.Now.Year.ToString () +
											DateTime.Now.Hour.ToString () +
											DateTime.Now.Minute.ToString () +
											DateTime.Now.Second.ToString () + ".png");
		TexturaPivote = tex;

		Pregunta.SetActive (false);

		File.WriteAllBytes ( RutaCaptura , TexturaPivote.EncodeToJPG() );
		AlmacenadorFotos.Add (RutaCaptura);

		GameObject nIm = (GameObject)Instantiate(TemplateImagenAsignar);

		nIm.GetComponentInChildren<RawImage> ().texture = tex;
		float aspect = (tex.width * 1.0f) / tex.height;

		if (tex.width > tex.height) {
			nIm.GetComponentInChildren<RawImage> ().rectTransform.sizeDelta = new Vector2 (256f, 256f / aspect);
		}
		else {
			nIm.GetComponentInChildren<RawImage> ().rectTransform.sizeDelta = new Vector2 (150f, 150f / aspect);
		}

		nIm.transform.SetParent (ImagenesScroll.transform, false);
		scrollShouldActive = false;

		ImagenCamara.gameObject.SetActive (false);
		ImagenUltimoPreview.gameObject.SetActive (true);
		AcptDesc.SetActive (true);
		InputFieldDesc.SetActive (true);
		BotonTerminoFotos.SetActive (false);
		BotonTomarFoto.SetActive (false);
		BotonGaleria.SetActive (false);

		nIm.GetComponent<Fotos> ().pathFoto = AlmacenadorFotos [AlmacenadorFotos.Count - 1];
		nIm.GetComponent<Button> ().onClick.AddListener (() => {
			if(scrollShouldActive){
				print ("From Gallery path:_________ " + nIm.GetComponent<Fotos>().pathFoto);
				ImagenUltimoPreview.gameObject.SetActive(true);
				ImagenCamara.gameObject.SetActive(false);
				Texture2D newFotillo = new Texture2D (1, 1);
				newFotillo.LoadImage (File.ReadAllBytes (nIm.GetComponent<Fotos>().pathFoto));
				ReescaleImages (newFotillo.width, newFotillo.height);
				ImagenUltimoPreview.texture = newFotillo;

				BotonTomarFoto.SetActive(false);
				BotonTerminoFotos.SetActive(false);
				BotonGaleria.SetActive(false);
				//ControlPanel.SetActive (true);
				Descripcion.gameObject.SetActive(true);
				lastIndexSelected = AlmacenadorFotos.FindIndex(s => s == nIm.GetComponent<Fotos>().pathFoto);
				print ("lastIndexSelected: " + lastIndexSelected);
				Descripcion.text = AlmacenadorDescripciones[AlmacenadorFotos.FindIndex(s => s == nIm.GetComponent<Fotos>().pathFoto)];
				ActualizarDesc.SetActive(true);
				EliminarImg.SetActive(true);
				Pregunta.SetActive(false);
			}
		});
		ArregloImagenes.Add (nIm);
		AlmacenadorDescripciones.Add ("");

		ImagenUltimoPreview.texture = tex;
		Descripcion.text = "";
	}

	public void AceptarDescripcion(){
		AlmacenadorDescripciones [AlmacenadorDescripciones.Count - 1] = Descripcion.text;
		scrollShouldActive = true;
		ReescaleImages ();
	}

	public void ActualizarDescripcion(){
		AlmacenadorDescripciones [lastIndexSelected] = Descripcion.text;
		scrollShouldActive = true;
		ReescaleImages ();
	}

	public void EliminarImagen(){
		AlmacenadorFotos.RemoveAt (lastIndexSelected);
		AlmacenadorDescripciones.RemoveAt (lastIndexSelected);
		ArregloImagenes [lastIndexSelected].transform.SetParent (null);
		ArregloImagenes.RemoveAt (lastIndexSelected);
		scrollShouldActive = true;
		ReescaleImages ();
	}

	public void EliminarTodasImagenes(){
		AlmacenadorFotos.Clear ();
		AlmacenadorDescripciones.Clear ();
		foreach (GameObject go in ArregloImagenes) {
			go.transform.SetParent (null);
		}
		ArregloImagenes.Clear ();
	}

	public void Agregar(){
		ReescaleImages ();
		GameObject nIm = (GameObject)Instantiate(TemplateImagenAsignar);

		Texture2D fotillo = new Texture2D (1, 1);
		fotillo.LoadImage (File.ReadAllBytes (AlmacenadorFotos[AlmacenadorFotos.Count - 1]));

		nIm.GetComponentInChildren<RawImage> ().texture = fotillo;

		nIm.transform.SetParent (ImagenesScroll.transform, false);
		scrollShouldActive = false;

		float aspect = (fotillo.width * 1.0f) / fotillo.height;
		nIm.GetComponentInChildren<RawImage> ().rectTransform.sizeDelta = new Vector2 (150f, 150f / aspect);

		if (fotillo.width == fotillo.height){
			nIm.transform.localEulerAngles = new Vector3 (0, 0, -90);
		}

		nIm.GetComponent<Fotos> ().pathFoto = AlmacenadorFotos [AlmacenadorFotos.Count - 1];
		nIm.GetComponent<Button> ().onClick.AddListener (() => {
			if(scrollShouldActive){
				print ("path:_________ " + nIm.GetComponent<Fotos>().pathFoto);
				ReescaleImages ();
				ImagenUltimoPreview.gameObject.SetActive(true);
				ImagenCamara.gameObject.SetActive(false);
				Texture2D newFotillo = new Texture2D (1, 1);
				newFotillo.LoadImage (File.ReadAllBytes (nIm.GetComponent<Fotos>().pathFoto));
				ImagenUltimoPreview.texture = newFotillo;

				BotonTomarFoto.SetActive(false);
				BotonTerminoFotos.SetActive(false);
				BotonGaleria.SetActive(false);
				Descripcion.gameObject.SetActive(true);
				lastIndexSelected = AlmacenadorFotos.FindIndex(s => s == nIm.GetComponent<Fotos>().pathFoto);
				print ("lastIndexSelected: " + lastIndexSelected);
				Descripcion.text = AlmacenadorDescripciones[AlmacenadorFotos.FindIndex(s => s == nIm.GetComponent<Fotos>().pathFoto)];
				ActualizarDesc.SetActive(true);
				EliminarImg.SetActive(true);
				Pregunta.SetActive(false);
			}
		});
		ArregloImagenes.Add (nIm);
		AlmacenadorDescripciones.Add ("");

		ImagenUltimoPreview.texture = fotillo;
		Descripcion.text = "";
	}

	public void ReescaleImages(){

		float w = TexturaWebCam.width * 1.0f;
		float h = TexturaWebCam.height * 1.0f;
		float aspect = w / h;

		ImagenCamara.GetComponent<RectTransform> ().SetSize (new Vector2 (1440f, 1440f / aspect));
		ImagenUltimoPreview.GetComponent<RectTransform> ().SetSize (new Vector2 (810f, 810f * aspect));

		#if UNITY_ANDROID
		if (TexturaWebCam.width == TexturaWebCam.height){
			Debug.Log ("Giro!");
			ImagenUltimoPreview.transform.localEulerAngles = new Vector3 (180, 0, 0);
		}
		#endif
	}

	public void ReescaleImages(float w, float h){
		float aspect = w / h;

		if (w > h) {
			ImagenUltimoPreview.GetComponent<RectTransform> ().SetSize (new Vector2 (810f, 810f / aspect));
		}
		else {
			ImagenUltimoPreview.GetComponent<RectTransform> ().SetSize (new Vector2 (810f, 810f / aspect));
		}

		#if UNITY_ANDROID
		if (TexturaWebCam.width == TexturaWebCam.height){
			Debug.Log ("Giro!");
			ImagenUltimoPreview.transform.localEulerAngles = new Vector3 (180, 0, 0);
		}
		#endif
	}

	public RectTransform ReescaleImage(RectTransform rt, Texture2D tex){
		float w = tex.width * 1.0f;
		float h = tex.height * 1.0f;
		float aspect = w / h;
		Vector2 newSize;
		if (w > h) {
			print ("landscape");
			newSize  = new Vector2 (w, w * aspect);
		}
		else {
			print ("portrait");
			newSize  = new Vector2 (h * aspect, h);
		}

		Vector2 oldSize = rt.rect.size;
		Vector2 deltaSize = newSize - oldSize;
		rt.offsetMin = rt.offsetMin - new Vector2 (deltaSize.x * rt.pivot.x, deltaSize.y * rt.pivot.y);
		rt.offsetMax = rt.offsetMax + new Vector2 (deltaSize.x * (1f - rt.pivot.x), deltaSize.y * (1f - rt.pivot.y));

		return rt;
	}

	void Start ()
    {


		Screen.fullScreen = false;
		sizeDeltaImagen = ImagenCamara.rectTransform.sizeDelta;

		ArregloImagenes = new List<GameObject> ();
		// Inicia la camara
		TexturaWebCam = new WebCamTexture (Screen.width, Screen.height);

		// Comprueba si la camara esta en espejo
		StartCoroutine(ComprobarCamara());

		PivoteFotos = 0;

		// Lista que ira conservando las rutas donde se encuentran las fotos tomadas
		AlmacenadorFotos = new List<string>();

		// Entero que indica el indice de video si es que se va a usar
		IndiceVideo = 0;
		IndexFotoActiva = 0;

		ReescaleImages ();



	}

	void Update(){
		if (AlmacenadorFotos.Count > 0 && scrollShouldActive && ImagenCamara.gameObject.activeSelf) {
			BotonTerminoFotos.SetActive (true);
		}
		else if (scrollShouldActive && ImagenCamara.gameObject.activeSelf) {
			BotonTerminoFotos.SetActive (false);
		}

		if (AlmacenadorFotos.Count > 2 && scrollShouldActive) {
			BotonTomarFoto.SetActive (false);
			BotonGaleria.SetActive (false);
		}
		else if (scrollShouldActive && ImagenCamara.gameObject.activeSelf) {
			BotonTomarFoto.SetActive (true);
			BotonGaleria.SetActive (true);
		}
	}

	// Funcion que determina si la camara esta en espejo
	IEnumerator ComprobarCamara()
	{
        if (!UniAndroidPermission.IsPermitted(AndroidPermission.CAMERA))
            UniAndroidPermission.RequestPermission(AndroidPermission.CAMERA);



		// La rutina inicia la camara
		TexturaWebCam.Play ();
		// Espera un segundo a que termine de cargar
		yield return new WaitForSeconds(1);

//		print ("Comprobacion: " + TexturaWebCam.width + ", " + TexturaWebCam.height);
		// Una vez termina, comprueba si no esta en espejo...
		if (!TexturaWebCam.videoVerticallyMirrored) 
		{
			// ... si lo esta, mantiene el giro por defecto
			ImagenCamara.transform.localEulerAngles = new Vector3 (180, 0, 0);
//			Debug.Log ("NO espejo");
		} 
		else 
		{
			// ... si no lo esta, gira la imagen
			Debug.Log ("SI espejo");
		}

		// Detiene la camara

		if (TexturaWebCam.width == TexturaWebCam.height){
			Debug.Log ("Cámara igual!");
		}

		TexturaWebCam.Stop ();
		comprobacionHecha = true;
		GameObject.Find ("LogIn").GetComponent<View> ().setCamaraChecked (true);
	}
		
	public bool camaraComprobacion(){
		return comprobacionHecha;
	}

	// Funcion para iniciar la camara sobre el render objetivo
	public void IniciarCamara()
	{
		TexturaWebCam.Play ();
		ImagenCamara.texture = TexturaWebCam;
	}

	// Funcion que detiene la camara para que no se muestre en partes de la aplicacion donde no esta el render activo
	public void DetenerCamara()
	{
		TexturaWebCam.Stop ();
	}


	/// <summary>
	/// Funcion que toma foto y la captura en un sprite
	/// </summary>
	public void TomarFotoConGuardadoEnSprite()
	{
		print ("lastIndexSelected: " + lastIndexSelected);
		// Calcula la matriz de rotacion para girarla y no se vea horizontal
		TexturaPivote = new Texture2D (TexturaWebCam.height, TexturaWebCam.width);
		if (TexturaWebCam.width > TexturaWebCam.height)
			TexturaPivote = RotateMatrix (TexturaWebCam );
		else
			TexturaPivote.SetPixels (TexturaWebCam.GetPixels ());
		TexturaPivote.Apply ();


		// Si se indica que se debe guardar en una mini-previsualizacion, la asignara al arreglo indicado anteriormente
		if (GuardarEnSprite) 
		{
			if (PivoteFotos <= (ImagenesParaAsignarFotos.Length - 1)) 
			{
				ImagenesParaAsignarFotos [PivoteFotos].texture = TexturaPivote;
				PivoteFotos++;
			}
		}

		// Si hay una imagen asignada como preview temporal...
		if (ImagenUltimoPreview != null)  
		{
			ImagenUltimoPreview.texture = TexturaPivote;
		}

		// Si se indica que al capturarla automaticamente se debe guardar en archivo...
		if (GuardarAlCapturar) 
		{
			// ... genera el nombre...
			string RutaCaptura = Path.Combine (Application.persistentDataPath, "Imagen" +
				DateTime.Now.Day.ToString () +
				DateTime.Now.Month.ToString () +
				DateTime.Now.Year.ToString () +
				DateTime.Now.Hour.ToString () +
				DateTime.Now.Minute.ToString () +
				DateTime.Now.Second.ToString () + ".png");

			Debug.Log ("Imagen guardada en la ruta: " + RutaCaptura);

			// ... y escribe los bytes
			File.WriteAllBytes ( RutaCaptura , TexturaPivote.EncodeToPNG() );

			AlmacenadorFotos.Add (RutaCaptura);
		}

		EventSystem.current.SetSelectedGameObject (Descripcion.gameObject, null);

		EventoAlTomarFoto.Invoke ();
	}

	/// <summary>
	/// Elimina una foto del preview siempre y cuando se haya guardado en sprite
	/// </summary>
	public void EliminarFoto(int Indice, bool BorroVideo)
	{
		// Recorre el carrete de fotos y disminuye el pivote para tomar la siguient
		try
		{
			for (int i = Indice; i < ImagenesParaAsignarFotos.Length; i++)
			{
				
				ImagenesParaAsignarFotos [i].texture = ( (i+1) < ImagenesParaAsignarFotos.Length ) ? ImagenesParaAsignarFotos [i + 1].texture : (ImagenNula == null) ? null : ImagenNula.texture ;
			}

		// Su se estab guardando en archivos, remueve el archivo de la lista
			if (!BorroVideo)
				AlmacenadorFotos.RemoveAt (Indice);

			PivoteFotos--;
		}
		catch 
		{
			Debug.Log ("No se pudo borrar el indice de foto");
		}

		scrollShouldActive = true;
	}

	/// <summary>
	/// Visualiza la ultima foto capturada
	/// </summary>
	public void VisualizarFoto()
	{
		if (ImagenPrevisualizar != null) 
		{
			// Detiene la camara y si el objetivo a presionar tiene textura con nombre vacio...
			DetenerCamara ();
			if (ImagenesParaAsignarFotos [ImagenesParaAsignarFotos.Length - 1].texture != null && ImagenesParaAsignarFotos [ImagenesParaAsignarFotos.Length - 1].texture.name.Equals (string.Empty)) 
			{
				// ... activa el panel de visualizacion y asigna la textura 
				ImagenPrevisualizar.gameObject.SetActive (true);
				ImagenPrevisualizar.texture = ImagenesParaAsignarFotos [ImagenesParaAsignarFotos.Length - 1].texture;
			}
			else 
			{
				Debug.Log ("Estas intentando ver una imagen vacia");
			}



		} 
		else 
		{
			Debug.Log ("Para previsualizar, asigna un RawImage en ImagenPrevisualizar");
		}
	}

	/// <summary>
	/// Abre una foto para su previsualizacion en grande
	/// </summary>
	public void VisualizarFoto(int Indice)
	{
		IndexFotoActiva = Indice;

		// Para no tener referencia nula, muestra mensaje de error si no hay imagen de previsualizacion establecida
		if (ImagenPrevisualizar != null) 
		{
			// Detiene la camara y si el objetivo a presionar tiene textura con nombre vacio...
			DetenerCamara ();
			if (ImagenesParaAsignarFotos [Indice].texture != null && ImagenesParaAsignarFotos [Indice].texture.name.Equals (string.Empty)) 
			{
				// ... activa el panel de visualizacion y asigna la textura 
				ImagenPrevisualizar.gameObject.SetActive (true);
				ImagenPrevisualizar.texture = ImagenesParaAsignarFotos [Indice].texture;
				if (ImagenPlayVideo != null) 
				{
					if (BotonesVideo[Indice].IsActive())
						ImagenPlayVideo.gameObject.SetActive (true);
					else
						ImagenPlayVideo.gameObject.SetActive (false);

					IndiceVideo = -1;

					for (int i = 0; i <= Indice; i++) 
					{
						if (BotonesVideo [i].IsActive ())
							IndiceVideo++;
					}
				}

				if (BotonAvanza != null) 
				{
					int IndicadorPrevisualizador = -1;
					for (int j = 0; j < ImagenesParaAsignarFotos.Length; j++) 
					{
						if (ImagenesParaAsignarFotos[j].texture.name.Equals(""))
							IndicadorPrevisualizador++;
					}

					if (Indice == IndicadorPrevisualizador)
						BotonAvanza.interactable = false;
					else
						BotonAvanza.interactable = true;
				}

				if (BotonRetrocede != null) 
				{
					if (Indice == 0)
						BotonRetrocede.interactable = false;
					else
						BotonRetrocede.interactable = true;
				}

				EventoAlPrevisualizar.Invoke ();
			}
			else 
			{
				Debug.Log ("Estas intentando ver una imagen vacia");
			}

		

		} 
		else 
		{
			Debug.Log ("Para previsualizar, asigna un RawImage en ImagenPrevisualizar");
		}
	}



	public void VerSiguienteFoto()
	{
		int IndicadorPrevisualizador = -1;
		for (int j = 0; j < ImagenesParaAsignarFotos.Length; j++) 
		{
			if (ImagenesParaAsignarFotos[j].texture.name.Equals(""))
				IndicadorPrevisualizador++;
		}

		if (IndexFotoActiva < IndicadorPrevisualizador) 
		{
			IndexFotoActiva++;
			ImagenPrevisualizar.texture = ImagenesParaAsignarFotos [IndexFotoActiva].texture;
		}

		if (BotonAvanza != null) 
		{
			if (IndexFotoActiva == IndicadorPrevisualizador)
				BotonAvanza.interactable = false;
		}

		if (BotonRetrocede != null) 
		{
			BotonRetrocede.interactable = true;
		}


		if (BotonesVideo [IndexFotoActiva].IsActive ()) 
		{
			ImagenPlayVideo.gameObject.SetActive (true);
			IndiceVideo = -1;

			for (int i = 0; i <= IndexFotoActiva; i++) 
			{
				if (BotonesVideo [i].IsActive ())
					IndiceVideo++;
			}
		}
		else
			ImagenPlayVideo.gameObject.SetActive (false);
	}

	public void VerAnteriorFoto()
	{
		if (IndexFotoActiva > 0) 
		{
			IndexFotoActiva--;
			ImagenPrevisualizar.texture = ImagenesParaAsignarFotos [IndexFotoActiva].texture;
		}

		if (BotonAvanza != null) 
		{
			BotonAvanza.interactable = true;
		}

		if (BotonRetrocede != null) 
		{
			if (IndexFotoActiva == 0)
				BotonRetrocede.interactable = false;
		}

		if (BotonesVideo [IndexFotoActiva].IsActive ()) 
		{
			ImagenPlayVideo.gameObject.SetActive (true);
			IndiceVideo = -1;

			for (int i = 0; i <= IndexFotoActiva; i++) 
			{
				if (BotonesVideo [i].IsActive ())
					IndiceVideo++;
			}
		}
		else
			ImagenPlayVideo.gameObject.SetActive (false);
	}


	 
	/// <summary>
	/// Funcion que toma una foto y devuelve la ruta donde se guardo el archivo
	/// </summary>
	/// <returns>Ruta en cualquier dispositivo donde se almaceno la foto en .png.</returns>
	public string TomarFoto()
	{
		// 


		string RutaCaptura = Path.Combine (Application.persistentDataPath, "Imagen" +
		                     DateTime.Now.Day.ToString () +
		                     DateTime.Now.Month.ToString () +
		                     DateTime.Now.Year.ToString () +
		                     DateTime.Now.Hour.ToString () +
		                     DateTime.Now.Minute.ToString () +
		                     DateTime.Now.Second.ToString () + ".png");

		TexturaPivote = new Texture2D (TexturaWebCam.height, TexturaWebCam.width);


			if (TexturaWebCam.width > TexturaWebCam.height)
				TexturaPivote = RotateMatrix (TexturaWebCam );
			else
				TexturaPivote.SetPixels (TexturaWebCam.GetPixels ());
			TexturaPivote.Apply ();

		File.WriteAllBytes ( RutaCaptura , TexturaPivote.EncodeToJPG() );

			return RutaCaptura;

	}






	public Texture2D RotateMatrix(WebCamTexture Origen) 
	{
		Texture2D Destino = new Texture2D(Origen.height, Origen.width);

		Debug.Log ("EL DISPOSITIVO ESTA en espejo: " + Origen.videoVerticallyMirrored);


		/*
		Color[] ret = new Color[n * n];

		for (int i = 0; i < n; ++i) 
		{
			for (int j = 0; j < n; ++j) 
			{
				ret[i*n + j] = matrix[(n - j - 1) * n + i];
			}
		}*/

		for (int i = Origen.width; i >= 0; i--)
		{
			for (int j = Origen.height; j >= 0; j--) 
			{
				Destino.SetPixel (j, Origen.width - i, Origen.GetPixel (i, j));
			}
		}

		return Destino;
	}



	public void ResetFotos()
	{
		AlmacenadorFotos.Clear ();
		PivoteFotos = 0;
		for (int i = 0; i < ImagenesParaAsignarFotos.Length; i++) 
		{
			ImagenesParaAsignarFotos [i].texture = ((ImagenNula == null) ? null : ImagenNula.texture);
		}




	}



}
