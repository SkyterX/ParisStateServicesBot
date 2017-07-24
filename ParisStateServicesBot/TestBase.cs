using NUnit.Framework;

namespace ParisStateServicesBot
{
    [TestFixture]
    public class TestBase
    {
        protected TheFactory Factory;

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Factory = new TheFactory();
        }

        [OneTimeTearDown]
        public virtual void TearDown()
        {
            Factory.Dispose();
        }
    }
}