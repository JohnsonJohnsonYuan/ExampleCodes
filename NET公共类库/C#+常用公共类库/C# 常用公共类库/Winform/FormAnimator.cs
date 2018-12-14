using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel; 
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WHC.OrderWater.Commons
{
    /// <summary> 
    /// Animates a form when it is shown, hidden or closed. 
    /// </summary> 
    /// <remarks> 
    /// MDI child forms do not support the Blend method and only support other 
    /// methods while being displayed for the first time and when closing. 
    /// </remarks> 
    public sealed class FormAnimator : IDisposable
    {
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags); 

        #region " Types "

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// The methods of animation available. 
        /// </summary> 
        /// <remarks> 
        /// </remarks> 
        /// <history> 
        /// [John] 31/08/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public enum AnimationMethod
        {
            [Description("Default animation method. Rolls out from edge when showing and into edge when hiding. Requires a direction.")]
            Roll = 0x0,
            [Description("Expands out from centre when showing and collapses into centre when hiding.")]
            Centre = 0x10,
            [Description("Slides out from edge when showing and slides into edge when hiding. Requires a direction.")]
            Slide = 0x40000,
            [Description("Fades from transaprent to opaque when showing and from opaque to transparent when hiding.")]
            Blend = 0x80000
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// The directions in which the Roll and Slide animations can be shown. 
        /// </summary> 
        /// <remarks> 
        /// Horizontal and vertical directions can be combined to create diagonal animations. 
        /// </remarks> 
        /// <history> 
        /// [John] 31/08/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        [Flags()]
        public enum AnimationDirection
        {
            [Description("From left to right.")]
            Right = 0x1,
            [Description("From right to left.")]
            Left = 0x2,
            [Description("From top to bottom.")]
            Down = 0x4,
            [Description("From bottom to top.")]
            Up = 0x8
        }

        #endregion

        #region " Constants "

        //Hide the form. 
        private const int AW_HIDE = 0x10000;
        //Activate the form. 
        private const int AW_ACTIVATE = 0x20000;

        #endregion

        #region " Variables "

        //The form to be animated. 
        private Form m_Form;

        //The animation method used to show and hide the form. 
        private AnimationMethod m_Method;
        //The direction in which to Roll or Slide the form. 
        private AnimationDirection m_Direction;
        //The number of milliseconds over which the animation is played. 
        private int m_Duration;

        #endregion

        #region " Properties "

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Gets or sets the animation method used to show and hide the form. 
        /// </summary> 
        /// <value> 
        /// The animation method used to show and hide the form. 
        /// </value> 
        /// <remarks> 
        /// Roll is used by default if no method is specified. 
        /// </remarks> 
        /// <history> 
        /// [John] 31/08/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        [Description("Gets or sets the animation method used to show and hide the form.")]
        public AnimationMethod Method
        {
            get { return this.m_Method; }
            set { this.m_Method = value; }
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Gets or sets the direction in which the animation is performed. 
        /// </summary> 
        /// <value> 
        /// The direction in which the animation is performed. 
        /// </value> 
        /// <remarks> 
        /// The direction is only applicable to the Roll and Slide methods. 
        /// </remarks> 
        /// <history> 
        /// [John] 31/08/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        [Description("Gets or sets the direction in which the animation is performed.")]
        public AnimationDirection Direction
        {
            get { return this.m_Direction; }
            set { this.m_Direction = value; }
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Gets or sets the number of milliseconds over which the animation is played. 
        /// </summary> 
        /// <value> 
        /// The number of milliseconds over which the animation is played. 
        /// </value> 
        /// <remarks> 
        /// </remarks> 
        /// <history> 
        /// [John] 5/09/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        [Description("Gets or sets the number of milliseconds over which the animation is played.")]
        public int Duration
        {
            get { return this.m_Duration; }
            set { this.m_Duration = value; }
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Gets the form to be animated. 
        /// </summary> 
        /// <value> 
        /// The form to be animated. 
        /// </value> 
        /// <remarks> 
        /// </remarks> 
        /// <history> 
        /// [John] 5/09/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        [Description("Gets the form to be animated.")]
        public Form Form
        {
            get { return this.m_Form; }
        }


        #endregion

        #region " Constructors "

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Creates a new FormAnimator object for the specified form. 
        /// </summary> 
        /// <param name="form"> 
        /// The form to be animated. 
        /// </param> 
        /// <remarks> 
        /// No animation will be used unless the Method and/or Direction properties are 
        /// set independently. The duration is set to quarter of a second by default. 
        /// </remarks> 
        /// <history> 
        /// [John] 5/09/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public FormAnimator(Form form)
        {
            this.m_Form = form;
            this.m_Form.Load += new EventHandler(m_Form_Load);
            this.m_Form.VisibleChanged += new EventHandler(m_Form_VisibleChanged);
            this.m_Form.Closing += new CancelEventHandler(m_Form_Closing);
            //The default animation length is quarter of a second. 
            this.m_Duration = 250;
        }

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Creates a new FormAnimator object for the specified form using the specified 
        /// method over the specified duration. 
        /// </summary> 
        /// <param name="form"> 
        /// The form to be animated. 
        /// </param> 
        /// <param name="method"> 
        /// The animation method used to show and hide the form. 
        /// </param> 
        /// <param name="duration"> 
        /// The number of milliseconds over which the animation is played. 
        /// </param> 
        /// <remarks> 
        /// No animation will be used for the Roll or Slide methods unless the Direction 
        /// property is set independently. 
        /// </remarks> 
        /// <history> 
        /// [John] 5/09/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public FormAnimator(Form form, AnimationMethod method, int duration)
            : this(form)
        {
            this.m_Method = method;
            this.m_Duration = duration;
        }

        // 
        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Creates a new FormAnimator object for the specified form using the specified 
        /// method in the specified direction over the specified duration. 
        /// </summary> 
        /// <param name="form"> 
        /// The form to be animated. 
        /// </param> 
        /// <param name="method"> 
        /// The animation method used to show and hide the form. 
        /// </param> 
        /// <param name="direction"> 
        /// The direction in which to animate the form. 
        /// </param> 
        /// <param name="duration"> 
        /// The number of milliseconds over which the animation is played. 
        /// </param> 
        /// <remarks> 
        /// The direction argument will have no effect if the Centre or Blend method is 
        /// specified. 
        /// </remarks> 
        /// <history> 
        /// [John] 5/09/2005 Created 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public FormAnimator(Form form, AnimationMethod method, AnimationDirection direction, int duration)
            : this(form, method, duration)
        {

            this.m_Direction = direction;
        }
        ~FormAnimator()
        {
            this.m_Form = null;
        }


        #endregion

        #region " Event Handlers "

        //Animates the form automatically when it is loaded. 
        private void m_Form_Load(object sender, System.EventArgs e)
        {
            //MDI child forms do not support transparency so do not try to use the Blend method. 
            if (this.m_Form.MdiParent == null || this.m_Method != AnimationMethod.Blend)
            {
                //Activate the form. 
                AnimateWindow(this.m_Form.Handle, this.m_Duration, (int)FormAnimator.AW_ACTIVATE | (int)this.m_Method | (int)this.m_Direction);
            }
        }

        //Animates the form automatically when it is shown or hidden. 
        private void m_Form_VisibleChanged(object sender, System.EventArgs e)  // ERROR: Handles clauses are not supported in C# 
        {
            //Do not attempt to animate MDI child forms while showing or hiding as they do not behave as expected. 
            if (this.m_Form.MdiParent == null)
            {
                int flags = (int)this.m_Method | (int)this.m_Direction;

                if (this.m_Form.Visible)
                {
                    //Activate the form. 
                    flags = flags | FormAnimator.AW_ACTIVATE;
                }
                else
                {
                    //Hide the form. 
                    flags = flags | FormAnimator.AW_HIDE;
                }
                AnimateWindow(this.m_Form.Handle, this.m_Duration, flags);
            }
        }

        //Animates the form automatically when it closes. 
        private void m_Form_Closing(object sender, System.ComponentModel.CancelEventArgs e) // ERROR: Handles clauses are not supported in C#  
        {
            if (!e.Cancel)
            {
                //MDI child forms do not support transparency so do not try to use the Blend method. 
                if (this.m_Form.MdiParent == null || this.m_Method != AnimationMethod.Blend)
                {
                    //Hide the form. 
                    AnimateWindow(this.m_Form.Handle, this.m_Duration, (int)FormAnimator.AW_HIDE | (int)this.m_Method | (int)AnimationDirection.Down);
                }

            }
        }
        #endregion
        
        public void Dispose()
        {
            this.m_Form.Load -= new EventHandler(m_Form_Load);
            this.m_Form.VisibleChanged -= new EventHandler(m_Form_VisibleChanged);
            this.m_Form.Closing -= new CancelEventHandler(m_Form_Closing);

        }
    }
}
