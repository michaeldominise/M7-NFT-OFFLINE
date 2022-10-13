using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

namespace M7.SFXRuntime
{
	[System.Serializable]
	public class SFX_Trigger
	{
		public string animationName;
		public string audioName;
	}
	
	public class SFX_Character : MonoBehaviour
	{
		[SerializeField] Animator animator;
		[SerializeField] List <SFX_Trigger> sfx_trigger = new List<SFX_Trigger>(); 
		[SerializeField] bool state;
	    // Update is called once per frame

	    //void Update()
	    //{
		   // for (int i = 0 ; i < sfx_trigger.Count; i++)
		   // {
			  //  if (isPlayingAnimation(sfx_trigger[i].animationName))
			  //  {
				 //   //Debug.Log(sfx_trigger[i].animationName);
				 //   MasterAudio.PlaySoundAndForget(sfx_trigger[i].audioName);
			  //  }
			  //  if (isPlayingAnimation ("Idle"))
			  //  {
					//MasterAudio.StopBus("FootStepFX");
			  //  }
		   // }
	    //}
	    
		bool isPlayingAnimation (string stateName)
		{
			if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
				animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
				return true;
			else
				return false;
		}
	}
}
