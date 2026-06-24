using System;
using System.Numerics;
using Content.Shared.Mobs;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.DamageOverlays.Overlays;

public sealed class DamageOverlay : Overlay
{
	private static readonly ProtoId<ShaderPrototype> CircleMaskShader = ProtoId<ShaderPrototype>.op_Implicit("GradientCircleMask");

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPlayerManager _playerManager;

	private readonly ShaderInstance _critShader;

	private readonly ShaderInstance _oxygenShader;

	private readonly ShaderInstance _bruteShader;

	public MobState State = MobState.Alive;

	public float PainLevel;

	private float _oldPainLevel;

	public float OxygenLevel;

	private float _oldOxygenLevel;

	public float CritLevel;

	private float _oldCritLevel;

	public float DeadLevel = 1f;

	public override OverlaySpace Space => (OverlaySpace)4;

	public DamageOverlay()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DamageOverlay>(this);
		_oxygenShader = _prototypeManager.Index<ShaderPrototype>(CircleMaskShader).InstanceUnique();
		_critShader = _prototypeManager.Index<ShaderPrototype>(CircleMaskShader).InstanceUnique();
		_bruteShader = _prototypeManager.Index<ShaderPrototype>(CircleMaskShader).InstanceUnique();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent val = default(EyeComponent);
		if (!_entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager)_playerManager).LocalEntity, ref val) || (object)args.Viewport.Eye != val.Eye)
		{
			return;
		}
		Box2 worldAABB = args.WorldAABB;
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		int width = ((UIBox2i)(ref args.ViewportBounds)).Width;
		float num = (float)_timing.RealTime.TotalSeconds;
		float lastFrameTime = (float)_timing.FrameTime.TotalSeconds;
		if (State != MobState.Dead)
		{
			DeadLevel = 1f;
		}
		else if (!MathHelper.CloseTo(0f, DeadLevel, 0.001f))
		{
			float value = 0f - DeadLevel;
			DeadLevel += GetDiff(value, lastFrameTime);
		}
		else
		{
			DeadLevel = 0f;
		}
		if (!MathHelper.CloseTo(_oldPainLevel, PainLevel, 0.001f))
		{
			float value2 = PainLevel - _oldPainLevel;
			_oldPainLevel += GetDiff(value2, lastFrameTime);
		}
		else
		{
			_oldPainLevel = PainLevel;
		}
		if (!MathHelper.CloseTo(_oldOxygenLevel, OxygenLevel, 0.001f))
		{
			float value3 = OxygenLevel - _oldOxygenLevel;
			_oldOxygenLevel += GetDiff(value3, lastFrameTime);
		}
		else
		{
			_oldOxygenLevel = OxygenLevel;
		}
		if (!MathHelper.CloseTo(_oldCritLevel, CritLevel, 0.001f))
		{
			float value4 = CritLevel - _oldCritLevel;
			_oldCritLevel += GetDiff(value4, lastFrameTime);
		}
		else
		{
			_oldCritLevel = CritLevel;
		}
		float num2 = 0f;
		num2 = _oldPainLevel;
		if (num2 > 0f && _oldCritLevel <= 0f)
		{
			float num3 = 3f;
			float x = num * num3;
			float num4 = 2f * (float)width;
			float num5 = 0.8f * (float)width;
			float num6 = 0.6f * (float)width;
			float num7 = 0.2f * (float)width;
			float num8 = num4 - num2 * (num4 - num5);
			float num9 = num6 - num2 * (num6 - num7);
			float num10 = MathF.Max(0f, MathF.Sin(x));
			_bruteShader.SetParameter("time", num10);
			_bruteShader.SetParameter("color", new Vector3(1f, 0f, 0f));
			_bruteShader.SetParameter("darknessAlphaOuter", 0.8f);
			_bruteShader.SetParameter("outerCircleRadius", num8);
			_bruteShader.SetParameter("outerCircleMaxRadius", num8 + 0.2f * (float)width);
			_bruteShader.SetParameter("innerCircleRadius", num9);
			_bruteShader.SetParameter("innerCircleMaxRadius", num9 + 0.02f * (float)width);
			((DrawingHandleBase)worldHandle).UseShader(_bruteShader);
			worldHandle.DrawRect(worldAABB, Color.White, true);
		}
		else
		{
			_oldPainLevel = PainLevel;
		}
		num2 = ((State != MobState.Critical) ? _oldOxygenLevel : 1f);
		if (num2 > 0f)
		{
			float num11 = 0.6f * (float)width;
			float num12 = 0.06f * (float)width;
			float num13 = 0.02f * (float)width;
			float num14 = 0.02f * (float)width;
			float num15 = num11 - num2 * (num11 - num12);
			float num16 = num13 - num2 * (num13 - num14);
			float num19;
			if (_oldCritLevel > 0f)
			{
				float num17 = num * 2f;
				float num18 = MathF.Max(0f, MathF.Sin(num17) + 2f * MathF.Sin(2f * num17 / 4f) + MathF.Sin(num17 / 4f) - 3f);
				num19 = ((!(num18 > 0f)) ? 1f : (1f - num18 / 1.5f));
			}
			else
			{
				num19 = MathF.Min(0.98f, 0.3f * MathF.Log(num2) + 1f);
			}
			_oxygenShader.SetParameter("time", 0f);
			_oxygenShader.SetParameter("color", new Vector3(0f, 0f, 0f));
			_oxygenShader.SetParameter("darknessAlphaOuter", num19);
			_oxygenShader.SetParameter("innerCircleRadius", num16);
			_oxygenShader.SetParameter("innerCircleMaxRadius", num16);
			_oxygenShader.SetParameter("outerCircleRadius", num15);
			_oxygenShader.SetParameter("outerCircleMaxRadius", num15 + 0.2f * (float)width);
			((DrawingHandleBase)worldHandle).UseShader(_oxygenShader);
			worldHandle.DrawRect(worldAABB, Color.White, true);
		}
		num2 = ((State != MobState.Dead) ? _oldCritLevel : DeadLevel);
		if (num2 > 0f)
		{
			float num20 = 2f * (float)width;
			float num21 = 1f * (float)width;
			float num22 = 0.6f * (float)width;
			float num23 = 0.02f * (float)width;
			float num24 = num20 - num2 * (num20 - num21);
			float num25 = num22 - num2 * (num22 - num23);
			float num26 = MathF.Max(0f, MathF.Sin(num));
			_critShader.SetParameter("time", num26);
			_critShader.SetParameter("color", new Vector3(1f, 1f, 1f));
			_critShader.SetParameter("darknessAlphaOuter", 1f);
			_critShader.SetParameter("innerCircleRadius", num25);
			_critShader.SetParameter("innerCircleMaxRadius", num25 + 0.005f * (float)width);
			_critShader.SetParameter("outerCircleRadius", num24);
			_critShader.SetParameter("outerCircleMaxRadius", num24 + 0.2f * (float)width);
			((DrawingHandleBase)worldHandle).UseShader(_critShader);
			worldHandle.DrawRect(worldAABB, Color.White, true);
		}
		((DrawingHandleBase)worldHandle).UseShader((ShaderInstance)null);
	}

	private float GetDiff(float value, float lastFrameTime)
	{
		float value2 = value * 5f * lastFrameTime;
		if (value < 0f)
		{
			return Math.Clamp(value2, value, 0f - value);
		}
		return Math.Clamp(value2, 0f - value, value);
	}
}
