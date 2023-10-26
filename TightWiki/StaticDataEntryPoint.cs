using RCM.Utils;

namespace TightWiki
{
    public class StaticDataEntryPoint
    {
        /// <summary>
        /// "DESKTOP-E0II68S"
        /// </summary>
        public static string DeveloperMachineName => "DESKTOP-E0II68S";

        /// <summary>
        /// "http://192.168.100.108:44397";
        /// </summary>
        public static string RCMConstructorBaseURI_Debug => EndpointsData.RCMConstructorBaseURI_Debug;

        /// <summary>
        /// "http://192.168.100.187:44397"
        /// </summary>
        public static string RCMConstructorBaseURI_Release => EndpointsData.RCMConstructorBaseURI_Release;


        public static string HostType_ParamName => "hostType";

        public static string Build_ParamName => "build";
    }
}
