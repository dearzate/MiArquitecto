using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tamano : MonoBehaviour {

	public GameObject[] TamanoBotones;
	public Text SiguienteButtonText;

	private int total = 5;
	private Color initColor;
	private int cont = 0;

	void Start (){
		initColor = SiguienteButtonText.color;
	}

	public void toggle(int index){

		if (cont < 3 && transform.GetComponent<CuestionarioData> ().respData.respTamano [index]){
			transform.GetComponent<CuestionarioData> ().respData.respTamano [index] = false;
			cont--;
		}
		else if (cont < 3) {
			cont++;
			transform.GetComponent<CuestionarioData> ().respData.respTamano [index] = true;
		}
		else if (cont == 3 && transform.GetComponent<CuestionarioData> ().respData.respTamano [index]){
			transform.GetComponent<CuestionarioData> ().respData.respTamano [index] = false;
			cont--;
		}

//		transform.GetComponent<CuestionarioData> ().respData.respTamano [index] = !transform.GetComponent<CuestionarioData> ().respData.respTamano [index];
		if (transform.GetComponent<CuestionarioData> ().respData.respTamano [index]) {
			SiguienteButtonText.color = initColor;
			SiguienteButtonText.text = "Siguiente";
			TamanoBotones [index].GetComponent<Image> ().color = new Color(0.56f, 0.56f, 0.56f, 0.5f);
		}
		else {
			TamanoBotones [index].GetComponent<Image> ().color = Color.white;
		}
	}

	public void toggleAll(){
		for (int i =0; i<total; i++){
			TamanoBotones [i].GetComponent<Image> ().color = Color.white;
			transform.GetComponent<CuestionarioData> ().respData.respTamano [i] = false;
		}
//		GameObject.Find ("TamanoScrollView").GetComponent<ScrollRect> ().verticalNormalizedPosition = 1;
		Vector3 pos = GameObject.Find ("TamanoContent").GetComponent<RectTransform> ().position;
		pos = new Vector3 (pos.x, 495, pos.z);
		GameObject.Find ("TamanoContent").GetComponent<RectTransform> ().position = pos;
	}

	public void restartCont(){
		cont = 0;
	}

	//Ya no es captura
	public void goToCaptura(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			if (transform.GetComponent<CuestionarioData>().AtLeastOneTamanoIsSelected()) {
				transform.GetComponent<Productos> ().deleteTipoContent ();
				for (int i=0; i<10; i++) {
					if (transform.GetComponent<CuestionarioData> ().respData.respTipo [i]) {
						transform.GetComponent<Productos> ().addCellToTipo (i);
					}
				}

				transform.GetComponent<Productos> ().deleteTamanoContent ();
				for (int i=0; i<5; i++) {
					if (transform.GetComponent<CuestionarioData> ().respData.respTamano [i]) {
						transform.GetComponent<Productos> ().addCellToTamano (i);
					}
				}

				print ("Captura index " + index);
				transform.GetComponent<ViewsManager> ().views[0].GetComponent<View>().goToIndexNoMove (4);
			}
			else {
				SiguienteButtonText.color = Color.red;
				SiguienteButtonText.text = "Selecciona una opción";
			}
		}
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (3);
		}
	}
}
