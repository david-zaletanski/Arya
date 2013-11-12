using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Arya
{
    public partial class frmCLI : Form
    {
        #region Constructor / Destructor
        public frmCLI()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            Core.OnMainFormLoad();
        }
        public void OnExit()
        {

        }
        #endregion

        #region OutputBox
        private delegate void OutputDelegate(string s);
        public void Output(string str)
        {
            if (outputBox.InvokeRequired)
            {
                Invoke(new OutputDelegate(Output),new object[] { str });
            }
            else
            {
                Output(str, Color.LightBlue);
            }
        }
        public void Output(string str, Color c)
        {
            if (outputBox.IsDisposed || outputBox.Disposing)
                return;
            string TimeStamp = DateTime.Now.ToString("M-d-y H:m:s:fff");
            outputBox.AppendText(TimeStamp+" "+str+"\n");
            outputBox.Select(outputBox.Text.Length - str.Length - 1, str.Length);
            outputBox.SelectionColor = c;
            outputBox.Select(outputBox.Text.Length, 0);
            outputBox.ScrollToCaret();
        }
        #endregion

        #region InputBox

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                string s = inputBox.Text;
                inputBox.Text = "";
                ProcessInput(s);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
        #endregion

        public void ProcessInput(string str)
        {
            Output(str);
            Core.RunCommand(str);
        }
    }
}
