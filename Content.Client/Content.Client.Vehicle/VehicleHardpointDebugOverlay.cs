using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._RMC14.Vehicle;
using Content.Client.Resources;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Vehicle;

public sealed class VehicleHardpointDebugOverlay : Overlay
{
	private readonly record struct DebugLine(string Text, Color Color);

	private readonly record struct DebugLabel(Vector2 WorldPosition, List<DebugLine> Lines);

	private const float PixelsPerMeter = 32f;

	private static readonly Vector2 GunLabelWorldOffset = new Vector2(0.18f, -0.18f);

	private static readonly Vector2 TurretLabelWorldOffset = new Vector2(-0.55f, -0.18f);

	private const float LabelLineHeight = 13f;

	private const float LabelPadding = 4f;

	private const float LabelCharWidth = 7f;

	private readonly IEntityManager _ents;

	private readonly IEyeManager _eye;

	private readonly SharedTransformSystem _transform;

	private readonly SharedContainerSystem _container;

	private readonly VehicleTurretMuzzleOffsetSystem _vehicleTurretMuzzle;

	private readonly VehicleTurretVisualSystem _vehicleTurretVisual;

	private readonly Font _font;

	private readonly EntityQuery<GunComponent> _gunQ;

	private readonly EntityQuery<GunFireArcComponent> _fireArcQ;

	private readonly EntityQuery<GridVehicleMoverComponent> _moverQ;

	private readonly EntityQuery<GunMuzzleOffsetComponent> _muzzleQ;

	private readonly EntityQuery<VehicleTurretComponent> _turretQ;

	private readonly EntityQuery<VehicleTurretMuzzleComponent> _turretMuzzleQ;

	private readonly EntityQuery<VehiclePortGunComponent> _portGunQ;

	private readonly List<DebugLabel> _labels = new List<DebugLabel>();

	public override OverlaySpace Space => (OverlaySpace)10;

	public bool Enabled { get; set; }

	public VehicleHardpointDebugOverlay(IEntityManager ents)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		_ents = ents;
		_eye = IoCManager.Resolve<IEyeManager>();
		IResourceCache cache = IoCManager.Resolve<IResourceCache>();
		_transform = ents.System<SharedTransformSystem>();
		_container = ents.System<SharedContainerSystem>();
		_vehicleTurretMuzzle = ents.System<VehicleTurretMuzzleOffsetSystem>();
		_vehicleTurretVisual = ents.System<VehicleTurretVisualSystem>();
		_font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
		_gunQ = ents.GetEntityQuery<GunComponent>();
		_fireArcQ = ents.GetEntityQuery<GunFireArcComponent>();
		_moverQ = ents.GetEntityQuery<GridVehicleMoverComponent>();
		_muzzleQ = ents.GetEntityQuery<GunMuzzleOffsetComponent>();
		_turretQ = ents.GetEntityQuery<VehicleTurretComponent>();
		_turretMuzzleQ = ents.GetEntityQuery<VehicleTurretMuzzleComponent>();
		_portGunQ = ents.GetEntityQuery<VehiclePortGunComponent>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		if (!Enabled)
		{
			return;
		}
		if ((int)args.Space == 2)
		{
			DrawLabels(in args);
			return;
		}
		_labels.Clear();
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQueryEnumerator<GunMuzzleOffsetComponent> val = _ents.EntityQueryEnumerator<GunMuzzleOffsetComponent>();
		EntityUid val2 = default(EntityUid);
		GunMuzzleOffsetComponent muzzle = default(GunMuzzleOffsetComponent);
		while (val.MoveNext(ref val2, ref muzzle))
		{
			if ((_turretQ.HasComp(val2) || _portGunQ.HasComp(val2)) && TryGetMuzzlePositions(val2, muzzle, args.MapId, out var origin, out var muzzlePos))
			{
				((DrawingHandleBase)worldHandle).DrawLine(origin, muzzlePos, new Color(0.95f, 0.95f, 0.95f, 0.7f));
				((DrawingHandleBase)worldHandle).DrawCircle(origin, 0.07f, new Color(0.2f, 0.9f, 1f, 0.9f), true);
				((DrawingHandleBase)worldHandle).DrawCircle(muzzlePos, 0.1f, new Color(1f, 0.75f, 0.2f, 0.95f), true);
				DrawShootArc(val2, origin, args.MapId, worldHandle);
				DrawShootTarget(val2, origin, args.MapId, worldHandle);
				AddGunDebugLabel(val2, muzzle, origin, muzzlePos);
			}
		}
		EntityQueryEnumerator<VehicleTurretMuzzleComponent> val3 = _ents.EntityQueryEnumerator<VehicleTurretMuzzleComponent>();
		EntityUid val4 = default(EntityUid);
		VehicleTurretMuzzleComponent turretMuzzle = default(VehicleTurretMuzzleComponent);
		while (val3.MoveNext(ref val4, ref turretMuzzle))
		{
			if (_turretQ.HasComp(val4) && TryGetTurretMuzzlePositions(val4, turretMuzzle, args.MapId, out var turretBasePos, out var basePos, out var leftPos, out var rightPos, out var leftRadius, out var rightRadius, out var useRightNext))
			{
				if (leftRadius > 0f)
				{
					((DrawingHandleBase)worldHandle).DrawCircle(turretBasePos, leftRadius, new Color(0.25f, 0.85f, 1f, 0.5f), true);
				}
				if (rightRadius > 0f && MathF.Abs(rightRadius - leftRadius) > 0.01f)
				{
					((DrawingHandleBase)worldHandle).DrawCircle(turretBasePos, rightRadius, new Color(1f, 0.6f, 0.2f, 0.5f), true);
				}
				Color val5 = (useRightNext ? new Color(0.4f, 0.4f, 0.4f, 0.7f) : new Color(0.2f, 0.95f, 0.4f, 0.95f));
				Color val6 = (useRightNext ? new Color(0.2f, 0.95f, 0.4f, 0.95f) : new Color(0.4f, 0.4f, 0.4f, 0.7f));
				if (leftRadius > 0f)
				{
					((DrawingHandleBase)worldHandle).DrawCircle(leftPos, 0.08f, val5, true);
				}
				if (rightRadius > 0f)
				{
					((DrawingHandleBase)worldHandle).DrawCircle(rightPos, 0.08f, val6, true);
				}
				((DrawingHandleBase)worldHandle).DrawLine(turretBasePos, basePos, new Color(0.4f, 0.9f, 1f, 0.5f));
			}
		}
		EntityQueryEnumerator<VehicleTurretComponent> val7 = _ents.EntityQueryEnumerator<VehicleTurretComponent>();
		EntityUid val8 = default(EntityUid);
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		while (val7.MoveNext(ref val8, ref turret))
		{
			if (TryGetTurretOverlayPositions(val8, turret, args.MapId, out var basePos2, out var anchorPos, out var turretPos))
			{
				((DrawingHandleBase)worldHandle).DrawCircle(basePos2, 0.06f, new Color(0.7f, 0.7f, 0.7f, 0.8f), true);
				((DrawingHandleBase)worldHandle).DrawLine(basePos2, anchorPos, new Color(0.2f, 0.8f, 0.95f, 0.7f));
				((DrawingHandleBase)worldHandle).DrawCircle(anchorPos, 0.07f, new Color(0.2f, 0.8f, 0.95f, 0.9f), true);
				if (anchorPos != turretPos)
				{
					((DrawingHandleBase)worldHandle).DrawLine(anchorPos, turretPos, new Color(1f, 0.7f, 0.2f, 0.8f));
					((DrawingHandleBase)worldHandle).DrawCircle(turretPos, 0.08f, new Color(1f, 0.7f, 0.2f, 0.95f), true);
				}
				if (!_muzzleQ.HasComp(val8))
				{
					AddTurretDebugLabel(val8, turret, basePos2, anchorPos, turretPos);
				}
			}
		}
	}

	private void DrawLabels(in OverlayDrawArgs args)
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (args.ViewportControl == null || _labels.Count == 0)
		{
			return;
		}
		List<(DebugLabel, Vector2, float, float)> list = new List<(DebugLabel, Vector2, float, float)>(_labels.Count);
		foreach (DebugLabel label in _labels)
		{
			Vector2 item = args.ViewportControl.WorldToScreen(label.WorldPosition);
			float approxLabelWidth = GetApproxLabelWidth(label.Lines);
			float item2 = (float)label.Lines.Count * 13f;
			list.Add((label, item, approxLabelWidth, item2));
		}
		list.Sort(delegate((DebugLabel Label, Vector2 Position, float Width, float Height) a, (DebugLabel Label, Vector2 Position, float Width, float Height) b)
		{
			int num2 = a.Position.Y.CompareTo(b.Position.Y);
			return (num2 == 0) ? a.Position.X.CompareTo(b.Position.X) : num2;
		});
		List<Box2> list2 = new List<Box2>(list.Count);
		foreach (var item4 in list)
		{
			Vector2 item3 = item4.Item2;
			Box2 labelRect = GetLabelRect(item3, item4.Item3, item4.Item4);
			int num = 0;
			while (num++ < 16)
			{
				Box2? val = null;
				foreach (Box2 item5 in list2)
				{
					Box2 current3 = item5;
					if (((Box2)(ref labelRect)).Intersects(ref current3))
					{
						val = current3;
						break;
					}
				}
				if (!val.HasValue)
				{
					break;
				}
				item3.Y = val.Value.Bottom + 4f;
				labelRect = GetLabelRect(item3, item4.Item3, item4.Item4);
			}
			list2.Add(labelRect);
			Vector2 vector = item3;
			foreach (DebugLine line in item4.Item1.Lines)
			{
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector + Vector2.One, line.Text, Color.Black);
				((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector, line.Text, line.Color);
				vector.Y += 13f;
			}
		}
	}

	private static float GetApproxLabelWidth(List<DebugLine> lines)
	{
		int num = 0;
		foreach (DebugLine line in lines)
		{
			if (line.Text.Length > num)
			{
				num = line.Text.Length;
			}
		}
		return (float)num * 7f;
	}

	private static Box2 GetLabelRect(Vector2 position, float width, float height)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		return Box2.FromDimensions(position - new Vector2(4f, 4f), new Vector2(width + 8f, height + 8f));
	}

	private unsafe void AddGunDebugLabel(EntityUid uid, GunMuzzleOffsetComponent muzzle, Vector2 origin, Vector2 muzzlePos)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		List<DebugLine> list = new List<DebugLine>();
		Angle rotation = _eye.CurrentEye.Rotation;
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		bool num = _turretQ.TryComp(uid, ref vehicleTurretComponent);
		bool flag = _portGunQ.HasComp(uid);
		string value = (num ? "turret-gun" : (flag ? "port-gun" : "gun"));
		list.Add(new DebugLine($"{value} {uid.Id}", Color.White));
		EntityUid val = uid;
		Angle val2 = _transform.GetWorldRotation(uid);
		Angle angle = val2;
		if (num && vehicleTurretComponent != null)
		{
			if (_vehicleTurretVisual.TryGetRenderedPose(uid, out var _, out var worldRotation))
			{
				angle = worldRotation;
			}
			if (TryGetVehicle(uid, out var vehicle))
			{
				TryGetAnchorTurret(uid, vehicleTurretComponent, out EntityUid _, out VehicleTurretComponent anchorTurret);
				Angle worldRotation2 = _transform.GetWorldRotation(vehicle);
				Angle vehicleFacingAngle = GetVehicleFacingAngle(vehicle, worldRotation2);
				Angle renderFacing = GetRenderFacing(vehicleTurretComponent, anchorTurret, worldRotation2, vehicleFacingAngle, rotation);
				Direction dir = (Direction)((!vehicleTurretComponent.UseDirectionalOffsets) ? (-1) : ((int)GetDirectionalDir(renderFacing)));
				Vector2 vector = (vehicleTurretComponent.UseDirectionalOffsets ? GetDirectionalOffset(vehicleTurretComponent, dir) : vehicleTurretComponent.PixelOffset);
				list.Add(new DebugLine($"veh {vehicle.Id} face {((Angle)(ref vehicleFacingAngle)).GetCardinalDir()} eye {FmtDeg(rotation)} phys {FmtDeg(val2)} vis {FmtDeg(angle)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
				list.Add(new DebugLine($"render {(vehicleTurretComponent.UseDirectionalOffsets ? ((object)(*(Direction*)(&dir))/*cast due to constrained. prefix*/).ToString() : "-")} {FmtVec(vector)} gun ", new Color(0.55f, 0.9f, 1f, 1f)));
				list[list.Count - 1] = new DebugLine(list[list.Count - 1].Text + (muzzle.UseDirectionalOffsets ? ((object)GetBaseDirection(val, val2)/*cast due to constrained. prefix*/).ToString() : "-") + " " + FmtVec(muzzle.UseDirectionalOffsets ? GetDirectionalGunOffset(muzzle, GetBaseDirection(val, val2)) : muzzle.Offset), new Color(1f, 0.75f, 0.3f, 1f));
			}
		}
		else
		{
			BaseContainer val3 = default(BaseContainer);
			if (muzzle.UseContainerOwner && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref val3))
			{
				val = val3.Owner;
			}
			val2 = GetBaseRotation(val, muzzle.AngleOffset);
			angle = val2;
			list.Add(new DebugLine($"base {val.Id} rot {FmtDeg(val2)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
		}
		Vector2 vector2 = muzzle.Offset;
		Vector2 vector3 = muzzle.Offset;
		string value2 = "-";
		if (muzzle.UseDirectionalOffsets)
		{
			Direction baseDirection = GetBaseDirection(val, val2);
			value2 = ((object)(*(Direction*)(&baseDirection))/*cast due to constrained. prefix*/).ToString();
			vector2 = GetDirectionalGunOffset(muzzle, baseDirection);
			vector3 = vector2;
		}
		Vector2 vector4 = ((muzzle.UseDirectionalOffsets && !muzzle.RotateDirectionalOffsets) ? vector3 : ((Angle)(ref val2)).RotateVec(ref vector3));
		Angle angle2 = val2;
		Vector2? vector5 = null;
		GunComponent gunComponent = default(GunComponent);
		if (_gunQ.TryComp(uid, ref gunComponent))
		{
			EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
			if (shootCoordinates.HasValue)
			{
				EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
				MapCoordinates val4 = _transform.ToMapCoordinates(valueOrDefault, true);
				vector5 = val4.Position;
				if (muzzle.UseAimDirection)
				{
					Vector2 vector6 = val4.Position - origin;
					if (vector6.LengthSquared() > 0.0001f)
					{
						angle2 = DirectionExtensions.ToWorldAngle(vector6) + muzzle.AngleOffset;
					}
				}
			}
		}
		list.Add(new DebugLine("origin " + FmtPos(origin) + " muzzle " + FmtPos(muzzlePos), new Color(0.3f, 0.9f, 1f, 1f)));
		list.Add(new DebugLine($"dir {value2} pick {FmtVec(vector2)} world {FmtVec(vector4)}", new Color(1f, 0.82f, 0.25f, 1f)));
		if (muzzle.MuzzleOffset != Vector2.Zero || muzzle.UseAimDirection)
		{
			list.Add(new DebugLine($"muzzle {FmtVec(muzzle.MuzzleOffset)} rot {FmtDeg(angle2)} aim={FmtBool(muzzle.UseAimDirection)}", new Color(0.72f, 1f, 0.76f, 1f)));
		}
		if (vector5.HasValue)
		{
			Vector2 valueOrDefault2 = vector5.GetValueOrDefault();
			list.Add(new DebugLine($"target {FmtPos(valueOrDefault2)} dist {(valueOrDefault2 - origin).Length():0.00}", new Color(0.95f, 0.4f, 0.95f, 1f)));
		}
		VehicleTurretMuzzleComponent vehicleTurretMuzzleComponent = default(VehicleTurretMuzzleComponent);
		if (_turretMuzzleQ.TryComp(uid, ref vehicleTurretMuzzleComponent))
		{
			Direction baseDirection2 = GetBaseDirection(uid, val2);
			Vector2 directionalTurretOffset = GetDirectionalTurretOffset(vehicleTurretMuzzleComponent, baseDirection2, useRight: false);
			Vector2 directionalTurretOffset2 = GetDirectionalTurretOffset(vehicleTurretMuzzleComponent, baseDirection2, useRight: true);
			list.Add(new DebugLine($"alt {(vehicleTurretMuzzleComponent.UseRightNext ? "R" : "L")} L {FmtVec(directionalTurretOffset)} R {FmtVec(directionalTurretOffset2)}", new Color(1f, 0.65f, 0.25f, 1f)));
		}
		_labels.Add(new DebugLabel(muzzlePos + GunLabelWorldOffset, list));
	}

	private unsafe void AddTurretDebugLabel(EntityUid uid, VehicleTurretComponent turret, Vector2 basePos, Vector2 anchorPos, Vector2 turretPos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		List<DebugLine> list = new List<DebugLine>
		{
			new DebugLine($"turret {uid.Id}", Color.White)
		};
		if (TryGetVehicle(uid, out var vehicle))
		{
			TryGetAnchorTurret(uid, turret, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret);
			Angle worldRotation = _transform.GetWorldRotation(vehicle);
			Angle rotation = _eye.CurrentEye.Rotation;
			Angle vehicleFacingAngle = GetVehicleFacingAngle(vehicle, worldRotation);
			Angle renderFacing = GetRenderFacing(turret, anchorTurret, worldRotation, vehicleFacingAngle, rotation);
			Direction dir = (Direction)((!turret.UseDirectionalOffsets) ? (-1) : ((int)GetDirectionalDir(renderFacing)));
			Vector2 vector = (turret.UseDirectionalOffsets ? GetDirectionalOffset(turret, dir) : turret.PixelOffset);
			Angle angle = _transform.GetWorldRotation(uid);
			if (_vehicleTurretVisual.TryGetRenderedPose(uid, out var _, out var worldRotation2))
			{
				angle = worldRotation2;
			}
			list.Add(new DebugLine($"veh {vehicle.Id} face {((Angle)(ref vehicleFacingAngle)).GetCardinalDir()} eye {FmtDeg(rotation)} render {FmtDeg(angle)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
			list.Add(new DebugLine($"anchor {anchorUid.Id} world {FmtDeg(turret.WorldRotation)} target {FmtDeg(turret.TargetRotation)}", new Color(0.55f, 0.9f, 1f, 1f)));
			list.Add(new DebugLine($"pix {(turret.UseDirectionalOffsets ? ((object)(*(Direction*)(&dir))/*cast due to constrained. prefix*/).ToString() : "-")} {FmtVec(vector)} base {FmtPos(basePos)} pos {FmtPos(turretPos)}", new Color(0.75f, 0.95f, 0.65f, 1f)));
		}
		_labels.Add(new DebugLabel(turretPos + TurretLabelWorldOffset, list));
	}

	private static string FmtBool(bool value)
	{
		if (!value)
		{
			return "N";
		}
		return "Y";
	}

	private static string FmtDeg(Angle angle)
	{
		return $"{((Angle)(ref angle)).Degrees:0.0}";
	}

	private static string FmtVec(Vector2 vector)
	{
		return $"<{vector.X:0.00},{vector.Y:0.00}>";
	}

	private static string FmtPos(Vector2 vector)
	{
		return $"{vector.X:0.00},{vector.Y:0.00}";
	}

	private bool TryGetMuzzlePositions(EntityUid uid, GunMuzzleOffsetComponent muzzle, MapId mapId, out Vector2 origin, out Vector2 muzzlePos)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		origin = default(Vector2);
		muzzlePos = default(Vector2);
		if (_turretQ.HasComp(uid) && _vehicleTurretMuzzle.TryGetGunOrigin(uid, null, out var origin2))
		{
			MapCoordinates val = _transform.ToMapCoordinates(origin2, true);
			if (val.MapId != mapId)
			{
				return false;
			}
			origin = val.Position;
			muzzlePos = origin;
			return true;
		}
		EntityUid val2 = uid;
		BaseContainer val3 = default(BaseContainer);
		if (muzzle.UseContainerOwner && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref val3))
		{
			val2 = val3.Owner;
		}
		TransformComponent val4 = default(TransformComponent);
		if (!_ents.TryGetComponent<TransformComponent>(val2, ref val4))
		{
			return false;
		}
		if (val4.MapID != mapId)
		{
			return false;
		}
		EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(val2);
		Angle baseRotation = GetBaseRotation(val2, muzzle.AngleOffset);
		(Vector2 Offset, bool Rotate) offset = GetOffset(muzzle, val2, baseRotation);
		Vector2 item = offset.Offset;
		EntityCoordinates val5 = (offset.Rotate ? ((EntityCoordinates)(ref moverCoordinates)).Offset(((Angle)(ref baseRotation)).RotateVec(ref item)) : ((EntityCoordinates)(ref moverCoordinates)).Offset(item));
		origin = _transform.ToMapCoordinates(val5, true).Position;
		EntityCoordinates val6 = val5;
		Angle val7 = baseRotation;
		if (muzzle.MuzzleOffset != Vector2.Zero)
		{
			GunComponent gunComponent = default(GunComponent);
			if (muzzle.UseAimDirection && _gunQ.TryComp(uid, ref gunComponent))
			{
				EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
				if (shootCoordinates.HasValue)
				{
					EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
					MapCoordinates val8 = _transform.ToMapCoordinates(val5, true);
					Vector2 vector = _transform.ToMapCoordinates(valueOrDefault, true).Position - val8.Position;
					if (vector.LengthSquared() > 0.0001f)
					{
						val7 = DirectionExtensions.ToWorldAngle(vector) + muzzle.AngleOffset;
					}
				}
			}
			val6 = ((EntityCoordinates)(ref val5)).Offset(((Angle)(ref val7)).RotateVec(ref muzzle.MuzzleOffset));
		}
		muzzlePos = _transform.ToMapCoordinates(val6, true).Position;
		return true;
	}

	private bool TryGetTurretMuzzlePositions(EntityUid uid, VehicleTurretMuzzleComponent turretMuzzle, MapId mapId, out Vector2 turretBasePos, out Vector2 basePos, out Vector2 leftPos, out Vector2 rightPos, out float leftRadius, out float rightRadius, out bool useRightNext)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		turretBasePos = default(Vector2);
		basePos = default(Vector2);
		leftPos = default(Vector2);
		rightPos = default(Vector2);
		leftRadius = 0f;
		rightRadius = 0f;
		useRightNext = turretMuzzle.UseRightNext;
		TransformComponent val = default(TransformComponent);
		if (!_ents.TryGetComponent<TransformComponent>(uid, ref val))
		{
			return false;
		}
		if (val.MapID != mapId)
		{
			return false;
		}
		Angle baseRotation = _transform.GetWorldRotation(uid);
		if (_vehicleTurretVisual.TryGetRenderedPose(uid, out var origin, out var worldRotation))
		{
			MapCoordinates val2 = _transform.ToMapCoordinates(origin, true);
			if (val2.MapId != mapId)
			{
				return false;
			}
			turretBasePos = val2.Position;
			baseRotation = worldRotation;
		}
		else
		{
			turretBasePos = _transform.ToMapCoordinates(_transform.GetMoverCoordinates(uid), true).Position;
		}
		if (!TryGetGunOriginCoordinates(uid, mapId, out var originCoords))
		{
			return false;
		}
		basePos = _transform.ToMapCoordinates(originCoords, true).Position;
		leftPos = basePos + GetTurretMuzzleWorldOffset(turretMuzzle, baseRotation, useRight: false);
		rightPos = basePos + GetTurretMuzzleWorldOffset(turretMuzzle, baseRotation, useRight: true);
		leftRadius = (leftPos - turretBasePos).Length();
		rightRadius = (rightPos - turretBasePos).Length();
		return true;
	}

	private bool TryGetGunOriginCoordinates(EntityUid uid, MapId mapId, out EntityCoordinates originCoords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		originCoords = default(EntityCoordinates);
		if (_turretQ.HasComp(uid) && _vehicleTurretMuzzle.TryGetGunOrigin(uid, null, out var origin))
		{
			if (_transform.ToMapCoordinates(origin, true).MapId != mapId)
			{
				return false;
			}
			originCoords = origin;
			return true;
		}
		EntityUid val = uid;
		GunMuzzleOffsetComponent gunMuzzleOffsetComponent = default(GunMuzzleOffsetComponent);
		BaseContainer val2 = default(BaseContainer);
		if (_muzzleQ.TryComp(uid, ref gunMuzzleOffsetComponent) && gunMuzzleOffsetComponent.UseContainerOwner && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref val2))
		{
			val = val2.Owner;
		}
		TransformComponent val3 = default(TransformComponent);
		if (!_ents.TryGetComponent<TransformComponent>(val, ref val3))
		{
			return false;
		}
		if (val3.MapID != mapId)
		{
			return false;
		}
		EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(val);
		if (gunMuzzleOffsetComponent == null)
		{
			originCoords = moverCoordinates;
			return true;
		}
		Angle baseRotation = GetBaseRotation(val, gunMuzzleOffsetComponent.AngleOffset);
		(Vector2 Offset, bool Rotate) offset = GetOffset(gunMuzzleOffsetComponent, val, baseRotation);
		Vector2 item = offset.Offset;
		EntityCoordinates val4 = (offset.Rotate ? ((EntityCoordinates)(ref moverCoordinates)).Offset(((Angle)(ref baseRotation)).RotateVec(ref item)) : ((EntityCoordinates)(ref moverCoordinates)).Offset(item));
		if (gunMuzzleOffsetComponent.MuzzleOffset != Vector2.Zero)
		{
			Angle val5 = baseRotation;
			GunComponent gunComponent = default(GunComponent);
			if (gunMuzzleOffsetComponent.UseAimDirection && _gunQ.TryComp(uid, ref gunComponent))
			{
				EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
				if (shootCoordinates.HasValue)
				{
					EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
					MapCoordinates val6 = _transform.ToMapCoordinates(val4, true);
					Vector2 vector = _transform.ToMapCoordinates(valueOrDefault, true).Position - val6.Position;
					if (vector.LengthSquared() > 0.0001f)
					{
						val5 = DirectionExtensions.ToWorldAngle(vector) + gunMuzzleOffsetComponent.AngleOffset;
					}
				}
			}
			val4 = ((EntityCoordinates)(ref val4)).Offset(((Angle)(ref val5)).RotateVec(ref gunMuzzleOffsetComponent.MuzzleOffset));
		}
		originCoords = val4;
		return true;
	}

	private bool TryGetTurretOverlayPositions(EntityUid turretUid, VehicleTurretComponent turret, MapId mapId, out Vector2 basePos, out Vector2 anchorPos, out Vector2 turretPos)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		basePos = default(Vector2);
		anchorPos = default(Vector2);
		turretPos = default(Vector2);
		if (!TryGetVehicle(turretUid, out var vehicle))
		{
			return false;
		}
		EntityCoordinates moverCoordinates = _transform.GetMoverCoordinates(vehicle);
		MapCoordinates val = _transform.ToMapCoordinates(moverCoordinates, true);
		if (val.MapId != mapId)
		{
			return false;
		}
		TryGetAnchorTurret(turretUid, turret, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret);
		Angle worldRotation = _transform.GetWorldRotation(vehicle);
		Angle rotation = _eye.CurrentEye.Rotation;
		Angle vehicleFacingAngle = GetVehicleFacingAngle(vehicle, worldRotation);
		Angle renderFacing = GetRenderFacing(anchorTurret, anchorTurret, worldRotation, vehicleFacingAngle, rotation);
		Vector2 offset = GetPixelOffset(anchorTurret, renderFacing) / 32f;
		Vector2 vehicleLocalOffset = GetVehicleLocalOffset(anchorTurret, offset, worldRotation, rotation);
		EntityCoordinates val2 = ((EntityCoordinates)(ref moverCoordinates)).Offset(vehicleLocalOffset);
		basePos = val.Position;
		anchorPos = _transform.ToMapCoordinates(val2, true).Position;
		if (anchorUid == turretUid)
		{
			turretPos = anchorPos;
			return true;
		}
		Angle val3 = (anchorTurret.RotateToCursor ? anchorTurret.WorldRotation : Angle.Zero);
		Angle renderFacing2 = GetRenderFacing(turret, anchorTurret, worldRotation, vehicleFacingAngle, rotation);
		Vector2 vector = GetPixelOffset(turret, renderFacing2) / 32f;
		Angle val4;
		Vector2 vector2;
		if (turret.OffsetRotatesWithTurret)
		{
			if (turret.UseDirectionalOffsets)
			{
				Angle directionalAngle = GetDirectionalAngle(GetDirectionalDir(renderFacing2));
				val4 = -directionalAngle;
				vector2 = ((Angle)(ref val4)).RotateVec(ref vector);
				val4 = val3 - directionalAngle;
				Vector2 vector3 = ((Angle)(ref val4)).RotateVec(ref vector);
			}
			else
			{
				vector2 = vector;
				Vector2 vector3 = ((Angle)(ref val3)).RotateVec(ref vector2);
			}
		}
		else
		{
			val4 = -worldRotation;
			Vector2 vector3 = ((Angle)(ref val4)).RotateVec(ref vector);
			val4 = -val3;
			vector2 = ((Angle)(ref val4)).RotateVec(ref vector3);
		}
		MapCoordinates val5 = ((!turret.OffsetRotatesWithTurret) ? _transform.ToMapCoordinates(((EntityCoordinates)(ref moverCoordinates)).Offset(vehicleLocalOffset + vector2), true) : _transform.ToMapCoordinates(new EntityCoordinates(anchorUid, vector2), true));
		if (val5.MapId != mapId)
		{
			return false;
		}
		turretPos = val5.Position;
		return true;
	}

	private Vector2 GetPixelOffset(VehicleTurretComponent turret, Angle facing)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!turret.UseDirectionalOffsets)
		{
			return turret.PixelOffset;
		}
		Vector2 pixelOffset = turret.PixelOffset;
		double num = facing.Theta % 6.2831854820251465;
		if (num < 0.0)
		{
			num += 6.2831854820251465;
		}
		Direction directionalDir = GetDirectionalDir((float)num);
		return pixelOffset + GetDirectionalOffset(turret, directionalDir);
	}

	private static Vector2 GetDirectionalOffset(VehicleTurretComponent turret, Direction dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)dir switch
		{
			0 => turret.PixelOffsetSouth, 
			2 => turret.PixelOffsetEast, 
			4 => turret.PixelOffsetNorth, 
			6 => turret.PixelOffsetWest, 
			_ => Vector2.Zero, 
		};
	}

	private static Direction GetDirectionalDir(Angle facing)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(facing);
	}

	private static Direction GetDirectionalDir(float normalized)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(new Angle((double)normalized));
	}

	private static Angle GetDirectionalAngle(Direction dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DirectionExtensions.ToAngle(dir);
	}

	private static Vector2 GetDirectionalGunOffset(GunMuzzleOffsetComponent muzzle, Direction dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)dir switch
		{
			4 => muzzle.OffsetNorth, 
			2 => muzzle.OffsetEast, 
			0 => muzzle.OffsetSouth, 
			6 => muzzle.OffsetWest, 
			_ => muzzle.Offset, 
		};
	}

	private static Vector2 GetDirectionalTurretOffset(VehicleTurretMuzzleComponent muzzle, Direction dir, bool useRight)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected I4, but got Unknown
		if (!muzzle.UseDirectionalOffsets)
		{
			if (!useRight)
			{
				return muzzle.OffsetLeft;
			}
			return muzzle.OffsetRight;
		}
		return (int)dir switch
		{
			4 => useRight ? muzzle.OffsetRightNorth : muzzle.OffsetLeftNorth, 
			2 => useRight ? muzzle.OffsetRightEast : muzzle.OffsetLeftEast, 
			0 => useRight ? muzzle.OffsetRightSouth : muzzle.OffsetLeftSouth, 
			6 => useRight ? muzzle.OffsetRightWest : muzzle.OffsetLeftWest, 
			_ => useRight ? muzzle.OffsetRight : muzzle.OffsetLeft, 
		};
	}

	private static Vector2 GetTurretMuzzleWorldOffset(VehicleTurretMuzzleComponent muzzle, Angle baseRotation, bool useRight)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Vector2 directionalTurretOffset = GetDirectionalTurretOffset(muzzle, VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation), useRight);
		return ((Angle)(ref baseRotation)).RotateVec(ref directionalTurretOffset);
	}

	private void DrawShootArc(EntityUid uid, Vector2 origin, MapId mapId, DrawingHandleWorld handle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		GunFireArcComponent gunFireArcComponent = default(GunFireArcComponent);
		BaseContainer val = default(BaseContainer);
		TransformComponent val2 = default(TransformComponent);
		if (_fireArcQ.TryComp(uid, ref gunFireArcComponent) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(uid, null)), ref val) && _ents.TryGetComponent<TransformComponent>(val.Owner, ref val2) && !(val2.MapID != mapId))
		{
			Angle baseRotation = GetBaseRotation(val.Owner, gunFireArcComponent.AngleOffset);
			Angle val3 = Angle.FromDegrees(((Angle)(ref gunFireArcComponent.Arc)).Degrees / 2.0);
			Angle val4 = baseRotation + val3;
			Angle val5 = baseRotation - val3;
			((DrawingHandleBase)handle).DrawLine(origin, origin + ((Angle)(ref baseRotation)).ToWorldVec() * 3.5f, new Color(0.2f, 0.9f, 0.3f, 0.8f));
			((DrawingHandleBase)handle).DrawLine(origin, origin + ((Angle)(ref val4)).ToWorldVec() * 3.5f, new Color(0.95f, 0.45f, 0.2f, 0.8f));
			((DrawingHandleBase)handle).DrawLine(origin, origin + ((Angle)(ref val5)).ToWorldVec() * 3.5f, new Color(0.95f, 0.45f, 0.2f, 0.8f));
		}
	}

	private void DrawShootTarget(EntityUid uid, Vector2 origin, MapId mapId, DrawingHandleWorld handle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComponent = default(GunComponent);
		if (_gunQ.TryComp(uid, ref gunComponent) && gunComponent.ShootCoordinates.HasValue)
		{
			MapCoordinates val = _transform.ToMapCoordinates(gunComponent.ShootCoordinates.Value, true);
			if (!(val.MapId != mapId))
			{
				((DrawingHandleBase)handle).DrawLine(origin, val.Position, new Color(0.9f, 0.2f, 0.9f, 0.7f));
				((DrawingHandleBase)handle).DrawCircle(val.Position, 0.08f, new Color(0.9f, 0.2f, 0.9f, 0.8f), true);
			}
		}
	}

	private Angle GetBaseRotation(EntityUid baseUid, Angle angleOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Angle val = _transform.GetWorldRotation(baseUid);
		GridVehicleMoverComponent gridVehicleMoverComponent = default(GridVehicleMoverComponent);
		if (_moverQ.TryComp(baseUid, ref gridVehicleMoverComponent) && gridVehicleMoverComponent.CurrentDirection != Vector2i.Zero)
		{
			val = DirectionExtensions.ToWorldAngle(new Vector2(gridVehicleMoverComponent.CurrentDirection.X, gridVehicleMoverComponent.CurrentDirection.Y));
		}
		return val + angleOffset;
	}

	private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent gridVehicleMoverComponent = default(GridVehicleMoverComponent);
		if (_moverQ.TryComp(vehicle, ref gridVehicleMoverComponent) && gridVehicleMoverComponent.CurrentDirection != Vector2i.Zero)
		{
			return DirectionExtensions.ToWorldAngle(new Vector2(gridVehicleMoverComponent.CurrentDirection.X, gridVehicleMoverComponent.CurrentDirection.Y));
		}
		return vehicleRot;
	}

	private static Vector2 GetVehicleLocalOffset(VehicleTurretComponent turret, Vector2 offset, Angle vehicleRot, Angle eyeRot)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Angle val;
		if (turret.UseDirectionalOffsets)
		{
			val = -eyeRot;
			offset = ((Angle)(ref val)).RotateVec(ref offset);
		}
		val = -vehicleRot;
		return ((Angle)(ref val)).RotateVec(ref offset);
	}

	private static Angle GetRenderFacing(VehicleTurretComponent turret, VehicleTurretComponent anchorTurret, Angle vehicleRot, Angle baseFacingAngle, Angle eyeRot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Angle val = GetOffsetFacing(turret, anchorTurret, vehicleRot, baseFacingAngle) + eyeRot;
		return ((Angle)(ref val)).Reduced();
	}

	private static Angle GetOffsetFacing(VehicleTurretComponent turret, VehicleTurretComponent anchorTurret, Angle vehicleRot, Angle baseFacingAngle)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!turret.OffsetRotatesWithTurret)
		{
			return baseFacingAngle;
		}
		Angle val = vehicleRot + anchorTurret.WorldRotation;
		return ((Angle)(ref val)).Reduced();
	}

	private (Vector2 Offset, bool Rotate) GetOffset(GunMuzzleOffsetComponent muzzle, EntityUid baseUid, Angle baseRotation)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected I4, but got Unknown
		if (!muzzle.UseDirectionalOffsets)
		{
			return (Offset: muzzle.Offset, Rotate: true);
		}
		Direction baseDirection = GetBaseDirection(baseUid, baseRotation);
		return (Offset: (int)baseDirection switch
		{
			4 => muzzle.OffsetNorth, 
			2 => muzzle.OffsetEast, 
			0 => muzzle.OffsetSouth, 
			6 => muzzle.OffsetWest, 
			_ => muzzle.Offset, 
		}, Rotate: muzzle.RotateDirectionalOffsets);
	}

	private Direction GetBaseDirection(EntityUid baseUid, Angle baseRotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent gridVehicleMoverComponent = default(GridVehicleMoverComponent);
		if (_moverQ.TryComp(baseUid, ref gridVehicleMoverComponent) && gridVehicleMoverComponent.CurrentDirection != Vector2i.Zero)
		{
			return DirectionExtensions.AsDirection(gridVehicleMoverComponent.CurrentDirection);
		}
		return ((Angle)(ref baseRotation)).GetCardinalDir();
	}

	private bool TryGetVehicle(EntityUid turretUid, out EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		vehicle = default(EntityUid);
		EntityUid item = turretUid;
		BaseContainer val = default(BaseContainer);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref val))
		{
			EntityUid owner = val.Owner;
			if (_ents.HasComponent<VehicleComponent>(owner))
			{
				vehicle = owner;
				return true;
			}
			item = owner;
		}
		return false;
	}

	private void TryGetAnchorTurret(EntityUid turretUid, VehicleTurretComponent turret, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		anchorUid = turretUid;
		anchorTurret = turret;
		if (_ents.HasComponent<VehicleTurretAttachmentComponent>(turretUid) && TryGetParentTurret(turretUid, out EntityUid parentUid, out VehicleTurretComponent parentTurret))
		{
			anchorUid = parentUid;
			anchorTurret = parentTurret;
		}
	}

	private bool TryGetParentTurret(EntityUid turretUid, out EntityUid parentUid, out VehicleTurretComponent parentTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		parentUid = default(EntityUid);
		parentTurret = null;
		EntityUid item = turretUid;
		BaseContainer val = default(BaseContainer);
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref val))
		{
			EntityUid owner = val.Owner;
			if (_turretQ.TryComp(owner, ref vehicleTurretComponent))
			{
				parentUid = owner;
				parentTurret = vehicleTurretComponent;
				return true;
			}
			item = owner;
		}
		return false;
	}
}
