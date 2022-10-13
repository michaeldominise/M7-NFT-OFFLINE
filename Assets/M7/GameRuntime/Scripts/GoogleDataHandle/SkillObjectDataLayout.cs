#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;
namespace M7.ToolDownload
{
    public class SkillObjectDataLayout : ContentBaseLayout
    {
        [DetailedInfoBox("Warning! ....", "Open the dropdown to see the table, Double check the contents before uploading.\nCheck if spread sheet is insync with your changes and please switch to the desired server before uploading")]
        [TableList(ShowPaging = true)]
        public SkillOjectCapsule[] SkillOjectCapsules = new SkillOjectCapsule[0];
        [DetailedInfoBox("Warning! ....", "Make sure to provide a reference SkillObject from which new ones will be derived")]
        public GameObject ReferenceSkillObject;
        public List<RPGElement> Elements = new List<RPGElement>();

    }
}
#endif
