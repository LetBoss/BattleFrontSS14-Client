// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sound.Components.SpamEmitSoundComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Sound.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class SpamEmitSoundComponent : 
  BaseEmitSoundComponent,
  ISerializationGenerated<SpamEmitSoundComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan NextSound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MinInterval = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan MaxInterval = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public LocId? PopUp;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedEmitSoundSystem)})]
  public bool Enabled = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpamEmitSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseEmitSoundComponent target1 = (BaseEmitSoundComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpamEmitSoundComponent) target1;
    if (serialization.TryCustomCopy<SpamEmitSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSound, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextSound, hookCtx, context);
    target.NextSound = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinInterval, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.MinInterval, hookCtx, context);
    target.MinInterval = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxInterval, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MaxInterval, hookCtx, context);
    target.MaxInterval = target4;
    LocId? target5 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.PopUp, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId?>(this.PopUp, hookCtx, context);
    target.PopUp = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target6, hookCtx, false, context))
      target6 = this.Enabled;
    target.Enabled = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpamEmitSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseEmitSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SpamEmitSoundComponent target1 = (SpamEmitSoundComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseEmitSoundComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SpamEmitSoundComponent target1 = (SpamEmitSoundComponent) target;
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
    SpamEmitSoundComponent target1 = (SpamEmitSoundComponent) target;
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
  virtual SpamEmitSoundComponent BaseEmitSoundComponent.Instantiate()
  {
    return new SpamEmitSoundComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpamEmitSoundComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpamEmitSoundComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SpamEmitSoundComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SpamEmitSoundComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextSound += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpamEmitSoundComponent_AutoState : IComponentState
  {
    public TimeSpan NextSound;
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpamEmitSoundComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpamEmitSoundComponent, ComponentGetState>(new ComponentEventRefHandler<SpamEmitSoundComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpamEmitSoundComponent, ComponentHandleState>(new ComponentEventRefHandler<SpamEmitSoundComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SpamEmitSoundComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpamEmitSoundComponent.SpamEmitSoundComponent_AutoState()
      {
        NextSound = component.NextSound,
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpamEmitSoundComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpamEmitSoundComponent.SpamEmitSoundComponent_AutoState current))
        return;
      component.NextSound = current.NextSound;
      component.Enabled = current.Enabled;
    }
  }
}
