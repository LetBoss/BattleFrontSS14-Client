// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.Components.XenoArtifactNodeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Destructible.Thresholds;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedXenoArtifactSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class XenoArtifactNodeComponent : 
  Component,
  ISerializationGenerated<XenoArtifactNodeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Depth;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Locked = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? TriggerTip;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? Attached;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Durability;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxDurability = 5;
  [DataField(null, false, 1, false, false, null)]
  public MinMax MaxDurabilityCanDecreaseBy = new MinMax(0, 2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BasePointValue = 4000f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ResearchValue;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ConsumedResearchValue;

  public bool Degraded => this.Durability <= 0;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoArtifactNodeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoArtifactNodeComponent) target1;
    if (serialization.TryCustomCopy<XenoArtifactNodeComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Depth, ref target2, hookCtx, false, context))
      target2 = this.Depth;
    target.Depth = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target3, hookCtx, false, context))
      target3 = this.Locked;
    target.Locked = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.TriggerTip, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.TriggerTip, hookCtx, context);
    target.TriggerTip = target4;
    NetEntity? target5 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Attached, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<NetEntity?>(this.Attached, hookCtx, context);
    target.Attached = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Durability, ref target6, hookCtx, false, context))
      target6 = this.Durability;
    target.Durability = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDurability, ref target7, hookCtx, false, context))
      target7 = this.MaxDurability;
    target.MaxDurability = target7;
    MinMax target8 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.MaxDurabilityCanDecreaseBy, ref target8, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.MaxDurabilityCanDecreaseBy, ref target8, hookCtx, context);
    target.MaxDurabilityCanDecreaseBy = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BasePointValue, ref target9, hookCtx, false, context))
      target9 = this.BasePointValue;
    target.BasePointValue = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.ResearchValue, ref target10, hookCtx, false, context))
      target10 = this.ResearchValue;
    target.ResearchValue = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.ConsumedResearchValue, ref target11, hookCtx, false, context))
      target11 = this.ConsumedResearchValue;
    target.ConsumedResearchValue = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoArtifactNodeComponent target,
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
    XenoArtifactNodeComponent target1 = (XenoArtifactNodeComponent) target;
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
    XenoArtifactNodeComponent target1 = (XenoArtifactNodeComponent) target;
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
    XenoArtifactNodeComponent target1 = (XenoArtifactNodeComponent) target;
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
  virtual XenoArtifactNodeComponent Component.Instantiate() => new XenoArtifactNodeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoArtifactNodeComponent_AutoState : IComponentState
  {
    public int Depth;
    public bool Locked;
    public LocId? TriggerTip;
    public NetEntity? Attached;
    public int Durability;
    public int MaxDurability;
    public float BasePointValue;
    public int ResearchValue;
    public int ConsumedResearchValue;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoArtifactNodeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoArtifactNodeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoArtifactNodeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoArtifactNodeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoArtifactNodeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoArtifactNodeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoArtifactNodeComponent.XenoArtifactNodeComponent_AutoState()
      {
        Depth = component.Depth,
        Locked = component.Locked,
        TriggerTip = component.TriggerTip,
        Attached = component.Attached,
        Durability = component.Durability,
        MaxDurability = component.MaxDurability,
        BasePointValue = component.BasePointValue,
        ResearchValue = component.ResearchValue,
        ConsumedResearchValue = component.ConsumedResearchValue
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoArtifactNodeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoArtifactNodeComponent.XenoArtifactNodeComponent_AutoState current))
        return;
      component.Depth = current.Depth;
      component.Locked = current.Locked;
      component.TriggerTip = current.TriggerTip;
      component.Attached = current.Attached;
      component.Durability = current.Durability;
      component.MaxDurability = current.MaxDurability;
      component.BasePointValue = current.BasePointValue;
      component.ResearchValue = current.ResearchValue;
      component.ConsumedResearchValue = current.ConsumedResearchValue;
    }
  }
}
