using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Backend_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LibarySync()
        {
            //If this goes wrong we are fucked
            Assert.AreEqual<int>(42, APIBackend.Database.Testing());
        }

        [TestMethod]
        public void CanConnectToDb()
        {
            APIBackend.Database.Connect();
        }
        
        [TestMethod]
        public void WriteToDb()
        {
            APIBackend.Database.Insert("INSERT INTO accounts SET username='Airwarfare', accountKey='Something'");
        }

        [TestMethod]
        public void UpdateToDb()
        {
            APIBackend.Database.Update("UPDATE accounts SET username='Test', accountKey='This' WHERE username = 'Airwarfare'");
        }

        [TestMethod]
        public void SelectFromDb()
        {
            var dbSelect = APIBackend.Database.Select("SELECT * FROM accounts;");

            string output = "";

            foreach (var c in dbSelect)
            {
                foreach (var s in c)
                {
                    output += " " + s;
                }
            }
            Trace.WriteLine(output);
        }
        
        [TestMethod]
        public void DeleteFromDb()
        {
            APIBackend.Database.Delete("DELETE FROM accounts WHERE username='Test'");
        }
    }

}
