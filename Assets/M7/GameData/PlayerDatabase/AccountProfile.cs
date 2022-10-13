using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.Settings;
using UnityEngine;

namespace M7.GameData
{
    [Serializable]
    public class AccountProfile : DirtyData
    {
        [JsonProperty] [SerializeField] public string playFabId;
        [JsonProperty] [SerializeField] public string id;
        [JsonProperty] [SerializeField] public string email; 
        [JsonProperty] [SerializeField] public string username; 
        [JsonProperty] [SerializeField] public string avatarId = "Hero_Naomi_Common";
        [JsonProperty] [SerializeField] public string walletAddress;
        [JsonProperty] [SerializeField] public PlayerType playerType;
        [JsonProperty] [SerializeField] public PlayerStatus playerStatus;
        [JsonProperty] [SerializeField] public string deviceId;
        [JsonProperty] [SerializeField] public string sessionTicket;
        
        public string PlayFabId => playFabId;
        public string Id => id;
        public string Email => email;
        public string Username => username;
        public string AvatarId => avatarId;
        public string WalletAddress => walletAddress;
        public string ShortWalletAddress => GetShortWalletAddress(walletAddress);
        public PlayerStatus PlayerStatus => playerStatus;
        public PlayerType PlayerType => playerType;
        public string DeviceId => deviceId;
        public string SessionTicket => sessionTicket;
        [ShowInInspector] public bool IsInitialized { get; set; }

        private void Save()
        {
            LocalData.SetPlayer(JsonConvert.SerializeObject(PlayerDatabase.AccountProfile));
        }
        
        public void OverwriteValues(string json)
        {
            var data = JsonConvert.DeserializeObject<AccountProfile>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            email = data.Email;
            id = data.Id;
            playFabId = data.PlayFabId;
            username = data.Username;
            avatarId = data.AvatarId;
            playerType = data.PlayerType;
            playerStatus = data.PlayerStatus;
            walletAddress = data.WalletAddress;
            deviceId = data.DeviceId;
            sessionTicket = data.SessionTicket;
            
            IsInitialized = true;
            IsDirty = true;
            Save();
        }

        public void SetPlayerStatus(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
            Save();
            IsDirty = true;
        }
        
        public void UpdateEmail(string email)
        {
            this.email = email;
            Save();
            IsDirty = true;
        }

        public void UpdateAvatar(string AvatarId)
        {
            // avatarId = AvatarId;
            // avatarId = "Hero_Ellie_Common";
            avatarId = AvatarId;
            Save();
            IsDirty = true;
        }
        
        public void UpdateDisplayName(string username)
        {
            this.username = username;
            Save();
            IsDirty = true;
        }
        
        public void SetWalletAddress(string walletAddress)
        {
            this.walletAddress = walletAddress;
            Save();
            IsDirty = true;
        }
        
        public void SetDeviceId()
        {
            deviceId = SystemInfo.deviceUniqueIdentifier;
            Save();
            IsDirty = true;
        }
        public void SetSessionTicket(string sessionTicket)
        {
            this.sessionTicket = sessionTicket;
            Save();
            IsDirty = true;
        }
        public static string GetShortWalletAddress(string walletAddress) => string.IsNullOrWhiteSpace(walletAddress) ? "" : $"{walletAddress.Substring(0, 5)}...{walletAddress.Substring(walletAddress.Length - 4, 4)}";


#if UNITY_EDITOR
        [BoxGroup("AVATAR ID"),SerializeField] private string avatarID;
        [BoxGroup("AVATAR ID"), Button]
        private void SetAvatarId()
        {
            UpdateAvatar(avatarID);
        }
#endif
    }
}
