using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel.Log
{
    public class NullLog : ILog
    {
        #region ILog Members

        LogLevel m_level = LogLevel.Error;

        public void WriteEntry(LogLevel type, string message) { }

        public LogLevel Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
