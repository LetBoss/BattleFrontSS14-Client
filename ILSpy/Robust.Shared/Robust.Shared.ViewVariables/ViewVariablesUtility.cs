using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Robust.Shared.ViewVariables;

public static class ViewVariablesUtility
{
	public static bool TypeHasVisibleMembers(Type type)
	{
		return type.GetAllFields().Cast<MemberInfo>().Concat(type.GetAllProperties())
			.Any((MemberInfo f) => TryGetViewVariablesAccess(f, out var _));
	}

	public static bool TryGetViewVariablesAccess(MemberInfo info, [NotNullWhen(true)] out VVAccess? access)
	{
		if (info.TryGetCustomAttribute<ViewVariablesAttribute>(out ViewVariablesAttribute attribute))
		{
			access = attribute.Access;
			return true;
		}
		if (info.HasCustomAttribute<DataFieldAttribute>() || info.HasCustomAttribute<IncludeDataFieldAttribute>())
		{
			access = VVAccess.ReadWrite;
			return true;
		}
		access = null;
		return false;
	}
}
