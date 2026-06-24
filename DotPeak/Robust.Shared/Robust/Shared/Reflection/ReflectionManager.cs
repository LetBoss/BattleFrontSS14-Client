// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Reflection.ReflectionManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable enable
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

  public event EventHandler<ReflectionUpdateEventArgs>? OnAssemblyAdded;

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<Assembly> Assemblies => (IReadOnlyList<Assembly>) this.assemblies;

  public void Initialize() => this._sawmill = this._logMan.GetSawmill("Reflection");

  public IEnumerable<Type> GetAllChildren<T>(bool inclusive = false)
  {
    return this.GetAllChildren(typeof (T), inclusive);
  }

  public IEnumerable<Type> GetAllChildren(Type baseType, bool inclusive = false)
  {
    this.EnsureGetAllTypesCache();
    foreach (Type c in this._getAllTypesCache)
    {
      if (baseType.IsAssignableFrom(c) && !c.IsAbstract && (!(baseType == c) || inclusive))
        yield return c;
    }
  }

  private void EnsureGetAllTypesCache()
  {
    if (this._getAllTypesCache.Count != 0)
      return;
    int num = 0;
    List<Type[]> typeArrayList = new List<Type[]>();
    foreach (Assembly assembly in this.assemblies)
    {
      Type[] types = assembly.GetTypes();
      typeArrayList.Add(types);
      num += types.Length;
    }
    this._getAllTypesCache.Capacity = num;
    foreach (Type[] typeArray in typeArrayList)
    {
      foreach (Type element in typeArray)
      {
        ReflectAttribute customAttribute = (ReflectAttribute) Attribute.GetCustomAttribute((MemberInfo) element, typeof (ReflectAttribute));
        if ((customAttribute != null ? (customAttribute.Discoverable ? 1 : 0) : 1) != 0)
          this._getAllTypesCache.Add(element);
      }
    }
  }

  public void LoadAssemblies(params Assembly[] args)
  {
    this.LoadAssemblies(((IEnumerable<Assembly>) args).AsEnumerable<Assembly>());
  }

  public void LoadAssemblies(IEnumerable<Assembly> assemblies)
  {
    Assembly[] array = assemblies.Distinct<Assembly>().ToArray<Assembly>();
    if (this.assemblies.Intersect<Assembly>((IEnumerable<Assembly>) array).Any<Assembly>())
      throw new InvalidOperationException("Attempted to load the same assembly multiple times!");
    this.assemblies.AddRange((IEnumerable<Assembly>) array);
    this._getAllTypesCache.Clear();
    EventHandler<ReflectionUpdateEventArgs> onAssemblyAdded = this.OnAssemblyAdded;
    if (onAssemblyAdded == null)
      return;
    onAssemblyAdded((object) this, new ReflectionUpdateEventArgs((IReflectionManager) this));
  }

  public Type? GetType(string name)
  {
    foreach (string typePrefix in this.TypePrefixes)
    {
      string name1 = typePrefix + name;
      foreach (Assembly assembly in (IEnumerable<Assembly>) this.Assemblies)
      {
        Type type = assembly.GetType(name1);
        if (type != (Type) null)
          return type;
      }
    }
    return (Type) null;
  }

  public Type LooseGetType(string name)
  {
    Type type;
    if (this.TryLooseGetType(name, out type))
      return type;
    throw new ArgumentException($"Unable to find type: {name}.");
  }

  public bool TryLooseGetType(string name, [NotNullWhen(true)] out Type? type)
  {
    lock (this._looseTypeCache)
    {
      if (this._looseTypeCache.TryGetValue(name, out type))
        return true;
      switch (name)
      {
        case "Byte":
          type = typeof (byte);
          this._looseTypeCache[name] = type;
          return true;
        case "Bool":
          type = typeof (bool);
          this._looseTypeCache[name] = type;
          return true;
        case "Double":
          type = typeof (double);
          this._looseTypeCache[name] = type;
          return true;
        case "SByte":
          type = typeof (sbyte);
          this._looseTypeCache[name] = type;
          return true;
        case "Single":
          type = typeof (float);
          this._looseTypeCache[name] = type;
          return true;
        case "String":
          type = typeof (string);
          this._looseTypeCache[name] = type;
          return true;
        default:
          foreach (Assembly assembly in this.assemblies)
          {
            foreach (TypeInfo definedType in assembly.DefinedTypes)
            {
              if (definedType.FullName.EndsWith(name))
              {
                type = (Type) definedType;
                this._looseTypeCache[name] = type;
                return true;
              }
            }
          }
          type = (Type) null;
          return false;
      }
    }
  }

  public IEnumerable<Type> FindTypesWithAttribute<T>() where T : Attribute
  {
    return this.FindTypesWithAttribute(typeof (T));
  }

  public IEnumerable<Type> FindTypesWithAttribute(Type attributeType)
  {
    this.EnsureGetAllTypesCache();
    return this._getAllTypesCache.Where<Type>((Func<Type, bool>) (type => Attribute.IsDefined((MemberInfo) type, attributeType)));
  }

  public IEnumerable<Type> FindAllTypes()
  {
    this.EnsureGetAllTypesCache();
    return (IEnumerable<Type>) this._getAllTypesCache;
  }

  public string GetEnumReference(Enum @enum)
  {
    using (this._enumCacheLock.ReadGuard())
    {
      string enumReference;
      if (this._reverseEnumCache.TryGetValue(@enum, out enumReference))
        return enumReference;
    }
    using (this._enumCacheLock.WriteGuard())
    {
      string key;
      if (this._reverseEnumCache.TryGetValue(@enum, out key))
        return key;
      string fullName = @enum.GetType().FullName;
      int num = fullName.LastIndexOf('.');
      if (num > 0 && num != fullName.Length)
      {
        key = $"enum.{fullName.Substring(num + 1)}.{@enum}";
        if (this._enumCache.TryAdd(key, @enum))
        {
          this._reverseEnumCache.Add(@enum, key);
          return key;
        }
      }
      key = $"enum.{fullName}.{@enum}";
      this._reverseEnumCache.Add(@enum, key);
      this._enumCache.Add(key, @enum);
      return key;
    }
  }

  public bool TryParseEnumReference(string reference, [NotNullWhen(true)] out Enum? @enum, bool shouldThrow = true)
  {
    if (!reference.StartsWith("enum."))
    {
      @enum = (Enum) null;
      return false;
    }
    using (this._enumCacheLock.ReadGuard())
    {
      if (this._enumCache.TryGetValue(reference, out @enum))
        return true;
    }
    using (this._enumCacheLock.WriteGuard())
    {
      if (this._enumCache.TryGetValue(reference, out @enum))
        return true;
      string str1 = reference.Substring(5);
      int length = str1.LastIndexOf('.');
      string str2 = str1.Substring(0, length);
      string str3 = str1.Substring(length + 1);
      foreach (Assembly assembly in this.assemblies)
      {
        foreach (TypeInfo definedType in assembly.DefinedTypes)
        {
          if (definedType.IsEnum && (definedType.FullName.Equals(str2) || definedType.FullName.EndsWith("." + str2) || definedType.FullName.EndsWith("+" + str2)))
          {
            @enum = (Enum) Enum.Parse((Type) definedType, str3);
            if (!this._reverseEnumCache.TryAdd(@enum, reference))
              this._sawmill.Warning($"Conflicting enum references encountered. Enum: {@enum}. Existing: {this._reverseEnumCache[@enum]}. New: {reference}");
            this._enumCache.Add(reference, @enum);
            return true;
          }
        }
      }
      if (shouldThrow)
        throw new ArgumentException($"Could not resolve enum reference: {reference}.");
      return false;
    }
  }

  public Type? YamlTypeTagLookup(Type baseType, string typeName)
  {
    using (this._yamlTypeTagCacheLock.ReadGuard())
    {
      Type type;
      if (this._yamlTypeTagCache.TryGetValue((baseType, typeName), out type))
        return type;
    }
    using (this._yamlTypeTagCacheLock.WriteGuard())
    {
      Type type1;
      if (this._yamlTypeTagCache.TryGetValue((baseType, typeName), out type1))
        return type1;
      Type type2 = (Type) null;
      foreach (Type allChild in this.GetAllChildren(baseType, false))
      {
        if (allChild.IsPublic)
        {
          if (allChild.Name == typeName)
          {
            type2 = allChild;
            break;
          }
          SerializedTypeAttribute customAttribute = allChild.GetCustomAttribute<SerializedTypeAttribute>();
          if (customAttribute != null && customAttribute.SerializeName == typeName)
          {
            type2 = allChild;
            break;
          }
        }
      }
      if (type2 == (Type) null)
      {
        this.TryLooseGetType(typeName, out type2);
        if (type2 == (Type) null || type2.IsAbstract || !type2.IsAssignableTo(baseType))
          type2 = (Type) null;
      }
      this._yamlTypeTagCache.Add((baseType, typeName), type2);
      return type2;
    }
  }
}
