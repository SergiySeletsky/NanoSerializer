# NanoSerializer
NanoSerializer is super fast and compact binary data contract serializer for .NET Standard 2.1

## Performance to de/serialize 100 000 objects:
* NanoSerializer	370 ms
* Google ProtoBuf	430 ms
* Newtonsoft.JSON	800 ms

* NanoSerializer Size:  529 bytes
* Google ProtoBuf Size: 543 bytes
* Newtonsoft.JSON Size: 781 bytes

### Usage
```C#
var serializer = new Serializer(typeof(TestContract));  //Create a nano serializer for all types with DataContract attribute in assembly
var data = serializer.Serialize(instance);              //Serialize instance of type
var instance = serializer.Deserialize<TestContract>(data);    //Deserialize data to new instance
```
#### Note: Solution can be opened and compiled only in VS2017
#### NuGet packege will be availible soon - so you can just use Serializer.cs in your project for now
#### Support of all .NET types will be implemented in near future
