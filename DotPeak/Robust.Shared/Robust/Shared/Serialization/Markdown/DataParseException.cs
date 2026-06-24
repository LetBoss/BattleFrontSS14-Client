// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.DataParseException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public sealed class DataParseException : Exception
{
  public DataParseException()
  {
  }

  public DataParseException(string message)
    : base(message)
  {
  }

  public DataParseException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
