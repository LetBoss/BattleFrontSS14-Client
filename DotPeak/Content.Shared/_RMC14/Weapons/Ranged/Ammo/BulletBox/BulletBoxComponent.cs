// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox.BulletBoxComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (BulletBoxSystem)})]
public sealed class BulletBoxComponent : 
  Component,
  ISerializationGenerated<BulletBoxComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Amount = 600;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Max = 600;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId BulletType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(1.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BulletBoxComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BulletBoxComponent) target1;
    if (serialization.TryCustomCopy<BulletBoxComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BulletType, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.BulletType, hookCtx, context);
    target.BulletType = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BulletBoxComponent target,
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
    BulletBoxComponent target1 = (BulletBoxComponent) target;
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
    BulletBoxComponent target1 = (BulletBoxComponent) target;
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
    BulletBoxComponent target1 = (BulletBoxComponent) target;
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
  virtual BulletBoxComponent Component.Instantiate() => new BulletBoxComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BulletBoxComponent_AutoState : IComponentState
  {
    public int Amount;
    public int Max;
    public EntProtoId BulletType;
    public TimeSpan Delay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BulletBoxComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BulletBoxComponent, ComponentGetState>(new ComponentEventRefHandler<BulletBoxComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BulletBoxComponent, ComponentHandleState>(new ComponentEventRefHandler<BulletBoxComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BulletBoxComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BulletBoxComponent.BulletBoxComponent_AutoState()
      {
        Amount = component.Amount,
        Max = component.Max,
        BulletType = component.BulletType,
        Delay = component.Delay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BulletBoxComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BulletBoxComponent.BulletBoxComponent_AutoState current))
        return;
      component.Amount = current.Amount;
      component.Max = current.Max;
      component.BulletType = current.BulletType;
      component.Delay = current.Delay;
    }
  }
}
