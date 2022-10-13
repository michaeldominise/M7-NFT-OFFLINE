using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurabilityCostSetting
{
   public DurabilityCost DurabilityCost = new DurabilityCost();
}
public class DurabilityCost
{
    public int[] hpValue;
    public float[] durabilityCost;
}
public class RecoveryData
{
    public string instanceID;
    public float gaianiteCost;
}
public class RarityList
{
    public float[] common_cost;
    public float[] uncommon_cost;
    public float[] rare_cost;
    public float[] epic_cost;
    public float[] legendary_cost;
}
