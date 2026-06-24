// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Flag.PlantableFlagComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Flag;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (PlantableFlagSystem)})]
public sealed class PlantableFlagComponent : 
  Component,
  ISerializationGenerated<PlantableFlagComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RaiseStartSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/flag_raising.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RaiseEndSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/flag_raised.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RaisedCombatSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RaisedCombatAlliesSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LowerStartSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/flag_lowering.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AlliesRequired = 14;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int AlliesRange = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 DeployOffset = new Vector2(0.0f, 0.5f);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlantableFlagComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantableFlagComponent) target1;
    if (serialization.TryCustomCopy<PlantableFlagComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RaiseStartSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.RaiseStartSound, hookCtx, context);
    target.RaiseStartSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RaiseEndSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.RaiseEndSound, hookCtx, context);
    target.RaiseEndSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RaisedCombatSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.RaisedCombatSound, hookCtx, context);
    target.RaisedCombatSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RaisedCombatAlliesSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.RaisedCombatAlliesSound, hookCtx, context);
    target.RaisedCombatAlliesSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LowerStartSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.LowerStartSound, hookCtx, context);
    target.LowerStartSound = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.AlliesRequired, ref target8, hookCtx, false, context))
      target8 = this.AlliesRequired;
    target.AlliesRequired = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.AlliesRange, ref target9, hookCtx, false, context))
      target9 = this.AlliesRange;
    target.AlliesRange = target9;
    Vector2 target10 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.DeployOffset, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2>(this.DeployOffset, hookCtx, context);
    target.DeployOffset = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlantableFlagComponent target,
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
    PlantableFlagComponent target1 = (PlantableFlagComponent) target;
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
    PlantableFlagComponent target1 = (PlantableFlagComponent) target;
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
    PlantableFlagComponent target1 = (PlantableFlagComponent) target;
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
  virtual PlantableFlagComponent Component.Instantiate() => new PlantableFlagComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PlantableFlagComponent_AutoState : IComponentState
  {
    public TimeSpan Delay;
    public SoundSpecifier? RaiseStartSound;
    public SoundSpecifier? RaiseEndSound;
    public SoundSpecifier? RaisedCombatSound;
    public SoundSpecifier? RaisedCombatAlliesSound;
    public SoundSpecifier? LowerStartSound;
    public int AlliesRequired;
    public int AlliesRange;
    public Vector2 DeployOffset;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PlantableFlagComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PlantableFlagComponent, ComponentGetState>(new ComponentEventRefHandler<PlantableFlagComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PlantableFlagComponent, ComponentHandleState>(new ComponentEventRefHandler<PlantableFlagComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PlantableFlagComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PlantableFlagComponent.PlantableFlagComponent_AutoState()
      {
        Delay = component.Delay,
        RaiseStartSound = component.RaiseStartSound,
        RaiseEndSound = component.RaiseEndSound,
        RaisedCombatSound = component.RaisedCombatSound,
        RaisedCombatAlliesSound = component.RaisedCombatAlliesSound,
        LowerStartSound = component.LowerStartSound,
        AlliesRequired = component.AlliesRequired,
        AlliesRange = component.AlliesRange,
        DeployOffset = component.DeployOffset
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PlantableFlagComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PlantableFlagComponent.PlantableFlagComponent_AutoState current))
        return;
      component.Delay = current.Delay;
      component.RaiseStartSound = current.RaiseStartSound;
      component.RaiseEndSound = current.RaiseEndSound;
      component.RaisedCombatSound = current.RaisedCombatSound;
      component.RaisedCombatAlliesSound = current.RaisedCombatAlliesSound;
      component.LowerStartSound = current.LowerStartSound;
      component.AlliesRequired = current.AlliesRequired;
      component.AlliesRange = current.AlliesRange;
      component.DeployOffset = current.DeployOffset;
    }
  }
}
