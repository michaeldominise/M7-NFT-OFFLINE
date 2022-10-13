using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using PathologicalGames;
using Sirenix.OdinInspector;

namespace M7.GameData
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EnvironmentItem : MonoBehaviour
    {
        [SerializeField, HideInInspector] Texture mainTexture;
        [SerializeField, HideInInspector] Texture subTexture;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Material material;
        [SerializeField] float offsetSpeed;

        const string MainTex = "_MainTex";
        const string SubTex = "_SubTex";

        internal void UpdateBlend(object waveProgres)
        {
            throw new NotImplementedException();
        }

        const string Blend = "_Blend";
#if UNITY_EDITOR

        [ShowInInspector, PropertyOrder(-1)] Texture MainTexture
        {
            get => mainTexture;
            set
            {
                mainTexture = value;
                InitRendererMaterial();
            }
        }

        [ShowInInspector, PropertyOrder(-1)] Texture SubTexture
        {
            get => subTexture;
            set
            {
                subTexture = value;
                InitRendererMaterial();
            }
        }

        [Button]
        public void UpdateHeightByWidth() => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x * (mainTexture ? (float)mainTexture.height / mainTexture.width : 1), 1);
        [Button]
        public void UpdateWidthByHeight() => transform.localScale = new Vector3(transform.localScale.y * (mainTexture ? (float)mainTexture.width / mainTexture.height : 1), transform.localScale.y, 1);
#endif

        public void InitRendererMaterial()
        {
            DestroyImmediate(meshRenderer.material);
            meshRenderer.sharedMaterial = new Material(material);
            meshRenderer.sharedMaterial.SetTexture(MainTex, mainTexture);
            meshRenderer.sharedMaterial.SetTexture(SubTex, subTexture);
        }

        public void UpdateOffset(float cameraXPos) => meshRenderer.material.SetTextureOffset(MainTex, Vector2.right * cameraXPos * offsetSpeed);
        public void UpdateBlend(float blend) => meshRenderer.material.SetFloat(Blend, blend);
    }
}