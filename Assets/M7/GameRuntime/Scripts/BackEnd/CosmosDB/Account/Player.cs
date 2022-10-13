using M7.GameData;

namespace M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account
{
    public class Player
    {
        public string playFabId;
        public string id;
        public string email;
        public string password;
        public PlayerType playerType;
        public PlayerStatus playerStatus;
        public string deviceId;
        public string sessionTicket;
    }
}