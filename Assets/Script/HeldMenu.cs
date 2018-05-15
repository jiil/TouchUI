using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeldObject))]
public class HeldMenu : MonoBehaviour {
	public Transform menuBase;

	HeldObject heldObject;
	// Use this for initialization
	void Start () {
		heldObject = GetComponent<HeldObject> ();
	}

	public void Drop(){
		menuBase.parent = null;
		heldObject.heldBase = null;	
	}

	public void PickUp(){
		menuBase.parent = heldObject.heldBase;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
