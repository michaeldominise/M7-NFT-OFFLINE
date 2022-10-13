using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M7.CDN.Addressable
{
    [System.Serializable]
    public class AssetReferenceData<T> where T : Object
    {
        [SerializeField, ReadOnly] string assetName;
        public string AssetName { get { return assetName; } }

        [SerializeField, ReadOnly] string objectTypeName;
        [SerializeField, ReadOnly] string guid;
        [SerializeField, ReadOnly] string subObjectName;

#if UNITY_EDITOR
        [ShowInInspector]
        public T Asset
        {
            get
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileName(assetPath);
                if (fileName != assetName)
                {
                    var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GUIDToAssetPath(guid));
                    foreach (var subAsset in subAssets)
                    {
                        if (subAsset.name == assetName && subAsset.GetType() == typeof(T))
                            return subAsset as T;
                    }
                }
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
        }

        public AssetReferenceData(Object asset) : this(asset == null ? "" : AssetDatabase.GetAssetPath(asset), asset?.name, asset?.GetType()) { }
        public AssetReferenceData(string assetPath, string assetName, System.Type objectType) : this(AssetDatabase.GUIDFromAssetPath(assetPath).ToString(), Path.GetFileNameWithoutExtension(assetPath), assetName, objectType) { }
#endif
        public AssetReferenceData(string guid, string assetName, string subObjectName, System.Type objectType)
        {
            this.assetName = assetName;
            this.subObjectName = assetName == subObjectName ? "" : subObjectName;
            this.guid = guid;
            SetObjectType(objectType);
        }

        void SetObjectType(System.Type objectType)
        {
            if (objectType == null)
                return;

            using var stringWriter = new StringWriter();
            stringWriter.Write("<{0}", objectType.FullName);
            while (objectType.BaseType != null && objectType.BaseType != objectType)
            {
                stringWriter.Write("<{0}", objectType.BaseType.FullName);
                objectType = objectType.BaseType;
            }
            objectTypeName = stringWriter.ToString();
        }

        public bool IsTypeOf(System.Type objectType) => objectTypeName.Contains(string.Format("<{0}", objectType.FullName));

        public AssetReferenceT<ObjectType> GetAssetReference<ObjectType>() where ObjectType : T
        {
            var assetRef = new AssetReferenceT<ObjectType>(guid);
            assetRef.SubObjectName = subObjectName;
            return assetRef;
        }
    }
}