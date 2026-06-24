// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableToggleableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableToggleableSystem)})]
public sealed class AttachableToggleableComponent : 
  Component,
  ISerializationGenerated<AttachableToggleableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedHand;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoInterrupt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakOnMove;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakOnRotate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakOnFullRotate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakOnDrop;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SlowOnBreak;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WieldedOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WieldedUseOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HeldOnlyActivate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UserOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UseDelay = TimeSpan.FromSeconds(0.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? DeactivateDoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoAfterNeedHand = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoAfterBreakOnMove = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AttachableInstantToggleConditions InstantToggle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SupercedeHolder;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AttachedOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ActivateSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_activate.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeactivateSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_deactivate.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowTogglePopup = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId ActivatePopupText = new LocId("attachable-popup-activate-generic");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId DeactivatePopupText = new LocId("attachable-popup-deactivate-generic");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ActionId = "CMActionToggleAttachable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ActionName = "Toggle Attachable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ActionDesc = "Toggle an attachable. If you're seeing this, someone forgot to set the description properly.";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? ActionsToRelayWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Objects/Weapons/Guns/Attachments/rail.rsi"), "flashlight");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier? IconActive;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Attached;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ActivateOnMove = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableToggleableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableToggleableComponent) target1;
    if (serialization.TryCustomCopy<AttachableToggleableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedHand, ref target3, hookCtx, false, context))
      target3 = this.NeedHand;
    target.NeedHand = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoInterrupt, ref target4, hookCtx, false, context))
      target4 = this.DoInterrupt;
    target.DoInterrupt = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnMove, ref target5, hookCtx, false, context))
      target5 = this.BreakOnMove;
    target.BreakOnMove = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnRotate, ref target6, hookCtx, false, context))
      target6 = this.BreakOnRotate;
    target.BreakOnRotate = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnFullRotate, ref target7, hookCtx, false, context))
      target7 = this.BreakOnFullRotate;
    target.BreakOnFullRotate = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnDrop, ref target8, hookCtx, false, context))
      target8 = this.BreakOnDrop;
    target.BreakOnDrop = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlowOnBreak, ref target9, hookCtx, false, context))
      target9 = this.SlowOnBreak;
    target.SlowOnBreak = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.WieldedOnly, ref target10, hookCtx, false, context))
      target10 = this.WieldedOnly;
    target.WieldedOnly = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.WieldedUseOnly, ref target11, hookCtx, false, context))
      target11 = this.WieldedUseOnly;
    target.WieldedUseOnly = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.HeldOnlyActivate, ref target12, hookCtx, false, context))
      target12 = this.HeldOnlyActivate;
    target.HeldOnlyActivate = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.UserOnly, ref target13, hookCtx, false, context))
      target13 = this.UserOnly;
    target.UserOnly = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UseDelay, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.UseDelay, hookCtx, context);
    target.UseDelay = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DoAfter, ref target15, hookCtx, false, context))
      target15 = this.DoAfter;
    target.DoAfter = target15;
    float? target16 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.DeactivateDoAfter, ref target16, hookCtx, false, context))
      target16 = this.DeactivateDoAfter;
    target.DeactivateDoAfter = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoAfterNeedHand, ref target17, hookCtx, false, context))
      target17 = this.DoAfterNeedHand;
    target.DoAfterNeedHand = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoAfterBreakOnMove, ref target18, hookCtx, false, context))
      target18 = this.DoAfterBreakOnMove;
    target.DoAfterBreakOnMove = target18;
    AttachableInstantToggleConditions target19 = AttachableInstantToggleConditions.None;
    if (!serialization.TryCustomCopy<AttachableInstantToggleConditions>(this.InstantToggle, ref target19, hookCtx, false, context))
      target19 = this.InstantToggle;
    target.InstantToggle = target19;
    bool target20 = false;
    if (!serialization.TryCustomCopy<bool>(this.SupercedeHolder, ref target20, hookCtx, false, context))
      target20 = this.SupercedeHolder;
    target.SupercedeHolder = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.AttachedOnly, ref target21, hookCtx, false, context))
      target21 = this.AttachedOnly;
    target.AttachedOnly = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ActivateSound, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.ActivateSound, hookCtx, context);
    target.ActivateSound = target22;
    SoundSpecifier target23 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeactivateSound, ref target23, hookCtx, true, context))
      target23 = serialization.CreateCopy<SoundSpecifier>(this.DeactivateSound, hookCtx, context);
    target.DeactivateSound = target23;
    bool target24 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowTogglePopup, ref target24, hookCtx, false, context))
      target24 = this.ShowTogglePopup;
    target.ShowTogglePopup = target24;
    LocId target25 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ActivatePopupText, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<LocId>(this.ActivatePopupText, hookCtx, context);
    target.ActivatePopupText = target25;
    LocId target26 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DeactivatePopupText, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<LocId>(this.DeactivatePopupText, hookCtx, context);
    target.DeactivatePopupText = target26;
    EntityUid? target27 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target27, hookCtx, false, context))
      target27 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target27;
    string target28 = (string) null;
    if (this.ActionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionId, ref target28, hookCtx, false, context))
      target28 = this.ActionId;
    target.ActionId = target28;
    string target29 = (string) null;
    if (this.ActionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionName, ref target29, hookCtx, false, context))
      target29 = this.ActionName;
    target.ActionName = target29;
    string target30 = (string) null;
    if (this.ActionDesc == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionDesc, ref target30, hookCtx, false, context))
      target30 = this.ActionDesc;
    target.ActionDesc = target30;
    EntityWhitelist target31 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ActionsToRelayWhitelist, ref target31, hookCtx, false, context))
    {
      if (this.ActionsToRelayWhitelist == null)
        target31 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ActionsToRelayWhitelist, ref target31, hookCtx, context);
    }
    target.ActionsToRelayWhitelist = target31;
    SpriteSpecifier target32 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target32, hookCtx, true, context))
      target32 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target32;
    SpriteSpecifier target33 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.IconActive, ref target33, hookCtx, true, context))
      target33 = serialization.CreateCopy<SpriteSpecifier>(this.IconActive, hookCtx, context);
    target.IconActive = target33;
    bool target34 = false;
    if (!serialization.TryCustomCopy<bool>(this.Attached, ref target34, hookCtx, false, context))
      target34 = this.Attached;
    target.Attached = target34;
    bool target35 = false;
    if (!serialization.TryCustomCopy<bool>(this.ActivateOnMove, ref target35, hookCtx, false, context))
      target35 = this.ActivateOnMove;
    target.ActivateOnMove = target35;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableToggleableComponent target,
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
    AttachableToggleableComponent target1 = (AttachableToggleableComponent) target;
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
    AttachableToggleableComponent target1 = (AttachableToggleableComponent) target;
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
    AttachableToggleableComponent target1 = (AttachableToggleableComponent) target;
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
  virtual AttachableToggleableComponent Component.Instantiate()
  {
    return new AttachableToggleableComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableToggleableComponent_AutoState : IComponentState
  {
    public bool Active;
    public bool NeedHand;
    public bool DoInterrupt;
    public bool BreakOnMove;
    public bool BreakOnRotate;
    public bool BreakOnFullRotate;
    public bool BreakOnDrop;
    public bool SlowOnBreak;
    public bool WieldedOnly;
    public bool WieldedUseOnly;
    public bool HeldOnlyActivate;
    public bool UserOnly;
    public TimeSpan UseDelay;
    public float DoAfter;
    public float? DeactivateDoAfter;
    public bool DoAfterNeedHand;
    public bool DoAfterBreakOnMove;
    public AttachableInstantToggleConditions InstantToggle;
    public bool SupercedeHolder;
    public bool AttachedOnly;
    public SoundSpecifier? ActivateSound;
    public SoundSpecifier? DeactivateSound;
    public bool ShowTogglePopup;
    public LocId ActivatePopupText;
    public LocId DeactivatePopupText;
    public NetEntity? Action;
    public string ActionId;
    public string ActionName;
    public string ActionDesc;
    public EntityWhitelist? ActionsToRelayWhitelist;
    public SpriteSpecifier Icon;
    public SpriteSpecifier? IconActive;
    public bool Attached;
    public bool ActivateOnMove;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableToggleableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableToggleableComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableToggleableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableToggleableComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableToggleableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableToggleableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableToggleableComponent.AttachableToggleableComponent_AutoState()
      {
        Active = component.Active,
        NeedHand = component.NeedHand,
        DoInterrupt = component.DoInterrupt,
        BreakOnMove = component.BreakOnMove,
        BreakOnRotate = component.BreakOnRotate,
        BreakOnFullRotate = component.BreakOnFullRotate,
        BreakOnDrop = component.BreakOnDrop,
        SlowOnBreak = component.SlowOnBreak,
        WieldedOnly = component.WieldedOnly,
        WieldedUseOnly = component.WieldedUseOnly,
        HeldOnlyActivate = component.HeldOnlyActivate,
        UserOnly = component.UserOnly,
        UseDelay = component.UseDelay,
        DoAfter = component.DoAfter,
        DeactivateDoAfter = component.DeactivateDoAfter,
        DoAfterNeedHand = component.DoAfterNeedHand,
        DoAfterBreakOnMove = component.DoAfterBreakOnMove,
        InstantToggle = component.InstantToggle,
        SupercedeHolder = component.SupercedeHolder,
        AttachedOnly = component.AttachedOnly,
        ActivateSound = component.ActivateSound,
        DeactivateSound = component.DeactivateSound,
        ShowTogglePopup = component.ShowTogglePopup,
        ActivatePopupText = component.ActivatePopupText,
        DeactivatePopupText = component.DeactivatePopupText,
        Action = this.GetNetEntity(component.Action),
        ActionId = component.ActionId,
        ActionName = component.ActionName,
        ActionDesc = component.ActionDesc,
        ActionsToRelayWhitelist = component.ActionsToRelayWhitelist,
        Icon = component.Icon,
        IconActive = component.IconActive,
        Attached = component.Attached,
        ActivateOnMove = component.ActivateOnMove
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableToggleableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableToggleableComponent.AttachableToggleableComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.NeedHand = current.NeedHand;
      component.DoInterrupt = current.DoInterrupt;
      component.BreakOnMove = current.BreakOnMove;
      component.BreakOnRotate = current.BreakOnRotate;
      component.BreakOnFullRotate = current.BreakOnFullRotate;
      component.BreakOnDrop = current.BreakOnDrop;
      component.SlowOnBreak = current.SlowOnBreak;
      component.WieldedOnly = current.WieldedOnly;
      component.WieldedUseOnly = current.WieldedUseOnly;
      component.HeldOnlyActivate = current.HeldOnlyActivate;
      component.UserOnly = current.UserOnly;
      component.UseDelay = current.UseDelay;
      component.DoAfter = current.DoAfter;
      component.DeactivateDoAfter = current.DeactivateDoAfter;
      component.DoAfterNeedHand = current.DoAfterNeedHand;
      component.DoAfterBreakOnMove = current.DoAfterBreakOnMove;
      component.InstantToggle = current.InstantToggle;
      component.SupercedeHolder = current.SupercedeHolder;
      component.AttachedOnly = current.AttachedOnly;
      component.ActivateSound = current.ActivateSound;
      component.DeactivateSound = current.DeactivateSound;
      component.ShowTogglePopup = current.ShowTogglePopup;
      component.ActivatePopupText = current.ActivatePopupText;
      component.DeactivatePopupText = current.DeactivatePopupText;
      component.Action = this.EnsureEntity<AttachableToggleableComponent>(current.Action, uid);
      component.ActionId = current.ActionId;
      component.ActionName = current.ActionName;
      component.ActionDesc = current.ActionDesc;
      component.ActionsToRelayWhitelist = current.ActionsToRelayWhitelist;
      component.Icon = current.Icon;
      component.IconActive = current.IconActive;
      component.Attached = current.Attached;
      component.ActivateOnMove = current.ActivateOnMove;
    }
  }
}
