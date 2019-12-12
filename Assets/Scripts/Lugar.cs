using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Lugar : MonoBehaviour {

	public Toggle Piso;
	public Toggle Muro;

	public Text Warning;

	private Color initColor;

	public void goToResumen(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			if (Piso.isOn && Muro.isOn) {
				transform.GetComponent<CuestionarioData> ().respData.respProducto = (CuestionarioData.Producto)3;
				transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (5);
				transform.GetComponent<Productos> ().deletePisoMuroContent ();
				transform.GetComponent<Productos> ().addCellToPisoMuro (true, true);
			}
			else if (Piso.isOn) {
				transform.GetComponent<CuestionarioData> ().respData.respProducto = (CuestionarioData.Producto)1;
				transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (5);
				transform.GetComponent<Productos> ().deletePisoMuroContent ();
				transform.GetComponent<Productos> ().addCellToPisoMuro (true, false);
			}
			else if (Muro.isOn) {
				transform.GetComponent<CuestionarioData> ().respData.respProducto = (CuestionarioData.Producto)2;
				transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (5);
				transform.GetComponent<Productos> ().deletePisoMuroContent ();
				transform.GetComponent<Productos> ().addCellToPisoMuro (false, true);
			}
			else {
				Warning.color = Color.red;
				Warning.text = "Selecciona una opción";
			}
		}
	}

	public void toggleAll(){
		Muro.isOn = false;
		Piso.isOn = false;
	}

	public void Update(){
		if (Warning.IsActive() && (Piso.isOn || Muro.isOn)) {
			Warning.color = initColor;
			Warning.text = "Siguiente";
		}
	}

	public void Start(){
		initColor = Warning.color;
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (4);
		}
	}
	
}
