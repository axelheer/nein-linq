using System;
using System.Collections;
using System.Collections.Generic;

namespace NeinLinq.Fakes.RewriteQuery
{
    public sealed class DummyEnumerable : IEnumerable<Dummy>, IDisposable
    {
        public IEnumerator<Dummy> GetEnumerator()
            => new DummyEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool DisposeCalled { get; set; }

        public void Dispose()
            => DisposeCalled = true;
    }
}
