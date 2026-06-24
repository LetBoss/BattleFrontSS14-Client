using System;
using Content.Client.IconSmoothing;
using Content.Shared.Chemistry.Components;
using Content.Shared.Fluids;
using Content.Shared.Fluids.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Fluids;

public sealed class PuddleSystem : SharedPuddleSystem
{
	[Dependency]
	private IconSmoothSystem _smooth;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PuddleComponent, AppearanceChangeEvent>((ComponentEventRefHandler<PuddleComponent, AppearanceChangeEvent>)OnPuddleAppearance, (Type[])null, (Type[])null);
	}

	private void OnPuddleAppearance(EntityUid uid, PuddleComponent component, ref AppearanceChangeEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null)
		{
			return;
		}
		float num = 1f;
		if (args.AppearanceData.TryGetValue(PuddleVisuals.CurrentVolume, out var value))
		{
			num = (float)value;
		}
		IconSmoothComponent iconSmoothComponent = default(IconSmoothComponent);
		if (((EntitySystem)this).TryComp<IconSmoothComponent>(uid, ref iconSmoothComponent))
		{
			if (num < 0.3f)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(iconSmoothComponent.StateBase + "a"));
				_smooth.SetEnabled(uid, value: false, iconSmoothComponent);
			}
			else if (num < 0.6f)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(iconSmoothComponent.StateBase + "b"));
				_smooth.SetEnabled(uid, value: false, iconSmoothComponent);
			}
			else if (!iconSmoothComponent.Enabled)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit(iconSmoothComponent.StateBase + "0"));
				_smooth.SetEnabled(uid, value: true, iconSmoothComponent);
				_smooth.DirtyNeighbours(uid);
			}
		}
		Color white = Color.White;
		if (args.AppearanceData.TryGetValue(PuddleVisuals.SolutionColor, out var value2))
		{
			Color val = (Color)value2;
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (ref val) * (ref white));
			return;
		}
		SpriteSystem sprite = _sprite;
		Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((uid, args.Sprite));
		Color color = args.Sprite.Color;
		sprite.SetColor(val2, (ref color) * (ref white));
	}

	public override bool TrySplashSpillAt(EntityUid uid, EntityCoordinates coordinates, Solution solution, out EntityUid puddleUid, bool sound = true, EntityUid? user = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		puddleUid = EntityUid.Invalid;
		return false;
	}

	public override bool TrySpillAt(EntityCoordinates coordinates, Solution solution, out EntityUid puddleUid, bool sound = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		puddleUid = EntityUid.Invalid;
		return false;
	}

	public override bool TrySpillAt(EntityUid uid, Solution solution, out EntityUid puddleUid, bool sound = true, TransformComponent? transformComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		puddleUid = EntityUid.Invalid;
		return false;
	}

	public override bool TrySpillAt(TileRef tileRef, Solution solution, out EntityUid puddleUid, bool sound = true, bool tileReact = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		puddleUid = EntityUid.Invalid;
		return false;
	}
}
