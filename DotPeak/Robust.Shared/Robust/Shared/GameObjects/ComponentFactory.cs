// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ComponentFactory
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

#nullable enable
namespace Robust.Shared.GameObjects;

[Virtual]
internal class ComponentFactory : IComponentFactory
{
  private readonly ISawmill _sawmill;
  private FrozenDictionary<string, ComponentRegistration> _names;
  private FrozenDictionary<string, string> _lowerCaseNames;
  private List<ComponentRegistration>? _networkedComponents;
  private FrozenDictionary<Type, ComponentRegistration> _types;
  private ComponentRegistration[] _array;
  private string? _ignoreMissingComponentPostfix;
  private FrozenSet<string> _ignored;
  private FrozenDictionary<CompIdx, Type> _idxToType;
  private FrozenDictionary<Type, CompIdx> _typeToIdx;

  public ComponentFactory(
    IDynamicTypeFactoryInternal _typeFactory,
    IReflectionManager _reflectionManager,
    ISerializationManager _serManager,
    ILogManager logManager)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003C_typeFactory\u003EP = _typeFactory;
    // ISSUE: reference to a compiler-generated field
    this.\u003C_reflectionManager\u003EP = _reflectionManager;
    // ISSUE: reference to a compiler-generated field
    this.\u003C_serManager\u003EP = _serManager;
    this._sawmill = logManager.GetSawmill("ent.componentFactory");
    this._names = FrozenDictionary<string, ComponentRegistration>.Empty;
    this._lowerCaseNames = FrozenDictionary<string, string>.Empty;
    this._types = FrozenDictionary<Type, ComponentRegistration>.Empty;
    this._array = Array.Empty<ComponentRegistration>();
    this._ignored = FrozenSet<string>.Empty;
    this._idxToType = FrozenDictionary<CompIdx, Type>.Empty;
    this._typeToIdx = FrozenDictionary<Type, CompIdx>.Empty;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public event Action<ComponentRegistration[]>? ComponentsAdded;

  public event Action<string>? ComponentIgnoreAdded;

  public IEnumerable<Type> AllRegisteredTypes => (IEnumerable<Type>) this._types.Keys;

  public IReadOnlyList<ComponentRegistration>? NetworkedComponents
  {
    get => (IReadOnlyList<ComponentRegistration>) this._networkedComponents;
  }

  private IEnumerable<ComponentRegistration> AllRegistrations
  {
    get => (IEnumerable<ComponentRegistration>) this._types.Values;
  }

  private ComponentRegistration Register(
    Type type,
    CompIdx idx,
    Dictionary<string, ComponentRegistration> names,
    Dictionary<string, string> lowerCaseNames,
    Dictionary<Type, ComponentRegistration> types,
    Dictionary<CompIdx, Type> idxToType,
    HashSet<string> ignored,
    bool overwrite = false)
  {
    if (this._networkedComponents != null)
      throw new ComponentRegistrationLockException();
    if (types.ContainsKey(type))
      throw new InvalidOperationException($"Type is already registered: {type}");
    if (!type.IsSubclassOf(typeof (Component)))
      throw new InvalidOperationException($"Type is not derived from component: {type}");
    string str1 = typeof (IComponent).IsAssignableFrom(type) ? ComponentFactory.CalculateComponentName(type) : throw new InvalidOperationException($"Type {type} has RegisterComponentAttribute but does not implement IComponent.");
    string lowerInvariant = str1.ToLowerInvariant();
    if (ignored.Contains(str1))
    {
      if (!overwrite)
        throw new InvalidOperationException(str1 + " is already marked as ignored component");
      ignored.Remove(str1);
    }
    ComponentRegistration componentRegistration1;
    if (names.TryGetValue(str1, out componentRegistration1))
    {
      if (!overwrite)
        throw new InvalidOperationException($"{str1} is already registered, previous: {componentRegistration1}");
      types.Remove(componentRegistration1.Type);
      names.Remove(componentRegistration1.Name);
      lowerCaseNames.Remove(componentRegistration1.Name);
    }
    string str2;
    if (!overwrite && lowerCaseNames.TryGetValue(lowerInvariant, out str2))
      throw new InvalidOperationException($"{lowerInvariant} is already registered, previous: {str2}");
    bool unsaved = type.HasCustomAttribute<UnsavedComponentAttribute>();
    ComponentRegistration componentRegistration2 = new ComponentRegistration(str1, type, idx, unsaved);
    idxToType[idx] = type;
    names[str1] = componentRegistration2;
    lowerCaseNames[lowerInvariant] = str1;
    types[type] = componentRegistration2;
    CompIdx.AssignArray<ComponentRegistration>(ref this._array, idx, componentRegistration2);
    return componentRegistration2;
  }

  private static string CalculateComponentName(Type type)
  {
    if (Attribute.GetCustomAttribute((MemberInfo) type, typeof (ComponentProtoNameAttribute)) is ComponentProtoNameAttribute customAttribute)
      return customAttribute.PrototypeName;
    string name = type.Name;
    string str1 = name.EndsWith("Component") ? name : throw new InvalidComponentNameException($"Component {type} must end with the word Component");
    int length1 = "Component".Length;
    string componentName = str1.Substring(0, str1.Length - length1);
    if (name.StartsWith("Client", StringComparison.Ordinal))
    {
      string str2 = name;
      int length2 = "Client".Length;
      int length3 = "Component".Length;
      componentName = str2.Substring(length2, str2.Length - length3 - length2);
    }
    else if (name.StartsWith("Server", StringComparison.Ordinal))
    {
      string str3 = name;
      int length4 = "Server".Length;
      int length5 = "Component".Length;
      componentName = str3.Substring(length4, str3.Length - length5 - length4);
    }
    else if (name.StartsWith("Shared", StringComparison.Ordinal))
    {
      string str4 = name;
      int length6 = "Shared".Length;
      int length7 = "Component".Length;
      componentName = str4.Substring(length6, str4.Length - length7 - length6);
    }
    return componentName;
  }

  public void RegisterNetworkedFields<T>(params string[] fields) where T : IComponent
  {
    this.RegisterNetworkedFields(this.GetRegistration(CompIdx.Index<T>()), fields);
  }

  public void RegisterNetworkedFields(ComponentRegistration compReg, params string[] fields)
  {
    if (compReg.NetworkedFields.Length != 0 || fields.Length == 0)
      return;
    compReg.NetworkedFields = fields.Length <= 32 /*0x20*/ ? fields : throw new NotSupportedException("Components with more than 32 networked fields unsupported! Consider splitting it up or making a pr for 64-bit flags");
    Dictionary<string, int> source = new Dictionary<string, int>(fields.Length);
    int num = 0;
    foreach (string field in fields)
    {
      source[field] = num;
      ++num;
    }
    compReg.NetworkedFieldLookup = source.ToFrozenDictionary<string, int>();
  }

  public void IgnoreMissingComponents(string postfix = "")
  {
    this._ignoreMissingComponentPostfix = (this._ignoreMissingComponentPostfix == null || !(this._ignoreMissingComponentPostfix != postfix) ? postfix : throw new InvalidOperationException("Ignoring multiple prefixes is not supported")) ?? throw new ArgumentNullException(nameof (postfix));
  }

  public IComponent GetComponent(Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry entry)
  {
    IComponent component = this.GetComponent(entry.Component.GetType());
    // ISSUE: reference to a compiler-generated field
    this.\u003C_serManager\u003EP.CopyTo<IComponent>(entry.Component, ref component, notNullableOverride: true);
    return component;
  }

  public void RegisterIgnore(params string[] names)
  {
    foreach (string name in names)
    {
      if (this._names.ContainsKey(name))
        throw new InvalidOperationException($"Cannot add {name} to ignored components: It is already registered as a component");
    }
    HashSet<string> hashSet = this._ignored.ToHashSet<string>();
    foreach (string name in names)
    {
      if (!hashSet.Add(name))
        this._sawmill.Warning("Duplicate ignored component: " + name);
    }
    this._ignored = hashSet.ToFrozenSet<string>();
    foreach (string name in names)
    {
      Action<string> componentIgnoreAdded = this.ComponentIgnoreAdded;
      if (componentIgnoreAdded != null)
        componentIgnoreAdded(name);
    }
  }

  public ComponentAvailability GetComponentAvailability(string componentName, bool ignoreCase = false)
  {
    string str;
    if (ignoreCase && this._lowerCaseNames.TryGetValue(componentName, out str))
      componentName = str;
    if (this._names.ContainsKey(componentName))
      return ComponentAvailability.Available;
    return this._ignored.Contains(componentName) || this._ignoreMissingComponentPostfix != null && componentName.EndsWith(this._ignoreMissingComponentPostfix) ? ComponentAvailability.Ignore : ComponentAvailability.Unknown;
  }

  public IComponent GetComponent(Type componentType)
  {
    ComponentRegistration componentRegistration;
    if (!this._types.TryGetValue(componentType, out componentRegistration))
      throw new InvalidOperationException($"{componentType} is not a registered component.");
    // ISSUE: reference to a compiler-generated field
    return this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked<IComponent>(componentRegistration.Type);
  }

  public IComponent GetComponent(CompIdx componentType)
  {
    // ISSUE: reference to a compiler-generated field
    return this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked<IComponent>(this._array[componentType.Value].Type);
  }

  public T GetComponent<T>() where T : IComponent, new()
  {
    ComponentRegistration componentRegistration;
    if (!this._types.TryGetValue(typeof (T), out componentRegistration))
      throw new InvalidOperationException($"{typeof (T)} is not a registered component.");
    // ISSUE: reference to a compiler-generated field
    return this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked<T>(componentRegistration.Type);
  }

  public IComponent GetComponent(ComponentRegistration reg)
  {
    // ISSUE: reference to a compiler-generated field
    return (IComponent) this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked(reg.Type);
  }

  public IComponent GetComponent(string componentName, bool ignoreCase = false)
  {
    string str;
    if (ignoreCase && this._lowerCaseNames.TryGetValue(componentName, out str))
      componentName = str;
    // ISSUE: reference to a compiler-generated field
    return this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked<IComponent>(this.GetRegistration(componentName, false).Type);
  }

  public IComponent GetComponent(ushort netId)
  {
    // ISSUE: reference to a compiler-generated field
    return this.\u003C_typeFactory\u003EP.CreateInstanceUnchecked<IComponent>(this.GetRegistration(netId).Type);
  }

  public ComponentRegistration GetRegistration(string componentName, bool ignoreCase = false)
  {
    string str;
    if (ignoreCase && this._lowerCaseNames.TryGetValue(componentName, out str))
      componentName = str;
    try
    {
      return this._names[componentName];
    }
    catch (KeyNotFoundException ex)
    {
      throw new UnknownComponentException("Unknown name: " + componentName);
    }
  }

  public string GetComponentName(Type componentType) => this.GetRegistration(componentType).Name;

  public string GetComponentName<T>() where T : IComponent, new() => this.GetRegistration<T>().Name;

  public string GetComponentName(ushort netID) => this.GetRegistration(netID).Name;

  public ComponentRegistration GetRegistration(ushort netID)
  {
    if (this._networkedComponents == null)
      throw new ComponentRegistrationLockException();
    try
    {
      return this._networkedComponents[(int) netID];
    }
    catch (KeyNotFoundException ex)
    {
      throw new UnknownComponentException($"Unknown net ID: {netID}");
    }
  }

  public ComponentRegistration GetRegistration(Type reference)
  {
    try
    {
      return this._types[reference];
    }
    catch (KeyNotFoundException ex)
    {
      throw new UnknownComponentException($"Unknown type: {reference}");
    }
  }

  public ComponentRegistration GetRegistration<T>() where T : IComponent, new()
  {
    return this.GetRegistration(CompIdx.Index<T>());
  }

  public ComponentRegistration GetRegistration(IComponent component)
  {
    return this.GetRegistration(component.GetType());
  }

  public ComponentRegistration GetRegistration(CompIdx idx) => this._array[idx.Value];

  public bool IsIgnored(string componentName) => this._ignored.Contains(componentName);

  public bool TryGetRegistration(
    string componentName,
    [NotNullWhen(true)] out ComponentRegistration? registration,
    bool ignoreCase = false)
  {
    string str;
    if (ignoreCase && this._lowerCaseNames.TryGetValue(componentName, out str))
      componentName = str;
    ComponentRegistration componentRegistration;
    if (this._names.TryGetValue(componentName, out componentRegistration))
    {
      registration = componentRegistration;
      return true;
    }
    registration = (ComponentRegistration) null;
    return false;
  }

  public bool TryGetRegistration(Type reference, [NotNullWhen(true)] out ComponentRegistration? registration)
  {
    ComponentRegistration componentRegistration;
    if (this._types.TryGetValue(reference, out componentRegistration))
    {
      registration = componentRegistration;
      return true;
    }
    registration = (ComponentRegistration) null;
    return false;
  }

  public bool TryGetRegistration<T>([NotNullWhen(true)] out ComponentRegistration? registration) where T : IComponent, new()
  {
    return this.TryGetRegistration(typeof (T), out registration);
  }

  public bool TryGetRegistration(ushort netID, [NotNullWhen(true)] out ComponentRegistration? registration)
  {
    ComponentRegistration componentRegistration;
    if (this._networkedComponents != null && this._networkedComponents.TryGetValue<ComponentRegistration>((int) netID, out componentRegistration))
    {
      registration = componentRegistration;
      return true;
    }
    registration = (ComponentRegistration) null;
    return false;
  }

  public bool TryGetRegistration(IComponent component, [NotNullWhen(true)] out ComponentRegistration? registration)
  {
    return this.TryGetRegistration(component.GetType(), out registration);
  }

  public void DoAutoRegistrations()
  {
    // ISSUE: reference to a compiler-generated field
    this.RegisterTypesInternal(this.\u003C_reflectionManager\u003EP.FindTypesWithAttribute<RegisterComponentAttribute>().ToArray<Type>(), false);
  }

  public CompIdx GetIndex(Type type) => this._typeToIdx[type];

  public int GetArrayIndex(Type type) => this._typeToIdx[type].Value;

  private void RegisterTypesInternal(Type[] types, bool overwrite)
  {
    Dictionary<string, ComponentRegistration> dictionary1 = this._names.ToDictionary<string, ComponentRegistration>();
    Dictionary<string, string> dictionary2 = this._lowerCaseNames.ToDictionary<string, string>();
    Dictionary<Type, ComponentRegistration> dictionary3 = this._types.ToDictionary<Type, ComponentRegistration>();
    Dictionary<CompIdx, Type> dictionary4 = this._idxToType.ToDictionary<CompIdx, Type>();
    HashSet<string> hashSet = this._ignored.ToHashSet<string>();
    ComponentRegistration[] componentRegistrationArray = new ComponentRegistration[types.Length];
    Dictionary<Type, CompIdx> dictionary5 = this._typeToIdx.ToDictionary<Type, CompIdx>();
    for (int index1 = 0; index1 < types.Length; ++index1)
    {
      Type type = types[index1];
      CompIdx index2 = CompIdx.GetIndex(type);
      dictionary5[type] = index2;
      componentRegistrationArray[index1] = this.Register(type, index2, dictionary1, dictionary2, dictionary3, dictionary4, hashSet, overwrite);
    }
    RStopwatch rstopwatch = RStopwatch.StartNew();
    this._typeToIdx = dictionary5.ToFrozenDictionary<Type, CompIdx>();
    this._names = dictionary1.ToFrozenDictionary<string, ComponentRegistration>();
    this._lowerCaseNames = dictionary2.ToFrozenDictionary<string, string>();
    this._types = dictionary3.ToFrozenDictionary<Type, ComponentRegistration>();
    this._idxToType = dictionary4.ToFrozenDictionary<CompIdx, Type>();
    this._ignored = hashSet.ToFrozenSet<string>();
    this._sawmill.Verbose($"Freezing component factory took {rstopwatch.Elapsed.TotalMilliseconds:f2}ms");
    Action<ComponentRegistration[]> componentsAdded = this.ComponentsAdded;
    if (componentsAdded == null)
      return;
    componentsAdded(componentRegistrationArray);
  }

  public void RegisterClass<T>(bool overwrite = false) where T : IComponent, new()
  {
    this.RegisterTypesInternal(new Type[1]{ typeof (T) }, overwrite);
  }

  public void RegisterTypes(params Type[] types)
  {
    foreach (Type type in types)
    {
      if (!type.IsAssignableTo(typeof (IComponent)) || !type.HasParameterlessConstructor())
        throw new InvalidOperationException($"Invalid type: {type}");
    }
    this.RegisterTypesInternal(types, false);
  }

  public IEnumerable<CompIdx> GetAllRefTypes()
  {
    return this.AllRegistrations.Select<ComponentRegistration, CompIdx>((Func<ComponentRegistration, CompIdx>) (x => x.Idx)).Distinct<CompIdx>();
  }

  public IEnumerable<ComponentRegistration> GetAllRegistrations()
  {
    return (IEnumerable<ComponentRegistration>) this._types.Values;
  }

  public void GenerateNetIds()
  {
    List<ComponentRegistration> componentRegistrationList = new List<ComponentRegistration>(this._names.Count);
    foreach (KeyValuePair<string, ComponentRegistration> name in this._names)
    {
      ComponentRegistration componentRegistration = name.Value;
      if (Attribute.GetCustomAttribute((MemberInfo) componentRegistration.Type, typeof (NetworkedComponentAttribute)) is NetworkedComponentAttribute)
        componentRegistrationList.Add(componentRegistration);
    }
    componentRegistrationList.Sort((Comparison<ComponentRegistration>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal)));
    for (ushort index = 0; (int) index < componentRegistrationList.Count; ++index)
      componentRegistrationList[(int) index].NetID = new ushort?(index);
    this._networkedComponents = componentRegistrationList;
  }

  public Type IdxToType(CompIdx idx) => this._idxToType[idx];

  public byte[] GetHash(bool networkedOnly)
  {
    if (this._networkedComponents == null)
      throw new ComponentRegistrationLockException();
    return this.GetHash(networkedOnly ? (IEnumerable<ComponentRegistration>) this._networkedComponents : (IEnumerable<ComponentRegistration>) this._array);
  }

  public byte[] GetHash(IEnumerable<ComponentRegistration> comps)
  {
    comps = (IEnumerable<ComponentRegistration>) comps.OrderBy<ComponentRegistration, string>((Func<ComponentRegistration, string>) (x => x.Name), (IComparer<string>) StringComparer.InvariantCulture);
    MemoryStream inputStream = new MemoryStream();
    using (StreamWriter streamWriter = new StreamWriter((Stream) inputStream, leaveOpen: true))
    {
      foreach (ComponentRegistration comp in comps)
      {
        streamWriter.Write(comp.Name);
        streamWriter.Write((object) comp.NetID);
      }
    }
    inputStream.Position = 0L;
    return SHA256.Create().ComputeHash((Stream) inputStream);
  }
}
