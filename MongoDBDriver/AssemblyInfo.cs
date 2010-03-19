using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;


// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("MongoDBDriver")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("1.0.*")]

// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.

[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: CLSCompliantAttribute(true)]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bson")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bson")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

[assembly: InternalsVisibleTo("MongoDB.Driver.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ed9e936c4563336be2e14ca802ea727ff49cad3bb1c0b287beed2a9b5eb823c4c44becc80be4bb11dcd7e49d5d6171f68b488853dcbdeb3152ea3db95ba13a70855a715ee21ac76b67f50bcbc93f2e29e409530a00b98fa79b06ac008dd1f4f3582ba6746af3d218b43b70a63254b094be1a2d493590837273f357fc56b2a7a0")]