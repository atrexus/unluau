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
                    HeaderEnabled = false,
                    VariableNameGuessing = true,
                    InlineTableDefintions = true,
                    RenameUpvalues = true
                });

                decompiler.Decompile();

                return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Trim();
            }
        }

        private void GetAndAssert(string binary, string expect)
        {
            string actual = GetCode(binary);
            string expected = new StreamReader(OpenRead(expect)).ReadToEnd();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_Namecall()
        {
            GetAndAssert("Binary/Namecall.luau", "Expect/Namecall.lua");
        }

        [TestMethod]
        public void Test_WhileLoops()
        {
            GetAndAssert("Binary/WhileLoops.luau", "Expect/WhileLoops.lua");
        }


        [TestMethod]
        public void Test_DictionaryTable()
        {
            GetAndAssert("Binary/Tables01.luau", "Expect/Tables01.lua");
        }

        [TestMethod]
        public void Test_ListTable()
        {
            GetAndAssert("Binary/Tables02.luau", "Expect/Tables02.lua");
        }

        [TestMethod]
        public void Test_luauDecExampleBasic()
        {
            GetAndAssert("Binary/luauDecBasic.luau", "Expect/luauDecBasic.lua");
        }

        [TestMethod]
        public void Test_BasicUpvalues()
        {
            GetAndAssert("Binary/Upvalue01.luau", "Expect/Upvalue01.lua");
        }

        [TestMethod]
        public void Test_LocalReassignment()
        {
            GetAndAssert("Binary/LocalReassign.luau", "Expect/LocalReassign.lua");
        }

        [TestMethod]
        public void Test_StringWithEscapeSequences()
        {
            GetAndAssert("Binary/String01.luau", "Expect/String01.lua");
        }
        
        [TestMethod]
        public void Test_StringWithEscapeSequencesAndLong()
        {
            GetAndAssert("Binary/String02.luau", "Expect/String02.lua");
        }

        [TestMethod]
        public void Test_BasicVararg()
        {
            GetAndAssert("Binary/Vararg01.luau", "Expect/Vararg01.lua");
        }

        [TestMethod]
        public void Test_LocalAbsorbVararg()
        {
            GetAndAssert("Binary/Vararg02.luau", "Expect/Vararg02.lua");
        }

        [TestMethod]
        public void Test_StringInterpolation()
        {
            GetAndAssert("Binary/StringInterpolation.luau", "Expect/StringInterpolation.lua");
        }
    }
}
