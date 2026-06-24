using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Utility;

namespace Content.Shared.DeviceNetwork;

public sealed class NetworkPayload : Dictionary<string, object?>
{
	public bool TryGetValue<T>(string key, [NotNullWhen(true)] out T? value)
	{
		T result = default(T);
		if (Extensions.TryCastValue<T, string, object>((Dictionary<string, object>)this, key, ref result))
		{
			value = result;
			return true;
		}
		value = default(T);
		return false;
	}
}
