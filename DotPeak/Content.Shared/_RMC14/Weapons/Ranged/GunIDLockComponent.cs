// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunIDLockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GunIDLockComponent : 
  Component,
  ISerializationGenerated<GunIDLockComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityUid User = EntityUid.Invalid;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Locked = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionID = (EntProtoId) "RMCActionToggleIDLock";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi LockedIcon = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Actions/id_lock_actions.rsi"), "id_lock_locked");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi UnlockedIcon = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Actions/id_lock_actions.rsi"), "id_lock_unlocked");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunIDLockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunIDLockComponent) target1;
    if (serialization.TryCustomCopy<GunIDLockComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionID, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionID, hookCtx, context);
    target.ActionID = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (this.LockedIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.LockedIcon, ref target3, hookCtx, false, context))
    {
      if (this.LockedIcon == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.LockedIcon, ref target3, hookCtx, context, true);
    }
    target.LockedIcon = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (this.UnlockedIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.UnlockedIcon, ref target4, hookCtx, false, context))
    {
      if (this.UnlockedIcon == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.UnlockedIcon, ref target4, hookCtx, context, true);
    }
    target.UnlockedIcon = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunIDLockComponent target,
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
    GunIDLockComponent target1 = (GunIDLockComponent) target;
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
    GunIDLockComponent target1 = (GunIDLockComponent) target;
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
    GunIDLockComponent target1 = (GunIDLockComponent) target;
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
  virtual GunIDLockComponent Component.Instantiate() => new GunIDLockComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunIDLockComponent_AutoState : IComponentState
  {
    public NetEntity User;
    public bool Locked;
    public EntProtoId ActionID;
    public SpriteSpecifier.Rsi LockedIcon;
    public SpriteSpecifier.Rsi UnlockedIcon;
    public NetEntity? Action;
    public SoundSpecifier ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunIDLockComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunIDLockComponent, ComponentGetState>(new ComponentEventRefHandler<GunIDLockComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunIDLockComponent, ComponentHandleState>(new ComponentEventRefHandler<GunIDLockComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunIDLockComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunIDLockComponent.GunIDLockComponent_AutoState()
      {
        User = this.GetNetEntity(component.User),
        Locked = component.Locked,
        ActionID = component.ActionID,
        LockedIcon = component.LockedIcon,
        UnlockedIcon = component.UnlockedIcon,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunIDLockComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunIDLockComponent.GunIDLockComponent_AutoState current))
        return;
      component.User = this.EnsureEntity<GunIDLockComponent>(current.User, uid);
      component.Locked = current.Locked;
      component.ActionID = current.ActionID;
      component.LockedIcon = current.LockedIcon;
      component.UnlockedIcon = current.UnlockedIcon;
      component.Action = this.EnsureEntity<GunIDLockComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
    }
  }
}
