// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandArgument
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Toolshed;

public readonly record struct CommandArgument(
  string Name,
  Type Type,
  ITypeParser? Parser,
  bool IsOptional,
  object? DefaultValue,
  bool IsParamsCollection)
;
