### Adding Abc.Dignostics libraries to your .NET project

The best and easiest way to add the Abc.Dignostics libraries to your .NET project is to use the NuGet package manager.

#### With Visual Studio IDE

From within Visual Studio, you can use the NuGet GUI to search for and install the Abc.Dignostics NuGet package. Or, as a shortcut, simply type the following command into the Package Manager Console:

Install-Package Abc.Diagnostics

### Configuration 

Configure sections in application configuration file

```xml
<configSections>
    <section name="diagnosticConfiguration" type="Abc.Diagnostics.Configuration.DiagnosticSettings, Abc.Diagnostics, Version=1.2.0.0"/>
</configSections>
```

### Nuget package changes

Nuget package *Abc.Diagnostics.dll* splitted to two packages
    *Abc.Diagnostics V1.0* for net20, net35 and *Abc.Diagnostics V1.2* for net4.5, netstandart16.

#### Retarget .NET20, .NET3.0, .NET3.5, .NET4.0 project to Abc.Diagnostics V1.0

- Type the following commands into the Package Manager Console:

  - Unnstall-Package Abc.Diagnostics.dll

  - Install-Package Abc.Diagnostics -Version 1.0.11

#### Retarget .NET45 project to Abc.Diagnostics V1.2

- Type the following commands into the Package Manager Console:

  - Unnstall-Package Abc.Diagnostics.dll

  - Install-Package Abc.Diagnostics

- Replace namespace *Diagnostic* to *Abc.Diagnostics*

- Recompile project