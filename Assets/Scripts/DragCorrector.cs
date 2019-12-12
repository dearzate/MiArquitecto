using UnityEngine;
using UnityEngine.EventSystems;

public class DragCorrector : MonoBehaviour 
{
	public int baseTH = 6;
	public int basePPI = 210;
	public int dragTH = 0;

	void Start(){
		#if UNITY_ANDROID
		dragTH = baseTH * (int)Screen.dpi / basePPI;
		//Debug.Log (Screen.dpi.ToString());

		EventSystem es = GetComponent<EventSystem>();

		if (es) 
			es.pixelDragThreshold = dragTH;
		#endif
	}
}
