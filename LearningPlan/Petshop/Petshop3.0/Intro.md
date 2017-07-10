# Utility.csproj

- DataProtector.cs: 调用Crypt32.dll加密解密
- ConnectionInfo.cs: 2 static method used for encrypt and decrypt connection string (实际可以加密解密ascii字符串), SqlHelper和OracleHelper执行sql时调用解密方法

