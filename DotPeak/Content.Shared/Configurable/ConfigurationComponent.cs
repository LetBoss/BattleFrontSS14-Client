// Decompiled with JetBrains decompiler
// Type: Content.Shared.Configurable.ConfigurationComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Configurable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class ConfigurationComponent : 
  Component,
  ISerializationGenerated<ConfigurationComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, string?> Config = new Dictionary<string, string>();
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> QualityNeeded = ProtoId<ToolQualityPrototype>.op_Implicit("Pulsing");
  [DataField(null, false, 1, false, false, null)]
  public Regex Validation = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConfigurationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ConfigurationComponent) component;
    if (serialization.TryCustomCopy<ConfigurationComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
    if (this.Config == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.Config, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, string>>(this.Config, hookCtx, context, false);
    target.Config = dictionary;
    ProtoId<ToolQualityPrototype> protoId = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, hookCtx, context, false);
    target.QualityNeeded = protoId;
    Regex regex = (Regex) null;
    if (this.Validation == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Regex>(this.Validation, ref regex, hookCtx, true, context))
      regex = serialization.CreateCopy<Regex>(this.Validation, hookCtx, context, false);
    target.Validation = regex;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConfigurationComponent target,
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
    ConfigurationComponent target1 = (ConfigurationComponent) target;
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
    ConfigurationComponent target1 = (ConfigurationComponent) target;
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
    ConfigurationComponent target1 = (ConfigurationComponent) target;
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
  virtual ConfigurationComponent Component.Instantiate() => new ConfigurationComponent();

  [NetSerializable]
  [Serializable]
  public sealed class ConfigurationUpdatedMessage : BoundUserInterfaceMessage
  {
    public Dictionary<string, string> Config { get; }

    public ConfigurationUpdatedMessage(Dictionary<string, string> config) => this.Config = config;
  }

  [NetSerializable]
  [Serializable]
  public sealed class ValidationUpdateMessage : BoundUserInterfaceMessage
  {
    public string ValidationString { get; }

    public ValidationUpdateMessage(string validationString)
    {
      this.ValidationString = validationString;
    }
  }

  [NetSerializable]
  [Serializable]
  public enum ConfigurationUiKey
  {
    Key,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ConfigurationComponent_AutoState : IComponentState
  {
    public Dictionary<string, string?> Config;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ConfigurationComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConfigurationComponent, ComponentGetState>(new ComponentEventRefHandler<ConfigurationComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConfigurationComponent, ComponentHandleState>(new ComponentEventRefHandler<ConfigurationComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      ConfigurationComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ConfigurationComponent.ConfigurationComponent_AutoState()
      {
        Config = component.Config
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ConfigurationComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ConfigurationComponent.ConfigurationComponent_AutoState current))
        return;
      component.Config = current.Config == null ? (Dictionary<string, string>) null : new Dictionary<string, string>((IDictionary<string, string>) current.Config);
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, ConfigurationComponent>(uid, component, ref handleStateEvent);
    }
  }
}
