using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ConsultasAPI))]
public class ConsultasAPIEditor : Editor 
{
	// Ruta de la API 
	SerializedProperty RutaAPI;
	// Lista reordenable de elementos para agregar
	ReorderableList ListaCondiciones;

	SerializedProperty ListaCasosExito;
	SerializedProperty ListaCasosVacio;
	SerializedProperty ListaCasosError;

	SerializedProperty EventoBien;
	SerializedProperty EventoNul;
	SerializedProperty EventoMal;

	SerializedProperty ListaValores;
	SerializedProperty ListaCabeceras;


	void OnEnable()
	{
		// Encuentra la propiedad de DireccionAPI
		RutaAPI = serializedObject.FindProperty ("DireccionAPI");
		EventoBien = serializedObject.FindProperty ("EventoExito");
		EventoNul = serializedObject.FindProperty ("EventoVacio");
		EventoMal = serializedObject.FindProperty ("EventoError");

		ListaCasosExito = serializedObject.FindProperty("CasosExito");
		ListaCasosVacio = serializedObject.FindProperty("CasosVacio");
		ListaCasosError = serializedObject.FindProperty("CasosError");

		ListaCabeceras = serializedObject.FindProperty("Cabeceras");
		ListaValores = serializedObject.FindProperty("Valores");


		// Inicia la condicion 
		ListaCondiciones = new ReorderableList ( serializedObject, serializedObject.FindProperty("Elementos"), true, true, true, true );

		// Diseña como se dibuja cada elemento de la lista
		ListaCondiciones.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) => 
		{
			var elemento = ListaCondiciones.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField( new Rect( rect.x, rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "TiposComparacion" ), GUIContent.none );



			ListaElementos.FormasComparacion Tipo = (ListaElementos.FormasComparacion) elemento.FindPropertyRelative( "TiposComparacion" ).enumValueIndex;

			switch(Tipo)
			{
				case ListaElementos.FormasComparacion.Float:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "Flotante" ), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.InputField:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "CadenaEntrada" ), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.Int:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "Entero" ), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.String:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "Cadena" ), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.TextField:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "TextoFijo" ), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.Toggle:
					EditorGUI.PropertyField( new Rect( rect.x + (rect.width/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "Togleador" ), GUIContent.none );
				break;

				default:
				break;
			}

			EditorGUI.PropertyField( new Rect( rect.x + (rect.width*2/3), rect.y, rect.width/3, EditorGUIUtility.singleLineHeight ), elemento.FindPropertyRelative( "NombreCampo" ), GUIContent.none );

		};



	}


	/// <summary>
	/// Raises the inspector GU event.
	/// </summary>
	public override void OnInspectorGUI()
	{
		// Actualiza los valores de la clase ConsultasAPI
		serializedObject.Update ();

		// Pone en el inspector la cadena de DireccionAPI

	    EditorGUILayout.PropertyField ( RutaAPI );

		EditorGUILayout.BeginVertical();
			EditorGUILayout.PropertyField(ListaCabeceras, true);
			EditorGUILayout.PropertyField(ListaValores, true);
		EditorGUILayout.EndVertical();

		// Dibuja la lista de condiciones
		EditorGUILayout.LabelField("Variables para POST");
		ListaCondiciones.DoLayoutList ();

		EditorGUILayout.PropertyField ( ListaCasosExito, true );
		EditorGUILayout.PropertyField ( ListaCasosVacio, true );
		EditorGUILayout.PropertyField ( ListaCasosError, true );

		EditorGUILayout.LabelField ("Eventos al finalizar consulta de API");

		// Coloca los 3 eventos a disparar segun lo devuelto por el API
		EditorGUILayout.PropertyField ( EventoBien );
		EditorGUILayout.PropertyField ( EventoNul );
		EditorGUILayout.PropertyField ( EventoMal );



		// Aplica las propiedades modificadas
		serializedObject.ApplyModifiedProperties ();
	}


}
