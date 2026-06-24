// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.EntProtoIdTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Syntax;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class EntProtoIdTypeParser : TypeParser<EntProtoId>
{
  public override bool TryParse(ParserContext ctx, out EntProtoId result)
  {
    result = new EntProtoId();
    ProtoId<EntityPrototype> parsed;
    if (!this.Toolshed.TryParse<ProtoId<EntityPrototype>>(ctx, out parsed))
      return false;
    result = new EntProtoId(parsed.Id);
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
