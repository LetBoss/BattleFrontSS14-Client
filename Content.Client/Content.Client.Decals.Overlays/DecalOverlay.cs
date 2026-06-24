using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Decals.Overlays;

public sealed class DecalOverlay : GridOverlay
{
	private readonly SpriteSystem _sprites;

	private readonly IEntityManager _entManager;

	private readonly IPrototypeManager _prototypeManager;

	private readonly Dictionary<string, (Texture Texture, bool SnapCardinals)> _cachedTextures = new Dictionary<string, (Texture, bool)>(64);

	private readonly List<(uint Id, Decal Decal)> _decals = new List<(uint, Decal)>();

	public DecalOverlay(SpriteSystem sprites, IEntityManager entManager, IPrototypeManager prototypeManager)
	{
		_sprites = sprites;
		_entManager = entManager;
		_prototypeManager = prototypeManager;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		if (args.MapId == MapId.Nullspace)
		{
			return;
		}
		EntityUid owner = ((GridOverlay)this).Grid.Owner;
		DecalGridComponent decalGridComponent = default(DecalGridComponent);
		TransformComponent val = default(TransformComponent);
		if (!_entManager.TryGetComponent<DecalGridComponent>(owner, ref decalGridComponent) || !_entManager.TryGetComponent<TransformComponent>(owner, ref val) || val.MapID != args.MapId)
		{
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		TransformSystem val2 = _entManager.System<TransformSystem>();
		IEye eye = args.Viewport.Eye;
		Angle val3 = ((eye != null) ? eye.Rotation : Angle.Zero);
		Matrix3x2 invWorldMatrix = ((SharedTransformSystem)val2).GetInvWorldMatrix(val);
		Box2Rotated val4 = ((Box2Rotated)(ref args.WorldBounds)).Enlarged(1f);
		Box2 val5 = Matrix3Helpers.TransformBox(invWorldMatrix, ref val4);
		ChunkIndicesEnumerator val6 = default(ChunkIndicesEnumerator);
		((ChunkIndicesEnumerator)(ref val6))._002Ector(val5, 32);
		_decals.Clear();
		Vector2i? val7 = default(Vector2i?);
		while (((ChunkIndicesEnumerator)(ref val6)).MoveNext(ref val7))
		{
			if (!decalGridComponent.ChunkCollection.ChunkCollection.TryGetValue(val7.Value, out DecalGridComponent.DecalChunk value))
			{
				continue;
			}
			foreach (var (item, decal2) in value.Decals)
			{
				if (((Box2)(ref val5)).Contains(decal2.Coordinates, true))
				{
					_decals.Add((item, decal2));
				}
			}
		}
		if (_decals.Count == 0)
		{
			return;
		}
		_decals.Sort(delegate((uint Id, Decal Decal) x, (uint Id, Decal Decal) y)
		{
			int num2 = x.Decal.ZIndex.CompareTo(y.Decal.ZIndex);
			return (num2 != 0) ? num2 : x.Id.CompareTo(y.Id);
		});
		ValueTuple<Vector2, Angle, Matrix3x2> worldPositionRotationMatrix = ((SharedTransformSystem)val2).GetWorldPositionRotationMatrix(val);
		Angle item2 = worldPositionRotationMatrix.Item2;
		Matrix3x2 item3 = worldPositionRotationMatrix.Item3;
		((DrawingHandleBase)worldHandle).SetTransform(ref item3);
		DecalPrototype decalPrototype = default(DecalPrototype);
		foreach (var decal3 in _decals)
		{
			Decal item4 = decal3.Decal;
			if (!_cachedTextures.TryGetValue(item4.Id, out (Texture, bool) value2))
			{
				if (!_prototypeManager.TryIndex<DecalPrototype>(item4.Id, ref decalPrototype))
				{
					continue;
				}
				value2 = (_sprites.Frame0(decalPrototype.Sprite), decalPrototype.SnapCardinals);
				_cachedTextures[item4.Id] = value2;
			}
			Angle val8 = Angle.Zero;
			if (value2.Item2)
			{
				Angle val9 = val3 + item2;
				val8 = DirectionExtensions.ToAngle(((Angle)(ref val9)).GetCardinalDir());
			}
			Angle val10 = item4.Angle - val8;
			if (((Angle)(ref val10)).Equals(Angle.Zero))
			{
				((DrawingHandleBase)worldHandle).DrawTexture(value2.Item1, item4.Coordinates, item4.Color);
			}
			else
			{
				worldHandle.DrawTexture(value2.Item1, item4.Coordinates, val10, item4.Color);
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}
}
