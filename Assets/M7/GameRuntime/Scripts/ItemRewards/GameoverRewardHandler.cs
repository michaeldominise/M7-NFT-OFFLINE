using System.Collections;
using UnityEngine;

public class GameoverRewardHandler : MonoBehaviour
{
    public ChipData chipData;
    public RewardChip ChipPrefab; 

   public IEnumerator  Init()
   {
       yield return new WaitForSeconds(2.3f);
       // This is just a dummy reward chip spawn. May change in future
       //for (int i = 0; i < 2; i++)
       //{
       //    var Item = Instantiate(ChipPrefab, transform);
       //    Item.gameObject.SetActive(true);
       //    Item.transform.localScale = Vector3.zero;
       //    Item.transform.localScaleTransition(Vector3.one, 1 , LeanEase.BounceIn);

       //}
       
   }
}
