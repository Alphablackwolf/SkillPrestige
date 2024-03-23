using NUnit.Framework;

namespace SkillPrestige.UnitTests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void SetInstanceFieldOfBase_SetsPrivateBaseClassProperty()
        {
            var item = new DerivedClass();
            Assert.That(item.GetCost == 2);
            item.SetProperty();
            Assert.That(item.GetCost == 5);
        }

        private class BaseClass
        {
            private readonly int _cost;

            // ReSharper disable once ConvertToAutoProperty - test requires the 'cost' property be private.
            public int GetCost => this._cost;

            protected BaseClass()
            {
                this._cost = 2;
            }
        }

        private class DerivedClass : BaseClass
        {
            public void SetProperty()
            {
                this.SetInstanceFieldOfBase("_cost", 5);
            }
        }
    }
}
