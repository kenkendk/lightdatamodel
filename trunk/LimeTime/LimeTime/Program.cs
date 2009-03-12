using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.LightDatamodel;

namespace LimeTime
{
    static class Program
    {
        /// <summary>
        /// The filename for the database
        /// </summary>
        private const string DATABASE_NAME = "LimeTime.sqlite";

        /// <summary>
        /// The path to the current database
        /// </summary>
        public static string DatabasePath;

        /// <summary>
        /// The connection to the current database
        /// </summary>
        public static IDataFetcherCached DataConnection;

        /// <summary>
        /// A lock for the entire application
        /// </summary>
        public static object MainLock = new object();

        /// <summary>
        /// The search dialog is a singleton
        /// </summary>
        public static SearchForm SearchDlg = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            DatabasePath = System.IO.Path.Combine(Application.StartupPath, DATABASE_NAME);
#else
            DatabasePath = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName), DATABASE_NAME);
#endif
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection();

            try
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(DatabasePath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DatabasePath));

                //This also opens the db for us :)
                DatabaseUpgrader.UpgradeDatebase(con, DatabasePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create, open or upgrade the database.\r\nError message: " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataConnection = new DataFetcherWithRelations(new SQLiteDataProvider(con));

            DataConnection.GetObjects<Datamodel.Project>();

            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Icon = Properties.Resources.TrayIcon;
            trayIcon.Visible = true;

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Search ...", Properties.Resources.SearchMenuIcon, new EventHandler(TraySearch_Clicked));
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            trayIcon.ContextMenuStrip.Items.Add("Exit", Properties.Resources.CloseMenuIcon, new EventHandler(TrayClose_Clicked));

            trayIcon.Click += new EventHandler(trayIcon_Click);

            Application.Run();

            trayIcon.Visible = false;
        }

        static void trayIcon_Click(object sender, EventArgs e)
        {
            //((NotifyIcon)sender).ContextMenuStrip.Show(Cursor.Position);
        }

        public static void TrayClose_Clicked(object sender, EventArgs args)
        {
            Application.Exit();
        }

        public static void TraySearch_Clicked(object sender, EventArgs args)
        {
            lock (MainLock)
            {
                if (SearchDlg == null || !SearchDlg.Visible)
                    SearchDlg = new SearchForm();
            }

            if (!SearchDlg.Visible)
                SearchDlg.Show();
        }
    }

}