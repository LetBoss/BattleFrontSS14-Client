using Content.Shared._PUBG.Party;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Party;

public sealed class PubgPartySpriteOverlay : Overlay
{
	private readonly IEntityManager _ent;

	private readonly IPlayerManager _player;

	private readonly PubgPartyClientSystem _party;

	private readonly SharedTransformSystem _xform;

	private readonly SpriteSystem _spriteSystem;

	public override OverlaySpace Space => (OverlaySpace)4;

	public float Opacity { get; set; } = 1f;

	public PubgPartySpriteOverlay(IEntityManager ent, IPlayerManager player, PubgPartyClientSystem party)
	{
		_ent = ent;
		_player = player;
		_party = party;
		_xform = ent.System<SharedTransformSystem>();
		_spriteSystem = ent.System<SpriteSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (Opacity <= 0f)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		NetEntity netEntity = _ent.GetNetEntity(localEntity.Value, (MetaDataComponent)null);
		IEye eye = args.Viewport.Eye;
		Angle val = ((eye != null) ? eye.Rotation : Angle.Zero);
		TransformComponent val2 = default(TransformComponent);
		SpriteComponent val3 = default(SpriteComponent);
		foreach (PubgPartyMemberState member in _party.Members)
		{
			if (member.Entity == netEntity)
			{
				continue;
			}
			EntityUid entity = _ent.GetEntity(member.Entity);
			if (_ent.TryGetComponent<TransformComponent>(entity, ref val2) && _ent.TryGetComponent<SpriteComponent>(entity, ref val3) && !(val2.MapID != args.MapId))
			{
				var (vector, val4) = _xform.GetWorldPositionRotation(val2);
				if (Opacity < 0.999f)
				{
					Color color = val3.Color;
					_spriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((entity, val3)), ((Color)(ref color)).WithAlpha(color.A * Opacity));
					_spriteSystem.RenderSprite(Entity<SpriteComponent>.op_Implicit((entity, val3)), ((OverlayDrawArgs)(ref args)).WorldHandle, val, val4, vector);
					_spriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((entity, val3)), color);
				}
				else
				{
					_spriteSystem.RenderSprite(Entity<SpriteComponent>.op_Implicit((entity, val3)), ((OverlayDrawArgs)(ref args)).WorldHandle, val, val4, vector);
				}
			}
		}
	}
}
