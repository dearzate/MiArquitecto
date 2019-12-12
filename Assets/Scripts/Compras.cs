using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compras : MonoBehaviour
{

	public GameObject basePrefabPdf;
	public GameObject pdfList;

	public class PurchaseData
	{
		public string date;
		public string url;

		public PurchaseData(string date, string url)
		{
			this.date = date;
			this.url = url;
		}
	}

	void Start()
	{
		/*UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Sound | UnityEngine.iOS.NotificationType.Badge);
		localNotification.applicationIconBadgeNumber = 1;
		localNotification.alertBody = "test";
		localNotification.fireDate = DateTime.Now.AddSeconds(10D);
		localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);*/
	}

	public void addPdfChilds(Proposals proposals)
	{
		/* Dummy data
		List<PurchaseData> list = new List<PurchaseData>();
		list.Add(new PurchaseData("2019-03-12", "https://roho.fitness/pdf/terms/terms.pdf"));
		list.Add(new PurchaseData("2019-03-11", "https://google.com")); */

		foreach (Transform child in pdfList.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		foreach (Proposal data in proposals.PROPUESTAS)
		{
			GameObject prefabInstance = Instantiate(basePrefabPdf) as GameObject;
			prefabInstance.transform.SetParent(pdfList.transform);
			prefabInstance.transform.localScale = new Vector3(1, 1, 1);
			prefabInstance.GetComponentsInChildren<Text>()[0].text = "Propuesta #" + data.PROP_ID; //+"\n\nCosto $"+data.PRECIO_COMPRA;
			prefabInstance.GetComponentsInChildren<Text>()[2].text = data.FECHA_ALTA;// + "\n\n" + data.STATUS_PDF;
			prefabInstance.GetComponentsInChildren<Text>()[3].text = "Costo $" + data.PRECIO_COMPRA;
			prefabInstance.GetComponentsInChildren<Text>()[1].text = data.STATUS_PDF;

			prefabInstance.GetComponentsInChildren<PdfData>()[0].date = data.FECHA_ALTA;
			prefabInstance.GetComponentsInChildren<PdfData>()[0].price = data.PRECIO_COMPRA;
			prefabInstance.GetComponentsInChildren<PdfData>()[0].status = data.STATUS_PDF;
			prefabInstance.GetComponentsInChildren<PdfData>()[0].prop_id = data.PROP_ID;
			prefabInstance.GetComponentsInChildren<PdfData>()[0].invoice = data.NOMBRE_FACTURA;

			prefabInstance.GetComponentsInChildren<PdfData>()[1].date = data.FECHA_ALTA;
			prefabInstance.GetComponentsInChildren<PdfData>()[1].price = data.PRECIO_COMPRA;
			prefabInstance.GetComponentsInChildren<PdfData>()[1].status = data.STATUS_PDF;
			prefabInstance.GetComponentsInChildren<PdfData>()[1].prop_id = data.PROP_ID;
			prefabInstance.GetComponentsInChildren<PdfData>()[1].invoice = data.NOMBRE_FACTURA;

			prefabInstance.GetComponentsInChildren<PdfData>()[0].script = transform.GetComponent<AmazonRequest>();
			prefabInstance.GetComponentsInChildren<PdfData>()[1].script = transform.GetComponent<AmazonRequest>();
		}
	}

	public void toggle(int index)
	{

	}

	public void toggleAll()
	{

	}

	public void back()
	{
		if (!transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().isMovingSomething)
		{
			transform.GetComponent<ViewsManager>().views[0].GetComponent<View>().goBack(2);
		}
	}
}
