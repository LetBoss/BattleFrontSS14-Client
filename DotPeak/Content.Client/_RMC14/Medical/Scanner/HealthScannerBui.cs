// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.Scanner.HealthScannerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable enable
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
  [Robust.Shared.ViewVariables.ViewVariables]
  private HealthScannerWindow? _window;
  private NetEntity _lastTarget;
  private readonly ShowHolocardIconsSystem _holocardIcons;
  private readonly SkillsSystem _skills;
  private readonly SharedWoundsSystem _wounds;
  private readonly RMCUnrevivableSystem _unrevivable;
  private readonly MobStateSystem _mob;
  private readonly RottingSystem _rot;
  private Dictionary<EntProtoId<SkillDefinitionComponent>, int> BloodPackSkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>()
  {
    [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillSurgery")] = 1
  };
  private Dictionary<EntProtoId<SkillDefinitionComponent>, int> DefibSkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>()
  {
    [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical")] = 2
  };
  private Dictionary<EntProtoId<SkillDefinitionComponent>, int> LarvaSurgerySkill = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>()
  {
    [EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillSurgery")] = 2
  };

  public HealthScannerBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._holocardIcons = this._entities.System<ShowHolocardIconsSystem>();
    this._skills = this._entities.System<SkillsSystem>();
    this._wounds = this._entities.System<SharedWoundsSystem>();
    this._unrevivable = this._entities.System<RMCUnrevivableSystem>();
    this._mob = this._entities.System<MobStateSystem>();
    this._rot = this._entities.System<RottingSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    if (!(this.State is HealthScannerBuiState state))
      return;
    this.UpdateState(state);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is HealthScannerBuiState uiState))
      return;
    this.UpdateState(uiState);
  }

  private void UpdateState(HealthScannerBuiState uiState)
  {
    if (this._window == null)
    {
      this._window = BoundUserInterfaceExt.CreateWindow<HealthScannerWindow>((BoundUserInterface) this);
      this._window.Title = Loc.GetString("rmc-health-analyzer-title");
    }
    EntityUid entity1 = this._entities.GetEntity(uiState.Target);
    if (!((EntityUid) ref entity1).Valid)
      return;
    this._lastTarget = uiState.Target;
    this._window.PatientLabel.Text = Loc.GetString("rmc-health-analyzer-patient", new (string, object)[1]
    {
      ("name", (object) Identity.Name(entity1, this._entities, ((ISharedPlayerManager) this._player).LocalEntity))
    });
    MobThresholdSystem mobThresholdSystem = this._entities.System<MobThresholdSystem>();
    DamageableComponent damageableComponent;
    if (!this._entities.TryGetComponent<DamageableComponent>(entity1, ref damageableComponent))
    {
      if (((BaseWindow) this._window).IsOpen)
        return;
      ((BaseWindow) this._window).OpenCentered();
    }
    else
    {
      Entity<DamageableComponent> entity2 = new Entity<DamageableComponent>(entity1, damageableComponent);
      this.AddGroup(entity2, this._window.BruteLabel, Color.FromHex((ReadOnlySpan<char>) "#DF3E3E", new Color?()), ProtoId<DamageGroupPrototype>.op_Implicit("Brute"), Loc.GetString("rmc-health-analyzer-brute"));
      this.AddGroup(entity2, this._window.BurnLabel, Color.FromHex((ReadOnlySpan<char>) "#FFB833", new Color?()), ProtoId<DamageGroupPrototype>.op_Implicit("Burn"), Loc.GetString("rmc-health-analyzer-burn"));
      this.AddGroup(entity2, this._window.ToxinLabel, Color.FromHex((ReadOnlySpan<char>) "#25CA4C", new Color?()), ProtoId<DamageGroupPrototype>.op_Implicit("Toxin"), Loc.GetString("rmc-health-analyzer-toxin"));
      this.AddGroup(entity2, this._window.OxygenLabel, Color.FromHex((ReadOnlySpan<char>) "#2E93DE", new Color?()), ProtoId<DamageGroupPrototype>.op_Implicit("Airloss"), Loc.GetString("rmc-health-analyzer-oxygen"));
      if (damageableComponent.DamagePerGroup["Genetic"] > 0)
      {
        ((Control) this._window.CloneBox).Visible = true;
        this.AddGroup(entity2, this._window.CloneLabel, Color.FromHex((ReadOnlySpan<char>) "#02c9c0", new Color?()), ProtoId<DamageGroupPrototype>.op_Implicit("Genetic"), Loc.GetString("rmc-health-analyzer-clone"));
      }
      else
        ((Control) this._window.CloneBox).Visible = false;
      bool flag1 = false;
      FixedPoint2? threshold1;
      FixedPoint2? nullable1;
      if (mobThresholdSystem.TryGetIncapThreshold(entity1, out threshold1))
      {
        FixedPoint2 fixedPoint2 = threshold1.Value - damageableComponent.TotalDamage;
        ((Range) this._window.HealthBar).MinValue = 0.0f;
        ((Range) this._window.HealthBar).MaxValue = 100f;
        if (this._mob.IsDead(entity1) && (this._entities.HasComponent<VictimBurstComponent>(entity1) || this._rot.IsRotten(entity1) || this._unrevivable.IsUnrevivable(entity1) || this._entities.HasComponent<RMCDefibrillatorBlockedComponent>(entity1)))
        {
          flag1 = true;
          ((Range) this._window.HealthBar).Value = 100f;
          ((Control) this._window.HealthBar).ModulateSelfOverride = new Color?(Color.Red);
          this._window.HealthBarText.Text = Loc.GetString("rmc-health-analyzer-permadead");
        }
        else
        {
          ((Control) this._window.HealthBar).ModulateSelfOverride = new Color?();
          FixedPoint2? threshold2;
          if (fixedPoint2 < 0 && mobThresholdSystem.TryGetDeadThreshold(entity1, out threshold2))
          {
            FixedPoint2? nullable2 = threshold2;
            FixedPoint2? nullable3 = threshold1;
            if ((nullable2.HasValue == nullable3.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            {
              nullable3 = threshold2;
              nullable1 = threshold1;
              threshold1 = nullable3.HasValue & nullable1.HasValue ? new FixedPoint2?(nullable3.GetValueOrDefault() - nullable1.GetValueOrDefault()) : new FixedPoint2?();
            }
          }
          float num = (float) ((double) fixedPoint2.Float() / (double) threshold1.Value.Float() * 100.0);
          ((Range) this._window.HealthBar).Value = num;
          string str;
          if (!MathHelper.CloseTo(num, 100f, 1E-07f))
            str = $"{num:F2}%";
          else
            str = "100%";
          this._window.HealthBarText.Text = Loc.GetString("rmc-health-analyzer-healthy", new (string, object)[1]
          {
            ("percent", (object) str)
          });
        }
      }
      this._window.ChangeHolocardButton.Text = Loc.GetString("ui-health-scanner-holocard-change");
      ((BaseButton) this._window.ChangeHolocardButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OpenChangeHolocardUI);
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (localEntity.HasValue && this._skills.HasSkill(Entity<SkillsComponent>.op_Implicit(localEntity.GetValueOrDefault()), HolocardSystem.SkillType, 2))
      {
        ((BaseButton) this._window.ChangeHolocardButton).Disabled = false;
        ((Control) this._window.ChangeHolocardButton).ToolTip = "";
      }
      else
      {
        ((BaseButton) this._window.ChangeHolocardButton).Disabled = true;
        ((Control) this._window.ChangeHolocardButton).ToolTip = Loc.GetString("ui-holocard-change-insufficient-skill");
      }
      HolocardStateComponent holocardStateComponent;
      string description;
      Color? holocardColor;
      if (this._entities.TryGetComponent<HolocardStateComponent>(entity1, ref holocardStateComponent) && this._holocardIcons.TryGetDescription(Entity<HolocardStateComponent>.op_Implicit((entity1, holocardStateComponent)), out description) && this._holocardIcons.TryGetHolocardColor(Entity<HolocardStateComponent>.op_Implicit((entity1, holocardStateComponent)), out holocardColor))
      {
        this._window.HolocardDescription.Text = description;
        if (this._window.HolocardPanel.PanelOverride is StyleBoxFlat panelOverride)
          panelOverride.BackgroundColor = holocardColor.Value;
      }
      else
      {
        this._window.HolocardDescription.Text = Loc.GetString("hc-none-description");
        ((Control) this._window.HolocardPanel).ModulateSelfOverride = new Color?();
        if (this._window.HolocardPanel.PanelOverride is StyleBoxFlat panelOverride)
          panelOverride.BackgroundColor = Color.Transparent;
      }
      ((Control) this._window.ChemicalsContainer).DisposeAllChildren();
      bool flag2 = false;
      bool flag3 = false;
      if (uiState.Chemicals != null)
      {
        foreach (ReagentQuantity content in uiState.Chemicals.Contents)
        {
          Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
          if (this._reagent.TryIndex(ProtoId<ReagentPrototype>.op_Implicit(content.Reagent.Prototype), out reagent))
          {
            if (reagent.Unknown)
            {
              flag3 = true;
            }
            else
            {
              string markup = $"{content.Quantity.Float():F1} {reagent.LocalizedName}";
              if (reagent.Overdose.HasValue)
              {
                FixedPoint2 quantity = content.Quantity;
                nullable1 = reagent.Overdose;
                if ((nullable1.HasValue ? (quantity > nullable1.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                  markup = $"[bold][color=red]{FormattedMessage.EscapeText(markup)} OD[/color][/bold]";
              }
              RichTextLabel label = new RichTextLabel();
              label.SetMarkupPermissive(markup);
              ((Control) this._window.ChemicalsContainer).AddChild((Control) label);
              flag2 = true;
            }
          }
        }
      }
      this._window.UnknownReagentsLabel.SetMarkupPermissive(Loc.GetString("rmc-health-analyzer-unknown-reagents"));
      ((Control) this._window.UnknownChemicalsPanel).Visible = flag3;
      ((Control) this._window.ChemicalContentsLabel).Visible = flag2;
      this._window.ChemicalContentsSeparator.Visible = flag2;
      ((Control) this._window.ChemicalsContainer).Visible = flag2;
      this._window.BloodTypeLabel.Text = "Blood:";
      FormattedMessage formattedMessage1 = new FormattedMessage();
      formattedMessage1.PushColor(Color.FromHex((ReadOnlySpan<char>) "#25B732", new Color?()));
      float num1 = uiState.MaxBlood == 0 ? 100f : (float) ((double) uiState.Blood.Float() / (double) uiState.MaxBlood.Float() * 100.0);
      string str1;
      if (!MathHelper.CloseTo(num1, 100f, 1E-07f))
        str1 = $"{num1:F1}%";
      else
        str1 = "100%";
      string str2 = str1;
      formattedMessage1.AddText($"{str2}, {uiState.Blood}cl");
      formattedMessage1.Pop();
      this._window.BloodAmountLabel.SetMessage(formattedMessage1, new Color?());
      if (uiState.Bleeding)
        this._window.Bleeding.SetMarkup(" [bold][color=#DF3E3E]\\[Bleeding\\][/color][/bold]");
      else
        this._window.Bleeding.SetMessage(string.Empty, new Color?());
      FormattedMessage formattedMessage2 = new FormattedMessage();
      float? temperature = uiState.Temperature;
      if (temperature.HasValue)
      {
        float valueOrDefault = temperature.GetValueOrDefault();
        float celsius = TemperatureHelpers.KelvinToCelsius(valueOrDefault);
        float fahrenheit = TemperatureHelpers.KelvinToFahrenheit(valueOrDefault);
        formattedMessage2.AddText($"{celsius:F1}ºC ({fahrenheit:F1}ºF)");
      }
      else
        formattedMessage2.AddText("None");
      this._window.BodyTemperatureLabel.SetMessage(formattedMessage2, new Color?());
      ((Control) this._window.AdviceContainer).DisposeAllChildren();
      if (!flag1)
      {
        ((Control) this._window.MedicalAdviceLabel).Visible = true;
        this._window.MedicalAdviceSeparator.Visible = true;
        this.MedicalAdvice(entity2, uiState, this._window);
        if (((Control) this._window.AdviceContainer).ChildCount == 0)
        {
          ((Control) this._window.MedicalAdviceLabel).Visible = false;
          this._window.MedicalAdviceSeparator.Visible = false;
        }
      }
      else
      {
        ((Control) this._window.MedicalAdviceLabel).Visible = false;
        this._window.MedicalAdviceSeparator.Visible = false;
      }
      if (((BaseWindow) this._window).IsOpen)
        return;
      ((BaseWindow) this._window).OpenCentered();
    }
  }

  private void OpenChangeHolocardUI(BaseButton.ButtonEventArgs obj)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new OpenChangeHolocardUIEvent(this._entities.GetNetEntity(localEntity.GetValueOrDefault(), (MetaDataComponent) null), this._lastTarget));
  }

  private void AddGroup(
    Entity<DamageableComponent> damageable,
    RichTextLabel label,
    Color color,
    ProtoId<DamageGroupPrototype> group,
    string labelStr)
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddText(labelStr + ": ");
    formattedMessage.PushColor(color);
    string str = damageable.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>(ProtoId<DamageGroupPrototype>.op_Implicit(group)).Int().ToString((IFormatProvider) CultureInfo.InvariantCulture);
    if (this._wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit(damageable.Owner), group))
      formattedMessage.AddText($"{{{str}}}");
    else
      formattedMessage.AddText(str ?? "");
    formattedMessage.Pop();
    label.SetMessage(formattedMessage, new Color?());
  }

  private void MedicalAdvice(
    Entity<DamageableComponent> target,
    HealthScannerBuiState uiState,
    HealthScannerWindow window)
  {
    WoundedComponent woundedComponent = (WoundedComponent) null;
    this._entities.TryGetComponent<WoundedComponent>(Entity<DamageableComponent>.op_Implicit(target), ref woundedComponent);
    bool flag1 = false;
    bool flag2 = false;
    if (woundedComponent != null && this._wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent>.op_Implicit(target), woundedComponent)), woundedComponent.BruteWoundGroup))
      flag1 = true;
    if (woundedComponent != null && this._wounds.HasUntreated(Entity<WoundedComponent>.op_Implicit((Entity<DamageableComponent>.op_Implicit(target), woundedComponent)), woundedComponent.BurnWoundGroup))
      flag2 = true;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault1 = localEntity.GetValueOrDefault();
    if (this._mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
    {
      FixedPoint2? threshold;
      if (this._entities.System<MobThresholdSystem>().TryGetDeadThreshold(Entity<DamageableComponent>.op_Implicit(target), out threshold))
      {
        FixedPoint2? nullable1 = threshold;
        FixedPoint2 fixedPoint2_1 = (FixedPoint2) 30;
        FixedPoint2? nullable2 = nullable1.HasValue ? new FixedPoint2?(nullable1.GetValueOrDefault() + fixedPoint2_1) : new FixedPoint2?();
        FixedPoint2 total1 = target.Comp.Damage.GetTotal();
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < total1 ? 1 : 0) : 0) != 0 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMEpinephrine", (List<ReagentData>) null))
        {
          this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-epinedrine"), window);
        }
        else
        {
          string text = string.Empty;
          FixedPoint2? nullable3 = threshold;
          FixedPoint2 fixedPoint2_2 = (FixedPoint2) 20;
          FixedPoint2? nullable4 = nullable3.HasValue ? new FixedPoint2?(nullable3.GetValueOrDefault() - fixedPoint2_2) : new FixedPoint2?();
          FixedPoint2 total2 = target.Comp.Damage.GetTotal();
          if ((nullable4.HasValue ? (nullable4.GetValueOrDefault() <= total2 ? 1 : 0) : 0) != 0 && woundedComponent != null && !flag1 && !flag2)
          {
            text = Loc.GetString("rmc-health-analyzer-advice-defib-repeated");
          }
          else
          {
            FixedPoint2? nullable5 = threshold;
            FixedPoint2 total3 = target.Comp.Damage.GetTotal();
            if ((nullable5.HasValue ? (nullable5.GetValueOrDefault() > total3 ? 1 : 0) : 0) != 0)
              text = Loc.GetString("rmc-health-analyzer-advice-defib");
          }
          if (text != string.Empty && !this._skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault1), this.DefibSkill))
            text = $"[color=#858585]{text}[/color]";
          if (text != string.Empty)
            this.AddAdvice(text, window);
        }
      }
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-cpr"), window);
    }
    HolocardStateComponent holocardStateComponent;
    if (this._entities.TryGetComponent<HolocardStateComponent>(Entity<DamageableComponent>.op_Implicit(target), ref holocardStateComponent) && holocardStateComponent.HolocardStatus == HolocardStatus.Xeno)
    {
      string str1 = Loc.GetString("rmc-health-analyzer-advice-larva-surgery");
      if (!this._skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault1), this.LarvaSurgerySkill))
      {
        string str2 = $"[color=#858585]{str1}[/color]";
      }
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-larva-surgery"), window);
    }
    if (flag1)
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-brute-wounds"), window);
    if (flag2)
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-burn-wounds"), window);
    if (uiState.Blood < uiState.MaxBlood)
    {
      FixedPoint2 fixedPoint2 = uiState.Blood / uiState.MaxBlood;
      if (fixedPoint2 < (FixedPoint2) 0.85)
      {
        string text = Loc.GetString("rmc-health-analyzer-advice-blood-pack");
        if (!this._skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(valueOrDefault1), this.BloodPackSkill))
          text = $"[color=#858585]{text}[/color]";
        this.AddAdvice(text, window);
      }
      if (fixedPoint2 < (FixedPoint2) 0.9 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("Nutriment", (List<ReagentData>) null))
        this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-food"), window);
    }
    FixedPoint2 valueOrDefault2 = target.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>("Airloss");
    FixedPoint2 valueOrDefault3 = target.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>("Brute");
    FixedPoint2 valueOrDefault4 = target.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>("Burn");
    FixedPoint2 valueOrDefault5 = target.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>("Toxin");
    target.Comp.DamagePerGroup.GetValueOrDefault<string, FixedPoint2>("Genetic");
    if (valueOrDefault2 > 0 && !this._mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
    {
      if (valueOrDefault2 > 10 && this._mob.IsCritical(Entity<DamageableComponent>.op_Implicit(target)))
        this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-cpr-crit"), window);
      if (valueOrDefault2 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMDexalin", (List<ReagentData>) null))
        this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-dexalin"), window);
    }
    if (valueOrDefault3 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMBicaridine", (List<ReagentData>) null) && !this._mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-bicaridine"), window);
    if (valueOrDefault4 > 30 && uiState.Chemicals != null && !uiState.Chemicals.ContainsReagent("CMKelotane", (List<ReagentData>) null) && !this._mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
      this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-kelotane"), window);
    if (!(valueOrDefault5 > 10) || uiState.Chemicals == null || uiState.Chemicals.ContainsReagent("CMDylovene", (List<ReagentData>) null) || uiState.Chemicals.ContainsReagent("Inaprovaline", (List<ReagentData>) null) || this._mob.IsDead(Entity<DamageableComponent>.op_Implicit(target)))
      return;
    this.AddAdvice(Loc.GetString("rmc-health-analyzer-advice-dylovene"), window);
  }

  private void AddAdvice(string text, HealthScannerWindow window)
  {
    RichTextLabel label = new RichTextLabel();
    label.SetMarkupPermissive(text);
    ((Control) window.AdviceContainer).AddChild((Control) label);
  }
}
