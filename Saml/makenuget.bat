rmdir /Q /S nuget
mkdir nuget\lib
copy bin\Debug nuget\lib
copy Saml.nuspec nuget
cd nuget
nuget.exe pack Saml.nuspec
pause