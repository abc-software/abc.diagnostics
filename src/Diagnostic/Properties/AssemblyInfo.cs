// ----------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="ABC software">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    The source code or its parts to use, reproduce, transfer, copy or
//    keep in an electronic form only from written agreement ABC SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Abc.Diagnostics")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ABC software")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("Copyright © ABC software 2008-2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("56963c3c-27fd-4833-b8fd-04426344c978")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if NET20 || NET30 || NET35 || NET40
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyFileVersion("1.0.0.13")]
[assembly: AssemblyInformationalVersion("1.0.13")]
#else
[assembly: AssemblyVersion("1.2")]
[assembly: AssemblyFileVersion("1.2.0.0")]
[assembly: AssemblyInformationalVersion("1.2.0-rc1")]
#endif

[assembly: CLSCompliant(true)] 
[assembly: NeutralResourcesLanguage("en")]
[assembly: InternalsVisibleTo("Diagnostic.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d1e3420a315f9780509968c65aecc1f0aeb868777c84afff7d2b32135396f624dc8715ef92bdaf600352293ec5b83564b154847f03edac1d7ff7c7e00523353da0cca9e2a21c36f2e037ae08dd6ffe7e9908d95db6639cf788e2e15a0ae76ada7fa296714b3621375feeedd0acc8f626edcf2a684f9ecbe4488ce1a4a32f758e")]