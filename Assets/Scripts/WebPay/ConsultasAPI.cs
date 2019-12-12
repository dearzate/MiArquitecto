using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using AndreScripts;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class ConsultasAPI : MonoBehaviour 
{



	// Direccion de la API para las consultas
	public string DireccionAPI;


	// Eventos de caso de exito, evento vacio y error
	public UnityEvent EventoExito;
	public UnityEvent EventoVacio;
	public UnityEvent EventoError;

	// Elementos para comparar

	public List<ListaElementos> Elementos;


	public string[] CasosExito;
	public string[] CasosVacio;
	public string[] CasosError;


	public List<string> Cabeceras;
	public List<string> Valores;
	//
	public int CasoDeError;

	// Variables para eventos fuera de esta clase
	public event EventHandler<InformacionRegresada> ConsultaCompleta;
	public string Resultado;

	public Texture2D texturaResultado;

	void Start ()
	{
		CasoDeError = 0;
	}
		
	// 
	public void Consultar()
	{
		StartCoroutine ("RutinaConsultar");
	}

	/// <summary>
	/// Agrega cabeceras para las consultas del Web Service
	/// </summary>
	/// <param name="Llave">Nombre de la variables</param>
	/// <param name="Contenido">Valor de la llave.</param>
	public void AgregarCabecera(string Llave, string Contenido)
	{
		Cabeceras.Add(Llave);
		Valores.Add(Contenido);
	}

	/// <summary>
	/// Reemplaza el valor de una llave especificada
	/// </summary>
	/// <param name="Llave">Llave a buscar.</param>
	/// <param name="Contenido">Nuevo valor a reemplazar.</param>
	public void ReemplazarCabecera(string Llave, string Contenido)
	{
		//Andre.Log("Reemplazando " + Llave + " con " + Contenido);
		Valores[Cabeceras.IndexOf(Llave)] = Contenido;
	}

	public IEnumerator RutinaConsultar()
	{
		WWWForm form = new WWWForm();
		foreach(ListaElementos listelem in Elementos)
		{
			form.AddField( listelem.NombreCampo, 
				(listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Float)) ? 
				listelem.Flotante.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.InputField)) ? 
				listelem.CadenaEntrada.text : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Int)) ? 
				listelem.Entero.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.String)) ? 
				listelem.Cadena : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Toggle)) ?
				( (listelem.Togleador.isOn) ? "1" : "0" ) : listelem.TextoFijo.text 
			);

			Andre.Log("Agregando campo " + listelem.NombreCampo);
		}

		WWW www = new WWW (DireccionAPI);
		yield return www;


		if (www.error == null)
		{
			Andre.Log(www.text);

			Resultado = www.text;


			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					CasoDeError = i;
					EventoError.Invoke();
					yield return null;
				}
			}


			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if ( www.text.Contains ( CasosVacio [i] ) )
				{
					EventoVacio.Invoke ();
					break;
				}
			}



			if (CasosExito.Length == 0)
				EventoExito.Invoke();
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{
					if ( www.text.Contains ( CasosExito [i] ) )
					{
						EventoExito.Invoke();
						break;
					}
				}
			}



		}
		else
		{
			Andre.Log(www.error);
			Resultado = www.error;
			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					EventoError.Invoke();
					break;
				}
			}
		}
	}



	public void ConsultaPersonalizada(string Consulta)
	{
		
	}

	public IEnumerator RutinaConsultaPersonalizada()
	{
		WWW www = new WWW("");
		yield return www;
	}


	public void ConsultaConHeaders ()
	{
		StartCoroutine(RutinaConsultaConHeaders());	
	}

	public IEnumerator RutinaConsultaConHeaders()
	{
		WWWForm form = new WWWForm();
		byte[] Datos;


		if (Elementos.Count > 0)
		{
			string CadenaConsulta = "{\n";

			foreach(ListaElementos listelem in Elementos)
			{
				string Campo = (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Float)) ? 
					listelem.Flotante.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.InputField)) ? 
					listelem.CadenaEntrada.text : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Int)) ? 
					listelem.Entero.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.String)) ? 
					listelem.Cadena : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Toggle)) ?
					( (listelem.Togleador.isOn) ? "1" : "0"  ) : listelem.TextoFijo.text;

				form.AddField( listelem.NombreCampo,  Campo);

			}

			Datos = form.data;
		}
		else
			Datos = null;


		Dictionary<string,string> headers = new Dictionary<string,string>();

		for (int i = 0; i < Cabeceras.Count; i++)
		{
			headers.Add(Cabeceras[i], Valores[i]);

		}

		WWW www = new WWW( DireccionAPI, Datos,  headers );

		yield return www;

		if (www.error == null)
		{
			Andre.Log(this.name + www.text);

			Resultado = www.text;

			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					CasoDeError = i;
					EventoError.Invoke();
					goto salida;
				}
			}

			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if ( www.text.Contains ( CasosVacio [i] ) )
				{
					Andre.Log(this.name + " esta vacio porque tiene " + CasosVacio[i]);
					EventoVacio.Invoke ();
					goto salida;
				}
			}
				


			if (CasosExito.Length == 0)
			{
				EventoExito.Invoke();
			}
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{
					
					if ( www.text.Contains ( CasosExito [i] ) )
					{
						EventoExito.Invoke();
						break;
					}
				}
			}




		}
		else
		{
			Andre.Log(this.name + www.error);
			Resultado = www.error;

			EventoError.Invoke();
		}


		salida:
		Andre.Log("Salio de la funcion");

	}

    public void ConsultaHeadersYBody(string Body)
    {
        StartCoroutine("RutinaConsultaHeadesYBody", Body);
    }

	public void ConsultaHeadersYBody(byte[] Body)
	{
		StartCoroutine("RutinaConsultaHeadesYBodyBytes", Body);
	}

	public IEnumerator RutinaConsultaHeadesYBody(string form)
	{
		byte[] Datos;
		Datos = Encoding.UTF8.GetBytes(form);





		Dictionary<string, string> headers = new Dictionary<string, string>();

		for (int i = 0; i < Cabeceras.Count; i++)
		{
			headers.Add(Cabeceras[i], Valores[i]);

		}

		WWW www = new WWW(DireccionAPI, Datos, headers);


		yield return www;

		if (www.error == null)
		{
			Andre.Log(this.name + www.text);

			Resultado = www.text;

			for (int i = 0; i < CasosError.Length; i++)
			{
				if (www.text.Contains(CasosError[i]))
				{
					CasoDeError = i;
					EventoError.Invoke();
					goto salida;
				}
			}

			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if (www.text.Contains(CasosVacio[i]))
				{
					Andre.Log(this.name + " esta vacio porque tiene " + CasosVacio[i]);
					EventoVacio.Invoke();
					yield return null;
				}
			}



			if (CasosExito.Length == 0)
			{
				EventoExito.Invoke();
			}
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{

					if (www.text.Contains(CasosExito[i]))
					{
						EventoExito.Invoke();
						break;
					}
				}
			}




		}
		else
		{
			Andre.Log(this.name + www.error);
			Resultado = www.error;

			EventoError.Invoke();
		}


		salida:
		Andre.Log("Salio de la funcion");
	}

	public IEnumerator RutinaConsultaHeadesYBodyBytes(byte[] Datos)
    {
        
        Dictionary<string, string> headers = new Dictionary<string, string>();

        for (int i = 0; i < Cabeceras.Count; i++)
        {
            headers.Add(Cabeceras[i], Valores[i]);
        }



		//form.data = Datos;
		//form.headers = headers;
		WWW www = new WWW(DireccionAPI,Datos, headers);

        yield return www;

        if (www.error == null)
        {
            Andre.Log(this.name + www.text);

            Resultado = www.text;

            for (int i = 0; i < CasosError.Length; i++)
            {
                if (www.text.Contains(CasosError[i]))
                {
                    CasoDeError = i;
                    EventoError.Invoke();
                    goto salida;
                }
            }

            for (int i = 0; i < CasosVacio.Length; i++)
            {
                if (www.text.Contains(CasosVacio[i]))
                {
                    Andre.Log(this.name + " esta vacio porque tiene " + CasosVacio[i]);
                    EventoVacio.Invoke();
                    yield return null;
                }
            }



            if (CasosExito.Length == 0)
            {
                EventoExito.Invoke();
            }
            else
            {
                for (int i = 0; i < CasosExito.Length; i++)
                {

                    if (www.text.Contains(CasosExito[i]))
                    {
                        EventoExito.Invoke();
                        break;
                    }
                }
            }




        }
        else
        {
            Andre.Log(this.name + www.error);
            Resultado = www.error;

            EventoError.Invoke();
        }


    salida:
        Andre.Log("Salio de la funcion");
    }

	/*public IEnumerator RutinaEnviarImagen(byte[] Datos)
	{

		byte[] ByteArray;

		UnityWebRequest WebRequest = new UnityWebRequest( DireccionAPI , UnityWebRequest.kHttpVerbPOST );
		UploadHandlerRaw MyUploadHandler = new UploadHandlerRaw( ByteArray );
		MyUploadHandler.contentType= "application/x-www-form-urlencoded"; // might work with 'multipart/form-data'
		WebRequest.uploadHandler= MyUploadHandler;
	}*/

	public void ConsultaHeadersPost()
	{
		StartCoroutine("RutinaConsultaHeadersPost");
	}

    public void ConsultaHeadersPost(string rawBody)
    {
        StartCoroutine("RutinaConsultaHeadersPostRaw", rawBody);
    }

	public IEnumerator RutinaConsultaHeadersPost()
	{
		WWWForm form = new WWWForm();


		Dictionary<string,string> headers = new Dictionary<string,string>();

		for (int i = 0; i < Cabeceras.Count; i++)
		{
			headers.Add(Cabeceras[i], Valores[i]);

		}

        string CadenaConsulta = ""; // "{\n";

		if (Elementos.Count > 0)
		{
			foreach(ListaElementos listelem in Elementos)
			{
				string Campo = (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Float)) ? 
					listelem.Flotante.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.InputField)) ? 
					"\"" + listelem.CadenaEntrada.text + "\"": (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Int)) ? 
					( (listelem.Entero < 0) ? "null" : listelem.Entero.ToString() ): (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.String)) ? 
					"\"" + listelem.Cadena + "\"" : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Toggle)) ?
					( (listelem.Togleador.isOn) ? "1" : "0"  ) : "\"" + listelem.TextoFijo.text + "\"";

				form.AddField(listelem.NombreCampo, Campo);
				CadenaConsulta = CadenaConsulta + "\t\"" + listelem.NombreCampo + "\"\t: " + Campo + ",\n";

                form.AddField("xml", listelem.Cadena);
			}

			CadenaConsulta = CadenaConsulta + "}";
			CadenaConsulta = CadenaConsulta.Replace(",\n}","\n}");
			Andre.Log(this.name + CadenaConsulta);


		}



        var DatosPars = form.data; // System.Text.Encoding.UTF8.GetBytes(CadenaConsulta);

		WWW www = new WWW(DireccionAPI, DatosPars, headers);
		yield return www;

		if (www.error == null)
		{
			Andre.Log("<color=blue>" + this.name + www.text + "</color>");

			Resultado = www.text;

			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					CasoDeError = i;
					EventoError.Invoke();
					//yield return null;
					goto salida;
				}
			}

			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if ( www.text.Contains ( CasosVacio [i] ) )
				{
                    EventoVacio.Invoke ();
					goto salida;
				}
			}


            if (CasosExito.Length == 0)
			{
				EventoExito.Invoke();
			}
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{
					if ( www.text.Contains ( CasosExito [i] ) )
					{
						EventoExito.Invoke();
						break;
					}
				}
			}
		}
		else
		{
			Andre.Log(this.name + www.error);
			Resultado = www.error;

			EventoError.Invoke();
		}

		salida:
		Andre.Log("Salio de la funcion");

	}
		
    public IEnumerator RutinaConsultaHeadersPostRaw(string rawParameter)
    {
        WWWForm form = new WWWForm();


        Dictionary<string, string> headers = new Dictionary<string, string>();

        for (int i = 0; i < Cabeceras.Count; i++)
        {
            headers.Add(Cabeceras[i], Valores[i]);

        }

        Debug.Log(rawParameter);

        var DatosPars = System.Text.Encoding.UTF8.GetBytes(rawParameter);

        WWW www = new WWW(DireccionAPI, DatosPars, headers);
        yield return www;

        if (www.error == null)
        {
            Andre.Log("<color=white>" + this.name + www.text + "</color>");

            Resultado = www.text;

            for (int i = 0; i < CasosError.Length; i++)
            {
                if (www.text.Contains(CasosError[i]))
                {
                    CasoDeError = i;
                    EventoError.Invoke();
                    //yield return null;
                    goto salida;
                }
            }

            for (int i = 0; i < CasosVacio.Length; i++)
            {
                if (www.text.Contains(CasosVacio[i]))
                {
                    EventoVacio.Invoke();
                    goto salida;
                }
            }


            if (CasosExito.Length == 0)
            {
                EventoExito.Invoke();
            }
            else
            {
                for (int i = 0; i < CasosExito.Length; i++)
                {
                    if (www.text.Contains(CasosExito[i]))
                    {
                        EventoExito.Invoke();
                        break;
                    }
                }
            }
        }
        else
        {
            Andre.Log(this.name + www.error);
            Resultado = www.error;

            EventoError.Invoke();
        }

    salida:
        Andre.Log("Salio de la funcion");

    }	

	public void ConsultaImagenHeadersPost()
	{
		StartCoroutine("RutinaConsultaImagenHeaders");
	}
	public IEnumerator RutinaConsultaImagenHeaders()
	{
		WWWForm form = new WWWForm();
		byte[] Datos;


		if (Elementos.Count > 0)
		{
			string CadenaConsulta = "{\n";

			foreach(ListaElementos listelem in Elementos)
			{
				string Campo = (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Float)) ? 
					listelem.Flotante.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.InputField)) ? 
					listelem.CadenaEntrada.text : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Int)) ? 
					listelem.Entero.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.String)) ? 
					listelem.Cadena : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Toggle)) ?
					( (listelem.Togleador.isOn) ? "1" : "0"  ) : listelem.TextoFijo.text;

				form.AddField( listelem.NombreCampo,  Campo);

			}

			Datos = form.data;
		}
		else
			Datos = null;


		Dictionary<string,string> headers = new Dictionary<string,string>();

		for (int i = 0; i < Cabeceras.Count; i++)
		{
			headers.Add(Cabeceras[i], Valores[i]);

		}

		WWW www = new WWW( DireccionAPI, Datos,  headers );

		yield return www;

		if (www.error == null)
		{
			//Andre.Log(this.name + www.text);

			Resultado = www.text;

			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					CasoDeError = i;
					EventoError.Invoke();
					goto salida;
				}
			}

			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if ( www.text.Contains ( CasosVacio [i] ) )
				{
					Andre.Log(this.name + " esta vacio porque tiene " + CasosVacio[i]);
					EventoVacio.Invoke ();
					goto salida;
				}
			}



			if (CasosExito.Length == 0)
			{
				//EventoExito.Invoke();
			}
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{

					if ( www.text.Contains ( CasosExito [i] ) )
					{
						//EventoExito.Invoke();
						break;
					}
				}
			}




		}
		else
		{
			Andre.Log(this.name + www.error);
			Resultado = www.error;

			EventoError.Invoke();
		}
			

		if (texturaResultado==null)
			texturaResultado = new Texture2D(1,1);

		texturaResultado = www.texture;
        texturaResultado.Apply();
        yield return new WaitForEndOfFrame();
        EventoExito.Invoke();

        salida:
		Andre.Log("");

	}


	protected virtual void MetodoConsultaCompleta( InformacionRegresada e)
	{
		EventHandler <InformacionRegresada> manejador = ConsultaCompleta;
		if (manejador != null)
		{
			manejador(this, e);
		}
	}
}


public class InformacionRegresada : EventArgs
{
	public string Contenido;
}
