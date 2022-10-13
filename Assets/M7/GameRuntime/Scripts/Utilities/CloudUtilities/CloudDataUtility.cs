using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;
using PlayFab.ClientModels;

namespace M7.CloudDataUtility
{
    public static class CloudDataUtility
    {
        public static List<string> testSeperators;
        public delegate void ErrorCallback(List<string> emptyDataKeys);
        private static ErrorCallback DefaultCallBack = SampleCallBack;
        public delegate void RetryMethod();
        public static Action<PlayFabError,RetryMethod> OnNetworkErrorCallback;

        #region Error Handling 
        public static Action<PlayFabError, RetryMethod> OnError = (error,retry) =>
        {
            CheckNetworkError(error,retry);
            DefaultError(error);
        };

        public static Action<ExecuteCloudScriptResult> OnInternalError = error =>
        {
            Debug.LogErrorFormat("{0}: {1}", error.Error.Error, error.Error.Message);
        };
        
        
        
        public static void CheckNetworkError(PlayFabError error, RetryMethod method = null)
        {
            if (error.Error == PlayFabErrorCode.ServiceUnavailable)
                OnNetworkErrorCallback.Invoke(error,method);
            else if (error.Error == PlayFabErrorCode.ConnectionError)
                OnNetworkErrorCallback.Invoke(error,method);
            else
                Debug.Log(error.ErrorMessage);
        }
        public static Action<PlayFabError> DefaultError = error => {
            Debug.LogError(error.GenerateErrorReport());
            //Debug.LogErrorFormat("{0}: {1}", error.HttpCode, error.ErrorMessage);
        };
        #endregion

        #region LoadCloudData Methods

        #region PlayerData Methods

        #region LoadingCurrentPlayerData Methods
        ///
        /// <summary>
        ///     Retrieves a single save data from the cloud using playfab api.
        /// </summary>
        /// <param name="fileName"> The file name of the save data in the cloud and local</param>
        /// <param name="OnResult">A container where we store the cloud data</param>
        ///
        [Button]
        public static void LoadPlayerCloudData(string fileName, Action<string> OnResult)
        {
            LoadPlayerCloudData(fileName, OnResult, DefaultCallBack);
        }

        /// <summary>
        ///     Retrieves a single save data from the cloud using playfab api.
        ///     If the cloud is empty then the error callback will be called.
        /// 
        /// </summary>
        /// <param name="fileName"> The file name of the save data in the cloud and local.</param>
        /// <param name="OnResult">A container where we store the data from the cloud.</param>
        /// <param name="errorCallbackMethod">Calledthe moment no cloud data is found. Use this Callback to save the current local data to the cloud</param>
        public static void LoadPlayerCloudData(string fileName, Action<string> OnResult, ErrorCallback errorCallbackMethod)
        {
            var Keys = new List<string>() { fileName };
            LoadPlayerCloudData(
                Keys,
                dataContainer =>
                {
                    OnResult(dataContainer[fileName]);
                },
                errorCallbackMethod);


        }

        /// <summary>
        ///  Retrieves multiple save data from the cloud using playfab api.
        /// </summary>
        /// <param name="fileNames">A list of save data file names in the cloud and local</param>
        /// <param name="OnResult">A container where we store the data from the cloud in a dictionary.</param>
        [Button]
        public static void LoadPlayerCloudData(List<string> fileNames, Action<Dictionary<string, string>> OnResult)
        {
            Dictionary<string, string> dataContainer = new Dictionary<string, string>();
            LoadPlayerCloudData(fileNames, OnResult, DefaultCallBack);
        }

        /// <summary>
        ///   Retrieves multiple save data from the cloud using playfab api.
        ///   If the cloud is empty then the error callback will be called.
        /// </summary>
        /// <param name="fileNames">A list of save data file names in the cloud and local</param>
        /// <param name="OnResult">A container where we store the data from the cloud in a dictionary.</param>
        /// <param name="errorCallbackMethod">Called the moment no cloud data is found. Use this Callback to save the current local data to the cloud</param>
        public static void LoadPlayerCloudData(List<string> fileNames, Action<Dictionary<string, string>> OnResult, ErrorCallback errorCallbackMethod)
        {
            Dictionary<string, string> dataContainer = new Dictionary<string, string>();
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            bool isEmptyExists = false;
            List<string> emptyDataKeys = new List<string>();
            PlayFabClientAPI.GetUserData(
                new PlayFab.ClientModels.GetUserDataRequest()
                {
                    Keys = fileNames
                }, result =>
                {
                    if (result.Data == null)
                    {
                        Debug.LogFormat("No Cloud savedata found");
                        errorCallbackMethod.Invoke(fileNames);
                        return;
                    }

                    foreach (string key in fileNames)
                    {
                        if (result.Data == null || !result.Data.ContainsKey(key))
                        {
                            Debug.LogFormat("Cloud savedata {0} is empty", key);

                            isEmptyExists = true;
                            emptyDataKeys.Add(key);
                            continue;

                        }

                        dataContainer[key] = result.Data[key].Value;
                        //Debug.LogFormat("{0} data =  {1}", key, dataContainer[key]);
                    }
                    if (isEmptyExists)
                        errorCallbackMethod.Invoke(emptyDataKeys);

                    OnResult(dataContainer);
                    return;

                }, err =>
                {
                    OnError(err, ()=> { LoadPlayerCloudData(fileNames, OnResult, errorCallbackMethod); });
                }

            );

        }

        #endregion

        #region LoadingSpecificPlayerData Methods
        /// <summary>
        ///   Retrieves a single save data from a specific player on the cloud using playfab api.
        /// </summary>
        /// <param name="playfabId">The PlayFab ID of player you want to retrieve data from</param>
        /// <param name="fileName">The file name of the save data in the cloud and local</param>
        /// <param name="dataContainer">A container where we store the cloud data</param>
        [Button]
        public static void LoadPlayerCloudData(string playfabId, string fileName, Action<string> OnResult)
        {
            LoadPlayerCloudData(playfabId, fileName, OnResult, DefaultCallBack);
        }

        /// <summary>
        ///  Retrieves a single save data from a specific player on the cloud using playfab api.
        ///  If the cloud is empty then the local playerpref data will be retrieved.
        /// </summary>
        /// <param name="playfabId">The PlayFab ID of player you want to retrieve data from</param>
        /// <param name="fileName">The file name of the save data in the cloud and local</param>
        /// <param name="dataContainer">A container where we store the cloud data</param>
        /// <param name="errorCallbackMethod">Called the moment no cloud data is found. Use this Callback to save the current local data to the cloud</param>
        public static void LoadPlayerCloudData(string playfabId, string fileName, Action<string> OnResult, ErrorCallback errorCallbackMethod)
        {
            var Keys = new List<string>() { fileName };
            Dictionary<string, string> storage = new Dictionary<string, string>();

            LoadPlayerCloudData(
                playfabId,
                Keys,
                dataContainer =>
                {
                    OnResult(dataContainer[fileName]);
                },
                errorCallbackMethod);
        }

        /// <summary>
        ///   Retrieves multiple save data a specific player on the cloud using playfab api.
        /// </summary>
        /// <param name="playfabId">The PlayFab ID of player you want to retrieve data from</param>
        /// <param name="fileNames">A container where we store the cloud data</param>
        /// <param name="OnResult"></param>
        [Button]
        public static void LoadPlayerCloudData(string playfabId, List<string> fileNames, Action<Dictionary<string, string>> OnResult)
        {
            LoadPlayerCloudData(playfabId, fileNames, OnResult, DefaultCallBack);
        }

        /// <summary>
        ///   Retrieves multiple save data a specific player on the cloud using playfab api.
        ///   If the cloud is empty then the local playerpref data will be retrieved.
        /// </summary>
        /// <param name="playfabId">The PlayFab ID of player you want to retrieve data from</param>
        /// <param name="fileNames">A container where we store the cloud data</param>
        /// <param name="OnResult">A container where we store the cloud data</param>
        /// <param name="errorCallbackMethod">Called the moment no cloud data is found. Use this Callback to save the current local data to the cloud</param>
        public static void LoadPlayerCloudData(string playfabId, List<string> fileNames, Action<Dictionary<string, string>> OnResult, ErrorCallback errorCallbackMethod)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            Dictionary<string, string> dataContainer = new Dictionary<string, string>();
            bool isEmptyExists = false;
            List<string> emptyDataKeys = new List<string>();
            PlayFabClientAPI.GetUserData(
                new PlayFab.ClientModels.GetUserDataRequest()
                {
                    PlayFabId = playfabId,
                    Keys = fileNames
                }, result =>
                {
                    if(result.Data == null)
                    {
                        Debug.LogFormat("No Cloud savedata found");
                        errorCallbackMethod.Invoke(fileNames);
                        return;
                    }

                    foreach (string key in fileNames)
                    {
                        if (!result.Data.ContainsKey(key))
                        {
                            Debug.LogFormat("Cloud savedata {0} is empty", key);

                            isEmptyExists = true;
                            emptyDataKeys.Add(key);
                            continue;

                        }

                        dataContainer[key] = result.Data[key].Value.ToString();
                        Debug.Log("Player ID = " + playfabId);
                        Debug.LogFormat("{0} data =  {1}", key, dataContainer[key]);
                    }
                    if (isEmptyExists)
                        errorCallbackMethod.Invoke(emptyDataKeys);

                    OnResult(dataContainer);
                    return;
                }, err =>
                {
                    OnError(err, () => { LoadPlayerCloudData(playfabId,fileNames, OnResult, errorCallbackMethod); });
                }

            );

        }
        #endregion

        public static void SampleCallBack(List<string> emptyKeys)
        {
            Debug.Log("Callback Error Called ");
        }

        #endregion

        #region InternalData Methods

        /// <summary>
        ///  Retrieves a single server save data from the cloud using playfab api.
        /// </summary>
        /// <param name="key"> a key/file name used to retrieve the corresponding data</param>
        /// <param name="storage">a string container used to store internal data retrieved from the cloud</param>
        [Button]
        public static void LoadInternalCloudData(string key, Action<string> OnResult, Action<PlayFabError> OnFail = null)
        {
            List<string> keys = new List<string>();
            keys.Add(key);

            LoadInternalCloudData(
                keys,
                result =>
                {
                    OnResult(result[key]);
                },Error =>
                {
                  
                    OnError(Error, () => { LoadInternalCloudData(key, OnResult, OnFail); });
                    
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                });


        }

        /// <summary>
        ///   Retrieves multiple server save data from the cloud using playfab api.
        /// </summary>
        /// <param name="keys">A list of key/file names used to retrieve the corresponding data</param>
        /// <param name="storage">A dictionary container to store internal data retrieved from the cloud</param>
        [Button]
        public static void LoadInternalCloudData(List<string> keys, Action<Dictionary<string, string>> OnResult, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            Dictionary<string, string> container = new Dictionary<string, string>();
            PlayFabClientAPI.GetTitleData(
                new GetTitleDataRequest()
                {
                   Keys = keys
                }, result =>
                {
                    if (result.Data != null)
                    {
                        // var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Data.ToString());
                        container = result.Data;
                        OnResult(container);
                        Debug.Log("response: " + container[keys[0]]);
                    }
                    else
                    {
                        Debug.Log("Internal Data is Empty");
                    }
                }, Error =>
                {
                    OnError(Error, ()=>{ LoadInternalCloudData(keys,OnResult, OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                });
        }

        /// <summary>
        /// Retrieves all server save data from the cloud using playfab api.
        /// </summary>
        /// <param name="storage">A dictionary container to store all the internal data retrieved from the cloud</param>
        [Button]
        public static void LoadAllInternalCloudData(Action<Dictionary<string, string>> OnResult, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            
            PlayFabClientAPI.GetTitleData(
                new GetTitleDataRequest()
                {
                }, result =>
                {
                    if (result.Data != null)
                    {
                        foreach (var data in result.Data.Keys)
                        {
                            Debug.LogFormat("data {0}: {1}", data, result.Data[data]);
                        }
                        var response = result.Data;
                        OnResult(response);
                    }
                    else
                    {
                        Debug.Log("Internal Data is Empty");
                    }
                }, Error=>
                {

                    OnError(Error, ()=> { LoadAllInternalCloudData(OnResult, OnFail); });
                    if(Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                 
                });
        }

        #endregion

        #endregion

        #region SaveCloudData Methods

        /// <summary>
        /// Stores a single JSON file to the cloud
        /// </summary>
        /// <param name="jsonData"> Custom JSON file to be converted to a string before saving to the cloud</param>
        /// <param name="fileName">the file name of the save data that should be the same with the player prefs local name</param>
        [Button]
        public static void SaveCloud(TextAsset jsonData, string fileName, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            SaveCloud(jsonData.text, fileName, OnSuccess, OnFail);
        }

        /// <summary>
        ///  Stores a single save data to the cloud
        /// </summary>
        /// <param name="data">the data that should be stored in the cloud.</param>
        /// <param name="filename">the file name of the save data that should be the same with the player prefs local name.</param>
        [Button]
        public static void SaveCloud(string data, string fileName, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            PlayFabClientAPI.UpdateUserData
            (
                new PlayFab.ClientModels.UpdateUserDataRequest()
                {
                    Data = new Dictionary<string, string>()
                    {
                        {fileName,data},
                    }
                }, result =>
                {
                    if(result != null)
                    {
                        if(OnSuccess != null)
                        {
                            OnSuccess(result);
                        }
                        
                    }

                    Debug.Log("Cloud savedata successfully saved.");
                }, Error =>
                {
                    OnError(Error, ()=> { SaveCloud(data, fileName, OnSuccess, OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                }
            );
        }

        /// <summary>
        /// Stores multiple JSON file to the cloud
        /// </summary>
        /// <param name="saveData"> a dictionary that contains multiple pairings of save data file name and a JSON save file </param>
        [Button]
        public static void SaveCloud(Dictionary<string, TextAsset> saveData, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            var data = new Dictionary<string, string>();
            foreach (var item in saveData)
            {
                data[item.Key] = item.Value.text;
            }

            SaveCloud(data, OnSuccess, OnFail);
        }

        /// <summary>
        ///  Stores multiple save data to the cloud
        /// </summary>
        /// <param name="saveData">a dictionary that contains multiple pairings of save data file name and a string save data </param>
        [Button]
        public static void SaveCloud(Dictionary<string, string> saveData, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            PlayFabClientAPI.UpdateUserData
            (
                new PlayFab.ClientModels.UpdateUserDataRequest()
                {
                    Data = saveData
                }, result =>
                {
                    Debug.Log("Cloud savedata successfully saved.");
                    if (result != null)
                    {
                        if (OnSuccess != null)
                        {
                            OnSuccess(result);
                        }

                    }

                }, Error =>
                {
                    OnError(Error, ()=> { SaveCloud(saveData, OnSuccess, OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                }
            );
        }

        #region CloudSavingJsonSeperator Methods


        /// <summary>
        ///    Seperates the json file into multiple files based on the separator keys.
        ///    After the file is separated, the saperated files will be uploaded immediately to the cloud
        /// </summary>
        /// <param name="seperatorKeys">A list of keys/file name to separate the json file</param>
        /// <param name="jsonData">A single Json file that needs to be separated</param>
        ///
        [Button]
        public static void SaveCloudSeparator(List<string> separatorKeys, TextAsset jsonData, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            SaveCloudSeparator(separatorKeys, jsonData.text, OnSuccess, OnFail);
        }

        /// <summary>
        ///  Seperates the string json file into multiple files based on the separator keys.
        ///  After the file is separated, the separated files will be uploaded immediately to the cloud
        /// </summary>
        /// <param name="seperatorKeys">A list of keys/file name to separate the json file</param>
        /// <param name="saveFile">A single Json string that needs to be separated</param>
        [Button]
        public static void SaveCloudSeparator(List<string> separatorKeys, string saveFile, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            JsonSeparator(
                separatorKeys,
                saveFile,
                result =>
                {
                    SaveCloud(result, OnSuccess, OnFail);
                });

        }

        /// <summary>
        /// Seperates the string json file into multiple files automatically.
        /// After the file is separated, it will be automatically save
        /// </summary>
        /// <param name="jsonData">A single Json file that needs to be separated</param>
        public static void SaveCloudSeparator(TextAsset jsonData, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            SaveCloudSeparator(jsonData.text, OnSuccess, OnFail);

        }

        /// <summary>
        /// Seperates the string json file into multiple files automatically.
        /// After the file is separated, it will be automatically save
        /// </summary>
        /// <param name="saveFile">A single Json string that needs to be separated</param>
        public static void SaveCloudSeparator(string saveFile, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            JsonSeparator(
                saveFile,
                result =>
                {
                    SaveCloud(result, OnSuccess, OnFail);
                });

        }

        #endregion

        #endregion

        #region JsonSeparator Methods

        /// <summary>
        /// Separates the Json into different files base on the separatorKeys provided
        /// </summary>
        /// <param name="separatorKeys">A list of keys/file name to separate the json file</param>
        /// <param name="jsonData">A single Json file that needs to be separated</param>
        /// <param name="OnResult">A container for getting the separated files</param>
        public static void JsonSeparator(List<string> separatorKeys, TextAsset jsonData, Action<Dictionary<string, string>> OnResult)
        {
            JsonSeparator(separatorKeys, jsonData.text, OnResult);
        }

        /// <summary>
        /// Separates the Json string into different files base on the separatorKeys provided
        /// </summary>
        /// <param name="separatorKeys">A list of keys/file name to separate the json file</param>
        /// <param name="dataFile">A single Json string that needs to be separated</param>
        /// <param name="OnResult">A container for getting the separated files</param>
        public static void JsonSeparator(List<string> separatorKeys, string dataFile, Action<Dictionary<string, string>> OnResult)
        {
            Dictionary<string, string> parsedFiles = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataFile);
            Dictionary<string, string> separatedFiles = new Dictionary<string, string>();
            foreach (string key in separatorKeys)
            {
                separatedFiles[key] = parsedFiles[key].ToString();
                Debug.LogFormat("Seperated {0} from save file: {1}", key, separatedFiles[key]);
            }
            OnResult(separatedFiles);
        }

        /// <summary>
        /// Separates the Json into different files automatically
        /// </summary>
        /// <param name="jsonData">A single Json file that needs to be separated</param>
        /// <param name="OnResult">A container for getting the separated files</param>
        public static void JsonSeparator(TextAsset jsonData, Action<Dictionary<string, string>> OnResult)
        {
            JsonSeparator(jsonData.text, OnResult);
        }

        /// <summary>
        /// Separates the Json into different files automatically
        /// </summary>
        /// <param name="dataFile">A single Json string that needs to be separated</param>
        /// <param name="OnResult">A container for getting the separated files</param>
        public static void JsonSeparator(string dataFile, Action<Dictionary<string, string>> OnResult)
        {
            Dictionary<string, string> parsedFiles = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataFile);
            Dictionary<string, string> separatedFiles = new Dictionary<string, string>();
            foreach (string key in parsedFiles.Keys)
            {
                separatedFiles[key] = parsedFiles[key].ToString();
                Debug.LogFormat("Seperated {0} from save file: {1}", key, separatedFiles[key]);
            }
            OnResult(separatedFiles);
        }

        #endregion

        #region DeleteCloudData Methods
        /// <summary>
        /// Deletes a single saved data stored on the cloud
        /// </summary>
        /// <param name="fileName">the file name of the save data that you want to delete</param>
        [Button]
        public static void DeleteCloud(string fileName, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                if (OnFail != null)
                {
                    PlayFabError error = new PlayFabError();
                }
                return;
            }

            PlayFabClientAPI.UpdateUserData(
                new PlayFab.ClientModels.UpdateUserDataRequest()
                {
                    KeysToRemove = new List<string>() { fileName }
                }, result => {
                    Debug.Log("Cloud savedata successfully deleted.");
                    if (result != null)
                    {
                        if (OnSuccess != null)
                        {
                            OnSuccess(result);
                        }

                    }
                },  Error =>
                {
                    OnError(Error, ()=> { DeleteCloud(fileName, OnSuccess, OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                }
            );
        }

        /// <summary>
        /// Deletes multiple save data stored on the cloud
        /// </summary>
        /// <param name="fileNames">a list of file names connected to a save data that you want to delete</param>
        [Button]
        public static void DeleteCloud(List<string> fileNames, Action<UpdateUserDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            PlayFabClientAPI.UpdateUserData(
                new PlayFab.ClientModels.UpdateUserDataRequest()
                {
                    KeysToRemove = fileNames
                }, result => {
                    Debug.Log("Cloud savedata successfully deleted.");
                     if (result != null)
                    {
                        if (OnSuccess != null)
                        {
                            OnSuccess(result);
                        }

                    } 
                }, Error =>
                {
                    OnError(Error,()=> { DeleteCloud(fileNames,OnSuccess,OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                }
            );
        }

        #endregion

        #region Server Methods
        

        /// <summary>
        /// Retrieves the server time in Playfab
        /// </summary>
        /// <param name="serverTime">returns the server time</param>
        public static void GetServerTime(Action<DateTime> serverTime, Action<PlayFabError> OnFail = null)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            
            PlayFabClientAPI.ExecuteCloudScript(
                new PlayFab.ClientModels.ExecuteCloudScriptRequest()
                {
                    FunctionName = "GetServerTime",
                    GeneratePlayStreamEvent = true

                }, result =>
                {
                    if (result.FunctionResult != null)
                    {
                        DateTime convertedDate = Convert.ToDateTime(result.FunctionResult.ToString());//DateTime.SpecifyKind(DateTime.Parse(result.FunctionResult.ToString()), DateTimeKind.Utc);
                       // Debug.LogError("Converted date UNIVERSAL:" + convertedDate.ToUniversalTime());
                        serverTime(convertedDate.ToUniversalTime());
                        
                    }   

                    if (result.Error != null)
                    {
                        OnInternalError(result);
                    }

                }, Error =>
                {
                    OnError(Error, () => { GetServerTime(serverTime, OnFail); });
                    if (Error != null && OnFail != null)
                    {
                        OnFail(Error);
                    }
                }
            ) ;
        }
        #endregion
    }
}

