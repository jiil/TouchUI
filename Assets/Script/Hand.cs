using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Hand : MonoBehaviour
{
	GameObject heldObject;

	bool trigging;
	public XRNode nodeType;
	public float GrabDistance = 0.1f;
	public string trigger;
	public bool showRayCast = false;

	public Transform pointingBase;
	public Transform indexBase;

	private Animator _anim;
	RaycastHit oldHit;

	int buttonLayerMask = 1 << 5; // UI
	void Start()
	{
		XRDevice.SetTrackingSpaceType (TrackingSpaceType.RoomScale);
		_anim = GetComponentInChildren<Animator> ();
		GetComponent<LineRenderer> ().enabled = false;
	}

	void Update()
	{
		//컨트롤러 트레킹 
		transform.localPosition = InputTracking.GetLocalPosition (nodeType);
		transform.localRotation = InputTracking.GetLocalRotation (nodeType);

		//핸드(포인팅) 콜라이더 
		Collider[] handCols = Physics.OverlapCapsule (transform.position + transform.forward * 0.15f, transform.position + transform.forward * 0.5f, 0.25f, buttonLayerMask);

		// 트리거 체크
		trigging = (Input.GetAxis (trigger) >= 0.1f);

		// 그랩 콜라이더 
		Collider[] cols = Physics.OverlapSphere (transform.position, GrabDistance);

		foreach (Collider col in cols) {
			if(col.GetComponent<HeldObject> () && !heldObject && trigging){ // 잡기 오브젝트 
				heldObject = col.gameObject;
				heldObject.GetComponent<HeldObject> ().heldBase = transform;
				heldObject.GetComponent<HeldObject> ().PickUp ();
			}
		}

		if (trigging) {
			handGrab ();
		} else {
			if (handCols.Length == 0) {
				handDefault ();
			} else {
				handPointing ();
			}
		}
	}

	void handGrab(){
		if (_anim.GetInteger ("handState") != 2) { // 잡기
			_anim.SetInteger ("handState", 2);
			GetComponent<LineRenderer> ().enabled = false;
			if (oldHit.collider != null) {
				oldHit.transform.GetComponent<TouchButtonObject> ().hoverRelease ();
			}
		}
		GetComponentInChildren<Light> ().enabled = false;
	}

	void handDefault(){
		if (_anim.GetInteger ("handState") != 0) {
			_anim.SetInteger ("handState", 0);
			GetComponent<LineRenderer> ().enabled = false;
		}
		if (heldObject != null) {
			heldObject.GetComponent<HeldObject> ().Drop ();
			heldObject = null;
		}
		GetComponentInChildren<Light> ().enabled = false;
	}

	void handPointing(){
		// 손모양 포인팅
		if (_anim.GetInteger ("handState") != 1) {
			_anim.SetInteger ("handState", 1);
		}
		if (heldObject != null) {
			heldObject.GetComponent<HeldObject> ().Drop ();
			heldObject = null;
		}
		//레이캐스트 - 버튼 호버
		Ray ray = new Ray(pointingBase.position, transform.forward);

		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 0.5f, buttonLayerMask)) {
			if (oldHit.collider == null){
				hit.transform.GetComponent<TouchButtonObject> ().hoverBase = pointingBase;
				hit.transform.GetComponent<TouchButtonObject> ().hover ();
			}else if (!hit.collider.name.Equals(oldHit.collider.name)) {
				hit.transform.GetComponent<TouchButtonObject> ().hoverBase = pointingBase;
				hit.transform.GetComponent<TouchButtonObject> ().hover ();
				oldHit.transform.GetComponent<TouchButtonObject> ().hoverRelease ();
			}
		} else {
			if (oldHit.collider != null) {
				oldHit.transform.GetComponent<TouchButtonObject> ().hoverRelease ();
			}
		}
		oldHit = hit;
		float dist = Vector3.Distance (pointingBase.position, hit.point);
		float length = 0.5f;
		if (dist < 0.5f) {
			length = dist;
		} 

		//레이캐스트 시뮬레이션 
		Vector3[] positions = new Vector3[2];
		positions [0] = pointingBase.position;
		positions [1] = pointingBase.position + transform.forward * length;
		GetComponent<LineRenderer> ().SetPositions (positions);
		GetComponent<LineRenderer> ().enabled = showRayCast;
		GetComponentInChildren<Light> ().enabled = true;

		indexBase.position = pointingBase.position + transform.forward * -0.01f;

		Collider[] cols = Physics.OverlapSphere (indexBase.position, 0.01f, buttonLayerMask);
		foreach (Collider col in cols) {
			if (col.GetComponent<TouchButtonObject> ()) {
				col.GetComponent<TouchButtonObject> ().touchBase = indexBase;
				col.GetComponent<TouchButtonObject> ().Touch ();
			}
		}
	}
}

