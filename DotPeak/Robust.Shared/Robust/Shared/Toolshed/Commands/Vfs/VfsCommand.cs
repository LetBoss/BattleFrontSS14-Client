// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Vfs.VfsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Vfs;

public abstract class VfsCommand : ToolshedCommand
{
  [Dependency]
  protected readonly IResourceManager Resources;
  public const string UserVfsLocVariableName = "user_vfs_loc";

  protected ResPath CurrentPath(IInvocationContext ctx)
  {
    return (ResPath?) ctx.ReadVar("user_vfs_loc") ?? ResPath.Root;
  }

  protected void SetPath(IInvocationContext ctx, ResPath path)
  {
    ctx.WriteVar("user_vfs_loc", (object) path.Clean());
  }
}
