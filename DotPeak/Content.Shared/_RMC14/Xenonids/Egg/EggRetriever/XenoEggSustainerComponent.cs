// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.EggRetriever.XenoEggSustainerComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg.EggRetriever;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoEggSustainerComponent : 
  Component,
  ISerializationGenerated<XenoEggSustainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> SustainedEggs = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxSustainedEggs = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SustainedEggsRange = 14;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SustainedEggMaxTime = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier DeathSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_egg_burst.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEggSustainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEggSustainerComponent) target1;
    if (serialization.TryCustomCopy<XenoEggSustainerComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.SustainedEggs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.SustainedEggs, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.SustainedEggs, hookCtx, context);
    target.SustainedEggs = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxSustainedEggs, ref target3, hookCtx, false, context))
      target3 = this.MaxSustainedEggs;
    target.MaxSustainedEggs = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.SustainedEggsRange, ref target4, hookCtx, false, context))
      target4 = this.SustainedEggsRange;
    target.SustainedEggsRange = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SustainedEggMaxTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.SustainedEggMaxTime, hookCtx, context);
    target.SustainedEggMaxTime = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.DeathSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeathSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.DeathSound, hookCtx, context);
    target.DeathSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEggSustainerComponent target,
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
    XenoEggSustainerComponent target1 = (XenoEggSustainerComponent) target;
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
    XenoEggSustainerComponent target1 = (XenoEggSustainerComponent) target;
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
    XenoEggSustainerComponent target1 = (XenoEggSustainerComponent) target;
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
  virtual XenoEggSustainerComponent Component.Instantiate() => new XenoEggSustainerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEggSustainerComponent_AutoState : IComponentState
  {
    public int MaxSustainedEggs;
    public int SustainedEggsRange;
    public TimeSpan SustainedEggMaxTime;
    public SoundSpecifier DeathSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEggSustainerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEggSustainerComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEggSustainerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEggSustainerComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEggSustainerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoEggSustainerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEggSustainerComponent.XenoEggSustainerComponent_AutoState()
      {
        MaxSustainedEggs = component.MaxSustainedEggs,
        SustainedEggsRange = component.SustainedEggsRange,
        SustainedEggMaxTime = component.SustainedEggMaxTime,
        DeathSound = component.DeathSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEggSustainerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEggSustainerComponent.XenoEggSustainerComponent_AutoState current))
        return;
      component.MaxSustainedEggs = current.MaxSustainedEggs;
      component.SustainedEggsRange = current.SustainedEggsRange;
      component.SustainedEggMaxTime = current.SustainedEggMaxTime;
      component.DeathSound = current.DeathSound;
    }
  }
}
