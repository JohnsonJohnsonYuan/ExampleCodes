# IpInfo.Core： Caching, models, and searching algorithm()
    * SearchService.cs : 二分查找算法 和 斐波那契查找

# IpInfo.Win
    * 读取ip地址库（测试数据为data\test.txt）, 导出格式为"StartNumber	EndNumber	StartIp	EndIp	地址	编码	查找结果"的数据
    
# IpInfo.Web
    * 查找的数据保存在App_Data\data.txt
    * 使用缓存, 输入ip地址查找， 比较二分查找算法和斐波那契查找两个算法用时和查找次数
   
    * 斐波那契查找查找次数要比二分法多， 但时间要比二分法少很多
    (斐波那契查找分割点是加减运算， 比二分法除要快)
    
    '例如搜索ip：1.3.255.255'
    
    查找方式      | 查找次数 | 用时(ms)
    ------------- | --------- | --------
    二分法查找    | 15        | 1.5452 
    斐波那契查找  | 21        | 0.5058 
    
    
    * 使用kendoui 界面时, 需要复制kendo文件到Scripts\kendoui(从nopcommerce复制)
   