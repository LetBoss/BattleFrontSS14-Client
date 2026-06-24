using System;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Robust.Shared.Sandboxing;

internal sealed class SandboxHelper : ISandboxHelper
{
	[Dependency]
	private readonly IModLoader _modLoader;

	public object CreateInstance(Type type)
	{
		if (!_modLoader.IsContentTypeAccessAllowed(type))
		{
			throw new SandboxArgumentException("Creating non-content types is not allowed.");
		}
		return Activator.CreateInstance(type);
	}
}
