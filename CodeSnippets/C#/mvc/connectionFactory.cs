// doc: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/obtaining-a-dbproviderfactory
// This example assumes a reference to System.Data.Common.
static DataTable GetProviderFactoryClasses()
{
    // Retrieve the installed providers and factories.
    DataTable table = DbProviderFactories.GetFactoryClasses();

    // Display each row and column value.
    foreach (DataRow row in table.Rows)
    {
        foreach (DataColumn column in table.Columns)
        {
            Console.WriteLine(row[column]);
        }
    }
    return table;
}

// Given a provider name and connection string, 
// create the DbProviderFactory and DbConnection.
// Returns a DbConnection on success; null on failure.
static DbConnection CreateDbConnection(
    string providerName, string connectionString)
{
    // Assume failure.
    DbConnection connection = null;

    // Create the DbProviderFactory and DbConnection.
    if (connectionString != null)
    {
        try
        {
            DbProviderFactory factory =
                DbProviderFactories.GetFactory(providerName);

            connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
        }
        catch (Exception ex)
        {
            // Set the connection to null if it was created.
            if (connection != null)
            {
                connection = null;
            }
            Console.WriteLine(ex.Message);
        }
    }
    // Return the connection.
    return connection;
}

// Retrieve a connection string by specifying the providerName.
// Assumes one connection string per provider in the config file.
static string GetConnectionStringByProvider(string providerName)
{
    // Return null on failure.
    string returnValue = null;

    // Get the collection of connection strings.
    ConnectionStringSettingsCollection settings =
        ConfigurationManager.ConnectionStrings;

    // Walk through the collection and return the first 
    // connection string matching the providerName.
    if (settings != null)
    {
        foreach (ConnectionStringSettings cs in settings)
        {
            if (cs.ProviderName == providerName)
                returnValue = cs.ConnectionString;
            break;
        }
    }
    return returnValue;
}