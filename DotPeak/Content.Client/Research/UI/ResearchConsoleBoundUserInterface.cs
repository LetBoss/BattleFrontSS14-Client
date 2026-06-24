// Decompiled with JetBrains decompiler
// Type: Content.Client.Research.UI.ResearchConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Research.UI;

public sealed class ResearchConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ResearchConsoleMenu? _consoleMenu;

  protected virtual void Open()
  {
    base.Open();
    EntityUid owner = this.Owner;
    this._consoleMenu = BoundUserInterfaceExt.CreateWindow<ResearchConsoleMenu>((BoundUserInterface) this);
    this._consoleMenu.SetEntity(owner);
    this._consoleMenu.OnTechnologyCardPressed += (Action<string>) (id => this.SendMessage((BoundUserInterfaceMessage) new ConsoleUnlockTechnologyMessage(id)));
    this._consoleMenu.OnServerButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new ConsoleServerSelectionMessage()));
  }

  public virtual void OnProtoReload(PrototypesReloadedEventArgs args)
  {
    base.OnProtoReload(args);
    if (!args.WasModified<TechnologyPrototype>() || !(this.State is ResearchConsoleBoundInterfaceState state))
      return;
    this._consoleMenu?.UpdatePanels(state);
    this._consoleMenu?.UpdateInformationPanel(state);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ResearchConsoleBoundInterfaceState state1))
      return;
    this._consoleMenu?.UpdatePanels(state1);
    this._consoleMenu?.UpdateInformationPanel(state1);
  }
}
