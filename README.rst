MongoDB-CSharp
==============
This is a preliminary release of a driver to connect to MongoDB using .Net.  It is written entirely in C# and has been tested and developed under both Windows and Mono 2.0 (Ubuntu 32-bit 9.04).  Currently only basic features have been implemented.  The api is very likely to change and be in flux for a while.  There are many features to be added and refined.

Current Features
================
- Connect to a server.
- Query
- Insert
- Update
- Delete
- Most BSON types supported
- DBRef support
- Isolation and conversion between BSON types and native .net types.
- Database, Collection and Cursor objects.
- Index handling routines (List, Create, Drop)
- Count
- Roughly 80% unit test coverage.  This can and will be improved on.

Missing Features
================
- Paired connections
- Auto reconnect options
- Connection pooling
- Authentication
- A few BSON Types (easy to add)
- Database commands (can send queries to $cmd if necessary)
- Exceptions
- hint, explain, $where
- database profiling: set/get profiling level, get profiling info
- GridFS support
- Many unit tests

Installation
============
Currently using the driver in the GAC is not supported.  Simply copy the driver assembly somewhere and reference it in your project.  It should be deployed in your application's bin directory.  It is not necessary to use the test assembly.

Patches
=======
Patches are welcome and will likely be accepted.  By submitting a patch you assign the copyright to me, Sam Corder.  This is necessary to simplify the number of copyright holders should it become necessary that the copyright need to be reassigned or the code relicensed.  The code will always be available under an OSI approved license.

Usage
=====
One of the best sources for how to use the driver is the unit tests.  Basic usage can be found in the TestCollection set of test cases.

At the simplest query the database like this:
 using MongoDB.Driver;
 Mongo db = new Mongo();
 db.Connect(); //Connect to localhost on the default port.
 Document query = new Document();
 query["field1"] = 10;
 Document result = db["tests"]["reads"].FindOne(query);
 db.Disconnect();


