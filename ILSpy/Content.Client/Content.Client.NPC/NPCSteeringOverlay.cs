using System.Numerics;
using Content.Client.PhysicsSystem.Controllers;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.NPC;

public sealed class NPCSteeringOverlay : Overlay
{
	private readonly IEntityManager _entManager;

	private readonly SharedTransformSystem _transformSystem;

	public override OverlaySpace Space => (OverlaySpace)4;

	public NPCSteeringOverlay(IEntityManager entManager)
	{
		_entManager = entManager;
		_transformSystem = _entManager.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (nPCSteeringComponent, mover, val) in _entManager.EntityQuery<NPCSteeringComponent, InputMoverComponent, TransformComponent>(true))
		{
			if (val.MapID != args.MapId)
			{
				continue;
			}
			Vector2 item = _transformSystem.GetWorldPositionRotation(val).Item1;
			if (!((Box2)(ref args.WorldAABB)).Contains(item, true))
			{
				continue;
			}
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawCircle(item, 1f, Color.Green, false);
			Angle parentGridAngle = _entManager.System<MoverController>().GetParentGridAngle(mover);
			foreach (Vector2 dangerPoint in nPCSteeringComponent.DangerPoints)
			{
				DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Color red = Color.Red;
				((DrawingHandleBase)worldHandle).DrawCircle(dangerPoint, 0.1f, ((Color)(ref red)).WithAlpha(0.6f), true);
			}
			for (int i = 0; i < 12; i++)
			{
				float x = nPCSteeringComponent.DangerMap[i];
				float x2 = nPCSteeringComponent.InterestMap[i];
				Angle val2 = Angle.FromDegrees((double)(i * 30));
				DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs)(ref args)).WorldHandle;
				Angle val3 = parentGridAngle + val2;
				Vector2 vector = new Vector2(x2, 0f);
				((DrawingHandleBase)worldHandle2).DrawLine(item, item + ((Angle)(ref val3)).RotateVec(ref vector), Color.LimeGreen);
				DrawingHandleWorld worldHandle3 = ((OverlayDrawArgs)(ref args)).WorldHandle;
				val3 = parentGridAngle + val2;
				vector = new Vector2(x, 0f);
				((DrawingHandleBase)worldHandle3).DrawLine(item, item + ((Angle)(ref val3)).RotateVec(ref vector), Color.Red);
			}
			((DrawingHandleBase)((OverlayDrawArgs)(ref args)).WorldHandle).DrawLine(item, item + ((Angle)(ref parentGridAngle)).RotateVec(ref nPCSteeringComponent.Direction), Color.Cyan);
		}
	}
}
