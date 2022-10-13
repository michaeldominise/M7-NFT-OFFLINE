#if UNITY_EDITOR

using UnityEditor;

using UnityEngine.Networking;

using System.Collections;

namespace M7.Tools.Utility
{
    public static class GoogleTSVDownloader
    {
        private const string GOOGLE_DOWNLOAD_URL_FORMAT = "https://docs.google.com/spreadsheets/d/{0}/pub?gid={1}&single=true&output=tsv";

        public static void Download(string spreadSheetID, string sheetId, System.Action<TSVDownloadResult> onDownloadComplete = null, bool showDone = false)
        {
            var downloadCoroutine = DownloadTSVRoutine(spreadSheetID, sheetId, onDownloadComplete, showDone);
            while (downloadCoroutine.MoveNext()) { }
        }

        static IEnumerator DownloadTSVRoutine(string spreadSheetID, string sheetId, System.Action<TSVDownloadResult> onDownloadComplete, bool showDone)
        {
            string url = string.Format(GOOGLE_DOWNLOAD_URL_FORMAT, spreadSheetID, sheetId);

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Downloading", "Connecting to google.com ...", 0.0f);
                var async = request.SendWebRequest();
                while (!async.isDone)
                {
                    //EditorUtility.DisplayProgressBar("Downloading", "Progress..", async.progress);
                    yield return null;
                }
                EditorUtility.ClearProgressBar();
                if (showDone)
                    EditorUtility.DisplayDialog("Successful", "Done!", "Ok");
#endif

                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (onDownloadComplete != null)
                    {
                        TSVDownloadResult result = new TSVDownloadResult()
                        {
                            result = ResultState.Success,
                            downloadedResult = request.downloadHandler.text
                        };
                        onDownloadComplete(result);
                    }
                }
                else
                {
                    if (onDownloadComplete != null)
                    {
                        TSVDownloadResult result = new TSVDownloadResult()
                        {
                            result = ResultState.Fail,
                            error = request.error
                        };
                        onDownloadComplete(result);
                    }
                }
            }

            //DestroyImmediate(gameObject);
            //yield return null;
        }
    }

    public enum ResultState { Success, Fail }

    public struct TSVGoogleSpreadSheetData
    {
        public string googleSheetID;
        public string sheetID;

        public TSVGoogleSpreadSheetData(string googleSheetID, string sheetID)
        {
            this.googleSheetID = googleSheetID;
            this.sheetID = sheetID;
        }
    }

    public class TSVDownloadResult
    {
        public ResultState result;
        public string downloadedResult;
        public string error;
    }
}
#endif
