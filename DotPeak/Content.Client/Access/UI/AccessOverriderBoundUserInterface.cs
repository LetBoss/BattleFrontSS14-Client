// Decompiled with JetBrains decompiler
// Type: Content.Client.Access.UI.AccessOverriderBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Access.UI;

public sealed class AccessOverriderBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly SharedAccessOverriderSystem _accessOverriderSystem;
  private AccessOverriderWindow? _window;

  public AccessOverriderBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._accessOverriderSystem = this.EntMan.System<SharedAccessOverriderSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<AccessOverriderWindow>((BoundUserInterface) this);
    this.RefreshAccess();
    this._window.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    this._window.OnSubmit += new Action<List<ProtoId<AccessLevelPrototype>>>(this.SubmitData);
    ((BaseButton) this._window.PrivilegedIdButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent(AccessOverriderComponent.PrivilegedIdCardSlotId)));
  }

  public virtual void OnProtoReload(PrototypesReloadedEventArgs args)
  {
    base.OnProtoReload(args);
    if (!args.WasModified<AccessLevelPrototype>())
      return;
    this.RefreshAccess();
    if (this.State == null)
      return;
    this._window?.UpdateState(this._prototypeManager, (AccessOverriderComponent.AccessOverriderBoundUserInterfaceState) this.State);
  }

  private void RefreshAccess()
  {
    AccessOverriderComponent overriderComponent;
    List<ProtoId<AccessLevelPrototype>> accessLevels;
    if (this.EntMan.TryGetComponent<AccessOverriderComponent>(this.Owner, ref overriderComponent))
    {
      accessLevels = overriderComponent.AccessLevels;
      accessLevels.Sort();
    }
    else
    {
      accessLevels = new List<ProtoId<AccessLevelPrototype>>();
      this._accessOverriderSystem.Log.Error($"No AccessOverrider component found for {this.EntMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(this.Owner))}!");
    }
    this._window?.SetAccessLevels(this._prototypeManager, accessLevels);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState(this._prototypeManager, (AccessOverriderComponent.AccessOverriderBoundUserInterfaceState) state);
  }

  public void SubmitData(List<ProtoId<AccessLevelPrototype>> newAccessList)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AccessOverriderComponent.WriteToTargetAccessReaderIdMessage(newAccessList));
  }
}
