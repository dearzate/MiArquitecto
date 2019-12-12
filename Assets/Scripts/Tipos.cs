using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tipos : MonoBehaviour {
	
	public GameObject[] TipoBotones;
	public Text SiguienteButtonText;

	private int total = 10;
	private Color initColor;
	private int cont = 0;

	void Start (){
		initColor = SiguienteButtonText.color;
	}

	public void toggle(int index){

		if (cont < 3 && transform.GetComponent<CuestionarioData> ().respData.respTipo [index]){
			transform.GetComponent<CuestionarioData> ().respData.respTipo [index] = false;
			cont--;
		}
		else if (cont < 3) {
			cont++;
			transform.GetComponent<CuestionarioData> ().respData.respTipo [index] = true;
		}
		else if (cont == 3 && transform.GetComponent<CuestionarioData> ().respData.respTipo [index]){
			transform.GetComponent<CuestionarioData> ().respData.respTipo [index] = false;
			cont--;
		}

//		transform.GetComponent<CuestionarioData> ().respData.respTipo [index] = !transform.GetComponent<CuestionarioData> ().respData.respTipo [index];
		if (transform.GetComponent<CuestionarioData> ().respData.respTipo [index]) {
			SiguienteButtonText.color = initColor;
			SiguienteButtonText.text = "Siguiente";
			TipoBotones [index].GetComponent<Image> ().color = new Color(0.56f, 0.56f, 0.56f, 0.5f);
		} 
		else {
			TipoBotones [index].GetComponent<Image> ().color = Color.white;
		}
	}

	public void toggleAll(){
		for (int i =0; i<total; i++){
			TipoBotones [i].GetComponent<Image> ().color = Color.white;
			transform.GetComponent<CuestionarioData> ().respData.respTipo [i] = false;
		}
//		GameObject.Find ("TipoScrollView").GetComponent<ScrollRect> ().verticalNormalizedPosition = 1;
		Vector3 pos = GameObject.Find ("TipoContent").GetComponent<RectTransform> ().position;
		pos = new Vector3 (pos.x, 495, pos.z);
		GameObject.Find ("TipoContent").GetComponent<RectTransform> ().position = pos;
	}

	public void restartCont (){
		cont = 0;
	}

	public void goToTextura(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			if (transform.GetComponent<CuestionarioData> ().AtLeastOneTipoIsSelected ()) {
				print ("Textura index " + index);
				transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (3);
				//Actualizar resumen
			}
			else {
				SiguienteButtonText.color = Color.red;
				SiguienteButtonText.text = "Selecciona una opción";
			}
		}
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (2);
		}
	}
}
