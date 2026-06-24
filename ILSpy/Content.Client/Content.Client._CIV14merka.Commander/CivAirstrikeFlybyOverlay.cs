using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybyOverlay : Overlay
{
	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IGameTiming _timing;

	private Texture? _usaTex;

	private Texture? _ruTex;

	public override OverlaySpace Space => (OverlaySpace)4;

	public List<CivAirstrikeFlybyInstance> Instances { get; } = new List<CivAirstrikeFlybyInstance>();

	public CivAirstrikeFlybyOverlay()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<CivAirstrikeFlybyOverlay>(this);
		RSI rSI = _resourceCache.GetResource<RSIResource>(new ResPath("/Textures/_CIV14merka/Effects/jet_shadows.rsi"), true).RSI;
		State val = default(State);
		if (rSI.TryGetState(StateId.op_Implicit("usa"), ref val))
		{
			_usaTex = val.Frame0;
		}
		State val2 = default(State);
		if (rSI.TryGetState(StateId.op_Implicit("ru"), ref val2))
		{
			_ruTex = val2.Frame0;
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
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		TimeSpan realTime = _timing.RealTime;
		foreach (CivAirstrikeFlybyInstance instance in Instances)
		{
			if (instance.MapId != args.MapId)
			{
				continue;
			}
			Texture val = ((instance.Side == CivAirstrikeSide.Ru) ? _ruTex : _usaTex);
			if (val == null)
			{
				continue;
			}
			float num = (float)(realTime - instance.StartTime).TotalSeconds;
			if (num < 0f)
			{
				continue;
			}
			float num2 = num * instance.Speed;
			if (!(num2 > instance.Total))
			{
				Vector2 zero = Vector2.Zero;
				Vector2 unitX = Vector2.UnitX;
				if (num2 <= instance.EntryLineLen)
				{
					float amount = ((instance.EntryLineLen > 0f) ? (num2 / instance.EntryLineLen) : 1f);
					zero = Vector2.Lerp(instance.Origin, instance.Entry, amount);
					unitX = instance.Entry - instance.Origin;
				}
				else if (num2 <= instance.EntryTurnEnd)
				{
					float dist = num2 - instance.EntryLineLen;
					zero = CivAirstrikeFlybyMath.ArcPos(instance.EntryCtr, instance.Entry, instance.Approach, instance.EntryCcw, dist);
					unitX = CivAirstrikeFlybyMath.ArcTangent(instance.EntryCtr, zero, instance.EntryCcw);
				}
				else if (num2 <= instance.ToTarget)
				{
					float num3 = num2 - instance.EntryTurnEnd;
					float amount2 = ((instance.RunInLen > 0f) ? (num3 / instance.RunInLen) : 1f);
					zero = Vector2.Lerp(instance.Approach, instance.Target, amount2);
					unitX = instance.Target - instance.Approach;
				}
				else if (num2 <= instance.RunEndDist)
				{
					float num4 = num2 - instance.ToTarget;
					float amount3 = ((instance.RunOutLen > 0f) ? (num4 / instance.RunOutLen) : 1f);
					zero = Vector2.Lerp(instance.Target, instance.RunEnd, amount3);
					unitX = instance.RunEnd - instance.Target;
				}
				else if (num2 <= instance.ExitTurnEnd)
				{
					float dist2 = num2 - instance.RunEndDist;
					zero = CivAirstrikeFlybyMath.ArcPos(instance.ExitCtr, instance.RunEnd, instance.ExitTurn, instance.ExitCcw, dist2);
					unitX = CivAirstrikeFlybyMath.ArcTangent(instance.ExitCtr, zero, instance.ExitCcw);
				}
				else
				{
					float num5 = num2 - instance.ExitTurnEnd;
					float amount4 = ((instance.ExitLineLen > 0f) ? (num5 / instance.ExitLineLen) : 1f);
					zero = Vector2.Lerp(instance.ExitTurn, instance.Exit, amount4);
					unitX = instance.Exit - instance.ExitTurn;
				}
				float num7;
				if (num2 <= instance.ToTarget)
				{
					float num6 = ((instance.ToTarget > 0f) ? (num2 / instance.ToTarget) : 1f);
					num7 = MathHelper.Lerp(instance.ScaleMin, instance.ScaleMax, num6);
				}
				else
				{
					float num8 = instance.Total - instance.ToTarget;
					float num9 = ((num8 > 0f) ? ((num2 - instance.ToTarget) / num8) : 1f);
					num7 = MathHelper.Lerp(instance.ScaleMax, instance.ScaleMin, num9);
				}
				float num10 = 1f;
				float num11 = ((instance.Total > 0f) ? (num2 / instance.Total) : 1f);
				if (num11 < 0.08f)
				{
					num10 = num11 / 0.08f;
				}
				else if (num11 > 0.92f)
				{
					num10 = (1f - num11) / 0.08f;
				}
				Color white = Color.White;
				Color value = ((Color)(ref white)).WithAlpha(instance.Alpha * num10);
				if (unitX.LengthSquared() < 0.0001f)
				{
					unitX = Vector2.UnitX;
				}
				Matrix3x2 value2 = Matrix3Helpers.CreateRotation((double)(MathF.Atan2(unitX.Y, unitX.X) + MathF.PI / 2f));
				Vector2 vector = new Vector2(num7, num7);
				Matrix3x2 value3 = Matrix3Helpers.CreateScale(ref vector);
				Vector2 value4 = new Vector2(0f - unitX.Y, unitX.X);
				Vector2 vector2 = ((value4.LengthSquared() > 0.0001f) ? Vector2.Normalize(value4) : Vector2.UnitY);
				Vector2 vector3 = new Vector2(val.Width, val.Height) / 32f / 2f;
				for (int i = 0; i < instance.Count; i++)
				{
					float num12 = ((float)i - (float)(instance.Count - 1) / 2f) * instance.Spacing;
					Matrix3x2 value5 = Matrix3Helpers.CreateTranslation(zero + vector2 * num12);
					Matrix3x2 matrix3x = Matrix3x2.Multiply(Matrix3x2.Multiply(value3, value2), value5);
					((DrawingHandleBase)worldHandle).SetTransform(ref matrix3x);
					worldHandle.DrawTextureRect(val, new Box2(-vector3, vector3), (Color?)value);
				}
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}
}
