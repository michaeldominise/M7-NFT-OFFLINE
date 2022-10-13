using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDamageFX : MonoBehaviour
{
    [SerializeField] RectTransform _hP, _hPDmg;
    float damaged = 0;

    // Start is called before the first frame update
    void OnDisable()
    {
        StopCoroutine(AdjustHP());
    }
    public void SetHP()
    {
        if (_hP != null && _hPDmg != null)
        {
            _hPDmg.anchorMax = _hP.anchorMax;
            _hPDmg.GetComponent<Image>().sprite = _hP.GetComponent<Image>().sprite;
        }
    }

    public void OnDamaged()
    {
        damaged++;
        if (_hP != null && _hPDmg != null)
            if (damaged > 0)
                StartCoroutine(AdjustHP());
    }

    IEnumerator AdjustHP()
    {
        while(damaged >= 0)
        {
            damaged -= Time.deltaTime / 2;
            yield return null;
        }

        yield return new WaitForSeconds(.5F);
        float duration = 0;
        while (duration < 1)
        {
            duration += Time.deltaTime * 5;
            _hPDmg.anchorMax = Vector2.Lerp(_hPDmg.anchorMax, _hP.anchorMax, duration);
            //Debug.LogError("HP Down!");
            yield return null;
        }
        StopCoroutine(AdjustHP());
    }
}
