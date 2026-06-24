// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.EntityTypeParser`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Toolshed.Commands.Entities;
using Robust.Shared.Toolshed.Syntax;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class EntityTypeParser<T> : TypeParser<Entity<T>> where T : IComponent
{
  [Dependency]
  private readonly IEntityManager _entMan;

  public override bool TryParse(ParserContext parser, out Entity<T> result)
  {
    result = new Entity<T>();
    EntityUid result1;
    T component;
    if (!EntityTypeParser.TryParseEntity(this._entMan, parser, out result1) || !this._entMan.TryGetComponent<T>(result1, out component))
      return false;
    result = new Entity<T>(result1, component);
    return true;
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    if (!ctx.CheckInvokable<EntitiesCommand>())
      return (CompletionResult) null;
    string argHint = ToolshedCommand.GetArgHint(arg, typeof (NetEntity));
    if (this._entMan.Count<T>() > 128 /*0x80*/)
      return CompletionResult.FromHint(argHint);
    AllEntityQueryEnumerator<T, MetaDataComponent> entityQueryEnumerator = this._entMan.AllEntityQueryEnumerator<T, MetaDataComponent>();
    List<CompletionOption> options = new List<CompletionOption>();
    MetaDataComponent comp2;
    while (entityQueryEnumerator.MoveNext(out T _, out comp2))
      options.Add(new CompletionOption(comp2.NetEntity.ToString(), comp2.EntityName));
    return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) options, argHint);
  }
}
