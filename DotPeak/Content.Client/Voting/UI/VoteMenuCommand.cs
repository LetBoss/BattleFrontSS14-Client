// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.UI.VoteMenuCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Robust.Shared.Console;

#nullable enable
namespace Content.Client.Voting.UI;

[AnyCommand]
public sealed class VoteMenuCommand : LocalizedCommands
{
  public virtual string Command => "votemenu";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    new VoteCallMenu().OpenCentered();
  }
}
