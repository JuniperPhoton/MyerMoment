using System;
using System.Collections.Generic;
using System.Text;

namespace MyerMomentUniversal.Model
{
    public class StyleList
    {
        public List<MomentStyle> Styles;

        public StyleList()
        {
            Styles = new List<MomentStyle>();
            Styles.Add(new MomentStyle("Alone"));
            Styles.Add(new MomentStyle("Brave"));
            Styles.Add(new MomentStyle("Dinner"));
            Styles.Add(new MomentStyle("Food"));
            Styles.Add(new MomentStyle("Scene"));
            Styles.Add(new MomentStyle("Thanks"));
        }
    }
}
