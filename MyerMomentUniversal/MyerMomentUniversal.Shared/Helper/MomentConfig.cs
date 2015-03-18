using ChaoFunctionRT;
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
            var level = Windows.System.MemoryManager.AppMemoryUsageLevel;
            if(!LocalSettingHelper.IsExist("Quality"))
            {
                switch (level)
                {
                    case Windows.System.AppMemoryUsageLevel.High: LocalSettingHelper.AddValue("Quality", "2"); break;
                    case Windows.System.AppMemoryUsageLevel.Medium: LocalSettingHelper.AddValue("Quality", "1"); break;
                    case Windows.System.AppMemoryUsageLevel.Low: LocalSettingHelper.AddValue("Quality", "0"); break;
                }
            }

            if(!LocalSettingHelper.IsExist("Position"))
            {
                LocalSettingHelper.AddValue("Position", "0");
            }
           
        }
    }
}
