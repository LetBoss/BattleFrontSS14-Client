// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Components.AirlockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Content.Shared.Doors.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Doors.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedAirlockSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
public sealed class AirlockComponent : 
  Component,
  ISerializationGenerated<AirlockComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Powered;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Safety = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EmergencyAccess;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier EmergencyOnSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/airlock_emergencyon.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier EmergencyOffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/airlock_emergencyoff.ogg");
  [DataField(null, false, 1, false, false, null)]
  public float PoweredPryModifier = 9f;
  [DataField(null, false, 1, false, false, null)]
  public bool OpenPanelVisible;
  [DataField(null, false, 1, false, false, null)]
  public bool KeepOpenIfClicked;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoClose = true;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AutoCloseDelay = TimeSpan.FromSeconds(5.0);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float AutoCloseDelayModifier = 1f;
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<SinkPortPrototype>))]
  public string AutoClosePort = nameof (AutoClose);
  [DataField(null, false, 1, false, false, null)]
  public bool OpenUnlitVisible;
  [DataField(null, false, 1, false, false, null)]
  public bool EmergencyAccessLayer = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AnimatePanel = true;
  [DataField(null, false, 1, false, false, null)]
  public string OpeningSpriteState = "opening_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string OpeningPanelSpriteState = "panel_opening";
  [DataField(null, false, 1, false, false, null)]
  public string ClosingSpriteState = "closing_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string ClosingPanelSpriteState = "panel_closing";
  [DataField(null, false, 1, false, false, null)]
  public string OpenSpriteState = "open_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string ClosedSpriteState = "closed_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string DenySpriteState = "deny_unlit";
  [DataField(null, false, 1, false, false, null)]
  public float DenyAnimationTime = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  public float BoltedPryModifier = 3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AirlockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AirlockComponent) target1;
    if (serialization.TryCustomCopy<AirlockComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Powered, ref target2, hookCtx, false, context))
      target2 = this.Powered;
    target.Powered = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Safety, ref target3, hookCtx, false, context))
      target3 = this.Safety;
    target.Safety = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.EmergencyAccess, ref target4, hookCtx, false, context))
      target4 = this.EmergencyAccess;
    target.EmergencyAccess = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.EmergencyOnSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EmergencyOnSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.EmergencyOnSound, hookCtx, context);
    target.EmergencyOnSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.EmergencyOffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EmergencyOffSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.EmergencyOffSound, hookCtx, context);
    target.EmergencyOffSound = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PoweredPryModifier, ref target7, hookCtx, false, context))
      target7 = this.PoweredPryModifier;
    target.PoweredPryModifier = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenPanelVisible, ref target8, hookCtx, false, context))
      target8 = this.OpenPanelVisible;
    target.OpenPanelVisible = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.KeepOpenIfClicked, ref target9, hookCtx, false, context))
      target9 = this.KeepOpenIfClicked;
    target.KeepOpenIfClicked = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoClose, ref target10, hookCtx, false, context))
      target10 = this.AutoClose;
    target.AutoClose = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AutoCloseDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.AutoCloseDelay, hookCtx, context);
    target.AutoCloseDelay = target11;
    string target12 = (string) null;
    if (this.AutoClosePort == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AutoClosePort, ref target12, hookCtx, false, context))
      target12 = this.AutoClosePort;
    target.AutoClosePort = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenUnlitVisible, ref target13, hookCtx, false, context))
      target13 = this.OpenUnlitVisible;
    target.OpenUnlitVisible = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.EmergencyAccessLayer, ref target14, hookCtx, false, context))
      target14 = this.EmergencyAccessLayer;
    target.EmergencyAccessLayer = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.AnimatePanel, ref target15, hookCtx, false, context))
      target15 = this.AnimatePanel;
    target.AnimatePanel = target15;
    string target16 = (string) null;
    if (this.OpeningSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningSpriteState, ref target16, hookCtx, false, context))
      target16 = this.OpeningSpriteState;
    target.OpeningSpriteState = target16;
    string target17 = (string) null;
    if (this.OpeningPanelSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningPanelSpriteState, ref target17, hookCtx, false, context))
      target17 = this.OpeningPanelSpriteState;
    target.OpeningPanelSpriteState = target17;
    string target18 = (string) null;
    if (this.ClosingSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosingSpriteState, ref target18, hookCtx, false, context))
      target18 = this.ClosingSpriteState;
    target.ClosingSpriteState = target18;
    string target19 = (string) null;
    if (this.ClosingPanelSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosingPanelSpriteState, ref target19, hookCtx, false, context))
      target19 = this.ClosingPanelSpriteState;
    target.ClosingPanelSpriteState = target19;
    string target20 = (string) null;
    if (this.OpenSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpenSpriteState, ref target20, hookCtx, false, context))
      target20 = this.OpenSpriteState;
    target.OpenSpriteState = target20;
    string target21 = (string) null;
    if (this.ClosedSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosedSpriteState, ref target21, hookCtx, false, context))
      target21 = this.ClosedSpriteState;
    target.ClosedSpriteState = target21;
    string target22 = (string) null;
    if (this.DenySpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DenySpriteState, ref target22, hookCtx, false, context))
      target22 = this.DenySpriteState;
    target.DenySpriteState = target22;
    float target23 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DenyAnimationTime, ref target23, hookCtx, false, context))
      target23 = this.DenyAnimationTime;
    target.DenyAnimationTime = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BoltedPryModifier, ref target24, hookCtx, false, context))
      target24 = this.BoltedPryModifier;
    target.BoltedPryModifier = target24;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AirlockComponent target,
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
    AirlockComponent target1 = (AirlockComponent) target;
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
    AirlockComponent target1 = (AirlockComponent) target;
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
    AirlockComponent target1 = (AirlockComponent) target;
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
  virtual AirlockComponent Component.Instantiate() => new AirlockComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AirlockComponent_AutoState : IComponentState
  {
    public bool Powered;
    public bool Safety;
    public bool EmergencyAccess;
    public bool AutoClose;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AirlockComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AirlockComponent, ComponentGetState>(new ComponentEventRefHandler<AirlockComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AirlockComponent, ComponentHandleState>(new ComponentEventRefHandler<AirlockComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AirlockComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new AirlockComponent.AirlockComponent_AutoState()
      {
        Powered = component.Powered,
        Safety = component.Safety,
        EmergencyAccess = component.EmergencyAccess,
        AutoClose = component.AutoClose
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AirlockComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AirlockComponent.AirlockComponent_AutoState current))
        return;
      component.Powered = current.Powered;
      component.Safety = current.Safety;
      component.EmergencyAccess = current.EmergencyAccess;
      component.AutoClose = current.AutoClose;
    }
  }
}
