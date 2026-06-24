using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;

namespace Content.Shared.EntityTable;

public sealed class EntityTableContext
{
	private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

	public EntityTableContext()
	{
	}

	public EntityTableContext(Dictionary<string, object> data)
	{
		_data = data;
	}

	public bool TryGetData<T>([ForbidLiteral] string key, [NotNullWhen(true)] out T? value)
	{
		value = default(T);
		if (!_data.TryGetValue(key, out object valueData) || !(valueData is T castValueData))
		{
			return false;
		}
		value = castValueData;
		return true;
	}
}
