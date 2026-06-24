// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.NoImplementationError
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class NoImplementationError : ConError
{
  public readonly ToolshedEnvironment Env;
  public readonly string Cmd;
  public readonly string? SubCommand;
  public readonly Type[]? Types;
  public readonly Type? PipedType;

  public NoImplementationError(ParserContext ctx)
  {
    this.Env = ctx.Environment;
    this.Cmd = ctx.Bundle.Command;
    this.SubCommand = ctx.Bundle.SubCommand;
    this.Types = ctx.Bundle.TypeArguments;
    this.PipedType = ctx.Bundle.PipedType;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public override FormattedMessage DescribeInner()
  {
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(76, 2);
    interpolatedStringHandler.AppendLiteral("Could not find an implementation of the '");
    interpolatedStringHandler.AppendFormatted(this.Cmd);
    interpolatedStringHandler.AppendLiteral("' command given the input type '");
    ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
    Type pipedType = this.PipedType;
    string str = ((object) pipedType != null ? pipedType.PrettyName() : (string) null) ?? "void";
    local.AppendFormatted(str);
    interpolatedStringHandler.AppendLiteral("'.\n");
    FormattedMessage formattedMessage = FormattedMessage.FromUnformatted(interpolatedStringHandler.ToStringAndClear());
    HashSet<Type> source = this.Env.GetCommand(this.Cmd).AcceptedTypes(this.SubCommand);
    if (source.Any<Type>((Func<Type, bool>) (x => x.IsGenericParameter)) || source.Any<Type>((Func<Type, bool>) (x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IEnumerable<>) && x.GetGenericArguments()[0].IsGenericParameter)))
      return formattedMessage;
    formattedMessage.AddText($"Accepted types: '{string.Join("','", source.Select<Type, string>((Func<Type, string>) (x => x.PrettyName())))}'.\n");
    return formattedMessage;
  }
}
