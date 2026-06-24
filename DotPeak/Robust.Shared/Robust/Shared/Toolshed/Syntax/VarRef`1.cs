// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Syntax.VarRef`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Robust.Shared.Toolshed.Syntax;

public sealed class VarRef<T>(string varName) : ValueRef<T>
{
  public readonly string VarName = varName;

  public override T? Evaluate(IInvocationContext ctx)
  {
    object obj1 = ctx.ReadVar(this.VarName);
    if (obj1 is T obj2)
      return obj2;
    VarRef<T>.BadVarTypeError err = new VarRef<T>.BadVarTypeError(obj1?.GetType(), typeof (T), this.VarName);
    ctx.ReportError((IConError) err);
    return default (T);
  }

  public record BadVarTypeError(Type? Got, Type Expected, string VarName) : IConError
  {
    public FormattedMessage DescribeInner()
    {
      string text;
      if (!(this.Got == (Type) null))
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 3);
        interpolatedStringHandler.AppendLiteral("Variable $");
        interpolatedStringHandler.AppendFormatted(this.VarName);
        interpolatedStringHandler.AppendLiteral(" is not of the expected type. Expected ");
        interpolatedStringHandler.AppendFormatted(this.Expected.PrettyName());
        interpolatedStringHandler.AppendLiteral(" but got ");
        ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
        Type got = this.Got;
        string str = (object) got != null ? got.PrettyName() : (string) null;
        local.AppendFormatted(str);
        interpolatedStringHandler.AppendLiteral(".");
        text = interpolatedStringHandler.ToStringAndClear();
      }
      else
        text = $"Variable ${this.VarName} is not assigned. Expected variable of type {this.Expected.PrettyName()}.";
      return FormattedMessage.FromUnformatted(text);
    }

    public string? Expression { get; set; }

    public Vector2i? IssueSpan { get; set; }

    public StackTrace? Trace { get; set; }

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("Got = ");
      builder.Append((object) this.Got);
      builder.Append(", Expected = ");
      builder.Append((object) this.Expected);
      builder.Append(", VarName = ");
      builder.Append((object) this.VarName);
      builder.Append(", Expression = ");
      builder.Append((object) this.Expression);
      builder.Append(", IssueSpan = ");
      builder.Append(this.IssueSpan.ToString());
      builder.Append(", Trace = ");
      builder.Append((object) this.Trace);
      return true;
    }
  }
}
