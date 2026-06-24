// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosPipeLayersComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
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
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedAtmosPipeLayersSystem)})]
public sealed class AtmosPipeLayersComponent : 
  Component,
  ISerializationGenerated<AtmosPipeLayersComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public byte NumberOfPipeLayers = 3;
  [DataField("pipeLayer", false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public AtmosPipeLayer CurrentPipeLayer;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<AtmosPipeLayer, string> SpriteRsiPaths = new Dictionary<AtmosPipeLayer, string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Dictionary<AtmosPipeLayer, string>> SpriteLayersRsiPaths = new Dictionary<string, Dictionary<AtmosPipeLayer, string>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<AtmosPipeLayer, EntProtoId> AlternativePrototypes = new Dictionary<AtmosPipeLayer, EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public bool PipeLayersLocked;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> Tool = ProtoId<ToolQualityPrototype>.op_Implicit("Screwing");
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AtmosPipeLayersComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AtmosPipeLayersComponent) component;
    if (serialization.TryCustomCopy<AtmosPipeLayersComponent>(this, ref target, hookCtx, false, context))
      return;
    byte num1 = 0;
    if (!serialization.TryCustomCopy<byte>(this.NumberOfPipeLayers, ref num1, hookCtx, false, context))
      num1 = this.NumberOfPipeLayers;
    target.NumberOfPipeLayers = num1;
    AtmosPipeLayer atmosPipeLayer = AtmosPipeLayer.Primary;
    if (!serialization.TryCustomCopy<AtmosPipeLayer>(this.CurrentPipeLayer, ref atmosPipeLayer, hookCtx, false, context))
      atmosPipeLayer = this.CurrentPipeLayer;
    target.CurrentPipeLayer = atmosPipeLayer;
    Dictionary<AtmosPipeLayer, string> dictionary1 = (Dictionary<AtmosPipeLayer, string>) null;
    if (this.SpriteRsiPaths == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<AtmosPipeLayer, string>>(this.SpriteRsiPaths, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<AtmosPipeLayer, string>>(this.SpriteRsiPaths, hookCtx, context, false);
    target.SpriteRsiPaths = dictionary1;
    Dictionary<string, Dictionary<AtmosPipeLayer, string>> dictionary2 = (Dictionary<string, Dictionary<AtmosPipeLayer, string>>) null;
    if (this.SpriteLayersRsiPaths == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<AtmosPipeLayer, string>>>(this.SpriteLayersRsiPaths, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<string, Dictionary<AtmosPipeLayer, string>>>(this.SpriteLayersRsiPaths, hookCtx, context, false);
    target.SpriteLayersRsiPaths = dictionary2;
    Dictionary<AtmosPipeLayer, EntProtoId> dictionary3 = (Dictionary<AtmosPipeLayer, EntProtoId>) null;
    if (this.AlternativePrototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<AtmosPipeLayer, EntProtoId>>(this.AlternativePrototypes, ref dictionary3, hookCtx, true, context))
      dictionary3 = serialization.CreateCopy<Dictionary<AtmosPipeLayer, EntProtoId>>(this.AlternativePrototypes, hookCtx, context, false);
    target.AlternativePrototypes = dictionary3;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.PipeLayersLocked, ref flag, hookCtx, false, context))
      flag = this.PipeLayersLocked;
    target.PipeLayersLocked = flag;
    ProtoId<ToolQualityPrototype> protoId = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Tool, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Tool, hookCtx, context, false);
    target.Tool = protoId;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref num2, hookCtx, false, context))
      num2 = this.Delay;
    target.Delay = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AtmosPipeLayersComponent target,
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
    AtmosPipeLayersComponent target1 = (AtmosPipeLayersComponent) target;
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
    AtmosPipeLayersComponent target1 = (AtmosPipeLayersComponent) target;
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
    AtmosPipeLayersComponent target1 = (AtmosPipeLayersComponent) target;
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
  virtual AtmosPipeLayersComponent Component.Instantiate() => new AtmosPipeLayersComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AtmosPipeLayersComponent_AutoState : IComponentState
  {
    public AtmosPipeLayer CurrentPipeLayer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AtmosPipeLayersComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AtmosPipeLayersComponent, ComponentGetState>(new ComponentEventRefHandler<AtmosPipeLayersComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AtmosPipeLayersComponent, ComponentHandleState>(new ComponentEventRefHandler<AtmosPipeLayersComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      AtmosPipeLayersComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AtmosPipeLayersComponent.AtmosPipeLayersComponent_AutoState()
      {
        CurrentPipeLayer = component.CurrentPipeLayer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AtmosPipeLayersComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AtmosPipeLayersComponent.AtmosPipeLayersComponent_AutoState current))
        return;
      component.CurrentPipeLayer = current.CurrentPipeLayer;
    }
  }
}
