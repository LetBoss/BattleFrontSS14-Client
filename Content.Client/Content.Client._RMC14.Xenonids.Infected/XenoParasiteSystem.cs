using System;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.DrawDepth;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Infected;

public sealed class XenoParasiteSystem : SharedXenoParasiteSystem
{
	[Dependency]
	private XenoVisualizerSystem _xenoVisualizer;

	[Dependency]
	private TagSystem _tags;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteComponent, GetDrawDepthEvent>((EntityEventRefHandler<XenoParasiteComponent, GetDrawDepthEvent>)OnGetParasiteDrawDepth, new Type[1] { typeof(XenoHideSystem) }, (Type[])null);
	}

	private void OnGetParasiteDrawDepth(Entity<XenoParasiteComponent> parasite, ref GetDrawDepthEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_tags.HasTag(Entity<XenoParasiteComponent>.op_Implicit(parasite), ParasiteIsPreparingLeapProtoID) || ((EntitySystem)this).HasComp<XenoLeapingComponent>(Entity<XenoParasiteComponent>.op_Implicit(parasite)))
		{
			args.DrawDepth = DrawDepth.Overdoors;
		}
		else
		{
			args.DrawDepth = DrawDepth.Mobs;
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityQueryEnumerator<XenoComponent, ThrownItemComponent, SpriteComponent, AppearanceComponent> val = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent, ThrownItemComponent, SpriteComponent, AppearanceComponent>();
		EntityUid item = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		ThrownItemComponent item2 = default(ThrownItemComponent);
		SpriteComponent item3 = default(SpriteComponent);
		AppearanceComponent item4 = default(AppearanceComponent);
		while (val.MoveNext(ref item, ref xenoComponent, ref item2, ref item3, ref item4))
		{
			_xenoVisualizer.UpdateSprite(Entity<SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent, XenoLeapingComponent, KnockedDownComponent>.op_Implicit((ValueTuple<EntityUid, SpriteComponent, MobStateComponent, AppearanceComponent, InputMoverComponent, ThrownItemComponent>)(item, item3, null, item4, null, item2)));
		}
	}
}
