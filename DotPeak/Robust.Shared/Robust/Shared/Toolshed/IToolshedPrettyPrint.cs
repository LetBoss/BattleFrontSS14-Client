// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.IToolshedPrettyPrint
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections;

#nullable enable
namespace Robust.Shared.Toolshed;

public interface IToolshedPrettyPrint
{
  string PrettyPrint(
    ToolshedManager toolshed,
    out IEnumerable? more,
    bool moreUsed = false,
    int? maxOutput = null);
}
