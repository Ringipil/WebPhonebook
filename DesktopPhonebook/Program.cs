using static task0.Form1;
using static task0.EfDatabaseHandler;
using static task0.SqlDatabaseHandler;

using System.Data.Entity;
using System.Data.SQLite;
using System.Data.Entity.Core.Common;
using System.Data.SQLite.EF6;


namespace task0
{

    public class SQLiteDbConfiguration : DbConfiguration
    {
        public SQLiteDbConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderServices("System.Data.SQLite",
                (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DbConfiguration.SetConfiguration(new SQLiteDbConfiguration());
            Application.Run(new Form1());
        }
    }
}
