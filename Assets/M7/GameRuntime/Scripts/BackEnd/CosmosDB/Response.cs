using System;

namespace M7.GameRuntime.Scripts.BackEnd.CosmosDB
{
    [Serializable]
    public struct Response
    {
        public ResponseResult ResponseResult;
        public string Message;
        public object Data;
    }

    public enum ResponseResult
    {
        OK, FAILED
    }
}