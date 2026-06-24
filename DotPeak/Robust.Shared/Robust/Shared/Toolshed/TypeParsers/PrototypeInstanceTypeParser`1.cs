// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.PrototypeInstanceTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class PrototypeInstanceTypeParser<T> : TypeParser<T> where T : class, IPrototype
{
  [Dependency]
  private readonly IPrototypeManager _proto;

  public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
  {
    string parsed;
    if (!this.Toolshed.TryParse<string>(ctx, out parsed))
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      parsed = ctx.GetWord(PrototypeInstanceTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken ?? (PrototypeInstanceTypeParser<T>.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    }
    if (parsed != null && this._proto.TryIndex<T>(parsed, out result))
      return true;
    string kind;
    this._proto.TryGetKindFrom<T>(out kind);
    ctx.Error = (IConError) new NotAValidPrototype(parsed ?? "[null]", kind);
    result = default (T);
    return false;
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    return this.Toolshed.TryAutocomplete(ctx, typeof (ProtoId<T>), arg);
  }
}
