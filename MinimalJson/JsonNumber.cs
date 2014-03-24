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
using System.Globalization;

namespace MinimalJson
{
    class JsonNumber : JsonValue
    {

        private readonly string str;

        internal JsonNumber(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            this.str = str;
        }

        public override string ToString()
        {
            return str;
        }

        internal override void write(JsonWriter writer)
        {
            writer.write(str);
        }

        public override bool isNumber()
        {
            return true;
        }

        public override int asInt()
        {
            return int.Parse(str);
        }

        public override long asLong()
        {
            return long.Parse(str);
        }

        public override float asFloat()
        {
            return float.Parse(str,CultureInfo.InvariantCulture);
        }

        public override double asDouble()
        {
            return double.Parse(str,CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return str.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            JsonNumber other = (JsonNumber)obj;
            return str == other.str;
        }
    }
}