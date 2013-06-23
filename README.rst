MongoDB-CSharp
==============
This is a community supported driver to connect to MongoDB using .Net. It is written entirely in C# and has been tested and developed under both Windows and Mono 2.0 (Ubuntu 32-bit 9.04). Its API is similar to the Mongo Client one. At this point it is becoming a solid base to add many more advanced features.

Home: http://github.com/mongodb-csharp/mongodb-csharp

A few of its Features
================
- All BSON types supported + DBRef
- Isolation and conversion between BSON types and native .net types.
- Query,Insert,Update,Delete
- Connection Pooling
- Typed Collections + Mapping
- Linq support
- Index handling routines (List, Create, Drop)
- Connection Strings
- Authentication 
- Database Commands
- GridFS support
- Map Reduce support
- Count, Hint, Explain, $where
- Safemode

Downloads
============
The latest binary version can be found here: http://github.com/mongodb-csharp/mongodb-csharp/downloads
The source related to that versions can be found on that page at the bottom.

Installation
============
Simply copy the driver assembly somewhere and reference it in your project. It should be deployed in your application's bin directory. It is not necessary to use the test assembly.

Patches
=======
Patches are welcome and will likely be accepted. By submitting a patch you assign the copyright to me, Sam Corder. This is necessary to simplify the number of copyright holders should it become necessary that the copyright need to be reassigned or the code relicensed. The code will always be available under an OSI approved license.

A bug fix patch should contain a test case that reproduces the issue along with the actual fix. Try to follow the same style that the code is already in so that things remain clean.  Keep your whitespace settings the same as the code to make reading and applying diffs manageable We use 4 spaces for tabs and Windows line endings.

Getting Started
=====
The best point to start is our Wiki at http://github.com/mongodb-csharp/mongodb-csharp/wiki There is also a FAQ section where you should look if you have problems. 
Another good source for how to use the driver is its unit tests. Basic usage can be found in the TestCollection set of test cases.

At the simplest query the database like this:
```csharp
 using MongoDB;
 using(Mongo mongo = new Mongo("localhost"))
 {
	 mongo.Connect(); 
	 var database = mongo.GetDatabase("database");
	 var collection = database.GetCollecton("collection");
	 //or short collection= mongo["database"]["collection"]
	 Document result = collection.FindOne(new Document("field1",10));	 
	 Console.WriteLine(result["value"]);
 }
```

There is also a growing number of samples in the examples directory.

Getting Help
============
The Google Group MongoDB-CSharp at (http://groups.google.com/group/mongodb-csharp) is the best place to go.

Reporting Bugs
==============
The bug tracker is the same as the Mongodb bug tracker located at http://jira.mongodb.org

Contributors
============
- Sam Corder (samus)
- Steve Wagner (lanwin)
- Craig Wilson (craiggwilson)
- Seth Edwards (Sedward)
- Arne Classen (Sdether)
- Andrew Rondeau (GWBasic)
- Doug Mayer (dougtmayer)
- Andrew Kempe
- Kevin Smith (codebrulee)
- Rashadh (rashadh)
- Sergey Bartunov (sbos)
- David O'Hara (davidmohara)
- Tim Raybrun (trayburn)
