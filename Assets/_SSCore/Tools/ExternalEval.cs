using UnityEngine;
using System.Collections;

public class ExternalEval : MonoBehaviour {

	[Multiline(20)]
	public string evalContent = "";

	// Use this for initialization
	void Start ()
	{
#if UNITY_WEBPLAYER
		/*
		string test0 =
			"var div1 = document.createElement('div');" +
			"div1.id = \"div1\";" +
			"var body0 = document.getElementsByTagName('body')[0];" +
			"body0.insertBefore(div1, body0.children[0]);";
		Application.ExternalEval(test0);

		string test1 = 
			"var script1 = document.createElement('script');" +
			"script1.async = true;" +
			"script1.type = \"text/javascript\";" +
			"script1.src = \"//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js\";" +
			"document.getElementById(\"div1\").appendChild(script1);";
		Application.ExternalEval(test1);
		
		string test2 =
			"var ins1 = document.createElement('ins');" +
			"ins1.className = \"adsbygoogle\";" +
			"var adstyle = document.createAttribute('style'); adstyle.value = \"display:inline-block;width:728px;height:90px\"; ins1.attributes.setNamedItem(adstyle);" +
			"var adclient = document.createAttribute('data-ad-client'); adclient.value = \"ca-pub-3023658699028886\"; ins1.attributes.setNamedItem(adclient);" +
			"var adslot = document.createAttribute('data-ad-slot'); adslot.value = \"6117737454\"; ins1.attributes.setNamedItem(adslot);" +
			"document.getElementById(\"div1\").appendChild(ins1);";
		Application.ExternalEval(test2);

		string test3 =
			"var script2 = document.createElement('script');" +
			"script2.innerHTML = \"(adsbygoogle = window.adsbygoogle || []).push({});\";" +
			"document.getElementById(\"div1\").appendChild(script2);";
		Application.ExternalEval(test3);

		string injection =
		"var headerElement = document.createElement('div');" +
		"headerElement.textContent = ('Check out our other great games: ...');" +
		"var body = document.getElementsByTagName('body')[0];" +
		"var insertionPoint = body.children[0]; " +
		"body.insertBefore(headerElement, insertionPoint);";
		//Application.ExternalEval(injection);
		*/
#endif
	}
}
