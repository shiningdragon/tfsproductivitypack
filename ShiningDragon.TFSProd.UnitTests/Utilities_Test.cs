using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using ShiningDragon.TFSProd.Common;

namespace ShiningDragon.TFSProd.UnitTests
{
    [TestClass]
    public class Utilities_Test
    {
        [TestMethod]
        public void ReplaceTFSServerPaths_Test()
        {
            string input = @"blah$/downCOUNT/main\Enginerah";
            string pattern = @"$/Downcount/Main/Engine";
            string replacement = @"$/Downcount/Release1.0\Engine";

            string output = Utilities.ReplaceTFSServerPaths(input, pattern, replacement);

            Assert.IsTrue(output == @"blah$/Downcount/Release1.0\Enginerah");

            input = @"blah$\#down.COUNT +  edge$()?/main\Enginerah";
            pattern = @"$/#Down.count +  edge$()?/Main/Engine";
            replacement = @"$/Downcount/Release1.0\Engine";

            output = Utilities.ReplaceTFSServerPaths(input, pattern, replacement);

            Assert.IsTrue(output == @"blah$/Downcount/Release1.0\Enginerah");
        }
    }
}
