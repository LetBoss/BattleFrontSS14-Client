// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.SharedVendingMachineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Advertise.Components;
using Content.Shared.Advertise.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.VendingMachines;

public abstract class SharedVendingMachineSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  protected SharedPointLightSystem Light;
  [Dependency]
  private SharedPowerReceiverSystem _receiver;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedSpeakOnUIClosedSystem _speakOn;
  [Dependency]
  protected SharedUserInterfaceSystem UISystem;
  [Dependency]
  protected IRobustRandom Randomizer;
  [Dependency]
  private EmagSystem _emag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<VendingMachineComponent, ComponentGetState>(new EntityEventRefHandler<VendingMachineComponent, ComponentGetState>(this.OnVendingGetState));
    this.SubscribeLocalEvent<VendingMachineComponent, MapInitEvent>(new ComponentEventHandler<VendingMachineComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<VendingMachineComponent, GotEmaggedEvent>(new ComponentEventRefHandler<VendingMachineComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<VendingMachineRestockComponent, AfterInteractEvent>(new ComponentEventHandler<VendingMachineRestockComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.Subs.BuiEvents<VendingMachineComponent>((object) VendingMachineUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<VendingMachineComponent>) (subs => subs.Event<VendingMachineEjectMessage>(new EntityEventRefHandler<VendingMachineComponent, VendingMachineEjectMessage>(this.OnInventoryEjectMessage))));
  }

  private void OnVendingGetState(Entity<VendingMachineComponent> entity, ref ComponentGetState args)
  {
    VendingMachineComponent comp = entity.Comp;
    Dictionary<string, VendingMachineInventoryEntry> dictionary1 = new Dictionary<string, VendingMachineInventoryEntry>();
    Dictionary<string, VendingMachineInventoryEntry> dictionary2 = new Dictionary<string, VendingMachineInventoryEntry>();
    Dictionary<string, VendingMachineInventoryEntry> dictionary3 = new Dictionary<string, VendingMachineInventoryEntry>();
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in comp.Inventory)
      dictionary1[keyValuePair.Key] = new VendingMachineInventoryEntry(keyValuePair.Value);
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in comp.EmaggedInventory)
      dictionary2[keyValuePair.Key] = new VendingMachineInventoryEntry(keyValuePair.Value);
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in comp.ContrabandInventory)
      dictionary3[keyValuePair.Key] = new VendingMachineInventoryEntry(keyValuePair.Value);
    args.State = (IComponentState) new VendingMachineComponentState()
    {
      Inventory = dictionary1,
      EmaggedInventory = dictionary2,
      ContrabandInventory = dictionary3,
      Contraband = comp.Contraband,
      EjectEnd = comp.EjectEnd,
      DenyEnd = comp.DenyEnd,
      DispenseOnHitEnd = comp.DispenseOnHitEnd
    };
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<VendingMachineComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VendingMachineComponent>();
    TimeSpan curTime = this.Timing.CurTime;
    EntityUid uid;
    VendingMachineComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      TimeSpan? nullable;
      if (comp1.Ejecting)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1.EjectEnd;
        if ((nullable.HasValue ? (timeSpan > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          comp1.EjectEnd = new TimeSpan?();
          this.Dirty(uid, (IComponent) comp1);
          this.EjectItem(uid, comp1);
          this.UpdateUI((Entity<VendingMachineComponent>) (uid, comp1));
        }
      }
      if (comp1.Denying)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1.DenyEnd;
        if ((nullable.HasValue ? (timeSpan > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          comp1.DenyEnd = new TimeSpan?();
          this.Dirty(uid, (IComponent) comp1);
          this.TryUpdateVisualState((Entity<VendingMachineComponent>) (uid, comp1));
        }
      }
      if (comp1.DispenseOnHitCoolingDown)
      {
        TimeSpan timeSpan = curTime;
        nullable = comp1.DispenseOnHitEnd;
        if ((nullable.HasValue ? (timeSpan > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          comp1.DispenseOnHitEnd = new TimeSpan?();
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }

  private void OnInventoryEjectMessage(
    Entity<VendingMachineComponent> entity,
    ref VendingMachineEjectMessage args)
  {
    if (!this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) entity.Owner) || this.Deleted((EntityUid) entity))
      return;
    EntityUid actor = args.Actor;
    if (!actor.Valid)
      return;
    this.AuthorizedVend(entity.Owner, actor, args.Type, args.ID, entity.Comp);
  }

  protected virtual void OnMapInit(
    EntityUid uid,
    VendingMachineComponent component,
    MapInitEvent args)
  {
    this.RestockInventoryFromPrototype(uid, component, component.InitialStockQuality);
  }

  protected virtual void EjectItem(
    EntityUid uid,
    VendingMachineComponent? vendComponent = null,
    bool forceEject = false)
  {
  }

  public bool IsAuthorized(EntityUid uid, EntityUid sender, VendingMachineComponent? vendComponent = null)
  {
    if (!this.Resolve<VendingMachineComponent>(uid, ref vendComponent))
      return false;
    AccessReaderComponent comp;
    if (!this.TryComp<AccessReaderComponent>(uid, out comp) || this._accessReader.IsAllowed(sender, uid, comp) || this.HasComp<EmaggedComponent>(uid))
      return true;
    this.Popup.PopupClient(this.Loc.GetString("vending-machine-component-try-eject-access-denied"), uid, new EntityUid?(sender));
    this.Deny((Entity<VendingMachineComponent>) (uid, vendComponent), new EntityUid?(sender));
    return false;
  }

  protected VendingMachineInventoryEntry? GetEntry(
    EntityUid uid,
    string entryId,
    InventoryType type,
    VendingMachineComponent? component = null)
  {
    if (!this.Resolve<VendingMachineComponent>(uid, ref component))
      return (VendingMachineInventoryEntry) null;
    if (type == InventoryType.Emagged && this.HasComp<EmaggedComponent>(uid))
      return component.EmaggedInventory.GetValueOrDefault<string, VendingMachineInventoryEntry>(entryId);
    return type == InventoryType.Contraband && component.Contraband ? component.ContrabandInventory.GetValueOrDefault<string, VendingMachineInventoryEntry>(entryId) : component.Inventory.GetValueOrDefault<string, VendingMachineInventoryEntry>(entryId);
  }

  public void TryEjectVendorItem(
    EntityUid uid,
    InventoryType type,
    string itemId,
    bool throwItem,
    EntityUid? user = null,
    VendingMachineComponent? vendComponent = null)
  {
    if (!this.Resolve<VendingMachineComponent>(uid, ref vendComponent) || vendComponent.Ejecting || vendComponent.Broken || !this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) uid))
      return;
    VendingMachineInventoryEntry entry = this.GetEntry(uid, itemId, type, vendComponent);
    if (string.IsNullOrEmpty(entry?.ID))
    {
      this.Popup.PopupClient(this.Loc.GetString("vending-machine-component-try-eject-invalid-item"), new EntityUid?(uid));
      this.Deny((Entity<VendingMachineComponent>) (uid, vendComponent));
    }
    else if (entry.Amount <= 0U)
    {
      this.Popup.PopupClient(this.Loc.GetString("vending-machine-component-try-eject-out-of-stock"), new EntityUid?(uid));
      this.Deny((Entity<VendingMachineComponent>) (uid, vendComponent));
    }
    else
    {
      vendComponent.EjectEnd = new TimeSpan?(this.Timing.CurTime + vendComponent.EjectDelay);
      vendComponent.NextItemToEject = entry.ID;
      vendComponent.ThrowNextItem = throwItem;
      SpeakOnUIClosedComponent comp;
      if (this.TryComp<SpeakOnUIClosedComponent>(uid, out comp))
        this._speakOn.TrySetFlag((Entity<SpeakOnUIClosedComponent>) (uid, comp));
      --entry.Amount;
      this.Dirty(uid, (IComponent) vendComponent);
      this.UpdateUI((Entity<VendingMachineComponent>) (uid, vendComponent));
      this.TryUpdateVisualState((Entity<VendingMachineComponent>) (uid, vendComponent));
      this.Audio.PlayPredicted(vendComponent.SoundVend, uid, user);
    }
  }

  public void Deny(Entity<VendingMachineComponent?> entity, EntityUid? user = null)
  {
    if (!this.Resolve<VendingMachineComponent>(entity.Owner, ref entity.Comp) || entity.Comp.Denying)
      return;
    entity.Comp.DenyEnd = new TimeSpan?(this.Timing.CurTime + entity.Comp.DenyDelay);
    this.Audio.PlayPredicted(entity.Comp.SoundDeny, entity.Owner, user, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
    this.TryUpdateVisualState(entity);
    this.Dirty<VendingMachineComponent>(entity);
  }

  protected virtual void UpdateUI(Entity<VendingMachineComponent?> entity)
  {
  }

  public void TryUpdateVisualState(Entity<VendingMachineComponent?> entity)
  {
    if (!this.Resolve<VendingMachineComponent>(entity.Owner, ref entity.Comp))
      return;
    VendingMachineVisualState machineVisualState = VendingMachineVisualState.Normal;
    if (entity.Comp.Broken)
      machineVisualState = VendingMachineVisualState.Broken;
    else if (entity.Comp.Ejecting)
      machineVisualState = VendingMachineVisualState.Eject;
    else if (entity.Comp.Denying)
      machineVisualState = VendingMachineVisualState.Deny;
    else if (!this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) entity.Owner))
      machineVisualState = VendingMachineVisualState.Off;
    SharedPointLightComponent component;
    if (this.Light.TryGetLight(entity.Owner, out component))
    {
      bool enabled = machineVisualState != VendingMachineVisualState.Broken && machineVisualState != VendingMachineVisualState.Off;
      this.Light.SetEnabled(entity.Owner, enabled, component);
    }
    this._appearanceSystem.SetData(entity.Owner, (Enum) VendingMachineVisuals.VisualState, (object) machineVisualState);
  }

  public void AuthorizedVend(
    EntityUid uid,
    EntityUid sender,
    InventoryType type,
    string itemId,
    VendingMachineComponent component)
  {
    if (!this.IsAuthorized(uid, sender, component))
      return;
    this.TryEjectVendorItem(uid, type, itemId, component.CanShoot, new EntityUid?(sender), component);
  }

  public void RestockInventoryFromPrototype(
    EntityUid uid,
    VendingMachineComponent? component = null,
    float restockQuality = 1f)
  {
    VendingMachineInventoryPrototype prototype;
    if (!this.Resolve<VendingMachineComponent>(uid, ref component) || !this.PrototypeManager.TryIndex<VendingMachineInventoryPrototype>(component.PackPrototypeId, out prototype))
      return;
    this.AddInventoryFromPrototype(uid, prototype.StartingInventory, InventoryType.Regular, component, restockQuality);
    this.AddInventoryFromPrototype(uid, prototype.EmaggedInventory, InventoryType.Emagged, component, restockQuality);
    this.AddInventoryFromPrototype(uid, prototype.ContrabandInventory, InventoryType.Contraband, component, restockQuality);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnEmagged(
    EntityUid uid,
    VendingMachineComponent component,
    ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction))
      return;
    args.Handled = component.EmaggedInventory.Count > 0;
  }

  public List<VendingMachineInventoryEntry> GetAllInventory(
    EntityUid uid,
    VendingMachineComponent? component = null)
  {
    if (!this.Resolve<VendingMachineComponent>(uid, ref component))
      return new List<VendingMachineInventoryEntry>();
    List<VendingMachineInventoryEntry> allInventory = new List<VendingMachineInventoryEntry>((IEnumerable<VendingMachineInventoryEntry>) component.Inventory.Values);
    if (this._emag.CheckFlag(uid, EmagType.Interaction))
      allInventory.AddRange((IEnumerable<VendingMachineInventoryEntry>) component.EmaggedInventory.Values);
    if (component.Contraband)
      allInventory.AddRange((IEnumerable<VendingMachineInventoryEntry>) component.ContrabandInventory.Values);
    return allInventory;
  }

  public List<VendingMachineInventoryEntry> GetAvailableInventory(
    EntityUid uid,
    VendingMachineComponent? component = null)
  {
    return !this.Resolve<VendingMachineComponent>(uid, ref component) ? new List<VendingMachineInventoryEntry>() : this.GetAllInventory(uid, component).Where<VendingMachineInventoryEntry>((Func<VendingMachineInventoryEntry, bool>) (_ => _.Amount > 0U)).ToList<VendingMachineInventoryEntry>();
  }

  private void AddInventoryFromPrototype(
    EntityUid uid,
    Dictionary<string, uint>? entries,
    InventoryType type,
    VendingMachineComponent? component = null,
    float restockQuality = 1f)
  {
    if (!this.Resolve<VendingMachineComponent>(uid, ref component) || entries == null)
      return;
    Dictionary<string, VendingMachineInventoryEntry> dictionary;
    switch (type)
    {
      case InventoryType.Regular:
        dictionary = component.Inventory;
        break;
      case InventoryType.Emagged:
        dictionary = component.EmaggedInventory;
        break;
      case InventoryType.Contraband:
        dictionary = component.ContrabandInventory;
        break;
      default:
        return;
    }
    foreach ((string str, uint num1) in entries)
    {
      if (this.PrototypeManager.HasIndex<EntityPrototype>(str))
      {
        uint amount = num1;
        float num2 = 1f - restockQuality;
        float num3 = this.Randomizer.NextFloat(0.0f, 1f);
        if ((double) num3 < (double) num2)
          amount = (uint) Math.Floor((double) num1 * (double) num3 / (double) num2);
        VendingMachineInventoryEntry machineInventoryEntry;
        if (dictionary.TryGetValue(str, out machineInventoryEntry))
          machineInventoryEntry.Amount = Math.Min(machineInventoryEntry.Amount + num1, 3U * amount);
        else
          dictionary.Add(str, new VendingMachineInventoryEntry(type, str, amount));
      }
    }
  }

  public bool TryAccessMachine(
    EntityUid uid,
    VendingMachineRestockComponent restock,
    VendingMachineComponent machineComponent,
    EntityUid user,
    EntityUid target)
  {
    WiresPanelComponent comp;
    if (this.TryComp<WiresPanelComponent>(target, out comp) && comp.Open)
      return true;
    this.Popup.PopupPredictedCursor(this.Loc.GetString("vending-machine-restock-needs-panel-open", ("this", (object) uid), (nameof (user), (object) user), (nameof (target), (object) target)), user);
    return false;
  }

  public bool TryMatchPackageToMachine(
    EntityUid uid,
    VendingMachineRestockComponent component,
    VendingMachineComponent machineComponent,
    EntityUid user,
    EntityUid target)
  {
    if (component.CanRestock.Contains(machineComponent.PackPrototypeId))
      return true;
    this.Popup.PopupPredictedCursor(this.Loc.GetString("vending-machine-restock-invalid-inventory", ("this", (object) uid), (nameof (user), (object) user), (nameof (target), (object) target)), user);
    return false;
  }

  private void OnAfterInteract(
    EntityUid uid,
    VendingMachineRestockComponent component,
    AfterInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    VendingMachineComponent comp;
    if (!args.CanReach || args.Handled || !this.TryComp<VendingMachineComponent>(args.Target, out comp) || !this.TryMatchPackageToMachine(uid, component, comp, args.User, valueOrDefault) || !this.TryAccessMachine(uid, component, comp, args.User, valueOrDefault))
      return;
    args.Handled = true;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, (float) component.RestockDelay.TotalSeconds, (DoAfterEvent) new RestockDoAfterEvent(), new EntityUid?(valueOrDefault), new EntityUid?(valueOrDefault), new EntityUid?(uid))
    {
      BreakOnMove = true,
      BreakOnDamage = true,
      NeedHand = true
    }))
      return;
    this.Popup.PopupPredicted(this.Loc.GetString("vending-machine-restock-start-self", ("target", (object) valueOrDefault)), this.Loc.GetString("vending-machine-restock-start-others", ("user", (object) Identity.Entity(args.User, (IEntityManager) this.EntityManager)), ("target", (object) valueOrDefault)), uid, new EntityUid?(args.User), PopupType.Medium);
    this.Audio.PlayPredicted(component.SoundRestockStart, uid, new EntityUid?(args.User));
  }
}
