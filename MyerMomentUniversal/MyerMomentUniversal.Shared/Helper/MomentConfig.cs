
using JP.Utils.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace MyerMomentUniversal.Helper
{
    public class MomentConfig
    {
        public static void InitialMomentConfig()
        {
            
            if(!LocalSettingHelper.IsExist("Position"))
            {
                LocalSettingHelper.AddValue("Position", "0");
            }

            if (!LocalSettingHelper.IsExist("QualityCompress"))
            {
#if WINDOWS_PHONE_APP
                LocalSettingHelper.AddValue("QualityCompress", "0");
#else
                LocalSettingHelper.AddValue("QualityCompress", "1");
#endif
            }

            if (!LocalSettingHelper.IsExist("TileColor"))
            {
                LocalSettingHelper.AddValue("TileColor", "1");
            }

            if(!LocalSettingHelper.IsExist("NewStyle"))
            {
                LocalSettingHelper.AddValue("NewStyle", "1");
            }

        }
    }
}
