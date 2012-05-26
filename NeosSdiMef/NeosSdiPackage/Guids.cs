// Guids.cs
// MUST match guids.h
using System;

namespace NeosSDI.NeosSdiPackage
{
    static class GuidList
    {
        public const string guidNeosSdiPackagePkgString = "ab7446fa-d643-4e12-84e2-9289e44f4880";
        public const string guidNeosSdiPackageCmdSetString = "0767792d-7fc3-4461-a36e-823f32c8af58";

        public static readonly Guid guidNeosSdiPackageCmdSet = new Guid(guidNeosSdiPackageCmdSetString);
    };
}