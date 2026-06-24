// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionRegenerationComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[AutoGenerateComponentPause]
[AutoGenerateComponentState(false, false)]
[NetworkedComponent]
[Access(new Type[] {typeof (SolutionRegenerationSystem)})]
public sealed class SolutionRegenerationComponent : 
  Component,
  ISerializationGenerated<SolutionRegenerationComponent>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, true, false, null)]
  public string SolutionName = string.Empty;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? SolutionRef;
  [DataField(null, false, 1, true, false, null)]
  public Solution Generated;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(1L);
  [DataField("nextChargeTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan NextRegenTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionRegenerationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionRegenerationComponent) component;
    if (serialization.TryCustomCopy<SolutionRegenerationComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref str, hookCtx, false, context))
      str = this.SolutionName;
    target.SolutionName = str;
    Solution solution = (Solution) null;
    if (this.Generated == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Solution>(this.Generated, ref solution, hookCtx, true, context))
    {
      if (this.Generated == null)
        solution = (Solution) null;
      else
        serialization.CopyTo<Solution>(this.Generated, ref solution, hookCtx, context, true);
    }
    target.Generated = solution;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context, false);
    target.Duration = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRegenTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextRegenTime, hookCtx, context, false);
    target.NextRegenTime = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionRegenerationComponent target,
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
    SolutionRegenerationComponent target1 = (SolutionRegenerationComponent) target;
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
    SolutionRegenerationComponent target1 = (SolutionRegenerationComponent) target;
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
    SolutionRegenerationComponent target1 = (SolutionRegenerationComponent) target;
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
  virtual SolutionRegenerationComponent Component.Instantiate()
  {
    return new SolutionRegenerationComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionRegenerationComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionRegenerationComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SolutionRegenerationComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SolutionRegenerationComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextRegenTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SolutionRegenerationComponent_AutoState : IComponentState
  {
    public TimeSpan NextRegenTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionRegenerationComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionRegenerationComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionRegenerationComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionRegenerationComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionRegenerationComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SolutionRegenerationComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SolutionRegenerationComponent.SolutionRegenerationComponent_AutoState()
      {
        NextRegenTime = component.NextRegenTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionRegenerationComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SolutionRegenerationComponent.SolutionRegenerationComponent_AutoState current))
        return;
      component.NextRegenTime = current.NextRegenTime;
    }
  }
}
