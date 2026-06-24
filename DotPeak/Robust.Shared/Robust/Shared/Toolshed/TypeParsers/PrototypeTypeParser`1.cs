// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.PrototypeTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

[Obsolete]
internal sealed class PrototypeTypeParser<T> : TypeParser<Prototype<T>> where T : class, IPrototype
{
  [Dependency]
  private readonly IPrototypeManager _prototype;

  public override bool TryParse(ParserContext ctx, out Prototype<T> result)
  {
    string parsed;
    if (!this.Toolshed.TryParse<string>(ctx, out parsed))
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      parsed = ctx.GetWord(PrototypeTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken ?? (PrototypeTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    }
    T prototype;
    if (parsed == null || !this._prototype.TryIndex<T>(parsed, out prototype))
    {
      string kind;
      this._prototype.TryGetKindFrom<T>(out kind);
      ctx.Error = (IConError) new NotAValidPrototype(parsed ?? "[null]", kind);
      result = new Prototype<T>();
      return false;
    }
    result = new Prototype<T>(prototype);
    return true;
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    IEnumerable<CompletionOption> options = !(typeof (T) != typeof (EntityPrototype)) ? (IEnumerable<CompletionOption>) Array.Empty<CompletionOption>() : CompletionHelper.PrototypeIDs<T>();
    string kind;
    this._prototype.TryGetKindFrom<T>(out kind);
    return CompletionResult.FromHintOptions(options, $"<{kind} prototype>");
  }
}
