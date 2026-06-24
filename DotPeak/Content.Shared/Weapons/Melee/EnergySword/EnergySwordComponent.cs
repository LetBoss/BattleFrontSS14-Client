// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.EnergySword.EnergySwordComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Melee.EnergySword;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (EnergySwordSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class EnergySwordComponent : 
  Component,
  ISerializationGenerated<EnergySwordComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color ActivatedColor = Color.DodgerBlue;
  [DataField(null, false, 1, false, false, null)]
  public List<Color> ColorOptions = new List<Color>()
  {
    Color.Tomato,
    Color.DodgerBlue,
    Color.Aqua,
    Color.MediumSpringGreen,
    Color.MediumOrchid
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hacked;
  [DataField(null, false, 1, false, false, null)]
  public float CycleRate = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EnergySwordComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EnergySwordComponent) target1;
    if (serialization.TryCustomCopy<EnergySwordComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.ActivatedColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.ActivatedColor, hookCtx, context);
    target.ActivatedColor = target2;
    List<Color> target3 = (List<Color>) null;
    if (this.ColorOptions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Color>>(this.ColorOptions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<Color>>(this.ColorOptions, hookCtx, context);
    target.ColorOptions = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hacked, ref target4, hookCtx, false, context))
      target4 = this.Hacked;
    target.Hacked = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CycleRate, ref target5, hookCtx, false, context))
      target5 = this.CycleRate;
    target.CycleRate = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EnergySwordComponent target,
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
    EnergySwordComponent target1 = (EnergySwordComponent) target;
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
    EnergySwordComponent target1 = (EnergySwordComponent) target;
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
    EnergySwordComponent target1 = (EnergySwordComponent) target;
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
  virtual EnergySwordComponent Component.Instantiate() => new EnergySwordComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EnergySwordComponent_AutoState : IComponentState
  {
    public Color ActivatedColor;
    public bool Hacked;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EnergySwordComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EnergySwordComponent, ComponentGetState>(new ComponentEventRefHandler<EnergySwordComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EnergySwordComponent, ComponentHandleState>(new ComponentEventRefHandler<EnergySwordComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EnergySwordComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EnergySwordComponent.EnergySwordComponent_AutoState()
      {
        ActivatedColor = component.ActivatedColor,
        Hacked = component.Hacked
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EnergySwordComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EnergySwordComponent.EnergySwordComponent_AutoState current))
        return;
      component.ActivatedColor = current.ActivatedColor;
      component.Hacked = current.Hacked;
    }
  }
}
