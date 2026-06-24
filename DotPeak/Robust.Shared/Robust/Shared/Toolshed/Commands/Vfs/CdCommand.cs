// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Vfs.CdCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Vfs;

[ToolshedCommand]
internal sealed class CdCommand : VfsCommand
{
  [CommandImplementation(null)]
  public void Cd(IInvocationContext ctx, ResPath path)
  {
    ResPath resPath = this.CurrentPath(ctx);
    ResPath path1 = !path.IsRooted ? resPath / path : path;
    this.SetPath(ctx, path1);
  }
}
