using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Paper;

public sealed class BlockWritingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlockWritingComponent, PaperWriteAttemptEvent>((EntityEventRefHandler<BlockWritingComponent, PaperWriteAttemptEvent>)OnPaperWriteAttempt, (Type[])null, (Type[])null);
	}

	private void OnPaperWriteAttempt(Entity<BlockWritingComponent> entity, ref PaperWriteAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.FailReason = LocId.op_Implicit(entity.Comp.FailWriteMessage);
		args.Cancelled = true;
	}
}
