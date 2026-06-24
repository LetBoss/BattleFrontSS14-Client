// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.TypeCheckFailedException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.ContentPack;

[Virtual]
[Serializable]
public class TypeCheckFailedException : Exception
{
  public TypeCheckFailedException()
  {
  }

  public TypeCheckFailedException(string message)
    : base(message)
  {
  }

  public TypeCheckFailedException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
