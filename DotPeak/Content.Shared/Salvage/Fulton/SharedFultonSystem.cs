// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Fulton.SharedFultonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Fulton;

public abstract class SharedFultonSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  protected IGameTiming Timing;
  [Robust.Shared.IoC.Dependency]
  private MetaDataSystem _metadata;
  [Robust.Shared.IoC.Dependency]
  protected SharedAudioSystem Audio;
  [Robust.Shared.IoC.Dependency]
  private SharedDoAfterSystem _doAfter;
  [Robust.Shared.IoC.Dependency]
  private FoldableSystem _foldable;
  [Robust.Shared.IoC.Dependency]
  protected SharedContainerSystem Container;
  [Robust.Shared.IoC.Dependency]
  private SharedPopupSystem _popup;
  [Robust.Shared.IoC.Dependency]
  private SharedStackSystem _stack;
  [Robust.Shared.IoC.Dependency]
  protected SharedTransformSystem TransformSystem;
  [Robust.Shared.IoC.Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  public static readonly EntProtoId EffectProto = (EntProtoId) "FultonEffect";
  protected static readonly Vector2 EffectOffset = Vector2.Zero;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SharedFultonSystem.FultonedDoAfterEvent>(new EntityEventHandler<SharedFultonSystem.FultonedDoAfterEvent>(this.OnFultonDoAfter));
    this.SubscribeLocalEvent<FultonedComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<FultonedComponent, GetVerbsEvent<InteractionVerb>>(this.OnFultonedGetVerbs));
    this.SubscribeLocalEvent<FultonedComponent, ExaminedEvent>(new ComponentEventHandler<FultonedComponent, ExaminedEvent>(this.OnFultonedExamine));
    this.SubscribeLocalEvent<FultonedComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<FultonedComponent, EntGotInsertedIntoContainerMessage>(this.OnFultonContainerInserted));
    this.SubscribeLocalEvent<FultonComponent, AfterInteractEvent>(new ComponentEventHandler<FultonComponent, AfterInteractEvent>(this.OnFultonInteract));
    this.SubscribeLocalEvent<FultonComponent, StackSplitEvent>(new ComponentEventRefHandler<FultonComponent, StackSplitEvent>(this.OnFultonSplit));
  }

  private void OnFultonContainerInserted(
    EntityUid uid,
    FultonedComponent component,
    EntGotInsertedIntoContainerMessage args)
  {
    this.RemCompDeferred<FultonedComponent>(uid);
  }

  private void OnFultonedExamine(EntityUid uid, FultonedComponent component, ExaminedEvent args)
  {
    string text = this.Loc.GetString("fulton-examine", ("time", (object) $"{(component.NextFulton + this._metadata.GetPauseTime(uid) - this.Timing.CurTime).TotalSeconds:0.00}"));
    args.PushText(text);
  }

  private void OnFultonedGetVerbs(
    EntityUid uid,
    FultonedComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("fulton-remove");
    interactionVerb.Act = (Action) (() => this.Unfulton(uid));
    verbs.Add(interactionVerb);
  }

  private void Unfulton(EntityUid uid, FultonedComponent? component = null)
  {
    if (!this.Resolve<FultonedComponent>(uid, ref component, false) || !component.Removeable)
      return;
    this.RemCompDeferred<FultonedComponent>(uid);
  }

  private void OnFultonDoAfter(SharedFultonSystem.FultonedDoAfterEvent args)
  {
    FultonComponent comp;
    if (args.Cancelled || !args.Target.HasValue || !this.TryComp<FultonComponent>(args.Used, out comp))
      return;
    SharedStackSystem stack = this._stack;
    EntityUid? nullable = args.Used;
    EntityUid uid = nullable.Value;
    if (!stack.Use(uid, 1))
      return;
    nullable = args.Target;
    FultonedComponent fultoned = this.AddComp<FultonedComponent>(nullable.Value);
    fultoned.Beacon = comp.Beacon;
    fultoned.NextFulton = this.Timing.CurTime + comp.FultonDuration;
    fultoned.FultonDuration = comp.FultonDuration;
    fultoned.Removeable = comp.Removeable;
    nullable = args.Target;
    this.UpdateAppearance(nullable.Value, fultoned);
    nullable = args.Target;
    this.Dirty(nullable.Value, (IComponent) fultoned);
    SharedAudioSystem audio = this.Audio;
    SoundSpecifier fultonSound = comp.FultonSound;
    nullable = args.Target;
    EntityUid source = nullable.Value;
    EntityUid? user = new EntityUid?(args.User);
    AudioParams? audioParams = new AudioParams?();
    audio.PlayPredicted(fultonSound, source, user, audioParams);
  }

  private void OnFultonInteract(EntityUid uid, FultonComponent component, AfterInteractEvent args)
  {
    if (!args.Target.HasValue || args.Handled || !args.CanReach)
      return;
    FultonBeaconComponent comp;
    if (this.TryComp<FultonBeaconComponent>(args.Target, out comp))
    {
      FoldableSystem foldable = this._foldable;
      EntityUid? target = args.Target;
      EntityUid uid1 = target.Value;
      if (!foldable.IsFolded(uid1))
      {
        FultonComponent fultonComponent = component;
        target = args.Target;
        EntityUid? nullable = new EntityUid?(target.Value);
        fultonComponent.Beacon = nullable;
        this.Audio.PlayPredicted(comp.LinkSound, uid, new EntityUid?(args.User));
        this._popup.PopupClient(this.Loc.GetString("fulton-linked"), uid, new EntityUid?(args.User));
      }
      else
      {
        component.Beacon = new EntityUid?(EntityUid.Invalid);
        this._popup.PopupClient(this.Loc.GetString("fulton-folded"), uid, new EntityUid?(args.User));
      }
    }
    else if (this.Deleted(component.Beacon))
      this._popup.PopupClient(this.Loc.GetString("fulton-not-found"), uid, new EntityUid?(args.User));
    else if (!this.CanApplyFulton(args.Target.Value, component))
      this._popup.PopupClient(this.Loc.GetString("fulton-invalid"), uid, new EntityUid?(uid));
    else if (this.HasComp<FultonedComponent>(args.Target))
    {
      this._popup.PopupClient(this.Loc.GetString("fulton-fultoned"), uid, new EntityUid?(uid));
    }
    else
    {
      args.Handled = true;
      SharedFultonSystem.FultonedDoAfterEvent @event = new SharedFultonSystem.FultonedDoAfterEvent();
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, component.ApplyFultonDuration, (DoAfterEvent) @event, args.Target, args.Target, new EntityUid?(args.Used))
      {
        MovementThreshold = 0.5f,
        BreakOnMove = true,
        Broadcast = true,
        NeedHand = true
      });
    }
  }

  private void OnFultonSplit(EntityUid uid, FultonComponent component, ref StackSplitEvent args)
  {
    FultonComponent fultonComponent = this.EnsureComp<FultonComponent>(args.NewId);
    fultonComponent.Beacon = component.Beacon;
    this.Dirty(args.NewId, (IComponent) fultonComponent);
  }

  protected virtual void UpdateAppearance(EntityUid uid, FultonedComponent fultoned)
  {
  }

  protected bool CanApplyFulton(EntityUid targetUid, FultonComponent component)
  {
    return this.CanFulton(targetUid) && !this._whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, targetUid);
  }

  protected bool CanFulton(EntityUid uid)
  {
    return !this.Transform(uid).Anchored && !this.Container.IsEntityInContainer(uid);
  }

  [NetSerializable]
  [Serializable]
  private sealed class FultonedDoAfterEvent : 
    SimpleDoAfterEvent,
    ISerializationGenerated<SharedFultonSystem.FultonedDoAfterEvent>,
    ISerializationGenerated
  {
    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedFultonSystem.FultonedDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedFultonSystem.FultonedDoAfterEvent) target1;
      serialization.TryCustomCopy<SharedFultonSystem.FultonedDoAfterEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedFultonSystem.FultonedDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref SimpleDoAfterEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedFultonSystem.FultonedDoAfterEvent target1 = (SharedFultonSystem.FultonedDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (SimpleDoAfterEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedFultonSystem.FultonedDoAfterEvent target1 = (SharedFultonSystem.FultonedDoAfterEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedFultonSystem.FultonedDoAfterEvent SimpleDoAfterEvent.Instantiate()
    {
      return new SharedFultonSystem.FultonedDoAfterEvent();
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class FultonAnimationMessage : EntityEventArgs
  {
    public NetEntity Entity;
    public NetCoordinates Coordinates;
  }
}
