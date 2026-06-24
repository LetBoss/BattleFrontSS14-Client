// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.LightCycleComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class LightCycleComponent : 
  Component,
  ISerializationGenerated<LightCycleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color OriginalColor = Color.Transparent;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromMinutes(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Offset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InitialOffset = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinLightLevel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxLightLevel = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ClipLight = 1.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color ClipLevel = new Color(1f, 1f, 1.25f, 1f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color MinLevel = new Color(0.1f, 0.15f, 0.5f, 1f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color MaxLevel = new Color(2f, 2f, 5f, 1f);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightCycleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightCycleComponent) target1;
    if (serialization.TryCustomCopy<LightCycleComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.OriginalColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.OriginalColor, hookCtx, context);
    target.OriginalColor = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Offset, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Offset, hookCtx, context);
    target.Offset = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target5, hookCtx, false, context))
      target5 = this.Enabled;
    target.Enabled = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.InitialOffset, ref target6, hookCtx, false, context))
      target6 = this.InitialOffset;
    target.InitialOffset = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinLightLevel, ref target7, hookCtx, false, context))
      target7 = this.MinLightLevel;
    target.MinLightLevel = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLightLevel, ref target8, hookCtx, false, context))
      target8 = this.MaxLightLevel;
    target.MaxLightLevel = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClipLight, ref target9, hookCtx, false, context))
      target9 = this.ClipLight;
    target.ClipLight = target9;
    Color target10 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.ClipLevel, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Color>(this.ClipLevel, hookCtx, context);
    target.ClipLevel = target10;
    Color target11 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.MinLevel, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<Color>(this.MinLevel, hookCtx, context);
    target.MinLevel = target11;
    Color target12 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.MaxLevel, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Color>(this.MaxLevel, hookCtx, context);
    target.MaxLevel = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightCycleComponent target,
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
    LightCycleComponent target1 = (LightCycleComponent) target;
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
    LightCycleComponent target1 = (LightCycleComponent) target;
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
    LightCycleComponent target1 = (LightCycleComponent) target;
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
  virtual LightCycleComponent Component.Instantiate() => new LightCycleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LightCycleComponent_AutoState : IComponentState
  {
    public Color OriginalColor;
    public TimeSpan Duration;
    public TimeSpan Offset;
    public bool Enabled;
    public bool InitialOffset;
    public float MinLightLevel;
    public float MaxLightLevel;
    public float ClipLight;
    public Color ClipLevel;
    public Color MinLevel;
    public Color MaxLevel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LightCycleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LightCycleComponent, ComponentGetState>(new ComponentEventRefHandler<LightCycleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LightCycleComponent, ComponentHandleState>(new ComponentEventRefHandler<LightCycleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      LightCycleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new LightCycleComponent.LightCycleComponent_AutoState()
      {
        OriginalColor = component.OriginalColor,
        Duration = component.Duration,
        Offset = component.Offset,
        Enabled = component.Enabled,
        InitialOffset = component.InitialOffset,
        MinLightLevel = component.MinLightLevel,
        MaxLightLevel = component.MaxLightLevel,
        ClipLight = component.ClipLight,
        ClipLevel = component.ClipLevel,
        MinLevel = component.MinLevel,
        MaxLevel = component.MaxLevel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LightCycleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LightCycleComponent.LightCycleComponent_AutoState current))
        return;
      component.OriginalColor = current.OriginalColor;
      component.Duration = current.Duration;
      component.Offset = current.Offset;
      component.Enabled = current.Enabled;
      component.InitialOffset = current.InitialOffset;
      component.MinLightLevel = current.MinLightLevel;
      component.MaxLightLevel = current.MaxLightLevel;
      component.ClipLight = current.ClipLight;
      component.ClipLevel = current.ClipLevel;
      component.MinLevel = current.MinLevel;
      component.MaxLevel = current.MaxLevel;
    }
  }
}
