using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateCookingSkill()
        {
            var skill = new CookingSkillPrestigeAdapter.CookingSkillMod();
            Assert.IsTrue(skill != null);
        }
    }
}
