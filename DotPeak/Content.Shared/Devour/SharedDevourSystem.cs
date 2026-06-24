// Decompiled with JetBrains decompiler
// Type: Content.Shared.Devour.SharedDevourSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Devour.Components;
using Content.Shared.DoAfter;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Devour;

public abstract class SharedDevourSystem : EntitySystem
{
  [Dependency]
  protected SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  protected SharedContainerSystem ContainerSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DevourerComponent, MapInitEvent>(new ComponentEventHandler<DevourerComponent, MapInitEvent>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DevourerComponent, DevourActionEvent>(new ComponentEventHandler<DevourerComponent, DevourActionEvent>((object) this, __methodptr(OnDevourAction)), (Type[]) null, (Type[]) null);
  }

  protected void OnInit(EntityUid uid, DevourerComponent component, MapInitEvent args)
  {
    component.Stomach = this.ContainerSystem.EnsureContainer<Container>(uid, "stomach", (ContainerManagerComponent) null);
    this._actionsSystem.AddAction(uid, ref component.DevourActionEntity, component.DevourAction, new EntityUid());
  }

  protected void OnDevourAction(EntityUid uid, DevourerComponent component, DevourActionEvent args)
  {
    if (args.Handled || this._whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, args.Target))
      return;
    args.Handled = true;
    EntityUid target = args.Target;
    MobStateComponent mobStateComponent;
    if (this.TryComp<MobStateComponent>(target, ref mobStateComponent))
    {
      switch (mobStateComponent.CurrentState)
      {
        case MobState.Critical:
        case MobState.Dead:
          this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, uid, component.DevourTime, (DoAfterEvent) new DevourDoAfterEvent(), new EntityUid?(uid), new EntityUid?(target), new EntityUid?(uid))
          {
            BreakOnMove = true
          });
          break;
        default:
          this._popupSystem.PopupClient(this.Loc.GetString("devour-action-popup-message-fail-target-alive"), uid, new EntityUid?(uid));
          break;
      }
    }
    else
    {
      this._popupSystem.PopupClient(this.Loc.GetString("devour-action-popup-message-structure"), uid, new EntityUid?(uid));
      if (component.SoundStructureDevour != null)
        this._audioSystem.PlayPredicted(component.SoundStructureDevour, uid, new EntityUid?(uid), new AudioParams?(component.SoundStructureDevour.Params));
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, uid, component.StructureDevourTime, (DoAfterEvent) new DevourDoAfterEvent(), new EntityUid?(uid), new EntityUid?(target), new EntityUid?(uid))
      {
        BreakOnMove = true
      });
    }
  }
}
