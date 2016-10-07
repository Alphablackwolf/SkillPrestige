using System;
using System.Linq;
using CookingSkillPrestigeAdapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var skill = new CookingSkillMod();
            var ids = skill.AdditionalSkills.SelectMany(x => x.GetAllProfessionIds()).ToList();
            Assert.IsTrue(ids.Any());
        }
    }
}
