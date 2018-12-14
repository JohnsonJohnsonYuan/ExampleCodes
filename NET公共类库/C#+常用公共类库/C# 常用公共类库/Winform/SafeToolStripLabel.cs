using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Provides a Sate Text property for a ToolStripStatusLabel
    /// </summary>
    public class SafeToolStripLabel : ToolStripStatusLabel
    {
        delegate void SetText(string text);
        delegate string GetString();

        public override string Text
        {
            get
            {
                if ((base.Parent != null) &&        // Make sure that the container is already built
                    (base.Parent.InvokeRequired))   // Is Invoke required?
                {
                    GetString getTextDel = delegate()
                                            {
                                                return base.Text;
                                            };
                    string text = String.Empty;
                    try
                    {
                        // Invoke the SetText operation from the Parent of the ToolStripStatusLabel
                        text = (string)base.Parent.Invoke(getTextDel, null);
                    }
                    catch
                    {
                    }

                    return text;
                }
                else
                {
                    return base.Text;
                }
            }

            set
            {
                // Get from the container if Invoke is required
                if ((base.Parent != null) &&        // Make sure that the container is already built
                    (base.Parent.InvokeRequired))   // Is Invoke required?
                {
                    SetText setTextDel = delegate(string text)
                    {
                        base.Text = text;
                    };

                    try
                    {
                        // Invoke the SetText operation from the Parent of the ToolStripStatusLabel
                        base.Parent.Invoke(setTextDel, new object[] { value });
                    }
                    catch
                    {
                    }
                }
                else
                    base.Text = value;
            }
        }
    }
}
