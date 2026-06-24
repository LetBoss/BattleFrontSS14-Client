// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Entrenching.EntrenchingToolComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Entrenching;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (BarricadeSystem)})]
public sealed class EntrenchingToolComponent : 
  Component,
  ISerializationGenerated<EntrenchingToolComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DigDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FillDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LayersPerDig = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TotalLayers;
  [AutoNetworkedField]
  public EntityCoordinates LastDigLocation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier DigSound = (SoundSpecifier) new SoundCollectionSpecifier("CMEntrenchingThud", new AudioParams?(AudioParams.Default.WithVolume(-3f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier FillSound = (SoundSpecifier) new SoundCollectionSpecifier("CMEntrenchingRustle", new AudioParams?(AudioParams.Default.WithVolume(-6f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntrenchingToolComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntrenchingToolComponent) target1;
    if (serialization.TryCustomCopy<EntrenchingToolComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DigDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DigDelay, hookCtx, context);
    target.DigDelay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FillDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.FillDelay, hookCtx, context);
    target.FillDelay = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.LayersPerDig, ref target4, hookCtx, false, context))
      target4 = this.LayersPerDig;
    target.LayersPerDig = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.TotalLayers, ref target5, hookCtx, false, context))
      target5 = this.TotalLayers;
    target.TotalLayers = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.DigSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DigSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.DigSound, hookCtx, context);
    target.DigSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.FillSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FillSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.FillSound, hookCtx, context);
    target.FillSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntrenchingToolComponent target,
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
    EntrenchingToolComponent target1 = (EntrenchingToolComponent) target;
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
    EntrenchingToolComponent target1 = (EntrenchingToolComponent) target;
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
    EntrenchingToolComponent target1 = (EntrenchingToolComponent) target;
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
  virtual EntrenchingToolComponent Component.Instantiate() => new EntrenchingToolComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EntrenchingToolComponent_AutoState : IComponentState
  {
    public TimeSpan DigDelay;
    public TimeSpan FillDelay;
    public int LayersPerDig;
    public int TotalLayers;
    public NetCoordinates LastDigLocation;
    public SoundSpecifier DigSound;
    public SoundSpecifier FillSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EntrenchingToolComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EntrenchingToolComponent, ComponentGetState>(new ComponentEventRefHandler<EntrenchingToolComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EntrenchingToolComponent, ComponentHandleState>(new ComponentEventRefHandler<EntrenchingToolComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EntrenchingToolComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EntrenchingToolComponent.EntrenchingToolComponent_AutoState()
      {
        DigDelay = component.DigDelay,
        FillDelay = component.FillDelay,
        LayersPerDig = component.LayersPerDig,
        TotalLayers = component.TotalLayers,
        LastDigLocation = this.GetNetCoordinates(component.LastDigLocation),
        DigSound = component.DigSound,
        FillSound = component.FillSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EntrenchingToolComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EntrenchingToolComponent.EntrenchingToolComponent_AutoState current))
        return;
      component.DigDelay = current.DigDelay;
      component.FillDelay = current.FillDelay;
      component.LayersPerDig = current.LayersPerDig;
      component.TotalLayers = current.TotalLayers;
      component.LastDigLocation = this.EnsureCoordinates<EntrenchingToolComponent>(current.LastDigLocation, uid);
      component.DigSound = current.DigSound;
      component.FillSound = current.FillSound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, EntrenchingToolComponent>(uid, component, ref args1);
    }
  }
}
