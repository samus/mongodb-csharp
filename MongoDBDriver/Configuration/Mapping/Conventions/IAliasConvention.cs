using System;
using System.Reflection;

namespace MongoDB.Driver.Configuration.Mapping.Conventions
{
    public interface IAliasConvention
    {
        string GetAlias(MemberInfo member);
    }
}