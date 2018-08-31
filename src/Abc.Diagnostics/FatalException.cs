// ----------------------------------------------------------------------------
// <copyright file="FatalException.cs" company="ABC Software Ltd">
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
#if !NETSTANDARD1_x
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Represent fatal exception.
    /// </summary>
#if !NETSTANDARD1_x
    [Serializable]
#endif
    public class FatalException
#if NETSTANDARD1_x
        : Exception {
#else
        : SystemException {
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalException"/> class.
        /// </summary>
        public FatalException() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FatalException(string message)
                : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FatalException(string message, Exception innerException)
                : base(message, innerException) {
        }

#if !NETSTANDARD1_x
        /// <summary>
        /// Initializes a new instance of the <see cref="FatalException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected FatalException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
#endif
    }
}
