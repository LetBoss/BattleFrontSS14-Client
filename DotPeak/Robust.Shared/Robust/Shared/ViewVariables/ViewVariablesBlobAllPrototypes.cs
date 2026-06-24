// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesBlobAllPrototypes
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable disable
namespace Robust.Shared.ViewVariables;

[NetSerializable]
[Serializable]
public sealed class ViewVariablesBlobAllPrototypes : ViewVariablesBlob
{
  public List<string> Prototypes { get; set; } = new List<string>();

  public string Variant { get; set; } = string.Empty;
}
