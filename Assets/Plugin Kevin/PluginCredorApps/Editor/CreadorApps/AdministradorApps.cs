using UnityEngine;
using System.Collections;
using UnityEditor;

public class AdministradorApps : EditorWindow 
{

	string myString = "Hello World";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;



	// Add menu named "My Window" to the Window menu
	[MenuItem ("Inmersys/Camara Nativa")]
	static void Init ()
	{
		// Obtiene la ventana de Panel de Control, o crea una nueva
		//AdministradorApps window = (AdministradorApps)EditorWindow.GetWindow (typeof (AdministradorApps));
		Canvas ObjetoCanvas = GameObject.FindObjectOfType<Canvas>(); 
		// Crea el objeto en la escena y lo asigna al canvas
		GameObject ObjetoScroll = Instantiate (Resources.Load("Camara Nativa")) as GameObject;
		ObjetoScroll.name = "PanelScroll";
		if (ObjetoCanvas!=null)
		{
			ObjetoScroll.transform.SetParent(ObjetoCanvas.transform, false);
		}

	}

	[MenuItem ("Inmersys/Panel scrolleable")]
	static void CrearPanelScroll()
	{
		// Busca el objeto Canvas para crear diversos objetos
		Canvas ObjetoCanvas = GameObject.FindObjectOfType<Canvas>(); 
		// Crea el objeto en la escena y lo asigna al canvas
		GameObject ObjetoScroll = Instantiate (Resources.Load("PanelScroll")) as GameObject;
		ObjetoScroll.name = "PanelScroll";
		if (ObjetoCanvas!=null)
		{
			ObjetoScroll.transform.SetParent(ObjetoCanvas.transform, false);
		}


	}
	
	void OnGUI ()
	{
		/*GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		myString = EditorGUILayout.TextField ("Text Field", myString);
		
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();*/
	}


}
