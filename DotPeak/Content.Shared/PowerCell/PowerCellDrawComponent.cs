// Decompiled with JetBrains decompiler
// Type: Content.Shared.PowerCell.PowerCellDrawComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PowerCell;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class PowerCellDrawComponent : 
  Component,
  ISerializationGenerated<PowerCellDrawComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanDraw;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUse;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public float DrawRate = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float UseRate;
  [DataField("nextUpdate", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextUpdateTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerCellDrawComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PowerCellDrawComponent) target1;
    if (serialization.TryCustomCopy<PowerCellDrawComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDraw, ref target2, hookCtx, false, context))
      target2 = this.CanDraw;
    target.CanDraw = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUse, ref target3, hookCtx, false, context))
      target3 = this.CanUse;
    target.CanUse = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target4, hookCtx, false, context))
      target4 = this.Enabled;
    target.Enabled = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DrawRate, ref target5, hookCtx, false, context))
      target5 = this.DrawRate;
    target.DrawRate = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UseRate, ref target6, hookCtx, false, context))
      target6 = this.UseRate;
    target.UseRate = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdateTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextUpdateTime, hookCtx, context);
    target.NextUpdateTime = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerCellDrawComponent target,
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
    PowerCellDrawComponent target1 = (PowerCellDrawComponent) target;
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
    PowerCellDrawComponent target1 = (PowerCellDrawComponent) target;
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
    PowerCellDrawComponent target1 = (PowerCellDrawComponent) target;
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
  virtual PowerCellDrawComponent Component.Instantiate() => new PowerCellDrawComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PowerCellDrawComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PowerCellDrawComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<PowerCellDrawComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      PowerCellDrawComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdateTime += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PowerCellDrawComponent_AutoState : IComponentState
  {
    public bool CanDraw;
    public bool CanUse;
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PowerCellDrawComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PowerCellDrawComponent, ComponentGetState>(new ComponentEventRefHandler<PowerCellDrawComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PowerCellDrawComponent, ComponentHandleState>(new ComponentEventRefHandler<PowerCellDrawComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      PowerCellDrawComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PowerCellDrawComponent.PowerCellDrawComponent_AutoState()
      {
        CanDraw = component.CanDraw,
        CanUse = component.CanUse,
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PowerCellDrawComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PowerCellDrawComponent.PowerCellDrawComponent_AutoState current))
        return;
      component.CanDraw = current.CanDraw;
      component.CanUse = current.CanUse;
      component.Enabled = current.Enabled;
    }
  }
}
