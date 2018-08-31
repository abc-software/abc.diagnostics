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
    *Abc.Diagnostics V1.0* for net20, net35 and *Abc.Diagnostics V1.2* for net4.5, netstandart16, netstandart2.

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

### Configuration for development and test environment

#### Configuration using system.diagnostics

Configure sections in application configuration file

```xml
<configSections>
    <section name="diagnosticConfiguration" type="Abc.Diagnostics.Configuration.DiagnosticSettings, Abc.Diagnostics, Version=1.2.0.0"/>
</configSections>
```

Use _Abc.Diagnostics.DefaultLogWriter_ to write in XML format

```xml
<diagnosticConfiguration type="Abc.Diagnostics.DefaultLogWriter, Abc.Diagnostics" defaultCategory="category" />
```

Use _System.Diagnostics.XmlWriterTraceListener_ to write to a file

```xml
<system.diagnostics>
    <sources>
        <source name="category" switchValue="All">
            <listeners>
                <add name="listener" initializeData="trace.svclog" type="System.Diagnostics.XmlWriterTraceListener, System.Diagnostics" />
            </listeners> 
        </source>
    </sources>
</system.diagnostics>
```

#### Configuration using Microsoft enterprise library

Configure sections in application configuration file

```xml
<configSections>
    <section name="diagnosticConfiguration" type="Abc.Diagnostics.Configuration.DiagnosticSettings, Abc.Diagnostics, Version=1.2.0.0"/>
    <section name="loggingConfiguration" type="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings, Microsoft.Practices.EnterpriseLibrary.Logging, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"/>
    <section name="dataConfiguration" type="Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseSettings, Microsoft.Practices.EnterpriseLibrary.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"/>
</configSections>
```

Use log writer according table below

| EntrLib version | type |
| --------------- |:-----|
| V3.0 | Abc.Diagnostics.EntrLib30LogWriter |
| V4.0 | Abc.Diagnostics.EntrLib40LogWriter |
| V5.0, V5.0 Upd 1 | Abc.Diagnostics.EntrLib50LogWriter |
| V6.0 | Abc.Diagnostics.EntrLib60LogWriter |


```xml
<diagnosticConfiguration type="Abc.Diagnostics.EntrLib40LogWriter, Abc.Diagnostics"/>
```

Configure microsoft Enterprise Library logging section. 
Use _Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.XmlTraceListener_ to write to a file

```xml
<loggingConfiguration name="Logging Application Block" defaultCategory="category">
    <listeners>
      <add name="listener" type="Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.XmlTraceListener, Microsoft.Practices.EnterpriseLibrary.Logging"/>
    </listeners>
    
    <categorySources>
      <add name="category" switchValue="All">
        <listeners>
          <add name="listener"/>
        </listeners>
      </add>
    </categorySources>
</loggingConfiguration>
```

#### Configuration multiple destinations

Configure sections in application configuration file

```xml
<configSections>
    <section name="diagnosticConfiguration" type="Abc.Diagnostics.Configuration.DiagnosticSettings, Abc.Diagnostics, Version=1.2.0.0"/>
</configSections>
```

Use _Abc.Diagnostics.RoutedLogWriter_ to write in different formats.
Events with **Kdvvdb** category writes using _Viss.Diagnostics.VissLogWriter_, others events writes using _Abc.Diagnostics.DefaultLogWriter_

```xml
<diagnosticConfiguration type="Abc.Diagnostics.RoutedLogWriter, Abc.Diagnostics" defaultCategory="category">
    <filters>
        <fileter categories="Kdvvdb" type="Viss.Diagnostics.VissLogWriter, Viss.Diagnostics" />
        <fileter categories="*" type="Abc.Diagnostics.DefaultLogWriter, Abc.Diagnostics" />
    </filters>
</diagnosticConfiguration>
```

Use _System.Diagnostics.XmlWriterTraceListener_ to write to a file and use _Viss.Diagnostics.KdvvdbTraceListener_ to write to a MassTransit

```xml
<system.diagnostics>
    <sources>
        <source name="category" switchValue="All">
            <listeners>
                <add name="listener" initializeData="trace.svclog" type="System.Diagnostics.XmlWriterTraceListener, System.Diagnostics" />
            </listeners> 
        </source>
      <source name="Kdvvdb" switchValue="All">
        <listeners>
          <add name="{$name}" type="Viss.Diagnostics.KdvvdbTraceListener, Viss.Diagnostics" initializeData="rabbitmq://{$host}/{$path}" username="{$username}" password="{$password}" />
        </listeners>
      </source>
    </sources>
</system.diagnostics>
```
