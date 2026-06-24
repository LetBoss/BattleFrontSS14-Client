// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.LocalVarParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class LocalVarParser(IVariableParser inner) : IVariableParser
{
  public readonly IVariableParser Inner = inner;
  public Dictionary<string, Type>? Variables;
  public HashSet<string>? ReadonlyVariables;

  public void SetLocalType(string name, Type? type, bool @readonly)
  {
    if (type == (Type) null)
    {
      this.Variables?.Remove(name);
    }
    else
    {
      if (this.Variables == null)
        this.Variables = new Dictionary<string, Type>();
      this.Variables[name] = type;
      if (!@readonly)
        return;
      if (this.ReadonlyVariables == null)
        this.ReadonlyVariables = new HashSet<string>();
      this.ReadonlyVariables.Add(name);
    }
  }

  public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
  {
    return this.Variables != null && this.Variables.TryGetValue(name, out type) || this.Inner.TryParseVar(name, out type);
  }

  public IEnumerable<(string, Type)> GetVars()
  {
    foreach ((string key, Type type) in this.Variables)
      yield return (key, type);
    foreach ((string key, Type type) in this.Inner.GetVars())
    {
      if (!this.Variables.ContainsKey(key))
        yield return (key, type);
    }
  }

  public bool IsReadonlyVar(string name)
  {
    return this.ReadonlyVariables != null && this.ReadonlyVariables.Contains(name);
  }
}
