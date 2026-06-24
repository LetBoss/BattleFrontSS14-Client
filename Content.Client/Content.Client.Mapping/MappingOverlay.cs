using System.Collections.Generic;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Mapping;

public sealed class MappingOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");

	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototypes;

	private readonly SpriteSystem _sprite;

	private static readonly Color PickColor = new Color((byte)1, byte.MaxValue, (byte)0, byte.MaxValue);

	private static readonly Color DeleteColor = new Color(byte.MaxValue, (byte)1, (byte)0, byte.MaxValue);

	private readonly Dictionary<EntityUid, Color> _oldColors = new Dictionary<EntityUid, Color>();

	private readonly MappingState _state;

	private readonly ShaderInstance _shader;

	public override OverlaySpace Space => (OverlaySpace)4;

	public MappingOverlay(MappingState state)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<MappingOverlay>(this);
		_sprite = _entities.System<SpriteSystem>();
		_state = state;
		_shader = _prototypes.Index<ShaderPrototype>(UnshadedShader).Instance();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val5 = default(SpriteComponent);
		foreach (var (val3, val4) in _oldColors)
		{
			if (_entities.TryGetComponent<SpriteComponent>(val3, ref val5) && (val5.Color == DeleteColor || val5.Color == PickColor))
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((val3, val5)), val4);
			}
		}
		_oldColors.Clear();
		if (!((ISharedPlayerManager)_player).LocalEntity.HasValue)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		((DrawingHandleBase)worldHandle).UseShader(_shader);
		switch (_state.State)
		{
		case MappingState.CursorState.Pick:
		{
			EntityUid? hoveredEntity = _state.GetHoveredEntity();
			if (hoveredEntity.HasValue)
			{
				EntityUid valueOrDefault2 = hoveredEntity.GetValueOrDefault();
				SpriteComponent val7 = default(SpriteComponent);
				if (_entities.TryGetComponent<SpriteComponent>(valueOrDefault2, ref val7))
				{
					_oldColors[valueOrDefault2] = val7.Color;
					_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((valueOrDefault2, val7)), PickColor);
				}
			}
			break;
		}
		case MappingState.CursorState.Delete:
		{
			EntityUid? hoveredEntity = _state.GetHoveredEntity();
			if (hoveredEntity.HasValue)
			{
				EntityUid valueOrDefault = hoveredEntity.GetValueOrDefault();
				SpriteComponent val6 = default(SpriteComponent);
				if (_entities.TryGetComponent<SpriteComponent>(valueOrDefault, ref val6))
				{
					_oldColors[valueOrDefault] = val6.Color;
					_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((valueOrDefault, val6)), DeleteColor);
				}
			}
			break;
		}
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}
}
