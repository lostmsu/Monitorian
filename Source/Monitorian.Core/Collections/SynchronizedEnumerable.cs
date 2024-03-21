using System;
using System.Collections;
using System.Collections.Generic;

namespace Monitorian.Core.Collections;

internal class SynchronizedEnumerable<T>(object @lock, IEnumerable<T> source) : IEnumerable<T>
{
	readonly object _lock = @lock ?? throw new ArgumentNullException(nameof(@lock));
	protected readonly IEnumerable<T> _source = source ?? throw new ArgumentNullException(nameof(source));

	public IEnumerator<T> GetEnumerator() => new SynchronizedEnumerator<T>(_lock, _source.GetEnumerator());
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
