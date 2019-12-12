using UnityEngine;
using System.Collections;

public class FirstTimeRunning : MonoBehaviour {

	public bool FirsTimeTests = false;

	// Use this for initialization
	void Start () {
		if (FirsTimeTests)
			PlayerPrefs.SetInt( "HasPlayed", 0);
		
		int hasPlayed = PlayerPrefs.GetInt( "HasPlayed");

		if( hasPlayed == 0 ){
			print ("First Time");

			PlayerPrefs.SetInt( "HasPlayed", 1);

			gameObject.GetComponent<ViewsManager> ().verTutorial ();

//			transform.GetComponent<GoogleAnalytics> ().estadisticaDescargas ();
//			transform.GetComponent<GoogleAnalytics> ().estadisticaDispositivo ();
		}
		else{
			print ("Not First Time");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
