using System;
using UnityEngine;

namespace M7.GameRuntime.Scripts.BackEnd.CosmosDB.Tests
{
    public class CosmosDBTests : MonoBehaviour
    {
        [SerializeField] private CosmosDBFunctions cosmosDBFunctions;
    }

    [Serializable]
    public class CosmosDBFunctions
    {
        public void Connect(string connectionString)
        {
            
        }

        public void CreateDB(string dbName)
        {
            
        }
    }
}


