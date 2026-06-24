// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Supply.CivSupplyVendorBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Vendors;
using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Vendors;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Client.GameObjects;
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
using System.Linq;

#nullable enable
namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyVendorBui : BoundUserInterface, ICMAutomatedVendorRefreshable
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SharedJobSystem _job;
  private readonly SharedMindSystem _mind;
  private readonly SharedRankSystem _rank;
  private readonly SharedRMCHolidaySystem _holiday;
  private readonly SpriteSystem _sprite;
  private CivSupplyVendorWindow? _window;

  public CivSupplyVendorBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._job = this.EntMan.System<SharedJobSystem>();
    this._mind = this.EntMan.System<SharedMindSystem>();
    this._rank = this.EntMan.System<SharedRankSystem>();
    this._holiday = this.EntMan.System<SharedRMCHolidaySystem>();
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<CivSupplyVendorWindow>((BoundUserInterface) this);
    this._window.Title = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(this.EntMan, this.Owner)?.EntityName ?? Loc.GetString("civ-eq-supply-vendor-default-title");
    CMAutomatedVendorComponent vendor;
    if (this.EntMan.TryGetComponent<CMAutomatedVendorComponent>(this.Owner, ref vendor))
      this.BuildSections(vendor);
    this._window.Search.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearchChanged);
    this.Refresh();
  }

  private void BuildSections(CMAutomatedVendorComponent vendor)
  {
    if (this._window == null)
      return;
    for (int index1 = 0; index1 < vendor.Sections.Count; ++index1)
    {
      CMVendorSection section = vendor.Sections[index1];
      CMAutomatedVendorSection automatedVendorSection = new CMAutomatedVendorSection()
      {
        Section = section
      };
      automatedVendorSection.Label.SetMessage(CivSupplyVendorBui.GetSectionName(section), new Color?());
      if (!this.IsSectionValid(section))
        automatedVendorSection.Visible = false;
      for (int index2 = 0; index2 < section.Entries.Count; ++index2)
      {
        CMVendorEntry entry1 = section.Entries[index2];
        CMAutomatedVendorEntry automatedVendorEntry = new CMAutomatedVendorEntry();
        EntityPrototype entityPrototype;
        if (this._prototype.TryIndex(entry1.Id, ref entityPrototype))
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          CivSupplyVendorBui.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100 = new CivSupplyVendorBui.\u003C\u003Ec__DisplayClass10_0();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.\u003C\u003E4__this = this;
          automatedVendorEntry.Texture.Textures = this._sprite.GetPrototypeTextures(entityPrototype).Select<IDirectionalTextureProvider, Texture>((Func<IDirectionalTextureProvider, Texture>) (texture => texture.Default)).ToList<Texture>();
          SpriteComponent spriteComponent;
          if (entityPrototype.TryGetComponent<SpriteComponent>("Sprite", ref spriteComponent))
            ((Control) automatedVendorEntry.Texture).Modulate = spriteComponent.AllLayers.First<ISpriteLayer>().Color;
          automatedVendorEntry.Panel.Button.TextLabel.Text = entry1.Name?.Replace("\\n", "\n") ?? entityPrototype.Name;
          Color color1 = Color.FromHex((ReadOnlySpan<char>) "#18211E", new Color?());
          Color color2 = Color.FromHex((ReadOnlySpan<char>) "#7D8F6E", new Color?());
          Color color3 = Color.FromHex((ReadOnlySpan<char>) "#A5B78D", new Color?());
          string name = entityPrototype.Name;
          if (section.TakeAll != null || section.TakeOne != null)
          {
            name = Loc.GetString("civ-eq-supply-mandatory", new (string, object)[1]
            {
              ("title", (object) name)
            });
            color1 = Color.FromHex((ReadOnlySpan<char>) "#2A2112", new Color?());
            color2 = Color.FromHex((ReadOnlySpan<char>) "#A8863E", new Color?());
            color3 = Color.FromHex((ReadOnlySpan<char>) "#C79F48", new Color?());
          }
          else if (entry1.Recommended)
          {
            automatedVendorEntry.Panel.Button.TextLabel.Text = "* " + automatedVendorEntry.Panel.Button.TextLabel.Text;
            name = Loc.GetString("civ-eq-supply-recommended", new (string, object)[1]
            {
              ("title", (object) name)
            });
            color1 = Color.FromHex((ReadOnlySpan<char>) "#162623", new Color?());
            color2 = Color.FromHex((ReadOnlySpan<char>) "#3E8C78", new Color?());
            color3 = Color.FromHex((ReadOnlySpan<char>) "#59B39C", new Color?());
          }
          automatedVendorEntry.Panel.Color = color1;
          automatedVendorEntry.Panel.BorderColor = color2;
          automatedVendorEntry.Panel.HoveredColor = color3;
          FormattedMessage formattedMessage = new FormattedMessage();
          formattedMessage.AddText(name);
          formattedMessage.PushNewline();
          if (!string.IsNullOrWhiteSpace(entityPrototype.Description))
            formattedMessage.AddText(entityPrototype.Description);
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.tooltip = new Tooltip();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.tooltip.SetMessage(formattedMessage);
          ((Control) automatedVendorEntry.TooltipLabel).TooltipDelay = new float?(0.0f);
          // ISSUE: method pointer
          ((Control) automatedVendorEntry.TooltipLabel).TooltipSupplier = new TooltipSupplier((object) cDisplayClass100, __methodptr(\u003CBuildSections\u003Eb__1));
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.sectionI = index1;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.entryI = index2;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass100.linkedEntryIndexes = new List<int>();
          int num = 0;
          foreach (CMVendorEntry entry2 in section.Entries)
          {
            if (entry1.LinkedEntries.Contains(entry2.Id))
            {
              // ISSUE: reference to a compiler-generated field
              cDisplayClass100.linkedEntryIndexes.Add(num);
            }
            ++num;
          }
          // ISSUE: reference to a compiler-generated method
          ((BaseButton) automatedVendorEntry.Panel.Button).OnPressed += new Action<BaseButton.ButtonEventArgs>(cDisplayClass100.\u003CBuildSections\u003Eb__2);
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
      if (this._holiday.IsActiveHoliday(holiday))
        flag3 = true;
    }
    return flag1 & flag2 & flag3;
  }

  private void OnButtonPressed(int sectionIndex, int entryIndex, List<int> linkedEntryIndexes)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new CMVendorVendBuiMsg(sectionIndex, entryIndex, linkedEntryIndexes));
  }

  private void OnSearchChanged(LineEdit.LineEditEventArgs args)
  {
    this.ApplySearchFilter(args.Text);
  }

  private void ApplySearchFilter(string text)
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
              string text1 = automatedVendorEntry1.Panel.Button.TextLabel.Text;
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
    if (!this.StructureMatches(vendor))
    {
      ((Control) this._window.Sections).DisposeAllChildren();
      this.BuildSections(vendor);
      this.ApplySearchFilter(this._window.Search.Text);
    }
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
    for (int index1 = 0; index1 < vendor.Sections.Count; ++index1)
    {
      CMVendorSection section = vendor.Sections[index1];
      CMAutomatedVendorSection child1 = (CMAutomatedVendorSection) ((Control) this._window.Sections).GetChild(index1);
      child1.Label.SetMessage(CivSupplyVendorBui.GetSectionName(section), new Color?());
      bool flag2 = false;
      (string Id, int Amount)? choices = section.Choices;
      int? nullable;
      int num3;
      if (choices.HasValue)
      {
        (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
        nullable = componentOrNull != null ? new int?(componentOrNull.Choices.GetValueOrDefault<string, int>(valueOrDefault.Id)) : new int?();
        num3 = valueOrDefault.Amount;
        if (nullable.GetValueOrDefault() >= num3 & nullable.HasValue || componentOrNull == null && valueOrDefault.Amount <= 0)
          flag2 = true;
      }
      bool flag3 = false;
      for (int index2 = 0; index2 < section.Entries.Count; ++index2)
      {
        CMVendorEntry entry = section.Entries[index2];
        CMAutomatedVendorEntry child2 = (CMAutomatedVendorEntry) ((Control) child1.Entries).GetChild(index2);
        int num4;
        if (!flag2)
        {
          nullable = entry.Amount;
          num3 = 0;
          num4 = nullable.GetValueOrDefault() <= num3 & nullable.HasValue ? 1 : 0;
        }
        else
          num4 = 1;
        bool flag4 = num4 != 0;
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
              goto label_33;
          }
          flag4 = true;
        }
        else
        {
          Label amount = child2.Amount;
          ref int? local = ref entry.Amount;
          string str;
          if (!local.HasValue)
          {
            str = (string) null;
          }
          else
          {
            num3 = local.GetValueOrDefault();
            str = num3.ToString();
          }
          if (str == null)
            str = string.Empty;
          amount.Text = str;
        }
label_33:
        ((Control) child2.Amount).Modulate = flag4 ? Color.Red : Color.White;
        ((BaseButton) child2.Panel.Button).Disabled = flag4;
        if (!string.IsNullOrWhiteSpace(child2.Amount.Text))
          flag3 = true;
      }
      for (int index3 = 0; index3 < section.Entries.Count; ++index3)
        ((Control) ((CMAutomatedVendorEntry) ((Control) child1.Entries).GetChild(index3)).Amount).Visible = flag3;
    }
    Label pointsLabel = this._window.PointsLabel;
    string empty;
    if (!flag1)
      empty = string.Empty;
    else
      empty = Loc.GetString("civ-eq-supply-points", new (string, object)[1]
      {
        ("points", (object) num2)
      });
    pointsLabel.Text = empty;
  }

  private bool StructureMatches(CMAutomatedVendorComponent vendor)
  {
    if (this._window == null || ((Control) this._window.Sections).ChildCount != vendor.Sections.Count)
      return false;
    for (int index = 0; index < vendor.Sections.Count; ++index)
    {
      if (!(((Control) this._window.Sections).GetChild(index) is CMAutomatedVendorSection child) || ((Control) child.Entries).ChildCount != vendor.Sections[index].Entries.Count)
        return false;
    }
    return true;
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (!(message is CMVendorRefreshBuiMsg))
      return;
    this.Refresh();
  }

  private static FormattedMessage GetSectionName(CMVendorSection section)
  {
    FormattedMessage sectionName = new FormattedMessage();
    sectionName.AddText(section.Name.ToUpperInvariant());
    if (section.TakeAll != null)
      sectionName.AddText(Loc.GetString("civ-eq-supply-section-take-all"));
    else if (section.TakeOne != null)
    {
      sectionName.AddText(Loc.GetString("civ-eq-supply-section-take-one"));
    }
    else
    {
      (string Id, int Amount)? choices = section.Choices;
      if (choices.HasValue)
      {
        (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
        sectionName.AddText(Loc.GetString("civ-eq-supply-section-choose", new (string, object)[1]
        {
          ("amount", (object) valueOrDefault.Amount)
        }));
      }
    }
    return sectionName;
  }
}
