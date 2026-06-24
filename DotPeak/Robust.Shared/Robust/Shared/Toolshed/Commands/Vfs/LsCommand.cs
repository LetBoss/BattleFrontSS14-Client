// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Vfs.LsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Vfs;

[ToolshedCommand]
internal sealed class LsCommand : VfsCommand
{
  [CommandImplementation("here")]
  public IEnumerable<ResPath> LsHere(IInvocationContext ctx)
  {
    ResPath curPath = this.CurrentPath(ctx);
    return this.Resources.ContentGetDirectoryEntries(curPath).Select<string, ResPath>((Func<string, ResPath>) (x => curPath / x));
  }

  [CommandImplementation("in")]
  public IEnumerable<ResPath> LsIn(IInvocationContext ctx, ResPath @in)
  {
    ResPath curPath = this.CurrentPath(ctx);
    if (@in.IsRooted)
      curPath = @in;
    else
      curPath /= @in;
    return this.Resources.ContentGetDirectoryEntries(curPath).Select<string, ResPath>((Func<string, ResPath>) (x => curPath / x));
  }
}
