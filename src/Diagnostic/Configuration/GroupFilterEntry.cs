// ----------------------------------------------------------------------------
// <copyright file="GroupFilterEntry.cs" company="ABC Software Ltd">
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

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic.Configuration {
#else
namespace Abc.Diagnostics.Configuration {
#endif
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

    /// <summary>
    /// Represents a single <see cref="GroupFilterEntry"/> configuration settings.
    /// </summary>
    public class GroupFilterEntry : NamedConfigurationElement {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupFilterEntry"/> class.
        /// </summary>
        public GroupFilterEntry() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupFilterEntry"/> class with a name.
        /// </summary>
        /// <param name="name">The name of the <see cref="NotificationFilterData"/>.</param>
        public GroupFilterEntry(string name)
            : base(name) {
        }
    }
}

