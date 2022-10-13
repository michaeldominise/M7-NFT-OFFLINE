using M7.GameData;
using M7.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using M7.ServerTestScripts;
using M7.PlayfabCloudscript.ServerTime;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Linq;
using M7.CDN.Addressable;
using UnityEditor;

namespace M7.GameRuntime
{
    [Serializable]
    public class AddHeroNftManager
    {
        public static AddHeroNftManager Instance => GameManager.Instance.AddHeroNftManager;

        [SerializeField, ReadOnly] AssetReferenceT<CharacterObject>[] charaterList;
#if UNITY_EDITOR
        [ShowInInspector]
        public CharacterObject[] Assets
        {
            get => charaterList.Select(assetReferene => assetReferene.editorAsset).ToArray();
            set
            {
                charaterList = value.Select(obj
                    => new AssetReferenceT<CharacterObject>(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(obj)).ToString())).ToArray();
                AddressableAssetHelper.MarkAssetAsAddressable(value, true);
            }
        }
#endif

        public IEnumerator GenerateHero(string instanceId, Action<SaveableCharacterData> onFinish)
        {
            var rnd = UnityEngine.Random.Range(0, charaterList.Length);
            CharacterObject charObj = null;
            charaterList[rnd].LoadAssetAsync(result => charObj = result);
            yield return new WaitWhile(() => charObj == null);

            var headgear = charObj.Equipments.headgears.Length > 0 ? charObj.Equipments.headgears[UnityEngine.Random.Range(0, charObj.Equipments.headgears.Length)].AssetName : "";
            var accessory = charObj.Equipments.accessories.Length > 0 ? charObj.Equipments.accessories[UnityEngine.Random.Range(0, charObj.Equipments.accessories.Length)].AssetName : "";
            var armor = charObj.Equipments.armors.Length > 0 ? charObj.Equipments.armors[UnityEngine.Random.Range(0, charObj.Equipments.armors.Length)].AssetName : "";
            var gloves = charObj.Equipments.gloves.Length > 0 ? charObj.Equipments.gloves[UnityEngine.Random.Range(0, charObj.Equipments.gloves.Length)].AssetName : "";
            var weapon = charObj.Equipments.weapon.Length > 0 ? charObj.Equipments.weapon[UnityEngine.Random.Range(0, charObj.Equipments.weapon.Length)].AssetName : "";
            var shoes = charObj.Equipments.shoes.Length > 0 ? charObj.Equipments.shoes[UnityEngine.Random.Range(0, charObj.Equipments.shoes.Length)].AssetName : "";
            var saveableCharacter = new SaveableCharacterData(charObj.MasterID, instanceId, 0, new string[] { headgear, accessory, armor, gloves, weapon, shoes });

            float hp = UnityEngine.Random.Range(0, 15);
            float attack = UnityEngine.Random.Range(0, 15);
            float defense = UnityEngine.Random.Range(0, 15);
            float passion = UnityEngine.Random.Range(0, 15);
            float luck = UnityEngine.Random.Range(0, 15);
            float durability = 100;

            saveableCharacter.BaseStats.AddValues(ref hp, ref attack, ref defense, ref passion, ref luck, ref durability);
            saveableCharacter.SaveableStats.AddValues(ref hp, ref attack, ref defense, ref passion, ref luck, ref durability);

            onFinish?.Invoke(saveableCharacter);
            Addressables.Release(charObj);
        }
    }
}