using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning;

[Serializable]
[NetSerializable]
public enum AcceptCloningUiButton
{
	Deny,
	Accept
}
