// Decompiled with JetBrains decompiler
// Type: Content.Shared.Remotes.EntitySystems.SharedDoorRemoteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Remotes.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Remotes.EntitySystems;

public abstract class SharedDoorRemoteSystem : EntitySystem
{
  [Dependency]
  protected SharedPopupSystem Popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DoorRemoteComponent, UseInHandEvent>(new EntityEventRefHandler<DoorRemoteComponent, UseInHandEvent>(this.OnInHandActivation));
  }

  private void OnInHandActivation(Entity<DoorRemoteComponent> entity, ref UseInHandEvent args)
  {
    string messageId;
    switch (entity.Comp.Mode)
    {
      case OperatingMode.OpenClose:
        entity.Comp.Mode = OperatingMode.ToggleBolts;
        messageId = "door-remote-switch-state-toggle-bolts";
        break;
      case OperatingMode.ToggleBolts:
        entity.Comp.Mode = OperatingMode.ToggleEmergencyAccess;
        messageId = "door-remote-switch-state-toggle-emergency-access";
        break;
      case OperatingMode.ToggleEmergencyAccess:
        entity.Comp.Mode = OperatingMode.OpenClose;
        messageId = "door-remote-switch-state-open-close";
        break;
      default:
        throw new InvalidOperationException($"{"DoorRemoteComponent"} had invalid mode {entity.Comp.Mode}");
    }
    this.Dirty<DoorRemoteComponent>(entity);
    this.Popup.PopupClient(this.Loc.GetString(messageId), (EntityUid) entity, new EntityUid?(args.User));
  }
}
