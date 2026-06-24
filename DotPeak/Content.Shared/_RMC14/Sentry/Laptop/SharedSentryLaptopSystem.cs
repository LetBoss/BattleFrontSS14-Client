// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.Laptop.SharedSentryLaptopSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Damage;
using Content.Shared.DeviceLinking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Sentry.Laptop;

public abstract class SharedSentryLaptopSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedSentryTargetingSystem _sentryTargeting;
  [Dependency]
  private GunIFFSystem _iff;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedDeviceLinkSystem _deviceLink;
  private const float UpdateInterval = 1f;
  private float _updateTimer;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SentryLaptopComponent, AfterInteractEvent>(new EntityEventRefHandler<SentryLaptopComponent, AfterInteractEvent>(this.OnLaptopAfterInteract));
    this.SubscribeLocalEvent<SentryLaptopComponent, ComponentShutdown>(new EntityEventRefHandler<SentryLaptopComponent, ComponentShutdown>(this.OnLaptopShutdown));
    this.SubscribeLocalEvent<SentryLaptopComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<SentryLaptopComponent, ActivatableUIOpenAttemptEvent>(this.OnLaptopUIOpenAttempt));
    this.SubscribeLocalEvent<SentryLaptopComponent, EntParentChangedMessage>(new EntityEventRefHandler<SentryLaptopComponent, EntParentChangedMessage>(this.OnLaptopParentChanged));
    this.SubscribeLocalEvent<SentryLaptopLinkedComponent, ComponentShutdown>(new EntityEventRefHandler<SentryLaptopLinkedComponent, ComponentShutdown>(this.OnSentryLinkedShutdown));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopUnlinkBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopUnlinkBuiMsg>(this.OnUnlinkMessage));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopUnlinkAllBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopUnlinkAllBuiMsg>(this.OnUnlinkAllMessage));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopSetFactionsBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopSetFactionsBuiMsg>(this.OnSetFactionsMessage));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopToggleFactionBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopToggleFactionBuiMsg>(this.OnToggleFactionMessage));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopResetTargetingBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopResetTargetingBuiMsg>(this.OnResetTargetingMessage));
    this.SubscribeLocalEvent<SentryLaptopComponent, SentryLaptopTogglePowerBuiMsg>(new EntityEventRefHandler<SentryLaptopComponent, SentryLaptopTogglePowerBuiMsg>(this.OnTogglePowerMessage));
    this.SubscribeLocalEvent<SentryComponent, DamageChangedEvent>(new EntityEventRefHandler<SentryComponent, DamageChangedEvent>(this.OnSentryDamageChanged));
    this.SubscribeLocalEvent<SentryComponent, GunShotEvent>(new EntityEventRefHandler<SentryComponent, GunShotEvent>(this.OnSentryShot));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._net.IsServer)
      return;
    this._updateTimer += frameTime;
    if ((double) this._updateTimer < 1.0)
      return;
    this._updateTimer = 0.0f;
    this.UpdateAllOpenUIs();
    this.CheckSentryAlerts();
  }

  private void UpdateAllOpenUIs()
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SentryLaptopComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SentryLaptopComponent>();
    EntityUid uid;
    SentryLaptopComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (this._ui.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) SentryLaptopUiKey.Key))
        this.UpdateUI((Entity<SentryLaptopComponent>) (uid, comp1));
    }
  }

  private void CheckSentryAlerts()
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SentryLaptopComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SentryLaptopComponent>();
    EntityUid uid;
    SentryLaptopComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.IsPowered && this._ui.IsUiOpen((Entity<UserInterfaceComponent>) uid, (Enum) SentryLaptopUiKey.Key))
      {
        foreach (EntityUid linkedSentry in this.GetLinkedSentries((Entity<SentryLaptopComponent>) (uid, comp1)))
        {
          SentryComponent comp;
          if (this.TryComp<SentryComponent>(linkedSentry, out comp))
          {
            this.CheckLowAmmoAlert(uid, (Entity<SentryComponent>) (linkedSentry, comp), comp1, curTime);
            this.CheckHealthAlert(uid, (Entity<SentryComponent>) (linkedSentry, comp), comp1, curTime);
          }
        }
      }
    }
  }

  private void CheckLowAmmoAlert(
    EntityUid laptopUid,
    Entity<SentryComponent> sentry,
    SentryLaptopComponent laptop,
    TimeSpan time)
  {
    int maxAmmo;
    int sentryAmmo = this.GetSentryAmmo((EntityUid) sentry, out maxAmmo);
    if (maxAmmo <= 0 || (double) sentryAmmo / (double) maxAmmo > (double) sentry.Comp.LowAmmoThreshold || !(time - sentry.Comp.LastLowAmmoAlert > sentry.Comp.AlertCooldown))
      return;
    sentry.Comp.LastLowAmmoAlert = time;
    this.Dirty<SentryComponent>(sentry);
    this.SendAlert(laptopUid, (EntityUid) sentry, SentryAlertType.LowAmmo, $"{this.GetSentryDisplayName((Entity<SentryLaptopComponent>) (laptopUid, laptop), (EntityUid) sentry)}: LOW AMMO ({sentryAmmo}/{maxAmmo})");
  }

  private void CheckHealthAlert(
    EntityUid laptopUid,
    Entity<SentryComponent> sentry,
    SentryLaptopComponent laptop,
    TimeSpan time)
  {
    float maxHealth;
    float sentryHealth = this.GetSentryHealth(sentry, out maxHealth);
    if ((double) maxHealth <= 0.0 || (double) sentryHealth / (double) maxHealth > (double) sentry.Comp.CriticalHealthThreshold || !(time - sentry.Comp.LastHealthAlert > sentry.Comp.AlertCooldown))
      return;
    sentry.Comp.LastHealthAlert = time;
    this.Dirty<SentryComponent>(sentry);
    this.SendAlert(laptopUid, (EntityUid) sentry, SentryAlertType.CriticalHealth, this.GetSentryDisplayName((Entity<SentryLaptopComponent>) (laptopUid, laptop), (EntityUid) sentry) + ": CRITICAL DAMAGE");
  }

  private void OnSentryDamageChanged(Entity<SentryComponent> sentry, ref DamageChangedEvent args)
  {
    if (!this._net.IsServer || !args.DamageIncreased || args.DamageDelta == null)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime - sentry.Comp.LastHealthAlert < sentry.Comp.AlertCooldown)
      return;
    sentry.Comp.LastHealthAlert = curTime;
    this.Dirty<SentryComponent>(sentry);
    Entity<SentryLaptopComponent>? laptop1;
    if (!this.TryGetLinkedLaptop(sentry.Owner, out laptop1))
      return;
    float maxHealth;
    float sentryHealth = this.GetSentryHealth(sentry, out maxHealth);
    int num = (double) maxHealth > 0.0 ? (int) ((double) sentryHealth / (double) maxHealth * 100.0) : 0;
    Entity<SentryLaptopComponent> laptop2 = laptop1.Value;
    this.SendAlert(laptop2.Owner, (EntityUid) sentry, SentryAlertType.Damaged, $"{this.GetSentryDisplayName(laptop2, (EntityUid) sentry)}: Taking damage! ({num}% health)");
  }

  private void OnSentryShot(Entity<SentryComponent> sentry, ref GunShotEvent args)
  {
    if (!this._net.IsServer)
      return;
    TimeSpan curTime = this._timing.CurTime;
    GunComponent comp;
    if (curTime - sentry.Comp.LastTargetAlert < sentry.Comp.AlertCooldown || !this.TryComp<GunComponent>((EntityUid) sentry, out comp) || !comp.Target.HasValue)
      return;
    sentry.Comp.LastTargetAlert = curTime;
    this.Dirty<SentryComponent>(sentry);
    Entity<SentryLaptopComponent>? laptop1;
    if (!this.TryGetLinkedLaptop(sentry.Owner, out laptop1))
      return;
    string str = this.Name(comp.Target.Value);
    Entity<SentryLaptopComponent> laptop2 = laptop1.Value;
    this.SendAlert(laptop2.Owner, (EntityUid) sentry, SentryAlertType.TargetAcquired, $"{this.GetSentryDisplayName(laptop2, (EntityUid) sentry)}: Engaging {str}");
  }

  private void SendAlert(
    EntityUid laptop,
    EntityUid sentry,
    SentryAlertType alertType,
    string message)
  {
    if (!this._net.IsServer)
      return;
    (string color, int num) = SharedSentryLaptopSystem.GetAlertStyle(alertType);
    SentryAlertEvent message1 = new SentryAlertEvent(this.GetNetEntity(sentry), alertType, message, color, num);
    if (this._ui.IsUiOpen((Entity<UserInterfaceComponent>) laptop, (Enum) SentryLaptopUiKey.Key))
    {
      this._ui.ServerSendUiMessage((Entity<UserInterfaceComponent>) laptop, (Enum) SentryLaptopUiKey.Key, (BoundUserInterfaceMessage) message1);
    }
    else
    {
      if (!this.HasComp<PlaceableSurfaceComponent>(this.Transform(laptop).ParentUid))
        return;
      PopupType popupType = SharedSentryLaptopSystem.GetPopupType(alertType);
      this._popup.PopupEntity(message, laptop, popupType);
    }
  }

  private static (string color, int size) GetAlertStyle(SentryAlertType alertType)
  {
    (string, int) alertStyle;
    switch (alertType)
    {
      case SentryAlertType.LowAmmo:
        alertStyle = ("#CED22B", 14);
        break;
      case SentryAlertType.CriticalHealth:
        alertStyle = ("#A42625", 16 /*0x10*/);
        break;
      case SentryAlertType.TargetAcquired:
        alertStyle = ("#A42625", 14);
        break;
      case SentryAlertType.Damaged:
        alertStyle = ("#A42625", 14);
        break;
      default:
        alertStyle = ("#88C7FA", 14);
        break;
    }
    return alertStyle;
  }

  private static PopupType GetPopupType(SentryAlertType alertType)
  {
    PopupType popupType;
    switch (alertType)
    {
      case SentryAlertType.LowAmmo:
        popupType = PopupType.Medium;
        break;
      case SentryAlertType.CriticalHealth:
        popupType = PopupType.LargeCaution;
        break;
      case SentryAlertType.TargetAcquired:
        popupType = PopupType.MediumCaution;
        break;
      case SentryAlertType.Damaged:
        popupType = PopupType.MediumCaution;
        break;
      default:
        popupType = PopupType.Medium;
        break;
    }
    return popupType;
  }

  private void OnLaptopAfterInteract(
    Entity<SentryLaptopComponent> laptop,
    ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    target = args.Target;
    if (!this.HasComp<PlaceableSurfaceComponent>(target.Value))
      return;
    args.Handled = true;
    if (!this._hands.TryDrop((Entity<HandsComponent>) args.User, laptop.Owner, checkActionBlocker: false) || !this._net.IsServer)
      return;
    Entity<SentryLaptopComponent> laptop1 = laptop;
    target = args.Target;
    EntityUid surface = target.Value;
    EntityUid user = args.User;
    this.PlaceLaptopOnSurface(laptop1, surface, user);
  }

  private void OnLaptopUIOpenAttempt(
    Entity<SentryLaptopComponent> laptop,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (!this.HasComp<PlaceableSurfaceComponent>(this.Transform((EntityUid) laptop).ParentUid))
    {
      this._popup.PopupClient("Place the laptop on a table first!", (EntityUid) laptop, new EntityUid?(args.User));
      args.Cancel();
    }
    else
    {
      if (laptop.Comp.IsOpen)
        return;
      laptop.Comp.IsOpen = true;
      this.SetPowered(laptop, true);
      this.UpdateLaptopVisuals(laptop);
      this.Dirty<SentryLaptopComponent>(laptop);
    }
  }

  private void OnLaptopShutdown(Entity<SentryLaptopComponent> laptop, ref ComponentShutdown args)
  {
    this.UnlinkAllSentries(laptop);
  }

  private void OnSentryLinkedShutdown(
    Entity<SentryLaptopLinkedComponent> sentry,
    ref ComponentShutdown args)
  {
    SentryLaptopComponent comp;
    if (!sentry.Comp.LinkedLaptop.HasValue || !this.TryComp<SentryLaptopComponent>(sentry.Comp.LinkedLaptop.Value, out comp))
      return;
    this.UnlinkSentry((Entity<SentryLaptopComponent>) (sentry.Comp.LinkedLaptop.Value, comp), sentry.Owner);
  }

  private void OnUnlinkMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopUnlinkBuiMsg args)
  {
    EntityUid? entity;
    if (!this._net.IsServer || !this.TryGetEntity(args.Sentry, out entity))
      return;
    this.UnlinkSentry(laptop, entity.Value);
    this.UpdateUI(laptop);
  }

  private void OnUnlinkAllMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopUnlinkAllBuiMsg args)
  {
    if (!this._net.IsServer)
      return;
    this.UnlinkAllSentries(laptop);
    this.UpdateUI(laptop);
  }

  private void OnSetFactionsMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopSetFactionsBuiMsg args)
  {
    EntityUid? entity;
    SentryTargetingComponent comp;
    if (!this._net.IsServer || !this.TryGetEntity(args.Sentry, out entity) || !this.GetLinkedSentries(laptop).Contains(entity.Value) || !this.TryComp<SentryTargetingComponent>(entity.Value, out comp))
      return;
    HashSet<string> factions = new HashSet<string>((IEnumerable<string>) args.Factions);
    this._sentryTargeting.SetFriendlyFactions((Entity<SentryTargetingComponent>) (entity.Value, comp), factions);
    this.UpdateUI(laptop);
  }

  private void OnToggleFactionMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopToggleFactionBuiMsg args)
  {
    EntityUid? entity;
    SentryTargetingComponent comp;
    if (!this._net.IsServer || !this.TryGetEntity(args.Sentry, out entity) || !this.GetLinkedSentries(laptop).Contains(entity.Value) || !this.TryComp<SentryTargetingComponent>(entity.Value, out comp))
      return;
    this._sentryTargeting.ToggleFaction((Entity<SentryTargetingComponent>) (entity.Value, comp), args.Faction, args.Targeted);
    this.UpdateUI(laptop);
  }

  private void OnTogglePowerMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopTogglePowerBuiMsg args)
  {
    EntityUid? entity;
    SentryComponent comp;
    if (!this._net.IsServer || !this.TryGetEntity(args.Sentry, out entity) || !this.GetLinkedSentries(laptop).Contains(entity.Value) || !this.TryComp<SentryComponent>(entity.Value, out comp) || comp.Mode == SentryMode.Item)
      return;
    SentryMode mode = comp.Mode == SentryMode.On ? SentryMode.Off : SentryMode.On;
    this.EntityManager.System<SentrySystem>().TrySetMode((Entity<SentryComponent>) (entity.Value, comp), mode);
    this.UpdateUI(laptop);
  }

  private void OnResetTargetingMessage(
    Entity<SentryLaptopComponent> laptop,
    ref SentryLaptopResetTargetingBuiMsg args)
  {
    EntityUid? entity;
    SentryTargetingComponent comp;
    if (!this._net.IsServer || !this.TryGetEntity(args.Sentry, out entity) || !this.GetLinkedSentries(laptop).Contains(entity.Value) || !this.TryComp<SentryTargetingComponent>(entity.Value, out comp))
      return;
    this._sentryTargeting.ResetToDefault((Entity<SentryTargetingComponent>) (entity.Value, comp));
    this.UpdateUI(laptop);
  }

  private void PlaceLaptopOnSurface(
    Entity<SentryLaptopComponent> laptop,
    EntityUid surface,
    EntityUid user)
  {
    TransformComponent transformComponent = this.Transform(surface);
    this._transform.SetCoordinates(laptop.Owner, transformComponent.Coordinates);
    this._transform.SetParent(laptop.Owner, surface);
    laptop.Comp.IsOpen = true;
    this.SetPowered(laptop, true);
    this.UpdateLaptopVisuals(laptop);
    this.Dirty<SentryLaptopComponent>(laptop);
  }

  private void OnLaptopParentChanged(
    Entity<SentryLaptopComponent> laptop,
    ref EntParentChangedMessage args)
  {
    bool powered = this.HasComp<PlaceableSurfaceComponent>(this.Transform((EntityUid) laptop).ParentUid);
    laptop.Comp.IsOpen = powered;
    this.SetPowered(laptop, powered);
    this.UpdateLaptopVisuals(laptop);
    this.Dirty<SentryLaptopComponent>(laptop);
    if (!this._net.IsServer || powered)
      return;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) laptop.Owner, (Enum) SentryLaptopUiKey.Key);
  }

  private bool IsSentryAlreadyLinked(Entity<SentryComponent> sentry)
  {
    SentryLaptopLinkedComponent comp;
    return this.TryComp<SentryLaptopLinkedComponent>((EntityUid) sentry, out comp) && comp.LinkedLaptop.HasValue;
  }

  private bool ValidateLaptopForLinking(
    Entity<SentryLaptopComponent> laptop,
    Entity<SentryComponent> sentry,
    EntityUid user)
  {
    if (!laptop.Comp.IsOpen)
    {
      this._popup.PopupClient("The laptop must be opened first!", (EntityUid) laptop, new EntityUid?(user));
      return false;
    }
    if (this.GetLinkedSentries(laptop).Count < laptop.Comp.MaxLinkedSentries)
      return true;
    this._popup.PopupClient($"The laptop can only control {laptop.Comp.MaxLinkedSentries} sentries at once!", (EntityUid) laptop, new EntityUid?(user));
    return false;
  }

  private void LinkSentryToLaptop(
    Entity<SentryLaptopComponent> laptop,
    Entity<SentryComponent> sentry,
    EntityUid user)
  {
    DeviceLinkSourceComponent sourceComponent = this.EnsureComp<DeviceLinkSourceComponent>(laptop.Owner);
    DeviceLinkSinkComponent sinkComponent = this.EnsureComp<DeviceLinkSinkComponent>(sentry.Owner);
    this._deviceLink.LinkDefaults(new EntityUid?(user), laptop.Owner, sentry.Owner, sourceComponent, sinkComponent);
    laptop.Comp.LinkedSentries.Add((EntityUid) sentry);
    SentryLaptopLinkedComponent laptopLinkedComponent = this.EnsureComp<SentryLaptopLinkedComponent>((EntityUid) sentry);
    laptopLinkedComponent.LinkedLaptop = new EntityUid?((EntityUid) laptop);
    this.Dirty((EntityUid) sentry, (IComponent) laptopLinkedComponent);
    this.InitializeSentryTargeting(sentry.Owner);
    this._popup.PopupEntity($"Successfully linked {this.Name((EntityUid) sentry)} to the laptop.", (EntityUid) sentry, user);
    if (laptop.Comp.LinkedSentries.Count == 1)
      this.SetPowered(laptop, true);
    this.Dirty<SentryLaptopComponent>(laptop);
    this.UpdateUI(laptop);
  }

  private void InitializeSentryTargeting(EntityUid sentry)
  {
    SentryTargetingComponent comp;
    if (!this.TryComp<SentryTargetingComponent>(sentry, out comp))
      comp = this.EnsureComp<SentryTargetingComponent>(sentry);
    if (!this.HasComp<GunIFFComponent>(sentry) && this.HasComp<GunComponent>(sentry))
      this._iff.EnableIntrinsicIFF(sentry);
    HashSet<string> factions = comp.FriendlyFactions.Count > 0 ? new HashSet<string>((IEnumerable<string>) comp.FriendlyFactions) : new HashSet<string>();
    if (factions.Count == 0)
    {
      if (!string.IsNullOrEmpty(comp.OriginalFaction))
        factions.Add(comp.OriginalFaction);
      else
        factions.Add("UNMC");
    }
    foreach (string humanoidFaction in this._sentryTargeting.GetHumanoidFactions())
      factions.Add(humanoidFaction);
    this._sentryTargeting.SetFriendlyFactions((Entity<SentryTargetingComponent>) (sentry, comp), factions);
  }

  public void UnlinkSentry(Entity<SentryLaptopComponent> laptop, EntityUid sentry)
  {
    if (!laptop.Comp.LinkedSentries.Remove(sentry))
      return;
    DeviceLinkSinkComponent comp1;
    if (this.TryComp<DeviceLinkSinkComponent>(sentry, out comp1))
      this._deviceLink.RemoveAllFromSink(sentry, comp1);
    laptop.Comp.SentryCustomNames.Remove(sentry);
    this.RemComp<SentryLaptopLinkedComponent>(sentry);
    SentryTargetingComponent comp2;
    if (this.TryComp<SentryTargetingComponent>(sentry, out comp2))
      this._sentryTargeting.ResetToDefault((Entity<SentryTargetingComponent>) (sentry, comp2));
    if (laptop.Comp.LinkedSentries.Count == 0)
      this.SetPowered(laptop, false);
    this.Dirty<SentryLaptopComponent>(laptop);
  }

  private void UnlinkAllSentries(Entity<SentryLaptopComponent> laptop)
  {
    foreach (EntityUid sentry in this.GetLinkedSentries(laptop).ToList<EntityUid>())
      this.UnlinkSentry(laptop, sentry);
  }

  public void SetPowered(Entity<SentryLaptopComponent> laptop, bool powered)
  {
    laptop.Comp.IsPowered = powered;
    this.UpdateLaptopVisuals(laptop);
    this.Dirty<SentryLaptopComponent>(laptop);
  }

  private void UpdateLaptopVisuals(Entity<SentryLaptopComponent> laptop)
  {
    SentryLaptopState sentryLaptopState = SentryLaptopState.Closed;
    if (laptop.Comp.IsOpen)
      sentryLaptopState = laptop.Comp.IsPowered ? SentryLaptopState.Active : SentryLaptopState.Open;
    this._appearance.SetData((EntityUid) laptop, (Enum) SentryLaptopVisuals.State, (object) sentryLaptopState);
  }

  protected virtual void UpdateUI(Entity<SentryLaptopComponent> laptop)
  {
  }

  protected List<SentryInfo> BuildSentryInfoList(Entity<SentryLaptopComponent> laptop)
  {
    List<SentryInfo> sentryInfoList = new List<SentryInfo>();
    foreach (EntityUid linkedSentry in this.GetLinkedSentries(laptop))
    {
      SentryComponent comp;
      if (this.TryComp<SentryComponent>(linkedSentry, out comp))
      {
        SentryInfo sentryInfo = this.BuildSentryInfo(laptop, linkedSentry, comp);
        sentryInfoList.Add(sentryInfo);
      }
    }
    return sentryInfoList;
  }

  protected SentryInfo BuildSentryInfo(
    Entity<SentryLaptopComponent> laptop,
    EntityUid sentryUid,
    SentryComponent sentry)
  {
    string sentryDisplayName = this.GetSentryDisplayName(laptop, sentryUid);
    string str;
    string CustomName = laptop.Comp.SentryCustomNames.TryGetValue(sentryUid, out str) ? str : (string) null;
    float sentryVisionRadius = this.GetSentryVisionRadius(sentryUid);
    float degrees = (float) ((Angle) ref sentry.MaxDeviation).Degrees;
    float maxHealth;
    int maxAmmo;
    return new SentryInfo(this.GetNetEntity(sentryUid), sentryDisplayName, sentry.Mode, this.GetSentryHealth(sentryUid, out maxHealth), maxHealth, this.GetSentryAmmo(sentryUid, out maxAmmo), maxAmmo, this.GetSentryLocation(sentryUid), this.GetSentryTarget(sentryUid), this.GetSentryFriendlyFactions(sentryUid), CustomName, sentryVisionRadius, degrees, this.GetSentryHumanoidAdded(sentryUid));
  }

  protected virtual float GetSentryVisionRadius(EntityUid sentry) => 5f;

  private string GetSentryDisplayName(Entity<SentryLaptopComponent> laptop, EntityUid sentry)
  {
    string str;
    return laptop.Comp.SentryCustomNames.TryGetValue(sentry, out str) ? str : this.Name(sentry);
  }

  private float GetSentryHealth(EntityUid sentry, out float maxHealth)
  {
    maxHealth = this.GetSentryMaxHealth(sentry);
    float sentryHealth = maxHealth;
    DamageableComponent comp;
    if (this.TryComp<DamageableComponent>(sentry, out comp))
    {
      float num = comp.TotalDamage.Float();
      sentryHealth = Math.Max(0.0f, maxHealth - num);
    }
    return sentryHealth;
  }

  private float GetSentryHealth(Entity<SentryComponent> sentry, out float maxHealth)
  {
    return this.GetSentryHealth(sentry.Owner, out maxHealth);
  }

  private int GetSentryAmmo(EntityUid sentry, out int maxAmmo)
  {
    maxAmmo = 0;
    int? ammoCount;
    int? ammoCapacity;
    if (!this.EntityManager.System<SentrySystem>().TryGetSentryAmmo(sentry, out ammoCount, out ammoCapacity))
      return 0;
    maxAmmo = ammoCapacity.Value;
    return ammoCount.Value;
  }

  private string GetSentryLocation(EntityUid sentry)
  {
    Entity<AreaComponent>? area;
    return this._area.TryGetArea(sentry, out area, out EntityPrototype _) ? this.Name((EntityUid) area.Value) : "Unknown";
  }

  protected List<EntityUid> GetLinkedSentries(Entity<SentryLaptopComponent> laptop)
  {
    List<EntityUid> source = new List<EntityUid>();
    DeviceLinkSourceComponent comp;
    if (this.TryComp<DeviceLinkSourceComponent>((EntityUid) laptop, out comp))
    {
      foreach (EntityUid key in comp.LinkedPorts.Keys)
      {
        if (this.HasComp<SentryComponent>(key))
          source.Add(key);
      }
      laptop.Comp.LinkedSentries = source.ToHashSet<EntityUid>();
      return source;
    }
    source.AddRange((IEnumerable<EntityUid>) laptop.Comp.LinkedSentries);
    return source;
  }

  private NetEntity? GetSentryTarget(EntityUid sentry)
  {
    GunComponent comp;
    return this.TryComp<GunComponent>(sentry, out comp) && comp.Target.HasValue ? new NetEntity?(this.GetNetEntity(comp.Target.Value)) : new NetEntity?();
  }

  private HashSet<string> GetSentryFriendlyFactions(EntityUid sentry)
  {
    SentryTargetingComponent comp;
    return this.TryComp<SentryTargetingComponent>(sentry, out comp) ? comp.FriendlyFactions : new HashSet<string>();
  }

  private HashSet<string> GetSentryHumanoidAdded(EntityUid sentry)
  {
    SentryTargetingComponent comp;
    return this.TryComp<SentryTargetingComponent>(sentry, out comp) ? comp.HumanoidAdded : new HashSet<string>();
  }

  protected virtual float GetSentryMaxHealth(EntityUid sentry) => 100f;

  private bool TryGetLinkedLaptop(EntityUid sentry, out Entity<SentryLaptopComponent>? laptop)
  {
    laptop = new Entity<SentryLaptopComponent>?();
    DeviceLinkSinkComponent comp1;
    if (!this.TryComp<DeviceLinkSinkComponent>(sentry, out comp1))
      return false;
    foreach (EntityUid linkedSource in comp1.LinkedSources)
    {
      SentryLaptopComponent comp2;
      if (this.TryComp<SentryLaptopComponent>(linkedSource, out comp2))
      {
        laptop = new Entity<SentryLaptopComponent>?(new Entity<SentryLaptopComponent>(linkedSource, comp2));
        return true;
      }
    }
    return false;
  }
}
