// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.Validation.FieldNotFoundErrorNode
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Markdown.Value;
using System;

#nullable enable
namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class FieldNotFoundErrorNode(ValueDataNode key, Type type) : ErrorNode((DataNode) key, $"Field \"{key.Value}\" not found in \"{type}\".", false)
{
  public Type FieldType { get; } = type;
}
