namespace Robust.Shared.Physics.Dynamics;

internal readonly record struct SolverData(float FrameTime, float DtRatio, float InvDt, bool WarmStarting, float MaxLinearCorrection, float MaxAngularCorrection, int VelocityIterations, int PositionIterations, float MaxLinearVelocity, float MaxAngularVelocity, float MaxTranslation, float MaxRotation, bool SleepAllowed, float AngTolSqr, float LinTolSqr, float TimeToSleep, float VelocityThreshold, float Baumgarte);
