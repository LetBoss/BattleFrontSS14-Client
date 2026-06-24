// Decompiled with JetBrains decompiler
// Type: Content.Shared.Standing.StandingStateComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Standing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (StandingStateSystem)})]
public sealed class StandingStateComponent : 
  Component,
  ISerializationGenerated<StandingStateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> ChangedFixtures = new List<string>();

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DownSound { get; private set; } = (SoundSpecifier) new SoundCollectionSpecifier("BodyFall");

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Standing { get; set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StandingStateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StandingStateComponent) target1;
    if (serialization.TryCustomCopy<StandingStateComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DownSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.DownSound, hookCtx, context);
    target.DownSound = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Standing, ref target3, hookCtx, false, context))
      target3 = this.Standing;
    target.Standing = target3;
    List<string> target4 = (List<string>) null;
    if (this.ChangedFixtures == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.ChangedFixtures, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<string>>(this.ChangedFixtures, hookCtx, context);
    target.ChangedFixtures = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StandingStateComponent target,
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
    StandingStateComponent target1 = (StandingStateComponent) target;
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
    StandingStateComponent target1 = (StandingStateComponent) target;
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
    StandingStateComponent target1 = (StandingStateComponent) target;
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
  virtual StandingStateComponent Component.Instantiate() => new StandingStateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StandingStateComponent_AutoState : IComponentState
  {
    public bool Standing;
    public List<string> ChangedFixtures;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StandingStateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StandingStateComponent, ComponentGetState>(new ComponentEventRefHandler<StandingStateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StandingStateComponent, ComponentHandleState>(new ComponentEventRefHandler<StandingStateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StandingStateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StandingStateComponent.StandingStateComponent_AutoState()
      {
        Standing = component.Standing,
        ChangedFixtures = component.ChangedFixtures
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StandingStateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StandingStateComponent.StandingStateComponent_AutoState current))
        return;
      component.Standing = current.Standing;
      component.ChangedFixtures = current.ChangedFixtures == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.ChangedFixtures);
    }
  }
}
