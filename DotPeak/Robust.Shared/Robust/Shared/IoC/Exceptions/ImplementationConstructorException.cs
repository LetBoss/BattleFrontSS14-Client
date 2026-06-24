// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.Exceptions.ImplementationConstructorException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.IoC.Exceptions;

[Virtual]
[Serializable]
public class ImplementationConstructorException : Exception
{
  public readonly string? typeName;

  public ImplementationConstructorException(Type type, Exception? inner)
    : base($"{type} threw an exception inside its constructor.", inner)
  {
    this.typeName = type.AssemblyQualifiedName;
  }
}
