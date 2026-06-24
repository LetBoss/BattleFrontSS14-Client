// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasAnalyzerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class GasAnalyzerComponent : 
  Component,
  ISerializationGenerated<GasAnalyzerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Target;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid User;
  [DataField("enabled", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Enabled;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasAnalyzerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasAnalyzerComponent) component;
    if (serialization.TryCustomCopy<GasAnalyzerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasAnalyzerComponent target,
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
    GasAnalyzerComponent target1 = (GasAnalyzerComponent) target;
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
    GasAnalyzerComponent target1 = (GasAnalyzerComponent) target;
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
    GasAnalyzerComponent target1 = (GasAnalyzerComponent) target;
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
  virtual GasAnalyzerComponent Component.Instantiate() => new GasAnalyzerComponent();

  [NetSerializable]
  [Serializable]
  public enum GasAnalyzerUiKey
  {
    Key,
  }

  [NetSerializable]
  [Serializable]
  public sealed class GasAnalyzerUserMessage : BoundUserInterfaceMessage
  {
    public string DeviceName;
    public NetEntity DeviceUid;
    public bool DeviceFlipped;
    public string? Error;
    public GasAnalyzerComponent.GasMixEntry[] NodeGasMixes;

    public GasAnalyzerUserMessage(
      GasAnalyzerComponent.GasMixEntry[] nodeGasMixes,
      string deviceName,
      NetEntity deviceUid,
      bool deviceFlipped,
      string? error = null)
    {
      this.NodeGasMixes = nodeGasMixes;
      this.DeviceName = deviceName;
      this.DeviceUid = deviceUid;
      this.DeviceFlipped = deviceFlipped;
      this.Error = error;
    }
  }

  [NetSerializable]
  [Serializable]
  public struct GasMixEntry(
    string name,
    float volume,
    float pressure,
    float temperature,
    GasAnalyzerComponent.GasEntry[]? gases = null)
  {
    public readonly string Name = name;
    public readonly float Volume = volume;
    public readonly float Pressure = pressure;
    public readonly float Temperature = temperature;
    public readonly GasAnalyzerComponent.GasEntry[]? Gases = gases;
  }

  [NetSerializable]
  [Serializable]
  public struct GasEntry(string name, float amount, string color)
  {
    public readonly string Name = name;
    public readonly float Amount = amount;
    public readonly string Color = color;

    public override string ToString()
    {
      return Loc.GetString("gas-entry-info", new (string, object)[2]
      {
        ("gasName", (object) this.Name),
        ("gasAmount", (object) this.Amount)
      });
    }
  }
}
