// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.SharedMaterialReclaimerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Materials;

public abstract class SharedMaterialReclaimerSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  protected SharedAmbientSoundSystem AmbientSound;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected SharedContainerSystem Container;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private EmagSystem _emag;
  public const string ActiveReclaimerContainerId = "active-material-reclaimer-container";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MaterialReclaimerComponent, ComponentShutdown>(new ComponentEventHandler<MaterialReclaimerComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<MaterialReclaimerComponent, ExaminedEvent>(new ComponentEventHandler<MaterialReclaimerComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<MaterialReclaimerComponent, GotEmaggedEvent>(new ComponentEventRefHandler<MaterialReclaimerComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<MaterialReclaimerComponent, MapInitEvent>(new ComponentEventHandler<MaterialReclaimerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<CollideMaterialReclaimerComponent, StartCollideEvent>(new ComponentEventRefHandler<CollideMaterialReclaimerComponent, StartCollideEvent>(this.OnCollide));
    this.SubscribeLocalEvent<ActiveMaterialReclaimerComponent, ComponentStartup>(new ComponentEventHandler<ActiveMaterialReclaimerComponent, ComponentStartup>(this.OnActiveStartup));
  }

  private void OnMapInit(EntityUid uid, MaterialReclaimerComponent component, MapInitEvent args)
  {
    component.NextSound = this.Timing.CurTime;
  }

  private void OnShutdown(
    EntityUid uid,
    MaterialReclaimerComponent component,
    ComponentShutdown args)
  {
    this._audio.Stop(component.Stream);
  }

  private void OnExamined(EntityUid uid, MaterialReclaimerComponent component, ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("recycler-count-items", ("items", (object) component.ItemsProcessed)));
  }

  private void OnEmagged(
    EntityUid uid,
    MaterialReclaimerComponent component,
    ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction))
      return;
    args.Handled = true;
  }

  private void OnCollide(
    EntityUid uid,
    CollideMaterialReclaimerComponent component,
    ref StartCollideEvent args)
  {
    MaterialReclaimerComponent comp;
    if (args.OurFixtureId != component.FixtureId || !this.TryComp<MaterialReclaimerComponent>(uid, out comp))
      return;
    this.TryStartProcessItem(uid, args.OtherEntity, comp);
  }

  private void OnActiveStartup(
    EntityUid uid,
    ActiveMaterialReclaimerComponent component,
    ComponentStartup args)
  {
    component.ReclaimingContainer = this.Container.EnsureContainer<Robust.Shared.Containers.Container>(uid, "active-material-reclaimer-container");
  }

  public bool TryStartProcessItem(
    EntityUid uid,
    EntityUid item,
    MaterialReclaimerComponent? component = null,
    EntityUid? user = null)
  {
    if (!this.Resolve<MaterialReclaimerComponent>(uid, ref component) || !this.CanStart(uid, component) || this.HasComp<MobStateComponent>(item) && !this.CanGib(uid, item, component) || this._whitelistSystem.IsWhitelistFail(component.Whitelist, item) || this._whitelistSystem.IsBlacklistPass(component.Blacklist, item) || this.Container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (item, (TransformComponent) null, (MetaDataComponent) null), out BaseContainer _) && !this.Container.TryRemoveFromContainer((Entity<TransformComponent, MetaDataComponent>) item))
      return false;
    if (user.HasValue)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(39, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user.Value), "player", "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" destroyed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" in the material reclaimer, ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.High, ref local);
    }
    if (this.Timing.CurTime > component.NextSound)
    {
      MaterialReclaimerComponent reclaimerComponent = component;
      (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPredicted(component.Sound, uid, user);
      ref (EntityUid, AudioComponent)? local = ref nullable1;
      EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      reclaimerComponent.Stream = nullable2;
      component.NextSound = this.Timing.CurTime + component.SoundCooldown;
    }
    GotReclaimedEvent args = new GotReclaimedEvent(this.Transform(uid).Coordinates);
    this.RaiseLocalEvent<GotReclaimedEvent>(item, ref args);
    TimeSpan reclaimingDuration = this.GetReclaimingDuration(uid, item, component);
    if (reclaimingDuration == TimeSpan.Zero)
    {
      this.Reclaim(uid, item, component: component);
      return true;
    }
    ActiveMaterialReclaimerComponent reclaimerComponent1 = this.EnsureComp<ActiveMaterialReclaimerComponent>(uid);
    reclaimerComponent1.Duration = reclaimingDuration;
    reclaimerComponent1.EndTime = this.Timing.CurTime + reclaimingDuration;
    this.Container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) item, (BaseContainer) reclaimerComponent1.ReclaimingContainer);
    return true;
  }

  public virtual bool TryFinishProcessItem(
    EntityUid uid,
    MaterialReclaimerComponent? component = null,
    ActiveMaterialReclaimerComponent? active = null)
  {
    if (!this.Resolve<MaterialReclaimerComponent, ActiveMaterialReclaimerComponent>(uid, ref component, ref active, false))
      return false;
    this.RemCompDeferred(uid, (IComponent) active);
    return true;
  }

  public virtual void Reclaim(
    EntityUid uid,
    EntityUid item,
    float completion = 1f,
    MaterialReclaimerComponent? component = null)
  {
    if (!this.Resolve<MaterialReclaimerComponent>(uid, ref component))
      return;
    ++component.ItemsProcessed;
    if (component.CutOffSound)
      this._audio.Stop(component.Stream);
    this.Dirty(uid, (IComponent) component);
  }

  public bool SetReclaimerEnabled(
    EntityUid uid,
    bool enabled,
    MaterialReclaimerComponent? component = null)
  {
    if (!this.Resolve<MaterialReclaimerComponent>(uid, ref component, false))
      return true;
    if (component.Broken & enabled)
      return false;
    component.Enabled = enabled;
    this.AmbientSound.SetAmbience(uid, enabled && component.Powered);
    this.Dirty(uid, (IComponent) component);
    return true;
  }

  public bool CanStart(EntityUid uid, MaterialReclaimerComponent component)
  {
    return !this.HasComp<ActiveMaterialReclaimerComponent>(uid) && component.Powered && component.Enabled && !component.Broken;
  }

  public bool CanGib(EntityUid uid, EntityUid victim, MaterialReclaimerComponent component)
  {
    return component.Powered && component.Enabled && !component.Broken && this.HasComp<BodyComponent>(victim) && this._emag.CheckFlag(uid, EmagType.Interaction);
  }

  public TimeSpan GetReclaimingDuration(
    EntityUid reclaimer,
    EntityUid item,
    MaterialReclaimerComponent? reclaimerComponent = null,
    PhysicalCompositionComponent? compositionComponent = null)
  {
    if (!this.Resolve<MaterialReclaimerComponent>(reclaimer, ref reclaimerComponent))
      return TimeSpan.Zero;
    if (!reclaimerComponent.ScaleProcessSpeed || !this.Resolve<PhysicalCompositionComponent>(item, ref compositionComponent, false))
      return reclaimerComponent.MinimumProcessDuration;
    int num1 = compositionComponent.MaterialComposition.Values.Sum();
    StackComponent stackComponent = this.CompOrNull<StackComponent>(item);
    int num2 = stackComponent != null ? stackComponent.Count : 1;
    TimeSpan reclaimingDuration = TimeSpan.FromSeconds((double) (num1 * num2) / (double) reclaimerComponent.MaterialProcessRate);
    if (reclaimingDuration < reclaimerComponent.MinimumProcessDuration)
      reclaimingDuration = reclaimerComponent.MinimumProcessDuration;
    return reclaimingDuration;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveMaterialReclaimerComponent, MaterialReclaimerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveMaterialReclaimerComponent, MaterialReclaimerComponent>();
    EntityUid uid;
    ActiveMaterialReclaimerComponent comp1;
    MaterialReclaimerComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(this.Timing.CurTime < comp1.EndTime))
        this.TryFinishProcessItem(uid, comp2, comp1);
    }
  }
}
