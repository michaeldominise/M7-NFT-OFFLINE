using M7.GameData;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace M7.GameRuntime
{
    public class BaseTeamManager<T> : MonoBehaviour where T : BaseCharacterInstance
    {
        [SerializeField] public Transform[] containers;
        [SerializeField] protected T charactersInstancePref;
        [SerializeField] protected bool flip;
        [SerializeField] protected bool destroyImmediately = true;

        T[] rawCharacters { get; set; }
        public T[] RawCharacters
        {
            get
            {
                if (rawCharacters == null || rawCharacters.Length == 0)
                    rawCharacters = new T[containers.Length];
                return rawCharacters;
            }
            set
            {
                rawCharacters = value;
            }

        }
        T[] rawCharsCache { get; set; }

        [ShowInInspector, ReadOnly] public List<T> ActiveCharacters => RawCharacters.Where(x => x != null).ToList();
        public float TotalTeamAttack => ActiveCharacters.Sum(x => x.StatsInstance.Attack);
        public float TotalTeamHp => ActiveCharacters.Sum(x => x.StatsInstance.MaxHp);
        [ShowInInspector, ReadOnly] public WaveData WaveData { get; set; } = new WaveData_Player();

        public virtual void Init(WaveData waveData, System.Action onFinish)
        {
            if(WaveData != null)
                WaveData.onValuesChanged -= Refresh;

            WaveData = waveData;
            WaveData.onValuesChanged += Refresh;
            Init(waveData.SaveableCharacters, onFinish);
        }

        public virtual void Init(List<SaveableCharacterData> saveableCharacters, System.Action onFinish)
        {
            var finishCount = 0;
            for (var x = 0; x < saveableCharacters.Count; x++)
                SetCharacterPosition(x, saveableCharacters[x], () =>
                {
                    finishCount++;
                    if (finishCount == saveableCharacters.Count)
                    {
                        PostInit();
                        onFinish?.Invoke();
                    }
                });
        }

        public void Refresh() => Init(WaveData, null);

        public virtual T SetCharacterPosition(int index, SaveableCharacterData saveableCharacter, System.Action onFinish)
        {
            if (saveableCharacter == null || string.IsNullOrWhiteSpace(saveableCharacter.MasterID) || index > containers.Length || index < 0 || (RawCharacters[index] != null && RawCharacters[index].SaveableCharacterData == saveableCharacter))
            {
                if(saveableCharacter == null)
                {
                    rawCharsCache = rawCharacters;
                    ClearCharacter(index, destroyImmediately);
                    RawCharacters[index]?.CleanInstance();
                }

                onFinish?.Invoke();
                return null;
            }

            rawCharsCache = rawCharacters;
            ClearCharacter(index, destroyImmediately);
            var charInstance = Instantiate(charactersInstancePref, containers[Mathf.Min(index, containers.Length - 1)], false);
            charInstance.name = $"{charactersInstancePref.name} {saveableCharacter.MasterID}";
            charInstance.transform.localScale = flip ? Vector3.Scale(charInstance.transform.localScale, new Vector3(-1, 1, 1)) : charInstance.transform.localScale;
            charInstance.Init(saveableCharacter, onFinish);
            SetLayerRecursively(charInstance.gameObject, gameObject.layer);
            RawCharacters[index] = charInstance;
            return charInstance;
        }

        public virtual void ClearCharacter(int index, bool destroyImmediately)
        {
            if (RawCharacters[index] == null)
                return;

            if(destroyImmediately)
                Destroy(RawCharacters[index].gameObject);
            RawCharacters[index] = null;
        }

        //IEnumerator DestroyDeadCharacters(int index)
        //{
        //    yield return new WaitForSeconds(0f);
        //    Destroy(RawCharacters[index].gameObject);
        //}

        public virtual void PostInit() { }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.yellow;
            var radius = 0.5f;
            UnityEditor.Handles.color = Color.yellow;
            foreach (var container in containers)
                if(container.gameObject.activeInHierarchy)
                    Gizmos.DrawWireSphere(container.position + (Vector3.up * radius), radius);
        }
#endif

        public void SetLayerRecursively(GameObject obj, int layer)
        {
            if (!obj)
                return;
            obj.layer = layer;
            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }

        private void OnDestroy() => WaveData.onValuesChanged -= Refresh;
    }
}
