using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ListaElementos
{
	public int Entero;
	public float Flotante;
	public string Cadena;
	public InputField CadenaEntrada;
	public Text TextoFijo;
	public Toggle Togleador;

	public enum FormasComparacion
	{
		Int,
		Float,
		String,
		InputField,
		TextField,
		Toggle
	};

	public FormasComparacion TiposComparacion;

	public string NombreCampo;


}


// A Partir de aqui editar segun JSON
public class Informacion
{
	
}








