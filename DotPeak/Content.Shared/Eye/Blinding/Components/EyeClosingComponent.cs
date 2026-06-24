// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Components.EyeClosingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Eye.Blinding.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class EyeClosingComponent : 
  Component,
  ISerializationGenerated<EyeClosingComponent>,
  ISerializationGenerated
{
  private static readonly ProtoId<SoundCollectionPrototype> DefaultEyeOpen = new ProtoId<SoundCollectionPrototype>("EyeOpen");
  private static readonly ProtoId<SoundCollectionPrototype> DefaultEyeClose = new ProtoId<SoundCollectionPrototype>("EyeClose");
  [DataField("eyeToggleAction", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string EyeToggleAction = "ActionToggleEyes";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? EyeToggleActionEntity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier EyeOpenSound = (SoundSpecifier) new SoundCollectionSpecifier((string) EyeClosingComponent.DefaultEyeOpen);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier EyeCloseSound = (SoundSpecifier) new SoundCollectionSpecifier((string) EyeClosingComponent.DefaultEyeClose);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EyesClosed;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField(null, false, 1, false, false, null)]
  public bool PreviousEyelidPosition;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField(null, false, 1, false, false, null)]
  public bool NaturallyCreated;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EyeClosingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EyeClosingComponent) target1;
    if (serialization.TryCustomCopy<EyeClosingComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.EyeToggleAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EyeToggleAction, ref target2, hookCtx, false, context))
      target2 = this.EyeToggleAction;
    target.EyeToggleAction = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.EyeToggleActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.EyeToggleActionEntity, hookCtx, context);
    target.EyeToggleActionEntity = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.EyeOpenSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EyeOpenSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.EyeOpenSound, hookCtx, context);
    target.EyeOpenSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.EyeCloseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EyeCloseSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.EyeCloseSound, hookCtx, context);
    target.EyeCloseSound = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.EyesClosed, ref target6, hookCtx, false, context))
      target6 = this.EyesClosed;
    target.EyesClosed = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreviousEyelidPosition, ref target7, hookCtx, false, context))
      target7 = this.PreviousEyelidPosition;
    target.PreviousEyelidPosition = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.NaturallyCreated, ref target8, hookCtx, false, context))
      target8 = this.NaturallyCreated;
    target.NaturallyCreated = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EyeClosingComponent target,
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
    EyeClosingComponent target1 = (EyeClosingComponent) target;
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
    EyeClosingComponent target1 = (EyeClosingComponent) target;
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
    EyeClosingComponent target1 = (EyeClosingComponent) target;
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
  virtual EyeClosingComponent Component.Instantiate() => new EyeClosingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EyeClosingComponent_AutoState : IComponentState
  {
    public SoundSpecifier EyeOpenSound;
    public SoundSpecifier EyeCloseSound;
    public bool EyesClosed;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EyeClosingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EyeClosingComponent, ComponentGetState>(new ComponentEventRefHandler<EyeClosingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EyeClosingComponent, ComponentHandleState>(new ComponentEventRefHandler<EyeClosingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EyeClosingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EyeClosingComponent.EyeClosingComponent_AutoState()
      {
        EyeOpenSound = component.EyeOpenSound,
        EyeCloseSound = component.EyeCloseSound,
        EyesClosed = component.EyesClosed
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EyeClosingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EyeClosingComponent.EyeClosingComponent_AutoState current))
        return;
      component.EyeOpenSound = current.EyeOpenSound;
      component.EyeCloseSound = current.EyeCloseSound;
      component.EyesClosed = current.EyesClosed;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, EyeClosingComponent>(uid, component, ref args1);
    }
  }
}
