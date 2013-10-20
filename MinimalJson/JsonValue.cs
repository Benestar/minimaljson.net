/*******************************************************************************
 * Copyright (c) 2008, 2013 EclipseSource.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 * Ralf Sternberg - initial implementation and API
 * Benestar - conversion into C#
 ******************************************************************************/
using System;
using System.IO;

namespace MinimalJson
{   
    /// <summary>
    /// Represents a JSON value. According to RFC 4627, a JSON value must be an object, an array, a number, a string, or one of the literal names true, false, and null.
    /// The literal names true, false, and null are represented by the constants <see cref="TRUE"/>, <see cref="FALSE"/>, and <see cref="NULL"/>.
    /// JSON objects and arrays are represented by the subtypes <see cref="JsonObject"/> and <see cref="JsonArray"/>.
    /// Instances of these types can be created using the public constructors.
    /// Instances for JSON numbers and strings can be created using the static factory methods <see cref="valueOf(string)"/>, <see cref="valueOf(long)"/>, <see cref="valueOf(double)"/> , etc.
    /// In order to find out whether an instance of this class is of a certain type, the methods <see cref="isObject()"/>, <see cref="isArray()"/>, <see cref="isString()"/>, <see cref="isNumber()"/> etc. can be used.
    /// If there is no doubt about the type of a JSON value, one of the methods <see cref="asObject()"/>, <see cref="asArray()"/>, <see cref="asString("/>, <see cref="asInt()"/>, etc. can be used to get this value directly in the appropriate target type.
    /// This class is not supposed to be extended by clients. 
    /// </summary>
    public abstract class JsonValue
    {

        /// <summary>
        /// Represents the JSON literal <code>true</code>.
        /// </summary>
        public static readonly JsonValue TRUE = new JsonLiteral("true");

        /// <summary>
        /// Represents the JSON literal <code>false</code>.
        /// </summary>
        public static readonly JsonValue FALSE = new JsonLiteral("false");

        /// <summary>
        /// The JSON literal <code>null</code>.
        /// </summary>
        public static readonly JsonValue NULL = new JsonLiteral("null");

        internal JsonValue()
        {
            // prevent subclasses outside of this package
        }

        public JsonValue this[string name]
        {
            get
            {
                return this.asObject().get(name);
            }
            set
            {
                this.asObject().set(name, value);
            }
        }

        public JsonValue this[int i]
        {
            get
            {
                return this.asArray().get(i);
            }
            set
            {
                this.asArray().set(i, value);
            }
        }

        /// <summary>
        /// Reads a JSON value from the given reader.
        /// Characters are read in chunks and buffered internally, therefore wrapping an existing reader in
        /// an additional BufferedReader does not improve reading performance.
        /// </summary>
        /// <param name="reader">the reader to read the JSON value from</param>
        /// <returns>the JSON value that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        public static JsonValue readFrom(TextReader reader)
        {
            return new JsonParser(reader).parse();
        }

        /// <summary>
        /// Reads a JSON value from the given string.
        /// </summary>
        /// <param name="text">the string that contains the JSON value</param>
        /// <returns>the JSON value that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        public static JsonValue readFrom(string text)
        {
            return new JsonParser(text).parse();
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given <code>int</code> value.
        /// </summary>
        /// <param name="value">the value to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given value</returns>
        public static JsonValue valueOf(int value)
        {
            return new JsonNumber(value.ToString());
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given <code>long</code> value.
        /// </summary>
        /// <param name="value">the value to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given value</returns>
        public static JsonValue valueOf(long value)
        {
            return new JsonNumber(value.ToString());
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given <code>float</code> value.
        /// </summary>
        /// <param name="value">the value to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given value</returns>
        public static JsonValue valueOf(float value)
        {
            if (float.IsInfinity(value) || float.IsNaN(value))
            {
                throw new ArgumentException("Infinite and NaN values not permitted in JSON", "value");
            }
            return new JsonNumber(cutOffPointZero(value.ToString()));
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given <code>double</code> value.
        /// </summary>
        /// <param name="value">the value to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given value</returns>
        public static JsonValue valueOf(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentException("Infinite and NaN values not permitted in JSON", "value");
            }
            return new JsonNumber(cutOffPointZero(value.ToString()));
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given string.
        /// </summary>
        /// <param name="str">the string to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given string</returns>
        public static JsonValue valueOf(string str)
        {
            return str == null ? NULL : new JsonString(str);
        }

        /// <summary>
        /// Returns a JsonValue instance that represents the given <code>bool</code> value.
        /// </summary>
        /// <param name="value">the value to get a JSON representation for</param>
        /// <returns>a JSON value that represents the given value</returns>
        public static JsonValue valueOf(bool value)
        {
            return value ? TRUE : FALSE;
        }

        /// <summary>
        /// Detects whether this value represents a JSON object. If this is the case, this value is an instance of <see cref="JsonObject"/>.
        /// </summary>
        /// <returns><code>true</code> if this value is an instance of JsonObject</returns>
        public virtual bool isObject()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents a JSON array. If this is the case, this value is an instance of <see cref="JsonArray"/>.
        /// </summary>
        /// <returns><code>true</code> if this value is an instance of JsonArray</returns>
        public virtual bool isArray()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents a JSON number.
        /// </summary>
        /// <returns><code>true</code> if this value represents a JSON number</returns>
        public virtual bool isNumber()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents a JSON string.
        /// </summary>
        /// <returns><code>true</code> if this value represents a JSON string</returns>
        public virtual bool isString()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents a bool value.
        /// </summary>
        /// <returns><code>true</code> if this value represents either the JSON literal <code>true</code> or <code>false</code></returns>
        public virtual bool isBool()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents the JSON literal <code>true</code>.
        /// </summary>
        /// <returns><code>true</code> if this value represents the JSON literal <code>true</code></returns>
        public virtual bool isTrue()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents the JSON literal <code>false</code>.
        /// </summary>
        /// <returns><code>true</code> if this value represents the JSON literal <code>false</code></returns>
        public virtual bool isFalse()
        {
            return false;
        }

        /// <summary>
        /// Detects whether this value represents the JSON literal <code>null</code>.
        /// </summary>
        /// <returns><code>true</code> if this value represents the JSON literal <code>null</code></returns>
        public virtual bool isNull()
        {
            return false;
        }

        /// <summary>
        /// Returns this JSON value as <see cref="JsonObject"/>, assuming that this value represents a JSON object. If this is not the case, an exception is thrown.
        /// </summary>
        /// <returns>a JSONObject for this value</returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON object</exception>
        public virtual JsonObject asObject()
        {
            throw new NotSupportedException("Not an object: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as <see cref="JsonArray"/>, assuming that this value represents a JSON array. If this is not the case, an exception is thrown.
        /// </summary>
        /// <returns>a JSONArray for this value</returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON array</exception>
        public virtual JsonArray asArray()
        {
            throw new NotSupportedException("Not an array: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as an <code>int</code> value, assuming that this value represents a JSON number that can be interpreted as Java <code>int</code>. If this is not the case, an exception is thrown.
        /// To be interpreted as C# <code>int</code>, the JSON number must neither contain an exponent nor a fraction part. Moreover, the number must be in the <code>Integer</code> range.
        /// </summary>
        /// <returns>this value as <code>int</code></returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON number</exception>
        /// <exception cref="FormatException">if this JSON number can not be interpreted as <code>int</code> value</exception>
        public virtual int asInt()
        {
            throw new NotSupportedException("Not a number: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as a <code>long</code> value, assuming that this value represents a JSON number that can be interpreted as Java <code>long</code>. If this is not the case, an exception is thrown.
        /// To be interpreted as C# <code>long</code>, the JSON number must neither contain an exponent nor a fraction part. Moreover, the number must be in the <code>Long</code> range.
        /// </summary>
        /// <returns>this value as <code>long</code></returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON number</exception>
        /// <exception cref="FormatException">if this JSON number can not be interpreted as <code>long</code> value</exception>
        public virtual long asLong()
        {
            throw new NotSupportedException("Not a number: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as a <code>float</code> value, assuming that this value represents a JSON number. If this is not the case, an exception is thrown.
        /// If the JSON number is out of the <code>Float</code> range, POSITIVE_INFINITY or NEGATIVE_INFINITY is returned.
        /// </summary>
        /// <returns>this value as <code>float</code></returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON number</exception>
        public virtual float asFloat()
        {
            throw new NotSupportedException("Not a number: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as a <code>double</code> value, assuming that this value represents a JSON number. If this is not the case, an exception is thrown.
        /// If the JSON number is out of the <code>Double</code> range, POSITIVE_INFINITY or NEGATIVE_INFINITY is returned.
        /// </summary>
        /// <returns>this value as <code>double</code></returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON number</exception>
        public virtual double asDouble()
        {
            throw new NotSupportedException("Not a number: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as string, assuming that this value represents a JSON string. If this is not the case, an exception is thrown.
        /// </summary>
        /// <returns>the string represented by this value</returns>
        /// <exception cref="NotSupportedException">if this value is not a JSON string</exception>
        /// <seealso cref="ToString()"/>
        public virtual string asString()
        {
            throw new NotSupportedException("Not a string: " + ToString());
        }

        /// <summary>
        /// Returns this JSON value as a <code>bool</code> value, assuming that this value is either <code>true</code> or <code>false</code>. If this is not the case, an exception is thrown.
        /// </summary>
        /// <returns>this value as <code>bool</code></returns>
        /// <exception cref="NotSupportedException">if this value is neither <code>true</code> or <code>false</code></exception>
        public virtual bool asBool()
        {
            throw new NotSupportedException("Not a bool: " + ToString());
        }

        /// <summary>
        /// Writes the JSON representation for this object to the given writer.
        /// </summary>
        /// <param name="writer">the writer to write this value to</param>
        public void writeTo(TextWriter writer)
        {
            write(new JsonWriter(writer));
        }

        /// <summary>
        /// Returns the JSON string for this value in its minimal form, without any additional whitespace.
        /// The result is guaranteed to be a valid input for the method <see cref="readFrom(string)"/> and to create a value that is equal to this object.
        /// </summary>
        /// <returns>a JSON string that represents this value</returns>
        public override string ToString()
        {
            StringWriter stringWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonWriter(stringWriter);
            write(jsonWriter);
            return stringWriter.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Indicates whether some other object is "equal to" this one.
        /// Two JsonValues are considered equal if and only if they represent the same JSON text. As a consequence, two given JsonObjects may be different even though they contain the same set of names with the same values, but in a different order.
        /// </summary>
        /// <param name="obj">the reference object with which to compare</param>
        /// <returns>true if this object is the same as the object argument; false otherwise</returns>
        public override bool Equals(Object obj)
        {
            return base.Equals(obj);
        }

        internal abstract void write(JsonWriter writer);

        private static string cutOffPointZero(string str)
        {
            if (str.EndsWith(".0"))
            {
                return str.Substring(0, str.Length - 2);
            }
            return str;
        }
    }
}