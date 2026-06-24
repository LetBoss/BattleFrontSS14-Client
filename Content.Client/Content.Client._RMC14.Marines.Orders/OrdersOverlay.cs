using System.Numerics;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Orders;
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

namespace Content.Client._RMC14.Marines.Orders;

public sealed class OrdersOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IPlayerManager _players;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private readonly SpriteSystem _sprite;

	private readonly TransformSystem _transform;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)8;

	public OrdersOverlay()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<OrdersOverlay>(this);
		_sprite = _entity.System<SpriteSystem>();
		_transform = _entity.System<TransformSystem>();
		_shader = _prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("unshaded")).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		if (_entity.HasComponent<MarineComponent>(((ISharedPlayerManager)_players).LocalEntity))
		{
			DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
			IEye eye = args.Viewport.Eye;
			_003F val = ((eye != null) ? eye.Rotation : default(Angle));
			Matrix3x2 scaleMatrix = Matrix3x2.CreateScale(new Vector2(1f, 1f));
			Matrix3x2 rotationMatrix = Matrix3Helpers.CreateRotation(Angle.op_Implicit(-(Angle)val));
			((DrawingHandleBase)worldHandle).UseShader(_shader);
			AllEntityQueryEnumerator<MoveOrderComponent, SpriteComponent, TransformComponent> val2 = _entity.AllEntityQueryEnumerator<MoveOrderComponent, SpriteComponent, TransformComponent>();
			EntityUid item = default(EntityUid);
			MoveOrderComponent moveOrderComponent = default(MoveOrderComponent);
			SpriteComponent item2 = default(SpriteComponent);
			TransformComponent item3 = default(TransformComponent);
			while (val2.MoveNext(ref item, ref moveOrderComponent, ref item2, ref item3))
			{
				DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item, item2, item3)), in args, moveOrderComponent.Icon, scaleMatrix, rotationMatrix);
			}
			AllEntityQueryEnumerator<HoldOrderComponent, SpriteComponent, TransformComponent> val3 = _entity.AllEntityQueryEnumerator<HoldOrderComponent, SpriteComponent, TransformComponent>();
			EntityUid item4 = default(EntityUid);
			HoldOrderComponent holdOrderComponent = default(HoldOrderComponent);
			SpriteComponent item5 = default(SpriteComponent);
			TransformComponent item6 = default(TransformComponent);
			while (val3.MoveNext(ref item4, ref holdOrderComponent, ref item5, ref item6))
			{
				DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item4, item5, item6)), in args, holdOrderComponent.Icon, scaleMatrix, rotationMatrix);
			}
			AllEntityQueryEnumerator<FocusOrderComponent, SpriteComponent, TransformComponent> val4 = _entity.AllEntityQueryEnumerator<FocusOrderComponent, SpriteComponent, TransformComponent>();
			EntityUid item7 = default(EntityUid);
			FocusOrderComponent focusOrderComponent = default(FocusOrderComponent);
			SpriteComponent item8 = default(SpriteComponent);
			TransformComponent item9 = default(TransformComponent);
			while (val4.MoveNext(ref item7, ref focusOrderComponent, ref item8, ref item9))
			{
				DrawIcon(Entity<SpriteComponent, TransformComponent>.op_Implicit((item7, item8, item9)), in args, focusOrderComponent.Icon, scaleMatrix, rotationMatrix);
			}
			Matrix3x2 identity = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle).SetTransform(ref identity);
			((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
		}
	}

	private void DrawIcon(Entity<SpriteComponent, TransformComponent> ent, in OverlayDrawArgs args, SpriteSpecifier icon, Matrix3x2 scaleMatrix, Matrix3x2 rotationMatrix)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Entity<SpriteComponent, TransformComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		val.Deconstruct(ref val2, ref val3, ref val4);
		SpriteComponent val5 = val3;
		TransformComponent val6 = val4;
		if (!(val6.MapID != args.MapId))
		{
			Box2 localBounds = _sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((ent.Owner, val5)));
			Vector2 worldPosition = ((SharedTransformSystem)_transform).GetWorldPosition(val6);
			Box2 val7 = ((Box2)(ref localBounds)).Translated(worldPosition);
			if (((Box2)(ref val7)).Intersects(ref args.WorldAABB))
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Matrix3x2 value = Matrix3x2.CreateTranslation(worldPosition);
				Matrix3x2 value2 = Matrix3x2.Multiply(scaleMatrix, value);
				Matrix3x2 matrix3x = Matrix3x2.Multiply(rotationMatrix, value2);
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				Texture frame = _sprite.GetFrame(icon, _timing.CurTime, true);
				float y = (((Box2)(ref localBounds)).Height + val5.Offset.Y) / 2f - (float)frame.Height / 32f * ((Box2)(ref localBounds)).Height;
				float x = (((Box2)(ref localBounds)).Width + val5.Offset.X) / 2f - (float)frame.Width / 32f - 0.25f;
				Vector2 vector = new Vector2(x, y);
				((DrawingHandleBase)worldHandle).DrawTexture(frame, vector, (Color?)null);
			}
		}
	}
}
