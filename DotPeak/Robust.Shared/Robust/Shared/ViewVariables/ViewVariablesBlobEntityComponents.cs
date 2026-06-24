// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesBlobEntityComponents
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
public sealed class ViewVariablesBlobEntityComponents : ViewVariablesBlob
{
  public List<ViewVariablesBlobEntityComponents.Entry> ComponentTypes { get; set; } = new List<ViewVariablesBlobEntityComponents.Entry>();

  [NetSerializable]
  [Serializable]
  public sealed class Entry : IComparable<ViewVariablesBlobEntityComponents.Entry>
  {
    public int CompareTo(ViewVariablesBlobEntityComponents.Entry other)
    {
      if (this == other)
        return 0;
      return other == null ? 1 : string.Compare(this.Stringified, other.Stringified, StringComparison.Ordinal);
    }

    public string FullName { get; set; }

    public string Stringified { get; set; }

    public string ComponentName { get; set; }
  }
}
