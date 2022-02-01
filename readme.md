# Blazor showcase

## Poetic love story about a c# developer penetrating front-end fortresses, guarded by angular/react/vue experts, using backend-like c# code (and bits of razor)

Code in this repo is for Core Frameworks Internal Arch Forum presentation. 

PowerPoint slides will be added.

Enjoy ;)

# How to run?

Ideally from a command line, switch to `/BlazorShowBackend` directory and do `dotnet run`. It will expose app on `https:\\localhost:7777`, so open your browser and go to that uri. For OnePass login, you should be on Absa network.

It is recommended that you have latest .Net 6 and updated Visual Studio 2022. If you get an error when running EF Core on Client sample, please check https://docs.microsoft.com/en-us/aspnet/core/blazor/webassembly-native-dependencies?view=aspnetcore-6.0 , especially the part about installing .Net WebAssembly build tools (`dotnet workload install wasm-tools`).