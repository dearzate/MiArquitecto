using UnityEngine;
using System.Collections;


namespace Tastybits.NativeGallery {
	

	[RequireComponent(typeof(RectTransform))]
	public class NativeGalleryController : MonoBehaviour {
		static NativeGalleryController _instance;

		public static NativeGalleryController instance {
			get {
				if( _instance == null ) {
					var gos = Object.FindObjectsOfType<GameObject>();
					foreach( var go in gos ) {
						var tmp = go.GetComponentInChildren<NativeGalleryController>(true);
						if( tmp != null ) {
							_instance = tmp;
							break;
						}
					}
				}
				return _instance;
			}
		}

		void Awake() {
			_instance=this;
			if( Time.frameCount < 3 ) {
				_instance.gameObject.SetActive(false);
			}
		}

		void Update() {
			if( _instance == null ){
				_instance=this;
			}
		}

		// This holds an instance to some UI we will show in the editor to make
		// it easier to integrate and we don't need to run on the device ( iOS/Android ) 
		// all the time to test the integration.
		public GameObject editorGallery;


		public static void OpenGallery( System.Action<Texture2D> callback ) {
			if( instance == null ) {
				Debug.LogError("Cannot open Test gallery in editor mode since no instance of NativeGalleryController was found" );
				callback(null);
				return;
			}
			instance.gameObject.SetActive(true);
			if( UnityEngine.Application.isEditor == false && instance.editorGallery != null ) {
				instance.editorGallery.SetActive(false);
			}
			ImagePicker.OpenGallery( ( Texture2D tx ) => {
				instance.gameObject.SetActive(false);
				callback( tx );
			} );
		}

		// Opens the editor gallery.
		public static void OpenEditorGallery( System.Action<Texture2D> callback ) {
			if( instance == null ) {
				Debug.LogError("Cannot open Test gallery in editor mode since no instance of NativeGalleryController was found" );
				callback(null);
				return;
			}
			if( instance.editorGallery == null || ( instance.editorGallery.GetComponent<EditorGalleryUI>()==null ) ) {
				Debug.LogError("Cannot open Test gallery in editor mode since no referance to an EditorGalleryUI object was found" );
				return;
			}
			var edgal = instance.editorGallery.GetComponent<EditorGalleryUI>();
			edgal.OpenGallery( callback );
		}
	}


}