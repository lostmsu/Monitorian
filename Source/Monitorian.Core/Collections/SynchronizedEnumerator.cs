using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Monitorian.Core.Collections;

class SynchronizedEnumerator<T>(object @lock, IEnumerator<T> source) : IEnumerator<T>
{
	readonly object _lock = @lock ?? throw new ArgumentNullException(nameof(@lock));
	readonly IEnumerator<T> _source = source ?? throw new ArgumentNullException(nameof(source));

	public T Current => _source.Current;
	object IEnumerator.Current => _source.Current;

	public bool MoveNext() => _source.MoveNext();

	public void Reset() => _source.Reset();

	bool disposed;
	public void Dispose()
	{
		if (disposed)
			return;

		disposed = true;
		Monitor.Exit(_lock);
	}
}
