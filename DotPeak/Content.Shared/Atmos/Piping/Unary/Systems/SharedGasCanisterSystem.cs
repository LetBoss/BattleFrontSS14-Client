// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Unary.Systems.SharedGasCanisterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.NodeContainer;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Piping.Unary.Systems;

public abstract class SharedGasCanisterSystem : EntitySystem
{
  [Dependency]
  protected ISharedAdminLogManager AdminLogger;
  [Dependency]
  private ItemSlotsSystem _slots;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  protected SharedUserInterfaceSystem UI;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<GasCanisterComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnCanisterContainerModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<GasCanisterComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnCanisterContainerModified)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, ItemSlotInsertAttemptEvent>(new ComponentEventRefHandler<GasCanisterComponent, ItemSlotInsertAttemptEvent>((object) this, __methodptr(OnCanisterInsertAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, ComponentStartup>(new EntityEventRefHandler<GasCanisterComponent, ComponentStartup>((object) this, __methodptr(OnCanisterStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>((object) this, __methodptr(OnHoldingTankEjectMessage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>((object) this, __methodptr(OnCanisterChangeReleasePressure)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>((object) this, __methodptr(OnCanisterChangeReleaseValve)), (Type[]) null, (Type[]) null);
  }

  private void OnCanisterStartup(Entity<GasCanisterComponent> ent, ref ComponentStartup args)
  {
    this._slots.AddItemSlot(ent.Owner, ent.Comp.ContainerName, ent.Comp.GasTankSlot);
  }

  private void OnCanisterContainerModified(
    EntityUid uid,
    GasCanisterComponent component,
    ContainerModifiedMessage args)
  {
    if (args.Container.ID != component.ContainerName)
      return;
    this.DirtyUI(uid, component);
    this._appearance.SetData(uid, (Enum) GasCanisterVisuals.TankInserted, (object) (args is EntInsertedIntoContainerMessage), (AppearanceComponent) null);
  }

  private static string GetContainedGasesString(Entity<GasCanisterComponent> canister)
  {
    return string.Join<(Gas, float)>(", ", (IEnumerable<(Gas, float)>) canister.Comp.Air);
  }

  private void OnHoldingTankEjectMessage(
    EntityUid uid,
    GasCanisterComponent canister,
    GasCanisterHoldingTankEjectMessage args)
  {
    if (!canister.GasTankSlot.Item.HasValue)
      return;
    EntityUid? nullable = canister.GasTankSlot.Item;
    this._slots.TryEjectToHands(uid, canister.GasTankSlot, new EntityUid?(((BaseBoundUserInterfaceEvent) args).Actor), true);
    if (canister.ReleaseValve)
    {
      ISharedAdminLogManager adminLogger = this.AdminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(80 /*0x50*/, 4);
      logStringHandler.AppendLiteral("Player ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" ejected tank ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(nullable, (MetaDataComponent) null), "tank", "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), nameof (canister), "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" while the valve was open, releasing [");
      logStringHandler.AppendFormatted(SharedGasCanisterSystem.GetContainedGasesString(Entity<GasCanisterComponent>.op_Implicit((uid, canister))));
      logStringHandler.AppendLiteral("] to atmosphere");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.CanisterTankEjected, LogImpact.High, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this.AdminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(27, 3);
      logStringHandler.AppendLiteral("Player ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" ejected tank ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(nullable, (MetaDataComponent) null), "tank", "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), nameof (canister), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.CanisterTankEjected, LogImpact.Medium, ref local);
    }
    GasCanisterBoundUserInterfaceState userInterfaceState1;
    if (this.UI.TryGetUiState<GasCanisterBoundUserInterfaceState>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) GasCanisterUiKey.Key, ref userInterfaceState1))
    {
      GasCanisterBoundUserInterfaceState userInterfaceState2 = new GasCanisterBoundUserInterfaceState(userInterfaceState1.CanisterPressure, userInterfaceState1.PortStatus, 0.0f);
      this.UI.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) GasCanisterUiKey.Key, (BoundUserInterfaceState) userInterfaceState2);
    }
    this.DirtyUI(uid, canister);
  }

  private void OnCanisterChangeReleasePressure(
    EntityUid uid,
    GasCanisterComponent canister,
    GasCanisterChangeReleasePressureMessage args)
  {
    float num = Math.Clamp(args.Pressure, canister.MinReleasePressure, canister.MaxReleasePressure);
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(33, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the release pressure on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), nameof (canister), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<float>(args.Pressure, "args.Pressure");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.CanisterPressure, LogImpact.Medium, ref local);
    canister.ReleasePressure = num;
    this.Dirty(uid, (IComponent) canister, (MetaDataComponent) null);
    this.DirtyUI(uid, canister);
  }

  private void OnCanisterChangeReleaseValve(
    EntityUid uid,
    GasCanisterComponent canister,
    GasCanisterChangeReleaseValveMessage args)
  {
    LogImpact logImpact = canister.GasTankSlot.HasItem ? LogImpact.Medium : LogImpact.High;
    Dictionary<Gas, float> values1 = new Dictionary<Gas, float>();
    Array values2 = Enum.GetValues(typeof (Gas));
    for (int index = 0; index < values2.Length; ++index)
      values1.Add((Gas) index, canister.Air[index]);
    ISharedAdminLogManager adminLogger = this.AdminLogger;
    int impact = (int) logImpact;
    LogStringHandler logStringHandler = new LogStringHandler(44, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent) args).Actor)), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set the valve on ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), nameof (canister), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<bool>(args.Valve, "valveState", "args.Valve");
    logStringHandler.AppendLiteral(" while it contained [");
    logStringHandler.AppendFormatted(string.Join<KeyValuePair<Gas, float>>(", ", (IEnumerable<KeyValuePair<Gas, float>>) values1));
    logStringHandler.AppendLiteral("]");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.CanisterValve, (LogImpact) impact, ref local);
    canister.ReleaseValve = args.Valve;
    this.Dirty(uid, (IComponent) canister, (MetaDataComponent) null);
    this.DirtyUI(uid, canister);
  }

  private void OnCanisterInsertAttempt(
    EntityUid uid,
    GasCanisterComponent component,
    ref ItemSlotInsertAttemptEvent args)
  {
    GasTankComponent gasTankComponent;
    if (args.Slot.ID != component.ContainerName || !args.User.HasValue || this.TryComp<GasTankComponent>(args.Item, ref gasTankComponent) && !gasTankComponent.IsValveOpen)
      return;
    args.Cancelled = true;
  }

  protected abstract void DirtyUI(
    EntityUid uid,
    GasCanisterComponent? component = null,
    NodeContainerComponent? nodes = null);
}
