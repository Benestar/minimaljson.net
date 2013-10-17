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
using System.IO;

namespace MinimalJson
{
    class JsonWriter
    {

        private const int CONTROL_CHARACTERS_START = 0x0000;
        private const int CONTROL_CHARACTERS_END = 0x001f;

        private static readonly char[] QUOT_CHARS = { '\\', '"' };
        private static readonly char[] BS_CHARS = { '\\', '\\' };
        private static readonly char[] LF_CHARS = { '\\', 'n' };
        private static readonly char[] CR_CHARS = { '\\', 'r' };
        private static readonly char[] TAB_CHARS = { '\\', 't' };
        // In JavaScript, U+2028 and U+2029 characters count as line endings and must be encoded.
        // http://stackoverflow.com/questions/2965293/javascript-parse-error-on-u2028-unicode-character
        private static readonly char[] UNICODE_2028_CHARS = { '\\', 'u', '2', '0', '2', '8' };
        private static readonly char[] UNICODE_2029_CHARS = { '\\', 'u', '2', '0', '2', '9' };
        private static readonly char[] HEX_DIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                             'a', 'b', 'c', 'd', 'e', 'f' };

        private readonly TextWriter writer;

        internal JsonWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        internal void write(string s)
        {
            writer.Write(s);
        }

        internal void writeString(string str)
        {
            writer.Write('"');
            int length = str.Length;
            int start = 0;
            char[] chars = str.ToCharArray();
            for (int index = 0; index < length; index++)
            {
                char[] replacement = getReplacementChars(chars[index]);
                if (replacement != null)
                {
                    writer.Write(chars, start, index - start);
                    writer.Write(replacement);
                    start = index + 1;
                }
            }
            writer.Write(chars, start, length - start);
            writer.Write('"');
        }

        private static char[] getReplacementChars(char ch)
        {
            char[] replacement = null;
            if (ch == '"')
            {
                replacement = QUOT_CHARS;
            }
            else if (ch == '\\')
            {
                replacement = BS_CHARS;
            }
            else if (ch == '\n')
            {
                replacement = LF_CHARS;
            }
            else if (ch == '\r')
            {
                replacement = CR_CHARS;
            }
            else if (ch == '\t')
            {
                replacement = TAB_CHARS;
            }
            else if (ch == '\u2028')
            {
                replacement = UNICODE_2028_CHARS;
            }
            else if (ch == '\u2029')
            {
                replacement = UNICODE_2029_CHARS;
            }
            else if (ch >= CONTROL_CHARACTERS_START && ch <= CONTROL_CHARACTERS_END)
            {
                replacement = new char[] { '\\', 'u', '0', '0', '0', '0' };
                replacement[4] = HEX_DIGITS[ch >> 4 & 0x000f];
                replacement[5] = HEX_DIGITS[ch & 0x000f];
            }
            return replacement;
        }

        internal void writeObject(JsonObject obj)
        {
            writeBeginObject();
            bool first = true;
            foreach (JsonObject.Member member in obj)
            {
                if (!first)
                {
                    writeObjectValueSeparator();
                }
                writeString(member.name);
                writeNameValueSeparator();
                member.value.write(this);
                first = false;
            }
            writeEndObject();
        }

        protected void writeBeginObject()
        {
            writer.Write('{');
        }

        protected void writeEndObject()
        {
            writer.Write('}');
        }

        protected void writeNameValueSeparator()
        {
            writer.Write(':');
        }

        protected void writeObjectValueSeparator()
        {
            writer.Write(',');
        }

        internal void writeArray(JsonArray array)
        {
            writeBeginArray();
            bool first = true;
            foreach (JsonValue value in array)
            {
                if (!first)
                {
                    writeArrayValueSeparator();
                }
                value.write(this);
                first = false;
            }
            writeEndArray();
        }

        protected void writeBeginArray()
        {
            writer.Write('[');
        }

        protected void writeEndArray()
        {
            writer.Write(']');
        }

        protected void writeArrayValueSeparator()
        {
            writer.Write(',');
        }
    }
}