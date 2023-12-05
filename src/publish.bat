dotnet build
dotnet pack
dotnet nuget push FireNeurons.NET\bin\Release\FireNeurons.NET.1.2.2.4.nupkg --skip-duplicate
dotnet nuget push FireNeurons.NET.Analyser\bin\Release\FireNeurons.NET.Analyser.1.0.2.nupkg --skip-duplicate