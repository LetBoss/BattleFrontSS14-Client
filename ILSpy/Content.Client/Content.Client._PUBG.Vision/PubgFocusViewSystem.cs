using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared._PUBG.Vision;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Camera;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Vision;

public sealed class PubgFocusViewSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private PubgWeaponModulesSystem _modules;

	[Dependency]
	private VehicleWeaponsSystem _vehicleWeapons;

	private EntityUid? _activeWeapon;

	private readonly Dictionary<EntityUid, Vector2> _currentOffsets = new Dictionary<EntityUid, Vector2>();

	private const float SmoothSpeed = 12f;

	private const float EdgeOffset = 0.9f;

	public override void Initialize()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0029: Expected O, but got Unknown
		CommandBinds.Builder.Bind(PubgKeyFunctions.PubgFocusView, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			SetActive(active: true);
		}, (StateInputCmdDelegate)delegate
		{
			SetActive(active: false);
		}, true, true)).Register<PubgFocusViewSystem>();
		((EntitySystem)this).SubscribeLocalEvent<PubgFocusViewComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>((EntityEventRefHandler<PubgFocusViewComponent, HeldRelayedEvent<GetEyeOffsetRelayedEvent>>)OnGetEyeOffset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, GetEyeOffsetEvent>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, GetEyeOffsetEvent>)OnOperatorGetEyeOffset, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<PubgFocusViewSystem>();
	}

	public MapCoordinates AdjustMapCoordinates(EntityUid weapon, MapCoordinates coordinates)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (coordinates.MapId == MapId.Nullspace)
		{
			return coordinates;
		}
		PubgFocusViewComponent pubgFocusViewComponent = default(PubgFocusViewComponent);
		if (((EntitySystem)this).TryComp<PubgFocusViewComponent>(weapon, ref pubgFocusViewComponent) && !pubgFocusViewComponent.AdjustShotCoordinates)
		{
			return coordinates;
		}
		if (!TryGetActiveOffset(weapon, out var offset) || offset == Vector2.Zero)
		{
			return coordinates;
		}
		return new MapCoordinates(coordinates.Position + offset, coordinates.MapId);
	}

	private void SetActive(bool active)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid? val = null;
		PubgFocusViewComponent focus = null;
		if (active)
		{
			if (!TryGetFocusWeapon(localEntity.Value, out EntityUid weapon, out focus))
			{
				return;
			}
			float maxOffset = focus.OffsetTiles + _modules.GetFocusBonusTiles(weapon);
			Vector2? vector = ComputeOffsetFromCursor(maxOffset, focus.FixedOffset);
			if (!vector.HasValue)
			{
				return;
			}
			val = weapon;
			_activeWeapon = weapon;
			_currentOffsets[weapon] = vector.Value;
		}
		else
		{
			if (!_activeWeapon.HasValue)
			{
				return;
			}
			if (!((EntitySystem)this).Exists(_activeWeapon))
			{
				_currentOffsets.Remove(_activeWeapon.Value);
				_activeWeapon = null;
				return;
			}
			val = _activeWeapon;
			if (!((EntitySystem)this).TryComp<PubgFocusViewComponent>(val.Value, ref focus))
			{
				return;
			}
		}
		if (focus.Active != active)
		{
			focus.Active = active;
			((EntitySystem)this).RaisePredictiveEvent<PubgFocusViewStateEvent>(new PubgFocusViewStateEvent(((EntitySystem)this).GetNetEntity(val.Value, (MetaDataComponent)null), active));
			if (!active)
			{
				_activeWeapon = null;
			}
			if (!active)
			{
				_currentOffsets.Remove(val.Value);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		if (!_activeWeapon.HasValue)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		if (TryGetFocusWeapon(localEntity.Value, out EntityUid weapon, out PubgFocusViewComponent focus))
		{
			EntityUid val = weapon;
			EntityUid? activeWeapon = _activeWeapon;
			if (activeWeapon.HasValue && !(val != activeWeapon.GetValueOrDefault()))
			{
				float maxOffset = focus.OffsetTiles + _modules.GetFocusBonusTiles(_activeWeapon.Value);
				Vector2? vector = ComputeOffsetFromCursor(maxOffset, focus.FixedOffset);
				if (vector.HasValue && _currentOffsets.TryGetValue(_activeWeapon.Value, out var value))
				{
					float amount = Math.Clamp(12f * frameTime, 0f, 1f);
					_currentOffsets[_activeWeapon.Value] = Vector2.Lerp(value, vector.Value, amount);
				}
				return;
			}
		}
		SetActive(active: false);
	}

	private void OnGetEyeOffset(Entity<PubgFocusViewComponent> ent, ref HeldRelayedEvent<GetEyeOffsetRelayedEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetActiveOffset(ent.Owner, out var offset))
		{
			args.Args.Offset += offset;
		}
	}

	private void OnOperatorGetEyeOffset(Entity<VehicleWeaponsOperatorComponent> ent, ref GetEyeOffsetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (vehicle.HasValue)
		{
			EntityUid valueOrDefault = vehicle.GetValueOrDefault();
			if (_vehicleWeapons.TryGetSelectedWeaponForOperator(valueOrDefault, ent.Owner, out var weapon) && TryGetActiveOffset(weapon, out var offset))
			{
				args.Offset += offset;
			}
		}
	}

	private bool TryGetFocusWeapon(EntityUid player, out EntityUid weapon, [NotNullWhen(true)] out PubgFocusViewComponent? focus)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		weapon = default(EntityUid);
		focus = null;
		VehicleWeaponsOperatorComponent vehicleWeaponsOperatorComponent = default(VehicleWeaponsOperatorComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(player, ref vehicleWeaponsOperatorComponent))
		{
			EntityUid? vehicle = vehicleWeaponsOperatorComponent.Vehicle;
			if (vehicle.HasValue)
			{
				EntityUid valueOrDefault = vehicle.GetValueOrDefault();
				if (_vehicleWeapons.TryGetSelectedWeaponForOperator(valueOrDefault, player, out var weapon2) && ((EntitySystem)this).TryComp<PubgFocusViewComponent>(weapon2, ref focus))
				{
					weapon = weapon2;
					return true;
				}
			}
		}
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((ValueTuple<EntityUid, HandsComponent>)(player, null)), out var item) && ((EntitySystem)this).TryComp<PubgFocusViewComponent>(item.Value, ref focus))
		{
			weapon = item.Value;
			return true;
		}
		return false;
	}

	private Vector2? ComputeOffsetFromCursor(float maxOffset, bool fixedOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (mouseScreenPosition.Window == WindowId.Invalid)
		{
			return null;
		}
		if (fixedOffset)
		{
			IEye currentEye = _eyeManager.CurrentEye;
			MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
			if (val.MapId == MapId.Nullspace || val.MapId != currentEye.Position.MapId)
			{
				return null;
			}
			Vector2 vector = val.Position - currentEye.Position.Position;
			if (vector.Length() < 0.5f)
			{
				return Vector2.Zero;
			}
			return Vector2Helpers.Normalized(vector) * maxOffset;
		}
		Vector2i size = _clyde.MainWindow.Size;
		float num = MathF.Min((float)size.X / 2f, (float)size.Y / 2f) * 0.9f;
		if (num <= 0f)
		{
			return null;
		}
		Vector2 value = new Vector2((0f - (((ScreenCoordinates)(ref mouseScreenPosition)).X - (float)size.X / 2f)) / num, (((ScreenCoordinates)(ref mouseScreenPosition)).Y - (float)size.Y / 2f) / num);
		Angle rotation = _eyeManager.CurrentEye.Rotation;
		Vector2 vector2 = Vector2.Transform(value, Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float)((Angle)(ref rotation)).Opposite().Theta));
		vector2 *= maxOffset;
		if (vector2.Length() > maxOffset)
		{
			vector2 = Vector2Helpers.Normalized(vector2) * maxOffset;
		}
		return vector2;
	}

	public bool TryGetActiveOffset(EntityUid weapon, out Vector2 offset)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		offset = Vector2.Zero;
		PubgFocusViewComponent pubgFocusViewComponent = default(PubgFocusViewComponent);
		if (!((EntitySystem)this).TryComp<PubgFocusViewComponent>(weapon, ref pubgFocusViewComponent) || !pubgFocusViewComponent.Active)
		{
			return false;
		}
		if (!_currentOffsets.TryGetValue(weapon, out var value) || value == Vector2.Zero)
		{
			return false;
		}
		offset = value;
		return true;
	}
}
