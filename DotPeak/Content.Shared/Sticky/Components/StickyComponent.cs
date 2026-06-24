// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sticky.Components.StickyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Sticky.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sticky.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (StickySystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class StickyComponent : 
  Component,
  ISerializationGenerated<StickyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StickDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public bool CanUnstick = true;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnstickDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public LocId? StickPopupStart;
  [DataField(null, false, 1, false, false, null)]
  public LocId? StickPopupSuccess;
  [DataField(null, false, 1, false, false, null)]
  public LocId? UnstickPopupStart;
  [DataField(null, false, 1, false, false, null)]
  public LocId? UnstickPopupSuccess;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? StuckTo;
  [DataField(null, false, 1, false, false, null)]
  public LocId VerbText = (LocId) "comp-sticky-unstick-verb-text";
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier VerbIcon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/eject.svg.192dpi.png"));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StickyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StickyComponent) target1;
    if (serialization.TryCustomCopy<StickyComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StickDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.StickDelay, hookCtx, context);
    target.StickDelay = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUnstick, ref target5, hookCtx, false, context))
      target5 = this.CanUnstick;
    target.CanUnstick = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnstickDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.UnstickDelay, hookCtx, context);
    target.UnstickDelay = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.StickPopupStart, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.StickPopupStart, hookCtx, context);
    target.StickPopupStart = target7;
    LocId? target8 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.StickPopupSuccess, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId?>(this.StickPopupSuccess, hookCtx, context);
    target.StickPopupSuccess = target8;
    LocId? target9 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UnstickPopupStart, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId?>(this.UnstickPopupStart, hookCtx, context);
    target.UnstickPopupStart = target9;
    LocId? target10 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.UnstickPopupSuccess, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<LocId?>(this.UnstickPopupSuccess, hookCtx, context);
    target.UnstickPopupSuccess = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.StuckTo, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.StuckTo, hookCtx, context);
    target.StuckTo = target11;
    LocId target12 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VerbText, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<LocId>(this.VerbText, hookCtx, context);
    target.VerbText = target12;
    SpriteSpecifier target13 = (SpriteSpecifier) null;
    if (this.VerbIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.VerbIcon, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SpriteSpecifier>(this.VerbIcon, hookCtx, context);
    target.VerbIcon = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StickyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StickyComponent target1 = (StickyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StickyComponent target1 = (StickyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    StickyComponent target1 = (StickyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StickyComponent Component.Instantiate() => new StickyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StickyComponent_AutoState : IComponentState
  {
    public NetEntity? StuckTo;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StickyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StickyComponent, ComponentGetState>(new ComponentEventRefHandler<StickyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StickyComponent, ComponentHandleState>(new ComponentEventRefHandler<StickyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, StickyComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new StickyComponent.StickyComponent_AutoState()
      {
        StuckTo = this.GetNetEntity(component.StuckTo)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StickyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StickyComponent.StickyComponent_AutoState current))
        return;
      component.StuckTo = this.EnsureEntity<StickyComponent>(current.StuckTo, uid);
    }
  }
}
