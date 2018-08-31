// ----------------------------------------------------------------------------
// <copyright file="IExceptionUtility.cs" company="ABC Software Ltd">
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

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The exception utility interface
    /// </summary>
    public interface IExceptionUtility {
        /// <summary>
        /// Throws the helper.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>The thrown exception.</returns>
        Exception ThrowHelper(Exception exception, TraceEventType eventType);
    }
}
