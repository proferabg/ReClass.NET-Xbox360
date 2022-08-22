# ReClass.NET Xbox 360 Plugin
A plugin which allows ReClass.NET to peek memory from the Xbox 360 using JRPC2.

## Installation
- Compile or download from <>
- Copy the dll files in the appropriate Plugin folder (ReClass.NET/x86/Plugins or ReClass.NET/x64/Plugins)
- Start ReClass.NET and check the plugins form if the Xbox360 plugin is listed. Open the "Native" tab and switch all available methods to the Xbox360 plugin.
- The process selection will attempt to connect to your default console in Xbox 360 Neighborhood using JRPC2.

## Compiling
If you want to compile the ReClass.NET Xbox360 Plugin just fork the repository and create the following folder structure. If you don't use this structure you need to fix the project references.

```
..\ReClass.NET\
..\ReClass.NET\ReClass.NET\ReClass.NET.csproj
..\ReClass.NET-Xbox360
..\ReClass.NET-Xbox360\Xbox360Plugin.csproj
```