using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Laws;

[Serializable]
[NetSerializable]
public sealed class SiliconLawsEuiState : EuiStateBase
{
	public List<SiliconLaw> Laws { get; }

	public NetEntity Target { get; }

	public SiliconLawsEuiState(List<SiliconLaw> laws, NetEntity target)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Laws = laws;
		Target = target;
	}
}
