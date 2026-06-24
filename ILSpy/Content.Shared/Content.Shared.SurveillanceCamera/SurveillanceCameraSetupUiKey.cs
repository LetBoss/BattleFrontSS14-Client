using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SurveillanceCamera;

[Serializable]
[NetSerializable]
public enum SurveillanceCameraSetupUiKey : byte
{
	Camera,
	Router
}
