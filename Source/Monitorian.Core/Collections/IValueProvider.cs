namespace Monitorian.Core.Collections;

public interface IValueProvider<T>
{
	T Value { get; }
}
