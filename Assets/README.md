# Assets for Ultimate Doom Builder

This directory and its subdirectories contain the assets that are required to run UDB, for example configuration files, images, certain DLLs. The files are copied to the output directory when the project is built. What files will be copied will depend on the operating system UDB is built on:

- Common: will always be copied
- Linux: will only be copied when built on Linux
- Linux.x64: will only be copied when built on Linux x64
- Linux.x86: will only be copied when built on Linux x86
- Windows: will only be copied when built on Windows
- Windows.x64: will only be copied when built on Windows x64
- Windows.x86: will only be copied when built on Windows x86

In Visual Studio the `Assets` folder does show up in the Solution Explorer in the Builder project, but it is not a real folder in the file system of the project. The actual files are located in the "Assets" directory in the repository root.

## Add new assets

Assets have to be added to the corresponding directory in the `Assets` directory in the repository root. 

** DO NOT CHANGE THE PROPERTIES OF THE FILES IN THE SOLUTION EXPLORER! **

** DO NOT TRY TO ADD FILES TO THE `Assets` DIRECTORY IN THE SOLUTION EXPLORER! **

The contents of the `Assets` directory uses a catch-all approach, so adding new files to the corresponding directory in the `Assets` directory in the repository root is sufficient.

If new have to be added that are not covered by the catch-all approach, the `.csproj` file of the Builder project has to be modified accordingly. For example it is configured like this:

```xml
<PropertyGroup>
  <IsWindows Condition="'$(OS)' == 'Windows_NT'">true</IsWindows>
  <IsLinux Condition="'$(OS)' != 'Windows_NT'">true</IsLinux>
</PropertyGroup>
<ItemGroup>
  <None Include="..\..\Assets\README.md">
    <Link>Assets\README.md</Link>
    <Visible>true</Visible>
  </None>
  <None Include="..\..\Assets\**\*" Exclude="..\..\Assets\README.md">
    <Link>Assets\%(RecursiveDir)%(Filename)%(Extension)</Link>
    <Visible>true</Visible>
  </None>
  <Content Include="..\..\Assets\Common\**\*">
    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Visible>false</Visible>
  </Content>
  <Content Include="..\..\Assets\Windows\**\*" Condition="'$(IsWindows)' == 'true'">
    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Visible>false</Visible>
  </Content>
  <Content Include="..\..\Assets\Linux\**\*" Condition="'$(IsLinux)' == 'true'">
    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Visible>false</Visible>
  </Content>
</ItemGroup>
```