using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace M7.GameRuntime
{
	public class BIncubatorItem : MonoBehaviour
	{
		public List <Sprite> incubatorImg = new List<Sprite>();
		public TextMeshProUGUI idIncubatorText;
		public TextMeshProUGUI nameIncubatorText;
		
		[Space (10)]
		public Transform trnCanister;
		public Transform trnCharacter;
	}
}
