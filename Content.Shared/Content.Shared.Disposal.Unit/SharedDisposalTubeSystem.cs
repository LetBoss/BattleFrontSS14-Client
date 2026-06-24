using System.Collections.Generic;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Tube;
using Robust.Shared.GameObjects;

namespace Content.Shared.Disposal.Unit;

public abstract class SharedDisposalTubeSystem : EntitySystem
{
	public virtual bool TryInsert(EntityUid uid, DisposalUnitComponent from, IEnumerable<string>? tags = null, DisposalEntryComponent? entry = null)
	{
		return false;
	}
}
