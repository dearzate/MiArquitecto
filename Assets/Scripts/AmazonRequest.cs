using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AmazonRequest : MonoBehaviour {

	/*     
	 * DEVELOPMENT
	 *
	static readonly string AWS_ACCESS_KEY_ID = "AKIAYJS2WBVK47Q7ZXVN";
	static readonly string AWS_SECRET_ACCESS_KEY = "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";
	static readonly string AWS_PREFIX = "https://";
	static readonly string AWS_S3_URL_BASE_VIRTUAL_HOSTED = "vu4uu7cohi.execute-api.us-west-2.amazonaws.com";
	static readonly string KEY = "MIARQ";
	static readonly string AMAZON_INTERCERAMIC_API_KEY = "epUBgSeUOE2boj9AzSrx246ZiQkdtvug4YGk4CTT";
	static readonly string AMAZON_REGION = "us-west-2";
	static readonly string AMAZON_SERVICE = "execute-api";
	static readonly string ENVIRONMENT = "dev";
	*/

	/*     
	 * PRODUCTION
	 */
	static readonly string AWS_ACCESS_KEY_ID = "AKIAYJS2WBVK6LKTP3E2";
	static readonly string AWS_SECRET_ACCESS_KEY = "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";
	static readonly string AWS_PREFIX = "https://";
	static readonly string AWS_S3_URL_BASE_VIRTUAL_HOSTED = "ek451vm8dj.execute-api.us-west-2.amazonaws.com";
	static readonly string KEY = "MIARQ";
	static readonly string AMAZON_INTERCERAMIC_API_KEY = "KJwRiczR135AsxtxdMW3zH1r4XxBEMC209hH7Qy5";
	static readonly string AMAZON_REGION = "us-west-2";
	static readonly string AMAZON_SERVICE = "execute-api";
	static readonly string ENVIRONMENT = "MIArquitecto-prod";

	string iso8601CurrentTime;
	PaymentTicket ticket;
	Proposals proposals;
	public PdfWebView webView;
	string dateString;
	string content_type_global;
	string signature_global;
	string prop_id = "0";
	string contact_id = "0";
	public int type;

	void SendAmazonS3Request(string type, string action, string content_type)
	{
		transform.GetComponent<ViewsManager>().showPopUpNoAnim(2);
		transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().startSpinning();
		Hashtable headers = new Hashtable();
		dateString = System.DateTime.UtcNow.AddSeconds(120.0f).ToString("yyyyMMdd");

		System.DateTime currentTime = System.DateTime.UtcNow;
		iso8601CurrentTime = currentTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
		iso8601CurrentTime = iso8601CurrentTime.Replace(" ", "").Replace("-", "").Replace(":", "");

		string policy = buildPolicy(type,action);

		string signature = ComputeSha256Hash(policy);

		string authorization = "AWS4-HMAC-SHA256\n" +
			iso8601CurrentTime + "Z" + "\n" +
			dateString + "/" + AMAZON_REGION + "/" + AMAZON_SERVICE + "/aws4_request\n" +
			signature;
		headers.Add("x-amz-date", iso8601CurrentTime);
		headers.Add("x-api-key", AMAZON_INTERCERAMIC_API_KEY);
		headers.Add("clave-modulo", KEY);
		headers.Add("claveapp", KEY);
		headers.Add("Prop_id", prop_id);
		headers.Add("Contacto_id", contact_id);
		headers.Add("Content-Type", content_type);
		signature_global = HexaFromByte(HmacSHA256(authorization, getSignatureKey(AWS_SECRET_ACCESS_KEY, dateString, AMAZON_REGION, AMAZON_SERVICE)));
		headers.Add("Authorization", " AWS4-HMAC-SHA256 Credential=" + AWS_ACCESS_KEY_ID + "/" + dateString + "/" + AMAZON_REGION + "/" + AMAZON_SERVICE + "/aws4_request" +
									 ", SignedHeaders=clave-modulo;claveapp;host;x-amz-date;x-api-key, Signature=" + HexaFromByte(HmacSHA256(authorization, getSignatureKey(AWS_SECRET_ACCESS_KEY, dateString, AMAZON_REGION, AMAZON_SERVICE))));

		// Setup the request url to be sent to Amazon
		WWW www = buildWWW(type, action, headers);

		// Send the request in this coroutine so as not to wait busily
		StartCoroutine(WaitForRequest(www));
	}

	void SendAmazonS3Request(string type, string action, string content_type, PaymentTicket ticket)
	{
		transform.GetComponent<ViewsManager>().showPopUpNoAnim(2);
		transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().startSpinning();
		this.ticket = ticket;
		Debug.Log("Si entre");
		Hashtable headers = new Hashtable();
		dateString = System.DateTime.UtcNow.AddSeconds(120.0f).ToString("yyyyMMdd");

		System.DateTime currentTime = System.DateTime.UtcNow;
		iso8601CurrentTime = currentTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
		iso8601CurrentTime = iso8601CurrentTime.Replace(" ", "").Replace("-", "").Replace(":", "");

		string policy = policyPostTicket(type, action, ticket);

		string signature = ComputeSha256Hash(policy);

		string authorization = "AWS4-HMAC-SHA256\n" +
			iso8601CurrentTime + "Z" + "\n" +
			dateString + "/" + AMAZON_REGION + "/" + AMAZON_SERVICE + "/aws4_request\n" +
			signature;
		headers.Add("x-amz-date", iso8601CurrentTime);
		headers.Add("x-api-key", AMAZON_INTERCERAMIC_API_KEY);
		headers.Add("clave-modulo", KEY);
		headers.Add("claveapp", KEY);
		headers.Add("Prop_id", prop_id);
		headers.Add("Contacto_id", contact_id);
		headers.Add("Content-Type", content_type);
		signature_global = HexaFromByte(HmacSHA256(authorization, getSignatureKey(AWS_SECRET_ACCESS_KEY, dateString, AMAZON_REGION, AMAZON_SERVICE)));
		headers.Add("Authorization", " AWS4-HMAC-SHA256 Credential=" + AWS_ACCESS_KEY_ID + "/" + dateString + "/" + AMAZON_REGION + "/" + AMAZON_SERVICE + "/aws4_request" +
									 ", SignedHeaders=clave-modulo;claveapp;host;x-amz-date;x-api-key, Signature=" + HexaFromByte(HmacSHA256(authorization, getSignatureKey(AWS_SECRET_ACCESS_KEY, dateString, AMAZON_REGION, AMAZON_SERVICE))));

		// Setup the request url to be sent to Amazon
		WWW www = buildWWW(type, action, headers);

		// Send the request in this coroutine so as not to wait busily
		StartCoroutine(WaitForRequest(www));
	}

	WWW buildWWW(string type, string action, Hashtable headers)
	{
		if (type.Equals("GET"))
			return new WWW(AWS_PREFIX + AWS_S3_URL_BASE_VIRTUAL_HOSTED + action, null, headers);
		else
			return new WWW(AWS_PREFIX + AWS_S3_URL_BASE_VIRTUAL_HOSTED + action, System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(ticket)), headers);
	}

	string buildPolicy(string type, string action)
	{
		return policyGet(type, action);
	}

	string policyPostTicket(string type, string action, PaymentTicket ticket)
	{
		return type + "\n" +
			action + "\n\n" +
			"clave-modulo:" + KEY + "\n" +
			"claveapp:" + KEY + "\n" +
			"host:" + AWS_S3_URL_BASE_VIRTUAL_HOSTED + "\n" +
			"x-amz-date:" + iso8601CurrentTime + "\n" +
			"x-api-key:" + AMAZON_INTERCERAMIC_API_KEY + "\n\n" +
			"clave-modulo;claveapp;host;x-amz-date;x-api-key\n" +
			ComputeSha256Hash(JsonUtility.ToJson(ticket));
	}

	string policyGet(string type, string action) 
	{
		return type + "\n" +
			action + "\n\n" +
			"clave-modulo:" + KEY + "\n" +
			"claveapp:" + KEY + "\n" +
			"host:" + AWS_S3_URL_BASE_VIRTUAL_HOSTED + "\n" +
			"x-amz-date:" + iso8601CurrentTime + "\n" +
			"x-api-key:" + AMAZON_INTERCERAMIC_API_KEY + "\n\n" +
			"clave-modulo;claveapp;host;x-amz-date;x-api-key\n" +
			"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
	}

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;

		switch (type)
		{
			case InterceramicPetitionType.TEST_CONNECTION:
				break;
			case InterceramicPetitionType.SEND_PAYMENT_RESULT:
				break;
			case InterceramicPetitionType.GET_BOUGHT_LIST:
				loadBoughtList(www);
				break;
			case InterceramicPetitionType.DOWNLOAD_PDF:
				downloadPDF(www);
				break;
			default:
				break;
		}

		// Check for errors
		if (www.error == null)
		{
			Debug.Log(www.text);
		}
		else
		{
			Debug.Log("WWW Error: " + www.error + " for URL: " + www.url);
		}
		transform.GetComponent<ViewsManager>().popUps[2].GetComponent<LoadingManager>().stopSpinning();
		transform.GetComponent<ViewsManager>().hidePopUpNoAnim(2);
	}

	void loadBoughtList(WWW www)
	{
		proposals = new Proposals();
		proposals = JsonUtility.FromJson<Proposals>(www.text);
		GetComponent<Compras>().addPdfChilds(proposals);
	}

	void downloadPDF(WWW www)
	{
		try
		{
			string myPath;
			#if UNITY_ANDROID
				myPath = "/storage/emulated/0/Download/response.pdf"; //ANDROID
			#elif UNITY_IOS
				myPath = Application.persistentDataPath + "/response.pdf"; //iOS
			#else
				myPath = Application.persistentDataPath + "/response.pdf"; //PC
			#endif
			File.WriteAllBytes(myPath, www.bytes);

			myPath = Application.dataPath + "/Snapshots/response.pdf";
			File.WriteAllBytes(myPath, www.bytes);
		}
		catch (Exception e)
		{
			Debug.Log("Error" + e.Message);
		}
		webView.gameObject.SetActive(true);
		webView.refreshWebView();
		GameObject.Find("PdfWebView").SetActive(true);
	}

	static string ComputeSha256Hash(string rawData)
	{
		// Create a SHA256   
		using (SHA256 sha256Hash = SHA256.Create())
		{
			// ComputeHash - returns byte array  
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

			// Convert byte array to a string   
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}
	}

	static byte[] HmacSHA256(String data, byte[] key)
	{
		String algorithm = "HmacSHA256";
		KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
		kha.Key = key;

		return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
	}

	static string HexaFromByte(byte[] bytes) 
	{
		StringBuilder builder = new StringBuilder();
		for (int i = 0; i < bytes.Length; i++)
		{
			builder.Append(bytes[i].ToString("x2"));
		}
		return builder.ToString();
	}

	static byte[] getSignatureKey(String key, String dateStamp, String regionName, String serviceName)
	{
		byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + key);
		byte[] kDate = HmacSHA256(dateStamp, kSecret);
		byte[] kRegion = HmacSHA256(regionName, kDate);
		byte[] kService = HmacSHA256(serviceName, kRegion);
		byte[] kSigning = HmacSHA256("aws4_request", kService);
		return kSigning;
	}


	// Use this for initialization
	void Start () {

	}

	public void testConecction() 
	{
		SendAmazonS3Request("GET", "/"+ ENVIRONMENT + "/pruebaconexion-get", "application/json");
	}

	public void sendPaymentResponse(String prop_id, PaymentTicket ticket) 
	{
		this.prop_id = prop_id;
		Debug.Log("prop_id "+ this.prop_id);
		SendAmazonS3Request("POST", "/" + ENVIRONMENT + "/enviarespuestapago", "application/json", ticket);
	}

	public void downloadProposalPdf(string doc_id) 
	{
		SendAmazonS3Request("GET", "/" + ENVIRONMENT + "/miarquitecto-repo/" + doc_id, "application/pdf");
	}

	public void downloadInvoicePdf(string doc_id)
	{
		SendAmazonS3Request("GET", "/" + ENVIRONMENT + "/facturacion-api/" + doc_id, "application/pdf");
	}

	public void getBoughtList(string contact_id)
	{
		this.contact_id = contact_id;
		SendAmazonS3Request("GET", "/" + ENVIRONMENT + "/obtenerpropuestascliente-get", "application/json");
	}
}
