using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Temperature;

[Serializable]
[NetSerializable]
public enum EntityHeaterSetting
{
	Off,
	Low,
	Medium,
	High
}
