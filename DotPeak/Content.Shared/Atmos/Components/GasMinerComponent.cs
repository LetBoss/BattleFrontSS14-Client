// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasMinerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[RegisterComponent]
public sealed class GasMinerComponent : 
  Component,
  ISerializationGenerated<GasMinerComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public GasMinerState MinerState;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float MaxExternalAmount = float.PositiveInfinity;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float MaxExternalPressure = 6500f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, true, false, null)]
  public Gas SpawnGas;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float SpawnTemperature = 293.15f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float SpawnAmount = 2078.55981f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasMinerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasMinerComponent) component;
    if (serialization.TryCustomCopy<GasMinerComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxExternalAmount, ref num1, hookCtx, false, context))
      num1 = this.MaxExternalAmount;
    target.MaxExternalAmount = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxExternalPressure, ref num2, hookCtx, false, context))
      num2 = this.MaxExternalPressure;
    target.MaxExternalPressure = num2;
    Gas gas = Gas.Oxygen;
    if (!serialization.TryCustomCopy<Gas>(this.SpawnGas, ref gas, hookCtx, false, context))
      gas = this.SpawnGas;
    target.SpawnGas = gas;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpawnTemperature, ref num3, hookCtx, false, context))
      num3 = this.SpawnTemperature;
    target.SpawnTemperature = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpawnAmount, ref num4, hookCtx, false, context))
      num4 = this.SpawnAmount;
    target.SpawnAmount = num4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasMinerComponent target,
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
    GasMinerComponent target1 = (GasMinerComponent) target;
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
    GasMinerComponent target1 = (GasMinerComponent) target;
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
    GasMinerComponent target1 = (GasMinerComponent) target;
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
  virtual GasMinerComponent Component.Instantiate() => new GasMinerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasMinerComponent_AutoState : IComponentState
  {
    public GasMinerState MinerState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasMinerComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasMinerComponent, ComponentGetState>(new ComponentEventRefHandler<GasMinerComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasMinerComponent, ComponentHandleState>(new ComponentEventRefHandler<GasMinerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, GasMinerComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasMinerComponent.GasMinerComponent_AutoState()
      {
        MinerState = component.MinerState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasMinerComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasMinerComponent.GasMinerComponent_AutoState current))
        return;
      component.MinerState = current.MinerState;
    }
  }
}
