namespace Robust.Shared.Localization;

public abstract record LocValue<T> : ILocValue
{
	public T Value { get; init; }

	object? ILocValue.Value => Value;

	protected LocValue(T val)
	{
		Value = val;
	}

	public abstract string Format(LocContext ctx);
}
