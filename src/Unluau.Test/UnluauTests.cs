// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unluau.Test
{
    public struct TestSettings
    {
        public bool StringInterpolation { get; set; } = false;
        public bool InlinedTables { get; set; } = true;

        public TestSettings()
        {
        }
    }

    [TestClass]
    public class UnluauTests
    { 
        private Stream OpenRead(string projectPath)
        {
            return File.OpenRead($"../../../{projectPath}");
        }

        private string GetCode(string FileName, TestSettings settings)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Stream stream = OpenRead(FileName))
            {
                Decompiler decompiler = new Decompiler(stream, new DecompilerOptions()
                {
                    Output = new Output(new StreamWriter(memoryStream)),
                    HeaderEnabled = false,
                    VariableNameGuessing = true,
                    InlineTableDefintions = settings.InlinedTables,
                    RenameUpvalues = true,
                    PerferStringInterpolation = settings.StringInterpolation
                });

                decompiler.Decompile();

                return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        private void GetAndAssert(string binary, string expect, TestSettings? settings = null)
        {
            string actual = GetCode(binary, (TestSettings)(settings is null ? new TestSettings() : settings));
            string expected = new StreamReader(OpenRead(expect)).ReadToEnd();

            string[] actual_lines = actual.Split('\n').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToArray();
            string[] expected_lines = expected.Split('\n').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToArray();

            for (int i = 0; i < actual_lines.Length; i++)
            {
                Assert.AreEqual(expected_lines[i].Trim(), actual_lines[i].Trim(), $"Line {i + 1} does not match");
            }

            Assert.AreEqual(actual_lines.Length, expected_lines.Length, "Incorrect number of lines");
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
        public void Test_ExpressionAndVararg()
        {
            GetAndAssert("Binary/Vararg03.luau", "Expect/Vararg03.lua");
        }

        [TestMethod]
        public void Test_ExpressionAndExtraVararg()
        {
            GetAndAssert("Binary/Vararg04.luau", "Expect/Vararg04.lua");
        }

        [TestMethod]
        public void Test_StringInterpolationWithFormat()
        {
            GetAndAssert("Binary/StringInterpolation.luau", "Expect/StringInterpolation01.lua");
        }

        [TestMethod]
        public void Test_StringInterpolation()
        {
            GetAndAssert("Binary/StringInterpolation.luau", "Expect/StringInterpolation02.lua", new TestSettings() 
            { 
                StringInterpolation = true 
            });
        }

        [TestMethod]
        public void Test_BinaryOperations()
        {
            GetAndAssert("Binary/BinaryExpressions.luau", "Expect/BinaryExpressions.lua");

            GetAndAssert("Binary/BinaryExpressionSimple.luau", "Expect/BinaryExpressionSimple.lua");
        }

        [TestMethod]
        public void Test_DummyLuauSample()
        {
            GetAndAssert("Binary/LuauSample.luau", "Expect/LuauSample.lua", new TestSettings() 
            { 
                InlinedTables = false, 
                StringInterpolation = true 
            });
        }

        [TestMethod]
        public void Test_BooleanAssignment()
        {
            GetAndAssert("Binary/BooleanAssign01.luau", "Expect/BooleanAssign01.lua");
        }

        [TestMethod]
        public void Test_RepeatUntil()
        {
            GetAndAssert("Binary/RepeatUntil01.luau", "Expect/RepeatUntil01.lua");
        }

        [TestMethod]
        public void Test_NumericForLoop()
        {
            GetAndAssert("Binary/NumericForLoop.luau", "Expect/NumericForLoop.lua");
        }

        [TestMethod]
        public void Test_GenericForLoopPairs()
        {
            GetAndAssert("Binary/GenericForLoopPairs.luau", "Expect/GenericForLoopPairs.lua");
        }
    }
}
