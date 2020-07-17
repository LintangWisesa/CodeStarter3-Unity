using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour {

	public delegate void CountdownFinished();
	public static event CountdownFinished OnCountdownFinished;//Gamemanager geriye sayım bitmiş mi öğrensin ve PageState None olsun diye


	Text countdown;

	void OnEnable(){
	 //coutdownPage GameManagerda aktif olduğunda OnEnable çalışır.
		countdown = GetComponent<Text>();
		countdown.text = "3";
		StartCoroutine ("Countdown");
	
	
	}

	IEnumerator Countdown(){
		//Coroutine işlemi için gerekli
		int count = 3;
		for (int i = 0; i < count; i++) {
			countdown.text = (count - i).ToString();
			yield return new WaitForSeconds (1);
		
		
		}
	
		OnCountdownFinished ();
	}
}
