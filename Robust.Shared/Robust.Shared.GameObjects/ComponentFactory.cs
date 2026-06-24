using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.GameObjects;

[Virtual]
internal class ComponentFactory(IDynamicTypeFactoryInternal _typeFactory, IReflectionManager _reflectionManager, ISerializationManager _serManager, ILogManager logManager) : IComponentFactory
{
	private readonly ISawmill _sawmill = logManager.GetSawmill("ent.componentFactory");

	private FrozenDictionary<string, ComponentRegistration> _names = FrozenDictionary<string, ComponentRegistration>.Empty;

	private FrozenDictionary<string, string> _lowerCaseNames = FrozenDictionary<string, string>.Empty;

	private List<ComponentRegistration>? _networkedComponents;

	private FrozenDictionary<Type, ComponentRegistration> _types = FrozenDictionary<Type, ComponentRegistration>.Empty;

	private ComponentRegistration[] _array = Array.Empty<ComponentRegistration>();

	private string? _ignoreMissingComponentPostfix;

	private FrozenSet<string> _ignored = FrozenSet<string>.Empty;

	private FrozenDictionary<CompIdx, Type> _idxToType = FrozenDictionary<CompIdx, Type>.Empty;

	private FrozenDictionary<Type, CompIdx> _typeToIdx = FrozenDictionary<Type, CompIdx>.Empty;

	public IEnumerable<Type> AllRegisteredTypes => _types.Keys;

	public IReadOnlyList<ComponentRegistration>? NetworkedComponents => _networkedComponents;

	private IEnumerable<ComponentRegistration> AllRegistrations => _types.Values;

	public event Action<ComponentRegistration[]>? ComponentsAdded;

	public event Action<string>? ComponentIgnoreAdded;

	private ComponentRegistration Register(Type type, CompIdx idx, Dictionary<string, ComponentRegistration> names, Dictionary<string, string> lowerCaseNames, Dictionary<Type, ComponentRegistration> types, Dictionary<CompIdx, Type> idxToType, HashSet<string> ignored, bool overwrite = false)
	{
		if (_networkedComponents != null)
		{
			throw new ComponentRegistrationLockException();
		}
		if (types.ContainsKey(type))
		{
			throw new InvalidOperationException($"Type is already registered: {type}");
		}
		if (!type.IsSubclassOf(typeof(Component)))
		{
			throw new InvalidOperationException($"Type is not derived from component: {type}");
		}
		if (!typeof(IComponent).IsAssignableFrom(type))
		{
			throw new InvalidOperationException($"Type {type} has RegisterComponentAttribute but does not implement IComponent.");
		}
		string text = CalculateComponentName(type);
		string text2 = text.ToLowerInvariant();
		if (ignored.Contains(text))
		{
			if (!overwrite)
			{
				throw new InvalidOperationException(text + " is already marked as ignored component");
			}
			ignored.Remove(text);
		}
		if (names.TryGetValue(text, out ComponentRegistration value))
		{
			if (!overwrite)
			{
				throw new InvalidOperationException($"{text} is already registered, previous: {value}");
			}
			types.Remove(value.Type);
			names.Remove(value.Name);
			lowerCaseNames.Remove(value.Name);
		}
		if (!overwrite && lowerCaseNames.TryGetValue(text2, out string value2))
		{
			throw new InvalidOperationException(text2 + " is already registered, previous: " + value2);
		}
		bool unsaved = type.HasCustomAttribute<UnsavedComponentAttribute>();
		ComponentRegistration componentRegistration = new ComponentRegistration(text, type, idx, unsaved);
		idxToType[idx] = type;
		names[text] = componentRegistration;
		lowerCaseNames[text2] = text;
		types[type] = componentRegistration;
		CompIdx.AssignArray(ref _array, idx, componentRegistration);
		return componentRegistration;
	}

	private static string CalculateComponentName(Type type)
	{
		if (Attribute.GetCustomAttribute(type, typeof(ComponentProtoNameAttribute)) is ComponentProtoNameAttribute componentProtoNameAttribute)
		{
			return componentProtoNameAttribute.PrototypeName;
		}
		string name = type.Name;
		if (!name.EndsWith("Component"))
		{
			throw new InvalidComponentNameException($"Component {type} must end with the word Component");
		}
		string text = name;
		int length = "Component".Length;
		string result = text.Substring(0, text.Length - length);
		if (name.StartsWith("Client", StringComparison.Ordinal))
		{
			text = name;
			length = "Client".Length;
			int length2 = "Component".Length;
			result = text.Substring(length, text.Length - length2 - length);
		}
		else if (name.StartsWith("Server", StringComparison.Ordinal))
		{
			text = name;
			int length2 = "Server".Length;
			length = "Component".Length;
			result = text.Substring(length2, text.Length - length - length2);
		}
		else if (name.StartsWith("Shared", StringComparison.Ordinal))
		{
			text = name;
			length = "Shared".Length;
			int length2 = "Component".Length;
			result = text.Substring(length, text.Length - length2 - length);
		}
		return result;
	}

	public void RegisterNetworkedFields<T>(params string[] fields) where T : IComponent
	{
		ComponentRegistration registration = GetRegistration(CompIdx.Index<T>());
		RegisterNetworkedFields(registration, fields);
	}

	public void RegisterNetworkedFields(ComponentRegistration compReg, params string[] fields)
	{
		if (compReg.NetworkedFields.Length == 0 && fields.Length != 0)
		{
			if (fields.Length > 32)
			{
				throw new NotSupportedException("Components with more than 32 networked fields unsupported! Consider splitting it up or making a pr for 64-bit flags");
			}
			compReg.NetworkedFields = fields;
			Dictionary<string, int> dictionary = new Dictionary<string, int>(fields.Length);
			int num = 0;
			foreach (string key in fields)
			{
				dictionary[key] = num;
				num++;
			}
			compReg.NetworkedFieldLookup = dictionary.ToFrozenDictionary();
		}
	}

	public void IgnoreMissingComponents(string postfix = "")
	{
		if (_ignoreMissingComponentPostfix != null && _ignoreMissingComponentPostfix != postfix)
		{
			throw new InvalidOperationException("Ignoring multiple prefixes is not supported");
		}
		_ignoreMissingComponentPostfix = postfix ?? throw new ArgumentNullException("postfix");
	}

	public IComponent GetComponent(EntityPrototype.ComponentRegistryEntry entry)
	{
		IComponent target = GetComponent(entry.Component.GetType());
		_serManager.CopyTo(entry.Component, ref target, null, skipHook: false, notNullableOverride: true);
		return target;
	}

	public void RegisterIgnore(params string[] names)
	{
		string[] array = names;
		foreach (string text in array)
		{
			if (_names.ContainsKey(text))
			{
				throw new InvalidOperationException("Cannot add " + text + " to ignored components: It is already registered as a component");
			}
		}
		HashSet<string> hashSet = _ignored.ToHashSet();
		array = names;
		foreach (string text2 in array)
		{
			if (!hashSet.Add(text2))
			{
				_sawmill.Warning("Duplicate ignored component: " + text2);
			}
		}
		_ignored = hashSet.ToFrozenSet();
		array = names;
		foreach (string obj in array)
		{
			this.ComponentIgnoreAdded?.Invoke(obj);
		}
	}

	public ComponentAvailability GetComponentAvailability(string componentName, bool ignoreCase = false)
	{
		if (ignoreCase && _lowerCaseNames.TryGetValue(componentName, out string value))
		{
			componentName = value;
		}
		if (_names.ContainsKey(componentName))
		{
			return ComponentAvailability.Available;
		}
		if (_ignored.Contains(componentName) || (_ignoreMissingComponentPostfix != null && componentName.EndsWith(_ignoreMissingComponentPostfix)))
		{
			return ComponentAvailability.Ignore;
		}
		return ComponentAvailability.Unknown;
	}

	public IComponent GetComponent(Type componentType)
	{
		if (!_types.TryGetValue(componentType, out ComponentRegistration value))
		{
			throw new InvalidOperationException($"{componentType} is not a registered component.");
		}
		return _typeFactory.CreateInstanceUnchecked<IComponent>(value.Type);
	}

	public IComponent GetComponent(CompIdx componentType)
	{
		return _typeFactory.CreateInstanceUnchecked<IComponent>(_array[componentType.Value].Type);
	}

	public T GetComponent<T>() where T : IComponent, new()
	{
		if (!_types.TryGetValue(typeof(T), out ComponentRegistration value))
		{
			throw new InvalidOperationException($"{typeof(T)} is not a registered component.");
		}
		return _typeFactory.CreateInstanceUnchecked<T>(value.Type);
	}

	public IComponent GetComponent(ComponentRegistration reg)
	{
		return (IComponent)_typeFactory.CreateInstanceUnchecked(reg.Type);
	}

	public IComponent GetComponent(string componentName, bool ignoreCase = false)
	{
		if (ignoreCase && _lowerCaseNames.TryGetValue(componentName, out string value))
		{
			componentName = value;
		}
		return _typeFactory.CreateInstanceUnchecked<IComponent>(GetRegistration(componentName).Type);
	}

	public IComponent GetComponent(ushort netId)
	{
		return _typeFactory.CreateInstanceUnchecked<IComponent>(GetRegistration(netId).Type);
	}

	public ComponentRegistration GetRegistration(string componentName, bool ignoreCase = false)
	{
		if (ignoreCase && _lowerCaseNames.TryGetValue(componentName, out string value))
		{
			componentName = value;
		}
		try
		{
			return _names[componentName];
		}
		catch (KeyNotFoundException)
		{
			throw new UnknownComponentException("Unknown name: " + componentName);
		}
	}

	public string GetComponentName(Type componentType)
	{
		return GetRegistration(componentType).Name;
	}

	public string GetComponentName<T>() where T : IComponent, new()
	{
		return GetRegistration<T>().Name;
	}

	public string GetComponentName(ushort netID)
	{
		return GetRegistration(netID).Name;
	}

	public ComponentRegistration GetRegistration(ushort netID)
	{
		if (_networkedComponents == null)
		{
			throw new ComponentRegistrationLockException();
		}
		try
		{
			return _networkedComponents[netID];
		}
		catch (KeyNotFoundException)
		{
			throw new UnknownComponentException($"Unknown net ID: {netID}");
		}
	}

	public ComponentRegistration GetRegistration(Type reference)
	{
		try
		{
			return _types[reference];
		}
		catch (KeyNotFoundException)
		{
			throw new UnknownComponentException($"Unknown type: {reference}");
		}
	}

	public ComponentRegistration GetRegistration<T>() where T : IComponent, new()
	{
		return GetRegistration(CompIdx.Index<T>());
	}

	public ComponentRegistration GetRegistration(IComponent component)
	{
		return GetRegistration(component.GetType());
	}

	public ComponentRegistration GetRegistration(CompIdx idx)
	{
		return _array[idx.Value];
	}

	public bool IsIgnored(string componentName)
	{
		return _ignored.Contains(componentName);
	}

	public bool TryGetRegistration(string componentName, [NotNullWhen(true)] out ComponentRegistration? registration, bool ignoreCase = false)
	{
		if (ignoreCase && _lowerCaseNames.TryGetValue(componentName, out string value))
		{
			componentName = value;
		}
		if (_names.TryGetValue(componentName, out ComponentRegistration value2))
		{
			registration = value2;
			return true;
		}
		registration = null;
		return false;
	}

	public bool TryGetRegistration(Type reference, [NotNullWhen(true)] out ComponentRegistration? registration)
	{
		if (_types.TryGetValue(reference, out ComponentRegistration value))
		{
			registration = value;
			return true;
		}
		registration = null;
		return false;
	}

	public bool TryGetRegistration<T>([NotNullWhen(true)] out ComponentRegistration? registration) where T : IComponent, new()
	{
		return TryGetRegistration(typeof(T), out registration);
	}

	public bool TryGetRegistration(ushort netID, [NotNullWhen(true)] out ComponentRegistration? registration)
	{
		if (_networkedComponents != null && _networkedComponents.TryGetValue(netID, out var value))
		{
			registration = value;
			return true;
		}
		registration = null;
		return false;
	}

	public bool TryGetRegistration(IComponent component, [NotNullWhen(true)] out ComponentRegistration? registration)
	{
		return TryGetRegistration(component.GetType(), out registration);
	}

	public void DoAutoRegistrations()
	{
		Type[] types = _reflectionManager.FindTypesWithAttribute<RegisterComponentAttribute>().ToArray();
		RegisterTypesInternal(types, overwrite: false);
	}

	public CompIdx GetIndex(Type type)
	{
		return _typeToIdx[type];
	}

	public int GetArrayIndex(Type type)
	{
		return _typeToIdx[type].Value;
	}

	private void RegisterTypesInternal(Type[] types, bool overwrite)
	{
		Dictionary<string, ComponentRegistration> dictionary = _names.ToDictionary();
		Dictionary<string, string> dictionary2 = _lowerCaseNames.ToDictionary();
		Dictionary<Type, ComponentRegistration> dictionary3 = _types.ToDictionary();
		Dictionary<CompIdx, Type> dictionary4 = _idxToType.ToDictionary();
		HashSet<string> hashSet = _ignored.ToHashSet();
		ComponentRegistration[] array = new ComponentRegistration[types.Length];
		Dictionary<Type, CompIdx> dictionary5 = _typeToIdx.ToDictionary();
		for (int i = 0; i < types.Length; i++)
		{
			Type type = types[i];
			CompIdx idx = (dictionary5[type] = CompIdx.GetIndex(type));
			array[i] = Register(type, idx, dictionary, dictionary2, dictionary3, dictionary4, hashSet, overwrite);
		}
		RStopwatch rStopwatch = RStopwatch.StartNew();
		_typeToIdx = dictionary5.ToFrozenDictionary();
		_names = dictionary.ToFrozenDictionary();
		_lowerCaseNames = dictionary2.ToFrozenDictionary();
		_types = dictionary3.ToFrozenDictionary();
		_idxToType = dictionary4.ToFrozenDictionary();
		_ignored = hashSet.ToFrozenSet();
		_sawmill.Verbose($"Freezing component factory took {rStopwatch.Elapsed.TotalMilliseconds:f2}ms");
		this.ComponentsAdded?.Invoke(array);
	}

	public void RegisterClass<T>(bool overwrite = false) where T : IComponent, new()
	{
		RegisterTypesInternal(new Type[1] { typeof(T) }, overwrite);
	}

	public void RegisterTypes(params Type[] types)
	{
		foreach (Type type in types)
		{
			if (!type.IsAssignableTo(typeof(IComponent)) || !type.HasParameterlessConstructor())
			{
				throw new InvalidOperationException($"Invalid type: {type}");
			}
		}
		RegisterTypesInternal(types, overwrite: false);
	}

	public IEnumerable<CompIdx> GetAllRefTypes()
	{
		return AllRegistrations.Select((ComponentRegistration x) => x.Idx).Distinct();
	}

	public IEnumerable<ComponentRegistration> GetAllRegistrations()
	{
		return _types.Values;
	}

	public void GenerateNetIds()
	{
		List<ComponentRegistration> list = new List<ComponentRegistration>(_names.Count);
		foreach (KeyValuePair<string, ComponentRegistration> name in _names)
		{
			ComponentRegistration value = name.Value;
			if (Attribute.GetCustomAttribute(value.Type, typeof(NetworkedComponentAttribute)) is NetworkedComponentAttribute)
			{
				list.Add(value);
			}
		}
		list.Sort((ComponentRegistration a, ComponentRegistration b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
		for (ushort num = 0; num < list.Count; num++)
		{
			list[num].NetID = num;
		}
		_networkedComponents = list;
	}

	public Type IdxToType(CompIdx idx)
	{
		return _idxToType[idx];
	}

	public byte[] GetHash(bool networkedOnly)
	{
		if (_networkedComponents == null)
		{
			throw new ComponentRegistrationLockException();
		}
		IEnumerable<ComponentRegistration> comps;
		if (!networkedOnly)
		{
			IEnumerable<ComponentRegistration> array = _array;
			comps = array;
		}
		else
		{
			IEnumerable<ComponentRegistration> array = _networkedComponents;
			comps = array;
		}
		return GetHash(comps);
	}

	public byte[] GetHash(IEnumerable<ComponentRegistration> comps)
	{
		comps = comps.OrderBy<ComponentRegistration, string>((ComponentRegistration x) => x.Name, StringComparer.InvariantCulture);
		MemoryStream memoryStream = new MemoryStream();
		using (StreamWriter streamWriter = new StreamWriter(memoryStream, null, -1, leaveOpen: true))
		{
			foreach (ComponentRegistration comp in comps)
			{
				streamWriter.Write(comp.Name);
				streamWriter.Write(comp.NetID);
			}
		}
		memoryStream.Position = 0L;
		return SHA256.Create().ComputeHash(memoryStream);
	}
}
