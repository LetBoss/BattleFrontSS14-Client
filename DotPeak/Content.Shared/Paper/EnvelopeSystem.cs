// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.EnvelopeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Paper;

public sealed class EnvelopeSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private ItemSlotsSystem _itemSlotsSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EnvelopeComponent, ItemSlotInsertAttemptEvent>(new EntityEventRefHandler<EnvelopeComponent, ItemSlotInsertAttemptEvent>(this.OnInsertAttempt));
    this.SubscribeLocalEvent<EnvelopeComponent, ItemSlotEjectAttemptEvent>(new EntityEventRefHandler<EnvelopeComponent, ItemSlotEjectAttemptEvent>(this.OnEjectAttempt));
    this.SubscribeLocalEvent<EnvelopeComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<EnvelopeComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAltVerbs));
    this.SubscribeLocalEvent<EnvelopeComponent, EnvelopeDoAfterEvent>(new EntityEventRefHandler<EnvelopeComponent, EnvelopeDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<EnvelopeComponent, ExaminedEvent>(new EntityEventRefHandler<EnvelopeComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnExamine(Entity<EnvelopeComponent> ent, ref ExaminedEvent args)
  {
    if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed)
    {
      args.PushMarkup(this.Loc.GetString("envelope-sealed-examine", ("envelope", (object) ent.Owner)));
    }
    else
    {
      if (ent.Comp.State != EnvelopeComponent.EnvelopeState.Torn)
        return;
      args.PushMarkup(this.Loc.GetString("envelope-torn-examine", ("envelope", (object) ent.Owner)));
    }
  }

  private void OnGetAltVerbs(Entity<EnvelopeComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || ent.Comp.State == EnvelopeComponent.EnvelopeState.Torn)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString(ent.Comp.State == EnvelopeComponent.EnvelopeState.Open ? "envelope-verb-seal" : "envelope-verb-tear");
    alternativeVerb.IconEntity = new NetEntity?(this.GetNetEntity(ent.Owner));
    alternativeVerb.Act = (Action) (() => this.TryStartDoAfter(ent, user, ent.Comp.State == EnvelopeComponent.EnvelopeState.Open ? ent.Comp.SealDelay : ent.Comp.TearDelay));
    verbs.Add(alternativeVerb);
  }

  private void OnInsertAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotInsertAttemptEvent args)
  {
    args.Cancelled |= ent.Comp.State != 0;
  }

  private void OnEjectAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotEjectAttemptEvent args)
  {
    args.Cancelled |= ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed;
  }

  private void TryStartDoAfter(Entity<EnvelopeComponent> ent, EntityUid user, TimeSpan delay)
  {
    if (ent.Comp.EnvelopeDoAfter.HasValue)
      return;
    DoAfterId? id;
    if (!this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) new EnvelopeDoAfterEvent(), new EntityUid?(ent.Owner), new EntityUid?(ent.Owner))
    {
      BreakOnDamage = true,
      NeedHand = true,
      BreakOnHandChange = true,
      MovementThreshold = 0.01f,
      DistanceThreshold = new float?(1f)
    }, out id))
      return;
    ent.Comp.EnvelopeDoAfter = id;
  }

  private void OnDoAfter(Entity<EnvelopeComponent> ent, ref EnvelopeDoAfterEvent args)
  {
    ent.Comp.EnvelopeDoAfter = new DoAfterId?();
    if (args.Cancelled)
      return;
    if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Open)
    {
      this._audioSystem.PlayPredicted((SoundSpecifier) ent.Comp.SealSound, ent.Owner, new EntityUid?(args.User));
      ent.Comp.State = EnvelopeComponent.EnvelopeState.Sealed;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
    }
    else
    {
      if (ent.Comp.State != EnvelopeComponent.EnvelopeState.Sealed)
        return;
      this._audioSystem.PlayPredicted((SoundSpecifier) ent.Comp.TearSound, ent.Owner, new EntityUid?(args.User));
      ent.Comp.State = EnvelopeComponent.EnvelopeState.Torn;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
      ItemSlot itemSlot;
      if (!this._itemSlotsSystem.TryGetSlot(ent.Owner, ent.Comp.SlotId, out itemSlot))
        return;
      this._itemSlotsSystem.TryEjectToHands(ent.Owner, itemSlot, new EntityUid?(args.User));
    }
  }
}
