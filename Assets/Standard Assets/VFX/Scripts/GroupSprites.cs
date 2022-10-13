using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GroupSprites : MonoBehaviour
{
    [Range(0, 1)]
    public float _alpha = 1;

    SpriteRenderer[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var obj in sprites)
        {
            Color color = obj.material.GetColor("_Color");
            if (color != null)
            {
                color.a = _alpha;
                obj.material.SetColor("_Color", color);
            }
        }
    }
}
