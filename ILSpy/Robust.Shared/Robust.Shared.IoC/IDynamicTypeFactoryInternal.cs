using System;

namespace Robust.Shared.IoC;

internal interface IDynamicTypeFactoryInternal : IDynamicTypeFactory
{
	object CreateInstanceUnchecked(Type type, bool oneOff = false, bool inject = true);

	object CreateInstanceUnchecked(Type type, object[] args, bool oneOff = false, bool inject = true);

	T CreateInstanceUnchecked<T>(bool oneOff = false, bool inject = true) where T : new();
}
