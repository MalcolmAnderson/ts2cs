using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;


namespace ts2cs
{
    public class FileHandler
    {
        public int GetDirectoryCount(string directoryName)
        {
            string[] directories = Directory.GetDirectories(directoryName);
            return directories.Length;
        }

        public string[] getFileContents(string fileNameAndPath)
        {
            //string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");
            string[] lines = System.IO.File.ReadAllLines(fileNameAndPath);
            return lines;
        }

        public string[] parseContents(string[] lines)
        {
            List<string> outList = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("function"))
                {
                    outList.Add(ParseFunctionLine(lines[i]));
                    outList.Add("{");
                }
                else
                {
                    outList.Add(lines[i]);
                }
            }
            string[] outArray = outList.ToArray();
            return outArray;
        }

        public void WriteContentsToFile(string[] lines, string fileNameAndPath)
        {
            //System.IO.File.WriteAllLines(@"c:\ts2cs_files-Old\testFile.txt", lines);
            System.IO.File.WriteAllLines(fileNameAndPath, lines);
        }

        public void WriteALineOfTextToAFile()
        {
            string line = "This is a line of text";
            FileStream fs = File.OpenWrite("testFile.txt");
            // Example #1: Write an array of strings to a file.
            // Create a string array that consists of three lines.
            string[] lines = { "First line", "Second line", "Third line", line };
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);
            System.IO.File.WriteAllLines(@"c:\ts2cs_files-Old\testFile.txt", lines);
        }

        public string[] GetParameters(string inputLine)
        {
            //string expected = @"string[] addBorder(string[] picture){";
            // string inputLine = @"function addBorder(picture: string[], length: number, name: string): string[] {";
            int start = inputLine.LastIndexOf('(') + 1;
            int end = inputLine.LastIndexOf(')');
            int strLength = end - start;
            string allParams = inputLine.Substring(start, strLength);
            string[] outputArray = allParams.Split(',');
            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = outputArray[i].Trim();
            }
            return outputArray;
        }

        public string GetMethodReturnType(string inputLine)
        {
            int start = inputLine.LastIndexOf(':') + 1;
            int end = inputLine.LastIndexOf('{') - 1;
            int nameLength = end - start;
            return inputLine.Substring(start, nameLength).Trim();
        }

        public string GetMethodName(string inputLine)
        {
            int methodNameStart = inputLine.IndexOf("function ") + 9;
            int methodNameEnd = inputLine.IndexOf("(");
            int methodNameLength = methodNameEnd - methodNameStart;
            return inputLine.Substring(methodNameStart, methodNameLength);
        }

        public string ParseFunctionLine(string inputLine)
        {
            //string inputLine = @"function addBorder(picture: string[]): string[] {";
            //string expected = @"string[] addBorder(string[] picture){";
            string returnType = GetMethodReturnType(inputLine);
            string methodName = GetMethodName(inputLine);
            string parameters = FormatParameters(GetParameters(inputLine));
            // string parameterType = "pTypeUnknown";
            // string parameterName = "pNameUnknown";
            return returnType + " " + methodName + "(" + parameters + ")";
        }

        public string FormatParameters(string[] source)
        {
            // string[] source = { "picture: string[]" };
            // string expected = "string[] picture";
            string outString = "";
            for (int i = 0; i < source.Length; i++)
            {
                int semicolonIndex = source[i].IndexOf(':');
                string pName = source[i].Substring(0, semicolonIndex).Trim();
                string pType = source[i].Substring(semicolonIndex + 1).Trim();
                if (pType == "number")
                {
                    pType = "int";
                }
                outString += pType + " " + pName;
                if (i < source.Length - 1)
                {
                    outString += ", ";
                }
            }
            return outString;
        }

        public string GetAlogithmNameFromPath(string filePath)
        {
            string algorithmName = Path.GetFileName(filePath);
            algorithmName = algorithmName.Substring(0, algorithmName.IndexOf('.'));
            return algorithmName;
        }

    }



    public class FileHandler_Tests
    {
        FileHandler o;
        [SetUp]
        public void Setup()
        {
            o = new FileHandler();
        }

        [Test]
        public void GetParameters_ForOneType()
        {
            string inputLine = @"function addBorder(picture: string[]): string[] {";
            //string expected = @"string[] addBorder(string[] picture){";
            string[] expected = { "picture: string[]" };
            string[] actual = o.GetParameters(inputLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BuildParameterString_ForOneType()
        {
            string[] source = { "picture: string[]" };
            string expected = "string[] picture";
            string actual = o.FormatParameters(source);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BuildParameterString_ForMultipleTypes()
        {
            string[] source = { "picture: string[]", "length: number", "name: string" };
            string expected = "string[] picture, int length, string name";
            string actual = o.FormatParameters(source);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void GetParameters_ForMulitpleTypes()
        {
            string inputLine = @"function addBorder(picture: string[], length: number, name: string): string[] {";
            //string expected = @"string[] addBorder(string[] picture){";
            string[] expected = { "picture: string[]", "length: number", "name: string" };
            string[] actual = o.GetParameters(inputLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMethodReturnType_Works()
        {
            string inputLine = @"function addBorder(picture: string[]): string[] {";
            //string expected = @"string[] addBorder(string[] picture){";
            string expected = "string[]";
            string actual = o.GetMethodReturnType(inputLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMethodName_Works()
        {
            string inputLine = @"function addBorder(picture: string[]): string[] {";
            //string expected = @"string[] addBorder(string[] picture){";
            string expected = "addBorder";
            string actual = o.GetMethodName(inputLine);
            Assert.AreEqual(expected, actual);
        }

        //[Ignore("next test up")]
        [Test]
        public void ParseFunctionLine()
        {
            string inputLine = @"function addBorder(picture: string[]): string[] {";
            string expected = @"string[] addBorder(string[] picture)";
            string actual = o.ParseFunctionLine(inputLine);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAlogithmNameFromPath_WorksAsExpected()
        {
            string inFileNameAndPath = @"c:\ts2cs_files-Old\addBorder.ts";
            string algorithmName = o.GetAlogithmNameFromPath(inFileNameAndPath);
            Assert.AreEqual("addBorder", algorithmName);
        }


        //[Ignore("integration test")]
        [Test]
        public void TestProcessor()
        {
            string[] templateContent = o.getFileContents("outputTemplate.txt");
            string inFileNameAndPath = @"c:\ts2cs_files-Old\addBorder.ts";
            string outFileNameAndPath = @"c:\ts2cs_files-Old\addBorder.cs";
            string[] contents = o.getFileContents(inFileNameAndPath);
            //string[] outContents = o.parseContents(contents);
            string[] outContents = o.parseContents(templateContent);
            o.WriteContentsToFile(outContents, outFileNameAndPath);
        }

        [Test]
        public void PrepareContentFromSourceTSFile_WorksAsExpected(){
            Assert.Fail();
        }

        [Test]
        public void InsertSourceContentIntoTemplateStringArray_WorksAsExpected(){
            Assert.Fail();
        }

        [Ignore("integration test")]
        [Test]
        public void CanConvertFunctionHeader()
        {
            string filePath = "c:\\ts2cs_CanRestoreTestData_TestDir";

            Assert.AreEqual(0, o.GetDirectoryCount(filePath));
            //o.RestoreTestData(filePath, "ts2cs_files-Master");
            // clear test data
            // o.RestoreTestData
            // Assert_TestDataSignature
        }

        [Test]
        public void WriteALineOfTextToAFile_Works()
        {
            o.WriteALineOfTextToAFile();
        }
    }
}