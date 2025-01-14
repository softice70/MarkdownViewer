﻿using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
namespace MarkdownViewer
{
    class MdBrowser : System.Windows.Forms.WebBrowser
    {
        private MarkdownSharp.Markdown _md = new MarkdownSharp.Markdown();
        private string _mdText;
        private bool _isFromBrowser = true;
        private MainForm _mainForm;
        public MdBrowser(MainForm mainForm)
        {
            this._mainForm = mainForm;
        }
        protected override void Dispose(bool disposing)
        {
            if(_t != null)
                _t.Abort();
            base.Dispose(disposing);
        }
        public string MdText
        {
            get
            {
                return _mdText;
            }
        }
        public string CssText { get; set; }

        public void SetUrl(string url)
        {
            _ctd.cancel = true;
            this.Navigate(url);
        }
        protected override void OnNavigating(System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            if (!_isFromBrowser){
                base.OnNavigating(e);
                _isFromBrowser = true;
            }
            else
            {
                _mainForm.openFile(e.Url.LocalPath);
                e.Cancel = true;
                base.OnNavigating(e);
            }
        }
        public string Md2Html(string md)
        {
            string html = _md.Transform(md);
            html = string.Format(HTML_TMPL, CssText, html);
            return html;
        }


        private const string HTML_TMPL = "<html><head><style type=\"text/css\">{0}</style></head><body>{1}</body></html>";
        public void resetView(string content)
        {
            if (content == null){
                return;
            }
            _ctd.cancel = true;
            _ctd.mdText = content;
            this._isFromBrowser = false;
            this._mdText = content;
//            startThread();
            this.DocumentText = Md2Html(content);
        }


        private SCrossThreadData _ctd = new SCrossThreadData();
        private void md2HtmlProc()
        {
            int idles = 0;
            while (true)
            {
                string str = _ctd.mdText;
                _ctd.mdText = null;
                _ctd.cancel = false;
                if (str != null)
                {
                    idles = 0;
                    //this.DocumentText = "Waitting...";
                    string html = Md2Html(str);
                    if (!_ctd.cancel)
                        this.DocumentText = html;
                }
                else
                {
                    idles++;
                    if (idles > 100) //close thread after 10s idle
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
            }
            _t = null;
        }
        private Thread _t = null;
        private void startThread()
        {
            if (_t == null)
            {
                _t = new Thread(md2HtmlProc);
                _t.Start();
            }
        }

    }

    public class  SCrossThreadData{
        public String mdText = null;
        public bool cancel = false;
    }

}
