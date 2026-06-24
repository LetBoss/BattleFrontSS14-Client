// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.AbstractFieldInfo
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Utility;

internal abstract class AbstractFieldInfo
{
  public abstract string Name { get; }

  internal abstract MemberInfo MemberInfo { get; }

  public abstract Type FieldType { get; }

  public abstract Type? DeclaringType { get; }

  public abstract Module Module { get; }

  public abstract object? GetValue(object? obj);

  public abstract void SetValue(object? obj, object? value);

  public abstract T? GetAttribute<T>(bool includeBacking = false) where T : Attribute;

  public abstract IEnumerable<T> GetAttributes<T>(bool includeBacking = false) where T : Attribute;

  public abstract bool HasAttribute<T>(bool includeBacking = false) where T : Attribute;

  public abstract bool TryGetAttribute<T>([NotNullWhen(true)] out T? attribute, bool includeBacking = false) where T : Attribute;

  public abstract bool TryGetAttribute(Type type, [NotNullWhen(true)] out Attribute? attribute, bool includeBacking = false);

  public abstract bool IsBackingField();

  public abstract bool HasBackingField();

  public abstract SpecificFieldInfo? GetBackingField();

  public abstract bool TryGetBackingField([NotNullWhen(true)] out SpecificFieldInfo? field);
}
