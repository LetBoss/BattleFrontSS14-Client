// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.EnvelopeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Paper;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class EnvelopeComponent : 
  Component,
  ISerializationGenerated<EnvelopeComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EnvelopeComponent.EnvelopeState State;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string SlotId = "letter_slot";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public DoAfterId? EnvelopeDoAfter;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan SealDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan TearDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundPathSpecifier? SealSound = new SoundPathSpecifier("/Audio/Effects/packetrip.ogg");
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundPathSpecifier? TearSound = new SoundPathSpecifier("/Audio/Effects/poster_broken.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EnvelopeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EnvelopeComponent) target1;
    if (serialization.TryCustomCopy<EnvelopeComponent>(this, ref target, hookCtx, false, context))
      return;
    EnvelopeComponent.EnvelopeState target2 = EnvelopeComponent.EnvelopeState.Open;
    if (!serialization.TryCustomCopy<EnvelopeComponent.EnvelopeState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    string target3 = (string) null;
    if (this.SlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotId, ref target3, hookCtx, false, context))
      target3 = this.SlotId;
    target.SlotId = target3;
    DoAfterId? target4 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.EnvelopeDoAfter, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<DoAfterId?>(this.EnvelopeDoAfter, hookCtx, context);
    target.EnvelopeDoAfter = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SealDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.SealDelay, hookCtx, context);
    target.SealDelay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TearDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.TearDelay, hookCtx, context);
    target.TearDelay = target6;
    SoundPathSpecifier target7 = (SoundPathSpecifier) null;
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.SealSound, ref target7, hookCtx, false, context))
    {
      if (this.SealSound == null)
        target7 = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.SealSound, ref target7, hookCtx, context);
    }
    target.SealSound = target7;
    SoundPathSpecifier target8 = (SoundPathSpecifier) null;
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.TearSound, ref target8, hookCtx, false, context))
    {
      if (this.TearSound == null)
        target8 = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.TearSound, ref target8, hookCtx, context);
    }
    target.TearSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EnvelopeComponent target,
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
    EnvelopeComponent target1 = (EnvelopeComponent) target;
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
    EnvelopeComponent target1 = (EnvelopeComponent) target;
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
    EnvelopeComponent target1 = (EnvelopeComponent) target;
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
  virtual EnvelopeComponent Component.Instantiate() => new EnvelopeComponent();

  [NetSerializable]
  [Serializable]
  public enum EnvelopeState : byte
  {
    Open,
    Sealed,
    Torn,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EnvelopeComponent_AutoState : IComponentState
  {
    public EnvelopeComponent.EnvelopeState State;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EnvelopeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EnvelopeComponent, ComponentGetState>(new ComponentEventRefHandler<EnvelopeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EnvelopeComponent, ComponentHandleState>(new ComponentEventRefHandler<EnvelopeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, EnvelopeComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new EnvelopeComponent.EnvelopeComponent_AutoState()
      {
        State = component.State
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EnvelopeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EnvelopeComponent.EnvelopeComponent_AutoState current))
        return;
      component.State = current.State;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, EnvelopeComponent>(uid, component, ref args1);
    }
  }
}
