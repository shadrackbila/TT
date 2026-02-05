using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimelyTastes.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        int expected = 6;
        int actual = 2 + 3;

        Assert.AreEqual(expected, actual);
    }
}
