using System;
using Content.Shared.Foldable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Folded;

public sealed class RMCFoldableSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, FoldAttemptEvent>((EntityEventRefHandler<FoldableComponent, FoldAttemptEvent>)OnFolded, (Type[])null, (Type[])null);
	}

	private void OnFolded(Entity<FoldableComponent> ent, ref FoldAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.AnchorOnUnfold && !args.Cancelled)
		{
			if (args.Comp.IsFolded)
			{
				_transform.AnchorEntity(Entity<FoldableComponent>.op_Implicit(ent));
			}
			else
			{
				_transform.Unanchor(Entity<FoldableComponent>.op_Implicit(ent));
			}
		}
	}

	public bool TryLockFold(EntityUid uid, bool locked, FoldableComponent? foldableComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FoldableComponent>(uid, ref foldableComp, false))
		{
			return false;
		}
		foldableComp.IsLocked = locked;
		return true;
	}
}
