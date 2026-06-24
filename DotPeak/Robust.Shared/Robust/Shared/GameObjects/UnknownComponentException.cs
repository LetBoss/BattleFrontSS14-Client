// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UnknownComponentException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[Serializable]
public sealed class UnknownComponentException : Exception
{
  public UnknownComponentException()
  {
  }

  public UnknownComponentException(string message)
    : base(message)
  {
  }

  public UnknownComponentException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
