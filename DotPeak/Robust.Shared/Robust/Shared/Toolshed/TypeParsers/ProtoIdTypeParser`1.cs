// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.ProtoIdTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class ProtoIdTypeParser<T> : TypeParser<ProtoId<T>> where T : class, IPrototype
{
  [Dependency]
  private readonly IConfigurationManager _config;
  [Dependency]
  private readonly IPrototypeManager _proto;

  public override bool TryParse(ParserContext ctx, out ProtoId<T> result)
  {
    result = new ProtoId<T>();
    Rune? nullable = ctx.PeekRune();
    Rune rune = new Rune('"');
    string parsed;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == rune ? 1 : 0) : 0) != 0)
    {
      if (!this.Toolshed.TryParse<string>(ctx, out parsed))
        return false;
    }
    else
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      parsed = ctx.GetWord(ProtoIdTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken ?? (ProtoIdTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    }
    if (parsed == null || !this._proto.HasIndex<T>(parsed))
    {
      string kind;
      this._proto.TryGetKindFrom<T>(out kind);
      ctx.Error = (IConError) new NotAValidPrototype(parsed ?? "[null]", kind);
      result = new ProtoId<T>();
      return false;
    }
    result = new ProtoId<T>(parsed);
    return true;
  }

  public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    if (typeof (T) == typeof (EntityPrototype))
      return CompletionResult.FromHint(this.GetArgHint(arg));
    string argHint = ToolshedCommand.GetArgHint(arg, typeof (ProtoId<T>));
    int cvar = this._config.GetCVar<int>(CVars.ToolshedPrototypesAutocompleteLimit);
    string input = ctx.Input;
    int index = ctx.Index;
    return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIdsLimited<T>(input.Substring(index, input.Length - index), this._proto, maxCount: cvar), argHint);
  }
}
