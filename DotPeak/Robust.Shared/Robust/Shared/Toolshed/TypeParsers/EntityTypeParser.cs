// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.EntityTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class EntityTypeParser : TypeParser<EntityUid>
{
  [Dependency]
  private readonly IEntityManager _entMan;

  public static bool TryParseEntity(IEntityManager entMan, ParserContext ctx, out EntityUid result)
  {
    int index = ctx.Index;
    if (ctx.EatMatch('e'))
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string word = ctx.GetWord(EntityTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken ?? (EntityTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
      if (EntityUid.TryParse(word.AsSpan(), out result))
        return true;
      ctx.Error = word != null ? (IConError) new InvalidEntity("e" + word) : (IConError) new OutOfInputError();
      ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
      return false;
    }
    ctx.EatMatch('n');
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    string word1 = ctx.GetWord(EntityTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken ?? (EntityTypeParser.\u003C\u003EO.\u003C0\u003E__IsToken = new Func<Rune, bool>(ParserContext.IsToken)));
    NetEntity entity;
    if (NetEntity.TryParse(word1.AsSpan(), out entity))
    {
      result = entMan.GetEntity(entity);
      return true;
    }
    result = new EntityUid();
    ctx.Error = word1 != null ? (IConError) new InvalidEntity(word1) : (IConError) new OutOfInputError();
    ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
    return false;
  }

  public override bool TryParse(ParserContext parser, out EntityUid result)
  {
    return EntityTypeParser.TryParseEntity(this._entMan, parser, out result);
  }

  public override CompletionResult TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    return CompletionResult.FromHint(ToolshedCommand.GetArgHint(arg, typeof (NetEntity)));
  }
}
