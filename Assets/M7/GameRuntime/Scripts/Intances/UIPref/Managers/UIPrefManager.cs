using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public abstract class UIPrefManager<UIPrefManagerType, UIBattleType, StatsInstanceActionsType, TargetRefernceType> : MonoBehaviour where UIPrefManagerType : MonoBehaviour where UIBattleType : UIBattle<UIPrefManagerType, TargetRefernceType, StatsInstanceActionsType> where StatsInstanceActionsType : InstanceActions where TargetRefernceType : Object
    {
        [SerializeField] protected UIBattleType uiPref;
        [SerializeField] protected Transform container;
        [SerializeField] protected List<UIBattleType> uiBattleList = new List<UIBattleType>();

        public List<UIBattleType> UIBattleList => uiBattleList;

        public virtual UIBattleType CreateUI(TargetRefernceType targetReference)
        {
            var uiBattle = Instantiate(uiPref, container, false);
            uiBattle.name = $"{uiPref.name} {targetReference.name}";
            uiBattle.Attach(this as UIPrefManagerType, targetReference);

            uiBattleList.Add(uiBattle);
            return uiBattle;
        }

        public virtual void DestroyItem(UIBattleType item, float delay = 0, bool removeInUIBattleList = true)
        {
            if(removeInUIBattleList)
                uiBattleList.Remove(item);

            if(item != null)
                Destroy(item.gameObject, delay);
        }
    }
}
