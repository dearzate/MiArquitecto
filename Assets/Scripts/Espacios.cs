using UnityEngine;
using System.Collections;

public class Espacios : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void goToEstilo(int index){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			print ("Estilo index " + index);
			transform.GetComponent<CuestionarioData> ().respData.respEspacios = (CuestionarioData.Espacios)index + 1;
			transform.GetComponent<ViewsManager> ().views[0].GetComponent<View>().goToIndexNoMove (1);
			transform.GetComponent<Productos> ().changeImagenAndTextEspacio (transform.GetComponent<CuestionarioData> ().respData.respEspacios);
		}

	}

	public void goToCompras(int index)
	{
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething)
		{
			// Petition for pdf list
			GetComponent<AmazonRequest>().type = InterceramicPetitionType.GET_BOUGHT_LIST;
			GetComponent<AmazonRequest>().getBoughtList("" + PlayerPrefs.GetInt("UserId"));
			transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().goToIndexNoMove(9);
		}
	}
}
