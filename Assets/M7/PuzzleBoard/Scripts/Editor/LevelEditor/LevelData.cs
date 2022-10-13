// NOTE: Commented out this whole script because it conflicts with "Level Select Level Data"

// using UnityEngine;

// using System.Collections;
// using System.Collections.Generic;

// using Sirenix.OdinInspector;

// using BGGamesCore.Utilities;

// public class LevelData : ScriptableObject
// {
//     [ReadOnly, HorizontalGroup("UID")]
//     [SerializeField] private ushort m_Id = 0;

//     [SerializeField] private string m_LevelDataTitle = "";
    
//     [SerializeField] private LevelData[] m_WaveSetDataList = new LevelData[0];

//     public ushort Id { get { return m_Id; } }

//     public string LevelDataTitle { get { return m_LevelDataTitle; } }

//     public LevelData[] WaveSetDataList { get { return m_WaveSetDataList; } }

//     [HorizontalGroup("UID")]
//     [Button("Generate")]
//     public void GenerateUniqueId()
//     {
//          m_Id = BGUtilities.GenerateUniqueGUID();
//     }

//     // public string Name;

//     // [InlineEditor(InlineEditorModes.LargePreview)]
//     // public GameObject nodePrefab;


//     //[HorizontalGroup("Split", 290), EnumToggleButtons, HideLabel]
//     //public CharacterAlignment CharacterAlignment;

//     //[TabGroup("Starting Inventory")]
//     //public ItemSlot[,] Inventory = new ItemSlot[12, 6];

//     //[TabGroup("Starting Stats"), HideLabel]
//     //public CharacterStats Skills = new CharacterStats();

//     //[HideLabel]
//     //[TabGroup("Starting Equipment")]
//     //public CharacterEquipment StartingEquipment;
// }
