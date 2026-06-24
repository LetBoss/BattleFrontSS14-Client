// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Fabricator.DropshipFabricatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared.Coordinates;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Fabricator;

public sealed class DropshipFabricatorSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PowerLoaderSystem _powerLoader;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private int _startingPoints;
  private TimeSpan _gainEvery;

  public ImmutableArray<EntProtoId<DropshipFabricatorPrintableComponent>> Printables { get; private set; }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<DropshipFabricatorComponent, MapInitEvent>(new EntityEventRefHandler<DropshipFabricatorComponent, MapInitEvent>(this.OnFabricatorMapInit));
    this.SubscribeLocalEvent<DropshipFabricatorComponent, DropshipFabricatoreRecycleDoafterEvent>(new EntityEventRefHandler<DropshipFabricatorComponent, DropshipFabricatoreRecycleDoafterEvent>(this.OnDropshipPartRecycled));
    this.Subs.BuiEvents<DropshipFabricatorComponent>((object) DropshipFabricatorUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<DropshipFabricatorComponent>) (subs => subs.Event<DropshipFabricatorPrintMsg>(new EntityEventRefHandler<DropshipFabricatorComponent, DropshipFabricatorPrintMsg>(this.OnPrintMsg))));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCDropshipFabricatorStartingPoints, (Action<int>) (v => this._startingPoints = v), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCDropshipFabricatorGainEverySeconds, (Action<float>) (v => this._gainEvery = TimeSpan.FromSeconds((double) v)), true);
    this.ReloadPrototypes();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
  {
    if (!ev.WasModified<EntityPrototype>())
      return;
    this.ReloadPrototypes();
  }

  private void OnFabricatorMapInit(Entity<DropshipFabricatorComponent> ent, ref MapInitEvent args)
  {
    if (!this._net.IsServer)
      return;
    ent.Comp.Account = new EntityUid?((EntityUid) this.EnsurePoints());
  }

  private void OnDropshipPartRecycled(
    Entity<DropshipFabricatorComponent> ent,
    ref DropshipFabricatoreRecycleDoafterEvent args)
  {
    DropshipFabricatorPrintableComponent comp1;
    DropshipFabricatorPointsComponent comp2;
    if (args.Cancelled || args.Handled || !this.TryComp<DropshipFabricatorPrintableComponent>(args.Used, out comp1) || !this.TryComp<DropshipFabricatorPointsComponent>(ent.Comp.Account, out comp2))
      return;
    args.Handled = true;
    int num = comp1.Cost;
    DropshipAmmoComponent comp3;
    if (this.TryComp<DropshipAmmoComponent>(args.Used, out comp3))
      num = (int) ((double) num * (double) comp3.Rounds / (double) comp3.MaxRounds);
    comp2.Points += (int) ((double) num * (double) comp1.RecycleMultiplier);
    this.Dirty(ent.Comp.Account.Value, (IComponent) comp2);
    this.Del(args.Used);
    this._audio.PlayPvs(ent.Comp.RecycleSound, (EntityUid) ent);
    this._powerLoader.TrySyncHands((Entity<PowerLoaderComponent>) args.User);
  }

  private void OnPrintMsg(
    Entity<DropshipFabricatorComponent> ent,
    ref DropshipFabricatorPrintMsg args)
  {
    EntityPrototype prototype;
    DropshipFabricatorPrintableComponent component;
    if (args.Id == new EntProtoId() || !this._prototypes.TryIndex(args.Id, out prototype) || !prototype.TryGetComponent<DropshipFabricatorPrintableComponent>(out component, this._compFactory))
      return;
    EntityUid actor = args.Actor;
    if (ent.Comp.Printing.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-dropship-fabricator-busy"), actor, new EntityUid?(actor), PopupType.SmallCaution);
    }
    else
    {
      DropshipFabricatorPointsComponent comp;
      if (!this.TryComp<DropshipFabricatorPointsComponent>(ent.Comp.Account, out comp) || component.Cost > comp.Points)
        return;
      comp.Points -= component.Cost;
      this.Dirty(ent.Comp.Account.Value, (IComponent) comp);
      ent.Comp.Points = comp.Points;
      ent.Comp.Printing = (EntProtoId<DropshipFabricatorPrintableComponent>?) prototype.ID;
      ent.Comp.PrintAt = this._timing.CurTime + component.Delay;
      this.Dirty<DropshipFabricatorComponent>(ent);
      this._appearance.SetData((EntityUid) ent, (Enum) DropshipFabricatorVisuals.State, (object) DropshipFabricatorState.Fabricating);
    }
  }

  private Entity<DropshipFabricatorPointsComponent> EnsurePoints()
  {
    EntityUid uid1;
    DropshipFabricatorPointsComponent comp1;
    if (this.EntityQueryEnumerator<DropshipFabricatorPointsComponent>().MoveNext(out uid1, out comp1))
      return (Entity<DropshipFabricatorPointsComponent>) (uid1, comp1);
    EntityUid uid2 = this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle());
    DropshipFabricatorPointsComponent fabricatorPointsComponent = this.EnsureComp<DropshipFabricatorPointsComponent>(uid2);
    fabricatorPointsComponent.Points = this._startingPoints;
    return (Entity<DropshipFabricatorPointsComponent>) (uid2, fabricatorPointsComponent);
  }

  private void ReloadPrototypes()
  {
    List<EntityPrototype> source = new List<EntityPrototype>();
    foreach (EntityPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<EntityPrototype>())
    {
      if (enumeratePrototype.HasComponent<DropshipFabricatorPrintableComponent>(this._compFactory))
        source.Add(enumeratePrototype);
    }
    source.Sort((Comparison<EntityPrototype>) ((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)));
    this.Printables = source.Select<EntityPrototype, EntProtoId<DropshipFabricatorPrintableComponent>>((Func<EntityPrototype, EntProtoId<DropshipFabricatorPrintableComponent>>) (e => new EntProtoId<DropshipFabricatorPrintableComponent>(e.ID))).ToImmutableArray<EntProtoId<DropshipFabricatorPrintableComponent>>();
  }

  public void ChangeBudget(int amount)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipFabricatorPointsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipFabricatorPointsComponent>();
    EntityUid uid;
    DropshipFabricatorPointsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Points += amount;
      this.Dirty(uid, (IComponent) comp1);
      this.SendUIStateAll(comp1.Points);
    }
  }

  private void SendUIStateAll(int points)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipFabricatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipFabricatorComponent>();
    EntityUid uid;
    DropshipFabricatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Points = points;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipFabricatorComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<DropshipFabricatorComponent, TransformComponent>();
    EntityUid uid1;
    DropshipFabricatorComponent comp1_1;
    TransformComponent comp2;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2))
    {
      if (!(curTime < comp1_1.PrintAt) && comp1_1.Printing.HasValue)
      {
        Angle worldRotation = this._transform.GetWorldRotation(comp2);
        EntityCoordinates coordinates = uid1.ToCoordinates().Offset(Vector2i.op_Implicit(((Vector2i) ref comp1_1.PrintOffset).Rotate(worldRotation)));
        this.SpawnAtPosition((string) comp1_1.Printing.Value, coordinates);
        comp1_1.Printing = new EntProtoId<DropshipFabricatorPrintableComponent>?();
        this.Dirty(uid1, (IComponent) comp1_1);
        this._appearance.SetData(uid1, (Enum) DropshipFabricatorVisuals.State, (object) DropshipFabricatorState.Idle);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipFabricatorPointsComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<DropshipFabricatorPointsComponent>();
    EntityUid uid2;
    DropshipFabricatorPointsComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(curTime < comp1_2.NextPointsAt))
      {
        comp1_2.NextPointsAt = curTime + this._gainEvery;
        ++comp1_2.Points;
        this.Dirty(uid2, (IComponent) comp1_2);
        this.SendUIStateAll(comp1_2.Points);
      }
    }
  }
}
