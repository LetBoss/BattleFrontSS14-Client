using System;
using Robust.Shared.ContentPack;

namespace Robust.Shared.IoC;

internal sealed class DynamicTypeFactory : IDynamicTypeFactoryInternal, IDynamicTypeFactory
{
	[Dependency]
	private readonly IDependencyCollection _dependencies;

	[Dependency]
	private readonly IModLoader _modLoader;

	public object CreateInstance(Type type, bool oneOff = false, bool inject = true)
	{
		if (!_modLoader.IsContentTypeAccessAllowed(type))
		{
			throw new SandboxArgumentException("Creating non-content types is not allowed.");
		}
		return CreateInstanceUnchecked(type, oneOff, inject);
	}

	public object CreateInstance(Type type, object[] args, bool oneOff = false, bool inject = true)
	{
		if (!_modLoader.IsContentTypeAccessAllowed(type))
		{
			throw new SandboxArgumentException("Creating non-content types is not allowed.");
		}
		return CreateInstanceUnchecked(type, args, oneOff, inject);
	}

	public T CreateInstance<T>(bool oneOff = false, bool inject = true) where T : new()
	{
		if (!_modLoader.IsContentTypeAccessAllowed(typeof(T)))
		{
			throw new SandboxArgumentException("Creating non-content types is not allowed.");
		}
		return CreateInstanceUnchecked<T>(oneOff, inject);
	}

	public object CreateInstanceUnchecked(Type type, bool oneOff = false, bool inject = true)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		object obj = Activator.CreateInstance(type);
		if (inject)
		{
			_dependencies.InjectDependencies(obj, oneOff);
		}
		return obj;
	}

	public object CreateInstanceUnchecked(Type type, object[] args, bool oneOff = false, bool inject = true)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		object obj = Activator.CreateInstance(type, args);
		if (inject)
		{
			_dependencies.InjectDependencies(obj, oneOff);
		}
		return obj;
	}

	public T CreateInstanceUnchecked<T>(bool oneOff = false, bool inject = true) where T : new()
	{
		T val = new T();
		if (inject)
		{
			_dependencies.InjectDependencies(val, oneOff);
		}
		return val;
	}
}
