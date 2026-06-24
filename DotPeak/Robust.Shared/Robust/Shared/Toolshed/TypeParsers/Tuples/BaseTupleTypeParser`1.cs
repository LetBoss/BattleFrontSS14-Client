// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Tuples.BaseTupleTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers.Tuples;

public abstract class BaseTupleTypeParser<TParses> : TypeParser<TParses> where TParses : ITuple
{
  public abstract IEnumerable<Type> Fields { get; }

  public abstract TParses Create(IReadOnlyList<object> values);

  public override bool TryParse(ParserContext parserContext, [NotNullWhen(true)] out TParses? result)
  {
    List<object> values = new List<object>();
    foreach (Type field in this.Fields)
    {
      object parsed;
      if (!this.Toolshed.TryParse(parserContext, field, out parsed))
      {
        result = default (TParses);
        return false;
      }
      values.Add(parsed);
    }
    result = this.Create((IReadOnlyList<object>) values);
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    foreach (Type field in this.Fields)
    {
      ParserRestorePoint point = parserContext.Save();
      if (!this.Toolshed.TryParse(parserContext, field, out object _) || !Rune.IsWhiteSpace(parserContext.PeekRune() ?? new Rune('.')))
      {
        parserContext.Restore(point);
        return this.Toolshed.TryAutocomplete(parserContext, field, new CommandArgument?());
      }
    }
    return (CompletionResult) null;
  }
}
