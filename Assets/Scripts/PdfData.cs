using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PdfData : MonoBehaviour
{

	public string date;
	public string price;
	public string status;
	public string prop_id;
	public string invoice;
	public AmazonRequest script;

	public void openProposalUrl()
	{
		Debug.Log("invoice: " + this.invoice);
		Debug.Log("invoice: " + this.prop_id);
		Debug.Log("invoice: " + this.date);
		script.type = InterceramicPetitionType.DOWNLOAD_PDF;
		script.downloadProposalPdf(this.prop_id + "_" + PlayerPrefs.GetInt("UserId"));//("117" + "_" + "20044");//prop_id+"_"+PlayerPrefs.GetInt("UserId"));
	}

	public void openInvoiceUrl()
	{
		Debug.Log("invoice: "+ this.invoice);
		Debug.Log("invoice: " + this.prop_id);
		Debug.Log("invoice: " + this.date);
		script.type = InterceramicPetitionType.DOWNLOAD_PDF;
		script.downloadInvoicePdf(this.invoice);//"DIN971119GG6TESTNCA8621.pdf");//prop_id+"_"+PlayerPrefs.GetInt("UserId"));
	}
}
