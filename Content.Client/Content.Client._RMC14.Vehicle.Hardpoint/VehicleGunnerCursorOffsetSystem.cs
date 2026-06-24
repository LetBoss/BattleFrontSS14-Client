using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Movement.Systems;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Camera;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Vehicle.Hardpoint;

public sealed class VehicleGunnerCursorOffsetSystem : EntitySystem
{
	[Dependency]
	private readonly ContentEyeSystem _contentEye;

	[Dependency]
	private readonly IEyeManager _eyeManager;

	[Dependency]
	private readonly IInputManager _inputManager;

	[Dependency]
	private readonly IClyde _clyde;

	[Dependency]
	private readonly IPlayerManager _player;

	private readonly Dictionary<EntityUid, Vector2> _currentPositions = new Dictionary<EntityUid, Vector2>();

	private static readonly float EdgeOffset = 0.9f;

	public override void Initialize()
	{
		((EntitySystem)this).UpdatesBefore.Add(typeof(VehicleTurretInputSystem));
		((EntitySystem)this).UpdatesBefore.Add(typeof(GunSystem));
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerViewUserComponent, GetEyeOffsetEvent>((EntityEventRefHandler<VehicleGunnerViewUserComponent, GetEyeOffsetEvent>)OnGetEyeOffset, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			VehicleGunnerViewUserComponent vehicleGunnerViewUserComponent = default(VehicleGunnerViewUserComponent);
			EyeComponent item = default(EyeComponent);
			if (((EntitySystem)this).TryComp<VehicleGunnerViewUserComponent>(valueOrDefault, ref vehicleGunnerViewUserComponent) && !(vehicleGunnerViewUserComponent.CursorMaxOffset <= 0f) && ((EntitySystem)this).TryComp<EyeComponent>(valueOrDefault, ref item))
			{
				_contentEye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((valueOrDefault, item)));
			}
		}
	}

	private void OnGetEyeOffset(Entity<VehicleGunnerViewUserComponent> ent, ref GetEyeOffsetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CursorMaxOffset <= 0f)
		{
			_currentPositions.Remove(ent.Owner);
			return;
		}
		Vector2? vector = OffsetAfterMouse(ent.Owner, ent.Comp);
		if (vector.HasValue)
		{
			args.Offset += vector.Value;
		}
	}

	private Vector2? OffsetAfterMouse(EntityUid uid, VehicleGunnerViewUserComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (mouseScreenPosition.Window == WindowId.Invalid)
		{
			return _currentPositions.GetValueOrDefault(uid, Vector2.Zero);
		}
		Vector2i size = _clyde.MainWindow.Size;
		float num = MathF.Min((float)size.X / 2f, (float)size.Y / 2f) * EdgeOffset;
		if (num <= 0f)
		{
			return _currentPositions.GetValueOrDefault(uid, Vector2.Zero);
		}
		Vector2 value = new Vector2((0f - (((ScreenCoordinates)(ref mouseScreenPosition)).X - (float)size.X / 2f)) / num, (((ScreenCoordinates)(ref mouseScreenPosition)).Y - (float)size.Y / 2f) / num);
		Angle rotation = _eyeManager.CurrentEye.Rotation;
		Vector2 vector = Vector2.Transform(value, Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float)((Angle)(ref rotation)).Opposite().Theta));
		vector *= component.CursorMaxOffset;
		if (vector.Length() > component.CursorMaxOffset)
		{
			vector = Vector2Helpers.Normalized(vector) * component.CursorMaxOffset;
		}
		Vector2 valueOrDefault = _currentPositions.GetValueOrDefault(uid, Vector2.Zero);
		if (valueOrDefault != vector)
		{
			Vector2 vector2 = vector - valueOrDefault;
			if (vector2.Length() > component.CursorOffsetSpeed)
			{
				vector2 = Vector2Helpers.Normalized(vector2) * component.CursorOffsetSpeed;
			}
			valueOrDefault += vector2;
			_currentPositions[uid] = valueOrDefault;
		}
		return valueOrDefault;
	}
}
