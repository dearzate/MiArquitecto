using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RegisterResponse : MonoBehaviour {

	public Text text;
	public Button ok;

	public void okPressed() {
		if (transform.GetComponent<Registrar> ().registerSuccesful) {
			transform.GetComponent<ViewsManager> ().hidePopUp (1);
//			transform.GetComponent<ViewsManager> ().hideAll ("izquierda");
//			transform.GetComponent<ViewsManager> ().views[transform.GetComponent<ViewsManager> ().activeView].GetComponent<View>().goToIndex (transform.GetComponent<ViewsManager> ().views[transform.GetComponent<ViewsManager> ().activeView].GetComponent<View>().mainView);
			transform.GetComponent<ViewsManager> ().goToIndex(0);

		}
		else {
			transform.GetComponent<ViewsManager> ().hidePopUp (1);
		}
	}

	public void changeText(string text) {
		this.text.text = text;
	}

}
