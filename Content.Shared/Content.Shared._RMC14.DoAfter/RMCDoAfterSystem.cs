using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.DoAfter;

public sealed class RMCDoAfterSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	public bool ShouldCancel(Content.Shared.DoAfter.DoAfter doAfter)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (doAfter.Args.BreakOnRest && ((EntitySystem)this).HasComp<XenoRestingComponent>(doAfter.Args.User))
		{
			return true;
		}
		return false;
	}

	public void TryCancel(Entity<DoAfterComponent?> ent, ushort? doAfterIndex)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (doAfterIndex.HasValue && ((EntitySystem)this).Resolve<DoAfterComponent>(Entity<DoAfterComponent>.op_Implicit(ent), ref ent.Comp, false) && ent.Comp.DoAfters.ContainsKey(doAfterIndex.Value))
		{
			_doAfter.Cancel(Entity<DoAfterComponent>.op_Implicit(ent), doAfterIndex.Value, Entity<DoAfterComponent>.op_Implicit(ent));
		}
	}
}
