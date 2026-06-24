using System.Numerics;
using Content.Client._RMC14.NightVision;
using Content.Shared._RMC14.Mobs;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Xenonids.HiveLeader;

public sealed class HiveLeaderOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space
	{
		get
		{
			if (_overlay.HasOverlay<NightVisionOverlay>())
			{
				return (OverlaySpace)4;
			}
			return (OverlaySpace)8;
		}
	}

	public HiveLeaderOverlay()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<HiveLeaderOverlay>(this);
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_xformQuery = _entity.GetEntityQuery<TransformComponent>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
		((Overlay)this).ZIndex = 1;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (!_entity.HasComponent<XenoComponent>(((ISharedPlayerManager)_players).LocalEntity) && !_entity.HasComponent<CMGhostXenoHudComponent>(((ISharedPlayerManager)_players).LocalEntity))
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		IEye eye = args.Viewport.Eye;
		_003F val = ((eye != null) ? eye.Rotation : default(Angle));
		Matrix3x2 value = Matrix3x2.CreateScale(new Vector2(1f, 1f));
		Matrix3x2 value2 = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
		Rsi val2 = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_leader.rsi"), "hudxenoleader");
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		EntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent> val3 = _entity.EntityQueryEnumerator<HiveLeaderComponent, SpriteComponent, TransformComponent>();
		EntityUid item = default(EntityUid);
		HiveLeaderComponent hiveLeaderComponent = default(HiveLeaderComponent);
		SpriteComponent val4 = default(SpriteComponent);
		TransformComponent val5 = default(TransformComponent);
		while (val3.MoveNext(ref item, ref hiveLeaderComponent, ref val4, ref val5))
		{
			if (!(val5.MapID != args.MapId))
			{
				Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((item, val4)));
				Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val5, _xformQuery);
				Box2 val6 = ((Box2)(ref localBounds)).Translated(worldPosition);
				if (((Box2)(ref val6)).Intersects(ref args.WorldAABB))
				{
					Matrix3x2 value3 = Matrix3x2.CreateTranslation(worldPosition);
					Matrix3x2 value4 = Matrix3x2.Multiply(value, value3);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(value2, value4);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					Texture frame = _sprite.GetFrame((SpriteSpecifier)(object)val2, _timing.CurTime, true);
					float y = (((Box2)(ref localBounds)).Height + val4.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height + 0.15f;
					float x = (((Box2)(ref localBounds)).Width + val4.Offset.X) / 2f - (float)frame.Width / 32f - 0.6f;
					Vector2 vector = new Vector2(x, y);
					((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
				}
			}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
