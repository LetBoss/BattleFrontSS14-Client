using System;
using System.Numerics;
using Content.Client.Movement.Components;
using Content.Shared._RMC14.Scoping;
using Content.Shared.Camera;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client.Movement.Systems;

public sealed class EyeCursorOffsetSystem : EntitySystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IClyde _clyde;

	private static float _edgeOffset = 0.9f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EyeCursorOffsetComponent, GetEyeOffsetEvent>((ComponentEventRefHandler<EyeCursorOffsetComponent, GetEyeOffsetEvent>)OnGetEyeOffsetEvent, (Type[])null, (Type[])null);
	}

	private void OnGetEyeOffsetEvent(EntityUid uid, EyeCursorOffsetComponent component, ref GetEyeOffsetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ScopingComponent>(uid))
		{
			Vector2? vector = OffsetAfterMouse(uid, component);
			if (vector.HasValue)
			{
				args.Offset += vector.Value;
			}
		}
	}

	public Vector2? OffsetAfterMouse(EntityUid uid, EyeCursorOffsetComponent? component)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		Vector2i size = _clyde.MainWindow.Size;
		float num = MathF.Min(size.X / 2, size.Y / 2) * _edgeOffset;
		Vector2 value = new Vector2((0f - (((ScreenCoordinates)(ref mouseScreenPosition)).X - (float)(size.X / 2))) / num, (((ScreenCoordinates)(ref mouseScreenPosition)).Y - (float)(size.Y / 2)) / num);
		if (!localEntity.HasValue)
		{
			return null;
		}
		_transform.GetWorldPosition(localEntity.Value);
		if (component == null)
		{
			component = ((EntitySystem)this).EnsureComp<EyeCursorOffsetComponent>(uid);
		}
		if (mouseScreenPosition.Window != WindowId.Invalid)
		{
			Angle rotation = _eyeManager.CurrentEye.Rotation;
			Vector2 vector = Vector2.Transform(value, Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float)((Angle)(ref rotation)).Opposite().Theta));
			vector *= component.MaxOffset;
			if (vector.Length() > component.MaxOffset)
			{
				vector = Vector2Helpers.Normalized(vector) * component.MaxOffset;
			}
			component.TargetPosition = vector;
			if (component.CurrentPosition != component.TargetPosition)
			{
				Vector2 vector2 = component.TargetPosition - component.CurrentPosition;
				if (vector2.Length() > component.OffsetSpeed)
				{
					vector2 = Vector2Helpers.Normalized(vector2) * component.OffsetSpeed;
				}
				component.CurrentPosition += vector2;
			}
		}
		return component.CurrentPosition;
	}
}
