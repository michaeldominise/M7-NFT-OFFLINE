using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.Utilities;

public enum VFXCurrency
{
    Energy, Gaianite, Gems, EnhancementKit, PowerUp
}
public class VFX_DisplayCurrencyHUD : MonoBehaviour
{
    public static VFX_DisplayCurrencyHUD Instance
    {
        get; private set;
    }

    [System.Serializable]
    public struct CurrencyType
    {
        public Transform HUD;
        public Transform Icon;
        public Material Texture;
    }
    [SerializeField] VFX_RewardsAttractor FXCurrency;
    [SerializeField] float hidePosition, tween = 1;
    [SerializeField] CurrencyType EnergyHUD;
    [SerializeField] CurrencyType GaianiteHUD;
    [SerializeField] CurrencyType GemsHUD;
    [SerializeField] CurrencyType EnhancementKit;
    [SerializeField] CurrencyType PowUpChip;

    VFX_RewardsAttractor RewardsFX { get { return attractFX.GetComponent<VFX_RewardsAttractor>(); } }
    const float showPos = 0;
    ParticleSystemRenderer attractFX;

    void Awake()
    {
        Instance = this;
        Hide(VFXCurrency.Gaianite.ToString());
        Hide(VFXCurrency.Energy.ToString());
        Hide(VFXCurrency.Gems.ToString());
        Hide(VFXCurrency.EnhancementKit.ToString());
        Hide(VFXCurrency.PowerUp.ToString());
        FXCurrency.transform.SetParent(transform.parent, false);
    }

    public void Show(string type, int amount)
    {
        attractFX = FX(type, amount);

        HUD(type).gameObject.SetActive(true);
        HUD(type).DOLocalMoveY(showPos, tween).SetEase(Ease.OutBounce).OnComplete(delegate ()
        {
            StartCoroutine(HideHUD(type));
        });        
    }

    IEnumerator HideHUD(string type)
    {
        attractFX.gameObject.SetActive(true);

        yield return new WaitUntil(() => RewardsFX.isDone);
        Hide(type);
        StopAllCoroutines();
    }
    
    void Hide(string type)
    {
        HUD(type).DOLocalMoveY(hidePosition, tween).SetEase(Ease.Flash).OnComplete(delegate ()
        {
            HUD(type).gameObject.SetActive(false);

        });
    }

    private Transform HUD(string hudName)
    {
        Transform hud = null;
        switch(hudName)
        {
            case "Energy":
                if (EnergyHUD.HUD != null)
                    hud = EnergyHUD.HUD;
                break;

            case "Gaianite":
                if (GaianiteHUD.HUD != null)
                    hud = GaianiteHUD.HUD;
                break;

            case "Gems":
                if (GemsHUD.HUD != null)
                    hud = GemsHUD.HUD;
                break;
            
            case "EnhancementKit":
                if (EnhancementKit.HUD != null)
                    hud = EnhancementKit.HUD;
                break;
            
            case "PowerUp":
                if (PowUpChip.HUD != null)
                    hud = PowUpChip.HUD;
                break;
            
            default:
                hud = null;
                break;
        }
        return hud;
    }

    private ParticleSystemRenderer FX(string fxType, int amount)
    {           
        ParticleSystemRenderer fxRender = FXObject().GetComponent<ParticleSystemRenderer>();
        VFX_RewardsAttractor fxTarget = fxRender.GetComponent<VFX_RewardsAttractor>();

        if (fxTarget != null)
        {
            fxTarget.endTarget = Icon(fxType);
            fxTarget.MaxParticles = amount;
        }

        switch (fxType)
        {
            case "Energy":
                if (EnergyHUD.Texture != null)
                    fxRender.material = EnergyHUD.Texture;
                break;

            case "Gaianite":
                if (GaianiteHUD.Texture != null)
                    fxRender.material = GaianiteHUD.Texture;
                break;

            case "Gems":
                if (GemsHUD.Texture != null)
                    fxRender.material = GemsHUD.Texture;
                break;

            case "EnhancementKit":
                if (EnhancementKit.Texture != null)
                    fxRender.material = EnhancementKit.Texture;
                break;

            case "PowerUp":
                if (PowUpChip.Texture != null)
                    fxRender.material = PowUpChip.Texture;
                break;
        }
        return fxRender;
    }

    private Transform Icon(string iconType)
    {
        Transform icon = null;
        switch (iconType)
        {
            case "Energy":
                if (EnergyHUD.Icon != null)
                    icon = EnergyHUD.Icon;
                break;

            case "Gaianite":
                if (GaianiteHUD.Icon != null)
                    icon = GaianiteHUD.Icon;
                break;

            case "Gems":
                if (GemsHUD.Icon != null)
                    icon = GemsHUD.Icon;
                break;

            case "EnhancementKit":
                if (EnhancementKit.Icon != null)
                    icon = EnhancementKit.Icon;
                break;

            case "PowerUp":
                if (PowUpChip.Icon != null)
                    icon = PowUpChip.Icon;
                break;
        }
        return icon;
    }
    
    private GameObject FXObject()
    {
        GameObject fx = null;
        if (FXCurrency != null)
        {
            if (FXObjPool.IsNullOrEmpty())
            {
                FXObjPool.Add(FXCurrency.gameObject);
                fx = FXCurrency.gameObject;
            }
            else
            {
                foreach(var addsubfx in FXObjPool)
                {
                    if (!addsubfx.activeInHierarchy)
                        fx = addsubfx;
                    else
                    {
                        if (FXObjPool.Count < 10)
                        {
                            GameObject addfx = Instantiate(FXCurrency.gameObject, transform.parent, false);
                            FXObjPool.Add(addfx);
                            fx = addfx;
                        }
                        break;
                    }
                }
            }
        }
        
        return fx;
    }

    private List<GameObject> FXObjPool = new List<GameObject>();

    private void Update()
    {
        if(Input.GetKey(KeyCode.Insert))
            Show(VFXCurrency.Gaianite.ToString(), 10);

        if (Input.GetKey(KeyCode.Home))
            Show(VFXCurrency.PowerUp.ToString(), 10);

        if (Input.GetKey(KeyCode.PageUp))
            Show(VFXCurrency.EnhancementKit.ToString(), 10);
    }
}
