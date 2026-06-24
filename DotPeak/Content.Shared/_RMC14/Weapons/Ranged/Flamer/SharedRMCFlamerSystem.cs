// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.SharedRMCFlamerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Fluids;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Temperature;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

public abstract class SharedRMCFlamerSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCFlammableSystem _rmcFlammable;
  [Dependency]
  private SharedRMCSpraySystem _rmcSpray;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private SolutionTransferSystem _solutionTransfer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCReagentSystem _reagent;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, MapInitEvent>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, MapInitEvent>(this.OnMapInit), after: new Type[1]
    {
      typeof (SharedSolutionContainerSystem)
    });
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, TakeAmmoEvent>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, TakeAmmoEvent>(this.OnTakeAmmo));
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, GetAmmoCountEvent>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, GetAmmoCountEvent>(this.OnGetAmmoCount));
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnInsertedIntoContainer));
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnRemovedFromContainer));
    this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, AttemptShootEvent>(new EntityEventRefHandler<RMCFlamerAmmoProviderComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<RMCFlamerTankComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<RMCFlamerTankComponent, BeforeRangedInteractEvent>(this.OnFlamerTankBeforeRangedInteract));
    this.SubscribeLocalEvent<RMCFlamerTankComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<RMCFlamerTankComponent, GetVerbsEvent<ExamineVerb>>(this.OnFlamerTankVerbExamine));
    this.SubscribeLocalEvent<RMCSprayAmmoProviderComponent, TakeAmmoEvent>(new EntityEventRefHandler<RMCSprayAmmoProviderComponent, TakeAmmoEvent>(this.OnSprayTakeAmmo));
    this.SubscribeLocalEvent<RMCSprayAmmoProviderComponent, GetAmmoCountEvent>(new EntityEventRefHandler<RMCSprayAmmoProviderComponent, GetAmmoCountEvent>(this.OnSprayGetAmmoCount));
    this.SubscribeLocalEvent<RMCIgniterComponent, MapInitEvent>(new EntityEventRefHandler<RMCIgniterComponent, MapInitEvent>(this.OnIgniterMapInit), after: new Type[1]
    {
      typeof (SharedSolutionContainerSystem)
    });
    this.SubscribeLocalEvent<RMCIgniterComponent, UniqueActionEvent>(new EntityEventRefHandler<RMCIgniterComponent, UniqueActionEvent>(this.OnIgniterUniqueAction));
    this.SubscribeLocalEvent<RMCIgniterComponent, IsHotEvent>(new EntityEventRefHandler<RMCIgniterComponent, IsHotEvent>(this.OnIgniterToggle));
    this.SubscribeLocalEvent<RMCIgniterComponent, AttemptShootEvent>(new EntityEventRefHandler<RMCIgniterComponent, AttemptShootEvent>(this.OnIgniterAttemptShoot));
    this.SubscribeLocalEvent<RMCIgniterComponent, ExaminedEvent>(new EntityEventRefHandler<RMCIgniterComponent, ExaminedEvent>(this.OnIgniterUniqueActionExamine), new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<RMCBroilerComponent, GetItemActionsEvent>(new EntityEventRefHandler<RMCBroilerComponent, GetItemActionsEvent>(this.OnBroilerGetItemActions));
    this.SubscribeLocalEvent<RMCBroilerComponent, RMCBroilerActionEvent>(new EntityEventRefHandler<RMCBroilerComponent, RMCBroilerActionEvent>(this.OnBroilerAction));
    this.SubscribeLocalEvent<RMCCanUseBroilerComponent, UniqueActionEvent>(new EntityEventRefHandler<RMCCanUseBroilerComponent, UniqueActionEvent>(this.OnBroilerUniqueAction));
    this.SubscribeLocalEvent<RMCCanUseBroilerComponent, ExaminedEvent>(new EntityEventRefHandler<RMCCanUseBroilerComponent, ExaminedEvent>(this.OnBroilerUniqueActionExamine), new Type[1]
    {
      typeof (SharedGunSystem)
    });
  }

  private void OnMapInit(Entity<RMCFlamerAmmoProviderComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnTakeAmmo(Entity<RMCFlamerAmmoProviderComponent> ent, ref TakeAmmoEvent args)
  {
    args.Ammo.Add((new EntityUid?((EntityUid) ent), (IShootable) ent.Comp));
  }

  private void OnGetAmmoCount(
    Entity<RMCFlamerAmmoProviderComponent> ent,
    ref GetAmmoCountEvent args)
  {
    Entity<SolutionComponent>? solutionEnt;
    if (!this.TryGetTankSolution(ent, out solutionEnt, out Entity<RMCFlamerTankComponent>? _))
      return;
    Solution solution = solutionEnt.Value.Comp.Solution;
    args.Count = solution.Volume.Int();
    args.Capacity = solution.MaxVolume.Int();
  }

  private void OnInsertedIntoContainer(
    Entity<RMCFlamerAmmoProviderComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.ContainerId)
      return;
    this.UpdateAppearance(ent);
  }

  private void OnRemovedFromContainer(
    Entity<RMCFlamerAmmoProviderComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.ContainerId)
      return;
    this.UpdateAppearance(ent);
  }

  private void OnAttemptShoot(
    Entity<RMCFlamerAmmoProviderComponent> ent,
    ref AttemptShootEvent args)
  {
    EntityCoordinates? toCoordinates = args.ToCoordinates;
    if (!toCoordinates.HasValue)
      return;
    EntityCoordinates valueOrDefault = toCoordinates.GetValueOrDefault();
    if (this.CanShootFlamer(ent, args.FromCoordinates, valueOrDefault, out List<LineTile> _, out Entity<SolutionComponent> _, out ReagentPrototype _, out Entity<RMCFlamerTankComponent>? _))
      return;
    args.Cancelled = true;
    args.ResetCooldown = true;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < ent.Comp.CantShootPopupLast + ent.Comp.CantShootPopupCooldown)
      return;
    ent.Comp.CantShootPopupLast = curTime;
    this.Dirty<RMCFlamerAmmoProviderComponent>(ent);
    args.Message = this.Loc.GetString("rmc-flamer-too-close");
  }

  private void OnFlamerTankBeforeRangedInteract(
    Entity<RMCFlamerTankComponent> tank,
    ref BeforeRangedInteractEvent args)
  {
    if (!this.HasComp<RMCFlamerAmmoProviderComponent>((EntityUid) tank))
    {
      this.RefillTank(tank, ref args);
    }
    else
    {
      EntityUid? target = args.Target;
      if (!target.HasValue)
        return;
      EntityUid valueOrDefault = target.GetValueOrDefault();
      Entity<SolutionComponent>? entity1;
      Solution solution;
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) tank.Owner, tank.Comp.SolutionId, out entity1, out solution))
        return;
      Entity<SolutionComponent>? soln1;
      Entity<SolutionComponent> sourceSolutionEnt;
      if (this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) valueOrDefault, out soln1, out solution))
      {
        sourceSolutionEnt = soln1.Value;
      }
      else
      {
        RMCFlamerTankComponent comp1;
        Entity<SolutionComponent>? entity2;
        if (this.TryComp<RMCFlamerTankComponent>(valueOrDefault, out comp1) && this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) valueOrDefault, comp1.SolutionId, out entity2))
        {
          sourceSolutionEnt = entity2.Value;
        }
        else
        {
          RMCFlamerBackpackComponent comp2;
          Entity<SolutionComponent>? entity3;
          if (this.TryComp<RMCFlamerBackpackComponent>(valueOrDefault, out comp2) && this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) valueOrDefault, comp2.SolutionId, out entity3))
          {
            sourceSolutionEnt = entity3.Value;
          }
          else
          {
            Entity<SolutionComponent>? soln2;
            if (!this.HasComp<ReagentTankComponent>(valueOrDefault) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) valueOrDefault, out soln2, out solution))
              return;
            sourceSolutionEnt = soln2.Value;
          }
        }
      }
      args.Handled = true;
      this.Transfer(valueOrDefault, sourceSolutionEnt, tank, entity1.Value, args.User);
    }
  }

  private void OnFlamerTankVerbExamine(
    Entity<RMCFlamerTankComponent> tank,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    EntityUid user = args.User;
    if (!args.CanInteract || !args.CanAccess || this.HasComp<XenoComponent>(user))
      return;
    FormattedMessage message = new FormattedMessage();
    // ISSUE: object of a compiler-generated type is created
    List<int> intList = new List<int>((IEnumerable<int>) new \u003C\u003Ez__ReadOnlyArray<int>(new int[3]
    {
      tank.Comp.MaxIntensity,
      tank.Comp.MaxDuration,
      tank.Comp.MaxRange
    }));
    for (int index = 0; index < intList.Count; ++index)
    {
      message.AddMarkupPermissive(this.Loc.GetString("rmc-flamer-tank-examine-line-" + index.ToString(), ("value", (object) intList[index])));
      if (index + 1 != intList.Count)
        message.PushNewline();
    }
    this._examine.AddDetailedExamineVerb(args, (Component) (RMCFlamerTankComponent) tank, message, this.Loc.GetString("rmc-flamer-tank-examine-short"), tank.Comp.ExamineIcon, this.Loc.GetString("rmc-flamer-tank-examine"));
  }

  private void OnSprayTakeAmmo(Entity<RMCSprayAmmoProviderComponent> ent, ref TakeAmmoEvent args)
  {
    args.Ammo.Add((new EntityUid?((EntityUid) ent), (IShootable) ent.Comp));
  }

  private void OnSprayGetAmmoCount(
    Entity<RMCSprayAmmoProviderComponent> ent,
    ref GetAmmoCountEvent args)
  {
    Entity<SolutionComponent>? entity;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.SolutionId, out entity, out Solution _))
      return;
    Solution solution = entity.Value.Comp.Solution;
    args.Count = solution.Volume.Int();
    args.Capacity = solution.MaxVolume.Int();
  }

  private void OnIgniterMapInit(Entity<RMCIgniterComponent> ent, ref MapInitEvent args)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) RMCIgniterVisuals.Ignited, (object) ent.Comp.Enabled);
  }

  private void OnIgniterUniqueAction(Entity<RMCIgniterComponent> ent, ref UniqueActionEvent args)
  {
    if (args.Handled || ent.Comp.Locked)
      return;
    args.Handled = true;
    ent.Comp.Enabled = !ent.Comp.Enabled;
    this.Dirty<RMCIgniterComponent>(ent);
    this._audio.PlayPredicted((SoundSpecifier) ent.Comp.Sound, (EntityUid) ent, new EntityUid?(args.UserUid));
    this._appearance.SetData((EntityUid) ent, (Enum) RMCIgniterVisuals.Ignited, (object) ent.Comp.Enabled);
  }

  private void OnIgniterToggle(Entity<RMCIgniterComponent> ent, ref IsHotEvent args)
  {
    args.IsHot = ent.Comp.Enabled;
  }

  protected virtual void OnIgniterAttemptShoot(
    Entity<RMCIgniterComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || ent.Comp.Enabled)
      return;
    args.Cancelled = true;
  }

  private void OnIgniterUniqueActionExamine(Entity<RMCIgniterComponent> ent, ref ExaminedEvent args)
  {
    if (ent.Comp.Locked)
      return;
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.ExamineText), 1);
  }

  private void UpdateAppearance(Entity<RMCFlamerAmmoProviderComponent> ent)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    FixedPoint2 fixedPoint2_1 = FixedPoint2.Zero;
    FixedPoint2 fixedPoint2_2 = FixedPoint2.Zero;
    bool flag = false;
    Entity<SolutionComponent>? solutionEnt;
    if (this.TryGetTankSolution(ent, out solutionEnt, out Entity<RMCFlamerTankComponent>? _, true))
    {
      Solution solution = solutionEnt.Value.Comp.Solution;
      fixedPoint2_1 = solution.Volume;
      fixedPoint2_2 = solution.MaxVolume;
      flag = true;
    }
    this._appearance.SetData((EntityUid) ent, (Enum) AmmoVisuals.HasAmmo, (object) (fixedPoint2_1 > FixedPoint2.Zero), comp);
    this._appearance.SetData((EntityUid) ent, (Enum) AmmoVisuals.AmmoCount, (object) fixedPoint2_1.Int(), comp);
    this._appearance.SetData((EntityUid) ent, (Enum) AmmoVisuals.AmmoMax, (object) fixedPoint2_2.Int(), comp);
    this._appearance.SetData((EntityUid) ent, (Enum) AmmoVisuals.MagLoaded, (object) flag, comp);
    this._appearance.SetData((EntityUid) ent, (Enum) RMCFlamerVisualLayers.Strip, (object) flag, comp);
  }

  public void ShootFlamer(
    Entity<RMCFlamerAmmoProviderComponent> flamer,
    Entity<GunComponent> gun,
    EntityUid? user,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates)
  {
    List<LineTile> tiles;
    Entity<SolutionComponent> solution;
    ReagentPrototype reagent;
    Entity<RMCFlamerTankComponent>? tank;
    if (!this.CanShootFlamer(flamer, fromCoordinates, toCoordinates, out tiles, out solution, out reagent, out tank))
      return;
    this._audio.PlayPredicted(gun.Comp.SoundGunshotModified, (EntityUid) gun, user);
    int num = tiles.Count;
    if (reagent.FireSpread && num > 2)
      num = (int) Math.Ceiling((double) num / 3.0);
    solution.Comp.Solution.RemoveSolution(flamer.Comp.CostPer * num);
    this._solution.UpdateChemicals(solution);
    if (this._net.IsClient)
      return;
    EntityUid uid = this.Spawn();
    RMCFlamerChainComponent flamerChainComponent = this.EnsureComp<RMCFlamerChainComponent>(uid);
    flamerChainComponent.Spawn = reagent.FireEntity;
    flamerChainComponent.Tiles = tiles;
    flamerChainComponent.Reagent = (ProtoId<ReagentPrototype>) reagent.ID;
    flamerChainComponent.MaxIntensity = tank.Value.Comp.MaxIntensity;
    flamerChainComponent.MaxDuration = tank.Value.Comp.MaxDuration;
    this.Dirty(uid, (IComponent) flamerChainComponent);
  }

  private bool CanShootFlamer(
    Entity<RMCFlamerAmmoProviderComponent> flamer,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates,
    [NotNullWhen(true)] out List<LineTile>? tiles,
    out Entity<SolutionComponent> solution,
    [NotNullWhen(true)] out ReagentPrototype? reagent,
    [NotNullWhen(true)] out Entity<RMCFlamerTankComponent>? tank)
  {
    tiles = (List<LineTile>) null;
    solution = new Entity<SolutionComponent>();
    reagent = (ReagentPrototype) null;
    Entity<SolutionComponent>? solutionEnt;
    if (!this.TryGetTankSolution(flamer, out solutionEnt, out tank))
      return false;
    FixedPoint2 volume = solutionEnt.Value.Comp.Solution.Volume;
    Vector2 delta;
    if (volume <= flamer.Comp.CostPer || !fromCoordinates.TryDelta((IEntityManager) this.EntityManager, this._transform, toCoordinates, out delta) || Vector2Helpers.IsLengthZero(delta))
      return false;
    Vector2 vector2 = -Vector2Helpers.Normalized(delta);
    fromCoordinates = fromCoordinates.Offset(vector2 * 0.23f);
    ReagentQuantity? element;
    if (!solutionEnt.Value.Comp.Solution.TryFirstOrNull<ReagentQuantity>(out element))
      return false;
    reagent = (ReagentPrototype) this._reagent.Index((ProtoId<ReagentPrototype>) element.Value.Reagent.Prototype);
    int val2 = Math.Min(tank.Value.Comp.MaxRange, reagent.Radius);
    int num = Math.Min((volume / flamer.Comp.CostPer).Int(), val2);
    if ((double) delta.Length() > (double) val2)
      toCoordinates = fromCoordinates.Offset(vector2 * (float) num);
    tiles = this._line.DrawLine(fromCoordinates, toCoordinates, flamer.Comp.DelayPer, new float?((float) val2), out EntityUid? _, true, reagent.FireSpread);
    if (tiles.Count == 0)
    {
      tiles = (List<LineTile>) null;
      return false;
    }
    solution = solutionEnt.Value;
    return true;
  }

  public void ShootSpray(
    Entity<RMCSprayAmmoProviderComponent> spray,
    Entity<GunComponent> gun,
    EntityUid? user,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates)
  {
    if (!user.HasValue)
      return;
    this._rmcSpray.Spray((EntityUid) spray, user.Value, this._transform.ToMapCoordinates(toCoordinates));
  }

  private bool TryGetTankSolution(
    Entity<RMCFlamerAmmoProviderComponent> flamer,
    [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEnt,
    [NotNullWhen(true)] out Entity<RMCFlamerTankComponent>? tankEnt,
    bool display = false)
  {
    solutionEnt = new Entity<SolutionComponent>?();
    tankEnt = new Entity<RMCFlamerTankComponent>?();
    RMCFlamerTankComponent comp1;
    if (this.TryComp<RMCFlamerTankComponent>((EntityUid) flamer, out comp1))
    {
      tankEnt = new Entity<RMCFlamerTankComponent>?((Entity<RMCFlamerTankComponent>) ((EntityUid) flamer, comp1));
    }
    else
    {
      BaseContainer container1;
      EntityUid? element;
      if (this._container.TryGetContainer((EntityUid) flamer, flamer.Comp.ContainerId, out container1) && container1.ContainedEntities.TryFirstOrNull<EntityUid>(out element) && this.TryComp<RMCFlamerTankComponent>(element, out comp1))
        tankEnt = new Entity<RMCFlamerTankComponent>?((Entity<RMCFlamerTankComponent>) (element.Value, comp1));
      else if (!display && this.HasComp<RMCCanUseBroilerComponent>((EntityUid) flamer))
      {
        BaseContainer container2;
        if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (flamer.Owner, (TransformComponent) null), out container2))
          return false;
        InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) container2.Owner);
        ContainerSlot container3;
        while (slotEnumerator.MoveNext(out container3))
        {
          RMCBroilerComponent comp2;
          if (this.TryComp<RMCBroilerComponent>(container3.ContainedEntity, out comp2))
          {
            Entity<RMCBroilerComponent> entity = (Entity<RMCBroilerComponent>) (container3.ContainedEntity.Value, comp2);
            List<string> stringList = this.BroilerListTanks(entity);
            if (stringList.Count > comp2.ActiveTank)
            {
              string id = stringList[comp2.ActiveTank];
              BaseContainer container4;
              if (this._container.TryGetContainer((EntityUid) entity, id, out container4) && container4.ContainedEntities.TryFirstOrNull<EntityUid>(out element) && this.TryComp<RMCFlamerTankComponent>(element, out comp1))
              {
                tankEnt = new Entity<RMCFlamerTankComponent>?((Entity<RMCFlamerTankComponent>) (element.Value, comp1));
                break;
              }
            }
          }
        }
      }
    }
    Entity<RMCFlamerTankComponent>? nullable = tankEnt;
    if (!nullable.HasValue)
      return false;
    Entity<RMCFlamerTankComponent> valueOrDefault = nullable.GetValueOrDefault();
    return this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) valueOrDefault.Owner, valueOrDefault.Comp.SolutionId, out solutionEnt, out Solution _);
  }

  private void Transfer(
    EntityUid source,
    Entity<SolutionComponent> sourceSolutionEnt,
    Entity<RMCFlamerTankComponent> target,
    Entity<SolutionComponent> targetSolutionEnt,
    EntityUid user)
  {
    Solution solution = targetSolutionEnt.Comp.Solution;
    foreach (ReagentQuantity content in sourceSolutionEnt.Comp.Solution.Contents)
    {
      List<ProtoId<ReagentPrototype>> reagentWhitelist = target.Comp.ReagentWhitelist;
      ReagentId reagent1;
      if (reagentWhitelist != null)
      {
        List<ProtoId<ReagentPrototype>> protoIdList = reagentWhitelist;
        reagent1 = content.Reagent;
        ProtoId<ReagentPrototype> prototype = (ProtoId<ReagentPrototype>) reagent1.Prototype;
        if (!protoIdList.Contains(prototype))
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-flamer-tank-not-whitelisted", ("tank", (object) target)), source, new EntityUid?(user));
          return;
        }
      }
      RMCReagentSystem reagent2 = this._reagent;
      reagent1 = content.Reagent;
      ProtoId<ReagentPrototype> prototype1 = (ProtoId<ReagentPrototype>) reagent1.Prototype;
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent3;
      ref Content.Shared._RMC14.Chemistry.Reagent.Reagent local = ref reagent3;
      if (reagent2.TryIndex(prototype1, out local) && (reagent3.Intensity <= 0 || reagent3.Duration <= 0 || reagent3.Radius <= 0))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-flamer-tank-not-potent-enough"), source, new EntityUid?(user));
        return;
      }
    }
    if (!(this._solutionTransfer.Transfer(new EntityUid?(user), source, sourceSolutionEnt, (EntityUid) target, targetSolutionEnt, solution.AvailableVolume) > FixedPoint2.Zero))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-flamer-refill", ("refilled", (object) target)), source, new EntityUid?(user));
  }

  private void RefillTank(Entity<RMCFlamerTankComponent> tank, ref BeforeRangedInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    Entity<SolutionComponent>? entity;
    Solution solution;
    Entity<SolutionComponent>? soln;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) tank.Owner, tank.Comp.SolutionId, out entity, out solution) || !this.HasComp<ReagentTankComponent>(valueOrDefault) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) valueOrDefault, out soln, out solution))
      return;
    Entity<SolutionComponent> sourceSolutionEnt = soln.Value;
    args.Handled = true;
    this.Transfer(valueOrDefault, sourceSolutionEnt, tank, entity.Value, args.User);
  }

  private void OnBroilerGetItemActions(
    Entity<RMCBroilerComponent> ent,
    ref GetItemActionsEvent args)
  {
    if (!args.SlotFlags.HasValue)
      return;
    SlotFlags? slotFlags1 = args.SlotFlags;
    SlotFlags slot = ent.Comp.Slot;
    SlotFlags? nullable = slotFlags1.HasValue ? new SlotFlags?(slotFlags1.GetValueOrDefault() & slot) : new SlotFlags?();
    SlotFlags slotFlags2 = SlotFlags.NONE;
    if (nullable.GetValueOrDefault() == slotFlags2 & nullable.HasValue)
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId, (EntityUid) ent);
    EntityUid? action = ent.Comp.Action;
    if (!action.HasValue)
      return;
    EntityUid valueOrDefault = action.GetValueOrDefault();
    int num = ent.Comp.ActiveTank + 1;
    this._action.SetIcon((Entity<ActionComponent>) valueOrDefault, (SpriteSpecifier) new SpriteSpecifier.Rsi(ent.Comp.NumberingResource, num.ToString()));
  }

  private List<string> BroilerListTanks(Entity<RMCBroilerComponent> ent)
  {
    List<string> stringList = new List<string>();
    foreach (BaseContainer allContainer in this._container.GetAllContainers((EntityUid) ent))
    {
      string id = allContainer.ID;
      if (id.StartsWith(ent.Comp.ContainerPrefix))
        stringList.Add(id);
    }
    return stringList;
  }

  private void OnBroilerAction(Entity<RMCBroilerComponent> ent, ref RMCBroilerActionEvent args)
  {
    args.Handled = true;
    ent.Comp.ActiveTank = (ent.Comp.ActiveTank + 1) % this.BroilerListTanks(ent).Count;
    this.Dirty<RMCBroilerComponent>(ent);
    int num = ent.Comp.ActiveTank + 1;
    EntityUid? action = ent.Comp.Action;
    if (action.HasValue)
      this._action.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) new SpriteSpecifier.Rsi(ent.Comp.NumberingResource, num.ToString()));
    this._popup.PopupClient(this.Loc.GetString("rmc-broiler-switch-tank", ("n", (object) num)), (EntityUid) ent, new EntityUid?(args.Performer));
  }

  public void OnBroilerUniqueAction(
    Entity<RMCCanUseBroilerComponent> ent,
    ref UniqueActionEvent args)
  {
    if (args.Handled)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.UserUid);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      if (this.TryComp<RMCBroilerComponent>(container.ContainedEntity, out RMCBroilerComponent _))
      {
        args.Handled = true;
        RMCBroilerActionEvent args1 = new RMCBroilerActionEvent();
        args1.Performer = args.UserUid;
        this.RaiseLocalEvent<RMCBroilerActionEvent>(container.ContainedEntity.Value, args1);
        break;
      }
    }
  }

  public void OnBroilerUniqueActionExamine(
    Entity<RMCCanUseBroilerComponent> ent,
    ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.ExamineText), 1);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCFlamerChainComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCFlamerChainComponent>();
    EntityUid uid;
    RMCFlamerChainComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Tiles.Count == 0)
      {
        this.QueueDel(new EntityUid?(uid));
      }
      else
      {
        foreach (LineTile tile in comp1.Tiles)
        {
          if (curTime >= tile.At)
          {
            comp1.Tiles.Remove(tile);
            EntityUid ent1 = this.Spawn((string) comp1.Spawn, tile.Coordinates, rotation: new Angle());
            Entity<TileFireComponent> ent2;
            if (this._rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(this._transform.ToCoordinates((Entity<TransformComponent>) ent1, tile.Coordinates), out ent2, facing: (DirectionFlag) 0) && ent2.Owner.Id != ent1.Id)
              this.QueueDel(new EntityUid?((EntityUid) ent2));
            Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
            if (this._reagent.TryIndex(comp1.Reagent, out reagent))
            {
              int num1 = Math.Min(comp1.MaxIntensity, reagent.Intensity);
              int num2 = Math.Min(comp1.MaxDuration, reagent.Duration);
              this._rmcFlammable.SetIntensityDuration((Entity<RMCIgniteOnCollideComponent, DamageOnCollideComponent>) ent1, new int?(num1), new int?(num2));
              break;
            }
            break;
          }
        }
      }
    }
  }
}
