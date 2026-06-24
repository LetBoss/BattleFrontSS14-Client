using System;
using System.Numerics;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Melee;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client.Weapons.Melee;

public sealed class MeleeArcOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly IEyeManager _eyeManager;

	private readonly IInputManager _inputManager;

	private readonly IPlayerManager _playerManager;

	private readonly MeleeWeaponSystem _melee;

	private readonly SharedCombatModeSystem _combatMode;

	private readonly SharedTransformSystem _transform;

	public override OverlaySpace Space => (OverlaySpace)8;

	public MeleeArcOverlay(IEntityManager entManager, IEyeManager eyeManager, IInputManager inputManager, IPlayerManager playerManager, MeleeWeaponSystem melee, SharedCombatModeSystem combatMode, SharedTransformSystem transform)
	{
		_entManager = entManager;
		_eyeManager = eyeManager;
		_inputManager = inputManager;
		_playerManager = playerManager;
		_melee = melee;
		_combatMode = combatMode;
		_transform = transform;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		TransformComponent val = default(TransformComponent);
		if (!_entManager.TryGetComponent<TransformComponent>(localEntity, ref val) || !_combatMode.IsInCombatMode(localEntity) || !_melee.TryGetWeapon(localEntity.Value, out EntityUid _, out MeleeWeaponComponent melee))
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		MapCoordinates val2 = _eyeManager.PixelToMap(mouseScreenPosition);
		if (val2.MapId != args.MapId)
		{
			return;
		}
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(localEntity.Value, val);
		if (val2.MapId != mapCoordinates.MapId)
		{
			return;
		}
		Vector2 vector = val2.Position - mapCoordinates.Position;
		if (!vector.Equals(Vector2.Zero))
		{
			vector = Vector2Helpers.Normalized(vector) * Math.Min(melee.Range, vector.Length());
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawLine(mapCoordinates.Position, mapCoordinates.Position + vector, Color.Aqua);
			if (melee.Angle.Theta != 0.0)
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Vector2 position = mapCoordinates.Position;
				Vector2 position2 = mapCoordinates.Position;
				Angle val3 = new Angle(Angle.op_Implicit(-melee.Angle) / 2.0);
				((DrawingHandleBase)worldHandle).DrawLine(position, position2 + ((Angle)(ref val3)).RotateVec(ref vector), Color.Orange);
				DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Vector2 position3 = mapCoordinates.Position;
				Vector2 position4 = mapCoordinates.Position;
				val3 = new Angle(Angle.op_Implicit(melee.Angle) / 2.0);
				((DrawingHandleBase)worldHandle2).DrawLine(position3, position4 + ((Angle)(ref val3)).RotateVec(ref vector), Color.Orange);
			}
		}
	}
}
