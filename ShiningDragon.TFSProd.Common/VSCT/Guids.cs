// Guids.cs
// MUST match guids.h
using System;

namespace ShiningDragon.TFSProd.Common.VSCT
{
    public static class GuidList
    {
        public const string guidTFSProductivityPackPkgString = "37237704-391c-429c-9368-85bfd5ab5d53";
        public const string guidTFSProductivityPackCmdSetString = "46238c3e-787a-42ad-aa95-551337be160b";
        public const string guidToolWindowPersistanceString = "31e21920-0aae-41ba-93eb-3195e6e62526";
        
        public static readonly Guid guidTFSProductivityPackCmdSet = new Guid(guidTFSProductivityPackCmdSetString);
    };
}