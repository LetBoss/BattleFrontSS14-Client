// Decompiled with JetBrains decompiler
// Type: Content.Client.Access.UI.IdCardConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.CrewManifest;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Access.UI;

public sealed class IdCardConsoleBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _cfgManager;
  private readonly SharedIdCardConsoleSystem _idCardConsoleSystem;
  private IdCardConsoleWindow? _window;
  private int _maxNameLength;
  private int _maxIdJobLength;

  public IdCardConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._idCardConsoleSystem = this.EntMan.System<SharedIdCardConsoleSystem>();
    this._maxNameLength = this._cfgManager.GetCVar<int>(CCVars.MaxNameLength);
    this._maxIdJobLength = this._cfgManager.GetCVar<int>(CCVars.MaxIdJobLength);
  }

  protected virtual void Open()
  {
    base.Open();
    IdCardConsoleComponent consoleComponent;
    List<ProtoId<AccessLevelPrototype>> accessLevels;
    if (this.EntMan.TryGetComponent<IdCardConsoleComponent>(this.Owner, ref consoleComponent))
    {
      accessLevels = consoleComponent.AccessLevels;
    }
    else
    {
      accessLevels = new List<ProtoId<AccessLevelPrototype>>();
      this._idCardConsoleSystem.Log.Error($"No IdCardConsole component found for {this.EntMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(this.Owner))}!");
    }
    IdCardConsoleWindow cardConsoleWindow = new IdCardConsoleWindow(this, this._prototypeManager, accessLevels);
    cardConsoleWindow.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    this._window = cardConsoleWindow;
    ((BaseButton) this._window.CrewManifestButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new CrewManifestOpenUiMessage()));
    ((BaseButton) this._window.PrivilegedIdButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent(IdCardConsoleComponent.PrivilegedIdCardSlotId)));
    ((BaseButton) this._window.TargetIdButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent(IdCardConsoleComponent.TargetIdCardSlotId)));
    ((BaseWindow) this._window).OnClose += new Action(((BoundUserInterface) this).Close);
    ((BaseWindow) this._window).OpenCentered();
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState((IdCardConsoleComponent.IdCardConsoleBoundUserInterfaceState) state);
  }

  public void SubmitData(
    string newFullName,
    string newJobTitle,
    List<ProtoId<AccessLevelPrototype>> newAccessList,
    string newJobPrototype)
  {
    if (newFullName.Length > this._maxNameLength)
      newFullName = newFullName.Substring(0, this._maxNameLength);
    if (newJobTitle.Length > this._maxIdJobLength)
      newJobTitle = newJobTitle.Substring(0, this._maxIdJobLength);
    this.SendMessage((BoundUserInterfaceMessage) new IdCardConsoleComponent.WriteToTargetIdMessage(newFullName, newJobTitle, newAccessList, ProtoId<AccessLevelPrototype>.op_Implicit(newJobPrototype)));
  }
}
