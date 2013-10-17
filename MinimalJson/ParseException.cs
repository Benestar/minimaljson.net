/*******************************************************************************
 * Copyright (c) 2013 EclipseSource.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *    Ralf Sternberg - initial implementation and API
 *    Benestar - conversion into C#
 ******************************************************************************/
using System;

namespace MinimalJson
{
    /// <summary>
    /// An unchecked exception to indicate that an input does not qualify as valid JSON.
    /// </summary>
    public sealed class ParseException : Exception
    {

        /// <summary>
        /// The absolute index of the character at which the error occurred.
        /// The index of the first character of a document is 0.
        /// </summary>
        public int offset { get; private set; }

        /// <summary>
        /// The number of the line in which the error occurred. The first line counts as 1.
        /// </summary>
        public int line { get; private set; }

        /// <summary>
        /// The index of the character at which the error occurred, relative to the line.
        /// The index of the first character of a line is 0.
        /// </summary>
        public int column { get; private set; }

        internal ParseException(string message, int offset, int line, int column)
            : base(message + " at " + line + ":" + column)
        {
            this.offset = offset;
            this.line = line;
            this.column = column;
        }
    }
}