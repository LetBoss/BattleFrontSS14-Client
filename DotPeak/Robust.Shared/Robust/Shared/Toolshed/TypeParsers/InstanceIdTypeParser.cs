// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.InstanceIdTypeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class InstanceIdTypeParser : TypeParser<InstanceId>
{
  public override bool TryParse(ParserContext parserContext, out InstanceId result)
  {
    result = new InstanceId(Guid.NewGuid());
    return true;
  }

  public override CompletionResult? TryAutocomplete(
    ParserContext parserContext,
    CommandArgument? arg)
  {
    return CompletionResult.FromHint(this.GetArgHint(arg));
  }
}
