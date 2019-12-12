using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Linq;

namespace Tastybits.NativeGallery {

	// This is an interface to use the androd gallery and the image picker to pick an image in unity.
	// You can still use the classes UIImagePicker directly for a more specific iOS related
	// set of functionality.
	public class ImagePicker {
		static System.Action<Texture2D> _callback;
		public static void OpenGallery( System.Action<Texture2D> callback ) {
			_callback = callback;

#if UNITY_IPHONE && !UNITY_EDITOR
			UIImagePicker.OpenPhotoAlbum( ( Texture2D texture, bool ok, string errMsg )=>{
				_callback(texture);
			} );
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidGallery.OpenGallery( (Texture2D tex)=>{
				_callback(tex);
			});
#elif UNITY_EDITOR
			NativeGalleryController.OpenEditorGallery( callback );
#else
			Debug.LogError("OpenGallery: Function not implemented for platform " + Application.platform );
#endif
		}
	}



}