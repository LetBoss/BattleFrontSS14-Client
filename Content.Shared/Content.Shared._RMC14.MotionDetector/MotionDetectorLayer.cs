using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.MotionDetector;

[Serializable]
[NetSerializable]
public enum MotionDetectorLayer
{
	Setting,
	Number
}
