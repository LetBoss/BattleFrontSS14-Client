// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hedgehog.XenoShardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hedgehog;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoShardSystem)})]
public sealed class XenoShardComponent : 
  Component,
  ISerializationGenerated<XenoShardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ArmorPerShard = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShardsPerArmorBonus = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShardsOnDamage = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedModifier = 0.45f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? SpikeShedCooldownEnd;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoShardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoShardComponent) target1;
    if (serialization.TryCustomCopy<XenoShardComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ArmorPerShard, ref target2, hookCtx, false, context))
      target2 = this.ArmorPerShard;
    target.ArmorPerShard = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShardsPerArmorBonus, ref target3, hookCtx, false, context))
      target3 = this.ShardsPerArmorBonus;
    target.ShardsPerArmorBonus = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShardsOnDamage, ref target4, hookCtx, false, context))
      target4 = this.ShardsOnDamage;
    target.ShardsOnDamage = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedModifier, ref target5, hookCtx, false, context))
      target5 = this.SpeedModifier;
    target.SpeedModifier = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.SpikeShedCooldownEnd, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.SpikeShedCooldownEnd, hookCtx, context);
    target.SpikeShedCooldownEnd = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoShardComponent target,
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
    XenoShardComponent target1 = (XenoShardComponent) target;
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
    XenoShardComponent target1 = (XenoShardComponent) target;
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
    XenoShardComponent target1 = (XenoShardComponent) target;
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
  virtual XenoShardComponent Component.Instantiate() => new XenoShardComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoShardComponent_AutoState : IComponentState
  {
    public float ArmorPerShard;
    public int ShardsPerArmorBonus;
    public int ShardsOnDamage;
    public float SpeedModifier;
    public TimeSpan? SpikeShedCooldownEnd;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoShardComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoShardComponent, ComponentGetState>(new ComponentEventRefHandler<XenoShardComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoShardComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoShardComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoShardComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoShardComponent.XenoShardComponent_AutoState()
      {
        ArmorPerShard = component.ArmorPerShard,
        ShardsPerArmorBonus = component.ShardsPerArmorBonus,
        ShardsOnDamage = component.ShardsOnDamage,
        SpeedModifier = component.SpeedModifier,
        SpikeShedCooldownEnd = component.SpikeShedCooldownEnd
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoShardComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoShardComponent.XenoShardComponent_AutoState current))
        return;
      component.ArmorPerShard = current.ArmorPerShard;
      component.ShardsPerArmorBonus = current.ShardsPerArmorBonus;
      component.ShardsOnDamage = current.ShardsOnDamage;
      component.SpeedModifier = current.SpeedModifier;
      component.SpikeShedCooldownEnd = current.SpikeShedCooldownEnd;
    }
  }
}
