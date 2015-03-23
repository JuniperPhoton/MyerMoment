using System;
using System.Collections.Generic;
using System.Text;

namespace MyerMomentUniversal.Model
{
    public class MomentStyleList
    {
        public List<MomentStyle> Styles;

        public MomentStyleList()
        {
            Styles = new List<MomentStyle>();
            Styles.Add(new MomentStyle("Alone",false));
            Styles.Add(new MomentStyle("Brave",false));
            Styles.Add(new MomentStyle("Couple",false));
            Styles.Add(new MomentStyle("Dinner",false));
            Styles.Add(new MomentStyle("Food",false));
            Styles.Add(new MomentStyle("Scene",false));
            Styles.Add(new MomentStyle("Thanks",false));
            Styles.Add(new MomentStyle("Place",false));
            Styles.Add(new MomentStyle("Memory",false));
            Styles.Add(new MomentStyle("Music",false));
        }
    }
}
