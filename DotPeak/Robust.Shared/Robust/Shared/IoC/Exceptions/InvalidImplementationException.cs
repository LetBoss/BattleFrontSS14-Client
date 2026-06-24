// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.Exceptions.InvalidImplementationException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.IoC.Exceptions;

[Virtual]
public class InvalidImplementationException : Exception
{
  private readonly string message;
  private readonly Type type;
  private readonly Type parent;

  public InvalidImplementationException(Type type, Type parent, string message)
  {
    this.type = type;
    this.parent = parent;
    this.message = message;
  }

  public override string Message
  {
    get => $"{this.type} incorrectly implements {this.parent}: {this.message}";
  }
}
