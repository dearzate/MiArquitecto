using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using AndreScripts;

public class Encuesta : MonoBehaviour {

	public ViewsManager MainCanvas;
	public GameObject Content;
	public GameObject OpcionMultiplePrefab;
	public GameObject ComentarioPrefab;

	private EncuestaJ encuesta;
	private LogIn.TokenClass token;
	private List<GameObject> PreguntasRef;

	private const string GetEncuestaURL = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/consulta/encuesta";
	private const string SendEncuestaURL = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/carga/respEncuesta";
	private const string TokenURL = "https://swsp.interceramic.com/ords/mastercontactossecws_sa/genera/token";
	private const string claveApp = "MIARQ";

	[Serializable]
	public class RespuestaJ{
		public int contacto_id;
		public string claveApp;
		public int detencuesta_id;
		public RespuestasArrJ[] respuestas;
	}

	[Serializable]
	public class RespuestasArrJ{
		public int preg_id;
		public int detopcion_id;
		public string comentario;
	}

	[Serializable]
	public class EncuestaJ{
		public int DETENCUESTA_ID;
		public Pregunta[] PREGUNTAS;
	}

	[Serializable]
	public class Pregunta{
		public int PREG_ID;
		public string PREGUNTA;
		public string TIPO;
		public Respuesta[] RESPUESTAS;
	}

	[Serializable]
	public class Respuesta{
		public int DETOPCION_ID;
		public string RESPUESTA;
	}

	public void ResetEncuesta (){
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in Content.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));
	}

	public void GetEncuesta (){
		StartCoroutine (corout_GetToken ());
	}

	public void SendEncuesta (){
		StartCoroutine (corout_SendToken ());
	}

	private IEnumerator corout_GetEncuesta (){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW (GetEncuestaURL, null, headers);
		yield return www;

		Andre.Log ("Encuesta resp: " + www.text);

		encuesta = JsonUtility.FromJson<EncuestaJ> (www.text);
		PreguntasRef = new List<GameObject> ();

		foreach (Pregunta p in encuesta.PREGUNTAS){
			switch (p.TIPO){
				case "CO":{
					GameObject pregunta = GameObject.Instantiate (ComentarioPrefab);
					pregunta.GetComponent<Comentario> ().Pregunta.text = p.PREGUNTA;
					pregunta.GetComponent<Comentario> ().Id = p.PREG_ID;
					pregunta.GetComponent<Comentario> ().detopcion_id = p.RESPUESTAS[0].DETOPCION_ID;
					pregunta.transform.SetParent (Content.transform, false);

					PreguntasRef.Add (pregunta);
					break;
				}	
				case "OM":{
					GameObject pregunta = GameObject.Instantiate (OpcionMultiplePrefab);
					pregunta.GetComponent<OpcionMultiple> ().Pregunta.text = p.PREGUNTA;
					pregunta.GetComponent<OpcionMultiple> ().Id = p.PREG_ID;

					int i=0;
					foreach (Respuesta r in p.RESPUESTAS) {
						pregunta.GetComponent<OpcionMultiple> ().RespsIds [i++] = r.DETOPCION_ID;
					}
					pregunta.transform.SetParent (Content.transform, false);

					PreguntasRef.Add (pregunta);
					break;
				}				
			}
		}

		MainCanvas.hidePopUp (2);
		MainCanvas.popUps [2].GetComponent<LoadingManager> ().stopSpinning ();
	}

	private IEnumerator corout_GetToken (){
		MainCanvas.showPopUpNoAnim (2);
		MainCanvas.popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW (TokenURL, null ,headers);
		yield return www;
		token = JsonUtility.FromJson<LogIn.TokenClass> (www.text);
		StartCoroutine (corout_GetEncuesta ());
	}

	private IEnumerator corout_SendToken (){
		MainCanvas.showPopUpNoAnim (2);
		MainCanvas.popUps [2].GetComponent<LoadingManager> ().startSpinning ();

		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Basic U09LT0xBQlM6U09LT0xBQlNQUjBEMTIzIw==");
		headers.Add ("pClaveApp", claveApp);

		WWW www = new WWW (TokenURL, null ,headers);
		yield return www;
		token = JsonUtility.FromJson<LogIn.TokenClass> (www.text);
		StartCoroutine (corout_SendEncuesta ());
	}

	private IEnumerator corout_SendEncuesta (){
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");	
		headers.Add ("Cookie", "Interceramic");
		headers.Add ("Authorization", "Bearer " + token.Token);
		headers.Add ("pClaveApp", claveApp);

		RespuestaJ resp = new RespuestaJ ();
		resp.contacto_id = PlayerPrefs.GetInt("UserId");
		resp.claveApp = claveApp;
		resp.detencuesta_id = encuesta.DETENCUESTA_ID;
		RespuestasArrJ[] respuestas = new RespuestasArrJ[PreguntasRef.Count];
		for (int i=0; i<PreguntasRef.Count; i++){
			respuestas[i] = new RespuestasArrJ ();
			if (PreguntasRef[i].GetComponent<Comentario> () != null){
				respuestas[i].preg_id = PreguntasRef[i].GetComponent<Comentario> ().Id;
				respuestas[i].detopcion_id = PreguntasRef[i].GetComponent<Comentario> ().detopcion_id;
				respuestas[i].comentario = PreguntasRef[i].GetComponent<Comentario> ().comentario;
			}
			else {
				respuestas[i].preg_id = PreguntasRef[i].GetComponent<OpcionMultiple> ().Id;
				respuestas[i].detopcion_id = PreguntasRef[i].GetComponent<OpcionMultiple> ().detopcion_id;
			}
		}
		resp.respuestas = respuestas;

		string SData = JsonUtility.ToJson(resp);
		byte[] JData = Encoding.ASCII.GetBytes (SData.ToCharArray());

		WWW www = new WWW (SendEncuestaURL, JData, headers);
		yield return www;

		Andre.Log ("Encuesta resp: " + www.text);

		MainCanvas.hidePopUp (2);
		MainCanvas.popUps [2].GetComponent<LoadingManager> ().stopSpinning ();

		MainCanvas.GetComponent<LogIn> ().logInNotSuccess ("Agradecemos tus comentarios");
		MainCanvas.GetComponent<ViewsManager> ().esconderEncuesta ();
	}

}