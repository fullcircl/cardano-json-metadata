using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardanoJsonMetadata.Tests
{
    [TestClass]
    public class TxMetaNumber_ConstructorShould
    {
        [TestMethod]
        public void Constructor_InputIs1_Works()
        {
            var number = new TxMetaNumber(1);
            Assert.IsInstanceOfType(number, typeof(TxMetaNumber));
        }
    }
}
