// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.ExplainCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;
using System;
using System.Reflection;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class ExplainCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public void Explain(IInvocationContext ctx, CommandRun expr)
  {
    StringBuilder builder = new StringBuilder();
    foreach ((ParsedCommand parsedCommand, Vector2i _) in expr.Commands)
    {
      builder.AppendLine();
      string fullName = parsedCommand.Implementor.FullName;
      StringBuilder stringBuilder1 = builder;
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder1);
      interpolatedStringHandler.AppendFormatted(fullName);
      interpolatedStringHandler.AppendLiteral(" - ");
      interpolatedStringHandler.AppendFormatted(parsedCommand.Implementor.Description());
      ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
      stringBuilder2.AppendLine(ref local1);
      Type pipedType = parsedCommand.PipedType;
      string str1 = ((object) pipedType != null ? pipedType.PrettyName() : (string) null) ?? "[none]";
      StringBuilder stringBuilder3 = builder;
      StringBuilder stringBuilder4 = stringBuilder3;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder3);
      interpolatedStringHandler.AppendLiteral("Pipe input: ");
      interpolatedStringHandler.AppendFormatted(str1);
      ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
      stringBuilder4.AppendLine(ref local2);
      StringBuilder stringBuilder5 = builder;
      StringBuilder stringBuilder6 = stringBuilder5;
      interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder5);
      interpolatedStringHandler.AppendLiteral("Pipe output: ");
      interpolatedStringHandler.AppendFormatted(parsedCommand.ReturnType.PrettyName());
      ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
      stringBuilder6.AppendLine(ref local3);
      builder.Append("Signature:\n  ");
      if (parsedCommand.PipedType != (Type) null)
      {
        ParameterInfo pipeArg = parsedCommand.Method.Base.PipeArg;
        string str2;
        if (this.Loc.TryGetString($"command-arg-sig-{parsedCommand.Implementor.LocName}-{pipeArg?.Name}", out str2))
        {
          builder.Append(str2);
          builder.Append(" → ");
        }
        else
        {
          StringBuilder stringBuilder7 = builder;
          StringBuilder stringBuilder8 = stringBuilder7;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder7);
          interpolatedStringHandler.AppendLiteral("<");
          interpolatedStringHandler.AppendFormatted(pipeArg?.Name);
          interpolatedStringHandler.AppendLiteral("> → ");
          ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler;
          stringBuilder8.Append(ref local4);
        }
      }
      if (parsedCommand.Bundle.Inverted)
        builder.Append("not ");
      parsedCommand.Implementor.AddMethodSignature(builder, parsedCommand.Method.Args, parsedCommand.Bundle.TypeArguments);
      builder.AppendLine();
    }
    ctx.WriteLine(builder.ToString().TrimEnd());
  }
}
