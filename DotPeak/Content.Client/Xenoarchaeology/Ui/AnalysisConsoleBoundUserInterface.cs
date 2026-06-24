// Decompiled with JetBrains decompiler
// Type: Content.Client.Xenoarchaeology.Ui.AnalysisConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Research.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Xenoarchaeology.Ui;

public sealed class AnalysisConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private AnalysisConsoleMenu? _consoleMenu;

  protected virtual void Open()
  {
    base.Open();
    this._consoleMenu = BoundUserInterfaceExt.CreateWindow<AnalysisConsoleMenu>((BoundUserInterface) this);
    this._consoleMenu.SetOwner(this.Owner);
    this._consoleMenu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._consoleMenu.OpenCentered();
    this._consoleMenu.OnServerSelectionButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new ConsoleServerSelectionMessage()));
    this._consoleMenu.OnExtractButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new AnalysisConsoleExtractButtonPressedMessage()));
  }

  public void Update(Entity<AnalysisConsoleComponent> ent) => this._consoleMenu?.Update(ent);

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._consoleMenu)?.Orphan();
  }
}
