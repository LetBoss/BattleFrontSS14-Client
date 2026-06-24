// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailSeize.XenoTailSeizeComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.TailSeize;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoTailSeizeComponent : 
  Component,
  ISerializationGenerated<XenoTailSeizeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Projectile = (EntProtoId) "XenoOppressorTailHook";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 30f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/oppressor_tail.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoTailSeizeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoTailSeizeComponent) target1;
    if (serialization.TryCustomCopy<XenoTailSeizeComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Projectile, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Projectile, hookCtx, context);
    target.Projectile = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target3, hookCtx, false, context))
      target3 = this.Speed;
    target.Speed = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoTailSeizeComponent target,
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
    XenoTailSeizeComponent target1 = (XenoTailSeizeComponent) target;
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
    XenoTailSeizeComponent target1 = (XenoTailSeizeComponent) target;
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
    XenoTailSeizeComponent target1 = (XenoTailSeizeComponent) target;
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
  virtual XenoTailSeizeComponent Component.Instantiate() => new XenoTailSeizeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoTailSeizeComponent_AutoState : IComponentState
  {
    public EntProtoId Projectile;
    public float Speed;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoTailSeizeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoTailSeizeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoTailSeizeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoTailSeizeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoTailSeizeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoTailSeizeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoTailSeizeComponent.XenoTailSeizeComponent_AutoState()
      {
        Projectile = component.Projectile,
        Speed = component.Speed,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoTailSeizeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoTailSeizeComponent.XenoTailSeizeComponent_AutoState current))
        return;
      component.Projectile = current.Projectile;
      component.Speed = current.Speed;
      component.Sound = current.Sound;
    }
  }
}
