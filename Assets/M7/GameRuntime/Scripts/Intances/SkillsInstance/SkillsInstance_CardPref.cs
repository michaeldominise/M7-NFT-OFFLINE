using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;

namespace M7.GameRuntime
{
    public class SkillsInstance_CardPref : MonoBehaviour
    {
        //public enum CardType { RegularCard, ManaCard, OverdriveCard }
        //public CardType cardType;

        public SkillObject SkillObject { get; set; }
        public CharacterInstance_Battle targetReference;
    }
}