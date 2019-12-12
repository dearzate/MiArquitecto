using System;
using UnityEngine;

#if UNITY_ANDROID && (!UNITY_EDITOR || NATIVE_GALLERY_DEV)

namespace Tastybits.NativeGallery {
	
	public class AndroidGallery {

		static AndroidJavaClass _AndroidCls=null;
		static AndroidJavaClass AndroidCls {
			get { 
				if( _AndroidCls == null ) {
					_AndroidCls = new AndroidJavaClass("com.NativeGallery.AndroidGallery");
				}
				return _AndroidCls;
			}
		}

		public static void OpenGallery( System.Action<Texture2D> callback ) { 
			if( Application.platform != RuntimePlatform.Android ) {
				Debug.LogError("Cannot do this on a non Android platform");
			}

			string callbackId=
			NativeIOSDelegate.CreateNativeIOSDelegate( ( System.Collections.Hashtable args )=>{
				Debug.Log("NativeCallback returned");
				bool succeeded = System.Convert.ToBoolean(args["succeeded"]);
				bool cancelled = System.Convert.ToBoolean(args["cancelled"]);
				string path = System.Convert.ToString( args["path"] );
				Debug.Log("AndroidGallery returned with succeeded = " + succeeded + " cancelled = " + cancelled + " path = " + path );

				var www = new WWW( path );
				if( succeeded && !cancelled ) {
					WWWUtil.Wait( www, ( WWW w, bool www_ok ) => {
						string msg = ( www_ok ? "" : "error loading file" );
						if( www_ok && w.texture == null ) {
							www_ok=false;
							msg = "texture is null";
						}
						callback( w.texture );
					} );
				} else {
					string msg = cancelled ? "cancelled" : "";
					callback( null );
				}

			}).name;

			AndroidCls.CallStatic("OpenGallery", new object[] { callbackId } );
		}

	}

}


#endif
