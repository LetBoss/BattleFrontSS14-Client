using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Screens;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Medicine;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Medical.Healing;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IUserInterfaceManager _ui;

	private readonly MobThresholdSystem _threshold;

	private readonly Font _font;

	private int _offsetY;

	private int _offsetX;

	private const float BarWidth = 300f;

	private const float BarHeight = 20f;

	private const float BarMarginBottom = 40f;

	private const float BarMarginRight = 350f;

	private const float EnergyBarWidth = 300f;

	private const float EnergyBarHeight = 12f;

	private const float EnergyBarSpacing = 6f;

	private static readonly Color LabelColor = Color.White;

	private static readonly Color LabelShadowColor;

	public override OverlaySpace Space => (OverlaySpace)2;

	public PubgHealthOverlay()
	{
		IoCManager.InjectDependencies<PubgHealthOverlay>(this);
		_threshold = _entManager.System<MobThresholdSystem>();
		_cfg.OnValueChanged<int>(CCVars.PubgHealthHudOffsetY, (Action<int>)OnOffsetChanged, true);
		_cfg.OnValueChanged<int>(CCVars.PubgHealthHudOffsetX, (Action<int>)OnOffsetXChanged, true);
		_font = _resourceCache.NotoStack();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!(_ui.ActiveScreen is BattlefrontGameScreen))
		{
			UIBox2i viewportBounds = args.ViewportBounds;
			float barX = (float)((UIBox2i)(ref viewportBounds)).Width - 300f - 350f + (float)_offsetX;
			float barY = (float)((UIBox2i)(ref viewportBounds)).Height - 40f - 20f - (float)_offsetY;
			DrawBars(((OverlayDrawArgs)(ref args)).ScreenHandle, barX, barY, 300f);
		}
	}

	public void DrawBars(DrawingHandleScreen handle, float barX, float barY, float barWidth, float barHeight = 20f, float resourceBarHeight = 12f, bool stackTempBar = false)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		DamageableComponent damageableComponent = default(DamageableComponent);
		MobThresholdsComponent thresholdComponent = default(MobThresholdsComponent);
		if (!localEntity.HasValue || !_entManager.HasComponent<PubgMedicalUserComponent>(localEntity) || !_entManager.TryGetComponent<DamageableComponent>(localEntity, ref damageableComponent) || !_entManager.TryGetComponent<MobThresholdsComponent>(localEntity, ref thresholdComponent) || !_threshold.TryGetThresholdForState(localEntity.Value, MobState.Critical, out var threshold, thresholdComponent))
		{
			return;
		}
		FixedPoint2 value = threshold.Value;
		FixedPoint2 fixedPoint = FixedPoint2.Max(value - damageableComponent.TotalDamage, FixedPoint2.Zero);
		float num = (float)(fixedPoint / value);
		PubgTemporaryHealthComponent pubgTemporaryHealthComponent = default(PubgTemporaryHealthComponent);
		_entManager.TryGetComponent<PubgTemporaryHealthComponent>(localEntity, ref pubgTemporaryHealthComponent);
		float num2 = barY - 6f - resourceBarHeight;
		if (stackTempBar)
		{
			num2 -= 6f + resourceBarHeight;
		}
		CivStaminaComponent civStaminaComponent = default(CivStaminaComponent);
		if (_entManager.TryGetComponent<CivStaminaComponent>(localEntity, ref civStaminaComponent) && civStaminaComponent.MaxStamina > 0f)
		{
			float percent = civStaminaComponent.Stamina / civStaminaComponent.MaxStamina;
			float? thresholdPercent = null;
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (_entManager.TryGetComponent<CivTeamMemberComponent>(localEntity, ref civTeamMemberComponent))
			{
				thresholdPercent = Math.Clamp(civTeamMemberComponent.RunStaminaThresholdRatio, 0f, 1f);
			}
			DrawResourceBar(handle, barX, num2, barWidth, resourceBarHeight, percent, Color.FromHex((ReadOnlySpan<char>)"#F6C445", (Color?)null), thresholdPercent);
			DrawTextWithShadow(handle, new Vector2(barX + 4f, num2 + 2f), "Стамина", 0.75f, LabelColor);
			num2 -= 6f + resourceBarHeight;
		}
		PubgEnergyComponent pubgEnergyComponent = default(PubgEnergyComponent);
		if (_entManager.TryGetComponent<PubgEnergyComponent>(localEntity, ref pubgEnergyComponent) && pubgEnergyComponent.MaxEnergy > 0f)
		{
			float percent2 = pubgEnergyComponent.Energy / pubgEnergyComponent.MaxEnergy;
			DrawResourceBar(handle, barX, num2, barWidth, resourceBarHeight, percent2, Color.FromHex((ReadOnlySpan<char>)"#4FC3F7", (Color?)null));
			DrawTextWithShadow(handle, new Vector2(barX + 4f, num2 + 2f), "Обезболивающее", 0.75f, LabelColor);
		}
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(barX - 2f, barY - 2f, barX + barWidth + 2f, barY + barHeight + 2f);
		handle.DrawRect(val, Color.FromHex((ReadOnlySpan<char>)"#1a1a1a", (Color?)null), true);
		UIBox2 val2 = default(UIBox2);
		((UIBox2)(ref val2))._002Ector(barX - 1f, barY - 1f, barX + barWidth + 1f, barY + barHeight + 1f);
		handle.DrawRect(val2, Color.FromHex((ReadOnlySpan<char>)"#333333", (Color?)null), true);
		UIBox2 val3 = default(UIBox2);
		((UIBox2)(ref val3))._002Ector(barX, barY, barX + barWidth, barY + barHeight);
		handle.DrawRect(val3, Color.FromHex((ReadOnlySpan<char>)"#2d2d2d", (Color?)null), true);
		FixedPoint2 pendingHealAmount = GetPendingHealAmount(localEntity.Value, damageableComponent, value);
		if (pendingHealAmount > FixedPoint2.Zero)
		{
			float num3 = (float)(FixedPoint2.Min(fixedPoint + pendingHealAmount, value) / value);
			float num4 = barWidth * num3;
			UIBox2 val4 = default(UIBox2);
			((UIBox2)(ref val4))._002Ector(barX, barY, barX + num4, barY + barHeight);
			handle.DrawRect(val4, Color.FromHex((ReadOnlySpan<char>)"#FFFFFF55", (Color?)null), true);
		}
		float num5 = barWidth * num;
		UIBox2 val5 = default(UIBox2);
		((UIBox2)(ref val5))._002Ector(barX, barY, barX + num5, barY + barHeight);
		Color val6 = ((num > 0.8f) ? Color.FromHex((ReadOnlySpan<char>)"#4CAF50", (Color?)null) : ((num > 0.5f) ? Color.FromHex((ReadOnlySpan<char>)"#8BC34A", (Color?)null) : ((!(num > 0.25f)) ? Color.FromHex((ReadOnlySpan<char>)"#F44336", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#FF9800", (Color?)null))));
		handle.DrawRect(val5, val6, true);
		if (pubgTemporaryHealthComponent != null && pubgTemporaryHealthComponent.TemporaryHealth > FixedPoint2.Zero)
		{
			float num6;
			float num7;
			float num8;
			float num9;
			if (stackTempBar)
			{
				num6 = barX;
				num7 = barWidth;
				num8 = resourceBarHeight;
				num9 = barY - 6f - resourceBarHeight;
			}
			else
			{
				num6 = barX + barWidth + 4f;
				num7 = barWidth * 0.6f;
				num8 = barHeight;
				num9 = barY;
			}
			float num10 = (float)(pubgTemporaryHealthComponent.TemporaryHealth / pubgTemporaryHealthComponent.MaxTemporaryHealth);
			UIBox2 val7 = default(UIBox2);
			((UIBox2)(ref val7))._002Ector(num6 - 2f, num9 - 2f, num6 + num7 + 2f, num9 + num8 + 2f);
			handle.DrawRect(val7, Color.FromHex((ReadOnlySpan<char>)"#1a1a1a", (Color?)null), true);
			UIBox2 val8 = default(UIBox2);
			((UIBox2)(ref val8))._002Ector(num6 - 1f, num9 - 1f, num6 + num7 + 1f, num9 + num8 + 1f);
			handle.DrawRect(val8, Color.FromHex((ReadOnlySpan<char>)"#333333", (Color?)null), true);
			UIBox2 val9 = default(UIBox2);
			((UIBox2)(ref val9))._002Ector(num6, num9, num6 + num7, num9 + num8);
			handle.DrawRect(val9, Color.FromHex((ReadOnlySpan<char>)"#2d2d2d", (Color?)null), true);
			float num11 = num7 * num10;
			UIBox2 val10 = default(UIBox2);
			((UIBox2)(ref val10))._002Ector(num6, num9, num6 + num11, num9 + num8);
			handle.DrawRect(val10, Color.FromHex((ReadOnlySpan<char>)"#215188", (Color?)null), true);
		}
		FixedPoint2 fixedPoint2 = FixedPoint2.New(20);
		float num12 = (float)((value - fixedPoint2) / value);
		float num13 = barX + barWidth * num12;
		UIBox2 val11 = default(UIBox2);
		((UIBox2)(ref val11))._002Ector(num13 - 1f, barY, num13 + 1f, barY + barHeight);
		handle.DrawRect(val11, Color.FromHex((ReadOnlySpan<char>)"#FFD700AA", (Color?)null), true);
		DrawTextWithShadow(handle, new Vector2(barX + 4f, barY + 4f), "ХП", 1f, LabelColor);
	}

	private void OnOffsetChanged(int offsetY)
	{
		_offsetY = offsetY;
	}

	private void OnOffsetXChanged(int offsetX)
	{
		_offsetX = offsetX;
	}

	private static void DrawResourceBar(DrawingHandleScreen handle, float barX, float barY, float barWidth, float barHeight, float percent, Color fillColor, float? thresholdPercent = null)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		if (percent < 0f)
		{
			percent = 0f;
		}
		if (percent > 1f)
		{
			percent = 1f;
		}
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(barX - 2f, barY - 2f, barX + barWidth + 2f, barY + barHeight + 2f);
		handle.DrawRect(val, Color.FromHex((ReadOnlySpan<char>)"#1a1a1a", (Color?)null), true);
		UIBox2 val2 = default(UIBox2);
		((UIBox2)(ref val2))._002Ector(barX - 1f, barY - 1f, barX + barWidth + 1f, barY + barHeight + 1f);
		handle.DrawRect(val2, Color.FromHex((ReadOnlySpan<char>)"#333333", (Color?)null), true);
		UIBox2 val3 = default(UIBox2);
		((UIBox2)(ref val3))._002Ector(barX, barY, barX + barWidth, barY + barHeight);
		handle.DrawRect(val3, Color.FromHex((ReadOnlySpan<char>)"#2d2d2d", (Color?)null), true);
		float num = barWidth * percent;
		UIBox2 val4 = default(UIBox2);
		((UIBox2)(ref val4))._002Ector(barX, barY, barX + num, barY + barHeight);
		handle.DrawRect(val4, fillColor, true);
		if (thresholdPercent.HasValue)
		{
			float num2 = Math.Clamp(thresholdPercent.Value, 0f, 1f);
			float num3 = barX + barWidth * num2;
			UIBox2 val5 = default(UIBox2);
			((UIBox2)(ref val5))._002Ector(num3 - 1f, barY, num3 + 1f, barY + barHeight);
			handle.DrawRect(val5, Color.FromHex((ReadOnlySpan<char>)"#FFF1A8CC", (Color?)null), true);
		}
	}

	private void DrawTextWithShadow(DrawingHandleScreen handle, Vector2 position, string text, float scale, Color textColor)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		handle.DrawString(_font, position + new Vector2(1f, 1f), (ReadOnlySpan<char>)text, scale, LabelShadowColor);
		handle.DrawString(_font, position, (ReadOnlySpan<char>)text, scale, textColor);
	}

	private FixedPoint2 GetPendingHealAmount(EntityUid player, DamageableComponent damageable, FixedPoint2 maxHp)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DoAfterComponent doAfterComponent = default(DoAfterComponent);
		if (!_entManager.TryGetComponent<DoAfterComponent>(player, ref doAfterComponent))
		{
			return FixedPoint2.Zero;
		}
		PubgMedicalComponent pubgMedicalComponent = default(PubgMedicalComponent);
		HealingComponent healingComponent = default(HealingComponent);
		foreach (Content.Shared.DoAfter.DoAfter value in doAfterComponent.DoAfters.Values)
		{
			if (value.Cancelled || value.Completed)
			{
				continue;
			}
			EntityUid? entity = _entManager.GetEntity(value.Args.NetUsed);
			if (!entity.HasValue)
			{
				continue;
			}
			if (_entManager.TryGetComponent<PubgMedicalComponent>(entity, ref pubgMedicalComponent))
			{
				FixedPoint2 fixedPoint = pubgMedicalComponent.HealAmount;
				if (pubgMedicalComponent.MaxHealThreshold > FixedPoint2.Zero)
				{
					FixedPoint2 fixedPoint2 = damageable.TotalDamage - pubgMedicalComponent.MaxHealThreshold;
					if (fixedPoint2 <= FixedPoint2.Zero)
					{
						return FixedPoint2.Zero;
					}
					fixedPoint = FixedPoint2.Min(fixedPoint, fixedPoint2);
				}
				return fixedPoint;
			}
			if (!_entManager.TryGetComponent<HealingComponent>(entity, ref healingComponent))
			{
				continue;
			}
			FixedPoint2 zero = FixedPoint2.Zero;
			foreach (var (_, fixedPoint4) in healingComponent.Damage.DamageDict)
			{
				if (fixedPoint4 < 0)
				{
					zero -= fixedPoint4;
				}
			}
			return zero;
		}
		return FixedPoint2.Zero;
	}

	static PubgHealthOverlay()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Color black = Color.Black;
		LabelShadowColor = ((Color)(ref black)).WithAlpha(0.85f);
	}
}
