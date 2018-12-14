using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WHC.OrderWater.Commons
{
    public class SafeStatusStrip : StatusStrip
    {
        delegate void SetText(ToolStripLabel toolStrip, string text);

        public void SafeSetText(ToolStripLabel toolStripLabel, string text)
        {
            if (InvokeRequired)
            {
                SetText setTextDel = delegate(ToolStripLabel toolStrip, string textVal)
                {
                    foreach (ToolStripItem item in base.Items)
                    {
                        if (item == toolStrip)
                        {
                            item.Text = textVal;
                        }
                    }
                };

                try
                {
                    Invoke(setTextDel, new object[] { toolStripLabel, text });
                }
                catch
                {
                }
            }
            else
            {
                foreach (ToolStripItem item in base.Items)
                {
                    if (item == toolStripLabel)
                    {
                        item.Text = text;
                    }
                }
            }
        }
    }
}
