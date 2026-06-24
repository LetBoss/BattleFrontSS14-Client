using System;
using System.Collections.Generic;
using System.Globalization;
using Content.Client._RMC14.Medical.HUD;
using Content.Client.Atmos.Rotting;
using Content.Client.Message;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Defibrillator;
using Content.Shared._RMC14.Medical.HUD;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.HUD.Systems;
using Content.Shared._RMC14.Medical.Scanner;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Medical.Wounds;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Systems;
using Content.Shared.Temperature;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Medical.Scanner;

public sealed class HealthScannerBui : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private RMCReagentSystem _reagent;

	[ViewVariables]
	private HealthScannerWindow? _window;

	private NetEntity _lastTarget;

	private readonly ShowHolocardIconsSystem _holocardIcons;

	private readonly SkillsSystem _skills;

	private readonly SharedWoundsSystem _wounds;

	private readonly RMCUnrevivableSystem _unrevivable;

	private readonly MobStateSystem _mob;

	private readonly RottingSystem _rot;

	private Dictionary<EntProtoId<SkillDefinitionComponent>, int> BloodPackSkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int> { [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillSurgery")] = 1 };

	private Dictionary<EntProtoId<SkillDefinitionComponent>, int> DefibSkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int> { [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical")] = 2 };

	private Dictionary<EntProtoId<SkillDefinitionComponent>, int> LarvaSurgerySkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int> { [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillSurgery")] = 2 };

	public HealthScannerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		_holocardIcons = _entities.System<ShowHolocardIconsSystem>();
		_skills = _entities.System<SkillsSystem>();
		_wounds = _entities.System<SharedWoundsSystem>();
		_unrevivable = _entities.System<RMCUnrevivableSystem>();
		_mob = _entities.System<MobStateSystem>();
		_rot = _entities.System<RottingSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		if (((BoundUserInterface)this).State is HealthScannerBuiState uiState)
		{
			UpdateState(uiState);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is HealthScannerBuiState uiState)
		{
			UpdateState(uiState);
		}
	}

	private void UpdateState(HealthScannerBuiState uiState)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Expected O, but got Unknown
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Expected O, but got Unknown
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Expected O, but got Unknown
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			_window = BoundUserInterfaceExt.CreateWindow<HealthScannerWindow>((BoundUserInterface)(object)this);
			((DefaultWindow)_window).Title = Loc.GetString("rmc-health-analyzer-title");
		}
		EntityUid entity = _entities.GetEntity(uiState.Target);
		if (!((EntityUid)(ref entity)).Valid)
		{
			return;
		}
		_lastTarget = uiState.Target;
		_window.PatientLabel.Text = Loc.GetString("rmc-health-analyzer-patient", new(string, object)[1] { ("name", Identity.Name(entity, _entities, ((ISharedPlayerManager)_player).LocalEntity)) });
		MobThresholdSystem mobThresholdSystem = _entities.System<MobThresholdSystem>();
		DamageableComponent damageableComponent = default(DamageableComponent);
		if (!_entities.TryGetComponent<DamageableComponent>(entity, ref damageableComponent))
		{
			if (!((BaseWindow)_window).IsOpen)
			{
				((BaseWindow)_window).OpenCentered();
			}
			return;
		}
		Entity<DamageableComponent> val = default(Entity<DamageableComponent>);
		val._002Ector(entity, damageableComponent);
		AddGroup(val, _window.BruteLabel, Color.FromHex((ReadOnlySpan<char>)"#DF3E3E", (Color?)null), ProtoId<DamageGroupPrototype>.op_Implicit("Brute"), Loc.GetString("rmc-health-analyzer-brute"));
		AddGroup(val, _window.BurnLabel, Color.FromHex((ReadOnlySpan<char>)"#FFB833", (Color?)null), ProtoId<DamageGroupPrototype>.op_Implicit("Burn"), Loc.GetString("rmc-health-analyzer-burn"));
		AddGroup(val, _window.ToxinLabel, Color.FromHex((ReadOnlySpan<char>)"#25CA4C", (Color?)null), ProtoId<DamageGroupPrototype>.op_Implicit("Toxin"), Loc.GetString("rmc-health-analyzer-toxin"));
		AddGroup(val, _window.OxygenLabel, Color.FromHex((ReadOnlySpan<char>)"#2E93DE", (Color?)null), ProtoId<DamageGroupPrototype>.op_Implicit("Airloss"), Loc.GetString("rmc-health-analyzer-oxygen"));
		if (damageableComponent.DamagePerGroup["Genetic"] > 0)
		{
			((Control)_window.CloneBox).Visible = true;
			AddGroup(val, _window.CloneLabel, Color.FromHex((ReadOnlySpan<char>)"#02c9c0", (Color?)null), ProtoId<DamageGroupPrototype>.op_Implicit("Genetic"), Loc.GetString("rmc-health-analyzer-clone"));
		}
		else
		{
			((Control)_window.CloneBox).Visible = false;
		}
		bool flag = false;
		if (mobThresholdSystem.TryGetIncapThreshold(entity, out var threshold))
		{
			FixedPoint2 fixedPoint = threshold.Value - damageableComponent.TotalDamage;
			((Range)_window.HealthBar).MinValue = 0f;
			((Range)_window.HealthBar).MaxValue = 100f;
			if (_mob.IsDead(entity) && (_entities.HasComponent<VictimBurstComponent>(entity) || _rot.IsRotten(entity) || _unrevivable.IsUnrevivable(entity) || _entities.HasComponent<RMCDefibrillatorBlockedComponent>(entity)))
			{
				flag = true;
				((Range)_window.HealthBar).Value = 100f;
				((Control)_window.HealthBar).ModulateSelfOverride = Color.Red;
				_window.HealthBarText.Text = Loc.GetString("rmc-health-analyzer-permadead");
			}
			else
			{
				((Control)_window.HealthBar).ModulateSelfOverride = null;
				if (fixedPoint < 0 && mobThresholdSystem.TryGetDeadThreshold(entity, out var threshold2) && threshold2 != threshold)
				{
					threshold = threshold2 - threshold;
				}
				float num = fixedPoint.Float() / threshold.Value.Float() * 100f;
				((Range)_window.HealthBar).Value = num;
				string item = (MathHelper.CloseTo(num, 100f, 1E-07f) ? "100%" : $"{num:F2}%");
				_window.HealthBarText.Text = Loc.GetString("rmc-health-analyzer-healthy", new(string, object)[1] { ("percent", item) });
			}
		}
		_window.ChangeHolocardButton.Text = Loc.GetString("ui-health-scanner-holocard-change");
		((BaseButton)_window.ChangeHolocardButton).OnPressed += OpenChangeHolocardUI;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(valueOrDefault), HolocardSystem.SkillType, 2))
			{
				((BaseButton)_window.ChangeHolocardButton).Disabled = false;
				((Control)_window.ChangeHolocardButton).ToolTip = "";
				goto IL_055a;
			}
		}
		((BaseButton)_window.ChangeHolocardButton).Disabled = true;
		((Control)_window.ChangeHolocardButton).ToolTip = Loc.GetString("ui-holocard-change-insufficient-skill");
		goto IL_055a;
		IL_055a:
		HolocardStateComponent item2 = default(HolocardStateComponent);
		if (_entities.TryGetComponent<HolocardStateComponent>(entity, ref item2) && _holocardIcons.TryGetDescription(Entity<HolocardStateComponent>.op_Implicit((entity, item2)), out string description) && _holocardIcons.TryGetHolocardColor(Entity<HolocardStateComponent>.op_Implicit((entity, item2)), out var holocardColor))
		{
			_window.HolocardDescription.Text = description;
			StyleBox panelOverride = _window.HolocardPanel.PanelOverride;
			StyleBoxFlat val2 = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
			if (val2 != null)
			{
				val2.BackgroundColor = holocardColor.Value;
			}
		}
		else
		{
			_window.HolocardDescription.Text = Loc.GetString("hc-none-description");
			((Control)_window.HolocardPanel).ModulateSelfOverride = null;
			StyleBox panelOverride2 = _window.HolocardPanel.PanelOverride;
			StyleBoxFlat val3 = (StyleBoxFlat)(object)((panelOverride2 is StyleBoxFlat) ? panelOverride2 : null);
			if (val3 != null)
			{
				val3.BackgroundColor = Color.Transparent;
			}
		}
		((Control)_window.ChemicalsContainer).DisposeAllChildren();
		bool visible = false;
		bool visible2 = false;
		if (uiState.Chemicals != null)
		{
			foreach (ReagentQuantity content in uiState.Chemicals.Contents)
			{
				if (!_reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), out Reagent reagent))
				{
					continue;
				}
				if (reagent.Unknown)
				{
					visible2 = true;
					continue;
				}
				string text = $"{content.Quantity.Float():F1} {reagent.LocalizedName}";
				if (reagent.Overdose.HasValue)
				{
					FixedPoint2 quantity = content.Quantity;
					FixedPoint2? overdose = reagent.Overdose;
					if (quantity > overdose)
					{
						text = "[bold][color=red]" + FormattedMessage.EscapeText(text) + " OD[/color][/bold]";
					}
				}
				RichTextLabel val4 = new RichTextLabel();
				val4.SetMarkupPermissive(text);
				((Control)_window.ChemicalsContainer).AddChild((Control)(object)val4);
				visible = true;
			}
		}
		_window.UnknownReagentsLabel.SetMarkupPermissive(Loc.GetString("rmc-health-analyzer-unknown-reagents"));
		((Control)_window.UnknownChemicalsPanel).Visible = visible2;
		((Control)_window.ChemicalContentsLabel).Visible = visible;
		((Control)_window.ChemicalContentsSeparator).Visible = visible;
		((Control)_window.ChemicalsContainer).Visible = visible;
		_window.BloodTypeLabel.Text = "Blood:";
		FormattedMessage val5 = new FormattedMessage();
		val5.PushColor(Color.FromHex((ReadOnlySpan<char>)"#25B732", (Color?)null));
		float num2 = ((uiState.MaxBlood == 0) ? 100f : (uiState.Blood.Float() / uiState.MaxBlood.Float() * 100f));
		string value = (MathHelper.CloseTo(num2, 100f, 1E-07f) ? "100%" : $"{num2:F1}%");
		val5.AddText($"{value}, {uiState.Blood}cl");
		val5.Pop();
		_window.BloodAmountLabel.SetMessage(val5, (Color?)null);
		if (uiState.Bleeding)
		{
			_window.Bleeding.SetMarkup(" [bold][color=#DF3E3E]\\[Bleeding\\][/color][/bold]");
		}
		else
		{
			_window.Bleeding.SetMessage(string.Empty, (Color?)null);
		}
		FormattedMessage val6 = new FormattedMessage();
		float? temperature = uiState.Temperature;
		if (temperature.HasValue)
		{
			float valueOrDefault2 = temperature.GetValueOrDefault();
			float value2 = TemperatureHelpers.KelvinToCelsius(valueOrDefault2);
			float value3 = TemperatureHelpers.KelvinToFahrenheit(valueOrDefault2);
			val6.AddText($"{value2:F1}ºC ({value3:F1}ºF)");
		}
		else
		{
			val6.AddText("None");
		}
		_window.BodyTemperatureLabel.SetMessage(val6, (Color?)null);
		((Control)_window.AdviceContainer).DisposeAllChildren();
		if (!flag)
		{
			((Control)_window.MedicalAdviceLabel).Visible = true;
			((Control)_window.MedicalAdviceSeparator).Visible = true;
			MedicalAdvice(val, uiState, _window);
			if (((Control)_window.AdviceContainer).ChildCount == 0)
			{
				((Control)_window.MedicalAdviceLabel).Visible = false;
				((Control)_window.MedicalAdviceSeparator).Visible = false;
			}
		}
		else
		{
			((Control)_window.MedicalAdviceLabel).Visible = false;
			((Control)_window.MedicalAdviceSeparator).Visible = false;
		}
		if (!((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).OpenCentered();
		}
	}

	private void OpenChangeHolocardUI(ButtonEventArgs obj)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new OpenChangeHolocardUIEvent(_entities.GetNetEntity(valueOrDefault, (MetaDataComponent)null), _lastTarget));
		}
	}

	private void AddGroup(Entity<DamageableComponent> damageable, RichTextLabel label, Color color, ProtoId<DamageGroupPrototype> group, string labelStr)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage val = new FormattedMessage();
		val.AddText(labelStr + ": ");
		val.PushColor(color);
		string text = damageable.Comp.DamagePerGroup.GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(group)).Int().ToString(CultureInfo.InvariantCulture);
		if (_wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit(damageable.Owner), group))
		{
			val.AddText("{" + text + "}");
		}
		else
		{
			val.AddText(text ?? "");
		}
		val.Pop();
		label.SetMessage(val, (Color?)null);
	}

	private void MedicalAdvice(Entity<DamageableComponent> target, HealthScannerBuiState uiState, HealthScannerWindow window)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		WoundedComponent woundedComponent = null;
		_entities.TryGetComponent<WoundedComponent>(Entity<DamageableComponent>.op_Implicit(target), ref woundedComponent);
		bool flag = false;
		bool flag2 = false;
		if (woundedComponent != null && _wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent>.op_Implicit(target), woundedComponent)), woundedComponent.BruteWoundGroup))
		{
			flag = true;
		}
		if (woundedComponent != null && _wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent>.op_Implicit(target), woundedComponent)), woundedComponent.BurnWoundGroup))
		{
			flag2 = true;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (_mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
		{
			if (_entities.System<MobThresholdSystem>().TryGetDeadThreshold(Entity<DamageableComponent>.op_Implicit(target), out var threshold))
			{
				FixedPoint2? fixedPoint = threshold + 30;
				FixedPoint2 total = target.Comp.Damage.GetTotal();
				if (fixedPoint.HasValue && fixedPoint.GetValueOrDefault() < total && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMEpinephrine", null))
				{
					AddAdvice(Loc.GetString("rmc-health-analyzer-advice-epinedrine"), window);
				}
				else
				{
					string text = string.Empty;
					fixedPoint = threshold - 20;
					total = target.Comp.Damage.GetTotal();
					if (fixedPoint.HasValue && fixedPoint.GetValueOrDefault() <= total && woundedComponent != null && !flag && !flag2)
					{
						text = Loc.GetString("rmc-health-analyzer-advice-defib-repeated");
					}
					else if (threshold > target.Comp.Damage.GetTotal())
					{
						text = Loc.GetString("rmc-health-analyzer-advice-defib");
					}
					if (text != string.Empty && !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault), DefibSkill))
					{
						text = "[color=#858585]" + text + "[/color]";
					}
					if (text != string.Empty)
					{
						AddAdvice(text, window);
					}
				}
			}
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-cpr"), window);
		}
		HolocardStateComponent holocardStateComponent = default(HolocardStateComponent);
		if (_entities.TryGetComponent<HolocardStateComponent>(Entity<DamageableComponent>.op_Implicit(target), ref holocardStateComponent) && holocardStateComponent.HolocardStatus == HolocardStatus.Xeno)
		{
			string text2 = Loc.GetString("rmc-health-analyzer-advice-larva-surgery");
			if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault), LarvaSurgerySkill))
			{
				text2 = "[color=#858585]" + text2 + "[/color]";
			}
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-larva-surgery"), window);
		}
		if (flag)
		{
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-brute-wounds"), window);
		}
		if (flag2)
		{
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-burn-wounds"), window);
		}
		if (uiState.Blood < uiState.MaxBlood)
		{
			FixedPoint2 fixedPoint2 = uiState.Blood / uiState.MaxBlood;
			if (fixedPoint2 < 0.85)
			{
				string text3 = Loc.GetString("rmc-health-analyzer-advice-blood-pack");
				if (!_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault), BloodPackSkill))
				{
					text3 = "[color=#858585]" + text3 + "[/color]";
				}
				AddAdvice(text3, window);
			}
			if (fixedPoint2 < 0.9 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("Nutriment", null))
			{
				AddAdvice(Loc.GetString("rmc-health-analyzer-advice-food"), window);
			}
		}
		FixedPoint2 valueOrDefault2 = target.Comp.DamagePerGroup.GetValueOrDefault("Airloss");
		FixedPoint2 valueOrDefault3 = target.Comp.DamagePerGroup.GetValueOrDefault("Brute");
		FixedPoint2 valueOrDefault4 = target.Comp.DamagePerGroup.GetValueOrDefault("Burn");
		FixedPoint2 valueOrDefault5 = target.Comp.DamagePerGroup.GetValueOrDefault("Toxin");
		target.Comp.DamagePerGroup.GetValueOrDefault("Genetic");
		if (valueOrDefault2 > 0 && !_mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
		{
			if (valueOrDefault2 > 10 && _mob.IsCritical(Entity<DamageableComponent>.op_Implicit(target)))
			{
				AddAdvice(Loc.GetString("rmc-health-analyzer-advice-cpr-crit"), window);
			}
			if (valueOrDefault2 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMDexalin", null))
			{
				AddAdvice(Loc.GetString("rmc-health-analyzer-advice-dexalin"), window);
			}
		}
		if (valueOrDefault3 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMBicaridine", null) && !_mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
		{
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-bicaridine"), window);
		}
		if (valueOrDefault4 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMKelotane", null) && !_mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
		{
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-kelotane"), window);
		}
		if (valueOrDefault5 > 10 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMDylovene", null) && !uiState.Chemicals.ContainsReagent("Inaprovaline", null) && !_mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
		{
			AddAdvice(Loc.GetString("rmc-health-analyzer-advice-dylovene"), window);
		}
	}

	private void AddAdvice(string text, HealthScannerWindow window)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		RichTextLabel val = new RichTextLabel();
		val.SetMarkupPermissive(text);
		((Control)window.AdviceContainer).AddChild((Control)(object)val);
	}
}
