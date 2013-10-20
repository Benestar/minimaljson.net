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
using System.Text;

namespace MinimalJson
{
    class JsonParser
    {

        private const int MIN_BUFFER_SIZE = 10;
        private const int DEFAULT_BUFFER_SIZE = 1024;

        private readonly TextReader reader;
        private readonly char[] buffer;
        private int bufferOffset;
        private int index;
        private int fill;
        private int line;
        private int lineOffset;
        private int current;
        private StringBuilder captureBuffer;
        private int captureStart;

        /*
         * |                      bufferOffset
         *                        v
         * [a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t]        < input
         *                       [l|m|n|o|p|q|r|s|t|?|?]    < buffer
         *                          ^               ^
         *                       |  index           fill
         */

        internal JsonParser(string str) : this(new StringReader(str), (int) Math.Max(MIN_BUFFER_SIZE, Math.Min(DEFAULT_BUFFER_SIZE, str.Length))) { }

        internal JsonParser(TextReader reader) : this(reader, DEFAULT_BUFFER_SIZE) { }

        internal JsonParser(TextReader reader, int buffersize)
        {
            this.reader = reader;
            buffer = new char[buffersize];
            line = 1;
            captureStart = -1;
        }

        internal JsonValue parse()
        {
            read();
            skipWhiteSpace();
            JsonValue result = readValue();
            skipWhiteSpace();
            if (!isEndOfText())
            {
                throw error("Unexpected character");
            }
            return result;
        }

        private JsonValue readValue()
        {
            switch (current)
            {
                case 'n':
                    return readNull();
                case 't':
                    return readTrue();
                case 'f':
                    return readFalse();
                case '"':
                    return readString();
                case '[':
                    return readArray();
                case '{':
                    return readObject();
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return readNumber();
                default:
                    throw expected("value");
            }
        }

        private JsonArray readArray()
        {
            read();
            JsonArray array = new JsonArray();
            skipWhiteSpace();
            if (readChar(']'))
            {
                return array;
            }
            do
            {
                skipWhiteSpace();
                array.add(readValue());
                skipWhiteSpace();
            } while (readChar(','));
            if (!readChar(']'))
            {
                throw expected("',' or ']'");
            }
            return array;
        }

        private JsonObject readObject()
        {
            read();
            JsonObject obj = new JsonObject();
            skipWhiteSpace();
            if (readChar('}'))
            {
                return obj;
            }
            do
            {
                skipWhiteSpace();
                string name = readName();
                skipWhiteSpace();
                if (!readChar(':'))
                {
                    throw expected("':'");
                }
                skipWhiteSpace();
                obj.add(name, readValue());
                skipWhiteSpace();
            } while (readChar(','));
            if (!readChar('}'))
            {
                throw expected("',' or '}'");
            }
            return obj;
        }

        private string readName()
        {
            if (current != '"')
            {
                throw expected("name");
            }
            return readStringInternal();
        }

        private JsonValue readNull()
        {
            read();
            readRequiredChar('u');
            readRequiredChar('l');
            readRequiredChar('l');
            return JsonValue.NULL;
        }

        private JsonValue readTrue()
        {
            read();
            readRequiredChar('r');
            readRequiredChar('u');
            readRequiredChar('e');
            return JsonValue.TRUE;
        }

        private JsonValue readFalse()
        {
            read();
            readRequiredChar('a');
            readRequiredChar('l');
            readRequiredChar('s');
            readRequiredChar('e');
            return JsonValue.FALSE;
        }

        private void readRequiredChar(char ch)
        {
            if (!readChar(ch))
            {
                throw expected("'" + ch + "'");
            }
        }

        private JsonValue readString()
        {
            return new JsonString(readStringInternal());
        }

        private string readStringInternal()
        {
            read();
            startCapture();
            while (current != '"')
            {
                if (current == '\\')
                {
                    pauseCapture();
                    readEscape();
                    startCapture();
                }
                else if (current < 0x20)
                {
                    throw expected("valid string character");
                }
                else
                {
                    read();
                }
            }
            string str = endCapture();
            read();
            return str;
        }

        private void readEscape()
        {
            read();
            switch (current)
            {
                case '"':
                case '/':
                case '\\':
                    captureBuffer.Append((char)current);
                    break;
                case 'b':
                    captureBuffer.Append('\b');
                    break;
                case 'f':
                    captureBuffer.Append('\f');
                    break;
                case 'n':
                    captureBuffer.Append('\n');
                    break;
                case 'r':
                    captureBuffer.Append('\r');
                    break;
                case 't':
                    captureBuffer.Append('\t');
                    break;
                case 'u':
                    char[] hexChars = new char[4];
                    for (int i = 0; i < 4; i++)
                    {
                        read();
                        if (!isHexDigit())
                        {
                            throw expected("hexadecimal digit");
                        }
                        hexChars[i] = (char)current;
                    }
                    captureBuffer.Append((char)int.Parse(new string(hexChars), System.Globalization.NumberStyles.HexNumber));
                    break;
                default:
                    throw expected("valid escape sequence");
            }
            read();
        }

        private JsonValue readNumber()
        {
            startCapture();
            readChar('-');
            int firstDigit = current;
            if (!readDigit())
            {
                throw expected("digit");
            }
            if (firstDigit != '0')
            {
                while (readDigit())
                {
                }
            }
            readFraction();
            readExponent();
            return new JsonNumber(endCapture());
        }

        private bool readFraction()
        {
            if (!readChar('.'))
            {
                return false;
            }
            if (!readDigit())
            {
                throw expected("digit");
            }
            while (readDigit())
            {
            }
            return true;
        }

        private bool readExponent()
        {
            if (!readChar('e') && !readChar('E'))
            {
                return false;
            }
            if (!readChar('+'))
            {
                readChar('-');
            }
            if (!readDigit())
            {
                throw expected("digit");
            }
            while (readDigit())
            {
            }
            return true;
        }

        private bool readChar(char ch)
        {
            if (current != ch)
            {
                return false;
            }
            read();
            return true;
        }

        private bool readDigit()
        {
            if (!isDigit())
            {
                return false;
            }
            read();
            return true;
        }

        private void skipWhiteSpace()
        {
            while (isWhiteSpace())
            {
                read();
            }
        }

        private void read()
        {
            if (isEndOfText())
            {
                throw error("Unexpected end of input");
            }
            if (index == fill)
            {
                if (captureStart != -1)
                {
                    captureBuffer.Append(buffer, captureStart, fill - captureStart);
                    captureStart = 0;
                }
                bufferOffset += fill;
                fill = reader.Read(buffer, 0, buffer.Length);
                index = 0;
                if (fill == 0)
                {
                    current = -1;
                    return;
                }
            }
            if (current == '\n')
            {
                line++;
                lineOffset = bufferOffset + index;
            }
            current = buffer[index++];
        }

        private void startCapture()
        {
            if (captureBuffer == null)
            {
                captureBuffer = new StringBuilder();
            }
            captureStart = index - 1;
        }

        private void pauseCapture()
        {
            int end = current == -1 ? index : index - 1;
            captureBuffer.Append(buffer, captureStart, end - captureStart);
            captureStart = -1;
        }

        private string endCapture()
        {
            int end = current == -1 ? index : index - 1;
            string captured;
            if (captureBuffer.Length > 0)
            {
                captureBuffer.Append(buffer, captureStart, end - captureStart);
                captured = captureBuffer.ToString();
                captureBuffer.Length = 0;
            }
            else
            {
                captured = new string(buffer, captureStart, end - captureStart);
            }
            captureStart = -1;
            return captured;
        }

        private ParseException expected(string expected)
        {
            if (isEndOfText())
            {
                return error("Unexpected end of input");
            }
            return error("Expected " + expected);
        }

        private ParseException error(string message)
        {
            int absIndex = bufferOffset + index;
            int column = absIndex - lineOffset;
            int offset = isEndOfText() ? absIndex : absIndex - 1;
            return new ParseException(message, offset, line, column - 1);
        }

        private bool isWhiteSpace()
        {
            return current == ' ' || current == '\t' || current == '\n' || current == '\r';
        }

        private bool isDigit()
        {
            return current >= '0' && current <= '9';
        }

        private bool isHexDigit()
        {
            return current >= '0' && current <= '9'
                || current >= 'a' && current <= 'f'
                || current >= 'A' && current <= 'F';
        }

        private bool isEndOfText()
        {
            return current == -1;
        }
    }
}