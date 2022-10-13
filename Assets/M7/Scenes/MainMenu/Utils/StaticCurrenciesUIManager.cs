using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCurrenciesUIManager : MonoBehaviour
{
    public static StaticCurrenciesUIManager Instance;

    [SerializeField] CurrencyInstance_Generic gai;
    [SerializeField] CurrencyInstance_Generic m7;
    [SerializeField] CurrencyInstance_Generic bnb;

    private void Awake()
    {
        Instance = this;
    }
    public CurrencyInstance_Generic Gai => gai;
    public CurrencyInstance_Generic M7 => m7;
    public CurrencyInstance_Generic BNB => bnb;

    public void InitInstanceUI()
    {
        Gai.InitThis();
        M7.InitThis();
        BNB.InitThis();
    }
}