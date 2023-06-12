using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unluau.Test
{
    [TestClass]
    public class UnluauTests
    { 
        private Stream OpenRead(string projectPath)
        {
            return File.OpenRead($"../../../{projectPath}");
        }

        private string GetCode(string FileName)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Stream stream = OpenRead(FileName))
            {
                Decompiler decompiler = new Decompiler(stream, new DecompilerOptions()
                {
                    Output = new Output(new StreamWriter(memoryStream)),
                    HeaderEnabled = false
                });

                decompiler.Decompile();

                return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Trim();
            }
        }

        [TestMethod]
        public void Multiple_Namecall()
        {
            string actual = GetCode("Binary/MultipleNamecallStatements.luau");

            string expected = "local var4 = game:GetService(\"Players\")\nlocal var8 = game:GetService(\"Misc\")";

            Assert.AreEqual(expected, actual);
        }
    }
}
