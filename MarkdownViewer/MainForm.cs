using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MarkdownViewer
{
    public partial class MainForm : Form
    {
        private string _file = null;
        private bool _changed = false;
        public const string TITLE = "MarkdownViewer";

        public MainForm(string file, bool isHideEdit)
        {
            AllowDrop = true;
            InitializeComponent();
            initEdit();
            if (file != null)
                openFile(file);
            _splitContainer.Panel1Collapsed = isHideEdit;
            _view.AllowWebBrowserDrop = true;
            _edit.AllowDrop = true;
            _edit.DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)){
                    e.Effect = DragDropEffects.Link;
                }else{
                    e.Effect = DragDropEffects.None;
                }
            };
            _edit.DragDrop += (sender, e) =>
            {
                this.openFile(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
            };
        }
        private void initEdit()
        {
            _edit.LanguageOption = RichTextBoxLanguageOptions.UIFonts | RichTextBoxLanguageOptions.DualFont;
            //init tab len
            int w = 28;//(int)_edit.Font.Size * 4;
            int[] tabs = new int[10];
            for (int i = 0; i < tabs.Length; i++)
                tabs[i] = w * (i + 1);
            _edit.SelectionTabs = tabs;
        }

        private void newFile()
        {
            if (!checkCanCloseFile())
            {
                return;
            }
            _file = null;
            _edit.Text = "";
            resetView("");
            setChanged(false);
        }

        public void openFile(string file)
        {
            if (!checkCanCloseFile())
                return;
            try {
                _file = file;
                string content = File.ReadAllText(file);

                _ignoreFirstChanged = true;
                resetAll(content);

                setChanged(false);
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }
        private void setChanged(bool changed = true)
        {
            _changed = changed;
            refreshTitle();
        }
        private void refreshTitle()
        {
            string title = TITLE + " - " + _file;
            if (_changed)
                title += "*";
            this.Text = title;
        }

        private void resetEdit(string content)
        {
            _edit.Text = content;
        }

        private string _strCss = null;
        private void resetView(string content)
        {
            if (null == _strCss)
            {
                _strCss = File.ReadAllText(Util.GetExePath() + "\\default.css");
                _view.CssText = _strCss;
            }

            _view.resetView(content);
        }
        private void resetAll(string content)
        {
            resetEdit(content);
            resetView(content);
        }

        private void saveFile(string file, string content)
        {
            File.WriteAllText(file, content);
            if (_file != file)
            {
                _file = file;
            }
            setChanged(false);
        }

        //check changed file
        private bool checkCanCloseFile()
        {
            if (_changed)
            {
                DialogResult dr = MessageBox.Show("File changed,do you save it?", "Save File", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    saveCurrFile();
                }
                else if (dr == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private const string EXT_FILTER = "Markdown files|*.md;*.mkd;*.markdown";
        private void fileOpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog() ;
            dlg.CheckFileExists = true;
            dlg.Filter = EXT_FILTER;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                String file = dlg.FileName;
                openFile(file);
            }
        }
        private void saveCurrFile()
        {
            string file = _file;
            if (!File.Exists(_file))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.AddExtension = true;
                dlg.Filter = EXT_FILTER;
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;
                file = dlg.FileName;
            }
            string content = _edit.Text.Replace("\n", "\r\n");
            saveFile(file, content);
        }
        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            saveCurrFile();
        }

        private bool _ignoreFirstChanged = true; //system invoke or openfile invoke
        private void _edit_TextChanged(object sender, EventArgs e)
        {
            if (_ignoreFirstChanged)
            {
                _ignoreFirstChanged = false;
                return;
            }
            resetView(_edit.Text);
            setChanged();
        }

        private void showOrHideEdit()
        {
            _splitContainer.Panel1Collapsed = !_splitContainer.Panel1Collapsed;
        }

        private void viewEditMenuItem_Click(object sender, EventArgs e)
        {
            showOrHideEdit();
        }
        private string STR_ABOUT = "#MarkdownViewer v1.1\nProject:<https://github.com/jijinggang/MarkdownViewer>\n##Author\njijinggang@gmail.com\n##Copyright\nFree For All";
        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            resetView(STR_ABOUT);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!checkCanCloseFile())
            {
                e.Cancel = true;
                return;
            }
        }

        private void _view_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //  this.Text = e.Url.ToString();
        }

        private void fileNewMenuItem_Click(object sender, EventArgs e)
        {
            newFile();
        }
    }
}
