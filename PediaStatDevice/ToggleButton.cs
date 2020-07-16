using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PediaStatDevice
{
    public partial class ToggleButton : UserControl
    {
        public EventHandler<EventArgs> ChannelStateChange;

        public ToggleButton()
        {
            InitializeComponent();
            ChannelState = false;

            OnColor = Color.GreenYellow;
            OffColor = Color.Transparent;
            
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            ChannelState = false;
            Notify();
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            ChannelState = true;
            Notify();
        }

        public void SetState(bool state)
        {
            ChannelState = state;

            // Set the state of the buttons
            buttonOn.Enabled = (ChannelState != true);
            buttonOff.Enabled = (ChannelState != false);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ChannelState == true)
            {
                tableLayoutPanel1.BackColor = OnColor;
               // buttonOn.Text = buttonOn.Text.ToUpper();
               // buttonOff.Text = buttonOff.Text.ToLower();
            }
            else
            {
                tableLayoutPanel1.BackColor = OffColor;
              //  buttonOn.Text = buttonOn.Text.ToLower();
              //  buttonOff.Text = buttonOff.Text.ToUpper();
            }
            base.OnPaint(e);
     
        }
        
        void Notify()
        {
            if (null != ChannelStateChange)
            {
                ChannelStateChange.Invoke(this, null);
            }
        }

        public bool ChannelState { get; set; }

        public Color OnColor { get; set; }

        public Color OffColor { get; set; }

        public string Title
        {
            get
            {
                return label2.Text;
            }
            set
            {
                label2.Text = value;
            }
        }

        public SwitchID ID { get; set; }

        public string RightButtonText
        {
            get
            {
                return buttonOff.Text;
            }
            set
            {
                buttonOff.Text = value;
            }
        }
        public string LeftButtonText
        {
            get
            {
                return buttonOn.Text;
            }
            set
            {
                buttonOn.Text = value;
            }
        }
    }
}
