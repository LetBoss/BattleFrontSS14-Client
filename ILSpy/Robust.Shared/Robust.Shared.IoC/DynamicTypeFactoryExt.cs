using System;

namespace Robust.Shared.IoC;

public static class DynamicTypeFactoryExt
{
	public static T CreateInstance<T>(this IDynamicTypeFactory dynamicTypeFactory, Type type, bool oneOff = false, bool inject = true)
	{
		return (T)dynamicTypeFactory.CreateInstance(type, oneOff, inject);
	}

	public static T CreateInstance<T>(this IDynamicTypeFactory dynamicTypeFactory, Type type, object[] args, bool oneOff = false, bool inject = true)
	{
		return (T)dynamicTypeFactory.CreateInstance(type, args, oneOff, inject);
	}

	internal static T CreateInstanceUnchecked<T>(this IDynamicTypeFactoryInternal dynamicTypeFactory, Type type, bool oneOff = false, bool inject = true)
	{
		return (T)dynamicTypeFactory.CreateInstanceUnchecked(type, oneOff, inject);
	}

	internal static T CreateInstanceUnchecked<T>(this IDynamicTypeFactoryInternal dynamicTypeFactory, Type type, object[] args, bool oneOff = false, bool inject = true)
	{
		return (T)dynamicTypeFactory.CreateInstanceUnchecked(type, args, oneOff, inject);
	}
}
