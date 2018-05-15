using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class watch : MonoBehaviour {
	public TextMesh time;
	public TextMesh date;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		time.text = "" +  DateTime.Now.Hour + ":" + DateTime.Now.Minute;
		String week = "";
		switch (DateTime.Now.DayOfWeek) {
		case DayOfWeek.Monday:
			week = "월요일";
			break;
		case DayOfWeek.Tuesday:
			week = "화요일";
			break;
		case DayOfWeek.Wednesday:
			week = "수요일";
			break;
		case DayOfWeek.Thursday:
			week = "목요일";
			break;
		case DayOfWeek.Friday:
			week = "금요일";
			break;
		case DayOfWeek.Saturday:
			week = "토요일";
			break;
		case DayOfWeek.Sunday:
			week = "일요일";
			break;
		}
		date.text = "" + DateTime.Now.Month + "월 " + DateTime.Now.Day + "일 " + week;
	}
}
