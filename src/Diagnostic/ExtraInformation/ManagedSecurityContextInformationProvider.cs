// ----------------------------------------------------------------------------
// <copyright file="ManagedSecurityContextInformationProvider.cs" company="ABC Software Ltd">
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
namespace Diagnostic.ExtraInformation {
#else
namespace Abc.Diagnostics.ExtraInformation {
#endif
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// Provides useful diagnostic information from the managed runtime.
    /// </summary>
    public class ManagedSecurityContextInformationProvider : IExtraInformationProvider {
        /// <summary>
        /// Gets the AuthenticationType, calculating it if necessary.
        /// </summary>
        /// <value>The type of the authentication.</value>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Property")]
        public string AuthenticationType {
            get {
                return Thread.CurrentPrincipal.Identity.AuthenticationType;
            }
        }

        /// <summary>
        /// Gets the IdentityName, calculating it if necessary.
        /// </summary>
        /// <value>The name of the identity.</value>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Property")]
        public string IdentityName {
            get {
                return Thread.CurrentPrincipal.Identity.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Property")]
        public bool IsAuthenticated {
            get {
                return Thread.CurrentPrincipal.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Populates an <see cref="T:System.Collections.Generic.IDictionary`2"/> with helpful diagnostic information.
        /// </summary>
        /// <param name="dictionary">Dictionary used to populate the <see cref="T:Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.ManagedSecurityContextInformationProvider"></see></param>
        public void PopulateDictionary(IDictionary<string, object> dictionary) {
            if (dictionary == null) {
                throw new ArgumentNullException("dictionary");
            }

            dictionary.Add(SR.ExtraInformation_AuthenticationType, this.AuthenticationType);
            dictionary.Add(SR.ExtraInformation_IdentityName, this.IdentityName);
            dictionary.Add(SR.ExtraInformation_IsAuthenticated, this.IsAuthenticated.ToString());
        }
    }
}
