using UnityEngine;
using System.Collections;

public class Estilos : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void goToProducto(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			print ("Producto index " + index);
			transform.GetComponent<CuestionarioData> ().respData.respEstilos = (CuestionarioData.Estilos)index + 1;
			transform.GetComponent<ViewsManager> ().views[0].GetComponent<View>().goToIndexNoMove (2);
			transform.GetComponent<Productos> ().changeImagenAndTextEstilo (transform.GetComponent<CuestionarioData> ().respData.respEstilos);
		}
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) 
        {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (1);
		}
	}
}
