// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.InvocationCtxVarParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class InvocationCtxVarParser(IInvocationContext ctx) : IVariableParser
{
  private readonly IInvocationContext _ctx = ctx;

  public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
  {
    type = this._ctx.ReadVar(name)?.GetType();
    return type != (Type) null;
  }

  public IEnumerable<(string, Type)> GetVars()
  {
    foreach (string var in this._ctx.GetVars())
    {
      Type type;
      if (this.TryParseVar(var, out type))
        yield return (var, type);
    }
  }

  public bool IsReadonlyVar(string name) => this._ctx.IsReadonlyVar(name);
}
