/// SqlBulkCopy ctor第三个参数可以使用transaction

//// https://sqlbulkcopy-tutorial.net/batchsize
/// BatchSize 属性： Default Value: 0 (Unlimited)
/// By default, SqlBulkCopy will process the operation in a single batch. 
/// 如果不限制当数据过多时, 可能会出现 Timeout Expired, OutOfMemory exception
/// Decrease SqlBulkCopy performance， Impact database server performance
/// 如果由10000条数据, BatchSize为2000, 会执行5此bulk inset 语句, 每次插入2000条数据, 默认0, 一次插入全部数据
/// ****** Recommend a SqlBulkCopy BatchSize value of 4000 ******/
static void Main()
{
    var dt = GetDataToInsert();

    using (var connection = new SqlConnection(CONNECTION_STRING))
    {
        connection.Open();

        var transaction = connection.BeginTransaction();
        using (var sqlBulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
        {
            // SET BatchSize value.
            sqlBulk.BatchSize = 4000;

            sqlBulk.DestinationTableName = "Customers";
            sqlBulk.WriteToServer(dt);
            
            transaction.Commit();
        }
    }
}

public static DataTable GetDataToInsert()
{
    var dt = new DataTable();
    dt.Columns.Add("CustomerID", typeof(int));
    dt.Columns.Add("Name");
    dt.Columns.Add("City");

    for (int i = 0; i < 1000; i++)
    {
        dt.Rows.Add(i, "Name_" + i, "City_" + i);
    }

    return dt;
}