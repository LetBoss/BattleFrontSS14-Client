namespace Robust.Shared.Utility;

public static class UniqueIndexExtensions
{
	public static void Clear<TKey, TValue>(this ref UniqueIndex<TKey, TValue> index) where TKey : notnull
	{
		index = default(UniqueIndex<TKey, TValue>);
	}
}
