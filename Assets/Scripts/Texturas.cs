using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Texturas : MonoBehaviour {
	
	public Toggle Brillante;
	public Toggle Lisa;
	public Toggle Rugosa;
	public Toggle Mate;

	public Text Warning;

	public void goToTamano(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {

			if (Brillante.isOn || Lisa.isOn || Rugosa.isOn || Mate.isOn) {
				transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (6);
			} else {
				Warning.gameObject.SetActive (true);
			}
		}
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (5);
		}
	}

	public void Update(){
		if (Warning.IsActive() && (Brillante.isOn || Lisa.isOn || Rugosa.isOn || Mate.isOn)) {
			Warning.gameObject.SetActive (false);
		}
	}
}
