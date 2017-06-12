# Notes:
    * make alias of subquery because query needs table object which we will get from making an alias to subquery. 
    SELECT name FROM (SELECT name FROM agentinformation)        -- wrong
    SELECT name FROM (SELECT name FROM agentinformation) as a   -- correct 