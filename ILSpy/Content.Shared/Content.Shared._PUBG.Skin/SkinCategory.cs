using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public enum SkinCategory
{
	Jumpsuit,
	OuterClothing,
	Shoes,
	Neck,
	Head,
	Ghost,
	AllItems
}
