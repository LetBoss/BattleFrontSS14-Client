using System.Numerics;
using Content.Shared.Audio;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Audio;

public sealed class AmbientSoundOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly AmbientSoundSystem _ambient;

	private readonly EntityLookupSystem _lookup;

	public override OverlaySpace Space => (OverlaySpace)4;

	public AmbientSoundOverlay(IEntityManager entManager, AmbientSoundSystem ambient, EntityLookupSystem lookup)
	{
		_entManager = entManager;
		_ambient = ambient;
		_lookup = lookup;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQuery<AmbientSoundComponent> entityQuery = _entManager.GetEntityQuery<AmbientSoundComponent>();
		EntityQuery<TransformComponent> entityQuery2 = _entManager.GetEntityQuery<TransformComponent>();
		SharedTransformSystem val = _entManager.System<SharedTransformSystem>();
		AmbientSoundComponent ambientSoundComponent = default(AmbientSoundComponent);
		TransformComponent val2 = default(TransformComponent);
		foreach (EntityUid item in _lookup.GetEntitiesIntersecting(args.MapId, args.WorldBounds, (LookupFlags)110))
		{
			if (!entityQuery.TryGetComponent(item, ref ambientSoundComponent) || !entityQuery2.TryGetComponent(item, ref val2))
			{
				continue;
			}
			Color val3;
			if (ambientSoundComponent.Enabled)
			{
				if (_ambient.IsActive(Entity<AmbientSoundComponent>.op_Implicit((item, ambientSoundComponent))))
				{
					Vector2 worldPosition = val.GetWorldPosition(val2);
					val3 = Color.LightGreen;
					((DrawingHandleBase)worldHandle).DrawCircle(worldPosition, 0.25f, ((Color)(ref val3)).WithAlpha(0.5f), true);
				}
				else
				{
					Vector2 worldPosition2 = val.GetWorldPosition(val2);
					val3 = Color.Orange;
					((DrawingHandleBase)worldHandle).DrawCircle(worldPosition2, 0.25f, ((Color)(ref val3)).WithAlpha(0.25f), true);
				}
			}
			else
			{
				Vector2 worldPosition3 = val.GetWorldPosition(val2);
				val3 = Color.Red;
				((DrawingHandleBase)worldHandle).DrawCircle(worldPosition3, 0.25f, ((Color)(ref val3)).WithAlpha(0.25f), true);
			}
		}
	}
}
