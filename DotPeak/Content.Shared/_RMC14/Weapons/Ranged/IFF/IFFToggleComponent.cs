// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.IFF.IFFToggleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class IFFToggleComponent : 
  Component,
  ISerializationGenerated<IFFToggleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequireIDLock;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ChangeStats;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<SelectiveFire, SelectiveFireModifierSet> IFFModifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>()
  {
    {
      SelectiveFire.SemiAuto,
      new SelectiveFireModifierSet(0.0f, 10.0, false, 2.0, new int?(6))
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<SelectiveFire, SelectiveFireModifierSet> BaseModifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SelectiveFire BaseFireModes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SelectiveFire IFFFireModes = SelectiveFire.SemiAuto;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionID = (EntProtoId) "RMCActionToggleIFF";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi EnabledIcon = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Actions/iff_toggle_actions.rsi"), "iff_toggle_on");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi DisabledIcon = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Actions/iff_toggle_actions.rsi"), "iff_toggle_off");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IFFToggleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IFFToggleComponent) target1;
    if (serialization.TryCustomCopy<IFFToggleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireIDLock, ref target3, hookCtx, false, context))
      target3 = this.RequireIDLock;
    target.RequireIDLock = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChangeStats, ref target4, hookCtx, false, context))
      target4 = this.ChangeStats;
    target.ChangeStats = target4;
    Dictionary<SelectiveFire, SelectiveFireModifierSet> target5 = (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null;
    if (this.IFFModifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.IFFModifiers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.IFFModifiers, hookCtx, context);
    target.IFFModifiers = target5;
    Dictionary<SelectiveFire, SelectiveFireModifierSet> target6 = (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null;
    if (this.BaseModifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.BaseModifiers, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<SelectiveFire, SelectiveFireModifierSet>>(this.BaseModifiers, hookCtx, context);
    target.BaseModifiers = target6;
    SelectiveFire target7 = SelectiveFire.Invalid;
    if (!serialization.TryCustomCopy<SelectiveFire>(this.BaseFireModes, ref target7, hookCtx, false, context))
      target7 = this.BaseFireModes;
    target.BaseFireModes = target7;
    SelectiveFire target8 = SelectiveFire.Invalid;
    if (!serialization.TryCustomCopy<SelectiveFire>(this.IFFFireModes, ref target8, hookCtx, false, context))
      target8 = this.IFFFireModes;
    target.IFFFireModes = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionID, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.ActionID, hookCtx, context);
    target.ActionID = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target10;
    SpriteSpecifier.Rsi target11 = (SpriteSpecifier.Rsi) null;
    if (this.EnabledIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.EnabledIcon, ref target11, hookCtx, false, context))
    {
      if (this.EnabledIcon == null)
        target11 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.EnabledIcon, ref target11, hookCtx, context, true);
    }
    target.EnabledIcon = target11;
    SpriteSpecifier.Rsi target12 = (SpriteSpecifier.Rsi) null;
    if (this.DisabledIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.DisabledIcon, ref target12, hookCtx, false, context))
    {
      if (this.DisabledIcon == null)
        target12 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.DisabledIcon, ref target12, hookCtx, context, true);
    }
    target.DisabledIcon = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IFFToggleComponent target,
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
    IFFToggleComponent target1 = (IFFToggleComponent) target;
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
    IFFToggleComponent target1 = (IFFToggleComponent) target;
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
    IFFToggleComponent target1 = (IFFToggleComponent) target;
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
  virtual IFFToggleComponent Component.Instantiate() => new IFFToggleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IFFToggleComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public bool RequireIDLock;
    public bool ChangeStats;
    public Dictionary<SelectiveFire, SelectiveFireModifierSet> IFFModifiers;
    public Dictionary<SelectiveFire, SelectiveFireModifierSet> BaseModifiers;
    public SelectiveFire BaseFireModes;
    public SelectiveFire IFFFireModes;
    public EntProtoId ActionID;
    public NetEntity? Action;
    public SpriteSpecifier.Rsi EnabledIcon;
    public SpriteSpecifier.Rsi DisabledIcon;
    public SoundSpecifier ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IFFToggleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IFFToggleComponent, ComponentGetState>(new ComponentEventRefHandler<IFFToggleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IFFToggleComponent, ComponentHandleState>(new ComponentEventRefHandler<IFFToggleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IFFToggleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IFFToggleComponent.IFFToggleComponent_AutoState()
      {
        Enabled = component.Enabled,
        RequireIDLock = component.RequireIDLock,
        ChangeStats = component.ChangeStats,
        IFFModifiers = component.IFFModifiers,
        BaseModifiers = component.BaseModifiers,
        BaseFireModes = component.BaseFireModes,
        IFFFireModes = component.IFFFireModes,
        ActionID = component.ActionID,
        Action = this.GetNetEntity(component.Action),
        EnabledIcon = component.EnabledIcon,
        DisabledIcon = component.DisabledIcon,
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IFFToggleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IFFToggleComponent.IFFToggleComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.RequireIDLock = current.RequireIDLock;
      component.ChangeStats = current.ChangeStats;
      component.IFFModifiers = current.IFFModifiers == null ? (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null : new Dictionary<SelectiveFire, SelectiveFireModifierSet>((IDictionary<SelectiveFire, SelectiveFireModifierSet>) current.IFFModifiers);
      component.BaseModifiers = current.BaseModifiers == null ? (Dictionary<SelectiveFire, SelectiveFireModifierSet>) null : new Dictionary<SelectiveFire, SelectiveFireModifierSet>((IDictionary<SelectiveFire, SelectiveFireModifierSet>) current.BaseModifiers);
      component.BaseFireModes = current.BaseFireModes;
      component.IFFFireModes = current.IFFFireModes;
      component.ActionID = current.ActionID;
      component.Action = this.EnsureEntity<IFFToggleComponent>(current.Action, uid);
      component.EnabledIcon = current.EnabledIcon;
      component.DisabledIcon = current.DisabledIcon;
      component.ToggleSound = current.ToggleSound;
    }
  }
}
