using System;
using Content.Client.Rotation;
using Content.Shared._RMC14.Buckle;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Rotation;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Buckle;

internal sealed class BuckleSystem : SharedBuckleSystem
{
	[Dependency]
	private RotationVisualizerSystem _rotationVisualizerSystem;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private SharedTransformSystem _xformSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, AppearanceChangeEvent>((ComponentEventRefHandler<BuckleComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, MoveEvent>((ComponentEventRefHandler<StrapComponent, MoveEvent>)OnStrapMoveEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, BuckledEvent>((EntityEventRefHandler<BuckleComponent, BuckledEvent>)OnBuckledEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, UnbuckledEvent>((EntityEventRefHandler<BuckleComponent, UnbuckledEvent>)OnUnbuckledEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BuckleComponent, AttemptMobCollideEvent>((EntityEventRefHandler<BuckleComponent, AttemptMobCollideEvent>)OnMobCollide, (Type[])null, (Type[])null);
	}

	private void OnMobCollide(Entity<BuckleComponent> ent, ref AttemptMobCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Buckled)
		{
			args.Cancelled = true;
		}
	}

	private void OnStrapMoveEvent(EntityUid uid, StrapComponent component, ref MoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).HasComp<RMCStrapNoDrawDepthChangeComponent>(uid) || args.NewRotation == args.OldRotation || !((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			return;
		}
		Angle val2 = _xformSystem.GetWorldRotation(uid) + _eye.CurrentEye.Rotation;
		bool flag = (int)((Angle)(ref val2)).GetCardinalDir() == 4;
		BuckleComponent buckleComponent = default(BuckleComponent);
		SpriteComponent val3 = default(SpriteComponent);
		foreach (EntityUid buckledEntity in component.BuckledEntities)
		{
			if (!((EntitySystem)this).TryComp<BuckleComponent>(buckledEntity, ref buckleComponent) || !((EntitySystem)this).TryComp<SpriteComponent>(buckledEntity, ref val3))
			{
				continue;
			}
			if (flag)
			{
				BuckleComponent buckleComponent2 = buckleComponent;
				int valueOrDefault = buckleComponent2.OriginalDrawDepth.GetValueOrDefault();
				if (!buckleComponent2.OriginalDrawDepth.HasValue)
				{
					valueOrDefault = val3.DrawDepth;
					buckleComponent2.OriginalDrawDepth = valueOrDefault;
				}
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((buckledEntity, val3)), val.DrawDepth - 1);
			}
			else if (buckleComponent.OriginalDrawDepth.HasValue)
			{
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((buckledEntity, val3)), buckleComponent.OriginalDrawDepth.Value);
				buckleComponent.OriginalDrawDepth = null;
			}
		}
	}

	private void OnBuckledEvent(Entity<BuckleComponent> ent, ref BuckledEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		SpriteComponent val2 = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<StrapComponent>.op_Implicit(args.Strap), ref val) || !((EntitySystem)this).TryComp<SpriteComponent>(ent.Owner, ref val2))
		{
			return;
		}
		Angle val3 = _xformSystem.GetWorldRotation(Entity<StrapComponent>.op_Implicit(args.Strap)) + _eye.CurrentEye.Rotation;
		if ((int)((Angle)(ref val3)).GetCardinalDir() == 4)
		{
			BuckleComponent comp = ent.Comp;
			int valueOrDefault = comp.OriginalDrawDepth.GetValueOrDefault();
			if (!comp.OriginalDrawDepth.HasValue)
			{
				valueOrDefault = val2.DrawDepth;
				comp.OriginalDrawDepth = valueOrDefault;
			}
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ent.Owner, val2)), val.DrawDepth - 1);
		}
	}

	private void OnUnbuckledEvent(Entity<BuckleComponent> ent, ref UnbuckledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(ent.Owner, ref item) && ent.Comp.OriginalDrawDepth.HasValue)
		{
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), ent.Comp.OriginalDrawDepth.Value);
			ent.Comp.OriginalDrawDepth = null;
		}
	}

	private void OnAppearanceChange(EntityUid uid, BuckleComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		RotationVisualsComponent rotationVisualsComponent = default(RotationVisualsComponent);
		if (((EntitySystem)this).TryComp<RotationVisualsComponent>(uid, ref rotationVisualsComponent))
		{
			bool flag = default(bool);
			if (!Appearance.TryGetData<bool>(uid, (Enum)BuckleVisuals.Buckled, ref flag, args.Component) || !flag || args.Sprite == null)
			{
				_rotationVisualizerSystem.SetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit((uid, rotationVisualsComponent)), rotationVisualsComponent.DefaultRotation);
			}
			else
			{
				_rotationVisualizerSystem.AnimateSpriteRotation(uid, args.Sprite, rotationVisualsComponent.HorizontalRotation, 0.125f);
			}
		}
	}
}
