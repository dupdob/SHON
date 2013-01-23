using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Shon.TestService
{
    public sealed class TestTracer: MarshalByRefObject, IDisposable
    {
        static private List<string> storage = new List<string>();

        static public void Log(string entry)
        {
            storage.Add(entry);
        }

        static public TestTracer FromDomain(AppDomain domain)
        {
            return (TestTracer)domain.CreateInstanceFromAndUnwrap(typeof(TestTracer).Assembly.CodeBase, typeof(TestTracer).FullName);
        }

        public string Pop()
        {
            if (storage.Count == 0)
            {
                return string.Empty;
            }
            string result = storage[0];
            storage.RemoveAt(0);
            return result;
        }

        public void Reset()
        {
            storage.Clear();
        }

        public bool IsEmpty
        {
            get
            {
               return storage.Count==0;
            }
        }
        public void Dispose()
        {
        }
    }
}
