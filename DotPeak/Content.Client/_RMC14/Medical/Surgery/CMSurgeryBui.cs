// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.Surgery.CMSurgeryBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Client.Administration.UI.CustomControls;
using Content.Shared._RMC14.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Medical.Surgery;

public sealed class CMSurgeryBui : BoundUserInterface
{
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPlayerManager _player;
  private readonly CMSurgerySystem _system;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CMSurgeryWindow? _window;
  private EntityUid? _part;
  private (EntityUid Ent, EntProtoId Proto)? _surgery;
  private readonly List<EntProtoId> _previousSurgeries = new List<EntProtoId>();

  public CMSurgeryBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._system = this._entities.System<CMSurgerySystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._system.OnRefresh += (Action) (() =>
    {
      this.UpdateDisabledPanel();
      this.RefreshUI();
    });
    if (!(this.State is CMSurgeryBuiState state))
      return;
    this.Update(state);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is CMSurgeryBuiState state1))
      return;
    this.Update(state1);
  }

  private void Update(CMSurgeryBuiState state)
  {
    if (this._window == null)
    {
      this._window = BoundUserInterfaceExt.CreateWindow<CMSurgeryWindow>((BoundUserInterface) this);
      ((BaseWindow) this._window).OnClose += (Action) (() => this._system.OnRefresh -= new Action(this.RefreshUI));
      this._window.Title = "Surgery";
      ((BaseButton) this._window.PartsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._part = new EntityUid?();
        this._surgery = new (EntityUid, EntProtoId)?();
        this._previousSurgeries.Clear();
        this.View(CMSurgeryBui.ViewType.Parts);
      });
      ((BaseButton) this._window.SurgeriesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._surgery = new (EntityUid, EntProtoId)?();
        this._previousSurgeries.Clear();
        NetEntity? nullable;
        List<EntProtoId> surgeryIds;
        if (!this._entities.TryGetNetEntity(this._part, ref nullable, (MetaDataComponent) null) || !(this.State is CMSurgeryBuiState state2) || !state2.Choices.TryGetValue(nullable.Value, out surgeryIds))
          return;
        this.OnPartPressed(nullable.Value, surgeryIds);
      });
      ((BaseButton) this._window.StepsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        NetEntity? nullable;
        if (!this._entities.TryGetNetEntity(this._part, ref nullable, (MetaDataComponent) null) || this._previousSurgeries.Count == 0)
          return;
        List<EntProtoId> previousSurgeries = this._previousSurgeries;
        EntProtoId entProtoId = previousSurgeries[previousSurgeries.Count - 1];
        this._previousSurgeries.RemoveAt(this._previousSurgeries.Count - 1);
        EntityUid? singleton = this._system.GetSingleton(entProtoId);
        if (!singleton.HasValue)
          return;
        EntityUid valueOrDefault = singleton.GetValueOrDefault();
        CMSurgeryComponent surgeryComponent;
        if (!this._entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault, ref surgeryComponent))
          return;
        this.OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault, surgeryComponent)), nullable.Value, entProtoId);
      });
    }
    ((Control) this._window.Surgeries).DisposeAllChildren();
    ((Control) this._window.Steps).DisposeAllChildren();
    ((Control) this._window.Parts).DisposeAllChildren();
    this.View(CMSurgeryBui.ViewType.Parts);
    (EntityUid Ent, EntProtoId Proto)? surgery = this._surgery;
    EntityUid? part = this._part;
    this._part = new EntityUid?();
    this._surgery = new (EntityUid, EntProtoId)?();
    List<Entity<BodyPartComponent>> entityList = new List<Entity<BodyPartComponent>>(state.Choices.Keys.Count);
    foreach (NetEntity key in state.Choices.Keys)
    {
      EntityUid? nullable;
      BodyPartComponent bodyPartComponent;
      if (this._entities.TryGetEntity(key, ref nullable) && this._entities.TryGetComponent<BodyPartComponent>(nullable, ref bodyPartComponent))
        entityList.Add(Entity<BodyPartComponent>.op_Implicit((nullable.Value, bodyPartComponent)));
    }
    entityList.Sort((Comparison<Entity<BodyPartComponent>>) ((a, b) => GetScore(a) - GetScore(b)));
    foreach (Entity<BodyPartComponent> entity in entityList)
    {
      NetEntity netPart = this._entities.GetNetEntity(entity.Owner, (MetaDataComponent) null);
      List<EntProtoId> surgeries = state.Choices[netPart];
      string entityName = this._entities.GetComponent<MetaDataComponent>(Entity<BodyPartComponent>.op_Implicit(entity)).EntityName;
      XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
      xenoChoiceControl.Set(entityName, (Texture) null);
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnPartPressed(netPart, surgeries));
      ((Control) this._window.Parts).AddChild((Control) xenoChoiceControl);
      foreach (EntProtoId entProtoId in surgeries)
      {
        EntityUid? nullable = this._system.GetSingleton(entProtoId);
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          CMSurgeryComponent surgeryComponent;
          if (this._entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault, ref surgeryComponent))
          {
            nullable = part;
            EntityUid entityUid = Entity<BodyPartComponent>.op_Implicit(entity);
            if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0 && (surgery.HasValue ? (EntProtoId.op_Equality(surgery.GetValueOrDefault().Proto, entProtoId) ? 1 : 0) : 0) != 0)
              this.OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault, surgeryComponent)), netPart, entProtoId);
          }
        }
      }
      EntityUid? nullable1 = part;
      EntityUid entityUid1 = Entity<BodyPartComponent>.op_Implicit(entity);
      if ((nullable1.HasValue ? (EntityUid.op_Equality(nullable1.GetValueOrDefault(), entityUid1) ? 1 : 0) : 0) != 0 && !surgery.HasValue)
        this.OnPartPressed(netPart, surgeries);
    }
    this.RefreshUI();
    this.UpdateDisabledPanel();
    if (((BaseWindow) this._window).IsOpen)
      return;
    ((BaseWindow) this._window).OpenCentered();

    static int GetScore(Entity<BodyPartComponent> part)
    {
      int score;
      switch (part.Comp.PartType)
      {
        case BodyPartType.Other:
          score = 8;
          break;
        case BodyPartType.Torso:
          score = 2;
          break;
        case BodyPartType.Head:
          score = 1;
          break;
        case BodyPartType.Arm:
          score = 3;
          break;
        case BodyPartType.Hand:
          score = 4;
          break;
        case BodyPartType.Leg:
          score = 5;
          break;
        case BodyPartType.Foot:
          score = 6;
          break;
        case BodyPartType.Tail:
          score = 7;
          break;
        default:
          score = 0;
          break;
      }
      return score;
    }
  }

  private void AddStep(EntProtoId stepId, NetEntity netPart, EntProtoId surgeryId)
  {
    if (this._window == null)
      return;
    EntityUid? singleton = this._system.GetSingleton(stepId);
    if (!singleton.HasValue)
      return;
    EntityUid valueOrDefault = singleton.GetValueOrDefault();
    new FormattedMessage().AddText(this._entities.GetComponent<MetaDataComponent>(valueOrDefault).EntityName);
    CMSurgeryStepButton surgeryStepButton = new CMSurgeryStepButton()
    {
      Step = valueOrDefault
    };
    ((BaseButton) surgeryStepButton.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new CMSurgeryStepChosenBuiMsg(netPart, surgeryId, stepId)));
    ((Control) this._window.Steps).AddChild((Control) surgeryStepButton);
  }

  private void OnSurgeryPressed(
    Entity<CMSurgeryComponent> surgery,
    NetEntity netPart,
    EntProtoId surgeryId)
  {
    if (this._window == null)
      return;
    this._part = new EntityUid?(this._entities.GetEntity(netPart));
    this._surgery = new (EntityUid, EntProtoId)?((Entity<CMSurgeryComponent>.op_Implicit(surgery), surgeryId));
    ((Control) this._window.Steps).DisposeAllChildren();
    EntProtoId? requirement1 = surgery.Comp.Requirement;
    if (requirement1.HasValue)
    {
      EntProtoId requirementId = requirement1.GetValueOrDefault();
      EntityUid? singleton = this._system.GetSingleton(requirementId);
      if (singleton.HasValue)
      {
        EntityUid requirement = singleton.GetValueOrDefault();
        XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
        ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          this._previousSurgeries.Add(surgeryId);
          CMSurgeryComponent surgeryComponent;
          if (!this._entities.TryGetComponent<CMSurgeryComponent>(requirement, ref surgeryComponent))
            return;
          this.OnSurgeryPressed(Entity<CMSurgeryComponent>.op_Implicit((requirement, surgeryComponent)), netPart, requirementId);
        });
        FormattedMessage msg = new FormattedMessage();
        string entityName = this._entities.GetComponent<MetaDataComponent>(requirement).EntityName;
        msg.AddMarkupOrThrow($"[bold]Requires: {entityName}[/bold]");
        xenoChoiceControl.Set(msg, (Texture) null);
        ((Control) this._window.Steps).AddChild((Control) xenoChoiceControl);
        BoxContainer steps = this._window.Steps;
        HSeparator hseparator = new HSeparator();
        hseparator.Color = Color.FromHex((ReadOnlySpan<char>) "#4972A1", new Color?());
        hseparator.Margin = new Thickness(0.0f, 0.0f, 0.0f, 1f);
        ((Control) steps).AddChild((Control) hseparator);
      }
    }
    foreach (EntProtoId step in surgery.Comp.Steps)
      this.AddStep(step, netPart, surgeryId);
    this.View(CMSurgeryBui.ViewType.Steps);
    this.RefreshUI();
  }

  private void OnPartPressed(NetEntity netPart, List<EntProtoId> surgeryIds)
  {
    if (this._window == null)
      return;
    this._part = new EntityUid?(this._entities.GetEntity(netPart));
    ((Control) this._window.Surgeries).DisposeAllChildren();
    List<(Entity<CMSurgeryComponent>, EntProtoId, string)> valueTupleList = new List<(Entity<CMSurgeryComponent>, EntProtoId, string)>();
    foreach (EntProtoId surgeryId in surgeryIds)
    {
      EntityUid? singleton = this._system.GetSingleton(surgeryId);
      if (singleton.HasValue)
      {
        EntityUid valueOrDefault = singleton.GetValueOrDefault();
        CMSurgeryComponent surgeryComponent;
        if (this._entities.TryGetComponent<CMSurgeryComponent>(valueOrDefault, ref surgeryComponent))
        {
          string entityName = this._entities.GetComponent<MetaDataComponent>(valueOrDefault).EntityName;
          valueTupleList.Add((Entity<CMSurgeryComponent>.op_Implicit((valueOrDefault, surgeryComponent)), surgeryId, entityName));
        }
      }
    }
    valueTupleList.Sort((Comparison<(Entity<CMSurgeryComponent>, EntProtoId, string)>) ((a, b) =>
    {
      int num = a.Ent.Comp.Priority.CompareTo(b.Ent.Comp.Priority);
      return num != 0 ? num : string.Compare(a.Name, b.Name, StringComparison.Ordinal);
    }));
    foreach ((_, _, _) in valueTupleList)
    {
      XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
      (Entity<CMSurgeryComponent>, EntProtoId, string) surgery;
      xenoChoiceControl.Set(surgery.Item3, (Texture) null);
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OnSurgeryPressed(surgery.Item1, netPart, surgery.Item2));
      ((Control) this._window.Surgeries).AddChild((Control) xenoChoiceControl);
    }
    this.RefreshUI();
    this.View(CMSurgeryBui.ViewType.Surgeries);
  }

  private void RefreshUI()
  {
    if (this._window == null)
      return;
    IEntityManager entities = this._entities;
    ref (EntityUid, EntProtoId)? local = ref this._surgery;
    EntityUid? nullable = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
    BodyPartComponent bodyPartComponent;
    if (!entities.HasComponent<CMSurgeryComponent>(nullable) || !this._entities.TryGetComponent<BodyPartComponent>(this._part, ref bodyPartComponent))
      return;
    (Entity<CMSurgeryComponent> Surgery, int Step)? nextStep = this._system.GetNextStep(this.Owner, this._part.Value, this._surgery.Value.Ent);
    int num = 0;
    foreach (Control child in ((Control) this._window.Steps).Children)
    {
      if (child is CMSurgeryStepButton surgeryStepButton)
      {
        CMSurgeryBui.StepStatus stepStatus = CMSurgeryBui.StepStatus.Incomplete;
        if (!nextStep.HasValue)
          stepStatus = CMSurgeryBui.StepStatus.Complete;
        else if (EntityUid.op_Inequality(nextStep.Value.Surgery.Owner, this._surgery.Value.Ent))
          stepStatus = CMSurgeryBui.StepStatus.Incomplete;
        else if (nextStep.Value.Step == num)
          stepStatus = CMSurgeryBui.StepStatus.Next;
        else if (num < nextStep.Value.Step)
          stepStatus = CMSurgeryBui.StepStatus.Complete;
        ((BaseButton) surgeryStepButton.Button).Disabled = stepStatus != 0;
        FormattedMessage msg = new FormattedMessage();
        msg.AddText(this._entities.GetComponent<MetaDataComponent>(surgeryStepButton.Step).EntityName);
        if (stepStatus == CMSurgeryBui.StepStatus.Complete)
        {
          ((Control) surgeryStepButton.Button).Modulate = Color.Green;
        }
        else
        {
          ((Control) surgeryStepButton.Button).Modulate = Color.White;
          EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
          string popup;
          StepInvalidReason reason;
          if (localEntity.HasValue && !this._system.CanPerformStep(localEntity.GetValueOrDefault(), this.Owner, bodyPartComponent.PartType, surgeryStepButton.Step, false, out popup, out reason, out HashSet<EntityUid> _))
          {
            surgeryStepButton.ToolTip = popup;
            ((BaseButton) surgeryStepButton.Button).Disabled = true;
            switch (reason)
            {
              case StepInvalidReason.MissingSkills:
                msg.AddMarkupOrThrow(" [color=red](Missing surgery skill)[/color]");
                break;
              case StepInvalidReason.NeedsOperatingTable:
                msg.AddMarkupOrThrow(" [color=red](Needs operating table)[/color]");
                break;
              case StepInvalidReason.Armor:
                msg.AddMarkupOrThrow(" [color=red](Remove their armor!)[/color]");
                break;
              case StepInvalidReason.MissingTool:
                msg.AddMarkupOrThrow(" [color=red](Missing tool)[/color]");
                break;
            }
          }
        }
        Texture texture = ((IDirectionalTextureProvider) EntityManagerExt.GetComponentOrNull<SpriteComponent>(this._entities, surgeryStepButton.Step)?.Icon)?.Default;
        surgeryStepButton.Set(msg, texture);
        ++num;
      }
    }
    this.UpdateDisabledPanel();
  }

  private void UpdateDisabledPanel()
  {
    if (this._window == null)
      return;
    if (this._system.IsLyingDown(this.Owner))
    {
      ((Control) this._window.DisabledPanel).Visible = false;
      ((Control) this._window.DisabledPanel).MouseFilter = (Control.MouseFilterMode) 2;
    }
    else
    {
      ((Control) this._window.DisabledPanel).Visible = true;
      FormattedMessage formattedMessage = new FormattedMessage();
      formattedMessage.AddMarkupOrThrow("[color=red][font size=16]They need to be lying down![/font][/color]");
      this._window.DisabledLabel.SetMessage(formattedMessage, new Color?());
      ((Control) this._window.DisabledPanel).MouseFilter = (Control.MouseFilterMode) 0;
    }
  }

  private void View(CMSurgeryBui.ViewType type)
  {
    if (this._window == null)
      return;
    ((Control) this._window.PartsButton).Parent.Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    ((Control) this._window.Parts).Visible = type == CMSurgeryBui.ViewType.Parts;
    ((BaseButton) this._window.PartsButton).Disabled = type == CMSurgeryBui.ViewType.Parts;
    ((Control) this._window.Surgeries).Visible = type == CMSurgeryBui.ViewType.Surgeries;
    ((BaseButton) this._window.SurgeriesButton).Disabled = type != CMSurgeryBui.ViewType.Steps;
    ((Control) this._window.Steps).Visible = type == CMSurgeryBui.ViewType.Steps;
    ((BaseButton) this._window.StepsButton).Disabled = type != CMSurgeryBui.ViewType.Steps || this._previousSurgeries.Count == 0;
    MetaDataComponent metaDataComponent1;
    if (this._entities.TryGetComponent<MetaDataComponent>(this._part, ref metaDataComponent1))
    {
      IEntityManager entities = this._entities;
      ref (EntityUid, EntProtoId)? local1 = ref this._surgery;
      EntityUid? nullable = local1.HasValue ? new EntityUid?(local1.GetValueOrDefault().Item1) : new EntityUid?();
      MetaDataComponent metaDataComponent2;
      ref MetaDataComponent local2 = ref metaDataComponent2;
      if (entities.TryGetComponent<MetaDataComponent>(nullable, ref local2))
      {
        this._window.Title = $"Surgery - {metaDataComponent1.EntityName}, {metaDataComponent2.EntityName}";
        return;
      }
    }
    if (metaDataComponent1 != null)
      this._window.Title = "Surgery - " + metaDataComponent1.EntityName;
    else
      this._window.Title = "Surgery";
  }

  private enum ViewType
  {
    Parts,
    Surgeries,
    Steps,
  }

  private enum StepStatus
  {
    Next,
    Complete,
    Incomplete,
  }
}
