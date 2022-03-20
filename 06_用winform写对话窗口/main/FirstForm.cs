using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static main.PtCadLib;
using System.Runtime.InteropServices; //应用[DllImport("user32.dll")]需要

namespace main
{
    public partial class FirstForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        public static extern int SetFocus(IntPtr hWnd);
        public FirstForm()
        {
            InitializeComponent(); //初始化窗口
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PtCadLib ac = new PtCadLib();

            SetFocus(ac.AcDoc.Window.Handle); //切换窗口焦点 非模态显示窗口时需要做该设置

            ac.WriteMessage(textBox1.Text + "\n");
        }
    }
}
