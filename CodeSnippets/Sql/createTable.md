
### Create table if not exist

[Find original question from stackoverflow](https://stackoverflow.com/questions/6520999/create-table-if-not-exists-equivalent-in-sql-server)

* mysql can use `Create table If Not Exists`:
    ```sql
        Create table If Not Exists Employee (Id int, Salary int)
    ```

* sql server equivalent:
  ```sql
    IF NOT EXISTS (SELECT * FROM sys.tables where name='Employee')
        CREATE TABLE Employee (
            Id INT,
            Salary INT
        )
  ```

### Create table

```sql
CREATE TABLE [Product] (
    [ProductId] varchar(10) PRIMARY KEY,
    [Category] varchar(10) NOT NULL REFERENCES [Category]([CatId]),
    [Name] varchar(80) NULL,
    [Descn] varchar(255) NULL
)
```
