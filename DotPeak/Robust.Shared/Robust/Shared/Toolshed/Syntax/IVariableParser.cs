// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.IVariableParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public interface IVariableParser
{
  static readonly IVariableParser Empty = (IVariableParser) new IVariableParser.EmptyVarParser();

  bool TryParseVar(string name, [NotNullWhen(true)] out Type? type);

  CompletionResult GenerateCompletions(bool includeReadonly = true)
  {
    return CompletionResult.FromHintOptions(this.GetVars().Where<(string, Type)>((Func<(string, Type), bool>) (x => includeReadonly || !this.IsReadonlyVar(x.Item1))).Select<(string, Type), CompletionOption>((Func<(string, Type), CompletionOption>) (x => new CompletionOption("$" + x.Item1, x.Item2.PrettyName() ?? ""))), "<variable name>");
  }

  CompletionResult GenerateCompletions<T>(bool includeReadonly = true)
  {
    return CompletionResult.FromHintOptions(this.GetVars().Where<(string, Type)>((Func<(string, Type), bool>) (x => x.Item2 == typeof (T))).Where<(string, Type)>((Func<(string, Type), bool>) (x => includeReadonly || !this.IsReadonlyVar(x.Item1))).Select<(string, Type), CompletionOption>((Func<(string, Type), CompletionOption>) (x => new CompletionOption("$" + x.Item1))), $"<Variable of type {typeof (T).PrettyName()}>");
  }

  bool IsReadonlyVar(string name) => false;

  IEnumerable<(string, Type)> GetVars();

  private sealed class EmptyVarParser : IVariableParser
  {
    public bool TryParseVar(string name, [NotNullWhen(true)] out Type? type)
    {
      type = (Type) null;
      return false;
    }

    public IEnumerable<(string, Type)> GetVars()
    {
      yield break;
    }
  }
}
