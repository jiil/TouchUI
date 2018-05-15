using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;

public class HeldObject : MonoBehaviour
{
	[HideInInspector]
	public Transform heldBase;

	public UnityEvent pickUp;

	public UnityEvent drop;

	void Start(){

	}

	private void Update(){

	}

	public void PickUp ()
	{
		pickUp.Invoke();
		if (pickUp.GetPersistentEventCount() == 0)
		{
			DefaultPickUp();
		}
	}

	public void Drop ()
	{
		drop.Invoke();
		if (drop.GetPersistentEventCount() == 0)
		{
			DefaultDrop();
		}
		heldBase = null;
	}

	public void DefaultDrop ()
	{
		transform.parent = null;
		heldBase = null;
	}

	public void DefaultPickUp ()
	{
		transform.parent = heldBase;
	}
}