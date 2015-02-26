// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace ShiningDragon.TFSProd.Common.VSCT
{
    public static class PkgCmdIDList
    {
        public const int cmdIdSCECompare = 110;
        public const int cmdIdSCEShowDeleted = 120;
        public const int cmdIdFindInSolExpFromSCE = 130;
        public const int cmdIdFindInSCEFromSolExp = 210;
        public const int cmdIdFindInSCEFromCodeWindow = 310;
        public const int cmdIdFindInSolExpFromCodeWindow = 320;
        public const int cmdIdDynamicCompareToBranchCommand = 2000;
        public const int menuIdCompareToBranchMenu = 140;
        public const int cmdIdBranchBuildDefinition = 410;
    };
}