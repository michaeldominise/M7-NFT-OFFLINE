#if ENABLE_PLAYFABADMIN_API && UNITY_EDITOR
using PlayFab;
using PlayFab.AdminModels;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using M7.Extensions.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace Chamoji.Murasaki7.Admin {
    public static class playMurasaki7AdminAPI {

        static readonly string CLOUDSCRIPT_FOLDER_NAME = "M7/BGGamesCore/BGCloudScript";
        static readonly string CLOUDSCRIPT_SOURCE_FOLDER_NAME = "src";
        static readonly string CLOUDSCRIPT_REVISIONS_FOLDER_NAME = "revisions";

        static string CloudScriptFolderPath { get { return Path.Combine( Application.dataPath, CLOUDSCRIPT_FOLDER_NAME ); } }
        static string CloudScriptSourceFolderPath { get { return Path.Combine( CloudScriptFolderPath, CLOUDSCRIPT_SOURCE_FOLDER_NAME ); } }
        static string CloudScriptRevisionsFolderPath { get { return Path.Combine( CloudScriptFolderPath, CLOUDSCRIPT_REVISIONS_FOLDER_NAME ); } }
        static string CloudScriptRevisionFilePath { get { return Path.Combine( CloudScriptRevisionsFolderPath, "revision.js" ); } }

        /// <summary>
        ///     Uploads a new CloudScript revision.
        /// </summary>
        [MenuItem( "Tools/Murasaki7/Admin/Upload Revision" )]
        static void UploadRevision() {
            var title = "Upload Revision";
            var message = string.Format( "Combine all .js files under \"{0}\" and upload as a new CloudScript revision for Title: {1}?", CloudScriptSourceFolderPath, PlayFabSettings.TitleId );
            var ok = "Okay";
            var cancel = "Cancel";

            if ( !EditorUtility.DisplayDialog( title, message, ok, cancel ) )
                return;

            TranspileCloudscript();
            var content = File.ReadAllText( CloudScriptRevisionFilePath );
            PlayFabAdminAPI.UpdateCloudScript(
                new UpdateCloudScriptRequest {
                    Files = new List<CloudScriptFile> {
                        new CloudScriptFile {
                            Filename = "main.js",
                            FileContents = content
                        }
                    },
                    Publish = false
                }, result => {
                    var successTitle = "Success!";
                    var successMessage = string.Format( "CloudScript Revision {0} uploaded successfully.", result.Revision );
                    EditorUtility.DisplayDialog( successTitle, successMessage, ok );
                    successMessage.Print( Color.green );
                }, error => {
                    var errorTitle = "Error!";
                    var errorMessage = string.Format( "{0}: {1}, {2}", error.Error, error.ErrorDetails, error.ErrorMessage );
                    EditorUtility.DisplayDialog( errorTitle, errorMessage, ok );
                    Debug.LogError( errorMessage );
                } );
        }

        public static void Print(this object target, Color color)
        {
            var serializedObject = JsonConvert.SerializeObject(target).Color(color);
            Debug.Log(serializedObject);
        }

        /// <summary>
        ///     Combines all files under Application.dataPath CloudScript folder into a single .js file.
        /// </summary>
        static void TranspileCloudscript() {
            // clear file contents
            File.WriteAllLines( CloudScriptRevisionFilePath, new string[] { "" } );

            // get source filepaths
            var filepaths = Directory.GetFiles( CloudScriptSourceFolderPath, "*.js" );

            // append each source file contents
            foreach ( var filepath in filepaths ) {
                var content = File.ReadAllText( filepath );
                if ( string.IsNullOrEmpty( content ) )
                    continue;
                File.AppendAllLines( CloudScriptRevisionFilePath, new string[] { "\n", content } );
            }
        }
    }
}
#endif
