// Decompiled with JetBrains decompiler
// Type: Content.Shared.Turrets.SharedDeployableTurretSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wires;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Turrets;

public abstract class SharedDeployableTurretSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedWiresSystem _wires;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DeployableTurretComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DeployableTurretComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<DeployableTurretComponent, AttemptChangePanelEvent>(new EntityEventRefHandler<DeployableTurretComponent, AttemptChangePanelEvent>(this.OnAttemptChangeWirePanelWire));
    this.SubscribeLocalEvent<DeployableTurretComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<DeployableTurretComponent, GetVerbsEvent<Verb>>(this.OnGetVerb));
  }

  private void OnGetVerb(Entity<DeployableTurretComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || !this._accessReader.IsAllowed(args.User, (EntityUid) ent))
      return;
    EntityUid user = args.User;
    Verb verb = new Verb()
    {
      Priority = 1,
      Text = ent.Comp.Enabled ? this.Loc.GetString("deployable-turret-component-deactivate") : this.Loc.GetString("deployable-turret-component-activate"),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png")),
      Disabled = !this.HasAmmo(ent),
      Impact = LogImpact.Low,
      Act = (Action) (() => this.TryToggleState(ent, new EntityUid?(user)))
    };
    args.Verbs.Add(verb);
  }

  private void OnActivate(Entity<DeployableTurretComponent> ent, ref ActivateInWorldEvent args)
  {
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) ent, out comp) && !this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) ent, comp), true))
      return;
    if (!this._accessReader.IsAllowed(args.User, (EntityUid) ent))
    {
      this._popup.PopupClient(this.Loc.GetString("deployable-turret-component-access-denied"), (EntityUid) ent, new EntityUid?(args.User));
      this._audio.PlayPredicted(ent.Comp.AccessDeniedSound, (EntityUid) ent, new EntityUid?(args.User));
    }
    else
      this.TryToggleState(ent, new EntityUid?(args.User));
  }

  private void OnAttemptChangeWirePanelWire(
    Entity<DeployableTurretComponent> ent,
    ref AttemptChangePanelEvent args)
  {
    if (!ent.Comp.Enabled || args.Cancelled)
      return;
    this._popup.PopupClient(this.Loc.GetString("deployable-turret-component-cannot-access-wires"), (EntityUid) ent, args.User);
    args.Cancelled = true;
  }

  public bool TryToggleState(Entity<DeployableTurretComponent> ent, EntityUid? user = null)
  {
    return this.TrySetState(ent, !ent.Comp.Enabled, user);
  }

  public bool TrySetState(Entity<DeployableTurretComponent> ent, bool enabled, EntityUid? user = null)
  {
    if (enabled && ent.Comp.CurrentState == DeployableTurretState.Broken)
    {
      if (user.HasValue)
        this._popup.PopupClient(this.Loc.GetString("deployable-turret-component-is-broken"), (EntityUid) ent, new EntityUid?(user.Value));
      return false;
    }
    if (enabled && !this.HasAmmo(ent))
    {
      if (user.HasValue)
        this._popup.PopupClient(this.Loc.GetString("deployable-turret-component-no-ammo"), (EntityUid) ent, new EntityUid?(user.Value));
      return false;
    }
    this.SetState(ent, enabled, user);
    return true;
  }

  protected virtual void SetState(
    Entity<DeployableTurretComponent> ent,
    bool enabled,
    EntityUid? user = null)
  {
    if (ent.Comp.Enabled == enabled)
      return;
    WiresPanelComponent comp1;
    if (enabled && this.TryComp<WiresPanelComponent>((EntityUid) ent, out comp1) && comp1.Open)
    {
      this._wires.TogglePanel((EntityUid) ent, comp1, false);
      this._audio.PlayPredicted(comp1.ScrewdriverCloseSound, (EntityUid) ent, user);
    }
    float num1 = MathF.Max((float) (ent.Comp.AnimationCompletionTime - this._timing.CurTime).TotalSeconds, 0.0f);
    float num2 = enabled ? ent.Comp.DeploymentLength : ent.Comp.RetractionLength;
    ent.Comp.AnimationCompletionTime = this._timing.CurTime + TimeSpan.FromSeconds((double) num2 + (double) num1);
    DamageableComponent comp2;
    if (this.TryComp<DamageableComponent>((EntityUid) ent, out comp2))
    {
      ProtoId<DamageModifierSetPrototype>? nullable1 = enabled ? ent.Comp.DeployedDamageModifierSetId : ent.Comp.RetractedDamageModifierSetId;
      DamageableSystem damageable = this._damageable;
      EntityUid uid = (EntityUid) ent;
      ProtoId<DamageModifierSetPrototype>? nullable2 = nullable1;
      string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
      DamageableComponent comp3 = comp2;
      damageable.SetDamageModifierSetId(uid, valueOrDefault, comp3);
    }
    FixturesComponent comp4;
    Fixture fixture;
    if (ent.Comp.DeployedFixture != null && this.TryComp<FixturesComponent>((EntityUid) ent, out comp4) && comp4.Fixtures.TryGetValue(ent.Comp.DeployedFixture, out fixture))
      this._physics.SetHard((EntityUid) ent, fixture, enabled);
    this._popup.PopupClient(this.Loc.GetString(enabled ? "deployable-turret-component-activating" : "deployable-turret-component-deactivating"), (EntityUid) ent, user);
    ent.Comp.Enabled = enabled;
    this.DirtyField<DeployableTurretComponent>((EntityUid) ent, ent.Comp, "Enabled");
  }

  public bool HasAmmo(Entity<DeployableTurretComponent> ent)
  {
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>((EntityUid) ent, ref args);
    return args.Count > 0;
  }
}
