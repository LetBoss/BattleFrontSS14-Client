// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.DumpNetComponentsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class DumpNetComponentsCommand : LocalizedCommands
{
  [Dependency]
  private readonly IComponentFactory _componentFactory;

  public override string Command => "dump_net_comps";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    IReadOnlyList<ComponentRegistration> networkedComponents = this._componentFactory.NetworkedComponents;
    if (networkedComponents == null)
    {
      shell.WriteError(this.Loc.GetString("cmd-dump_net_comps-error-writeable"));
    }
    else
    {
      shell.WriteLine(this.Loc.GetString("cmd-dump_net_comps-header"));
      for (int index = 0; index < networkedComponents.Count; ++index)
      {
        ComponentRegistration componentRegistration = networkedComponents[index];
        shell.WriteLine($"  [{index,4}] {componentRegistration.Name,-16} {componentRegistration.Type.Name}");
      }
    }
  }
}
