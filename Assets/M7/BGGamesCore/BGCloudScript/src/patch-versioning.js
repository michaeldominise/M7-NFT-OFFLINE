/* ============================= Patch Versioning Methods =============================*/

const APPLICATION_MAINTENANCE_KEY = "applicationCurrentVersion";
const APPLICATION_CDN_ANDROID_KEY = "applicationCDNAndroidCurrentVersion";
const APPLICATION_CDN_IOS_KEY = "applicationCDNIOSCurrentVersion";
const APPLICATION_CDN_WEBGL_KEY = "applicationCDNWEBGLCurrentVersion";

handlers.CheckApplicationVersion = function(args, content)
{
  try 
  {
      
    let Type = args.Type;
    let Platform = args.Platform;
    let CurrentVersion = args.Version;
    let ServerVersion  = "";
    
    let payload = "";
    let flag = false;
    
    switch(Type)
    {
        case "Maintenance":
            ServerVersion = GetServerInternalData([APPLICATION_MAINTENANCE_KEY])[APPLICATION_MAINTENANCE_KEY];
            log.info("server/current: " + ServerVersion + "/" + CurrentVersion);
            var server = ServerVersion.split(".");
            var client = CurrentVersion.split(".");
            flag = server <= client ? true:false;
        break;
        case "CDN":
            switch(Platform)
            {
                case "Android":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_ANDROID_KEY])[APPLICATION_CDN_ANDROID_KEY];
                    break;
                case "iOS":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_IOS_KEY])[APPLICATION_CDN_IOS_KEY];
                    break;
                case "WEBGL":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_WEBGL_KEY])[APPLICATION_CDN_WEBGL_KEY];
                    break;
            }
            flag = CurrentVersion === ServerVersion ? true:false;
            payload = ServerVersion;
        break;
    }
    
    let data = {};
    data.Flag = flag;
    data.Payload = payload;
    return data;
  } 
  catch (err) 
  {
    LogError({
        "EventName" : "CheckApplicationVersion",
        "Error" : err
    });

    return err.apiErrorInfo.apiError;
  }
    
}
/* ============================= Patch Versioning Methods =============================*/