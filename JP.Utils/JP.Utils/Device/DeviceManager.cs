using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace JP.Utils.Deivce
{
    public static class DeviceManager
    {
        /// <summary>
        /// 返回设备UID //目前不可用
        /// </summary>
        /// <returns></returns>
        public static String GetDeviceId()
        {
            // nonce is an IBuffer object that would be sent from the cloud service.
            HardwareToken packageSpecificToken;

            packageSpecificToken = HardwareIdentification.GetPackageSpecificToken(null);

            // hardware id, signature, certificate IBuffer objects 
            // that can be accessed through properties.
            IBuffer hardwareId = packageSpecificToken.Id;
            IBuffer signature = packageSpecificToken.Signature;
            IBuffer certificate = packageSpecificToken.Certificate;

            return "";
        }
    }
}
