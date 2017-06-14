# Contents

## Find nth highest value

Example table
Id | Salary
-- | ------
1  | 100
2  | 200
3  | 300

### Second Highest Salary

`Question`: the query should return `200` as the second highest salary

```sql
-- Runtime: 106 ms
SELECT MAX(Salary) AS SecondHighestSalary FROM Employee
WHERE Salary < (SELECT MAX(Salary) FROM Employee)
```

### Nth Highest Salary

`Question`: the query should return `200` as the second highest salary

```sql
CREATE FUNCTION getNthHighestSalary(N INT) RETURNS INT
BEGIN
  # LIMIT start index is 0, so M = N - 1
  DECLARE M INT;
  SET M = N - 1;
  RETURN (
      # Write your MySQL query statement below.
      SELECT IFNULL(
        (SELECT DISTINCT Salary FROM Employee ORDER BY Salary DESC LIMIT M, 1)
      , NULL)
  );
END
```

# Rank Scores

[Origin](https://leetcode.com/problems/rank-scores/#/description)

 Id | Score
----|-------
 1  | 3.50
 2  | 3.65
 3  | 4.00
 4  | 3.85
 5  | 4.00
 6  | 3.65

For example, given the above Scores table, your query should generate the following report (order by highest score):
Score | Rank
------|-------
| 4.00  | 1    |
| 4.00  | 1    |
| 3.85  | 2    |
| 3.65  | 3    |
| 3.65  | 3    |
| 3.50  | 4    |

`Solution 1`:

```sql
# 使用Aggregate function返回多列数据时必须使用GROUP BY(mysql 不报错, sql server会报错)
SELECT s.Score, COUNT(DISTINCT t.Score) RANK
FROM Scores s INNER JOIN Scores t ON s.Score <= t.Score
GROUP BY s.Id
ORDER BY s.Score DESC
```

`Solution 2 (Always Count: 1322 ms)`

```sql
SELECT
  Score,
  (SELECT count(distinct Score) FROM Scores WHERE Score >= s.Score) Rank
FROM Scores s
ORDER BY Score desc
```

`Solution 3 (Always Count, Pre-uniqued: 795 ms)`:

```sql
# Same as the previous one, but faster because I have a subquery that "uniquifies" the scores first. Not entirely sure why it's faster, I'm guessing MySQL makes tmp a temporary table and uses it for every outer Score.
SELECT
  Score,
  (SELECT count(*) FROM (SELECT distinct Score s FROM Scores) tmp WHERE s >= Score) Rank
FROM Scores
ORDER BY Score desc
```