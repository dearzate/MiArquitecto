using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class View : MonoBehaviour {

	public bool flagToShow;
	public int mainView = 0;
	public GameObject[] childrenViews;
	public int activeView = 0;
	public int lastView = 0;

	[HideInInspector]
	public bool isMovingSomething = false;

	private static float speed = 2f;

	private int sWidth;
	private bool camaraChecked = false;

	public void Update(){
		
	}

	public void setCamaraChecked(bool b){
		camaraChecked = b;
	}

	public void goToIndexNoMove(int index){
		childrenViews [index].SetActive (true);
		StartCoroutine (moveThisCenter (index));
		activeView = index;
	}

	public void goToIndex(int index, string animation = "izquierda") {
		for (int i = 0; i < childrenViews.Length; i++) {
			if (i != index && animation == "derecha") {
				StartCoroutine (moveThisRight (i));
			} 
			else if (i != index && animation == "izquierda") {
				StartCoroutine (moveThisLeft (i));
			} 
			else {
				StartCoroutine (moveThisCenter (index));
			}
		}
		activeView = index;
	}

	public void goBack(int index){
		childrenViews [index - 1].SetActive (true);
		StartCoroutine (moveThisRight (index));
	}

	public void deactivateOthers(int index){
		if (camaraChecked) {
//			if (index == 0) {
//				for (int i=2; i<childrenViews.Length; i++) {
//					childrenViews [i].SetActive (false);
//				}
//				childrenViews [0].SetActive (true);
//				childrenViews [1].SetActive (true);
//			}
//			else if (index == childrenViews.Length - 1) {
//				for (int i=0; i<childrenViews.Length - 2; i++) {
//					childrenViews [i].SetActive (false);
//				}
//				childrenViews [5].SetActive (true);
//				childrenViews [6].SetActive (true);
//			}
//			else {
//				for (int i=0; i<childrenViews.Length; i++) {
//					if (i == index - 1 || i == index || i == index + 1) {
//						childrenViews [i].SetActive (true);
//					}
//					else {
//						childrenViews [i].SetActive (false);
//					}
//				}
//			}
			for (int i=0; i<childrenViews.Length; i++) {
				if (i != index) {
					childrenViews [i].SetActive (false);
				} else {
					childrenViews [index].SetActive (true);
				}
			}
		}
	}

	public void goToIndexNoAnim(int index, string animation = "izquierda") {
		for (int i = 0; i < childrenViews.Length; i++) {
			if (i != index && animation == "derecha")
				childrenViews [i].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			else if (i != index && animation == "izquierda")
				childrenViews [i].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
			else
				childrenViews [index].transform.localPosition = new Vector3 (0, 0, 0);
		}
		activeView = index;
	}

	public void hideAll(string animation = "izquierda") {
		int i = 0;
		foreach (GameObject view in childrenViews) {
			if (animation == "izquierda")
				StartCoroutine (moveThisLeft (i));
			else if (animation == "derecha")
				StartCoroutine (moveThisRight (i));
			i++;
		}
		lastView = activeView;
	}

	public void hideAllNoAnim(string animation =  "izquierda"){
		int i = 0;
		foreach (GameObject view in childrenViews) {
			if (animation == "izquierda")
				childrenViews [i].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
			else if (animation == "derecha")
				childrenViews [i].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
			i++;
		}
		lastView = activeView;
	}

	public void setActiveAll(bool active){
		foreach (GameObject view in childrenViews) {
			view.SetActive (active);
		}
	}

	public void hideAllJustAnimThis(int index, string animation = "izquierda"){
		int i = 0;
		foreach (GameObject view in childrenViews) {
			if (animation == "izquierda") {
				if (i == index) {
					StartCoroutine (moveThisLeft(index));
				}
				else {
					childrenViews [i].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
				}
			} 
			else if (animation == "derecha") {
				if (i == index) {
					StartCoroutine (moveThisRight(index));
				}
				else {
					childrenViews [i].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
				}
			}
			i++;
		}
		lastView = activeView;
	}

	public void Awake() {
		sWidth = Screen.width;
		foreach (GameObject view in childrenViews) {
			view.transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		}
	}

	private IEnumerator moveThisRight (int index) {
		#if UNITY_ANDROID
		childrenViews [index].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
		yield return null;
		#else
		isMovingSomething = true;
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (childrenViews [index].transform.localPosition.x > sWidth * 1.9f) {
				childrenViews [index].transform.localPosition = new Vector3 (sWidth * 2, 0, 0);
				elapsedTime = speed;
			}
			else {
				childrenViews [index].transform.localPosition = Vector3.Lerp (childrenViews [index].transform.localPosition, new Vector3 (sWidth * 2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
//			print (childrenViews [index].transform.localPosition.x);
			yield return null;
		}
		#endif
		deactivateOthers (index - 1);
		isMovingSomething = false;
	}

	private IEnumerator moveThisLeft (int index) {
		#if UNITY_ANDROID
		childrenViews [index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
		yield return null;
		#else
		isMovingSomething = true;
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
			if (childrenViews [index].transform.localPosition.x < sWidth *-1.9f) {
				childrenViews [index].transform.localPosition = new Vector3 (sWidth *-2, 0, 0);
				elapsedTime = speed;
			}
			else {
				childrenViews [index].transform.localPosition = Vector3.Lerp (childrenViews [index].transform.localPosition, new Vector3 (sWidth *-2, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
		isMovingSomething = false;
	}

	private IEnumerator moveThisCenter (int index) {
		#if UNITY_ANDROID
		childrenViews [index].transform.localPosition = new Vector3 (0, 0, 0);
		yield return null;
		#else
		isMovingSomething = true;
		float elapsedTime = 0f;
		while (elapsedTime < speed) {
//			print (childrenViews [index].transform.localPosition.x);
			if (childrenViews [index].transform.localPosition.x < 0.1) {
				childrenViews [index].transform.localPosition = new Vector3 (0, 0, 0);
				elapsedTime = speed;
			}
			else {
				childrenViews [index].transform.localPosition = Vector3.Lerp (childrenViews [index].transform.localPosition, new Vector3 (0, 0, 0), elapsedTime/speed);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		#endif
		deactivateOthers (index);
		isMovingSomething = false;
	}
}
