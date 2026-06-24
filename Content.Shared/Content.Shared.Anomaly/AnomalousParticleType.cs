using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly;

[Serializable]
[NetSerializable]
public enum AnomalousParticleType : byte
{
	Delta,
	Epsilon,
	Zeta,
	Sigma,
	Default
}
