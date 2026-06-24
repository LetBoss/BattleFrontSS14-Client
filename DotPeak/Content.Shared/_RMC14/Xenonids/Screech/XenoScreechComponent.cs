// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Screech.XenoScreechComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Xenonids.Screech;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoScreechSystem)})]
public sealed class XenoScreechComponent : 
  Component,
  ISerializationGenerated<XenoScreechComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 250;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CloseDeafTime = TimeSpan.FromSeconds(7L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FarDeafTime = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StunRange = 7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ParalyzeRange = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ParasiteStunRange = 11.2838f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ParasiteStunTime = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "CMEffectScreech";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_queen_screech.ogg", new AudioParams?(AudioParams.Default.WithVolume(-7f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoScreechComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoScreechComponent) target1;
    if (serialization.TryCustomCopy<XenoScreechComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParalyzeTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ParalyzeTime, hookCtx, context);
    target.ParalyzeTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CloseDeafTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.CloseDeafTime, hookCtx, context);
    target.CloseDeafTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FarDeafTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.FarDeafTime, hookCtx, context);
    target.FarDeafTime = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunRange, ref target7, hookCtx, false, context))
      target7 = this.StunRange;
    target.StunRange = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ParalyzeRange, ref target8, hookCtx, false, context))
      target8 = this.ParalyzeRange;
    target.ParalyzeRange = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ParasiteStunRange, ref target9, hookCtx, false, context))
      target9 = this.ParasiteStunRange;
    target.ParasiteStunRange = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ParasiteStunTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.ParasiteStunTime, hookCtx, context);
    target.ParasiteStunTime = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoScreechComponent target,
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
    XenoScreechComponent target1 = (XenoScreechComponent) target;
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
    XenoScreechComponent target1 = (XenoScreechComponent) target;
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
    XenoScreechComponent target1 = (XenoScreechComponent) target;
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
  virtual XenoScreechComponent Component.Instantiate() => new XenoScreechComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoScreechComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public TimeSpan StunTime;
    public TimeSpan ParalyzeTime;
    public TimeSpan CloseDeafTime;
    public TimeSpan FarDeafTime;
    public float StunRange;
    public float ParalyzeRange;
    public float ParasiteStunRange;
    public TimeSpan ParasiteStunTime;
    public EntProtoId Effect;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoScreechComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoScreechComponent, ComponentGetState>(new ComponentEventRefHandler<XenoScreechComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoScreechComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoScreechComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoScreechComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoScreechComponent.XenoScreechComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        StunTime = component.StunTime,
        ParalyzeTime = component.ParalyzeTime,
        CloseDeafTime = component.CloseDeafTime,
        FarDeafTime = component.FarDeafTime,
        StunRange = component.StunRange,
        ParalyzeRange = component.ParalyzeRange,
        ParasiteStunRange = component.ParasiteStunRange,
        ParasiteStunTime = component.ParasiteStunTime,
        Effect = component.Effect,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoScreechComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoScreechComponent.XenoScreechComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.StunTime = current.StunTime;
      component.ParalyzeTime = current.ParalyzeTime;
      component.CloseDeafTime = current.CloseDeafTime;
      component.FarDeafTime = current.FarDeafTime;
      component.StunRange = current.StunRange;
      component.ParalyzeRange = current.ParalyzeRange;
      component.ParasiteStunRange = current.ParasiteStunRange;
      component.ParasiteStunTime = current.ParasiteStunTime;
      component.Effect = current.Effect;
      component.Sound = current.Sound;
    }
  }
}
