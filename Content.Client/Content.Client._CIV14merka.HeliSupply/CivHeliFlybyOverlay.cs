using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliFlybyOverlay : Overlay
{
	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IGameTiming _timing;

	private const float HeadingLookahead = 2.5f;

	private static readonly Color DustColor = Color.FromHex((ReadOnlySpan<char>)"#C2B280", (Color?)null);

	private Texture[] _usaFrames = Array.Empty<Texture>();

	private Texture[] _ruFrames = Array.Empty<Texture>();

	private Texture[] _dustFrames = Array.Empty<Texture>();

	private float _frameDelay = 0.1f;

	public override OverlaySpace Space => (OverlaySpace)4;

	public List<CivHeliFlybyInstance> Instances { get; } = new List<CivHeliFlybyInstance>();

	public CivHeliFlybyOverlay()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CivHeliFlybyOverlay>(this);
		RSI rSI = _resourceCache.GetResource<RSIResource>(new ResPath("/Textures/_CIV14merka/Effects/heli_shadows.rsi"), true).RSI;
		State val = default(State);
		if (rSI.TryGetState(StateId.op_Implicit("usa"), ref val))
		{
			_usaFrames = val.GetFrames((RsiDirection)0);
			float[] delays = val.GetDelays();
			if (delays.Length != 0 && delays[0] > 0f)
			{
				_frameDelay = delays[0];
			}
		}
		State val2 = default(State);
		if (rSI.TryGetState(StateId.op_Implicit("ru"), ref val2))
		{
			_ruFrames = val2.GetFrames((RsiDirection)0);
		}
		State val3 = default(State);
		if (_resourceCache.GetResource<RSIResource>(new ResPath("/Textures/Effects/chemsmoke.rsi"), true).RSI.TryGetState(StateId.op_Implicit("chemsmoke_white"), ref val3))
		{
			_dustFrames = val3.GetFrames((RsiDirection)0);
		}
		((Overlay)this).ZIndex = -10;
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		return Instances.Count > 0;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		TimeSpan realTime = _timing.RealTime;
		Matrix3x2 identity;
		foreach (CivHeliFlybyInstance instance in Instances)
		{
			if (instance.MapId != args.MapId)
			{
				continue;
			}
			identity = Matrix3x2.Identity;
			((DrawingHandleBase)worldHandle).SetTransform(ref identity);
			DrawDust(worldHandle, instance);
			Texture[] array = ((instance.Side == CivAirstrikeSide.Ru) ? _ruFrames : _usaFrames);
			if (array.Length == 0)
			{
				continue;
			}
			float num = (float)(realTime - instance.StartTime).TotalSeconds;
			if (num < 0f)
			{
				continue;
			}
			float num2 = num * instance.Speed;
			if (!(num2 > instance.Path.TotalCost))
			{
				instance.Path.SampleByCost(num2, out var pos, out var tangent);
				instance.Path.SampleByCost(MathF.Max(0f, num2 - 2.5f), out var pos2, out var tangent2);
				instance.Path.SampleByCost(MathF.Min(instance.Path.TotalCost, num2 + 2.5f), out var pos3, out tangent2);
				Vector2 vector = pos3 - pos2;
				if (vector.LengthSquared() < 0.0001f)
				{
					vector = tangent;
				}
				if (vector.LengthSquared() < 0.0001f)
				{
					vector = Vector2.UnitX;
				}
				float num3 = ComputeScale(instance, num2);
				float num4 = 1f;
				float num5 = ((instance.Path.TotalCost > 0f) ? (num2 / instance.Path.TotalCost) : 1f);
				if (num5 < 0.05f)
				{
					num4 = num5 / 0.05f;
				}
				else if (num5 > 0.95f)
				{
					num4 = (1f - num5) / 0.05f;
				}
				Color white = Color.White;
				Color value = ((Color)(ref white)).WithAlpha(instance.Alpha * num4);
				Matrix3x2 value2 = Matrix3Helpers.CreateRotation((double)(MathF.Atan2(vector.Y, vector.X) + instance.AngleOffset));
				tangent2 = new Vector2(num3, num3);
				Matrix3x2 value3 = Matrix3Helpers.CreateScale(ref tangent2);
				Texture val = array[(int)(num / _frameDelay) % array.Length];
				Vector2 vector2 = new Vector2(val.Width, val.Height) / 32f / 2f;
				Matrix3x2 matrix3x = Matrix3x2.Multiply(value2: Matrix3Helpers.CreateTranslation(pos), value1: Matrix3x2.Multiply(value3, value2));
				((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
				worldHandle.DrawTextureRect(val, new Box2(-vector2, vector2), (Color?)value);
			}
		}
		identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private static float ComputeScale(CivHeliFlybyInstance inst, float cost)
	{
		float num = inst.Scale;
		float num2 = inst.Path.DistAtCost(cost);
		if (inst.TakeoffZone > 0.01f && num2 < inst.TakeoffZone)
		{
			num = MathF.Max(num, MathHelper.Lerp(inst.TakeoffScale, inst.Scale, num2 / inst.TakeoffZone));
		}
		if (num2 < inst.DropDist)
		{
			float num3 = inst.DropDist - num2;
			if (inst.DropZone > 0.01f && num3 < inst.DropZone)
			{
				num = MathF.Max(num, MathHelper.Lerp(inst.DropScale, inst.Scale, num3 / inst.DropZone));
			}
		}
		else
		{
			float num4 = num2 - inst.DropDist;
			if (inst.ClimbZone > 0.01f && num4 < inst.ClimbZone)
			{
				num = MathF.Max(num, MathHelper.Lerp(inst.DropScale, inst.Scale, num4 / inst.ClimbZone));
			}
		}
		return num;
	}

	private void DrawDust(DrawingHandleWorld handle, CivHeliFlybyInstance inst)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (_dustFrames.Length == 0 || inst.Dust.Count == 0)
		{
			return;
		}
		foreach (CivHeliDustParticle item in inst.Dust)
		{
			float num = Math.Clamp(item.Age / item.Life, 0f, 1f);
			Texture val = _dustFrames[Math.Min(_dustFrames.Length - 1, (int)(num * (float)_dustFrames.Length))];
			float num2 = item.Size * (0.7f + num * 0.8f);
			float num3 = 0.35f * (1f - num);
			Vector2 vector = new Vector2(num2, num2) / 2f;
			handle.DrawTextureRect(val, new Box2(item.Pos - vector, item.Pos + vector), (Color?)((Color)(ref DustColor)).WithAlpha(num3));
		}
	}
}
