# ExposedObject

Fast dynamic wrapper for accessing hidden methods and fields of .Net objects. Uses 4.0 `dynamic` feature to allow seamless access to non-public object members. Facilitates white-box unit testing, exposes APIs that should be public and allows construction of elaborate hacks. Should not kill your cat provided you are careful. Available on [NuGet](http://nuget.org/packages/ExposedObject).

## Usage examples

Handling private instance methods, fields and properties on a visible type:
```csharp
// create an Exposed instance from a ClassWithHiddenMethods instance
dynamic exposed = Exposed.From(new ClassWithHiddenMethods());
// calling a private method
string password = exposed.GeneratePassword(8);
// reading a private field
int privateFieldValue = exposed.internalCount;
// setting a private field
exposed.internalCount = privateFieldValue * 2;
// reading a protected property
char protectedPropertyValue = exposed.InternalData;
```

Accessing private field on a hidden type:
```csharp
// get the Type via reflection
Type hiddenClass = Type.GetType("TestSubjects.HiddenClass, TestSubjects");
// call the parameterless constructor of a hidden type
dynamic exposed = Exposed.New(hiddenClass);
string password = exposed.password;
```

Accessing internal static method from type:
```csharp
dynamic exposed = Exposed.From(typeof(ClassWithInternalStaticMethod));
decimal convertValue = exposed.ConvertValue(8);
```

## TODO:

 - performance assessment (binding restrictions vs caching, compare with [CreateDelegate](http://msmvps.com/blogs/jon_skeet/archive/2008/08/09/making-reflection-fly-and-exploring-delegates.aspx) solution)
 - generic methods (there are tests for them, some suggested solutions on [StackOverflow](http://stackoverflow.com/questions/6954069/how-can-i-handle-generic-method-invocations-in-my-dynamicobject) )
 - constructors (currently `Exposed.New()` delegates to `Activator.CreateInstance()` )

## Ideas borrowed from:

 - [Bug squash: Testing private methods with C# 4.0](http://bugsquash.blogspot.com/2009/05/testing-private-methods-with-c-40.html) - starting idea
 - [Igor Ostrovsky: Use C# dynamic typing to conveniently access internals of an object](http://igoro.com/archive/use-c-dynamic-typing-to-conveniently-access-internals-of-an-object/) - convenient API
 - [Miguel de Icaza: C# 4's Dynamic in Mono](http://tirania.org/blog/archive/2009/Aug-11.html) - performant dynamic object implementation using [`GetMetaObject()`](http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.getmetaobject)
