using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;

namespace MarkdownViewer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
            string file = null;
            bool isArgsValid = true;
            HashSet<String> options = new HashSet<String>();
            foreach (String arg in args){
                if (arg.StartsWith("-")){
                    options.Add(arg.Substring(1).ToLower());
                }else{
                    if (file == null){
                        file = arg;
                    }else{
                        MessageBox.Show("Usage:\r\n\tMarkdownViewer.exe [-v] file");
                        isArgsValid = false;
                    }
                }
            }
            if (isArgsValid) {
                //if (!checkDefaultProgram(file))
                //	return;
                bool isHideEdit = options.Contains("v");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(file, isHideEdit));
            }
        }
        private static bool checkDefaultProgram(string param)
        {
            const string STR_PARAM = "/setreg";
            if (!RegDoc.IsDefaultProgram())
            {
                if (param == STR_PARAM)
                {
                    try
                    {
                        RegDoc.RegMe(Util.GetExeFile());
                    }
                    catch (Exception)
                    {
                    }
                    return false;
                }
                else
                {
                    DialogResult dr = MessageBox.Show("MarkdownViewer isn't default editor for markdown files, do you set it?", "MarkdownViewer", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Util.RunAsAdmin(Util.GetExeFile(), STR_PARAM);
                    }
                }
            }
            return true;
        }
    }
}
