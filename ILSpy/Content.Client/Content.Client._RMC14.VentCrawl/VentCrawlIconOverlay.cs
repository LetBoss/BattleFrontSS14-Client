using System.Numerics;
using Content.Client.SubFloor;
using Content.Shared._RMC14.Vents;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.VentCrawl;

public sealed class VentCrawlIconOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IGameTiming _timing;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly ContainerSystem _container;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly ResPath _rsiPath = new ResPath("/Textures/_RMC14/Interface/vent_crawl.rsi");

	public override OverlaySpace Space => (OverlaySpace)16;

	public VentCrawlIconOverlay()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<VentCrawlIconOverlay>(this);
		_container = _entity.System<ContainerSystem>();
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
		((Overlay)this).ZIndex = -5;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (!_entity.HasComponent<VentSightComponent>(((ISharedPlayerManager)_players).LocalEntity))
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		Matrix3x2 scale = Matrix3x2.CreateScale(new Vector2(1f, 1f));
		Matrix3x2 rotate = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		AllEntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent, TransformComponent, SpriteComponent> val2 = _entity.AllEntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent, TransformComponent, SpriteComponent>();
		EntityUid val3 = default(EntityUid);
		VentCrawlingComponent ventCrawlingComponent = default(VentCrawlingComponent);
		VentCrawlerComponent crawler = default(VentCrawlerComponent);
		TransformComponent transform = default(TransformComponent);
		SpriteComponent sprite = default(SpriteComponent);
		BaseContainer val5 = default(BaseContainer);
		SubFloorHideComponent subFloorHideComponent = default(SubFloorHideComponent);
		while (val2.MoveNext(ref val3, ref ventCrawlingComponent, ref crawler, ref transform, ref sprite))
		{
			EntityUid val4 = val3;
			EntityUid? localEntity = ((ISharedPlayerManager)_players).LocalEntity;
			if ((localEntity.HasValue && !(val4 != localEntity.GetValueOrDefault())) || (((SharedContainerSystem)_container).TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(val3), ref val5) && (!_entity.TryGetComponent<SubFloorHideComponent>(val3, ref subFloorHideComponent) || !subFloorHideComponent.IsUnderCover || _entity.HasComponent<TrayRevealedComponent>(val5.Owner))))
			{
				DrawIcon(args, scale, rotate, val3, crawler, transform, sprite);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private void DrawIcon(OverlayDrawArgs args, Matrix3x2 scale, Matrix3x2 rotate, EntityUid ent, VentCrawlerComponent crawler, TransformComponent transform, SpriteComponent sprite)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		if (!(transform.MapID != args.MapId))
		{
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent, sprite)));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(transform, _xformQuery);
			Box2 val = ((Box2)(ref localBounds)).Translated(worldPosition);
			if (((Box2)(ref val)).Intersects(ref args.WorldAABB))
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
				Matrix3x2 value2 = Matrix3x2.Multiply(scale, value);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(rotate, value2);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				Rsi val2 = new Rsi(_rsiPath, crawler.VentCrawlIcon);
				Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val2, _timing.CurTime, true);
				Vector2 vector = ((Box2)(ref localBounds)).Center + sprite.Offset + new Vector2(-0.5f, -0.5f);
				((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
			}
		}
	}
}
