using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Philosophical_Artificial_Intelligence
{
    public partial class BConsole : Form
    {
        Command command = new Command();
        public BConsole()
        {
            InitializeComponent();
            this.TB_CONS.LanguageOption = 0;
        }

        public void SubMit(string command)
        {
            string[] commands = command.Split(' ');
            TB_CONS.AppendText(">" + command + "\r\n");

            if (commands.Length == 1)
            {
                AppendText(this.command.Submit(command, ""));
                return;
            }

            string parameters = command.Substring(commands[0].Length + 1);

            AppendText(this.command.Submit(commands[0], parameters));
        }

        private void TB_CMD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = e.SuppressKeyPress = true;
                button1_Click(null, null);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SubMit(this.TB_CMD.Text);
            TB_CMD.Text = string.Empty;
        }

        private void BConsole_Shown(object sender, EventArgs e)
        {
            this.TB_CMD.Focus();
        }

        public void AppendText(CommandResult text)
        {
            switch(text.ResultTypes)
            {
                case ResultType.NONE:
                    AppendText(text.Result, Color.White);
                    break;
                case ResultType.INFORMATION:
                    AppendText(text.Result, Color.Yellow);
                    break;
                case ResultType.ERROR:
                    AppendText(text.Result, Color.Red);
                    break;
            }
        }

        public void AppendText(string text, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            this.TB_CONS.SelectionStart = this.TB_CONS.TextLength;
            this.TB_CONS.SelectionLength = 0;

            this.TB_CONS.SelectionColor = color;
            this.TB_CONS.AppendText(text);
            this.TB_CONS.SelectionColor = this.TB_CONS.ForeColor;

            this.TB_CONS.Select(TB_CONS.TextLength, 0);
            TB_CONS.ScrollToCaret();
        }
    }
}
