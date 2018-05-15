using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchButtonObject: MonoBehaviour {

	public AudioClip toggleSound;
	public AudioClip hoverSound;
	AudioSource aus;
	public float pitch;
	[HideInInspector]
	public Transform touchBase;
	[HideInInspector]
	public Transform hoverBase;

	public GameObject tooltipBase;

	public Transform pointA;
	public Transform pointB;
	float disconnectDistance = 0.01f;

	public UnityEvent HitA;
	public UnityEvent HitB;

	public UnityEvent ReleasedA;
	public UnityEvent ReleasedB;
	Quaternion cubeRotation = Quaternion.Euler(90f,0f,0f);

	bool isToggle = false;
	//bool isHover = false;

	int state = 0;
	int prevState = 0;
	int tooltipState = 0; // 0 hide, 1 hide -> show, 2 show , 3 show -> hide 
	float tooltipAlpha = 0f;

	float dist;
	Vector3 offset;
	Color upColor;
	Color downColor;

	void Start () {
		aus = GetComponent<AudioSource> ();
		dist = Vector3.Distance(pointA.position, pointB.position); 
		upColor = colorNow ();
		downColor = colorToggle ();

		//hideToolTip ();
	}

	void Update ()
	{
		if (touchBase != null) {// touched 
			transform.position = ClosestPointOnLine (touchBase.position) - offset;
			if(Vector3.Distance(GetComponent<Collider>().bounds.ClosestPoint(touchBase.position), touchBase.position) > disconnectDistance){
				Release ();
			}

		} else { // untouched 
			transform.position = Vector3.MoveTowards (transform.position, pointA.position, 0.01f);
		}

		// button color setting 
		float rate;
		float selfDist = Vector3.Distance (pointA.position, transform.position);
		Color buttonColor;
		if (selfDist < 0.001f) {
			if (hoverBase != null) {
				rate = Vector3.Distance(hoverBase.position, transform.position) / 0.25f;
				buttonColor = Color.Lerp (colorHover(), upColor, rate);
			} else {
				buttonColor = upColor;
			}
		} else {
			rate = Vector3.Distance(pointA.position, transform.position) / dist;
			buttonColor = Color.Lerp (upColor, downColor, rate);
		}
		// button color update
		GetComponent<MeshRenderer> ().material.color = buttonColor;

		// tooltip alpha update 
		if (hoverBase != null) { // hover
			if (1f > tooltipAlpha) { // 조금이라도 투명하면 
				updateToolTip (buttonColor);
				tooltipAlpha += Time.deltaTime * 2f;
			} else {
				tooltipAlpha = 1f;
			}
			tooltipBase.transform.Find ("cube").Rotate (cubeRotation * transform.forward * 3, Space.World);
		} else { // hover released 
			tooltipBase.transform.Find ("cube").GetComponent<MeshRenderer>().enabled = false;
			if (tooltipAlpha > 0f) { // 투명하지 않으면 
				updateToolTip (buttonColor);
				tooltipAlpha -= Time.deltaTime * 1.5f;
			} else {
				tooltipAlpha = 0f;
				hideToolTip ();
			}
		}

		// tooltip update 


		// trigger state change
		if (transform.position == pointA.position)
			state = 1;
		else if (transform.position == pointB.position)
			state = 2;
		else
			state = 0;

		// trigger execute
		if (state == 1 && prevState == 0) {
			//HitA.Invoke ();
			downColor = colorToggle ();
		} else if (state == 2 && prevState == 0) {
			aus.PlayOneShot (toggleSound);
			//HitB.Invoke ();
			isToggle = !isToggle;
			upColor = colorNow ();
			foreach (ParticleSystem com in  GetComponentsInChildren<ParticleSystem> ()) {
				com.Play ();
			}
		} else if (state == 0 && prevState == 1) {
			ReleasedA.Invoke ();
			aus.volume = 0.3f;
			aus.pitch = pitch;
			aus.PlayOneShot (hoverSound);
		}else if (state == 0 && prevState == 2)
			ReleasedB.Invoke ();
		
		prevState = state;
	}

	Color colorNow(){
		if (isToggle) {
			return Color.magenta;
		} else {
			return Color.cyan;
		}	
	}

	Color colorToggle(){
		if (isToggle) {
			return Color.cyan;
		} else {
			return Color.magenta;
		}	
	}

	Color colorHover(){
		if (isToggle) {
			return Color.red;
		} else {
			return Color.blue;
		}	
	}

	public void Touch ()
	{
		if(offset == Vector3.zero)
			offset = touchBase.position - transform.position;
		
	}

	public void Release ()
	{
		offset = Vector3.zero;
		touchBase = null;
	}

	public void hover (){
		//upColor = colorHover();
		//isHover = true;
		showToolTip();
	}

	public void hoverRelease(){
		hoverBase = null;
		//upColor = colorNow();
	}

	Vector3 ClosestPointOnLine (Vector3 point)
	{
		Vector3 va = pointA.position + offset;
		Vector3 vb = pointB.position + offset;

		Vector3 vVector1 = point - va; // va -> point

		Vector3 vVector2 = (vb - va).normalized; // va -> vb 방향 정규화 

		float t = Vector3.Dot(vVector2, vVector1); // va -> point 에서 va -> vb 방향 크기 추출 

		if (t <= 0) // va 보다 뒤 거나 va
			return va;

		if (t >= Vector3.Distance(va, vb)) // vb 를 넘어가거나 vb
			return vb;

		Vector3 vVector3 = vVector2 * t; // 방향 , 크기

		Vector3 vClosestPoint = va + vVector3; // va 로 부터의 방향과 크기

		return vClosestPoint;
	}

	void hideToolTip(){
		foreach(MeshRenderer rend  in tooltipBase.GetComponentsInChildren<MeshRenderer> ()){
			rend.enabled = false;
		}
	}

	void showToolTip(){		
		foreach(MeshRenderer rend  in tooltipBase.GetComponentsInChildren<MeshRenderer> ()){
			rend.enabled = true;
		}
	}

	void updateToolTip(Color col){
		Color co;
		co = col;
		co.a = tooltipAlpha;
		foreach(MeshRenderer rend  in tooltipBase.GetComponentsInChildren<MeshRenderer> ()){
			if (rend.transform.position != tooltipBase.transform.position) {
				//rend.material.color = co; //cube 
			} else {
				Color temp = rend.material.color;
				temp.a = tooltipAlpha;
				rend.material.color = temp;
			}
		}
		tooltipBase.GetComponentInChildren<TextMesh> ().color = colorHover();


		//Text.color = buttonColor;
		//PointCube.GetComponent<MeshRenderer> ().material.color = buttonColor;
		//PointCube.Rotate(cubeRotation*transform.forward * 3, Space.World); 
	}
}