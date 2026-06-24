// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.Components.RadioJammerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Radio.Components;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RadioJammerComponent : 
  Component,
  ISerializationGenerated<RadioJammerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public RadioJammerComponent.RadioJamSetting[] Settings;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SelectedPowerLevel = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RadioJammerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RadioJammerComponent) target1;
    if (serialization.TryCustomCopy<RadioJammerComponent>(this, ref target, hookCtx, false, context))
      return;
    RadioJammerComponent.RadioJamSetting[] target2 = (RadioJammerComponent.RadioJamSetting[]) null;
    if (this.Settings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<RadioJammerComponent.RadioJamSetting[]>(this.Settings, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<RadioJammerComponent.RadioJamSetting[]>(this.Settings, hookCtx, context);
    target.Settings = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.SelectedPowerLevel, ref target3, hookCtx, false, context))
      target3 = this.SelectedPowerLevel;
    target.SelectedPowerLevel = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RadioJammerComponent target,
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
    RadioJammerComponent target1 = (RadioJammerComponent) target;
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
    RadioJammerComponent target1 = (RadioJammerComponent) target;
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
    RadioJammerComponent target1 = (RadioJammerComponent) target;
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
  virtual RadioJammerComponent Component.Instantiate() => new RadioJammerComponent();

  [DataDefinition]
  public struct RadioJamSetting : 
    ISerializationGenerated<RadioJammerComponent.RadioJamSetting>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, true, false, null)]
    public float Wattage;
    [DataField(null, false, 1, true, false, null)]
    public float Range;
    [DataField(null, false, 1, true, false, null)]
    public LocId Message;
    [DataField(null, false, 1, true, false, null)]
    public LocId Name;

    public RadioJamSetting()
    {
      this.Wattage = 0.0f;
      this.Range = 0.0f;
      this.Message = (LocId) string.Empty;
      this.Name = (LocId) string.Empty;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref RadioJammerComponent.RadioJamSetting target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<RadioJammerComponent.RadioJamSetting>(this, ref target, hookCtx, false, context))
        return;
      float target1 = 0.0f;
      if (!serialization.TryCustomCopy<float>(this.Wattage, ref target1, hookCtx, false, context))
        target1 = this.Wattage;
      float target2 = 0.0f;
      if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
        target2 = this.Range;
      LocId target3 = new LocId();
      if (!serialization.TryCustomCopy<LocId>(this.Message, ref target3, hookCtx, false, context))
        target3 = serialization.CreateCopy<LocId>(this.Message, hookCtx, context);
      LocId target4 = new LocId();
      if (!serialization.TryCustomCopy<LocId>(this.Name, ref target4, hookCtx, false, context))
        target4 = serialization.CreateCopy<LocId>(this.Name, hookCtx, context);
      target = target with
      {
        Wattage = target1,
        Range = target2,
        Message = target3,
        Name = target4
      };
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref RadioJammerComponent.RadioJamSetting target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      RadioJammerComponent.RadioJamSetting target1 = (RadioJammerComponent.RadioJamSetting) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public RadioJammerComponent.RadioJamSetting Instantiate()
    {
      return new RadioJammerComponent.RadioJamSetting();
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RadioJammerComponent_AutoState : IComponentState
  {
    public int SelectedPowerLevel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RadioJammerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RadioJammerComponent, ComponentGetState>(new ComponentEventRefHandler<RadioJammerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RadioJammerComponent, ComponentHandleState>(new ComponentEventRefHandler<RadioJammerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RadioJammerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RadioJammerComponent.RadioJammerComponent_AutoState()
      {
        SelectedPowerLevel = component.SelectedPowerLevel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RadioJammerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RadioJammerComponent.RadioJammerComponent_AutoState current))
        return;
      component.SelectedPowerLevel = current.SelectedPowerLevel;
    }
  }
}
