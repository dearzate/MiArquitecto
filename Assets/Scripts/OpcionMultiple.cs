using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OpcionMultiple : MonoBehaviour {

	public Text Pregunta;
    public Button[] Respuestas;
    public int Id;

    public int[] RespsIds = new int[5] {1, 1, 1, 1, 1};

    public int detopcion_id;

    public void answer (int pos){
        foreach (Button b in Respuestas){
            b.GetComponent<Image> ().color = new Color (1, 1, 1, 70/255f);
        }
        Respuestas[pos].GetComponent<Image> ().color = Color.white;
        detopcion_id = pos;
    }

}
