using System.Collections;
using System.Collections.Generic;

namespace NeinLinq.Fakes.RewriteQuery
{
    public sealed class DummyEnumerator : IEnumerator<Dummy>
    {
        public Dummy Current { get; set; }
            = new Dummy();

        object IEnumerator.Current => Current;

        public bool DisposeCalled { get; set; }

        public void Dispose()
            => DisposeCalled = true;

        public bool MoveNextCalled { get; set; }

        public bool MoveNext()
            => !(MoveNextCalled = true);

        public bool ResetCalled { get; set; }

        public void Reset()
            => ResetCalled = true;
    }
}
