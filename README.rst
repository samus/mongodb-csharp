MongoDB-CSharp
==============
This is a community supported release of a driver to connect to MongoDB using .Net.  It is written entirely in C# and has been tested and developed under both Windows and Mono 2.0 (Ubuntu 32-bit 9.04).  Currently many features have been implemented with a few remaining.  The api is very likely to change and be in flux for a while but is quickly settling down.  At this point it is becoming a solid base to add many more advanced features.

Current Features
================
- Connect to a server.
- Query
- Insert
- Update
- Delete
- All BSON types supported
- DBRef support
- Isolation and conversion between BSON types and native .net types.
- Database, Collection and Cursor objects.
- Index handling routines (List, Create, Drop)
- Count
- Roughly 80% unit test coverage.  This can and will be improved on.
- Paired connections
- Authentication (Does not reauthorize on auto reconnect yet).
- Database Commands
- Basic Linq support
- GridFS support
- Map Reduce helpers.
- hint, explain, $where
- Safemode
- Exceptions
- Connection Pooling

Missing Features
================
- Auto reconnect options
- database profiling: set/get profiling level, get profiling info
- Many unit tests

Installation
============
Currently using the driver in the GAC is not supported.  Simply copy the driver assembly somewhere and reference it in your project.  It should be deployed in your application's bin directory.  It is not necessary to use the test assembly.

Patches
=======
Patches are welcome and will likely be accepted.  By submitting a patch you assign the copyright to me, Sam Corder.  This is necessary to simplify the number of copyright holders should it become necessary that the copyright need to be reassigned or the code relicensed.  The code will always be available under an OSI approved license.

A bug fix patch should contain a test case that reproduces the issue along with the actual fix.  Try to follow the same style that the code is already in so that things remain clean.  Keep your whitespace settings the same as the code to make reading and applying diffs manageable.  We use 4 spaces for tabs and Windows line endings.

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

There is also a growing number of samples in the examples directory.

Getting Help
============
The Google Group mongodb-csharp at (http://groups.google.com/group/mongodb-csharp) is the best place to go.

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
