using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chamoji.Social {
    
    public class ErrorAction {

        public readonly PlayFabErrorCode errorCode;
        public readonly Action task;

        public ErrorAction(PlayFabErrorCode errorCode, Action task) {
            this.errorCode = errorCode;
            this.task = task;
        }

    }
    public static class PlayFabExtensions {

        public static Action OnNetworkError = () => { };

        public static void HandleError(this PlayFabError error, Action onUnhandled, params ErrorAction[] handlers) {
            foreach (var handler in handlers) {
                if (error.Error != handler.errorCode)
                    continue;
                handler.task.Invoke();
                return;
            }

           // NetworkMethods.CheckNetworkError(error);
            onUnhandled.Invoke();
        }
        

        public static void Validate(this ExecuteCloudScriptResult result, Action<ExecuteCloudScriptResult> onSuccess, Action<ExecuteCloudScriptResult> onError) {
            if (result.Error == null)
                onSuccess.Invoke(result);
            else
                onError.Invoke(result);
        }

        public static string GenerateMessage(this PlayFabError error) {
            var display = string.Format("{0} ({1}): {2}", error.Error.ToString(), (int)error.Error, error.ErrorMessage);
            return display;
        }

        public static string GenerateErrorMessage(this ExecuteCloudScriptResult cloudScriptResult) {
            var display = string.Format("{0}: {1}", cloudScriptResult.Error.Error, cloudScriptResult.Error.Message);
            return display;
        }

        public static void Log(this PlayFabError error) {
            var display = error.GenerateMessage();
            Debug.LogError(display);
        }

        public static void LogError(this ExecuteCloudScriptResult cloudScriptResult) {
            var display = cloudScriptResult.GenerateErrorMessage();
            Debug.LogError(display);
        }

        public static Dictionary<string, StatisticModel> ToMap(this List<StatisticModel> statistics) {
            var output = new Dictionary<string, StatisticModel>();
            foreach (var statistic in statistics) {
                if (output.ContainsKey(statistic.Name) && output[statistic.Name].Version < statistic.Version)
                    output[statistic.Name] = statistic;
                else
                    output.Add(statistic.Name, statistic);
            }
            return output;
        }

        public static int? GetStatistic(this List<StatisticModel> statisticModel, string statisticName) {
            try {
                var statistic = statisticModel
                    .FindLast(entry => entry.Name == statisticName);
                return statistic == null ? null : (int?)statistic.Value;
            } catch (NullReferenceException) {
                return null;
            }
        }

        public static int? GetStatistic(this List<StatisticValue> statisticValue, string statisticName) {
            try {
                var statistic = statisticValue
                    .FindLast(entry => entry.StatisticName == statisticName);
                return statistic == null ? null : (int?)statistic.Value;
            } catch (NullReferenceException) {
                return null;
            }
        }

        public static string DisplayName( this FriendInfo friend ) {
            return DisplayName( friend.TitleDisplayName, friend.FriendPlayFabId );
        }

        public static string DisplayName( this PlayerLeaderboardEntry entry ) {
            return DisplayName( entry.DisplayName, entry.PlayFabId );
        }

        public static string DisplayName( this PlayerProfileModel playerProfile ) {
            return DisplayName( playerProfile.DisplayName, playerProfile.PlayerId );
        }

        static string DisplayName( string name, string id ) {
            return string.IsNullOrEmpty(name) ? string.Format( "Agent {0}", id.Substring( 0, 6 ) ) : name;
        }

    }
}