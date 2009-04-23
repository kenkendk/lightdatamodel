using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel.Log
{
    public class FileLog : System.Data.LightDatamodel.Log.ILog
    {
        private LogLevel m_level = LogLevel.Error;
        private string m_filename;
        private Random m_rnd = new Random();

        public FileLog(string filename)
        {
            m_filename = filename;
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
                    TryWrite(string.Format("{0} - {1} - {2}\r\nStacktrace: {3}\r\n\r\n", type, DateTime.Now, message, st.ToString()));
                }
                else
                    TryWrite(string.Format("{0} - {1} - {2}", type, DateTime.Now, message));
            }
        }

        private void TryWrite(string line)
        {
            int retries = 3;
            while (retries-- > 0)
            {
                try
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(m_filename, true))
                        sw.WriteLine(line);
                    return;
                }
                catch
                {
                }

                //Sleep between 50 and 100 ms
                System.Threading.Thread.Sleep(50 + (m_rnd.Next(0, 10) * 5));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
