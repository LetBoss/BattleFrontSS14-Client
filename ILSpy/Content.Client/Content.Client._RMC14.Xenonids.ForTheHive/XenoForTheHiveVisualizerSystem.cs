using System;
using Content.Shared._RMC14.Xenonids.ForTheHive;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Xenonids.ForTheHive;

public sealed class XenoForTheHiveVisualizerSystem : VisualizerSystem<ForTheHiveComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ForTheHiveComponent, ForTheHiveActivatedEvent>((EntityEventRefHandler<ForTheHiveComponent, ForTheHiveActivatedEvent>)OnForTheHiveAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ForTheHiveComponent, ForTheHiveCancelledEvent>((EntityEventRefHandler<ForTheHiveComponent, ForTheHiveCancelledEvent>)OnForTheHiveRemoved, (Type[])null, (Type[])null);
	}

	private void OnForTheHiveAdded(Entity<ForTheHiveComponent> xeno, ref ForTheHiveActivatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), (Enum)ForTheHiveVisualLayers.Base, ref num, false) && xeno.Comp.ActiveSprite != null)
		{
			_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), num, 0f);
			_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), num, new ResPath(xeno.Comp.ActiveSprite), (StateId?)null);
		}
	}

	private void OnForTheHiveRemoved(Entity<ForTheHiveComponent> xeno, ref ForTheHiveCancelledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), (Enum)ForTheHiveVisualLayers.Base, ref num, false) && xeno.Comp.BaseSprite != null)
		{
			_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), num, 0f);
			_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((xeno.Owner, item)), num, new ResPath(xeno.Comp.BaseSprite), (StateId?)null);
		}
	}

	protected override void OnAppearanceChange(EntityUid xeno, ForTheHiveComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		float num = default(float);
		int num2 = default(int);
		if (((EntitySystem)this).HasComp<ActiveForTheHiveComponent>(xeno) && sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(xeno, (Enum)ForTheHiveVisuals.Time, ref num, args.Component) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((xeno, sprite)), (Enum)ForTheHiveVisualLayers.Base, ref num2, false) && num >= 0f)
		{
			_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((xeno, sprite)), num2, (float)(component.AnimationTimeBase.TotalSeconds * (double)num));
		}
	}
}
