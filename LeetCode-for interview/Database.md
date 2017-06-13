# Find nth highest value

Example table
Id | Salary
-- | ------
1  | 100
2  | 200
3  | 300

## Second Highest Salary

`Question`: the query should return `200` as the second highest salary

```sql
-- Runtime: 106 ms
SELECT MAX(Salary) AS SecondHighestSalary FROM Employee
WHERE Salary < (SELECT MAX(Salary) FROM Employee)
```


## Second Highest Salary

`Question`: the query should return `200` as the second highest salary
