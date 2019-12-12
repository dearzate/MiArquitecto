using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using System.IO;
using System;

public class NewWebPay : MonoBehaviour 
{


    private string XMLstring;
    private string encryptedString;
    public string decryptedString;

    public ConsultasAPI consultaXML;
    public SampleWebView webView;
    private WebPayView _webPayView;

    public string key;

    private void Start()
    {
        _webPayView = GameObject.FindObjectOfType<WebPayView>();
        //ConstruirXML("250");
    }

    public void ConstruirXML (string cantidad, string intencion)
    {
        if (!cantidad.Contains("."))
            cantidad = cantidad + ".00";


        /*XMLstring =                  
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<P>\n" +
            "<business>\n" +
            "<id_company>C187</id_company>\n" +
            "<id_branch>5211</id_branch>\n" +
            "<user>C187SIUS0</user>\n" +
            "<pwd>C2EDUPIVB9</pwd>\n" +
            "</business>\n" +
            "<url>\n" +
            "<reference>+ intencion + </reference>\n" +
            "<amount>"+ cantidad +"</amount>\n" +
            "<moneda>MXN</moneda>\n" +
            "<canal>W</canal>\n" +
            "<omitir_notif_default>1</omitir_notif_default>\n" +
            "<st_correo>0</st_correo>\n" +
            //"<mail_cliente>nospam@gmail.com</mail_cliente>" +
            "</url>\n" +
            "</P>";*/

        XMLstring =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<P>\n" +
            "<business>\n" +
            "<id_company>C187</id_company>\n" +
            "<id_branch>035</id_branch>\n" +
            "<user>C187ZAUS0</user>\n" +
            "<pwd>W8ERS3VAA2</pwd>\n" +
            "</business>\n" +
            "<url>\n" +
            "<reference>" + intencion +  "</reference>\n" +
            "<amount>" + cantidad + "</amount>\n" +
            "<moneda>MXN</moneda>\n" +
            "<canal>W</canal>\n" +
            "<omitir_notif_default>1</omitir_notif_default>\n" +
            "<st_correo>0</st_correo>\n" +
            //"<mail_cliente>nospam@gmail.com</mail_cliente>" +
            "</url>\n" +
            "</P>";


        CifrarXML();
        EnviarXML();
	}
	
	public void CifrarXML()
    {
        Debug.Log("Antes de cifrar es: " + XMLstring);

        encryptedString = webpayplus.AESCrypto.encrypt(XMLstring, key);
        //encryptedString = encryptedString.Replace("%", "%25").Replace(" ", "%20").Replace("+", "%2B").Replace("=", "%3D").Replace("/", "%2F");
        Debug.Log("<color=red>La cadena convertida es: " + encryptedString + "</color>");

       // Debug.Log( "Esto dijo al redecifrar: " +  webpayplus.AESCrypto.decrypt( key, "YXtak0XQlED1Gwsmzl378NCbfhBI4hSrtaF/sWFGRm8OIHr6SDGqW/G4ImxG7Rh/3M00oPDXP2xksheLYHmRyUnKx4DGHqrP13YlfTjas/I=" ) );
    }

    public void EnviarXML()
    {
        consultaXML.Elementos[0].Cadena = "<pgs><data0>9265654618</data0><data>"+ encryptedString +"</data></pgs>";
        string cadenaFinal = consultaXML.Elementos[0].Cadena;
        Debug.Log("La cadena antes de reemplazar valores es: " + cadenaFinal);
        cadenaFinal = consultaXML.Elementos[0].Cadena.Replace("<","%3C");
        cadenaFinal = cadenaFinal.Replace(">","%3E");
        cadenaFinal = cadenaFinal.Replace("/", "%2F");
        cadenaFinal = cadenaFinal.Replace("+","%2B");
        cadenaFinal = cadenaFinal.Replace("=", "%3D");
        consultaXML.ConsultaHeadersPost("xml=" + cadenaFinal);
    }

    public void DescifrarResultado( )
    {
        

        decryptedString = webpayplus.AESCrypto.decrypt(key, consultaXML.Resultado);
        File.WriteAllText( Path.Combine( Application.persistentDataPath, "temp.xml" ) , decryptedString);
        Debug.Log("Dijo: " + decryptedString);


        Debug.Log( Path.Combine(Application.persistentDataPath, "temp.xml") );
        P_RESPONSE p_resp = new P_RESPONSE();


        XmlSerializer serializador = new XmlSerializer(typeof(P_RESPONSE));
        FileStream stream = new  FileStream( Path.Combine(Application.persistentDataPath, "temp.xml"), FileMode.Open);
        p_resp = serializador.Deserialize( stream ) as P_RESPONSE;
        stream.Close();

        Debug.Log(p_resp.nb_url);
        webView.AbrirEnlace(p_resp.nb_url);


    }


    public void InterpretarResultado(ConsultasAPI consultaStatusPago)
    {
        Debug.Log(consultaStatusPago.Resultado);
        if (consultaStatusPago.Resultado.Contains("Denied"))
        {
            //_webPayView.DeniedPayment();
            return;
        }
        else if (consultaStatusPago.Resultado.Contains("Error"))
        {
            //_webPayView.ErrorPayment();
            return;
        }
        else
        {
            
        }
    }

    public void EsperarParaVolverAConsultar()
    {
        
        StartCoroutine("RutinaEsperarParaConsultar");
    }

    IEnumerator RutinaEsperarParaConsultar()
    {
        yield return new WaitForSeconds(3);
        _webPayView.consultaStatusBanco.ConsultaConHeaders();
    }




}







[XmlRoot("P_RESPONSE")]
public class P_RESPONSE
{
    public string cd_response;
    public string nb_response;
    public string nb_url;

}

