using System;
using Content.Client._RMC14.Sprite;
using Content.Client._RMC14.Xenonids;
using Content.Shared._RMC14.Buckle;
using Content.Shared._RMC14.Sprite;
using Content.Shared.Buckle.Components;
using Content.Shared.DrawDepth;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Buckle;

public sealed class RMCBuckleVisualsSystem : EntitySystem
{
	[Dependency]
	private RMCSpriteSystem _rmcSprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<BuckleComponent, AfterAutoHandleStateEvent>)OnBuckleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCBuckleDrawDepthComponent, GetDrawDepthEvent>((EntityEventRefHandler<RMCBuckleDrawDepthComponent, GetDrawDepthEvent>)OnGetDrawDepth, (Type[])null, new Type[1] { typeof(XenoVisualizerSystem) });
	}

	private void OnBuckleState(Entity<BuckleComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_rmcSprite.UpdateDrawDepth(ent.Owner);
	}

	private void OnGetDrawDepth(Entity<RMCBuckleDrawDepthComponent> ent, ref GetDrawDepthEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckleComponent = default(BuckleComponent);
		StrapComponent item = default(StrapComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(Entity<RMCBuckleDrawDepthComponent>.op_Implicit(ent), ref buckleComponent) && ((EntitySystem)this).TryComp<StrapComponent>(buckleComponent.BuckledTo, ref item))
		{
			int? drawDepth = GetDrawDepth(Entity<BuckleComponent>.op_Implicit((Entity<RMCBuckleDrawDepthComponent>.op_Implicit(ent), buckleComponent)), Entity<StrapComponent>.op_Implicit((buckleComponent.BuckledTo.Value, item)));
			if (drawDepth.HasValue)
			{
				int valueOrDefault = drawDepth.GetValueOrDefault();
				args.DrawDepth = (DrawDepth)valueOrDefault;
			}
		}
	}

	private int? GetDrawDepth(Entity<BuckleComponent> buckle, Entity<StrapComponent> strap)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		int? result = (int?)((EntitySystem)this).CompOrNull<RMCBuckleDrawDepthComponent>(Entity<BuckleComponent>.op_Implicit(buckle))?.BuckleDepth;
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<StrapComponent>.op_Implicit(strap), ref val))
		{
			return null;
		}
		if (((EntitySystem)this).HasComp<RMCStrapNoDrawDepthChangeComponent>(Entity<StrapComponent>.op_Implicit(strap)) && !result.HasValue)
		{
			result = val.DrawDepth + 1;
		}
		if (!result.HasValue)
		{
			Angle localRotation = ((EntitySystem)this).Transform(Entity<StrapComponent>.op_Implicit(strap)).LocalRotation;
			result = (((int)((Angle)(ref localRotation)).GetCardinalDir() != 4) ? new int?(val.DrawDepth + 1) : new int?(val.DrawDepth - 1));
		}
		return result;
	}
}
