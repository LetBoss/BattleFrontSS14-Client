// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Reflection.IReflectionManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Reflection;

[NotContentImplementable]
public interface IReflectionManager
{
  IEnumerable<Type> GetAllChildren<T>(bool inclusive = false);

  IEnumerable<Type> GetAllChildren(Type baseType, bool inclusive = false);

  IReadOnlyList<Assembly> Assemblies { get; }

  Type? GetType(string name);

  Type LooseGetType(string name);

  bool TryLooseGetType(string name, [NotNullWhen(true)] out Type? type);

  IEnumerable<Type> FindTypesWithAttribute<T>() where T : Attribute;

  IEnumerable<Type> FindTypesWithAttribute(Type attributeType);

  void LoadAssemblies(IEnumerable<Assembly> assemblies);

  void LoadAssemblies(params Assembly[] args);

  event EventHandler<ReflectionUpdateEventArgs>? OnAssemblyAdded;

  bool TryParseEnumReference(string reference, [NotNullWhen(true)] out Enum? @enum, bool shouldThrow = true);

  string GetEnumReference(Enum @enum);

  Type? YamlTypeTagLookup(Type baseType, string typeName);

  IEnumerable<Type> FindAllTypes();

  void Initialize();
}
