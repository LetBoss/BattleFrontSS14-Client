// Decompiled with JetBrains decompiler
// Type: Content.Shared.Blocking.BlockingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Blocking;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class BlockingComponent : 
  Component,
  ISerializationGenerated<BlockingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsBlocking;
  public const string BlockFixtureID = "blocking-active";
  [DataField(null, false, 1, false, false, null)]
  public IPhysShape Shape;
  [DataField("passiveBlockModifier", false, 1, true, false, null)]
  public DamageModifierSet PassiveBlockDamageModifer;
  [DataField("activeBlockModifier", false, 1, true, false, null)]
  public DamageModifierSet ActiveBlockDamageModifier;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId BlockingToggleAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? BlockingToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BlockSound;
  [DataField(null, false, 1, false, false, null)]
  public float PassiveBlockFraction;
  [DataField(null, false, 1, false, false, null)]
  public float ActiveBlockFraction;

  public BlockingComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/block_metal1.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.25f));
    this.BlockSound = (SoundSpecifier) soundPathSpecifier;
    this.PassiveBlockFraction = 0.5f;
    this.ActiveBlockFraction = 1f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BlockingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BlockingComponent) component;
    if (serialization.TryCustomCopy<BlockingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context, false);
    target.User = nullable1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBlocking, ref flag, hookCtx, false, context))
      flag = this.IsBlocking;
    target.IsBlocking = flag;
    IPhysShape iphysShape = (IPhysShape) null;
    if (this.Shape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.Shape, ref iphysShape, hookCtx, true, context))
      iphysShape = serialization.CreateCopy<IPhysShape>(this.Shape, hookCtx, context, false);
    target.Shape = iphysShape;
    DamageModifierSet damageModifierSet1 = (DamageModifierSet) null;
    if (this.PassiveBlockDamageModifer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.PassiveBlockDamageModifer, ref damageModifierSet1, hookCtx, true, context))
    {
      if (this.PassiveBlockDamageModifer == null)
        damageModifierSet1 = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.PassiveBlockDamageModifer, ref damageModifierSet1, hookCtx, context, true);
    }
    target.PassiveBlockDamageModifer = damageModifierSet1;
    DamageModifierSet damageModifierSet2 = (DamageModifierSet) null;
    if (this.ActiveBlockDamageModifier == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.ActiveBlockDamageModifier, ref damageModifierSet2, hookCtx, true, context))
    {
      if (this.ActiveBlockDamageModifier == null)
        damageModifierSet2 = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.ActiveBlockDamageModifier, ref damageModifierSet2, hookCtx, context, true);
    }
    target.ActiveBlockDamageModifier = damageModifierSet2;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BlockingToggleAction, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.BlockingToggleAction, hookCtx, context, false);
    target.BlockingToggleAction = entProtoId;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BlockingToggleActionEntity, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.BlockingToggleActionEntity, hookCtx, context, false);
    target.BlockingToggleActionEntity = nullable2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.BlockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BlockSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.BlockSound, hookCtx, context, false);
    target.BlockSound = soundSpecifier;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PassiveBlockFraction, ref num1, hookCtx, false, context))
      num1 = this.PassiveBlockFraction;
    target.PassiveBlockFraction = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ActiveBlockFraction, ref num2, hookCtx, false, context))
      num2 = this.ActiveBlockFraction;
    target.ActiveBlockFraction = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BlockingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BlockingComponent target1 = (BlockingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BlockingComponent target1 = (BlockingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BlockingComponent target1 = (BlockingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BlockingComponent Component.Instantiate() => new BlockingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BlockingComponent_AutoState : IComponentState
  {
    public NetEntity? User;
    public bool IsBlocking;
    public NetEntity? BlockingToggleActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BlockingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BlockingComponent, ComponentGetState>(new ComponentEventRefHandler<BlockingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BlockingComponent, ComponentHandleState>(new ComponentEventRefHandler<BlockingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, BlockingComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new BlockingComponent.BlockingComponent_AutoState()
      {
        User = this.GetNetEntity(component.User, (MetaDataComponent) null),
        IsBlocking = component.IsBlocking,
        BlockingToggleActionEntity = this.GetNetEntity(component.BlockingToggleActionEntity, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BlockingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is BlockingComponent.BlockingComponent_AutoState current))
        return;
      component.User = this.EnsureEntity<BlockingComponent>(current.User, uid);
      component.IsBlocking = current.IsBlocking;
      component.BlockingToggleActionEntity = this.EnsureEntity<BlockingComponent>(current.BlockingToggleActionEntity, uid);
    }
  }
}
