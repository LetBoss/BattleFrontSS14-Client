// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Ordering.SortByCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Ordering;

[ToolshedCommand]
public sealed class SortByCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (MapBlockOutputParser)
  };

  public override Type[] TypeParameterParsers => SortByCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> SortBy<TOrd, T>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<T> input,
    Block<T, TOrd> orderer)
    where TOrd : IComparable<TOrd>
  {
    return (IEnumerable<T>) input.OrderBy<T, TOrd>((Func<T, TOrd>) (x => orderer.Invoke(x, ctx)));
  }
}
