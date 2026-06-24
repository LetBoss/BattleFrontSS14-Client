using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Mining.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Mining;

public sealed class MiningOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	private readonly EntityLookupSystem _lookup;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _xform;

	private readonly EntityQuery<SpriteComponent> _spriteQuery;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly HashSet<Entity<MiningScannerViewableComponent>> _viewableEnts = new HashSet<Entity<MiningScannerViewableComponent>>();

	public override OverlaySpace Space => (OverlaySpace)4;

	public override bool RequestScreenTexture => false;

	public MiningOverlay()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<MiningOverlay>(this);
		_lookup = _entityManager.System<EntityLookupSystem>();
		_sprite = _entityManager.System<SpriteSystem>();
		_xform = _entityManager.System<TransformSystem>();
		_spriteQuery = _entityManager.GetEntityQuery<SpriteComponent>();
		_xformQuery = _entityManager.GetEntityQuery<TransformComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		MiningScannerViewerComponent miningScannerViewerComponent = default(MiningScannerViewerComponent);
		if (!_entityManager.TryGetComponent<MiningScannerViewerComponent>(valueOrDefault, ref miningScannerViewerComponent) || !miningScannerViewerComponent.LastPingLocation.HasValue)
		{
			return;
		}
		Vector2 one = Vector2.One;
		Matrix3x2 value = Matrix3Helpers.CreateScale(ref one);
		_viewableEnts.Clear();
		_lookup.GetEntitiesInRange<MiningScannerViewableComponent>(miningScannerViewerComponent.LastPingLocation.Value, miningScannerViewerComponent.ViewRange, _viewableEnts, (LookupFlags)110);
		TransformComponent val = default(TransformComponent);
		SpriteComponent val2 = default(SpriteComponent);
		int num = default(int);
		foreach (Entity<MiningScannerViewableComponent> viewableEnt in _viewableEnts)
		{
			if (!_xformQuery.TryComp(Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), ref val) || !_spriteQuery.TryComp(Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), ref val2) || val.MapID != args.MapId || !val2.Visible || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<MiningScannerViewableComponent>.op_Implicit(viewableEnt), val2)), (Enum)MiningScannerVisualLayers.Overlay, ref num, false))
			{
				continue;
			}
			ISpriteLayer val3 = val2[num];
			RSI actualRsi = val3.ActualRsi;
			if (actualRsi == null)
			{
				continue;
			}
			_ = actualRsi.Path;
			if (0 == 0 && val3.RsiState.Name != null)
			{
				_003F val4;
				if (val.GridUid.HasValue)
				{
					TransformComponent obj = _xformQuery.CompOrNull(val.GridUid.Value);
					val4 = ((obj != null) ? obj.LocalRotation : Angle.op_Implicit(0f));
				}
				else
				{
					val4 = Angle.op_Implicit(0f);
				}
				Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit((Angle)val4));
				Matrix3x2 value3 = Matrix3Helpers.CreateTranslation(((SharedTransformSystem)_xform).GetWorldPosition(val));
				Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				Rsi val5 = new Rsi(val3.ActualRsi.Path, val3.RsiState.Name);
				Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val5, TimeSpan.FromSeconds(val3.AnimationTime), true);
				double totalSeconds = (miningScannerViewerComponent.NextPingTime - _timing.CurTime).TotalSeconds;
				float num2 = ((totalSeconds < (double)miningScannerViewerComponent.AnimationDuration) ? 0f : ((float)Math.Clamp((totalSeconds - (double)miningScannerViewerComponent.AnimationDuration) / (double)miningScannerViewerComponent.AnimationDuration, 0.0, 1.0)));
				Color white = Color.White;
				Color value5 = ((Color)(ref white)).WithAlpha(num2);
				worldHandle.DrawTexture(frame, -Vector2i.op_Implicit(frame.Size) / 2f / 32f, val3.Rotation, (Color?)value5);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}
}
