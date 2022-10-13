using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderbordBase
{
    public string PlayerID;
    public int MMR;
}

[System.Serializable]
public class Leaderbord
{
    public List<LeaderbordBase> Board = new List<LeaderbordBase>();
}
