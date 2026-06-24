// Decompiled with JetBrains decompiler
// Type: Content.Shared.Advertise.Components.SpeakOnUIClosedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Advertise.Systems;
using Content.Shared.Dataset;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Advertise.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSpeakOnUIClosedSystem)})]
public sealed class SpeakOnUIClosedComponent : 
  Component,
  ISerializationGenerated<SpeakOnUIClosedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireFlag = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Flag;

  [DataField(null, false, 1, true, false, null)]
  public ProtoId<LocalizedDatasetPrototype> Pack { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpeakOnUIClosedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SpeakOnUIClosedComponent) component;
    if (serialization.TryCustomCopy<SpeakOnUIClosedComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<LocalizedDatasetPrototype> protoId = new ProtoId<LocalizedDatasetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(this.Pack, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(this.Pack, hookCtx, context, false);
    target.Pack = protoId;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag1, hookCtx, false, context))
      flag1 = this.Enabled;
    target.Enabled = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireFlag, ref flag2, hookCtx, false, context))
      flag2 = this.RequireFlag;
    target.RequireFlag = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Flag, ref flag3, hookCtx, false, context))
      flag3 = this.Flag;
    target.Flag = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpeakOnUIClosedComponent target,
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
    SpeakOnUIClosedComponent target1 = (SpeakOnUIClosedComponent) target;
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
    SpeakOnUIClosedComponent target1 = (SpeakOnUIClosedComponent) target;
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
    SpeakOnUIClosedComponent target1 = (SpeakOnUIClosedComponent) target;
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
  virtual SpeakOnUIClosedComponent Component.Instantiate() => new SpeakOnUIClosedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpeakOnUIClosedComponent_AutoState : IComponentState
  {
    public bool Flag;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpeakOnUIClosedComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SpeakOnUIClosedComponent, ComponentGetState>(new ComponentEventRefHandler<SpeakOnUIClosedComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SpeakOnUIClosedComponent, ComponentHandleState>(new ComponentEventRefHandler<SpeakOnUIClosedComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      SpeakOnUIClosedComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SpeakOnUIClosedComponent.SpeakOnUIClosedComponent_AutoState()
      {
        Flag = component.Flag
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpeakOnUIClosedComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SpeakOnUIClosedComponent.SpeakOnUIClosedComponent_AutoState current))
        return;
      component.Flag = current.Flag;
    }
  }
}
