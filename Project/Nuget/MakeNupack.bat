rd /s /q "../ReleaseBinary"
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" "../Codeer.Friendly.Store/Codeer.Friendly.Store.sln" /rebuild Release
"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe" "../Codeer.Friendly.Store/Codeer.Friendly.Store.sln" /rebuild Release-Eng
nuget pack friendly.store.nuspec