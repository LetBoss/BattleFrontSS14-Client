using System.Numerics;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Ranged;

public sealed class GunSpreadOverlay : Overlay
{
	private IEntityManager _entManager;

	private readonly IEyeManager _eye;

	private readonly IGameTiming _timing;

	private readonly IInputManager _input;

	private readonly IPlayerManager _player;

	private readonly GunSystem _guns;

	private readonly SharedTransformSystem _transform;

	public override OverlaySpace Space => (OverlaySpace)4;

	public GunSpreadOverlay(IEntityManager entManager, IEyeManager eyeManager, IGameTiming timing, IInputManager input, IPlayerManager player, GunSystem system, SharedTransformSystem transform)
	{
		_entManager = entManager;
		_eye = eyeManager;
		_input = input;
		_timing = timing;
		_player = player;
		_guns = system;
		_transform = transform;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		TransformComponent val = default(TransformComponent);
		if (!localEntity.HasValue || !_entManager.TryGetComponent<TransformComponent>(localEntity, ref val))
		{
			return;
		}
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(localEntity.Value, val);
		if (!(mapCoordinates.MapId == MapId.Nullspace) && _guns.TryGetGun(localEntity.Value, out EntityUid _, out GunComponent gunComp))
		{
			ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
			MapCoordinates val2 = _eye.PixelToMap(mouseScreenPosition);
			if (!(mapCoordinates.MapId != val2.MapId))
			{
				Angle maxAngleModified = gunComp.MaxAngleModified;
				Angle minAngleModified = gunComp.MinAngleModified;
				double totalSeconds = (_timing.CurTime - gunComp.NextFire).TotalSeconds;
				Angle val3 = default(Angle);
				((Angle)(ref val3))._002Ector(MathHelper.Clamp(gunComp.CurrentAngle.Theta - gunComp.AngleDecayModified.Theta * totalSeconds, gunComp.MinAngleModified.Theta, gunComp.MaxAngleModified.Theta));
				Vector2 vector = val2.Position - mapCoordinates.Position;
				((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, val2.Position + vector, Color.Orange);
				((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, val2.Position + ((Angle)(ref maxAngleModified)).RotateVec(ref vector), Color.Red);
				Vector2 position = mapCoordinates.Position;
				Vector2 position2 = val2.Position;
				Angle val4 = -maxAngleModified;
				((DrawingHandleBase)worldHandle).DrawLine(position, position2 + ((Angle)(ref val4)).RotateVec(ref vector), Color.Red);
				((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, val2.Position + ((Angle)(ref minAngleModified)).RotateVec(ref vector), Color.Green);
				Vector2 position3 = mapCoordinates.Position;
				Vector2 position4 = val2.Position;
				val4 = -minAngleModified;
				((DrawingHandleBase)worldHandle).DrawLine(position3, position4 + ((Angle)(ref val4)).RotateVec(ref vector), Color.Green);
				((DrawingHandleBase)worldHandle).DrawLine(mapCoordinates.Position, val2.Position + ((Angle)(ref val3)).RotateVec(ref vector), Color.Yellow);
				Vector2 position5 = mapCoordinates.Position;
				Vector2 position6 = val2.Position;
				val4 = -val3;
				((DrawingHandleBase)worldHandle).DrawLine(position5, position6 + ((Angle)(ref val4)).RotateVec(ref vector), Color.Yellow);
			}
		}
	}
}
