using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Comentario : MonoBehaviour {

	public Text Pregunta;
	public InputField ComentarioField;
	public int Id;

	public int detopcion_id;
	public string comentario;

	public void OnInputFieldChanged (){
		comentario = ComentarioField.text;
	}
	
}
