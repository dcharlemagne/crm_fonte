using NUnit.Framework;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class SDKoreTeste
    {
        [Test]
        public void SaveTrace() {
            var trace = new SDKore.Helper.Trace("Intelbras");
            trace.Add("Add 01");
            trace.Add("Add 02");
            trace.Save();
        }
    }
}
