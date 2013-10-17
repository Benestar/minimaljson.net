minimaljson.net
===============

minimaljson.net is a Json parser written in C# which works for DotNetFramework v2.0 or later. It is based on the minimaljson parser from the eclipse opensource project and a very good explanation can be found [here](http://eclipsesource.com/blogs/2013/04/18/minimal-json-parser-for-java/). Besides some modifications the use and syntax is the same.

Code Examples
-------------

### Read JSON from a String or a Reader:

Reading is buffered already, so you *don't* need to wrap your reader in a BufferedReader.

```csharp
JsonObject jsonObject = JsonObject.readFrom(string);
JsonArray jsonArray = JsonArray.readFrom(reader);
```

### Access the contents of a JSON object:

```csharp
string name = jsonObject.get("name").asString();
int age = jsonObject.get("age").asInt(); // asLong(), asFloat(), asDouble(), ...

// or iterate over the members:
foreach(JsonObject.Member member in jsonObject)
{
  string name = member.name;
  JsonValue value = member.value;
  // ...
}
```

### Access the contents of a JSON array:

```csharp
String name = jsonArray.get(0).asString();
int age = jsonArray.get(1).asInt(); // asLong(), asFloat(), asDouble(), ...

// or iterate over the values:
foreach(JsonValue value in jsonArray)
{
  // ...
}
```

### Create JSON objects and arrays:

```csharp
JsonObject jsonObject = new JsonObject().add("name", "John").add("age", 23);
JsonArray jsonArray = new JsonArray().add("John").add( 23 );
```

### Write JSON to a Writer:

```csharp
jsonObject.writeTo(writer);
jsonArray.writeTo(writer);
```

### Export JSON as a String:

```csharp
jsonObject.toString();
jsonArray.toString();
```

License
-------

The code is available under the terms of the [Eclipse Public License](http://www.eclipse.org/legal/epl-v10.html).
