using UnityEngine;
using System.Collections;


namespace Tastybits.NativeGallery {

	public class EditorGalleryUI : MonoBehaviour {
		System.Action<Texture2D> callback;

		public void OpenGallery( System.Action<Texture2D> callback ){
			this.callback = callback;	
			this.gameObject.SetActive(true);
		}
		public void OnGalleryItemClicked( GameObject go ) {
			if( this.callback!=null ) {
				var rawimg = go.transform.Find("Image").GetComponent<UnityEngine.UI.RawImage>();
				this.callback( (Texture2D)rawimg.texture );
				this.gameObject.SetActive(false);
			}
		}
	}


}