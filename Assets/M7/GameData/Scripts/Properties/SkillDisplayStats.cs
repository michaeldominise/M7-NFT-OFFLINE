using M7.GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class SkillDisplayStats
    {
        [SerializeField] string displayName;
        [SerializeField] string description;
        [SerializeField] Sprite skill_Image;

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public Sprite skillimage
        {
            get => skill_Image;
            set => skill_Image = value;
        }
    }
}