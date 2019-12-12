using UnityEngine;
using System.Collections;

public class Capturas : MonoBehaviour {

	public GameObject fotografias;

	public void goToWizi(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			fotografias.GetComponent<ScriptFotosNativo> ().DetenerCamara ();
			transform.GetComponent<ViewsManager> ().views[0].GetComponent<View>().goToIndexNoMove (7);
		}
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (6);
			fotografias.GetComponent<ScriptFotosNativo> ().DetenerCamara ();
		}
	}
}
