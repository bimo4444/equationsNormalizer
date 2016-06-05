using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class IOService
    {
        public void Proceed(params string[] s)
        {
            Logic logic = new Logic();
            List<Task> tasks = new List<Task>();
            foreach (var path in s)                                         //new task for each file
            {
                Task t = Task.Run(() =>
                {
                    List<string> input = Read(path);                        //multiline file support
                    List<string> output = new List<string>();
                    foreach (var i in input)
                    {
                        string result = logic.Go(i);
                        output.Add(result);
                    }
                    string newPath = InvertPath(path);
                    Write(output, newPath);
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        private List<string> Read(string s)
        {
            List<string> result = new List<string>();
            using (StreamReader reader = new StreamReader(s))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
        private string InvertPath(string path)                              //change old extension with .out
        {
            return Path.ChangeExtension(path, ".out");
        }
        private void Write(List<string> list, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var s in list)
                {
                    writer.WriteLine(s);
                }
            }

        }
    }
}
