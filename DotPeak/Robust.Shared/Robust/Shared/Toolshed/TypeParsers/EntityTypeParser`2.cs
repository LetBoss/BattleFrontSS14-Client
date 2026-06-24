// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.EntityTypeParser`2
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

internal sealed class EntityTypeParser<T1, T2> : TypeParser<Entity<T1, T2>>
  where T1 : IComponent
  where T2 : IComponent
{
  [Dependency]
  private readonly IEntityManager _entMan;

  public override bool TryParse(ParserContext parser, out Entity<T1, T2> result)
  {
    result = new Entity<T1, T2>();
    EntityUid result1;
    T1 component1;
    T2 component2;
    if (!EntityTypeParser.TryParseEntity(this._entMan, parser, out result1) || !this._entMan.TryGetComponent<T1>(result1, out component1) || !this._entMan.TryGetComponent<T2>(result1, out component2))
      return false;
    result = new Entity<T1, T2>(result1, component1, component2);
    return true;
  }

  public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
  {
    if (!ctx.CheckInvokable<EntitiesCommand>())
      return (CompletionResult) null;
    string argHint = ToolshedCommand.GetArgHint(arg, typeof (NetEntity));
    if (this._entMan.Count<T1>() > 128 /*0x80*/)
      return CompletionResult.FromHint(argHint);
    AllEntityQueryEnumerator<T1, T2, MetaDataComponent> entityQueryEnumerator = this._entMan.AllEntityQueryEnumerator<T1, T2, MetaDataComponent>();
    List<CompletionOption> options = new List<CompletionOption>();
    MetaDataComponent comp3;
    while (entityQueryEnumerator.MoveNext(out T1 _, out T2 _, out comp3))
      options.Add(new CompletionOption(comp3.NetEntity.ToString(), comp3.EntityName));
    return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) options, argHint);
  }
}
