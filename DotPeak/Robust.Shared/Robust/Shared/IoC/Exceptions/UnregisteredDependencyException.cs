// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.Exceptions.UnregisteredDependencyException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.IoC.Exceptions;

[Virtual]
[Serializable]
public class UnregisteredDependencyException : Exception
{
  public readonly string? OwnerType;
  public readonly string? TargetType;
  public readonly string? FieldName;

  public UnregisteredDependencyException(Type owner, Type target, string fieldName)
    : base($"{owner} requested unregistered type with its field {target}: {fieldName}")
  {
    this.OwnerType = owner.AssemblyQualifiedName;
    this.TargetType = target.AssemblyQualifiedName;
    this.FieldName = fieldName;
  }
}
