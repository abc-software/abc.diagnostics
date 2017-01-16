// ----------------------------------------------------------------------------
// <copyright file="IStackTraceUtility.cs" company="ABC Software Ltd">
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
    using System.Diagnostics;

    /// <summary>
    /// Contract for accessing debug information.
    /// </summary>
    public interface IStackTraceUtility {
        /// <summary>
        /// Returns a text representation of the stack trace with source information if available.
        /// </summary>
        /// <param name="stackTrace">The source to represent textually.</param>
        /// <returns>The textual representation of the stack.</returns>
        string GetStackTraceWithSourceInfo(StackTrace stackTrace);
    }
}
