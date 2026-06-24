// Decompiled with JetBrains decompiler
// Type: Content.Client.LateJoin.LateJoinGui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CrewManifest;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.CCVar;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.LateJoin;

public sealed class LateJoinGui : DefaultWindow
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IClientConsoleHost _consoleHost;
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IEntitySystemManager _entitySystem;
  [Dependency]
  private JobRequirementsManager _jobRequirements;
  [Dependency]
  private IClientPreferencesManager _preferencesManager;
  [Dependency]
  private ILogManager _logManager;
  private readonly ClientGameTicker _gameTicker;
  private readonly SpriteSystem _sprites;
  private readonly CrewManifestSystem _crewManifest;
  private readonly ISawmill _sawmill;
  private readonly Dictionary<NetEntity, Dictionary<string, List<JobButton>>> _jobButtons = new Dictionary<NetEntity, Dictionary<string, List<JobButton>>>();
  private readonly Dictionary<NetEntity, Dictionary<string, BoxContainer>> _jobCategories = new Dictionary<NetEntity, Dictionary<string, BoxContainer>>();
  private readonly List<ScrollContainer> _jobLists = new List<ScrollContainer>();
  private readonly Control _base;

  public event Action<(NetEntity, string)> SelectedId;

  public LateJoinGui()
  {
    Vector2 vector2 = new Vector2(360f, 560f);
    ((Control) this).SetSize = vector2;
    ((Control) this).MinSize = vector2;
    IoCManager.InjectDependencies<LateJoinGui>(this);
    this._sprites = this._entitySystem.GetEntitySystem<SpriteSystem>();
    this._crewManifest = this._entitySystem.GetEntitySystem<CrewManifestSystem>();
    this._gameTicker = this._entitySystem.GetEntitySystem<ClientGameTicker>();
    this._sawmill = this._logManager.GetSawmill("latejoin.panel");
    this.Title = Loc.GetString("late-join-gui-title");
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).VerticalExpand = true;
    this._base = (Control) boxContainer;
    this.Contents.AddChild(this._base);
    this._jobRequirements.Updated += new Action(this.RebuildUI);
    this.RebuildUI();
    this.SelectedId += (Action<(NetEntity, string)>) (x =>
    {
      (NetEntity netEntity2, string str2) = x;
      this._sawmill.Info("Late joining as ID: " + str2);
      ((IConsoleHost) this._consoleHost).ExecuteCommand($"joingame {CommandParsing.Escape(str2)} {netEntity2}");
      ((BaseWindow) this).Close();
    });
    this._gameTicker.LobbyJobsAvailableUpdated += new Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>(this.JobsAvailableUpdated);
  }

  private void RebuildUI()
  {
    this._base.RemoveAllChildren();
    this._jobLists.Clear();
    this._jobButtons.Clear();
    this._jobCategories.Clear();
    if (!this._gameTicker.DisallowedLateJoin && this._gameTicker.StationNames.Count == 0)
      this._sawmill.Warning("No stations exist, nothing to display in late-join GUI");
    foreach (KeyValuePair<NetEntity, string> stationName in (IEnumerable<KeyValuePair<NetEntity, string>>) this._gameTicker.StationNames)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LateJoinGui.\u003C\u003Ec__DisplayClass19_0 cDisplayClass190 = new LateJoinGui.\u003C\u003Ec__DisplayClass19_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass190.\u003C\u003E4__this = this;
      string str1;
      // ISSUE: reference to a compiler-generated field
      (cDisplayClass190.id, str1) = stationName;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LateJoinGui.\u003C\u003Ec__DisplayClass19_1 cDisplayClass191 = new LateJoinGui.\u003C\u003Ec__DisplayClass19_1();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass191.CS\u0024\u003C\u003E8__locals1 = cDisplayClass190;
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer1).Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
      BoxContainer boxContainer2 = boxContainer1;
      ContainerButton containerButton1 = new ContainerButton();
      ((Control) containerButton1).HorizontalAlignment = (Control.HAlignment) 3;
      ((BaseButton) containerButton1).ToggleMode = true;
      Control.OrderedChildCollection children1 = ((Control) containerButton1).Children;
      TextureRect textureRect1 = new TextureRect();
      ((Control) textureRect1).StyleClasses.Add("optionTriangle");
      ((Control) textureRect1).Margin = new Thickness(8f, 0.0f);
      ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
      children1.Add((Control) textureRect1);
      ContainerButton containerButton2 = containerButton1;
      Control control = this._base;
      StripeBack stripeBack = new StripeBack();
      Control.OrderedChildCollection children2 = ((Control) stripeBack).Children;
      PanelContainer panelContainer1 = new PanelContainer();
      Control.OrderedChildCollection children3 = ((Control) panelContainer1).Children;
      Label label1 = new Label();
      ((Control) label1).StyleClasses.Add("LabelBig");
      label1.Text = str1;
      label1.Align = (Label.AlignMode) 1;
      children3.Add((Control) label1);
      ((Control) panelContainer1).Children.Add((Control) containerButton2);
      children2.Add((Control) panelContainer1);
      control.AddChild((Control) stripeBack);
      if (this._configManager.GetCVar<bool>(CCVars.CrewManifestWithoutEntity))
      {
        Button button = new Button()
        {
          Text = Loc.GetString("crew-manifest-button-label")
        };
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated method
        ((BaseButton) button).OnPressed += new Action<BaseButton.ButtonEventArgs>(cDisplayClass191.CS\u0024\u003C\u003E8__locals1.\u003CRebuildUI\u003Eb__1);
        this._base.AddChild((Control) button);
      }
      ScrollContainer scrollContainer = new ScrollContainer();
      ((Control) scrollContainer).VerticalExpand = true;
      ((Control) scrollContainer).Children.Add((Control) boxContainer2);
      ((Control) scrollContainer).Visible = false;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass191.jobListScroll = scrollContainer;
      if (this._jobLists.Count == 0)
      {
        // ISSUE: reference to a compiler-generated field
        ((Control) cDisplayClass191.jobListScroll).Visible = true;
      }
      // ISSUE: reference to a compiler-generated field
      this._jobLists.Add(cDisplayClass191.jobListScroll);
      // ISSUE: reference to a compiler-generated field
      this._base.AddChild((Control) cDisplayClass191.jobListScroll);
      // ISSUE: reference to a compiler-generated method
      ((BaseButton) containerButton2).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(cDisplayClass191.\u003CRebuildUI\u003Eb__0);
      bool flag = true;
      DepartmentPrototype[] array = this._prototypeManager.EnumerateCM<DepartmentPrototype>().ToArray<DepartmentPrototype>();
      Array.Sort<DepartmentPrototype>(array, (IComparer<DepartmentPrototype>) DepartmentUIComparer.Instance);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._jobButtons[cDisplayClass191.CS\u0024\u003C\u003E8__locals1.id] = new Dictionary<string, List<JobButton>>();
      foreach (DepartmentPrototype departmentPrototype in array)
      {
        string str2 = Loc.GetString(LocId.op_Implicit(departmentPrototype.Name));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this._jobCategories[cDisplayClass191.CS\u0024\u003C\u003E8__locals1.id] = new Dictionary<string, BoxContainer>();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Dictionary<ProtoId<JobPrototype>, int?> dictionary = this._gameTicker.JobsAvailable[cDisplayClass191.CS\u0024\u003C\u003E8__locals1.id];
        List<JobPrototype> jobPrototypeList = new List<JobPrototype>();
        foreach (ProtoId<JobPrototype> role in departmentPrototype.Roles)
        {
          if (dictionary.ContainsKey(role))
            jobPrototypeList.Add(this._prototypeManager.Index<JobPrototype>(role));
        }
        jobPrototypeList.Sort((IComparer<JobPrototype>) JobUIComparer.Instance);
        if (jobPrototypeList.Count != 0)
        {
          BoxContainer boxContainer3 = new BoxContainer();
          boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
          ((Control) boxContainer3).Name = departmentPrototype.ID;
          ((Control) boxContainer3).ToolTip = Loc.GetString("late-join-gui-jobs-amount-in-department-tooltip", new (string, object)[1]
          {
            ("departmentName", (object) str2)
          });
          BoxContainer boxContainer4 = boxContainer3;
          if (flag)
            flag = false;
          else
            ((Control) boxContainer4).AddChild(new Control()
            {
              MinSize = new Vector2(0.0f, 23f)
            });
          BoxContainer boxContainer5 = boxContainer4;
          PanelContainer panelContainer2 = new PanelContainer();
          Control.OrderedChildCollection children4 = ((Control) panelContainer2).Children;
          Label label2 = new Label();
          ((Control) label2).StyleClasses.Add("LabelBig");
          label2.Text = Loc.GetString("late-join-gui-department-jobs-label", new (string, object)[1]
          {
            ("departmentName", (object) str2)
          });
          Label label3 = label2;
          children4.Add((Control) label3);
          PanelContainer panelContainer3 = panelContainer2;
          ((Control) boxContainer5).AddChild((Control) panelContainer3);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this._jobCategories[cDisplayClass191.CS\u0024\u003C\u003E8__locals1.id][departmentPrototype.ID] = boxContainer4;
          ((Control) boxContainer2).AddChild((Control) boxContainer4);
          foreach (JobPrototype job in jobPrototypeList)
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            LateJoinGui.\u003C\u003Ec__DisplayClass19_2 cDisplayClass192 = new LateJoinGui.\u003C\u003Ec__DisplayClass19_2();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass192.CS\u0024\u003C\u003E8__locals2 = cDisplayClass191;
            int? amount = dictionary[ProtoId<JobPrototype>.op_Implicit(job.ID)];
            Label label4 = new Label();
            ((Control) label4).Margin = new Thickness(5f, 0.0f, 0.0f, 0.0f);
            Label jobLabel = label4;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass192.jobButton = new JobButton(jobLabel, ProtoId<JobPrototype>.op_Implicit(job.ID), job.LocalizedName, amount);
            BoxContainer boxContainer6 = new BoxContainer();
            boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 0;
            ((Control) boxContainer6).HorizontalExpand = true;
            BoxContainer boxContainer7 = boxContainer6;
            TextureRect textureRect2 = new TextureRect();
            textureRect2.TextureScale = new Vector2(2f, 2f);
            ((Control) textureRect2).VerticalAlignment = (Control.VAlignment) 2;
            TextureRect textureRect3 = textureRect2;
            JobIconPrototype jobIconPrototype = this._prototypeManager.Index<JobIconPrototype>(job.Icon);
            textureRect3.Texture = this._sprites.Frame0(jobIconPrototype.Icon);
            ((Control) boxContainer7).AddChild((Control) textureRect3);
            ((Control) boxContainer7).AddChild((Control) jobLabel);
            // ISSUE: reference to a compiler-generated field
            ((Control) cDisplayClass192.jobButton).AddChild((Control) boxContainer7);
            // ISSUE: reference to a compiler-generated field
            ((Control) boxContainer4).AddChild((Control) cDisplayClass192.jobButton);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated method
            ((BaseButton) cDisplayClass192.jobButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(cDisplayClass192.\u003CRebuildUI\u003Eb__2);
            FormattedMessage reason;
            if (!this._jobRequirements.IsAllowed(job, (HumanoidCharacterProfile) this._preferencesManager.Preferences?.SelectedCharacter, out reason))
            {
              // ISSUE: reference to a compiler-generated field
              ((BaseButton) cDisplayClass192.jobButton).Disabled = true;
              if (!reason.IsEmpty)
              {
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type
                LateJoinGui.\u003C\u003Ec__DisplayClass19_3 cDisplayClass193 = new LateJoinGui.\u003C\u003Ec__DisplayClass19_3();
                // ISSUE: reference to a compiler-generated field
                cDisplayClass193.tooltip = new Tooltip();
                // ISSUE: reference to a compiler-generated field
                cDisplayClass193.tooltip.SetMessage(reason);
                // ISSUE: reference to a compiler-generated field
                // ISSUE: method pointer
                ((Control) cDisplayClass192.jobButton).TooltipSupplier = new TooltipSupplier((object) cDisplayClass193, __methodptr(\u003CRebuildUI\u003Eb__3));
              }
              BoxContainer boxContainer8 = boxContainer7;
              TextureRect textureRect4 = new TextureRect();
              textureRect4.TextureScale = new Vector2(0.4f, 0.4f);
              textureRect4.Stretch = (TextureRect.StretchMode) 4;
              textureRect4.Texture = this._sprites.Frame0((SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Nano/lock.svg.192dpi.png")));
              ((Control) textureRect4).HorizontalExpand = true;
              ((Control) textureRect4).HorizontalAlignment = (Control.HAlignment) 3;
              ((Control) boxContainer8).AddChild((Control) textureRect4);
            }
            else
            {
              int? nullable = amount;
              int num = 0;
              if (nullable.GetValueOrDefault() == num & nullable.HasValue)
              {
                // ISSUE: reference to a compiler-generated field
                ((BaseButton) cDisplayClass192.jobButton).Disabled = true;
              }
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            if (!this._jobButtons[cDisplayClass192.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.id].ContainsKey(job.ID))
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              this._jobButtons[cDisplayClass192.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.id][job.ID] = new List<JobButton>();
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            this._jobButtons[cDisplayClass192.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.id][job.ID].Add(cDisplayClass192.jobButton);
          }
        }
      }
    }
  }

  private void JobsAvailableUpdated(
    IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> updatedJobs)
  {
    foreach (KeyValuePair<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> updatedJob in (IEnumerable<KeyValuePair<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>) updatedJobs)
    {
      if (this._jobButtons.ContainsKey(updatedJob.Key))
      {
        Dictionary<ProtoId<JobPrototype>, int?> dictionary = updatedJob.Value;
        foreach (KeyValuePair<string, List<JobButton>> keyValuePair in this._jobButtons[updatedJob.Key])
        {
          if (dictionary.ContainsKey(ProtoId<JobPrototype>.op_Implicit(keyValuePair.Key)))
          {
            int? amount1 = dictionary[ProtoId<JobPrototype>.op_Implicit(keyValuePair.Key)];
            foreach (JobButton jobButton1 in keyValuePair.Value)
            {
              int? amount2 = jobButton1.Amount;
              int? nullable = amount1;
              if (!(amount2.GetValueOrDefault() == nullable.GetValueOrDefault() & amount2.HasValue == nullable.HasValue))
              {
                jobButton1.RefreshLabel(amount1);
                JobButton jobButton2 = jobButton1;
                int num1 = ((BaseButton) jobButton2).Disabled ? 1 : 0;
                nullable = jobButton1.Amount;
                int num2 = 0;
                int num3 = nullable.GetValueOrDefault() == num2 & nullable.HasValue ? 1 : 0;
                ((BaseButton) jobButton2).Disabled = (num1 | num3) != 0;
              }
            }
          }
        }
      }
    }
  }

  [Obsolete]
  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    this._jobRequirements.Updated -= new Action(this.RebuildUI);
    this._gameTicker.LobbyJobsAvailableUpdated -= new Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>(this.JobsAvailableUpdated);
    this._jobButtons.Clear();
    this._jobCategories.Clear();
  }
}
