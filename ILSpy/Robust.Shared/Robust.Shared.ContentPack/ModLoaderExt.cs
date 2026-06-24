using System;
using System.Reflection;

namespace Robust.Shared.ContentPack;

public static class ModLoaderExt
{
	public static bool IsContentType(this IModLoader modLoader, Type type)
	{
		return modLoader.IsContentAssembly(type.Assembly);
	}

	public static bool IsContentTypeAccessAllowed(this IModLoader modLoader, Type type)
	{
		if (!modLoader.IsContentType(type))
		{
			return type.GetCustomAttribute(typeof(ContentAccessAllowedAttribute)) != null;
		}
		return true;
	}
}
