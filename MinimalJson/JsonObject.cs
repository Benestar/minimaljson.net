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
    /// Represents a JSON object. A JSON object contains a sequence of members, which are pairs of a name and a JSON value (see <see cref="JsonValue"/>). Although JSON objects should be used for unordered collections, this class stores members in document order.
    /// Members can be added using one of the <code>add(name, value)</code> methods. Accepted values are either instances of <see cref="JsonValue"/>, strings, primitive numbers, or bool values. To override values in an object, the <code>set(name, value)</code> methods can be used. However, not that the <code>add</code> methods perform better than <code>set</code>.
    /// Members can be accessed by their name using <see cref="get(string)"/>. A list of all names can be obtained from the method <see cref="names()"/>.
    /// This class also supports iterating over the members in document order:
    /// <example>
    /// foreach(JsonObject.Member member in jsonObject)
    /// {
    ///     string name = member.name;
    ///     JsonValue value = member.value;
    ///     ...
    /// }
    /// </example>
    /// Note that this class is not thread-safe. If multiple threads access a <code>JsonObject</code> instance concurrently, while at least one of these threads modifies the contents of this object, access to the instance must be synchronized externally. Failure to do so may lead to an inconsistent state.
    /// This class is not supposed to be extended by clients.
    /// </summary>
    public sealed class JsonObject : JsonValue, IEnumerable
    {

        private IDictionary<string, JsonValue> values;

        /// <summary>
        /// Creates a new empty JsonObject.
        /// </summary>
        public JsonObject()
        {
            values = new Dictionary<string, JsonValue>();
        }

        /// <summary>
        /// Creates a new JsonObject, initialized with the contents of the specified JSON object.
        /// </summary>
        /// <param name="obj">the JSON object to get the initial contents from, must not be <code>null</code></param>
        public JsonObject(JsonObject obj) : this(obj, false) { }

        private JsonObject(JsonObject obj, bool unmodifiable)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (unmodifiable)
            {
                values = new ReadOnlyDictionary<string, JsonValue>(obj.values);
            }
            else
            {
                values = new Dictionary<string, JsonValue>(obj.values);
            }
        }

        public JsonValue this[string name]
        {
            get { return values[name]; }
            set { values[name] = value; }
        }

        /// <summary>
        /// Reads a JSON object from the given reader.
        /// Characters are read in chunks and buffered internally, therefore wrapping an existing reader in an additional BufferedReader does not improve reading performance.
        /// </summary>
        /// <param name="reader">the reader to read the JSON object from</param>
        /// <returns>the JSON object that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        /// <exception cref="NotSupportedException">if the input does not contain a JSON object</exception>
        public static new JsonObject readFrom(TextReader reader)
        {
            return JsonValue.readFrom(reader).asObject();
        }

        /// <summary>
        /// Reads a JSON object from the given string.
        /// </summary>
        /// <param name="str">the string that contains the JSON object</param>
        /// <returns>the JSON object that has been read</returns>
        /// <exception cref="ParseException">if the input is not valid JSON</exception>
        /// <exception cref="NotSupportedException">if the input does not contain a JSON object</exception>
        public static new JsonObject readFrom(string str)
        {
            return JsonValue.readFrom(str).asObject();
        }

        /// <summary>
        /// Returns an unmodifiable JsonObject for the specified one. This method allows to provide read-only access to a JsonObject.
        /// The returned JsonObject is backed by the given object and reflect changes that happen to it.
        /// </summary>
        /// <param name="obj">the JsonObject for which an unmodifiable JsonObject is to be returned</param>
        /// <returns>an unmodifiable view of the specified JsonObject</returns>
        public static JsonObject unmodifiableObject(JsonObject obj)
        {
            return new JsonObject(obj, true);
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified <code>int</code> value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, int value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified <code>long</code> value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, long value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified <code>float</code> value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, float value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified <code>double</code> value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, double value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified <code>bool</code> value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, bool value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the JSON representation of the specified string.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value"> the value of the member to add</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject add(string name, string value)
        {
            values.Add(name, valueOf(value));
            return this;
        }

        /// <summary>
        /// Adds a new member at the end of this object, with the specified name and the specified JSON value.
        /// </summary>
        /// <param name="name">the name of the member to add</param>
        /// <param name="value">the value of the member to add, must not be <code>null</code></param>
        /// <returns>the object itself, to enable method chaining</returns>
        /// <exception cref="ArgumentNullException">if the value is <code>null</code></exception>
        public JsonObject add(string name, JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            values.Add(name, value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>int</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, int value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>long</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, long value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>float</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, float value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>double</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, double value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>bool</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, bool value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified <code>string</code> value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject set(string name, string value)
        {
            values[name] = valueOf(value);
            return this;
        }

        /// <summary>
        /// Sets the value of the member with the specified name to the JSON representation of the specified JSON value.
        /// If this object does not contain a member with this name, a new member is added at the end of the object.
        /// </summary>
        /// <param name="name">the name of the member to replace</param>
        /// <param name="value">the value to set to the member, must not be <code>null</code></param>
        /// <returns>the object itself, to enable method chaining</returns>
        /// <exception cref="ArgumentNullException">if the value is <code>null</code></exception>
        public JsonObject set(string name, JsonValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            values[name] = value;
            return this;
        }

        /// <summary>
        /// Removes a member with the specified name from this object. If this object does not contain a member with the specified name, the object is not modified.
        /// </summary>
        /// <param name="name">the name of the member to remove</param>
        /// <returns>the object itself, to enable method chaining</returns>
        public JsonObject removeAt(string name)
        {
            values.Remove(name);
            return this;
        }

        /// <summary>
        /// Returns the value of the member with the specified name in this object.
        /// </summary>
        /// <param name="name">the name of the member whose value is to be returned</param>
        /// <returns>the value of the last member with the specified name, or <code>null</code> if this object does not contain a member with that name</returns>
        public JsonValue get(string name)
        {
            return values.ContainsKey(name) ? values[name] : null;
        }

        /// <summary>
        /// Returns the number of members (i.e. name/value pairs) in this object.
        /// </summary>
        /// <returns>the number of members in this object</returns>
        public int size()
        {
            return values.Count;
        }

        /// <summary>
        /// Returns <code>true</code> if this object contains no members.
        /// </summary>
        /// <returns><code>true</code> if this object contains no members</returns>
        public bool isEmpty()
        {
            return values.Count == 0;
        }

        /// <summary>
        /// Returns a list of the names in this object in document order. It cannot be used to modify this object.
        /// </summary>
        /// <returns>a list of the names in this object</returns>
        public ICollection<string> names()
        {
            return values.Keys;
        }

        internal override void write(JsonWriter writer)
        {
            writer.writeObject(this);
        }

        public override bool isObject()
        {
            return true;
        }

        public override JsonObject asObject()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return values.GetHashCode();
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
            JsonObject other = (JsonObject)obj;
            return this.ToString() == other.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JsonValue> member in values)
            {
                yield return new Member(member.Key, member.Value);
            }
        }

        /// <summary>
        /// Represents a member of a JSON object, i.e. a pair of name and value.
        /// </summary>
        public class Member
        {
            /// <summary>
            /// The name of this member
            /// </summary>
            public string name { get; private set; }

            /// <summary>
            /// The value of this member
            /// </summary>
            public JsonValue value { get; private set; }

            internal Member(string name, JsonValue value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}
