
dotnet build -c Release -p:AssemblyVersion=0.5.4 -p:FileVersion=0.5.4 -p:Version=0.5.4

mkdir nuget
mkdir nuget\lib
mkdir nuget\lib\net40
mkdir nuget\lib\netstandard2.0

copy IctBaden.RevolutionPi\bin\Release\IctBaden.RevolutionPi.dll nuget\lib\net40
copy IctBaden.RevolutionPi.Standard\bin\Release\netstandard2.0\IctBaden.RevolutionPi.dll nuget\lib\netstandard2.0

nuget pack RevolutionPi.nuspec -OutputDirectory nuget
