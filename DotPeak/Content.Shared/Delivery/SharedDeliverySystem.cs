// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.SharedDeliverySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.FingerprintReader;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Objectives.Components;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Tag;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Delivery;

public abstract class SharedDeliverySystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private FingerprintReaderSystem _fingerprintReader;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private NameModifierSystem _nameModifier;
  private static readonly ProtoId<TagPrototype> TrashTag = ProtoId<TagPrototype>.op_Implicit("Trash");
  private static readonly ProtoId<TagPrototype> RecyclableTag = ProtoId<TagPrototype>.op_Implicit("Recyclable");

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryComponent, ExaminedEvent>(new EntityEventRefHandler<DeliveryComponent, ExaminedEvent>((object) this, __methodptr(OnDeliveryExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryComponent, UseInHandEvent>(new EntityEventRefHandler<DeliveryComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DeliveryComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(OnGetDeliveryVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryComponent, AttemptSimpleToolUseEvent>(new EntityEventRefHandler<DeliveryComponent, AttemptSimpleToolUseEvent>((object) this, __methodptr(OnAttemptSimpleToolUse)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryComponent, SimpleToolDoAfterEvent>(new EntityEventRefHandler<DeliveryComponent, SimpleToolDoAfterEvent>((object) this, __methodptr(OnSimpleToolUse)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliverySpawnerComponent, ExaminedEvent>(new EntityEventRefHandler<DeliverySpawnerComponent, ExaminedEvent>((object) this, __methodptr(OnSpawnerExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliverySpawnerComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DeliverySpawnerComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(OnGetSpawnerVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnDeliveryExamine(Entity<DeliveryComponent> ent, ref ExaminedEvent args)
  {
    string str1 = ent.Comp.RecipientJobTitle ?? this.Loc.GetString("delivery-recipient-no-job");
    string str2 = ent.Comp.RecipientName ?? this.Loc.GetString("delivery-recipient-no-name");
    using (args.PushGroup("DeliveryComponent", 1))
    {
      if (ent.Comp.IsOpened)
        args.PushText(this.Loc.GetString("delivery-already-opened-examine"));
      args.PushText(this.Loc.GetString("delivery-recipient-examine", ("recipient", (object) str2), ("job", (object) str1)));
    }
    if (!ent.Comp.IsLocked)
      return;
    float deliveryMultiplier = this.GetDeliveryMultiplier(ent);
    double num = Math.Round((double) ent.Comp.BaseSpesoReward * (double) deliveryMultiplier);
    args.PushMarkup(this.Loc.GetString("delivery-earnings-examine", ("spesos", (object) num)), -1);
  }

  private void OnSpawnerExamine(Entity<DeliverySpawnerComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("delivery-teleporter-amount-examine", ("amount", (object) ent.Comp.ContainedDeliveryAmount)), 50);
  }

  private void OnUseInHand(Entity<DeliveryComponent> ent, ref UseInHandEvent args)
  {
    args.Handled = true;
    if (ent.Comp.IsOpened)
      return;
    if (ent.Comp.IsLocked)
      this.TryUnlockDelivery(ent, args.User);
    else
      this.OpenDelivery(ent, args.User);
  }

  private void OnGetDeliveryVerbs(
    Entity<DeliveryComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || ent.Comp.IsOpened || this._hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.User), new EntityUid?(Entity<DeliveryComponent>.op_Implicit(ent))))
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Act = (Action) (() =>
    {
      if (ent.Comp.IsLocked)
        this.TryUnlockDelivery(ent, user);
      else
        this.OpenDelivery(ent, user, false);
    });
    alternativeVerb.Text = ent.Comp.IsLocked ? this.Loc.GetString("delivery-unlock-verb") : this.Loc.GetString("delivery-open-verb");
    verbs.Add(alternativeVerb);
  }

  private void OnAttemptSimpleToolUse(
    Entity<DeliveryComponent> ent,
    ref AttemptSimpleToolUseEvent args)
  {
    if (!ent.Comp.IsOpened && ent.Comp.IsLocked)
      return;
    args.Cancelled = true;
  }

  private void OnSimpleToolUse(Entity<DeliveryComponent> ent, ref SimpleToolDoAfterEvent args)
  {
    if (ent.Comp.IsOpened || args.Cancelled)
      return;
    this.HandlePenalty(ent);
    this.TryUnlockDelivery(ent, args.User, false, true);
    this.OpenDelivery(ent, args.User, false, true);
  }

  private void OnGetSpawnerVerbs(
    Entity<DeliverySpawnerComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Act = (Action) (() =>
    {
      this._audio.PlayPredicted(ent.Comp.OpenSound, ent.Owner, new EntityUid?(user), new AudioParams?());
      if (ent.Comp.ContainedDeliveryAmount == 0)
      {
        this._popup.PopupPredicted(this.Loc.GetString("delivery-teleporter-empty", ("entity", (object) ent)), (string) null, Entity<DeliverySpawnerComponent>.op_Implicit(ent), new EntityUid?(user));
      }
      else
      {
        this.SpawnDeliveries(Entity<DeliverySpawnerComponent>.op_Implicit(ent.Owner));
        this.UpdateDeliverySpawnerVisuals(Entity<DeliverySpawnerComponent>.op_Implicit(ent), ent.Comp.ContainedDeliveryAmount);
      }
    });
    alternativeVerb.Text = this.Loc.GetString("delivery-teleporter-empty-verb");
    verbs.Add(alternativeVerb);
  }

  private bool TryUnlockDelivery(
    Entity<DeliveryComponent> ent,
    EntityUid user,
    bool rewardMoney = true,
    bool force = false)
  {
    FingerprintReaderComponent fingerprintReaderComponent;
    if (!force && this.TryComp<FingerprintReaderComponent>(Entity<DeliveryComponent>.op_Implicit(ent), ref fingerprintReaderComponent) && !this._fingerprintReader.IsAllowed(Entity<FingerprintReaderComponent>.op_Implicit((Entity<DeliveryComponent>.op_Implicit(ent), fingerprintReaderComponent)), user))
      return false;
    string baseName = this._nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
    if (!force)
      this._audio.PlayPredicted(ent.Comp.UnlockSound, user, new EntityUid?(user), new AudioParams?());
    ent.Comp.IsLocked = false;
    this.UpdateAntiTamperVisuals(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp.IsLocked);
    this.DirtyField<DeliveryComponent>(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp, "IsLocked", (MetaDataComponent) null);
    this.RemCompDeferred<SimpleToolUsageComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
    DeliveryUnlockedEvent deliveryUnlockedEvent = new DeliveryUnlockedEvent(user);
    this.RaiseLocalEvent<DeliveryUnlockedEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref deliveryUnlockedEvent, false);
    if (rewardMoney)
      this.GrantSpesoReward(ent.AsNullable());
    if (!force)
      this._popup.PopupPredicted(this.Loc.GetString("delivery-unlocked-self", ("delivery", (object) baseName)), this.Loc.GetString("delivery-unlocked-others", new (string, object)[3]
      {
        ("delivery", (object) baseName),
        ("recipient", (object) Identity.Name(user, (IEntityManager) this.EntityManager)),
        ("possadj", (object) user)
      }), user, new EntityUid?(user));
    return true;
  }

  private void OpenDelivery(
    Entity<DeliveryComponent> ent,
    EntityUid user,
    bool attemptPickup = true,
    bool force = false)
  {
    string baseName = this._nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
    this._audio.PlayPredicted(ent.Comp.OpenSound, user, new EntityUid?(user), new AudioParams?());
    DeliveryOpenedEvent deliveryOpenedEvent = new DeliveryOpenedEvent(user);
    this.RaiseLocalEvent<DeliveryOpenedEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref deliveryOpenedEvent, false);
    if (attemptPickup)
      this._hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), Entity<DeliveryComponent>.op_Implicit(ent));
    ent.Comp.IsOpened = true;
    this._appearance.SetData(Entity<DeliveryComponent>.op_Implicit(ent), (Enum) DeliveryVisuals.IsTrash, (object) ent.Comp.IsOpened, (AppearanceComponent) null);
    this._tag.AddTags(Entity<DeliveryComponent>.op_Implicit(ent), SharedDeliverySystem.TrashTag, SharedDeliverySystem.RecyclableTag);
    this.EnsureComp<SpaceGarbageComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
    this.RemCompDeferred<StealTargetComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
    this.DirtyField<DeliveryComponent>(ent.Owner, ent.Comp, "IsOpened", (MetaDataComponent) null);
    if (!force)
      this._popup.PopupPredicted(this.Loc.GetString("delivery-opened-self", ("delivery", (object) baseName)), this.Loc.GetString("delivery-opened-others", new (string, object)[3]
      {
        ("delivery", (object) baseName),
        ("recipient", (object) Identity.Name(user, (IEntityManager) this.EntityManager)),
        ("possadj", (object) user)
      }), user, new EntityUid?(user));
    BaseContainer baseContainer;
    if (!this._container.TryGetContainer(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp.Container, ref baseContainer, (ContainerManagerComponent) null))
      return;
    if (attemptPickup)
    {
      foreach (EntityUid entity in baseContainer.ContainedEntities.ToArray<EntityUid>())
        this._hands.PickupOrDrop(new EntityUid?(user), entity);
    }
    else
      this._container.EmptyContainer(baseContainer, true, new EntityCoordinates?(), true);
  }

  private void UpdateAntiTamperVisuals(EntityUid uid, bool isLocked)
  {
    this._appearance.SetData(uid, (Enum) DeliveryVisuals.IsLocked, (object) isLocked, (AppearanceComponent) null);
    if (!this.HasComp<DeliveryPriorityComponent>(uid))
      return;
    this._appearance.SetData(uid, (Enum) DeliveryVisuals.PriorityState, (object) DeliveryPriorityState.Inactive, (AppearanceComponent) null);
  }

  public void UpdatePriorityVisuals(Entity<DeliveryPriorityComponent> ent)
  {
    DeliveryComponent deliveryComponent;
    if (!this.TryComp<DeliveryComponent>(Entity<DeliveryPriorityComponent>.op_Implicit(ent), ref deliveryComponent) || !deliveryComponent.IsLocked || deliveryComponent.IsOpened)
      return;
    this._appearance.SetData(Entity<DeliveryPriorityComponent>.op_Implicit(ent), (Enum) DeliveryVisuals.PriorityState, (object) (DeliveryPriorityState) (ent.Comp.Expired ? 2 : 1), (AppearanceComponent) null);
  }

  public void UpdateBrokenVisuals(Entity<DeliveryFragileComponent> ent, bool isFragile)
  {
    this._appearance.SetData(Entity<DeliveryFragileComponent>.op_Implicit(ent), (Enum) DeliveryVisuals.IsBroken, (object) ent.Comp.Broken, (AppearanceComponent) null);
    this._appearance.SetData(Entity<DeliveryFragileComponent>.op_Implicit(ent), (Enum) DeliveryVisuals.IsFragile, (object) isFragile, (AppearanceComponent) null);
  }

  public void UpdateBombVisuals(Entity<DeliveryBombComponent> ent)
  {
    bool flag = this.HasComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent));
    this._appearance.SetData(Entity<DeliveryBombComponent>.op_Implicit(ent), (Enum) DeliveryVisuals.IsBomb, (object) (DeliveryBombState) (flag ? 2 : 1), (AppearanceComponent) null);
  }

  protected void UpdateDeliverySpawnerVisuals(EntityUid uid, int contents)
  {
    this._appearance.SetData(uid, (Enum) DeliverySpawnerVisuals.Contents, (object) (contents > 0), (AppearanceComponent) null);
  }

  protected float GetDeliveryMultiplier(Entity<DeliveryComponent> ent)
  {
    GetDeliveryMultiplierEvent deliveryMultiplierEvent = new GetDeliveryMultiplierEvent();
    this.RaiseLocalEvent<GetDeliveryMultiplierEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref deliveryMultiplierEvent, false);
    return Math.Max(deliveryMultiplierEvent.AdditiveMultiplier * deliveryMultiplierEvent.MultiplicativeMultiplier, 0.0f);
  }

  protected virtual void GrantSpesoReward(Entity<DeliveryComponent?> ent)
  {
  }

  protected virtual void HandlePenalty(Entity<DeliveryComponent> ent, string? reason = null)
  {
  }

  protected virtual void SpawnDeliveries(Entity<DeliverySpawnerComponent?> ent)
  {
  }
}
