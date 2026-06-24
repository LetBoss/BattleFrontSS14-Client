using System;
using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared.DrawDepth;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Hide;

public sealed class XenoHideVisualizerSystem : VisualizerSystem<XenoHideComponent>
{
	[Dependency]
	private RMCSpriteSystem _rmcSprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoHideComponent, GetDrawDepthEvent>((EntityEventRefHandler<XenoHideComponent, GetDrawDepthEvent>)OnXenoHideGetDrawDepth, new Type[1] { typeof(XenoVisualizerSystem) }, (Type[])null);
	}

	private void OnXenoHideGetDrawDepth(Entity<XenoHideComponent> ent, ref GetDrawDepthEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(Entity<XenoHideComponent>.op_Implicit(ent), (Enum)XenoVisualLayers.Hide, ref flag, (AppearanceComponent)null) && flag)
		{
			args.DrawDepth = DrawDepth.Walls;
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, XenoHideComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_rmcSprite.UpdateDrawDepth(uid);
	}
}
