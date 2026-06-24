// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Exceptions.InvalidInstanceReturnedException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Exceptions;

public sealed class InvalidInstanceReturnedException : Exception
{
  public readonly Type Expected;
  public readonly Type? Actual;

  public override string Message
  {
    get
    {
      return $"Expected InstantiationDelegate to return value of type {this.Expected}, but {this.Actual?.ToString() ?? "[NULLVALUE]"} was returned";
    }
  }

  public InvalidInstanceReturnedException(Type expected, Type? actual)
  {
    this.Expected = expected;
    this.Actual = actual;
  }
}
