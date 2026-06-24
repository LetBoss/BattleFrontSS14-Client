// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Components.ResearchServerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ResearchServerComponent : 
  Component,
  ISerializationGenerated<ResearchServerComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  [DataField("serverName", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string ServerName = "RDSERVER";
  [AutoNetworkedField]
  [DataField("points", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Points;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public int Id;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public List<EntityUid> Clients = new List<EntityUid>();
  [DataField("nextUpdateTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextUpdateTime = TimeSpan.Zero;
  [DataField("researchConsoleUpdateTime", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan ResearchConsoleUpdateTime = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ResearchServerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ResearchServerComponent) target1;
    if (serialization.TryCustomCopy<ResearchServerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ServerName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ServerName, ref target2, hookCtx, false, context))
      target2 = this.ServerName;
    target.ServerName = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Points, ref target3, hookCtx, false, context))
      target3 = this.Points;
    target.Points = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdateTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextUpdateTime, hookCtx, context);
    target.NextUpdateTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ResearchConsoleUpdateTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ResearchConsoleUpdateTime, hookCtx, context);
    target.ResearchConsoleUpdateTime = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ResearchServerComponent target,
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
    ResearchServerComponent target1 = (ResearchServerComponent) target;
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
    ResearchServerComponent target1 = (ResearchServerComponent) target;
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
    ResearchServerComponent target1 = (ResearchServerComponent) target;
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
  virtual ResearchServerComponent Component.Instantiate() => new ResearchServerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ResearchServerComponent_AutoState : IComponentState
  {
    public string ServerName;
    public int Points;
    public int Id;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ResearchServerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ResearchServerComponent, ComponentGetState>(new ComponentEventRefHandler<ResearchServerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ResearchServerComponent, ComponentHandleState>(new ComponentEventRefHandler<ResearchServerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ResearchServerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ResearchServerComponent.ResearchServerComponent_AutoState()
      {
        ServerName = component.ServerName,
        Points = component.Points,
        Id = component.Id
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ResearchServerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ResearchServerComponent.ResearchServerComponent_AutoState current))
        return;
      component.ServerName = current.ServerName;
      component.Points = current.Points;
      component.Id = current.Id;
    }
  }
}
