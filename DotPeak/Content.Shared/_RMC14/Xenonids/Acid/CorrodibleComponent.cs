// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Acid.CorrodibleComponent
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
namespace Content.Shared._RMC14.Xenonids.Acid;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoAcidSystem)})]
public sealed class CorrodibleComponent : 
  Component,
  ISerializationGenerated<CorrodibleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsCorrodible = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeToApply = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Structure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MeltTimeMult = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoAcidStrength MinimumAcidStrength = XenoAcidStrength.Weak;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CorrodibleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CorrodibleComponent) target1;
    if (serialization.TryCustomCopy<CorrodibleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsCorrodible, ref target2, hookCtx, false, context))
      target2 = this.IsCorrodible;
    target.IsCorrodible = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeToApply, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TimeToApply, hookCtx, context);
    target.TimeToApply = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Structure, ref target4, hookCtx, false, context))
      target4 = this.Structure;
    target.Structure = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MeltTimeMult, ref target5, hookCtx, false, context))
      target5 = this.MeltTimeMult;
    target.MeltTimeMult = target5;
    XenoAcidStrength target6 = (XenoAcidStrength) 0;
    if (!serialization.TryCustomCopy<XenoAcidStrength>(this.MinimumAcidStrength, ref target6, hookCtx, false, context))
      target6 = this.MinimumAcidStrength;
    target.MinimumAcidStrength = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CorrodibleComponent target,
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
    CorrodibleComponent target1 = (CorrodibleComponent) target;
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
    CorrodibleComponent target1 = (CorrodibleComponent) target;
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
    CorrodibleComponent target1 = (CorrodibleComponent) target;
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
  virtual CorrodibleComponent Component.Instantiate() => new CorrodibleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CorrodibleComponent_AutoState : IComponentState
  {
    public bool IsCorrodible;
    public TimeSpan TimeToApply;
    public bool Structure;
    public float MeltTimeMult;
    public XenoAcidStrength MinimumAcidStrength;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CorrodibleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CorrodibleComponent, ComponentGetState>(new ComponentEventRefHandler<CorrodibleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CorrodibleComponent, ComponentHandleState>(new ComponentEventRefHandler<CorrodibleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CorrodibleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CorrodibleComponent.CorrodibleComponent_AutoState()
      {
        IsCorrodible = component.IsCorrodible,
        TimeToApply = component.TimeToApply,
        Structure = component.Structure,
        MeltTimeMult = component.MeltTimeMult,
        MinimumAcidStrength = component.MinimumAcidStrength
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CorrodibleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CorrodibleComponent.CorrodibleComponent_AutoState current))
        return;
      component.IsCorrodible = current.IsCorrodible;
      component.TimeToApply = current.TimeToApply;
      component.Structure = current.Structure;
      component.MeltTimeMult = current.MeltTimeMult;
      component.MinimumAcidStrength = current.MinimumAcidStrength;
    }
  }
}
