// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lifesteal.XenoLifestealComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Lifesteal;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoLifestealSystem)})]
public sealed class XenoLifestealComponent : 
  Component,
  ISerializationGenerated<XenoLifestealComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BasePercentage = (FixedPoint2) 0.07;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxPercentage = (FixedPoint2) 0.09;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TargetIncreasePercentage = (FixedPoint2) 0.01;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MinHeal = (FixedPoint2) 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxHeal = (FixedPoint2) 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TargetRange = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? MaxEffect = (EntProtoId?) "RMCEffectHeal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? EmoteCooldown = new TimeSpan?(TimeSpan.FromSeconds(5L));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color AuraColor = Color.FromHex((ReadOnlySpan<char>) "#6C6F24", new Color?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoLifestealComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoLifestealComponent) target1;
    if (serialization.TryCustomCopy<XenoLifestealComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BasePercentage, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.BasePercentage, hookCtx, context);
    target.BasePercentage = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxPercentage, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxPercentage, hookCtx, context);
    target.MaxPercentage = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TargetIncreasePercentage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.TargetIncreasePercentage, hookCtx, context);
    target.TargetIncreasePercentage = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinHeal, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.MinHeal, hookCtx, context);
    target.MinHeal = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxHeal, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.MaxHeal, hookCtx, context);
    target.MaxHeal = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetRange, ref target7, hookCtx, false, context))
      target7 = this.TargetRange;
    target.TargetRange = target7;
    EntProtoId? target8 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.MaxEffect, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId?>(this.MaxEffect, hookCtx, context);
    target.MaxEffect = target8;
    ProtoId<EmotePrototype>? target9 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmoteCooldown, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.EmoteCooldown, hookCtx, context);
    target.EmoteCooldown = target10;
    Color target11 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.AuraColor, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<Color>(this.AuraColor, hookCtx, context);
    target.AuraColor = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoLifestealComponent target,
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
    XenoLifestealComponent target1 = (XenoLifestealComponent) target;
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
    XenoLifestealComponent target1 = (XenoLifestealComponent) target;
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
    XenoLifestealComponent target1 = (XenoLifestealComponent) target;
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
  virtual XenoLifestealComponent Component.Instantiate() => new XenoLifestealComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoLifestealComponent_AutoState : IComponentState
  {
    public FixedPoint2 BasePercentage;
    public FixedPoint2 MaxPercentage;
    public FixedPoint2 TargetIncreasePercentage;
    public FixedPoint2 MinHeal;
    public FixedPoint2 MaxHeal;
    public float TargetRange;
    public EntProtoId? MaxEffect;
    public ProtoId<EmotePrototype>? Emote;
    public TimeSpan? EmoteCooldown;
    public Color AuraColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoLifestealComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoLifestealComponent, ComponentGetState>(new ComponentEventRefHandler<XenoLifestealComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoLifestealComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoLifestealComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoLifestealComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoLifestealComponent.XenoLifestealComponent_AutoState()
      {
        BasePercentage = component.BasePercentage,
        MaxPercentage = component.MaxPercentage,
        TargetIncreasePercentage = component.TargetIncreasePercentage,
        MinHeal = component.MinHeal,
        MaxHeal = component.MaxHeal,
        TargetRange = component.TargetRange,
        MaxEffect = component.MaxEffect,
        Emote = component.Emote,
        EmoteCooldown = component.EmoteCooldown,
        AuraColor = component.AuraColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoLifestealComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoLifestealComponent.XenoLifestealComponent_AutoState current))
        return;
      component.BasePercentage = current.BasePercentage;
      component.MaxPercentage = current.MaxPercentage;
      component.TargetIncreasePercentage = current.TargetIncreasePercentage;
      component.MinHeal = current.MinHeal;
      component.MaxHeal = current.MaxHeal;
      component.TargetRange = current.TargetRange;
      component.MaxEffect = current.MaxEffect;
      component.Emote = current.Emote;
      component.EmoteCooldown = current.EmoteCooldown;
      component.AuraColor = current.AuraColor;
    }
  }
}
