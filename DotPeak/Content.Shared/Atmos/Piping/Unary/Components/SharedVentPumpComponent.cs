// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Unary.Components.GasVentPumpData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Piping.Unary.Components;

[NetSerializable]
[Serializable]
public sealed class GasVentPumpData : IAtmosDeviceData
{
  public static GasVentPumpData FilterModePreset = new GasVentPumpData()
  {
    Enabled = true,
    PumpDirection = VentPumpDirection.Releasing,
    PressureChecks = VentPressureBound.ExternalBound,
    ExternalPressureBound = 101.325f,
    InternalPressureBound = 0.0f,
    PressureLockoutOverride = false
  };
  public static GasVentPumpData FillModePreset = new GasVentPumpData()
  {
    Enabled = true,
    Dirty = true,
    PumpDirection = VentPumpDirection.Releasing,
    PressureChecks = VentPressureBound.ExternalBound,
    ExternalPressureBound = 5066.25f,
    InternalPressureBound = 0.0f,
    PressureLockoutOverride = true
  };
  public static GasVentPumpData PanicModePreset = new GasVentPumpData()
  {
    Enabled = false,
    Dirty = true,
    PumpDirection = VentPumpDirection.Releasing,
    PressureChecks = VentPressureBound.ExternalBound,
    ExternalPressureBound = 101.325f,
    InternalPressureBound = 0.0f,
    PressureLockoutOverride = false
  };
  public static GasVentPumpData ReplaceModePreset = new GasVentPumpData()
  {
    Enabled = false,
    IgnoreAlarms = true,
    Dirty = true,
    PumpDirection = VentPumpDirection.Releasing,
    PressureChecks = VentPressureBound.ExternalBound,
    ExternalPressureBound = 101.325f,
    InternalPressureBound = 0.0f,
    PressureLockoutOverride = false
  };

  public bool Enabled { get; set; }

  public bool Dirty { get; set; }

  public bool IgnoreAlarms { get; set; }

  public VentPumpDirection PumpDirection { get; set; } = VentPumpDirection.Releasing;

  public VentPressureBound PressureChecks { get; set; } = VentPressureBound.ExternalBound;

  public float ExternalPressureBound { get; set; } = 101.325f;

  public float InternalPressureBound { get; set; }

  public bool PressureLockoutOverride { get; set; }
}
