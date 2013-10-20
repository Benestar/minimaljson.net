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
    class JsonString : JsonValue
    {

        private readonly string str;

        internal JsonString(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            this.str = str;
        }

        internal override void write(JsonWriter writer)
        {
            writer.writeString(str);
        }

        public override bool isString()
        {
            return true;
        }

        public override string asString()
        {
            return str;
        }

        public override int GetHashCode()
        {
            return str.GetHashCode();
        }

        public override bool Equals(Object obj)
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
            JsonString other = (JsonString)obj;
            return str == other.str;
        }
    }
}
