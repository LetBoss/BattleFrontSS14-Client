using System.Collections.Generic;

namespace Robust.Shared.Log;

public sealed class ProxyLogManager : ILogManager
{
	private readonly ILogManager _impl;

	ISawmill ILogManager.RootSawmill => _impl.RootSawmill;

	public IEnumerable<ISawmill> AllSawmills => _impl.AllSawmills;

	public ProxyLogManager(ILogManager impl)
	{
		_impl = impl;
	}

	ISawmill ILogManager.GetSawmill(string name)
	{
		return _impl.GetSawmill(name);
	}
}
