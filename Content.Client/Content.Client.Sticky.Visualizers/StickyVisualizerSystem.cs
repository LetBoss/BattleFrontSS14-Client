using System;
using Content.Shared.Sticky.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Sticky.Visualizers;

public sealed class StickyVisualizerSystem : VisualizerSystem<StickyVisualizerComponent>
{
	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<StickyVisualizerComponent, ComponentInit>((EntityEventRefHandler<StickyVisualizerComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<StickyVisualizerComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (_spriteQuery.TryComp(Entity<StickyVisualizerComponent>.op_Implicit(ent), ref val))
		{
			ent.Comp.OriginalDrawDepth = val.DrawDepth;
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, StickyVisualizerComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)StickyVisuals.IsStuck, ref flag, args.Component))
		{
			int num = (flag ? comp.StuckDrawDepth : comp.OriginalDrawDepth);
			base.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num);
		}
	}
}
