/*
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Collections;
using UnityEngine;

public class SampleWebView : MonoBehaviour
{
    public string Url;
    public GUIText status;
    WebViewObject webViewObject;
    private bool showingBrowser;

    private void Start()
    {
        //AbrirEnlace("https://vip.e-pago.com.mx/pgs/WebPay?id_company=C187&xmlm=5647586E6A376D4C4A384452524F43744E42415236355459627A58736D744138305554677254515145657574623445783768725A37755453787732583645335656556B6F44362F7848634D410A363562674B5079414C7643776F2F6C714C314A6F70326F36784E66644D2F6B70745A38326168736D696E6D3567576E657971414554306A6B4673787030716A727241666C38583978365635780A654B306B65395564574371326A3041505536364C78326838342F762F356756592F6B61562B564A796938646F664F50372F2B5967325532483064616F53513D3D&xmla=363034644f48656f4d5633414e654b5251716b494d6e6d4436432f537a5342544d695253337a564a75584b5748684e3474336b485939335875546c7239462b2f347231612f4334356e564f3434387838467435427254385542355077736d4850444b50726e78674e46794d44436f366c4258374551526c377541774764755255384566784648486b334a47504a456e644574336f4f55637058623666355a433675424263634c383837566d75736c6c3334376a55557a77673854726e6c76756c2f6b4e737651306c70624b57743068672b35494449734e57477150345951586a51314a7a5457736359642b38356253477370314765556b36366e437048784a624c6c38774647684a4f6b43355538497735486c6f732b58526c3846334a3342765432524d48537a707238586c37584b50647271305657747a3432645976775a73&xmle=");
        showingBrowser = false;
        //AbrirEnlace("http://www.google.com");
    }

    public void AbrirEnlace(string url)
    {
        Url = url;
        StartCoroutine("AbrirPagina");
    }

    IEnumerator AbrirPagina()
    {
        showingBrowser = true;
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                status.text = msg;
                status.GetComponent<Animation>().Play();
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                status.text = msg;
                status.GetComponent<Animation>().Play();
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
#if !UNITY_ANDROID
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#endif
                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            },
            //ua: "custom user agent string",
            enableWKWebView: true);
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
        webViewObject.SetMargins(0, Screen.height/8, 0, 0);
        webViewObject.SetVisibility(true);

#if !UNITY_WEBPLAYER
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts) {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://")) {  // for Android
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
                } else {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html") {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
        if (Url.StartsWith("http")) {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        } else {
            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
        }
        webViewObject.EvaluateJS(
            "parent.$(function() {" +
            "   window.Unity = {" +
            "       call:function(msg) {" +
            "           parent.unityWebView.sendMessage('WebViewObject', msg)" +
            "       }" +
            "   };" +
            "});");
#endif
        yield break;
    }

#if !UNITY_WEBPLAYER
    /*void OnGUI()
    {
        if (!showingBrowser)
            return;

        GUI.enabled = webViewObject.CanGoBack();
        if (GUI.Button(new Rect(10, 10, 80, 80), "<")) {
            webViewObject.GoBack();
        }
        GUI.enabled = true;

        GUI.enabled = webViewObject.CanGoForward();
        if (GUI.Button(new Rect(100, 10, 80, 80), ">")) {
            webViewObject.GoForward();
        }
        GUI.enabled = true;

        GUI.TextField(new Rect(200, 10, 300, 80), "" + webViewObject.Progress());
    }*/
#endif
}
