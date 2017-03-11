# NanoSerializer
NanoSerializer is super fast and compact binary data contract serializer

##Performance to de/serialize 100 000 objects:
* NanoSerializer	370 ms
* Google ProtoBuf	430 ms
* Newtonsoft.JSON	800 ms

* NanoSerializer Size:  529 bytes
* Google ProtoBuf Size: 543 bytes
* Newtonsoft.JSON Size: 781 bytes

### Usage
```C#
var serializer = Serializer.Build<TestContract>();  //Create a nano serializer for type

var data = serializer.Serialize(instance);      //Serialize instance of type
var instance = serializer.Deserialize(data);    //Deserialize data to new instance
```

