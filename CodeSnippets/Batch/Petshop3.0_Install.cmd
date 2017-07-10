@echo off

rem Adjust this to match the version of the framework you are using

rem set FRAMEWORK_VERSION=v1.0.3705
rem set FRAMEWORK_VERSION=v1.1.4322
set FRAMEWORK_VERSION=v4.0.30319
set PATH=%PATH%;%SYSTEMROOT%\Microsoft.NET\Framework\%FRAMEWORK_VERSION%
rem set WEB_FOLDER=C:\inetpub\wwwroot\mspetshop32
set WEB_FOLDER=Web
set CONFIGTOOL_FOLDER=ConfigTool
set MODEL_FOLDER=Model
set IDAL_FOLDER=IDAL
set DAL_FOLDER=DALFactory
set ORACLE_FOLDER=Oracle
set SQLSERVER_FOLDER=SQLServer
set BLL_FOLDER=BLL
set UTILITY_FOLDER=Utility


echo **********************************
echo Before you use this installer, make sure that you
echo have configured the correct connect strings in:
echo 	ConfigTool\PetShopConfigTool.exe.config
echo.
echo Also check this installer is uing the correct version of the framework
echo Compiling with Framework version %FRAMEWORK_VERSION%
echo. 
echo If you have not fixed this file then press CTRL C now!
echo.
pause

echo.
echo *************************************
echo Creating directories...
echo *************************************
echo.

rmdir "%MODEL_FOLDER%\bin\Release"  /s /q
rmdir "%DAL_FOLDER%\bin\Release"  /s /q
rmdir "%BLL_FOLDER%\bin\Release"  /s /q
rmdir "%ORACLE_FOLDER%\bin\Release"  /s /q
rmdir "%SQLSERVER_FOLDER%\bin\Release"  /s /q
rmdir "%IDAL_FOLDER%\bin\Release"  /s /q
rmdir "%CONFIGTOOL_FOLDER%\bin\Release"  /s /q
rmdir "%UTILITY_FOLDER%\bin\Release"  /s /q

del %WEB_FOLDER%\bin\*.* /q

mkdir "%MODEL_FOLDER%\bin\Release" 
mkdir "%IDAL_FOLDER%\bin\Release" 
mkdir "%Oracle_FOLDER%\bin\Release" 
mkdir "%SQLServer_FOLDER%\bin\Release" 
mkdir "%DAL_FOLDER%\bin\Release" 
mkdir "%BLL_FOLDER%\bin\Release"
mkdir "%CONFIGTOOL_FOLDER%\bin\Release"   
mkdir "%UTILITY_FOLDER%\bin\Release"   

echo.
echo ****************************************************************
echo Compiling PetShop.Utility.dll...       (Utilities)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:library /unsafe /out:"%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll" /recurse:"%UTILITY_FOLDER%\*.cs"
rem copy "%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll" "%WEB_FOLDER%\bin"
rem pause

echo.
echo ****************************************************************
echo Compiling Configuration Tool...        (PetShop Config Tool)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:exe /out:"%CONFIGTOOL_FOLDER%\bin\release\PetShopConfigTool.exe" /recurse:"%CONFIGTOOL_FOLDER%\*.cs" /r:"%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll"
rem copy "%CONFIGTOOL_FOLDER%\bin\release\PetShopConfigTool.exe" "%CONFIGTOOL_FOLDER%"
copy "%CONFIGTOOL_FOLDER%"\App.Config "%CONFIGTOOL_FOLDER%\bin\release\PetShopConfigTool.exe.Config" 
rem pause

echo.
echo ****************************************************************
echo Compiling PetShop.Model.dll...         (Entities)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%MODEL_FOLDER%\bin\release\PetShop.Model.dll" /recurse:"%MODEL_FOLDER%\*.cs" 
copy "%MODEL_FOLDER%\bin\release\PetShop.Model.dll" "%WEB_FOLDER%\bin"
rem pause

echo.
echo ****************************************************************
echo Compiling PetShop.IDAL.dll...         (Interface to the DAL)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%IDAL_FOLDER%\bin\release\PetShop.IDAL.dll" /recurse:"%IDAL_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll"
copy "%IDAL_FOLDER%\bin\release\PetShop.IDAL.dll" "%WEB_FOLDER%\bin"
rem pause

echo.
echo ****************************************************************
echo Compiling PetShop.OracleDAL.dll...     (Oracle specific DAL)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%Oracle_FOLDER%\bin\release\PetShop.OracleDAL.dll" /recurse:"%Oracle_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.IDAL.dll";"%SYSTEMROOT%\Microsoft.NET\Framework\%FRAMEWORK_VERSION%\System.Data.OracleClient.dll";"%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll"
rem copy "%Oracle_FOLDER%\bin\release\PetShop.OracleDAL.dll" "%WEB_FOLDER%\bin"
rem pause

echo.
echo ****************************************************************
echo Compiling PetShop.SQLServerDAL.dll...  (SQL Server specific DAL)
echo ****************************************************************
echo.
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%SQLServer_FOLDER%\bin\release\PetShop.SQLServerDAL.dll" /recurse:"%SQLServer_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.IDAL.dll";"%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll"
rem copy "%SQLServer_FOLDER%\bin\release\PetShop.SQLServerDAL.dll" "%WEB_FOLDER%\bin"
rem pause


echo.
echo ****************************************************************
echo Compiling PetShop.DAL.dll...           (DAL Factories)
echo ****************************************************************
echo.
rem csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%DAL_FOLDER%\bin\release\PetShop.DAL.dll" /recurse:"%DAL_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.IDAL.dll"
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%DAL_FOLDER%\bin\release\PetShop.DAL.dll" /recurse:"%DAL_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.IDAL.dll";"%SQLServer_FOLDER%\bin\release\PetShop.SQLServerDAL.dll"
copy "%DAL_FOLDER%\bin\release\PetShop.DAL.dll" "%WEB_FOLDER%\bin"
rem pause

echo.
echo ****************************************************************
echo Compiling PetShop.BLL.dll...           (Business Logic Layer)
echo ****************************************************************
echo.

csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%BLL_FOLDER%\bin\release\PetShop.BLL.dll" /recurse:"%BLL_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.DAL.dll";"%WEB_FOLDER%\bin\PetShop.IDAL.dll";"%SQLServer_FOLDER%\bin\release\PetShop.SQLServerDAL.dll"
copy "%BLL_FOLDER%\bin\release\PetShop.BLL.dll" "%WEB_FOLDER%\bin"
rem pause


echo.
echo ****************************************************************
echo Compiling PetShop.Web.dll...           (Web Assembly)
echo ****************************************************************
echo.

rem csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%WEB_FOLDER%\bin\PetShop.Web.dll" /recurse:"%WEB_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.BLL.dll";"%WEB_FOLDER%\bin\OracleDAL.dll";"%WEB_FOLDER%\bin\SQLServerDAL.dll"
csc /nologo /debug- /o+ /d:NOLOG /t:library /out:"%WEB_FOLDER%\bin\PetShop.Web.dll" /recurse:"%WEB_FOLDER%\*.cs" /r:"%WEB_FOLDER%\bin\PetShop.Model.dll";"%WEB_FOLDER%\bin\PetShop.BLL.dll"


pause

echo.
echo ****************************************************************
echo Registering PetShop.Components.dll...
echo ****************************************************************
echo.

echo Adding assemblies to the gac
rem These files go in the GAC because they are referenced in some way by the BLL assembly
gacutil /nologo /i "%Oracle_FOLDER%\bin\release\PetShop.OracleDAL.dll"
gacutil /nologo /i "%SQLServer_FOLDER%\bin\release\PetShop.SQLServerDAL.dll"
gacutil /nologo /i "%UTILITY_FOLDER%\bin\release\PetShop.Utility.dll"
gacutil /nologo /i "%WEB_FOLDER%\bin\PetShop.Model.dll"
gacutil /nologo /i "%WEB_FOLDER%\bin\PetShop.IDAL.dll"

echo.

echo Registering the Business Logic assembly in the COM+ Catalog
regsvcs /nologo "%WEB_FOLDER%\bin\PetShop.BLL.dll"

echo.

echo Adding Event source for error logging and encrypting database connection strings
"%CONFIGTOOL_FOLDER%\bin\release\PetShopConfigTool.exe"


pause
