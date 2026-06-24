// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commands.ShowPvoCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Pvo;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._CIV14merka.Commands;

public sealed class ShowPvoCommand : IConsoleCommand
{
  [Dependency]
  private IEntityManager _entities;

  public string Command => "showpvo";

  public string Description => "тест. оверлей радиуса пво";

  public string Help => "Usage: " + this.Command;

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 0)
    {
      shell.WriteLine(this.Help);
    }
    else
    {
      bool flag = this._entities.System<CivPvoOverlaySystem>().Toggle();
      shell.WriteLine($"showpvo: {flag}");
    }
  }
}
