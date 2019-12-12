using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchemeController : MonoBehaviour {

	public WebPayView webpayview;
	static string BaseString = "miarqui://";

	public void OnOpenWithUrl (string url){

		if (url.Contains (BaseString)){
			print ("FULL SCHEME: " + url);
			Dictionary<string, string> parsedScheme = ParseScheme (url);
			print ("SCHEME: " + parsedScheme ["respuesta"]);
			webpayview.ReceivedMessage (parsedScheme ["respuesta"]);
		}
		else {
			print ("OTHER SCHEME");
		}
	}

	Dictionary<string, string> ParseScheme (string scheme){

		scheme = scheme.Replace (BaseString, "");
		string[] parameters = scheme.Split ('&');

		Dictionary<string, string> dict = new Dictionary<string, string> ();

		foreach (string s in parameters){
			string[] keyValue = s.Split ('=');
			dict.Add (keyValue[0], keyValue[1]);
		}

		return dict;
	}

}
