using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.BUI;

[Serializable]
[NetSerializable]
public sealed class CargoPalletConsoleInterfaceState : BoundUserInterfaceState
{
	public int Appraisal;

	public int Count;

	public bool Enabled;

	public CargoPalletConsoleInterfaceState(int appraisal, int count, bool enabled)
	{
		Appraisal = appraisal;
		Count = count;
		Enabled = enabled;
	}
}
