using UnityEngine;
using System.Collections;

namespace AndreScripts{

	public static class Andre{
		public static void Log<T> (T t){
			#if UNITY_EDITOR
			Debug.Log ("> " + t);
			#endif	
		}

		public static int GetKeyboardSize (){
			#if UNITY_ANDROID && !UNITY_EDITOR
	        using(AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
	            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
	            using(AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect")){
	                View.Call("getWindowVisibleDisplayFrame", Rct);
	                return Screen.height - Rct.Call<int>("height");
	            }
	        }
	        #elif UNITY_IPHONE && !UNITY_EDITOR
	        return (int)TouchScreenKeyboard.area.size.y;
	        #else
	        return (int) (Screen.height / 3f);
	        #endif
	        return 0;
	    }

	}
}
