// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Abduct.XenoAbductComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Abduct;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoAbductSystem)})]
public sealed class XenoAbductComponent : 
  Component,
  ISerializationGenerated<XenoAbductComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoafterTime = TimeSpan.FromSeconds(0.8);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Telegraph = (EntProtoId) "RMCEffectXenoTelegraphAbduct";
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> Tiles = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype>? Emote = (ProtoId<EmotePrototype>?) "XenoRoar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Cost = (FixedPoint2) 180;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowTime = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RootTime = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeTime = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunTime = TimeSpan.FromSeconds(2.6);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxTargets = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TileRadius = 0.4f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAbductComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAbductComponent) target1;
    if (serialization.TryCustomCopy<XenoAbductComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoafterTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DoafterTime, hookCtx, context);
    target.DoafterTime = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Telegraph, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Telegraph, hookCtx, context);
    target.Telegraph = target3;
    List<EntityUid> target4 = (List<EntityUid>) null;
    if (this.Tiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Tiles, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntityUid>>(this.Tiles, hookCtx, context);
    target.Tiles = target4;
    ProtoId<EmotePrototype>? target5 = new ProtoId<EmotePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>?>(this.Emote, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<EmotePrototype>?>(this.Emote, hookCtx, context);
    target.Emote = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Cost, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.Cost, hookCtx, context);
    target.Cost = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target9, hookCtx, false, context))
      target9 = this.Range;
    target.Range = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.SlowTime, hookCtx, context);
    target.SlowTime = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RootTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.RootTime, hookCtx, context);
    target.RootTime = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.DazeTime, hookCtx, context);
    target.DazeTime = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTargets, ref target14, hookCtx, false, context))
      target14 = this.MaxTargets;
    target.MaxTargets = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TileRadius, ref target15, hookCtx, false, context))
      target15 = this.TileRadius;
    target.TileRadius = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAbductComponent target,
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
    XenoAbductComponent target1 = (XenoAbductComponent) target;
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
    XenoAbductComponent target1 = (XenoAbductComponent) target;
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
    XenoAbductComponent target1 = (XenoAbductComponent) target;
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
  virtual XenoAbductComponent Component.Instantiate() => new XenoAbductComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoAbductComponent_AutoState : IComponentState
  {
    public TimeSpan DoafterTime;
    public EntProtoId Telegraph;
    public ProtoId<EmotePrototype>? Emote;
    public FixedPoint2 Cost;
    public SoundSpecifier Sound;
    public TimeSpan Cooldown;
    public int Range;
    public TimeSpan SlowTime;
    public TimeSpan RootTime;
    public TimeSpan DazeTime;
    public TimeSpan StunTime;
    public int MaxTargets;
    public float TileRadius;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoAbductComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoAbductComponent, ComponentGetState>(new ComponentEventRefHandler<XenoAbductComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoAbductComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoAbductComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoAbductComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoAbductComponent.XenoAbductComponent_AutoState()
      {
        DoafterTime = component.DoafterTime,
        Telegraph = component.Telegraph,
        Emote = component.Emote,
        Cost = component.Cost,
        Sound = component.Sound,
        Cooldown = component.Cooldown,
        Range = component.Range,
        SlowTime = component.SlowTime,
        RootTime = component.RootTime,
        DazeTime = component.DazeTime,
        StunTime = component.StunTime,
        MaxTargets = component.MaxTargets,
        TileRadius = component.TileRadius
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoAbductComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoAbductComponent.XenoAbductComponent_AutoState current))
        return;
      component.DoafterTime = current.DoafterTime;
      component.Telegraph = current.Telegraph;
      component.Emote = current.Emote;
      component.Cost = current.Cost;
      component.Sound = current.Sound;
      component.Cooldown = current.Cooldown;
      component.Range = current.Range;
      component.SlowTime = current.SlowTime;
      component.RootTime = current.RootTime;
      component.DazeTime = current.DazeTime;
      component.StunTime = current.StunTime;
      component.MaxTargets = current.MaxTargets;
      component.TileRadius = current.TileRadius;
    }
  }
}
