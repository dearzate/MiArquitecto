using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour {

	public ScrollRect _scrollRect;
	public Text pageStateText;
	public Text descripcion;
	public Text titulo;
	public Image[] scrollImages;

	public string[] Descripciones;
	public Sprite[] Imagenes;
	public string[] titulos;

	private bool hasTouched = false;
	private float speed = 4f;
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

		if (Input.touchCount > 0 && vm.getTutorialIsShown ()) {
			hasTouched = true;
			lastTouch = Input.touches [0];
			print ("scrollRect: " + _scrollRect.horizontalNormalizedPosition);
		}
		else if (hasTouched && Input.touchCount == 0 && vm.getTutorialIsShown ()) {
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
		// pageStateImage.sprite = pageState [n];
		pageStateText.text = n + 1 + " / 13";
		hasTouched = false;
		StartCoroutine (animScroll (n));
	}

	public void changeImagesAndText(){
		for (int i=0; i<scrollImages.Length; i++) {
			scrollImages [i].sprite = Imagenes [i];
		}
	}

	public void resetPos(){
		currentPage = 0;
		// pageStateImage.sprite = pageState [0];
		pageStateText.text = 1 + " / 13";
		_scrollRect.horizontalNormalizedPosition = 0;
	}

	private IEnumerator animScroll(int n){
		float fHorPos = n/12f;
		print (fHorPos);
		int dir = (_scrollRect.horizontalNormalizedPosition > fHorPos) ? -1 : 1;
		while (!FastApproximately(_scrollRect.horizontalNormalizedPosition, fHorPos, Time.deltaTime / speed) && !hasTouched) {
			_scrollRect.horizontalNormalizedPosition += dir * Time.deltaTime / speed;
			yield return null;
		}
		_scrollRect.horizontalNormalizedPosition = fHorPos;
	}

	public static bool FastApproximately(float a, float b, float threshold){
		return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
	}
}
