using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Utility;

internal interface IUniqueIndex<TKey, TValue> : IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>, IEnumerable where TKey : notnull
{
	int KeyCount { get; }

	bool Add(TKey key, TValue value);

	int AddRange(TKey key, IEnumerable<TValue> values);

	bool Remove(TKey key);

	bool Remove(TKey key, TValue value);

	int RemoveRange(TKey key, IEnumerable<TValue> values);

	bool Replace(TKey key, TValue oldValue, TValue newValue);

	void Touch(TKey key);

	void Initialize(IEnumerable<TKey> keys);

	void Initialize(IEnumerable<KeyValuePair<TKey, HashSet<TValue>>> index);
}
