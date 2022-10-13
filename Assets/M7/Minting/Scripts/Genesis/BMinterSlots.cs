using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Lean.Transition.Method;

public class BMinterSlots : MonoBehaviour
{
	/*
	[HideInInspector]
	public LeanRectTransformAnchoredPosition targetLean;
	Image slotImg;
	
	public Color GreenColorFill = Color.green;
	public Color RedColorFill  = Color.red;
	public Color WhitelColorFill  = Color.white;
	
	public void Start () 
	{
		targetLean = this.GetComponent<LeanRectTransformAnchoredPosition>();
		targetLean.Data.Target = null;

		slotImg = this.GetComponent<Image>();
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (targetLean.Data.Target != null)
		{
			slotImg.color = RedColorFill;
			return;
		}
		targetLean.Data.Target = other.GetComponent<RectTransform>();
		targetLean.BeginThisTransition();
		slotImg.color = GreenColorFill;
		
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if (targetLean.Data.Target.name == other.name)
		{
			targetLean.Data.Target = null;
			slotImg.color = WhitelColorFill;
		}
		else 
		{
			slotImg.color = GreenColorFill;
		}
	
	}
	*/
}
