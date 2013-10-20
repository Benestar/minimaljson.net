/*******************************************************************************
 * Copyright (c) 2008, 2013 EclipseSource.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.IO;

namespace MinimalJson
{
    /// <summary>
    /// Represents a JSON array. A JSON array is a sequence of elements, which are JSON values (see <see cref="JsonValue"/>).
    /// Elements can be added using one of the <code>add(name, value)</code> methods. Accepted values are either instances of <see cref="JsonValue"/>, strings, primitive numbers, or bool values. To replace an element of an array, the <code>set(name, value)</code> methods can be used.
    /// Elements can be accessed by their index using <see cref="get(int)"/>. This class also supports iterating over the elements in document order:
    /// <example>
    /// foreach(JsonValue value in jsonArray)
    /// {
    ///     ...
    /// }
    /// </example>
    /// An equivalent List can be obtained from the method <see cref="values()"/>.
    /// Note that this class is not thread-safe. If multiple threads access a <code>JsonArray</code> instance concurrently, while at least one of these threads modifies the contents of this array, access to the instance must be synchronized externally. Failure to do so may lead to an inconsistent state.
    /// This class is <strong>not supposed to be extended</strong> by clients.
    /// </summary>
    public sealed class JsonArray : JsonValue, IEnumerable
    {

        private readonly IList<JsonValue> values;

        /// <summary>
        /// Creates a new empty JsonArray.
        /// </summary>
        public JsonArray()
        {
            values = new List<JsonValue>();
        }

        /// <summary>
        /// Creates a new JsonArray with the contents of the specified JSON array.
        /// </summary>
        /// <param name="array">the JsonArray to get the initial contents from, must not be <code>null</code></param>
        public JsonArray(JsonArray array) : this(array, false) { }

        private JsonArray(JsonArray array, bool unmodifiable)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (unmodifiable)
            {
                values = new ReadOnlyCollection<JsonValue>(array.values);
            }
            else
            {
                values = new List<JsonValue>(array.values);
            }
        }

        /// <summary>
        /// Reads a JSON array from the given reader.
        /// Characters are read in chunks and buffered internally, therefore wrapping an existing reader in an additional BufferedReader does not improve reading performance.
        /// </summary>
        /// <param name="reader">the reader to read the JSON array from</param>
        /// <returns>the JSON array that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        /// <exception cref="NotSupportedException">if the input does not contain a JSON array</exception>
        public static new JsonArray readFrom(TextReader reader)
        {
            return JsonValue.readFrom(reader).asArray();
        }

        /// <summary>
        /// Reads a JSON array from the given string.
        /// </summary>
        /// <param name="str">the string that contains the JSON array</param>
        /// <returns>the JSON array that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        /// <exception cref="NotSupportedException">if the input does not contain a JSON array</exception>
        public static new JsonArray readFrom(string str)
        {
            return JsonValue.readFrom(str).asArray();
        }

        /// <summary>
        /// Returns an unmodifiable wrapper for the specified JsonArray. This method allows to provide read-only access to a JsonArray.
        /// The returned JsonArray is backed by the given array and reflects subsequent changes.
        /// </summary>
        /// <param name="array">the JsonArray for which an unmodifiable JsonArray is to be returned</param>
        /// <returns>an unmodifiable view of the specified JsonArray</returns>
        public static JsonArray unmodifiableArray(JsonArray array)
        {
            return new JsonArray(array, true);
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>int</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(int value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>long</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(long value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>float</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(float value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>double</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(double value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>bool</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(bool value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified <code>string</code> value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array</param>
        /// <returns>the array itself, to enable method chaining</returns>
        public JsonArray add(string value)
        {
            values.Add(valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds the JSON representation of the specified JSON value to the array.
        /// </summary>
        /// <param name="value">the value to add to the array, must not be <code>null</code></param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentNullException">if the value is <code>null</code></exception>
        public JsonArray add(JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value is null");
            }
            values.Add(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified <code>long</code> value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position</param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, long value)
        {
            values[index] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified <code>float</code> value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position</param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, float value)
        {
            values[index] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified <code>double</code> value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position</param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, double value)
        {
            values[index] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified <code>bool</code> value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position</param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, bool value)
        {
            values[index] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified <code>string</code> value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position</param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, string value)
        {
            values[index] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Replaces the element at the specified position in this array with the JSON representation of the specified JSON value.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <param name="value">the value to be stored at the specified array position, must not be <code>null</code></param>
        /// <returns>the array itself, to enable method chaining</returns>
        /// <exception cref="ArgumentNullException">if the value is <code>null</code></exception>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonArray set(int index, JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            values[index] = value;
            return this;
        }

        /// <summary>
        /// Removes the first appereance of the value in this array.
        /// </summary>
        /// <param name="value">the value to be removed from this array</param>
        /// <returns>true if the value was successfully removed, false if it didn't exist</returns>
        public bool remove(JsonValue value)
        {
            return values.Remove(value);
        }

        /// <summary>
        /// Removes the element at the specified position in this array.
        /// </summary>
        /// <param name="index">the index of the array element to replace</param>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public void removeAt(int index)
        {
            values.RemoveAt(index);
        }

        /// <summary>
        /// Returns the value of the element at the specified position in this array.
        /// </summary>
        /// <param name="index">the index of the array element to return</param>
        /// <returns>the value of the element at the specified position</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range, i.e. <code>index &lt; 0</code> or <code>index >= size</code></exception>
        public JsonValue get(int index)
        {
            return values[index];
        }

        /// <summary>
        /// Returns the number of elements in this array.
        /// </summary>
        /// <returns>the number of elements in this array</returns>
        public int size()
        {
            return values.Count;
        }

        /// <summary>
        /// Returns <code>true</code> if this array contains no elements.
        /// </summary>
        /// <returns><code>true</code> if this array contains no elements</returns>
        public bool isEmpty()
        {
            return values.Count == 0;
        }

        /// <summary>
        /// Returns a list of the values in this array in document order. It cannot be used to modify this array.
        /// </summary>
        /// <returns>a list of the values in this array</returns>
        public IList<JsonValue> getValues()
        {
            return new ReadOnlyCollection<JsonValue>(values);
        }

        internal override void write(JsonWriter writer)
        {
            writer.writeArray(this);
        }

        public override bool isArray()
        {
            return true;
        }

        public override JsonArray asArray()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return values.GetHashCode();
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
            JsonArray other = (JsonArray)obj;
            return this.ToString() == other.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
