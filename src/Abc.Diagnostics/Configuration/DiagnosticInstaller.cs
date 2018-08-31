// ----------------------------------------------------------------------------
// <copyright file="DiagnosticInstaller.cs" company="ABC Software Ltd">
//    Copyright © 2017 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or.
//    modify it under the terms of the GNU Lesser General Public
//    License  as published by the Free Software Foundation, either
//    version 3 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with the library. If not, see http://www.gnu.org/licenses/.
// </copyright>
// ----------------------------------------------------------------------------

#if !NETSTANDARD
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic.Configuration {
#else
namespace Abc.Diagnostics.Configuration {
#endif
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;

    /// <summary>
    /// Diagnostic installer class;
    /// </summary>
    [RunInstaller(true)]
    public class DiagnosticInstaller : Installer {
        private static readonly string EventSourceName = typeof(LogUtility).Assembly.GetName().Name;
        private readonly EventLogInstaller eventLogInstaller;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticInstaller"/> class.
        /// </summary>
        public DiagnosticInstaller() {
            this.eventLogInstaller = new EventLogInstaller();
            this.eventLogInstaller.Source = EventSourceName;
            this.eventLogInstaller.Log = "Application";
            this.eventLogInstaller.UninstallAction = UninstallAction.Remove;
            this.Installers.Add(this.eventLogInstaller);
        }
    }
}
#endif