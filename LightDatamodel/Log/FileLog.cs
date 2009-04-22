using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel.Log
{
    public class FileLog : System.Data.LightDatamodel.Log.ILog
    {
        private System.IO.StreamWriter m_file;
        private LogLevel m_level = LogLevel.Error;

        public FileLog(string filename)
        {
            m_file = new System.IO.StreamWriter(filename, true);
        }

        public LogLevel Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public void WriteEntry(LogLevel type, string message)
        {
            if (type >= m_level)
            {
                if (type >= LogLevel.Error)
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(new System.Diagnostics.StackFrame(1, true));
                    m_file.WriteLine(string.Format("{0} - {1} - {2}\r\nStacktrace: {3}\r\n\r\n", type, DateTime.Now, message, st.ToString()));
                }
                else
                    m_file.WriteLine(string.Format("{0} - {1} - {2}", type, DateTime.Now, message));

                m_file.Flush();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_file != null)
            {
                m_file.Flush();
                m_file.Close();
                m_file.Dispose();
                m_file = null;
            }
        }

        #endregion
    }
}
