using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExpandView : MonoBehaviour {

	public ScrollRect _scrollRect;
	public Image pageStateImage;
	public Text descripcion;
	public Text titulo;
	public Image[] scrollImages;

	public Sprite[] pageState;
	public string[] Descripciones;
	public Sprite[] Imagenes;
	public string[] titulos;

	private bool hasTouched = false;
	private float speed = 2f;
	private ViewsManager vm;

	private Vector2 touchStart, touchEnd;
	private int currentPage = 0;
	private Touch lastTouch;

	public void Start(){
		vm = GameObject.Find ("MainCanvas").GetComponent<ViewsManager>();
	}

	public void Update(){
		if (Input.touchCount > 0 && !hasTouched){
			touchStart = Input.touches [0].position;
		}

		if (Input.touchCount > 0 && vm.getExpandIsShown ()) {
			hasTouched = true;
			lastTouch = Input.touches [0];
			print ("scrollRect: " + _scrollRect.horizontalNormalizedPosition);
		}
		else if (hasTouched && Input.touchCount == 0 && vm.getExpandIsShown ()) {
			touchEnd = lastTouch.position;
			float deltaX = touchStart.x - touchEnd.x;
			print ("deltaX antes: " + deltaX);
			deltaX = deltaX / Screen.width;
			print ("deltaX: " + deltaX);

			float hNP = _scrollRect.horizontalNormalizedPosition;

			if (deltaX < -0.2f && currentPage != 0) {
				currentPage--;
			}
			else if (deltaX > 0.2f && currentPage != Imagenes.Length - 1) {
				currentPage++;
			}

			updatePage (currentPage); 
		}
	}

	private void updatePage(int n){
		pageStateImage.sprite = pageState [n];
		hasTouched = false;
		StartCoroutine (animScroll (n));
	}

	public void changeImagesAndText(int n){
		if (Descripciones [n].Length > 0) {
			descripcion.gameObject.SetActive (true);
			descripcion.text = Descripciones [n];
		}
		else {
			descripcion.gameObject.SetActive (false);
		}

		titulo.text = titulos [n];
		scrollImages [0].sprite = Imagenes [n * 3];
		scrollImages [1].sprite = Imagenes [n * 3 + 1];
		scrollImages [2].sprite = Imagenes [n * 3 + 2];
	}

	public void resetPos(){
		currentPage = 0;
		pageStateImage.sprite = pageState [0];
		_scrollRect.horizontalNormalizedPosition = 0;
	}

	private IEnumerator animScroll(int n){
		float fHorPos = n/2f;
		int dir = (_scrollRect.horizontalNormalizedPosition > fHorPos) ? -1 : 1;
		while (!FastApproximately(_scrollRect.horizontalNormalizedPosition, fHorPos, Time.deltaTime) && !hasTouched) {
			_scrollRect.horizontalNormalizedPosition += dir * Time.deltaTime;
			yield return null;
		}
		_scrollRect.horizontalNormalizedPosition = fHorPos;
	}

	public static bool FastApproximately(float a, float b, float threshold){
		return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
	}
}
