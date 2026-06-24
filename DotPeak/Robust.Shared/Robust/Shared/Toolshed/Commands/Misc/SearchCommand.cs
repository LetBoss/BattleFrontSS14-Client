// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.SearchCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class SearchCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<FormattedMessage> Search<T>([PipedArgument] IEnumerable<T> input, string term)
  {
    return input.Select<T, string>((Func<T, string>) (x => this.Toolshed.PrettyPrintType((object) x, out IEnumerable _))).ToList<string>().Where<string>((Func<string, bool>) (x => x.Contains(term, StringComparison.InvariantCultureIgnoreCase))).Select<string, FormattedMessage>((Func<string, FormattedMessage>) (x =>
    {
      int num = x.IndexOf(term, StringComparison.InvariantCultureIgnoreCase);
      return ConHelpers.HighlightSpan(x, Vector2i.op_Implicit((num, num + term.Length)), Color.Aqua);
    }));
  }
}
