using System;
using System.Numerics;
using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Xenonids.Hide;
using Content.Shared.DrawDepth;
using Content.Shared.Ghost;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteSystem : SharedRMCSpriteSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private TransformSystem _transform;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCMobStateDrawDepthComponent, AppearanceChangeEvent>((EntityEventRefHandler<RMCMobStateDrawDepthComponent, AppearanceChangeEvent>)OnDrawDepthAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnDrawDepthAppearanceChange(Entity<RMCMobStateDrawDepthComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (args.AppearanceData.ContainsKey(RMCSpriteDrawDepth.Key))
		{
			UpdateDrawDepth(Entity<RMCMobStateDrawDepthComponent>.op_Implicit(ent));
		}
	}

	public override DrawDepth UpdateDrawDepth(EntityUid sprite)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		DrawDepth drawDepth = base.UpdateDrawDepth(sprite);
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(sprite, ref item))
		{
			return drawDepth;
		}
		_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((sprite, item)), (int)drawDepth);
		return drawDepth;
	}

	public void UpdatePosition(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(uid);
		if (((EntitySystem)this).Transform(uid).MapID == MapId.Nullspace)
		{
			SpriteComponent item = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
			{
				_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), default(Vector2));
			}
		}
		else
		{
			Vector2 vector = worldPosition;
			vector.Y = worldPosition.Y + 0.0001f;
			Vector2 vector2 = vector;
			((SharedTransformSystem)_transform).SetWorldPosition(uid, vector2);
		}
	}

	public override void Update(float frameTime)
	{
		UpdateColors();
		UpdatePositions();
		UpdateLocalDrawDepth();
	}

	private void UpdateColors()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EntityQueryEnumerator<SpriteColorComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<SpriteColorComponent, SpriteComponent>();
			EntityUid item = default(EntityUid);
			SpriteColorComponent spriteColorComponent = default(SpriteColorComponent);
			SpriteComponent item2 = default(SpriteComponent);
			while (val.MoveNext(ref item, ref spriteColorComponent, ref item2))
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((item, item2)), spriteColorComponent.Color);
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error updating {"SpriteColorComponent"} colors:\n{value}");
		}
	}

	private void UpdatePositions()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EntityQueryEnumerator<RMCUpdateClientLocationComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCUpdateClientLocationComponent>();
			EntityUid uid = default(EntityUid);
			RMCUpdateClientLocationComponent rMCUpdateClientLocationComponent = default(RMCUpdateClientLocationComponent);
			while (val.MoveNext(ref uid, ref rMCUpdateClientLocationComponent))
			{
				UpdatePosition(uid);
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error updating {"RMCUpdateClientLocationComponent"} positions:\n{value}");
		}
	}

	private void UpdateLocalDrawDepth()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				XenoHideComponent xenoHideComponent = default(XenoHideComponent);
				SpriteComponent item = default(SpriteComponent);
				if (!((EntitySystem)this).HasComp<GhostComponent>(valueOrDefault) && (!((EntitySystem)this).TryComp<XenoHideComponent>(valueOrDefault, ref xenoHideComponent) || !xenoHideComponent.Hiding) && ((EntitySystem)this).TryComp<SpriteComponent>(valueOrDefault, ref item) && !((EntitySystem)this).HasComp<ParaDroppingComponent>(valueOrDefault) && !((EntitySystem)this).HasComp<CrashLandingComponent>(valueOrDefault))
				{
					_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((valueOrDefault, item)), 5);
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error updating local draw depth:\n{value}");
		}
	}
}
