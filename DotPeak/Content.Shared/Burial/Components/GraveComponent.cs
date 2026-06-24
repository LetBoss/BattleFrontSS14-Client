// Decompiled with JetBrains decompiler
// Type: Content.Shared.Burial.Components.GraveComponent
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
namespace Content.Shared.Burial.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GraveComponent : 
  Component,
  ISerializationGenerated<GraveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan DigDelay;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DigOutByHandModifier;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SoundPathSpecifier DigSound;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool DiggingComplete;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Stream;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool ActiveShovelDigging;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public DoAfterId? HandDiggingDoAfter;

  public GraveComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Items/shovel_dig.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithLoop(true);
    this.DigSound = soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GraveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GraveComponent) component;
    if (serialization.TryCustomCopy<GraveComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DigDelay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.DigDelay, hookCtx, context, false);
    target.DigDelay = timeSpan;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DigOutByHandModifier, ref num, hookCtx, false, context))
      num = this.DigOutByHandModifier;
    target.DigOutByHandModifier = num;
    SoundPathSpecifier soundPathSpecifier = (SoundPathSpecifier) null;
    if (this.DigSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.DigSound, ref soundPathSpecifier, hookCtx, false, context))
    {
      if (this.DigSound == null)
        soundPathSpecifier = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.DigSound, ref soundPathSpecifier, hookCtx, context, true);
    }
    target.DigSound = soundPathSpecifier;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.DiggingComplete, ref flag1, hookCtx, false, context))
      flag1 = this.DiggingComplete;
    target.DiggingComplete = flag1;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Stream, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.Stream, hookCtx, context, false);
    target.Stream = nullable1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ActiveShovelDigging, ref flag2, hookCtx, false, context))
      flag2 = this.ActiveShovelDigging;
    target.ActiveShovelDigging = flag2;
    DoAfterId? nullable2 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.HandDiggingDoAfter, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<DoAfterId?>(this.HandDiggingDoAfter, hookCtx, context, false);
    target.HandDiggingDoAfter = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GraveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GraveComponent target1 = (GraveComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GraveComponent target1 = (GraveComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GraveComponent target1 = (GraveComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GraveComponent Component.Instantiate() => new GraveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GraveComponent_AutoState : IComponentState
  {
    public bool ActiveShovelDigging;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GraveComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GraveComponent, ComponentGetState>(new ComponentEventRefHandler<GraveComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GraveComponent, ComponentHandleState>(new ComponentEventRefHandler<GraveComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, GraveComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GraveComponent.GraveComponent_AutoState()
      {
        ActiveShovelDigging = component.ActiveShovelDigging
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GraveComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GraveComponent.GraveComponent_AutoState current))
        return;
      component.ActiveShovelDigging = current.ActiveShovelDigging;
    }
  }
}
