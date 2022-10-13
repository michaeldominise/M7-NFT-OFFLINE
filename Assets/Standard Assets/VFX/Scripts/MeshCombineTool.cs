using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public class MeshCombineTool : MonoBehaviour
{
    ArrayList materials;
    ArrayList combineInstanceArrays;
    MeshFilter[] meshFilters;
    MeshFilter meshFilterCombine;
    MeshRenderer meshRendererCombine;
    Mesh[] meshes;
    CombineInstance[] combineInstances;

    public bool CombineDone
    {
        get; set;
    }

    private void Start()
    {
        CombineDone = false;
        StartCoroutine(MeshCombine());
    }

    IEnumerator MeshCombine()
    {
        materials = new ArrayList();
        combineInstanceArrays = new ArrayList();
        meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        
        // Get / Create mesh filter & renderer
        meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }

        yield return new WaitForEndOfFrame();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        yield return new WaitForEndOfFrame();

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        
        // Cast Shadow Off
        meshRendererCombine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // Disable Children
        for (int i = transform.childCount - 1; i >= 0; i--)
            transform.GetChild(i).gameObject.SetActive(false);

        // Set to origin
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        yield return new WaitForEndOfFrame();

        // Assign respective materials
        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        meshes = new Mesh[materials.Count];
        combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        yield return new WaitForEndOfFrame();

        // Assign materials
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        CombineDone = true;
        StopAllCoroutines();
    }

    public void DestroyChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
}