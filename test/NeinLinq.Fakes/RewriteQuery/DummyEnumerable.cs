using System;
using System.Collections;
using System.Collections.Generic;

namespace NeinLinq.Fakes.RewriteQuery
{
    public class DummyEnumerable : IEnumerable<Dummy>, IDisposable
    {
        public IEnumerator<Dummy> GetEnumerator()
        {
            return new DummyEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool DisposeCalled { get; set; }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }
}
