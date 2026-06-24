// Decompiled with JetBrains decompiler
// Type: Content.Client.Kitchen.UI.ReagentGrinderBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Kitchen.UI;

public sealed class ReagentGrinderBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GrinderMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<GrinderMenu>((BoundUserInterface) this);
    this._menu.OnToggleAuto += new Action(this.ToggleAutoMode);
    this._menu.OnGrind += new Action(this.StartGrinding);
    this._menu.OnJuice += new Action(this.StartJuicing);
    this._menu.OnEjectAll += new Action(this.EjectAll);
    this._menu.OnEjectBeaker += new Action(this.EjectBeaker);
    this._menu.OnEjectChamber += new Action<EntityUid>(this.EjectChamberContent);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ReagentGrinderInterfaceState state1))
      return;
    this._menu?.UpdateState(state1);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    base.ReceiveMessage(message);
    this._menu?.HandleMessage(message);
  }

  public void ToggleAutoMode()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ReagentGrinderToggleAutoModeMessage());
  }

  public void StartGrinding()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ReagentGrinderStartMessage(GrinderProgram.Grind));
  }

  public void StartJuicing()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ReagentGrinderStartMessage(GrinderProgram.Juice));
  }

  public void EjectAll()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ReagentGrinderEjectChamberAllMessage());
  }

  public void EjectBeaker()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent(SharedReagentGrinder.BeakerSlotId));
  }

  public void EjectChamberContent(EntityUid uid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ReagentGrinderEjectChamberContentMessage(this.EntMan.GetNetEntity(uid, (MetaDataComponent) null)));
  }
}
