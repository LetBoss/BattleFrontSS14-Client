using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.IoC;

[NotContentImplementable]
public interface IDynamicTypeFactory
{
	object CreateInstance(Type type, bool oneOff = false, bool inject = true);

	object CreateInstance(Type type, object[] args, bool oneOff = false, bool inject = true);

	T CreateInstance<T>(bool oneOff = false, bool inject = true) where T : new();
}
