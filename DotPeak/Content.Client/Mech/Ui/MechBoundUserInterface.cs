// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.Ui.MechBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Mech.Ui;

public sealed class MechBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private MechMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<MechMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.OnRemoveButtonPressed += (Action<EntityUid>) (uid => this.SendMessage((BoundUserInterfaceMessage) new MechEquipmentRemoveMessage(this.EntMan.GetNetEntity(uid, (MetaDataComponent) null))));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is MechBoundUiState state1))
      return;
    this.UpdateEquipmentControls(state1);
    this._menu?.UpdateMechStats();
    this._menu?.UpdateEquipmentView();
  }

  public void UpdateEquipmentControls(MechBoundUiState state)
  {
    MechComponent mechComponent;
    if (!this.EntMan.TryGetComponent<MechComponent>(this.Owner, ref mechComponent))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ((BaseContainer) mechComponent.EquipmentContainer).ContainedEntities)
    {
      UIFragment equipmentUi = this.GetEquipmentUi(new EntityUid?(containedEntity));
      if (equipmentUi != null)
      {
        foreach ((NetEntity key, BoundUserInterfaceState state1) in state.EquipmentStates)
        {
          if (EntityUid.op_Equality(containedEntity, this.EntMan.GetEntity(key)))
            equipmentUi.UpdateState(state1);
        }
      }
    }
  }

  public UIFragment? GetEquipmentUi(EntityUid? uid)
  {
    UIFragmentComponent componentOrNull = EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(this.EntMan, uid);
    componentOrNull?.Ui?.Setup((BoundUserInterface) this, uid);
    return componentOrNull?.Ui;
  }
}
