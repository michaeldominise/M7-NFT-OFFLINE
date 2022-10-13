using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    [System.Serializable]
    public class BattleData
    {
        public List<string> actionHistoryLog;
        public List<string> logTime;

        public int roundCount;
        public int manaRoundCount;
        public int playerActionCount;
        public int enemyActionCount;
    }
}
