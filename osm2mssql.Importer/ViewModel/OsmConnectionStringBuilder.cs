using System.Data.SqlClient;
using osm2mssql.Importer.Model;

namespace osm2mssql.Importer.ViewModel
{
	internal class OsmConnectionStringBuilder
    {
        internal SqlConnectionStringBuilder CreateSqlConnectionStringBuilder(ImporterModel model)
        {
            var con = new SqlConnectionStringBuilder();
            if (string.IsNullOrEmpty(model.Username) && string.IsNullOrEmpty(model.Password))
                con.IntegratedSecurity = true;
            else
            {
                con.UserID = model.Username;
                con.Password = model.Password;
            }
            con.DataSource = model.Host ?? string.Empty;
            con.InitialCatalog = model.Database ?? string.Empty;
            return con;
        }
    }
}
