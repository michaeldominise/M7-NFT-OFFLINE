    using System;
    using System.Collections.Generic;
    using Chamoji.Social;
    using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
    using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
    using Newtonsoft.Json;
    using PlayFab;
    using PlayFab.ClientModels;
    using UnityEngine;

    public static class VersionValidator
    {
        private static ValidateCurrentVersionResponse CachedResponse;
        // private static Response CachedResponse;
        /// <summary>
        /// Validates the application or the cdn version from the server
        /// </summary>
        /// <param name="CurrentVersion">This is the current version of the app/cdn</param>
        /// <param name="Type">Type determines if it will check the server match via Maintenance or CDN </param>
        /// <param name="OnSuccess">Returns a payload if the version matches with the server version</param>
        /// <param name="useCache">Re-uses the last response from the server</param>
        public static void ValidateCurrentVersion(string CurrentVersion, VersionValidatorType Type, Action<ValidateCurrentVersionResponse> OnSuccess, bool useCache = true)
        // public static void ValidateCurrentVersion(string CurrentVersion, VersionValidatorType Type, Action<Response> OnSuccess, bool useCache = true)
        {
//             var functionParam = new ValidateCurrentVersionRequest
//             {
//                 Type = Type.ToString(),
// #if UNITY_ANDROID
//                 Platform = "Android",
// #elif UNITY_IOS
//                         Platform = "iOS",
// #elif UNITY_WEBGL
//                           Platform = "WEBGL",
// #else
//                      Platform = "",
// #endif
//                 Version = CurrentVersion
//             };

            var functionParam = new Dictionary<string, string>
            {
                { "Type", Type.ToString() },
#if UNITY_ANDROID
                { "Platform", "Android" },
#elif UNITY_IOS
                { "Platform", "iOS" },
#elif UNITY_WEBGL
                { "Platform", "WEBGL" },
#else
                { "Platform", "" },
#endif
                {"Version", CurrentVersion},
            };

            var func = JsonConvert.SerializeObject(functionParam);
            
            Debug.Log($"Function param {func}");
            
            if (CachedResponse != null && useCache)
            {
                OnSuccess(CachedResponse);
            }
            else
            {
                AzureFunction.GetAppVersion(functionParam,
                result =>
                {
                    var response = JsonConvert.DeserializeObject<Response>(result);
            
                    if (response.Data != null)
                    {
                        var versionResponse = JsonConvert.DeserializeObject<ValidateCurrentVersionResponse>(response.Data.ToString());
                        versionResponse.Flag = string.IsNullOrEmpty(versionResponse.Payload) == false;
                        CachedResponse = versionResponse;
                        OnSuccess(versionResponse);
                    }
                }, error =>
                {
                    ValidateCurrentVersion(CurrentVersion, Type, OnSuccess);
                });
            }
        }

        public enum VersionValidatorType
        {
            Maintenance,
            CDN
        }

        public class ValidateCurrentVersionRequest
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public string Platform { get; set; }
        }
        public class ValidateCurrentVersionResponse
        {
            public bool Flag { get; set; }
            public string Payload { get; set; }
        }

    }