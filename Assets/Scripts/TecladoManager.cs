using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AndreScripts;

public class TecladoManager : MonoBehaviour {
	public float keyboardOffset = 10f;

	private EventSystem currentEventSystem;
	private bool viewWasResized = false;
	private Vector3 lastPPos;
	private InputField lastIF;
	private static float speed = 0.8f;

	void Start (){
		currentEventSystem = EventSystem.current;
		#if UNITY_ANDROID
		keyboardOffset += 120f;
		#endif
	}

	void Update (){
		int KeyboardSize = Andre.GetKeyboardSize ();
		if (currentEventSystem.currentSelectedGameObject != null && KeyboardSize != 0){
			// print ("InputField Name: " + currentEventSystem.currentSelectedGameObject.name + " keyboard: " + Andre.GetKeyboardSize ());
			if (currentEventSystem.currentSelectedGameObject.GetComponent<InputField> () != null && !viewWasResized){
				InputField iF = currentEventSystem.currentSelectedGameObject.GetComponent<InputField> ();
				RectTransform rT = iF.GetComponent<RectTransform> ();

				float iFBasePoint = iF.transform.position.y - (rT.rect.size.y / 2) - keyboardOffset;
				Andre.Log ("Offset: " + iFBasePoint);

				if (iFBasePoint < KeyboardSize){
					float offset = KeyboardSize - iFBasePoint + keyboardOffset;
					// Debug.Log ("Should resize view");
					viewWasResized = true;
					lastIF = iF;

					ScrollRect[] scrolls = FindObjectsOfType (typeof (ScrollRect)) as ScrollRect[]; 
					foreach (ScrollRect s in scrolls){
						s.enabled = false;
					}

					Transform p = iF.transform.parent;
					lastPPos = p.position;
					StartCoroutine (moveTo (p, new Vector3 (p.position.x, p.position.y + offset, p.position.z)));
				}
				else if (KeyboardSize == 0){
					// Debug.Log("Not visible but keyboard not shown");
					Transform p = lastIF.transform.parent;
					p.position = lastPPos;
					viewWasResized = false;
				}
				else {
					// Debug.Log ("Input field is visible");
				}
			}
		}
		else if (viewWasResized){
			ScrollRect[] scrolls = FindObjectsOfType (typeof (ScrollRect)) as ScrollRect[]; 
			foreach (ScrollRect s in scrolls){
				s.enabled = true;
			}
			Transform p = lastIF.transform.parent;
			StartCoroutine (moveTo (p, lastPPos));
			viewWasResized = false;
		}
	}

	IEnumerator moveTo (Transform targetTrans, Vector3 targetPos){
		#if UNITY_ANDROID
		targetTrans.transform.position = targetPos;
		yield return null;
		#else
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (elapsedTime > speed * 0.9f) {
				targetTrans.transform.position = targetPos;
				elapsedTime = speed;
			}
			else {
				targetTrans.transform.position = Vector3.Lerp (targetTrans.transform.position, targetPos, elapsedTime);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
	}

}
