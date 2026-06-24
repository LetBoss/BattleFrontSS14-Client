// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.ParasiteAIComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoParasiteSystem)})]
public sealed class ParasiteAIComponent : 
  Component,
  ISerializationGenerated<ParasiteAIComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ParasiteMode Mode;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextActiveTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? DeathTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LifeTime = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  public int InitialJumps = 2;
  [DataField(null, false, 1, false, false, null)]
  public int JumpsLeft = 2;
  [DataField(null, false, 1, false, false, null)]
  public int MaxSurroundingParas = 2;
  [DataField(null, false, 1, false, false, null)]
  public int MaxInfectRange = 3;
  [DataField(null, false, 1, false, false, null)]
  public float IdleChance = 0.15f;
  [DataField(null, false, 1, false, false, null)]
  public int MinIdleTime = 5;
  [DataField(null, false, 1, false, false, null)]
  public int MaxIdleTime = 15;
  [DataField(null, false, 1, false, false, null)]
  public string RestAction = "ActionXenoRest";
  [DataField(null, false, 1, false, false, null)]
  public float RangeCheck = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public float CannibalizeCheck = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan JumpTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? NextJump;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParasiteAIComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParasiteAIComponent) target1;
    if (serialization.TryCustomCopy<ParasiteAIComponent>(this, ref target, hookCtx, false, context))
      return;
    ParasiteMode target2 = ParasiteMode.Idle;
    if (!serialization.TryCustomCopy<ParasiteMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextActiveTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextActiveTime, hookCtx, context);
    target.NextActiveTime = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DeathTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.DeathTime, hookCtx, context);
    target.DeathTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LifeTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.LifeTime, hookCtx, context);
    target.LifeTime = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.InitialJumps, ref target6, hookCtx, false, context))
      target6 = this.InitialJumps;
    target.InitialJumps = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.JumpsLeft, ref target7, hookCtx, false, context))
      target7 = this.JumpsLeft;
    target.JumpsLeft = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxSurroundingParas, ref target8, hookCtx, false, context))
      target8 = this.MaxSurroundingParas;
    target.MaxSurroundingParas = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxInfectRange, ref target9, hookCtx, false, context))
      target9 = this.MaxInfectRange;
    target.MaxInfectRange = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdleChance, ref target10, hookCtx, false, context))
      target10 = this.IdleChance;
    target.IdleChance = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinIdleTime, ref target11, hookCtx, false, context))
      target11 = this.MinIdleTime;
    target.MinIdleTime = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxIdleTime, ref target12, hookCtx, false, context))
      target12 = this.MaxIdleTime;
    target.MaxIdleTime = target12;
    string target13 = (string) null;
    if (this.RestAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RestAction, ref target13, hookCtx, false, context))
      target13 = this.RestAction;
    target.RestAction = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeCheck, ref target14, hookCtx, false, context))
      target14 = this.RangeCheck;
    target.RangeCheck = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CannibalizeCheck, ref target15, hookCtx, false, context))
      target15 = this.CannibalizeCheck;
    target.CannibalizeCheck = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JumpTime, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.JumpTime, hookCtx, context);
    target.JumpTime = target16;
    TimeSpan? target17 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextJump, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan?>(this.NextJump, hookCtx, context);
    target.NextJump = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParasiteAIComponent target,
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
    ParasiteAIComponent target1 = (ParasiteAIComponent) target;
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
    ParasiteAIComponent target1 = (ParasiteAIComponent) target;
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
    ParasiteAIComponent target1 = (ParasiteAIComponent) target;
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
  virtual ParasiteAIComponent Component.Instantiate() => new ParasiteAIComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ParasiteAIComponent_AutoState : IComponentState
  {
    public ParasiteMode Mode;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParasiteAIComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ParasiteAIComponent, ComponentGetState>(new ComponentEventRefHandler<ParasiteAIComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ParasiteAIComponent, ComponentHandleState>(new ComponentEventRefHandler<ParasiteAIComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ParasiteAIComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ParasiteAIComponent.ParasiteAIComponent_AutoState()
      {
        Mode = component.Mode
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ParasiteAIComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ParasiteAIComponent.ParasiteAIComponent_AutoState current))
        return;
      component.Mode = current.Mode;
    }
  }
}
