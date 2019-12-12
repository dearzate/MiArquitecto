using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CuestionarioData : MonoBehaviour{

	public enum Espacios{
		Baños = 1,
		Cocinas = 2,
		Comedores = 3,
		Pasillos = 4,
		Recamaras = 5,
		Salas = 6,
		Exteriores = 7,
		Comerciales = 8
	};

	public enum Estilos{
		Clasico = 1,
		Contemporaneo = 2,
		Minimalista = 3,
		Rustico = 4,
		Vintage = 5
	};

	public enum Producto{
		Piso = 1,
		Muro = 2,
		PisoYMuro = 3
	};

	public enum Tipo{
		Cemento = 1,
		Decorado = 2,
		Granito = 3,
		Madera = 4,
		Marmol = 5,
		Metal = 6,
		Piedra = 7,
		Solido = 8,
		Textil = 9,
		Vidrio = 10
	};

	public enum Textura{
		Brillante = 1,
		Lisa = 2,
		Rugosa = 3,
		Mate = 4
	};

	public enum Tamano{
		Extra = 1,
		Grande = 2,
		Mediano = 3,
		Chico = 4,
		Tablon = 5
	};

	[Serializable]
	public class Data{
		public Espacios respEspacios;
		public Estilos respEstilos;
		public Producto respProducto;
//		public Tipo respTipo;
		public bool[] respTipo = new bool[10];
//		public Tamano respTamano;
		public bool[] respTamano = new bool[5];

		public int respPosicion;
	}

	public Data respData;

	public bool AtLeastOneTipoIsSelected(){
		for (int i=0; i<10; i++) {
			if (respData.respTipo[i]) {
				return true;
			}
		}

		return false;
	}

	public bool AtLeastOneTamanoIsSelected(){
		for (int i=0; i<5; i++) {
			if (respData.respTamano[i]) {
				return true;
			}
		}

		return false;
	}

	public void SaveData(){
		if (!Directory.Exists("Saves"))
			Directory.CreateDirectory("Saves");

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Create("Saves/save.binary");

		formatter.Serialize(saveFile, respData);

		saveFile.Close();
	}

	public void LoadData(){
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Open("Saves/save.binary", FileMode.Open);

		respData = (Data)formatter.Deserialize(saveFile);

		saveFile.Close();
	}


}
