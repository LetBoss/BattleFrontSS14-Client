// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandMethod
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Toolshed;

internal sealed class CommandMethod
{
  public readonly MethodInfo Info;
  public readonly ParameterInfo? PipeArg;
  public readonly bool Generic;
  public readonly bool Invertible;
  public readonly bool PipeGeneric;
  public readonly CommandArgument[] Arguments;

  public CommandMethod(MethodInfo info, ToolshedCommandImplementor impl)
  {
    this.Info = info;
    this.PipeArg = info.ConsoleGetPipedArgument();
    this.Invertible = info.ConsoleHasInvertedArgument();
    this.Arguments = ((IEnumerable<ParameterInfo>) info.GetParameters()).Where<ParameterInfo>((Func<ParameterInfo, bool>) (x => x.IsCommandArgument())).Select<ParameterInfo, CommandArgument>(new Func<ParameterInfo, CommandArgument>(impl.GetCommandArgument)).ToArray<CommandArgument>();
    if (!info.IsGenericMethodDefinition)
      return;
    this.Generic = true;
    this.PipeGeneric = info.HasCustomAttribute<TakesPipedTypeAsGenericAttribute>();
  }
}
