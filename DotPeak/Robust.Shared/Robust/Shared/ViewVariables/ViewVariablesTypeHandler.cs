// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesTypeHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.ViewVariables;

public abstract class ViewVariablesTypeHandler
{
  internal abstract ViewVariablesPath? HandlePath(ViewVariablesPath path, string relativePath);

  internal abstract IEnumerable<string> ListPath(ViewVariablesPath path);
}
