using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Productos : MonoBehaviour {

	public GameObject fotografias;

	public GameObject imagenEspacio;
	public GameObject imagenEstilo;
	public GameObject imagenTipo;
	public GameObject imagenTamano;

	public Sprite[] espacios;
	public Sprite[] estilos;
	public Sprite[] tipos;
	public Sprite[] tamanos;
	public Sprite[] pisoMuro;

	public GameObject TipoScrollContent;
	public GameObject TamanoScrollContent;
	public GameObject PisoMuroScrollContent;
	public GameObject ResumenCell;

	private List<GameObject> contentScrollTipo;
	private List<GameObject> contentScrollTamano;
	private List<GameObject> contentScrollPisoMuro;
	private Color initColor;

	public void goToTipo(int index){
		transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goToIndexNoMove (6);
		fotografias.GetComponent<ScriptFotosNativo> ().ReescaleImages ();
		fotografias.GetComponent<ScriptFotosNativo> ().IniciarCamara ();
	}

	public void back(){
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething) {
			transform.GetComponent<ViewsManager> ().views [0].GetComponent<View> ().goBack (5);
		}
	}

	public void Start(){
		contentScrollTipo = new List<GameObject> ();
		contentScrollTamano = new List<GameObject> ();
		contentScrollPisoMuro = new List<GameObject> ();
	}

	public void deleteTamanoContent (){
		foreach (GameObject go in contentScrollTamano){
			Destroy (go);
		}
		contentScrollTamano.Clear ();
	}

	public void deleteTipoContent (){
		foreach (GameObject go in contentScrollTipo) {
			Destroy (go);
		}
		contentScrollTipo.Clear ();
	}

	public void deletePisoMuroContent (){
		foreach (GameObject go in contentScrollPisoMuro) {
			Destroy (go);
		}
		contentScrollPisoMuro.Clear ();
	}

	public void addCellToPisoMuro (bool p, bool m){
		if (p){
			GameObject cell = (GameObject)Instantiate (ResumenCell);
			cell.transform.SetParent (PisoMuroScrollContent.transform, false);
			cell.transform.localScale = Vector3.one;
			contentScrollPisoMuro.Add (cell);
			cell.GetComponent<Image> ().sprite = pisoMuro [0];
			cell.GetComponentInChildren<Text> ().text = "Piso";
		}

		if (m){
			GameObject cell = (GameObject)Instantiate (ResumenCell);
			cell.transform.SetParent (PisoMuroScrollContent.transform, false);
			cell.transform.localScale = Vector3.one;
			contentScrollPisoMuro.Add (cell);
			cell.GetComponent<Image> ().sprite = pisoMuro [1];
			cell.GetComponentInChildren<Text> ().text = "Muro";
		}

	}

	public void addCellToTamano(int index){
		GameObject cell = (GameObject)Instantiate (ResumenCell);
		contentScrollTamano.Add (cell);
		contentScrollTamano [contentScrollTamano.Count - 1].transform.parent = TamanoScrollContent.transform;
		contentScrollTamano [contentScrollTamano.Count - 1].transform.localScale = Vector3.one;

		switch (index) {
		case 0:
			cell.GetComponent<Image> ().sprite = tamanos [0];
			cell.GetComponentInChildren<Text> ().text = "Extra";
			break;
		case 1:
			cell.GetComponent<Image> ().sprite = tamanos [1];
			cell.GetComponentInChildren<Text> ().text = "Grande";
			break;
		case 2:
			cell.GetComponent<Image> ().sprite = tamanos [2];
			cell.GetComponentInChildren<Text> ().text = "Mediano";
			break;
		case 3:
			cell.GetComponent<Image> ().sprite = tamanos [3];
			cell.GetComponentInChildren<Text> ().text = "Chico";
			break;
		case 4:
			cell.GetComponent<Image> ().sprite = tamanos [4];
			cell.GetComponentInChildren<Text> ().text = "Tablón";
			break;
		}
	}

	public void addCellToTipo(int index){
		GameObject cell = (GameObject)Instantiate(ResumenCell);
		contentScrollTipo.Add (cell);
		contentScrollTipo[contentScrollTipo.Count - 1].transform.parent = TipoScrollContent.transform;
		contentScrollTipo[contentScrollTipo.Count - 1].transform.localScale = Vector3.one;

		switch (index) {
		case 0:
			cell.GetComponent<Image> ().sprite = tipos [0];
			cell.GetComponentInChildren<Text> ().text = "Cemento";
			break;
		case 1:
			cell.GetComponent<Image> ().sprite = tipos [1];
			cell.GetComponentInChildren<Text> ().text = "Decorado";
			break;
		case 2:
			cell.GetComponent<Image> ().sprite = tipos [2];
			cell.GetComponentInChildren<Text> ().text = "Granito";
			break;
		case 3:
			cell.GetComponent<Image> ().sprite = tipos [3];
			cell.GetComponentInChildren<Text> ().text = "Madera";
			break;
		case 4:
			cell.GetComponent<Image> ().sprite = tipos [4];
			cell.GetComponentInChildren<Text> ().text = "Mármol";
			break;
		case 5:
			cell.GetComponent<Image> ().sprite = tipos [5];
			cell.GetComponentInChildren<Text> ().text = "Metal";
			break;
		case 6:
			cell.GetComponent<Image> ().sprite = tipos [6];
			cell.GetComponentInChildren<Text> ().text = "Piedra";
			break;
		case 7:
			cell.GetComponent<Image> ().sprite = tipos [7];
			cell.GetComponentInChildren<Text> ().text = "Solido";
			break;
		case 8:
			cell.GetComponent<Image> ().sprite = tipos [8];
			cell.GetComponentInChildren<Text> ().text = "Textil";
			break;
		case 9:
			cell.GetComponent<Image> ().sprite = tipos [9];
			cell.GetComponentInChildren<Text> ().text = "Vidrio";
			break;
		}
	}

	public void changeImagenAndTextEspacio(CuestionarioData.Espacios data){
		switch (data) {
			case CuestionarioData.Espacios.Baños: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [0];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Baño";
				break;
			}
			case CuestionarioData.Espacios.Cocinas: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [1];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Cocina";
				break;
			}
			case CuestionarioData.Espacios.Comedores: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [2];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Comedor";
				break;
			}
			case CuestionarioData.Espacios.Comerciales: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [3];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Comercial";
				break;
			}
			case CuestionarioData.Espacios.Exteriores: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [4];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Exterior";
				break;
			}
			case CuestionarioData.Espacios.Pasillos: {
				imagenEspacio.GetComponent<Image> ().sprite = espacios [5];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Pasillo";
				break;
			}
			case CuestionarioData.Espacios.Recamaras:{
				imagenEspacio.GetComponent<Image> ().sprite = espacios [6];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Recamara";
				break;
			}
			case CuestionarioData.Espacios.Salas:{
				imagenEspacio.GetComponent<Image> ().sprite = espacios [7];
				imagenEspacio.GetComponentInChildren<Text> ().text = "Sala";
				break;
			}
		}
	}

	public void changeImagenAndTextEstilo(CuestionarioData.Estilos data){
		switch (data) {
			
			case CuestionarioData.Estilos.Clasico:{
				imagenEstilo.GetComponent<Image> ().sprite = estilos [0];
				imagenEstilo.GetComponentInChildren<Text> ().text = "Clásico";
				break;
			}
			case CuestionarioData.Estilos.Contemporaneo:{
				imagenEstilo.GetComponent<Image> ().sprite = estilos [1];
				imagenEstilo.GetComponentInChildren<Text> ().text = "Contemporáneo";
				break;
			}
			case CuestionarioData.Estilos.Minimalista:{
				imagenEstilo.GetComponent<Image> ().sprite = estilos [2];
				imagenEstilo.GetComponentInChildren<Text> ().text = "Minimalista";
				break;
			}
			case CuestionarioData.Estilos.Rustico:{
				imagenEstilo.GetComponent<Image> ().sprite = estilos [3];
				imagenEstilo.GetComponentInChildren<Text> ().text = "Rústico";
				break;
			}
			case CuestionarioData.Estilos.Vintage:{
				imagenEstilo.GetComponent<Image> ().sprite = estilos [4];
				imagenEstilo.GetComponentInChildren<Text> ().text = "Vintage";
				break;
			}

		}
	}

	public void changeImagenAndTextTipo(CuestionarioData.Tipo data){
		switch (data) {
			case CuestionarioData.Tipo.Cemento:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [0];
				imagenTipo.GetComponentInChildren<Text> ().text = "Cemento";
				break;
			}
			case CuestionarioData.Tipo.Decorado:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [1];
				imagenTipo.GetComponentInChildren<Text> ().text = "Decorado";
				break;
			}
			case CuestionarioData.Tipo.Granito:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [2];
				imagenTipo.GetComponentInChildren<Text> ().text = "Granito";
				break;
			}
			case CuestionarioData.Tipo.Madera:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [3];
				imagenTipo.GetComponentInChildren<Text> ().text = "Madera";
				break;
			}
			case CuestionarioData.Tipo.Marmol:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [4];
				imagenTipo.GetComponentInChildren<Text> ().text = "Mármol";
				break;
			}
			case CuestionarioData.Tipo.Metal:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [5];
				imagenTipo.GetComponentInChildren<Text> ().text = "Metal";
				break;
			}
			case CuestionarioData.Tipo.Piedra:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [6];
				imagenTipo.GetComponentInChildren<Text> ().text = "Piedra";
				break;
			}
			case CuestionarioData.Tipo.Solido:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [7];
				imagenTipo.GetComponentInChildren<Text> ().text = "Solido";
				break;
			}
			case CuestionarioData.Tipo.Textil:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [8];
				imagenTipo.GetComponentInChildren<Text> ().text = "Textil";
				break;
			}
			case CuestionarioData.Tipo.Vidrio:{
				imagenTipo.GetComponent<Image> ().sprite = tipos [9];
				imagenTipo.GetComponentInChildren<Text> ().text = "Vidrio";
				break;
			}
		}
	}

	public void changeImagenAndTextTamano(CuestionarioData.Tamano data){
		switch (data) {
			case CuestionarioData.Tamano.Chico:{
				imagenTamano.GetComponent<Image> ().sprite = tamanos [0];
				imagenTamano.GetComponentInChildren<Text> ().text = "Chico";
				break;
			}
			case CuestionarioData.Tamano.Extra:{
				imagenTamano.GetComponent<Image> ().sprite = tamanos [1];
				imagenTamano.GetComponentInChildren<Text> ().text = "Extra";
				break;
			}
			case CuestionarioData.Tamano.Grande:{
				imagenTamano.GetComponent<Image> ().sprite = tamanos [2];
				imagenTamano.GetComponentInChildren<Text> ().text = "Grande";
				break;
			}
			case CuestionarioData.Tamano.Mediano:{
				imagenTamano.GetComponent<Image> ().sprite = tamanos [3];
				imagenTamano.GetComponentInChildren<Text> ().text = "Mediano";
				break;
			}
			case CuestionarioData.Tamano.Tablon:{
				imagenTamano.GetComponent<Image> ().sprite = tamanos [4];
				imagenTamano.GetComponentInChildren<Text> ().text = "Tablón";
				break;
			}
		}
	}

}
