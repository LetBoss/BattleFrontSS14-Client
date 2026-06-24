using System;
using System.Numerics;
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

public sealed class PubgPartyWorldOverlay : Overlay
{
	private readonly IEntityManager _entityManager;

	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly SpriteSystem _spriteSystem;

	private readonly PubgPartyClientSystem _party;

	public override OverlaySpace Space => (OverlaySpace)4;

	public PubgPartyWorldOverlay(IEntityManager entityManager, IPlayerManager player, SharedTransformSystem transform, PubgPartyClientSystem party)
	{
		_entityManager = entityManager;
		_player = player;
		_transform = transform;
		_spriteSystem = entityManager.System<SpriteSystem>();
		_party = party;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		NetEntity netEntity = _entityManager.GetNetEntity(localEntity.Value, (MetaDataComponent)null);
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		TransformComponent val = default(TransformComponent);
		SpriteComponent item = default(SpriteComponent);
		foreach (PubgPartyMemberState member in _party.Members)
		{
			if (!(member.Entity == netEntity))
			{
				EntityUid entity = _entityManager.GetEntity(member.Entity);
				if (_entityManager.TryGetComponent<TransformComponent>(entity, ref val) && _entityManager.TryGetComponent<SpriteComponent>(entity, ref item) && !(val.MapID != args.MapId))
				{
					ValueTuple<Vector2, Angle> worldPositionRotation = _transform.GetWorldPositionRotation(val);
					Vector2 item2 = worldPositionRotation.Item1;
					Angle item3 = worldPositionRotation.Item2;
					SpriteSystem spriteSystem = _spriteSystem;
					Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((entity, item));
					IEye eye = args.Viewport.Eye;
					Box2Rotated val3 = spriteSystem.CalculateBounds(val2, item2, item3, (Angle)((eye != null) ? eye.Rotation : default(Angle)));
					Color partyColor = GetPartyColor(member.SlotIndex);
					worldHandle.DrawRect(ref val3, ((Color)(ref partyColor)).WithAlpha(0.8f), false);
				}
			}
		}
	}

	private static Color GetPartyColor(int slotIndex)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(slotIndex switch
		{
			1 => Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null), 
			2 => Color.FromHex((ReadOnlySpan<char>)"#ffeb3b", (Color?)null), 
			3 => Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null), 
		});
	}
}
