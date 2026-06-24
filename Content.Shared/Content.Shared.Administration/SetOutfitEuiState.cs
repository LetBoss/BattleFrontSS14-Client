using System;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class SetOutfitEuiState : EuiStateBase
{
	public NetEntity TargetNetEntity;
}
