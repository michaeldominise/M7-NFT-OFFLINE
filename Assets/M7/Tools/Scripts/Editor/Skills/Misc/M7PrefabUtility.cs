using M7.Skill;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class M7PrefabUtility : MonoBehaviour
{
    public static void RemoveAllChildrenWithName(string prefabPath, string name)
    {
        using(var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;

            for(int i = prefabRoot.transform.childCount - 1; i >= 0; i--)
            {
                if (prefabRoot.transform.GetChild(i).gameObject.name == name)
                {
                    DestroyImmediate(prefabRoot.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public static void AddOrReplaceChild(GameObject objectToAdd, string prefabPath, string tag, Action<GameObject, GameObject> onChildAdd = null, Action onPostPrefabMode = null)
    {
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;

            foreach(Transform child in prefabRoot.transform)
            {
                if(InspectorTag.GetTag(child.gameObject.name) == tag)
                {
                    child.gameObject.name = "todelete";
                }
            }

            var newTargetObject = PrefabUtility.InstantiatePrefab(objectToAdd, prefabRoot.transform) as GameObject;
            PrefabUtility.UnpackPrefabInstance(newTargetObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            onChildAdd?.Invoke(prefabRoot, newTargetObject);

            foreach (Transform child in prefabRoot.transform)
            {
                if (child.gameObject.name == "todelete")
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        onPostPrefabMode?.Invoke();
    }

    public static void AddOrReplaceChild(GameObject objectToAdd, string prefabPath, string parentName, string toReplaceName, Action<GameObject, GameObject> onChildAdd = null)
    {
        using(var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var parent = prefabRoot.transform.FindRecursive(parentName);

            if (parent != null)
            {
                for (int i = parent.transform.childCount - 1; i >= 0; i--)
                {
                    if (parent.transform.GetChild(i).gameObject.name == toReplaceName)
                    {
                        DestroyImmediate(parent.transform.GetChild(i).gameObject);
                    }
                }
            }

            var newObject = PrefabUtility.InstantiatePrefab(objectToAdd, parent) as GameObject;
            PrefabUtility.UnpackPrefabInstance(newObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            newObject.name = toReplaceName;

            onChildAdd?.Invoke(prefabRoot, newObject);
            
        }
    }

    /// <summary>
    /// Somehow, the references inside the prefab scope is different from outside
    /// Thus, we get the appropriate child index from outside
    /// then delete the same child index inside the prefab
    /// 
    /// TODO: Improve this workaround
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectToRemove"></param>
    /// <param name="prefabPath"></param>
    /// <param name="parentPath"></param>
    public static void RemoveChildWithComponent<T>(T objectToRemove, string prefabPath, string parentPath) where T : class?
    {
        int childIndex = GetChildIndex(objectToRemove, prefabPath, parentPath);

        if(childIndex == -1)
        {
            EditorUtility.DisplayDialog("Error", "No object found.", "OK");
            return;
        }

        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var parentTransform = prefabRoot.transform.Find(parentPath);
            DestroyImmediate(parentTransform.GetChild(childIndex).gameObject);
        }
    }

    private static int GetChildIndex<T>(T objectToRemove, string prefabPath, string parentPath) where T : class?
    {
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var parentTransform = go.transform.Find(parentPath);
        int childIndex = -1;
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            var o1 = (parentTransform.GetChild(i).GetComponent<T>() as Component);
            var o2 = (objectToRemove as Component);
            Debug.Log($"{o1} {o2} {o1 == o2}");
            if (o1 == o2)
            {
                childIndex = i;
                break;
            }
        }
        return childIndex;
    }

    //public static void AddOrReplaceChild(GameObject objectToAdd, string prefabPath, string objectName, Action<GameObject, GameObject> onChildAdd = null)
    //{
    //    using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
    //    {
    //        var prefabRoot = editingScope.prefabContentsRoot;

    //        var inspectorParent = prefabRoot.transform.Find(objectName);

    //        if (inspectorParent == null)
    //        {
    //            var inspectorObject = new GameObject();
    //            inspectorObject.transform.SetParent(prefabRoot.transform);
    //            inspectorObject.name = objectName;

    //            inspectorParent = inspectorObject.transform;
    //        }

    //        if (inspectorParent.transform.childCount > 0)
    //        {
    //            foreach (Transform child in inspectorParent.transform)
    //            {
    //                DestroyImmediate(child.gameObject);
    //            }
    //        }

    //        var newTargetObject = PrefabUtility.InstantiatePrefab(objectToAdd, inspectorParent.transform) as GameObject;
    //        PrefabUtility.UnpackPrefabInstance(newTargetObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

    //        onChildAdd?.Invoke(prefabRoot, newTargetObject);
    //    }
    //}

    public static void RemoveChild(string prefabPath, string tagName)
    {
        using(var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var child = prefabRoot.transform.Find(tagName);
            if(child != null)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public static void RemoveChildWithNamePrefix(string prefabPath, string prefix)
    {
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;

            foreach(Transform child in prefabRoot.transform)
            {
                if(child.gameObject.name.StartsWith(prefix))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    public static void RemoveChildWithNameContain(string prefabPath, string nameContains, Action<GameObject> onChildRemoved = null)
    {
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var childToRemove = prefabRoot.transform.FindRecursive(c => c.name.Contains(nameContains));
            if(childToRemove != null)
            {
                DestroyImmediate(childToRemove.gameObject);
            }

            onChildRemoved?.Invoke(prefabRoot);
        }
    }


    public static void AppendObject(string prefabPath, string parentPath, GameObject objectToAdd, Action<GameObject, GameObject> onObjectAppended = null)
    {
        using(var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var parentTransform = prefabRoot.transform.Find(parentPath);

            var newObject = PrefabUtility.InstantiatePrefab(objectToAdd, parentTransform) as GameObject;
            PrefabUtility.UnpackPrefabInstance(newObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            onObjectAppended?.Invoke(prefabRoot, newObject);
        }
    }

    public static void AppendObjectToParentWithComponent<T>(GameObject initialParent, GameObject objectToAdd, Action<GameObject, GameObject> onObjectAppended = null) where T : Component
    {
        Debug.Log(initialParent.transform.parent.parent);
        GameObject trueParent = initialParent.GetComponentInParent<T>().gameObject;
        string assetPath = AssetDatabase.GetAssetPath(trueParent);

        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            var parentInPrefab = prefabRoot.transform.Find(initialParent.name);
            var newObject = PrefabUtility.InstantiatePrefab(objectToAdd, parentInPrefab.transform) as GameObject;
            PrefabUtility.UnpackPrefabInstance(newObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            onObjectAppended?.Invoke(initialParent, newObject);
        }

        
    }

    public static void OpenPrefab(string prefabPath, Action<GameObject> onObjectOpen = null)
    {
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;
            onObjectOpen?.Invoke(prefabRoot);
        }
    }

    public static string CreateNew(GameObject go, string name, string savePath)
    {
        string path = Path.Combine(savePath, name);
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.AutomatedAction);

        return path;
    }

    public static void SaveAsAnotherPrefab(GameObject go, string name, string savePath)
    {
        //PrefabUtility.sav
    }

    public static GameObject AddChild(GameObject PrefabAsset, GameObject ObjecttoAdd, string NewObjectsName)
    {
        var objectToAddInstance = PrefabUtility.InstantiatePrefab(ObjecttoAdd) as GameObject;
        objectToAddInstance.name = NewObjectsName;
        

        var Contents = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(PrefabAsset));

        objectToAddInstance.transform.parent = Contents.transform;
        PrefabUtility.UnpackPrefabInstance(objectToAddInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

        PrefabUtility.SaveAsPrefabAsset(Contents, AssetDatabase.GetAssetPath(PrefabAsset));
        PrefabUtility.UnloadPrefabContents(Contents);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return objectToAddInstance;
    }

    public static void AddChild(GameObject PrefabAsset, GameObject ObjecttoAdd, string NewObjectsName, Action<GameObject> onChildAdd)
    {
        var Contents = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(PrefabAsset));

        var objectToAddInstance = PrefabUtility.InstantiatePrefab(ObjecttoAdd) as GameObject;
        objectToAddInstance.name = NewObjectsName;

        objectToAddInstance.transform.parent = Contents.transform;
        PrefabUtility.UnpackPrefabInstance(objectToAddInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

        onChildAdd?.Invoke(objectToAddInstance);

        PrefabUtility.SaveAsPrefabAsset(Contents, AssetDatabase.GetAssetPath(PrefabAsset));
        PrefabUtility.UnloadPrefabContents(Contents);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void RemoveChild(GameObject PrefabAsset, string ChildsName, bool RemoveAllChildrenWithName)
    {
        GameObject Contents = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(PrefabAsset));
        bool FoundChild = false;
        for (int i = 1; i < Contents.transform.childCount + 1; i++)
        {
            if (Contents.transform.GetChild(i - 1).gameObject.name == ChildsName)
            {
                FoundChild = true;
                DestroyImmediate(Contents.transform.GetChild(i - 1).gameObject);
            }
        }

        if (!FoundChild)
        {
            Debug.LogError("There were no children found with that name on the prefab, please recheck your spelling");
        }
        PrefabUtility.SaveAsPrefabAsset(Contents, AssetDatabase.GetAssetPath(PrefabAsset));
        PrefabUtility.UnloadPrefabContents(Contents);
    }
}
