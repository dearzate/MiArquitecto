using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {

	public Image imagen;
	public Image background;
	bool spin = false;
	float maxTime = 25.0f;

	public void startSpinning(){
		spin = true;
	}

	public void stopSpinning(){
		spin = false;
	}

	public void Update(){
		if (spin) {
			maxTime -= Time.deltaTime;
//			if (maxTime <= 0.0f) {
//				spin = false;
//				maxTime = 25.0f;
//				transform.GetComponentInParent<ViewsManager> ().hidePopUpNoAnim (2);
//				transform.GetComponentInParent<LogIn> ().logInNotSuccess ("Tiempo de conexión agotado");
//				transform.GetComponentInParent<LogIn> ().stopPost ();
//				transform.GetComponentInParent<Registrar> ().stopPost ();
//			}
			imagen.transform.Rotate (0, 0, Time.deltaTime*-100);
		}
		else {
			imagen.transform.rotation = new Quaternion (0, 0, 0, 1);
		}
	}

}
