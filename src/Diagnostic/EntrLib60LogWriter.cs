// ----------------------------------------------------------------------------
// <copyright file="EntrLib60LogWriter.cs" company="ABC Software Ltd">
//    Copyright © 2015 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or
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

#if !(NET20 || NET35 || NET40)
namespace Abc.Diagnostics {
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Proxy class for Enterprise Library 6.0 LogWriter class.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Entr", Justification = "Contraction.")]
    public class EntrLib60LogWriter : EntrLibLogWriter {
        private static Assembly loggingAssembly;
        private static object initializeLock = new object(); 
        private static bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntrLib60LogWriter"/> class.
        /// </summary>
        public EntrLib60LogWriter()
            : this(null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntrLib60LogWriter"/> class.
        /// </summary>
        /// <param name="initializeData">The initialize data.</param>
        public EntrLib60LogWriter(string initializeData)
            : base(initializeData) {
            if (EntrLib60LogWriter.Available) {
                this.Initialize(loggingAssembly);
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is Enterprise Library 5.0 available.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is Enterprise Library 5.0 available; otherwise, <c>false</c>.
        /// </value>
        public static bool Available {
            get {
                if (!initialized) {
                    lock (initializeLock) {
                        if (!initialized) {
                            try {
                                loggingAssembly =
                                    Assembly.Load(
                                        "Microsoft.Practices.EnterpriseLibrary.Logging, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                            }
                            catch (FileNotFoundException) {
                            }
                            catch (FileLoadException) {
                            }
                        }

                        initialized = true;
                    }
                }

                return loggingAssembly != null;
            }
        }
    }
}
#endif