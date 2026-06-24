// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgBipodSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

public sealed class PubgBipodSystem : EntitySystem
{
  [Dependency]
  private readonly PubgWeaponModulesSystem _modules;
  [Dependency]
  private readonly SharedHandsSystem _hands;
  [Dependency]
  private readonly SharedDoAfterSystem _doAfter;
  [Dependency]
  private readonly SharedGunSystem _gun;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, PubgToggleBipodActionEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, PubgToggleBipodActionEvent>(this.OnToggleBipodAction));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, PubgBipodDeployDoAfterEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, PubgBipodDeployDoAfterEvent>(this.OnDeployDoAfter));
    this.SubscribeLocalEvent<PubgBipodDeployedComponent, MoveInputEvent>(new EntityEventRefHandler<PubgBipodDeployedComponent, MoveInputEvent>(this.OnDeployedMoveInput));
  }

  private void OnToggleBipodAction(
    Entity<PubgWeaponModulesComponent> ent,
    ref PubgToggleBipodActionEvent args)
  {
    HandsComponent comp1;
    EntityUid module;
    PubgBipodComponent comp2;
    if (args.Handled || !this.TryComp<HandsComponent>(args.Performer, out comp1) || !this._hands.IsHolding((Entity<HandsComponent>) (args.Performer, comp1), new EntityUid?(ent.Owner)) || !this._modules.TryGetInstalledModule((EntityUid) ent, PubgModuleSlotType.Underbarrel, out module, out PubgWeaponModuleSlotDefinition _, ent.Comp) || !this.TryComp<PubgBipodComponent>(module, out comp2))
      return;
    if (comp2.Deployed)
    {
      this.Undeploy((Entity<PubgBipodComponent>) (module, comp2), (EntityUid) ent, args.Performer);
    }
    else
    {
      EntityManager entityManager = this.EntityManager;
      EntityUid performer = args.Performer;
      double deployTime = (double) comp2.DeployTime;
      PubgBipodDeployDoAfterEvent @event = new PubgBipodDeployDoAfterEvent();
      EntityUid? eventTarget = new EntityUid?(ent.Owner);
      EntityUid? nullable = new EntityUid?(ent.Owner);
      EntityUid? target = new EntityUid?();
      EntityUid? used = nullable;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, performer, (float) deployTime, (DoAfterEvent) @event, eventTarget, target, used)
      {
        NeedHand = true,
        BreakOnMove = true,
        BreakOnHandChange = true
      });
    }
    args.Handled = true;
  }

  private void OnDeployDoAfter(
    Entity<PubgWeaponModulesComponent> ent,
    ref PubgBipodDeployDoAfterEvent args)
  {
    EntityUid module;
    PubgBipodComponent comp;
    if (args.Cancelled || args.Handled || !this._modules.TryGetInstalledModule((EntityUid) ent, PubgModuleSlotType.Underbarrel, out module, out PubgWeaponModuleSlotDefinition _, ent.Comp) || !this.TryComp<PubgBipodComponent>(module, out comp))
      return;
    if (!comp.Deployed)
      this.Deploy((Entity<PubgBipodComponent>) (module, comp), (EntityUid) ent, args.User);
    args.Handled = true;
  }

  private void Deploy(Entity<PubgBipodComponent> bipod, EntityUid gun, EntityUid user)
  {
    bipod.Comp.Deployed = true;
    this.Dirty<PubgBipodComponent>(bipod);
    this.EnsureComp<PubgBipodDeployedComponent>(user);
    this._gun.RefreshModifiers((Entity<GunComponent>) gun);
    this._audio.PlayPredicted(bipod.Comp.DeploySound, gun, new EntityUid?(user));
    this._popup.PopupClient(this.Loc.GetString("pubg-bipod-deploy"), user, new EntityUid?(user));
  }

  private void Undeploy(Entity<PubgBipodComponent> bipod, EntityUid gun, EntityUid user)
  {
    if (!bipod.Comp.Deployed)
      return;
    bipod.Comp.Deployed = false;
    this.Dirty<PubgBipodComponent>(bipod);
    this._gun.RefreshModifiers((Entity<GunComponent>) gun);
    this._audio.PlayPredicted(bipod.Comp.UndeploySound, gun, new EntityUid?(user));
    this._popup.PopupClient(this.Loc.GetString("pubg-bipod-undeploy"), user, new EntityUid?(user));
    if (this.HasDeployedBipodInHands(user))
      return;
    this.RemCompDeferred<PubgBipodDeployedComponent>(user);
  }

  private void OnDeployedMoveInput(Entity<PubgBipodDeployedComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    HandsComponent comp1;
    if (!this.TryComp<HandsComponent>(ent.Owner, out comp1))
    {
      this.RemCompDeferred<PubgBipodDeployedComponent>((EntityUid) ent);
    }
    else
    {
      foreach (string key in comp1.Hands.Keys)
      {
        EntityUid? held;
        PubgWeaponModulesComponent comp2;
        EntityUid module;
        PubgBipodComponent comp3;
        if (this._hands.TryGetHeldItem((Entity<HandsComponent>) (ent.Owner, comp1), key, out held) && held.HasValue && this.TryComp<PubgWeaponModulesComponent>(held.Value, out comp2) && this._modules.TryGetInstalledModule(held.Value, PubgModuleSlotType.Underbarrel, out module, out PubgWeaponModuleSlotDefinition _, comp2) && this.TryComp<PubgBipodComponent>(module, out comp3) && comp3.Deployed)
          this.Undeploy((Entity<PubgBipodComponent>) (module, comp3), held.Value, ent.Owner);
      }
      this.RemCompDeferred<PubgBipodDeployedComponent>((EntityUid) ent);
    }
  }

  private bool HasDeployedBipodInHands(EntityUid user)
  {
    HandsComponent comp1;
    if (!this.TryComp<HandsComponent>(user, out comp1))
      return false;
    foreach (string key in comp1.Hands.Keys)
    {
      EntityUid? held;
      PubgWeaponModulesComponent comp2;
      EntityUid module;
      PubgBipodComponent comp3;
      if (this._hands.TryGetHeldItem((Entity<HandsComponent>) (user, comp1), key, out held) && held.HasValue && this.TryComp<PubgWeaponModulesComponent>(held.Value, out comp2) && this._modules.TryGetInstalledModule(held.Value, PubgModuleSlotType.Underbarrel, out module, out PubgWeaponModuleSlotDefinition _, comp2) && this.TryComp<PubgBipodComponent>(module, out comp3) && comp3.Deployed)
        return true;
    }
    return false;
  }
}
