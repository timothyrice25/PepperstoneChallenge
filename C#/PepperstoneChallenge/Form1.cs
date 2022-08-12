using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PepperstoneChallenge
{
    public partial class frmMain : Form
    {
        ClassChallengeProcessor Processor;

        public frmMain()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Processor = new ClassChallengeProcessor();
        }
        private void btnDictionary_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            f.Title = "Open Dictionary File";
            f.DefaultExt = ".txt";
            f.Filter = "Text Files (*.txt)|*.txt|All files(*.*)|*.*";
            if (f.ShowDialog() == DialogResult.OK) {
                txtDictionary.Text = f.FileName;
            }

        }
        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            f.Title = "Open Search File";
            f.DefaultExt = ".txt";
            f.Filter = "Text Files (*.txt)|*.txt|All files(*.*)|*.*";
            if (f.ShowDialog() == DialogResult.OK)
            {
                txtSearch.Text = f.FileName;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            Processor = new ClassChallengeProcessor(txtDictionary.Text, txtSearch.Text);
            if (IsValidInputs())
            {
                Processor.Process();
                ShowMessage(Processor.GenerateResult());
            }
        }

        private bool IsValidInputs()
        {
            if (txtDictionary.Text == "")
            {
                ShowMessage("Please browse for the directory file.");
                return false;
            }

            if (txtSearch.Text == "")
            {
                ShowMessage("Please browse for the search file.");
                return false;
            }

            if (!File.Exists(txtDictionary.Text))
            {
                ShowMessage("Invalid Directory File");
                return false;
            }

            if (!File.Exists(txtSearch.Text))
            {
                ShowMessage("Invalid Search File");
                return false;
            }

            var msg = Processor.CheckDuplicateWordInDictionaryFile();
            if (msg != "")
            {
                ShowMessage(string.Format("Duplicate word found: {0}", msg));
                AppendMessage("No two words in the dictionary should be the same.");
                return false;
            }

            msg = Processor.CheckWordLengthInDictionaryFile();
            if (msg != "")
            {
                if(msg.Length<=2)
                    ShowMessage(string.Format("This word is too short: {0}", msg));
                else if (msg.Length >= 105)
                    ShowMessage(string.Format("This word is too long: {0}", msg));
                AppendMessage("Each word in the dictionary must be between 2 and 105 letters long.");
                return false;
            }

            if (!Processor.CheckDictionaryFileLength())
            {
                ShowMessage("The sum of lengths of all words in the dictionary must not exceed 105.");
                return false;
            }

            return true;
        }

        public void ShowMessage(string message)
        {
            txtConsole.Text = message;
        }

        public void AppendMessage(string message)
        {
            if(txtConsole.Text == "")
                txtConsole.Text += message;
            else
                txtConsole.Text += Environment.NewLine + message;
        }
    }
}
