// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vendors.CMAutomatedVendorBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared._RMC14.Vendors;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
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
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Vendors;

public sealed class CMAutomatedVendorBui : BoundUserInterface, ICMAutomatedVendorRefreshable
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IResourceCache _resource;
  private readonly SharedJobSystem _job;
  private readonly SharedMindSystem _mind;
  private readonly SharedRankSystem _rank;
  private readonly SharedRMCHolidaySystem _rmcHoliday;
  private readonly SpriteSystem _sprite;
  private CMAutomatedVendorWindow? _window;

  public CMAutomatedVendorBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._job = this.EntMan.System<SharedJobSystem>();
    this._mind = this.EntMan.System<SharedMindSystem>();
    this._rmcHoliday = this.EntMan.System<SharedRMCHolidaySystem>();
    this._rank = this.EntMan.System<SharedRankSystem>();
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<CMAutomatedVendorWindow>((BoundUserInterface) this);
    this._window.Title = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(this.EntMan, this.Owner)?.EntityName ?? "ColMarTech Vendor";
    this._window.ReagentsBar.ForegroundStyleBoxOverride = (StyleBox) new StyleBoxFlat(Color.FromHex((ReadOnlySpan<char>) "#AF7F38", new Color?()));
    CMAutomatedVendorComponent vendor;
    if (this.EntMan.TryGetComponent<CMAutomatedVendorComponent>(this.Owner, ref vendor))
      this.RebuildSections(vendor, EntityManagerExt.GetComponentOrNull<CMVendorUserComponent>(this.EntMan, ((ISharedPlayerManager) this._player).LocalEntity));
    this._window.Search.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearchChanged);
    this.Refresh();
  }

  private bool NeedsSectionRebuild(CMAutomatedVendorComponent vendor)
  {
    if (this._window == null)
      return false;
    if (((Control) this._window.Sections).ChildCount != vendor.Sections.Count)
      return true;
    for (int index = 0; index < vendor.Sections.Count; ++index)
    {
      if (!(((Control) this._window.Sections).GetChild(index) is CMAutomatedVendorSection child) || ((Control) child.Entries).ChildCount != vendor.Sections[index].Entries.Count)
        return true;
    }
    return false;
  }

  private void RebuildSections(CMAutomatedVendorComponent vendor, CMVendorUserComponent? user)
  {
    if (this._window == null)
      return;
    ((Control) this._window.Sections).DisposeAllChildren();
    for (int index1 = 0; index1 < vendor.Sections.Count; ++index1)
    {
      CMVendorSection section = vendor.Sections[index1];
      CMAutomatedVendorSection automatedVendorSection = new CMAutomatedVendorSection()
      {
        Section = section
      };
      automatedVendorSection.Label.SetMessage(this.GetSectionName(user, section), new Color?());
      automatedVendorSection.Visible = this.IsSectionValid(section);
      for (int index2 = 0; index2 < section.Entries.Count; ++index2)
      {
        CMVendorEntry entry1 = section.Entries[index2];
        CMAutomatedVendorEntry automatedVendorEntry = new CMAutomatedVendorEntry();
        EntityPrototype entityPrototype;
        if (this._prototype.TryIndex(entry1.Id, ref entityPrototype))
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          CMAutomatedVendorBui.\u003C\u003Ec__DisplayClass12_0 cDisplayClass120 = new CMAutomatedVendorBui.\u003C\u003Ec__DisplayClass12_0();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.\u003C\u003E4__this = this;
          automatedVendorEntry.Texture.Textures = this._sprite.GetPrototypeTextures(entityPrototype).Select<IDirectionalTextureProvider, Texture>((Func<IDirectionalTextureProvider, Texture>) (o => o.Default)).ToList<Texture>();
          SpriteComponent spriteComponent;
          if (entityPrototype.TryGetComponent<SpriteComponent>("Sprite", ref spriteComponent))
            ((Control) automatedVendorEntry.Texture).Modulate = spriteComponent.AllLayers.First<ISpriteLayer>().Color;
          automatedVendorEntry.Panel.Button.Label.Text = entry1.Name?.Replace("\\n", "\n") ?? entityPrototype.Name;
          string str = entityPrototype.Name;
          Color color1 = CMAutomatedVendorPanel.DefaultColor;
          Color color2 = CMAutomatedVendorPanel.DefaultBorderColor;
          Color color3 = CMAutomatedVendorPanel.DefaultBorderColor;
          if (section.TakeAll != null || section.TakeOne != null)
          {
            str = "Mandatory: " + str;
            color1 = Color.FromHex((ReadOnlySpan<char>) "#251A0C", new Color?());
            color2 = Color.FromHex((ReadOnlySpan<char>) "#805300", new Color?());
            color3 = Color.FromHex((ReadOnlySpan<char>) "#805300", new Color?());
          }
          else if (entry1.Recommended)
          {
            automatedVendorEntry.Panel.Button.Label.Text = "★ " + automatedVendorEntry.Panel.Button.Label.Text;
            str = "Recommended: " + str;
            color1 = Color.FromHex((ReadOnlySpan<char>) "#102919", new Color?());
            color2 = Color.FromHex((ReadOnlySpan<char>) "#3A9B52", new Color?());
            color3 = Color.FromHex((ReadOnlySpan<char>) "#3A9B52", new Color?());
          }
          automatedVendorEntry.Panel.Color = color1;
          automatedVendorEntry.Panel.BorderColor = color2;
          automatedVendorEntry.Panel.HoveredColor = color3;
          FormattedMessage formattedMessage = new FormattedMessage();
          formattedMessage.AddText(str);
          formattedMessage.PushNewline();
          if (!string.IsNullOrWhiteSpace(entityPrototype.Description))
            formattedMessage.AddText(entityPrototype.Description);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.tooltip = new Tooltip();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.tooltip.SetMessage(formattedMessage);
          ((Control) automatedVendorEntry.TooltipLabel).ToolTip = entityPrototype.Description;
          ((Control) automatedVendorEntry.TooltipLabel).TooltipDelay = new float?(0.0f);
          // ISSUE: method pointer
          ((Control) automatedVendorEntry.TooltipLabel).TooltipSupplier = new TooltipSupplier((object) cDisplayClass120, __methodptr(\u003CRebuildSections\u003Eb__1));
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.sectionI = index1;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.entryI = index2;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass120.linkedEntryIndexes = new List<int>();
          foreach (EntProtoId linkedEntry in entry1.LinkedEntries)
          {
            int num = 0;
            foreach (CMVendorEntry entry2 in section.Entries)
            {
              if (EntProtoId.op_Equality(entry2.Id, linkedEntry))
              {
                // ISSUE: reference to a compiler-generated field
                cDisplayClass120.linkedEntryIndexes.Add(num);
              }
              ++num;
            }
          }
          // ISSUE: reference to a compiler-generated method
          ((BaseButton) automatedVendorEntry.Panel.Button).OnPressed += new Action<BaseButton.ButtonEventArgs>(cDisplayClass120.\u003CRebuildSections\u003Eb__2);
        }
        ((Control) automatedVendorSection.Entries).AddChild((Control) automatedVendorEntry);
      }
      ((Control) this._window.Sections).AddChild((Control) automatedVendorSection);
    }
  }

  private bool IsSectionValid(CMVendorSection section)
  {
    bool flag1 = true;
    bool flag2 = true;
    Entity<MindComponent>? mind;
    if (((ISharedPlayerManager) this._player).LocalSession != null && this._mind.TryGetMind(((ISharedPlayerManager) this._player).LocalSession.UserId, out mind))
    {
      foreach (ProtoId<JobPrototype> job1 in section.Jobs)
      {
        SharedJobSystem job2 = this._job;
        Entity<MindComponent>? nullable = mind;
        EntityUid? mindId = nullable.HasValue ? new EntityUid?(Entity<MindComponent>.op_Implicit(nullable.GetValueOrDefault())) : new EntityUid?();
        string id = job1.Id;
        if (!job2.MindHasJobWithId(mindId, id))
        {
          flag1 = false;
        }
        else
        {
          flag1 = true;
          break;
        }
      }
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (localEntity.HasValue)
      {
        foreach (ProtoId<RankPrototype> rank1 in section.Ranks)
        {
          SharedRankSystem rank2 = this._rank;
          localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
          EntityUid uid = localEntity.Value;
          RankPrototype rank3 = rank2.GetRank(uid);
          if (rank3 == null || ProtoId<RankPrototype>.op_Inequality(ProtoId<RankPrototype>.op_Implicit(rank3), rank1))
          {
            flag2 = false;
          }
          else
          {
            flag2 = true;
            break;
          }
        }
      }
    }
    bool flag3 = section.Holidays.Count == 0;
    foreach (string holiday in section.Holidays)
    {
      if (this._rmcHoliday.IsActiveHoliday(holiday))
        flag3 = true;
    }
    return flag1 & flag3 & flag2;
  }

  private void OnButtonPressed(int sectionIndex, int entryIndex, List<int> linkedEntryIndexes)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new CMVendorVendBuiMsg(sectionIndex, entryIndex, linkedEntryIndexes));
  }

  private void OnSearchChanged(LineEdit.LineEditEventArgs args)
  {
    this.ApplySearchFilter(args.Text);
  }

  private void ApplySearchFilter(string? text)
  {
    if (this._window == null)
      return;
    foreach (Control child1 in ((Control) this._window.Sections).Children)
    {
      if (child1 is CMAutomatedVendorSection automatedVendorSection)
      {
        bool flag = false;
        foreach (Control child2 in ((Control) automatedVendorSection.Entries).Children)
        {
          if (child2 is CMAutomatedVendorEntry automatedVendorEntry1)
          {
            if (string.IsNullOrWhiteSpace(text))
            {
              automatedVendorEntry1.Visible = true;
            }
            else
            {
              CMAutomatedVendorEntry automatedVendorEntry = automatedVendorEntry1;
              string text1 = automatedVendorEntry1.Panel.Button.Label.Text;
              int num = text1 != null ? (text1.Contains(text, StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 0;
              automatedVendorEntry.Visible = num != 0;
            }
            if (automatedVendorEntry1.Visible)
              flag = true;
          }
        }
        automatedVendorSection.Visible = flag && (automatedVendorSection.Section == null || this.IsSectionValid(automatedVendorSection.Section));
      }
    }
  }

  public void Refresh()
  {
    CMAutomatedVendorComponent vendor;
    if (this._window == null || !this.EntMan.TryGetComponent<CMAutomatedVendorComponent>(this.Owner, ref vendor))
      return;
    bool flag1 = false;
    CMVendorUserComponent componentOrNull = EntityManagerExt.GetComponentOrNull<CMVendorUserComponent>(this.EntMan, ((ISharedPlayerManager) this._player).LocalEntity);
    int num1;
    if (vendor.PointsType != null)
    {
      int? nullable;
      if (componentOrNull == null)
      {
        nullable = new int?();
      }
      else
      {
        Dictionary<string, int> extraPoints = componentOrNull.ExtraPoints;
        nullable = extraPoints != null ? new int?(extraPoints.GetValueOrDefault<string, int>(vendor.PointsType)) : new int?();
      }
      num1 = nullable.GetValueOrDefault();
    }
    else
      num1 = componentOrNull != null ? componentOrNull.Points : 0;
    int num2 = num1;
    if (this.NeedsSectionRebuild(vendor))
      this.RebuildSections(vendor, componentOrNull);
    for (int index1 = 0; index1 < vendor.Sections.Count; ++index1)
    {
      CMVendorSection section = vendor.Sections[index1];
      CMAutomatedVendorSection child1 = (CMAutomatedVendorSection) ((Control) this._window.Sections).GetChild(index1);
      child1.Label.SetMessage(this.GetSectionName(componentOrNull, section), new Color?());
      bool flag2 = false;
      (string Id, int Amount)? choices = section.Choices;
      int? nullable;
      if (choices.HasValue)
      {
        (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
        nullable = componentOrNull != null ? new int?(componentOrNull.Choices.GetValueOrDefault<string, int>(valueOrDefault.Id)) : new int?();
        int amount = valueOrDefault.Amount;
        if (nullable.GetValueOrDefault() >= amount & nullable.HasValue || componentOrNull == null && valueOrDefault.Amount <= 0)
          flag2 = true;
      }
      bool flag3 = false;
      for (int index2 = 0; index2 < section.Entries.Count; ++index2)
      {
        CMVendorEntry entry = section.Entries[index2];
        CMAutomatedVendorEntry child2 = (CMAutomatedVendorEntry) ((Control) child1.Entries).GetChild(index2);
        int num3;
        if (!flag2)
        {
          nullable = entry.Amount;
          int num4 = 0;
          num3 = nullable.GetValueOrDefault() <= num4 & nullable.HasValue ? 1 : 0;
        }
        else
          num3 = 1;
        bool flag4 = num3 != 0;
        string takeAll1 = section.TakeAll;
        if (takeAll1 != null)
        {
          HashSet<(string, EntProtoId)> takeAll2 = componentOrNull?.TakeAll;
          if (takeAll2 != null && takeAll2.Contains((takeAll1, entry.Id)))
            flag4 = true;
        }
        string takeOne1 = section.TakeOne;
        if (takeOne1 != null)
        {
          HashSet<string> takeOne2 = componentOrNull?.TakeOne;
          if (takeOne2 != null && takeOne2.Contains(takeOne1))
            flag4 = true;
        }
        if (entry.Points.HasValue)
        {
          flag1 = true;
          child2.Amount.Text = $"{entry.Points}P";
          if (componentOrNull != null)
          {
            int num5 = num2;
            nullable = entry.Points;
            int valueOrDefault = nullable.GetValueOrDefault();
            if (!(num5 < valueOrDefault & nullable.HasValue))
              goto label_28;
          }
          flag4 = true;
        }
        else
          child2.Amount.Text = entry.Amount.ToString();
label_28:
        ((Control) child2.Amount).Modulate = flag4 ? Color.Red : Color.White;
        ((BaseButton) child2.Panel.Button).Disabled = flag4;
        if (!string.IsNullOrWhiteSpace(child2.Amount.Text))
          flag3 = true;
      }
      for (int index3 = 0; index3 < section.Entries.Count; ++index3)
        ((Control) ((CMAutomatedVendorEntry) ((Control) child1.Entries).GetChild(index3)).Amount).Visible = flag3;
    }
    this.ApplySearchFilter(this._window.Search.Text);
    Label pointsLabel = this._window.PointsLabel;
    string str;
    if (!flag1)
      str = string.Empty;
    else
      str = $"Points Remaining: {num2}";
    pointsLabel.Text = str;
    CMSolutionRefillerComponent refillerComponent;
    if (!this.EntMan.TryGetComponent<CMSolutionRefillerComponent>(this.Owner, ref refillerComponent))
    {
      ((Control) this._window.ReagentsContainer).Visible = false;
    }
    else
    {
      ((Control) this._window.ReagentsContainer).Visible = true;
      FixedPoint2 current = refillerComponent.Current;
      FixedPoint2 max = refillerComponent.Max;
      ((Range) this._window.ReagentsBar).MinValue = 0.0f;
      ((Range) this._window.ReagentsBar).MaxValue = (float) max.Int();
      ((Range) this._window.ReagentsBar).SetAsRatio((refillerComponent.Current / refillerComponent.Max).Float());
      this._window.ReagentsLabel.Text = $"{current.Int()} units";
    }
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (!(message is CMVendorRefreshBuiMsg))
      return;
    this.Refresh();
  }

  private FormattedMessage GetSectionName(CMVendorUserComponent? user, CMVendorSection section)
  {
    FormattedMessage sectionName = new FormattedMessage();
    sectionName.PushTag(new MarkupNode("bold", new MarkupParameter?(new MarkupParameter(section.Name.ToUpperInvariant())), (Dictionary<string, MarkupParameter>) null, false), false);
    sectionName.AddText(section.Name.ToUpperInvariant());
    if (section.TakeAll != null)
    {
      HashSet<(string, EntProtoId)> takeAll = user?.TakeAll;
      foreach (CMVendorEntry entry in section.Entries)
      {
        if (takeAll == null || !takeAll.Contains((section.TakeAll, entry.Id)))
        {
          sectionName.AddText(" (TAKE ALL)");
          break;
        }
      }
    }
    else if (section.TakeOne != null)
    {
      HashSet<string> takeOne = user?.TakeOne;
      if (takeOne == null || !takeOne.Contains(section.TakeOne))
        sectionName.AddText(" (TAKE ONE)");
    }
    else
    {
      (string Id, int Amount)? choices = section.Choices;
      if (choices.HasValue)
      {
        (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
        if (user == null)
        {
          sectionName.AddText($" (CHOOSE {valueOrDefault.Amount})");
        }
        else
        {
          int num = valueOrDefault.Amount - user.Choices.GetValueOrDefault<string, int>(valueOrDefault.Id);
          if (num > 0)
            sectionName.AddText($" (CHOOSE {num})");
        }
      }
    }
    sectionName.Pop();
    return sectionName;
  }
}
