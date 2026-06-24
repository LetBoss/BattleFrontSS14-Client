// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.SolverData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Physics.Dynamics;

internal readonly record struct SolverData(
  float FrameTime,
  float DtRatio,
  float InvDt,
  bool WarmStarting,
  float MaxLinearCorrection,
  float MaxAngularCorrection,
  int VelocityIterations,
  int PositionIterations,
  float MaxLinearVelocity,
  float MaxAngularVelocity,
  float MaxTranslation,
  float MaxRotation,
  bool SleepAllowed,
  float AngTolSqr,
  float LinTolSqr,
  float TimeToSleep,
  float VelocityThreshold,
  float Baumgarte)
;
