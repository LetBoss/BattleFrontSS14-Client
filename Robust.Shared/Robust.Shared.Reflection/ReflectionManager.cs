using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Reflection;

public abstract class ReflectionManager : IReflectionManager
{
	[Dependency]
	private readonly ILogManager _logMan;

	private readonly List<Assembly> assemblies = new List<Assembly>();

	private readonly Dictionary<(Type baseType, string typeName), Type?> _yamlTypeTagCache = new Dictionary<(Type, string), Type>();

	private readonly Dictionary<string, Type> _looseTypeCache = new Dictionary<string, Type>();

	private readonly Dictionary<string, Enum> _enumCache = new Dictionary<string, Enum>();

	private readonly Dictionary<Enum, string> _reverseEnumCache = new Dictionary<Enum, string>();

	private readonly ReaderWriterLockSlim _enumCacheLock = new ReaderWriterLockSlim();

	private readonly ReaderWriterLockSlim _yamlTypeTagCacheLock = new ReaderWriterLockSlim();

	private readonly List<Type> _getAllTypesCache = new List<Type>();

	private ISawmill _sawmill;

	protected abstract IEnumerable<string> TypePrefixes { get; }

	[ViewVariables]
	public IReadOnlyList<Assembly> Assemblies => assemblies;

	public event EventHandler<ReflectionUpdateEventArgs>? OnAssemblyAdded;

	public void Initialize()
	{
		_sawmill = _logMan.GetSawmill("Reflection");
	}

	public IEnumerable<Type> GetAllChildren<T>(bool inclusive = false)
	{
		return GetAllChildren(typeof(T), inclusive);
	}

	public IEnumerable<Type> GetAllChildren(Type baseType, bool inclusive = false)
	{
		EnsureGetAllTypesCache();
		foreach (Type item in _getAllTypesCache)
		{
			if (baseType.IsAssignableFrom(item) && !item.IsAbstract && (!(baseType == item) || inclusive))
			{
				yield return item;
			}
		}
	}

	private void EnsureGetAllTypesCache()
	{
		if (_getAllTypesCache.Count != 0)
		{
			return;
		}
		int num = 0;
		List<Type[]> list = new List<Type[]>();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			list.Add(types);
			num += types.Length;
		}
		_getAllTypesCache.Capacity = num;
		foreach (Type[] item in list)
		{
			foreach (Type type in item)
			{
				ReflectAttribute obj = (ReflectAttribute)Attribute.GetCustomAttribute(type, typeof(ReflectAttribute));
				if (obj == null || obj.Discoverable)
				{
					_getAllTypesCache.Add(type);
				}
			}
		}
	}

	public void LoadAssemblies(params Assembly[] args)
	{
		LoadAssemblies(args.AsEnumerable());
	}

	public void LoadAssemblies(IEnumerable<Assembly> assemblies)
	{
		Assembly[] array = assemblies.Distinct().ToArray();
		if (this.assemblies.Intersect(array).Any())
		{
			throw new InvalidOperationException("Attempted to load the same assembly multiple times!");
		}
		this.assemblies.AddRange(array);
		_getAllTypesCache.Clear();
		this.OnAssemblyAdded?.Invoke(this, new ReflectionUpdateEventArgs(this));
	}

	public Type? GetType(string name)
	{
		foreach (string typePrefix in TypePrefixes)
		{
			string name2 = typePrefix + name;
			foreach (Assembly assembly in Assemblies)
			{
				Type type = assembly.GetType(name2);
				if (type != null)
				{
					return type;
				}
			}
		}
		return null;
	}

	public Type LooseGetType(string name)
	{
		if (TryLooseGetType(name, out Type type))
		{
			return type;
		}
		throw new ArgumentException("Unable to find type: " + name + ".");
	}

	public bool TryLooseGetType(string name, [NotNullWhen(true)] out Type? type)
	{
		lock (_looseTypeCache)
		{
			if (_looseTypeCache.TryGetValue(name, out type))
			{
				return true;
			}
			switch (name)
			{
			case "Byte":
				type = typeof(byte);
				_looseTypeCache[name] = type;
				return true;
			case "Bool":
				type = typeof(bool);
				_looseTypeCache[name] = type;
				return true;
			case "Double":
				type = typeof(double);
				_looseTypeCache[name] = type;
				return true;
			case "SByte":
				type = typeof(sbyte);
				_looseTypeCache[name] = type;
				return true;
			case "Single":
				type = typeof(float);
				_looseTypeCache[name] = type;
				return true;
			case "String":
				type = typeof(string);
				_looseTypeCache[name] = type;
				return true;
			default:
				foreach (Assembly assembly in assemblies)
				{
					foreach (TypeInfo definedType in assembly.DefinedTypes)
					{
						if (definedType.FullName.EndsWith(name))
						{
							type = definedType;
							_looseTypeCache[name] = type;
							return true;
						}
					}
				}
				type = null;
				return false;
			}
		}
	}

	public IEnumerable<Type> FindTypesWithAttribute<T>() where T : Attribute
	{
		return FindTypesWithAttribute(typeof(T));
	}

	public IEnumerable<Type> FindTypesWithAttribute(Type attributeType)
	{
		EnsureGetAllTypesCache();
		return _getAllTypesCache.Where((Type type) => Attribute.IsDefined(type, attributeType));
	}

	public IEnumerable<Type> FindAllTypes()
	{
		EnsureGetAllTypesCache();
		return _getAllTypesCache;
	}

	public string GetEnumReference(Enum @enum)
	{
		using (_enumCacheLock.ReadGuard())
		{
			if (_reverseEnumCache.TryGetValue(@enum, out string value))
			{
				return value;
			}
		}
		using (_enumCacheLock.WriteGuard())
		{
			if (_reverseEnumCache.TryGetValue(@enum, out string value2))
			{
				return value2;
			}
			string fullName = @enum.GetType().FullName;
			int num = fullName.LastIndexOf('.');
			if (num > 0 && num != fullName.Length)
			{
				string value3 = fullName.Substring(num + 1);
				value2 = $"enum.{value3}.{@enum}";
				if (_enumCache.TryAdd(value2, @enum))
				{
					_reverseEnumCache.Add(@enum, value2);
					return value2;
				}
			}
			value2 = $"enum.{fullName}.{@enum}";
			_reverseEnumCache.Add(@enum, value2);
			_enumCache.Add(value2, @enum);
			return value2;
		}
	}

	public bool TryParseEnumReference(string reference, [NotNullWhen(true)] out Enum? @enum, bool shouldThrow = true)
	{
		if (!reference.StartsWith("enum."))
		{
			@enum = null;
			return false;
		}
		using (_enumCacheLock.ReadGuard())
		{
			if (_enumCache.TryGetValue(reference, out @enum))
			{
				return true;
			}
		}
		using (_enumCacheLock.WriteGuard())
		{
			if (_enumCache.TryGetValue(reference, out @enum))
			{
				return true;
			}
			string text = reference.Substring(5);
			int num = text.LastIndexOf('.');
			string text2 = text.Substring(0, num);
			string value = text.Substring(num + 1);
			foreach (Assembly assembly in assemblies)
			{
				foreach (TypeInfo definedType in assembly.DefinedTypes)
				{
					if (definedType.IsEnum && (definedType.FullName.Equals(text2) || definedType.FullName.EndsWith("." + text2) || definedType.FullName.EndsWith("+" + text2)))
					{
						@enum = (Enum)Enum.Parse(definedType, value);
						if (!_reverseEnumCache.TryAdd(@enum, reference))
						{
							_sawmill.Warning($"Conflicting enum references encountered. Enum: {@enum}. Existing: {_reverseEnumCache[@enum]}. New: {reference}");
						}
						_enumCache.Add(reference, @enum);
						return true;
					}
				}
			}
			if (shouldThrow)
			{
				throw new ArgumentException("Could not resolve enum reference: " + reference + ".");
			}
			return false;
		}
	}

	public Type? YamlTypeTagLookup(Type baseType, string typeName)
	{
		using (_yamlTypeTagCacheLock.ReadGuard())
		{
			if (_yamlTypeTagCache.TryGetValue((baseType, typeName), out Type value))
			{
				return value;
			}
		}
		using (_yamlTypeTagCacheLock.WriteGuard())
		{
			if (_yamlTypeTagCache.TryGetValue((baseType, typeName), out Type value2))
			{
				return value2;
			}
			Type type = null;
			foreach (Type allChild in GetAllChildren(baseType))
			{
				if (allChild.IsPublic)
				{
					if (allChild.Name == typeName)
					{
						type = allChild;
						break;
					}
					SerializedTypeAttribute customAttribute = allChild.GetCustomAttribute<SerializedTypeAttribute>();
					if (customAttribute != null && customAttribute.SerializeName == typeName)
					{
						type = allChild;
						break;
					}
				}
			}
			if (type == null)
			{
				TryLooseGetType(typeName, out type);
				if (type == null || type.IsAbstract || !type.IsAssignableTo(baseType))
				{
					type = null;
				}
			}
			_yamlTypeTagCache.Add((baseType, typeName), type);
			return type;
		}
	}
}
