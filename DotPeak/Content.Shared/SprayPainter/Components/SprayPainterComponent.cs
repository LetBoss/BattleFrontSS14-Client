// Decompiled with JetBrains decompiler
// Type: Content.Shared.SprayPainter.Components.SprayPainterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.SprayPainter.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SprayPainterComponent : 
  Component,
  ISerializationGenerated<SprayPainterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SpraySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/spray2.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan AirlockSprayTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PipeSprayTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? PickedColor;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Color> ColorPalette = new Dictionary<string, Color>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Index;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SprayPainterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SprayPainterComponent) target1;
    if (serialization.TryCustomCopy<SprayPainterComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.SpraySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpraySound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.SpraySound, hookCtx, context);
    target.SpraySound = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AirlockSprayTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.AirlockSprayTime, hookCtx, context);
    target.AirlockSprayTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PipeSprayTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.PipeSprayTime, hookCtx, context);
    target.PipeSprayTime = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PickedColor, ref target5, hookCtx, false, context))
      target5 = this.PickedColor;
    target.PickedColor = target5;
    Dictionary<string, Color> target6 = (Dictionary<string, Color>) null;
    if (this.ColorPalette == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Color>>(this.ColorPalette, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<string, Color>>(this.ColorPalette, hookCtx, context);
    target.ColorPalette = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Index, ref target7, hookCtx, false, context))
      target7 = this.Index;
    target.Index = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SprayPainterComponent target,
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
    SprayPainterComponent target1 = (SprayPainterComponent) target;
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
    SprayPainterComponent target1 = (SprayPainterComponent) target;
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
    SprayPainterComponent target1 = (SprayPainterComponent) target;
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
  virtual SprayPainterComponent Component.Instantiate() => new SprayPainterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SprayPainterComponent_AutoState : IComponentState
  {
    public string? PickedColor;
    public int Index;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SprayPainterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SprayPainterComponent, ComponentGetState>(new ComponentEventRefHandler<SprayPainterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SprayPainterComponent, ComponentHandleState>(new ComponentEventRefHandler<SprayPainterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SprayPainterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SprayPainterComponent.SprayPainterComponent_AutoState()
      {
        PickedColor = component.PickedColor,
        Index = component.Index
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SprayPainterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SprayPainterComponent.SprayPainterComponent_AutoState current))
        return;
      component.PickedColor = current.PickedColor;
      component.Index = current.Index;
    }
  }
}
