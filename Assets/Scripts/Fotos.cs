using UnityEngine;
using System.Collections;

public class Fotos : MonoBehaviour {

	public string pathFoto;
	public int index;

	public void click(){
		print ("Click");
	}

	public void removeFromParent(){
		transform.SetParent (null);
	}

}
