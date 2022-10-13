using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class VFX_ScreenBounce : MonoBehaviour
{
    [SerializeField] Button _ADButton;
    [SerializeField] float floatDuration = 1;

    [Range(.1F, 1)]
    [SerializeField] float screenOffset = 1;

    const float initialWidth = 1080, initialHeight = 1920; //reference screen resolution

    void Awake()
    {
        gameObject.SetActive(false);
        _ADButton?.onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        Spawn();
    }

    public void Spawn()
    {
        DOTween.Kill(this);
        
        Vector2 screenSize = new Vector2(initialWidth, initialHeight) / 2;
        Vector2 randomPos = Random.insideUnitSphere * screenSize * screenOffset;
        transform.localPosition = randomPos;
        transform.localScale = Vector2.zero;

        Scale(Vector2.one, Ease.OutElastic);
    }

    public void OnClick()
    {
        Scale(Vector2.zero, Ease.InOutElastic, true);
    }

    void Scale(Vector2 scale, Ease setEase, bool isEnd = false)
    {
        transform.DOScale(scale, .3F).SetEase(setEase).OnComplete(delegate ()
        {
            if (isEnd)
            {
                DOTween.Kill(this);
                gameObject.SetActive(false);
            }
            else
            {
                MoveObject();
            }
        });
    }

    void MoveObject()
    {
        transform.DOLocalPath(Waypoints(), floatDuration).SetEase(Ease.Linear).SetOptions(true).OnComplete(delegate ()
        {
            Scale(Vector2.zero, Ease.InOutElastic, true);
        });
    }

    Vector3[] Waypoints()
    {
        Vector3[] points = new Vector3[6];
        int i = 0;

        while (i < points.Length)
        {
            int r = Random.Range(0, points.Length);
            if (!points.Contains(RandomPoint(r)))
            {
                points.SetValue(RandomPoint(r), i);
                i++;
            }
        }

        return points;
    }

    Vector3 RandomPoint(int arrayValue)
    {
        float randomWidth = Random.Range(0, initialWidth) / 2;
        float randomHeight = Random.Range(0, initialHeight) / 2;

        Vector3 point = new Vector3();
        switch (arrayValue)
        {
            case 0:
                point = new Vector2(-initialWidth / 2, randomHeight) * screenOffset; //top left
                break;
            case 1:
                point = new Vector2(Random.Range(-randomWidth, randomWidth), initialHeight / 2) * screenOffset; //top
                break;
            case 2:
                point = new Vector2(initialWidth / 2, randomHeight) * screenOffset; //top right
                break;
            case 3:
                point = new Vector2(-initialWidth / 2, -randomHeight) * screenOffset; //bottom left
                break;
            case 4:
                point = new Vector2(Random.Range(-randomWidth, randomWidth), -initialHeight / 2) * (screenOffset * .8F); //bottom
                break;
            case 5:
                point = new Vector2(initialWidth / 2, -randomHeight) * screenOffset; //bottom right 
                break;
        }

        return point;
    }
}
