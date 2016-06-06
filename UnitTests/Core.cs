using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class Core
    {
        Logic core = new Logic();
        [TestMethod]
        public void Mocking()
        {
            Assert.AreEqual("0 = 0", core.Go("0=0"));
            Assert.AreEqual("0 = 0", core.Go("0=   0"));
            Assert.AreEqual("0 = 0", core.Go("-1-1=-(1+1)"));
            Assert.AreEqual("0 = 0", core.Go("1+1=-(1+(-(1+1)-2)+1)"));
            Assert.AreEqual("a = 0", core.Go("0.5a+0.5a=0"));
            Assert.AreEqual("0.5a = 0", core.Go("a-0.5a=0"));
            Assert.AreEqual("0.5a - b = 0", core.Go("a-0.5a=2b-b"));

        }
        [TestMethod]
        public void IOMocking()
        {
            string mockingFileName = "mock";
            string resultFileName = "mock.out";
            string assemblyPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string inputPath = Path.Combine(assemblyPath, mockingFileName);
            string outputPath = Path.Combine(assemblyPath, resultFileName);
            using (StreamWriter writer = new StreamWriter(inputPath))               //create input file
            {
                writer.WriteLine("a+a=0");
                writer.WriteLine("a=a+b");
            }
            List<string> input = new List<string>();
            List<string> output = new List<string>();
            using (StreamReader reader = new StreamReader(inputPath))               //read file
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    input.Add(line);
                }
            }
            foreach (var i in input)
            {
                string result = core.Go(i);                                         //get result
                output.Add(result);
            }
            using (StreamWriter writer = new StreamWriter(outputPath))              
            {
                foreach (var s in output)
                {
                    writer.WriteLine(s);                                            //write result to file
                }
            }
            using (StreamReader reader = new StreamReader(outputPath))
            {
                Assert.AreEqual("2a = 0", reader.ReadLine());           
                Assert.AreEqual("- b = 0", reader.ReadLine());                      //check result
            }
            File.Delete(inputPath);
            File.Delete(outputPath);
        }
        [TestMethod]
        public void Exceptions()
        {
            var exc = AssertException.Throws<Exception>(() => core.Go("2j0"));
            Assert.AreEqual("input doesn't contain '='", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2j0==0"));
            Assert.AreEqual("'=' can't be more then once", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2j0="));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2.j0=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2.=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go(".+1=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2.?=0"));
            Assert.AreEqual("not supported char", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("   "));
            Assert.AreEqual("empty string", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("(1+1))=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("(1+1(=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go(")1+1(=0"));
            Assert.AreEqual("wrong format", exc.Message);

            exc = AssertException.Throws<Exception>(() => core.Go("2.?=0"));
            Assert.AreEqual("not supported char", exc.Message);
        }
    }
    public static class AssertException
    {
        public static T Throws<T>(Action expression, string message = "") where T : Exception
        {
            try
            {
                expression();
            }
            catch (T exception)
            {
                return exception;
            }
            Assert.Fail(message);
            return null;
        }
    }
}
