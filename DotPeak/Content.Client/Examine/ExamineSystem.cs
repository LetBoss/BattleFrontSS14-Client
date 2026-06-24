// Decompiled with JetBrains decompiler
// Type: Content.Client.Examine.ExamineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Verbs;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

#nullable enable
namespace Content.Client.Examine;

public sealed class ExamineSystem : ExamineSystemShared
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private VerbSystem _verbSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private List<Verb> _verbList = new List<Verb>();
  public const string StyleClassEntityTooltip = "entity-tooltip";
  private EntityUid _examinedEntity;
  private Popup? _examineTooltipOpen;
  private ScreenCoordinates _popupPos;
  private CancellationTokenSource? _requestCancelTokenSource;
  private int _idCounter;

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this.SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>(new EntityEventHandler<GetVerbsEvent<ExamineVerb>>(this.AddExamineVerb), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<ExamineSystemMessages.ExamineInfoResponseMessage>(new EntityEventHandler<ExamineSystemMessages.ExamineInfoResponseMessage>(this.OnExamineInfoResponse), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemComponent, DroppedEvent>(new ComponentEventHandler<ItemComponent, DroppedEvent>((object) this, __methodptr(OnExaminedItemDropped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.ExamineEntity, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleExamine)), true, true)).Register<ExamineSystem>();
    this._idCounter = 0;
  }

  private void OnExaminedItemDropped(EntityUid item, ItemComponent comp, DroppedEvent args)
  {
    EntityUid user1 = args.User;
    if (!((EntityUid) ref user1).Valid || this._examineTooltipOpen == null || !EntityUid.op_Equality(item, this._examinedEntity))
      return;
    EntityUid user2 = args.User;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(user2, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    this.CloseTooltip();
  }

  public virtual void Update(float frameTime)
  {
    Popup examineTooltipOpen = this._examineTooltipOpen;
    if (examineTooltipOpen == null || !((Control) examineTooltipOpen).Visible || !((EntityUid) ref this._examinedEntity).Valid)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || this.CanExamine(localEntity.GetValueOrDefault(), this._examinedEntity))
      return;
    this.CloseTooltip();
  }

  public virtual void Shutdown()
  {
    CommandBinds.Unregister<ExamineSystem>();
    base.Shutdown();
  }

  public override bool CanExamine(
    EntityUid examiner,
    MapCoordinates target,
    SharedInteractionSystem.Ignored? predicate = null,
    EntityUid? examined = null,
    ExaminerComponent? examinerComp = null)
  {
    if (!this.Resolve<ExaminerComponent>(examiner, ref examinerComp, false))
      return false;
    if (examinerComp.SkipChecks)
      return true;
    if (examinerComp.CheckInRangeUnOccluded)
    {
      Box2Rotated worldViewbounds = this._eyeManager.GetWorldViewbounds();
      if (!((Box2Rotated) ref worldViewbounds).Contains(target.Position))
        return false;
    }
    return base.CanExamine(examiner, target, predicate, examined, examinerComp);
  }

  private bool HandleExamine(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    EntityUid entityUid = args.EntityUid;
    if (!((EntityUid) ref args.EntityUid).IsValid() || !this.Exists(entityUid))
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this.CanExamine(localEntity.GetValueOrDefault(), entityUid))
      return false;
    this.DoExamine(entityUid);
    return true;
  }

  private void AddExamineVerb(GetVerbsEvent<ExamineVerb> args)
  {
    if (!this.CanExamine(args.User, args.Target))
      return;
    ExamineVerb examineVerb = new ExamineVerb();
    examineVerb.Category = VerbCategory.Examine;
    examineVerb.Priority = 10;
    examineVerb.Act = (Action) (() => this.DoExamine(args.Target, false));
    examineVerb.Text = this.Loc.GetString("examine-verb-name");
    examineVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"));
    examineVerb.ShowOnExamineTooltip = false;
    examineVerb.ClientExclusive = true;
    args.Verbs.Add(examineVerb);
  }

  private void OnExamineInfoResponse(
    ExamineSystemMessages.ExamineInfoResponseMessage ev)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || ev.Id != 0 && ev.Id != this._idCounter)
      return;
    EntityUid entity = this.GetEntity(ev.EntityUid);
    this.OpenTooltip(localEntity.Value, entity, ev.CenterAtCursor, ev.OpenAtOldTooltip, ev.KnowTarget);
    this.UpdateTooltipInfo(localEntity.Value, entity, ev.Message, ev.Verbs, false);
  }

  public override void SendExamineTooltip(
    EntityUid player,
    EntityUid target,
    FormattedMessage message,
    bool getVerbs,
    bool centerAtCursor)
  {
    this.OpenTooltip(player, target, centerAtCursor);
    this.UpdateTooltipInfo(player, target, message, getVerbs: getVerbs);
  }

  public void OpenTooltip(
    EntityUid player,
    EntityUid target,
    bool centeredOnCursor = true,
    bool openAtOldTooltip = true,
    bool knowTarget = true)
  {
    ScreenCoordinates? nullable1 = this._examineTooltipOpen != null ? new ScreenCoordinates?(this._popupPos) : new ScreenCoordinates?();
    this.CloseTooltip();
    this._examinedEntity = target;
    if (openAtOldTooltip && nullable1.HasValue)
      this._popupPos = nullable1.Value;
    else if (centeredOnCursor)
    {
      this._popupPos = this._userInterfaceManager.MousePositionScaled;
    }
    else
    {
      this._popupPos = this._eyeManager.CoordinatesToScreen(this.Transform(target).Coordinates);
      this._popupPos = this._userInterfaceManager.ScreenToUIPosition(this._popupPos);
    }
    Popup popup = new Popup();
    ((Control) popup).MaxWidth = 400f;
    this._examineTooltipOpen = popup;
    ((Control) this._userInterfaceManager.ModalRoot).AddChild((Control) this._examineTooltipOpen);
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).Name = "ExaminePopupPanel";
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).AddStyleClass("entity-tooltip");
    PanelContainer panelContainer3 = panelContainer2;
    Color lightGray = Color.LightGray;
    Color? nullable2 = new Color?(((Color) ref lightGray).WithAlpha(0.9f));
    ((Control) panelContainer3).ModulateSelfOverride = nullable2;
    ((Control) this._examineTooltipOpen).AddChild((Control) panelContainer2);
    BoxContainer boxContainer1 = new BoxContainer();
    ((Control) boxContainer1).Name = "ExaminePopupVbox";
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).MaxWidth = ((Control) this._examineTooltipOpen).MaxWidth;
    BoxContainer boxContainer2 = boxContainer1;
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(5);
    ((Control) boxContainer3).Margin = new Thickness(6f, 0.0f, 6f, 0.0f);
    BoxContainer boxContainer4 = boxContainer3;
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    if (this.HasComp<SpriteComponent>(target))
    {
      SpriteView spriteView1 = new SpriteView();
      spriteView1.OverrideDirection = new Direction?((Direction) 0);
      ((Control) spriteView1).SetSize = new Vector2(32f, 32f);
      SpriteView spriteView2 = spriteView1;
      spriteView2.SetEntity(new EntityUid?(target));
      ((Control) boxContainer4).AddChild((Control) spriteView2);
    }
    if (knowTarget)
    {
      FormattedMessage formattedMessage = FormattedMessage.FromMarkupPermissive($"[bold]{FormattedMessage.EscapeText((string) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(player)))}[/bold]");
      RichTextLabel richTextLabel = new RichTextLabel();
      richTextLabel.SetMessage(formattedMessage, new Color?());
      ((Control) boxContainer4).AddChild((Control) richTextLabel);
    }
    else
    {
      RichTextLabel richTextLabel = new RichTextLabel();
      richTextLabel.SetMessage(FormattedMessage.FromMarkupOrThrow("[bold]???[/bold]"), new Color?());
      ((Control) boxContainer4).AddChild((Control) richTextLabel);
    }
    ((Control) panelContainer2).Measure(Vector2Helpers.Infinity);
    this._examineTooltipOpen.Open(new UIBox2?(UIBox2.FromDimensions(this._popupPos.Position, Vector2.Max(new Vector2(300f, 0.0f), ((Control) panelContainer2).DesiredSize))), new Vector2?(), new Vector2?());
  }

  public void UpdateTooltipInfo(
    EntityUid player,
    EntityUid target,
    FormattedMessage message,
    List<Verb>? verbs = null,
    bool getVerbs = true)
  {
    Control child = ((Control) this._examineTooltipOpen)?.GetChild(0).GetChild(0);
    if (child == null)
      return;
    foreach (MarkupNode node in (IEnumerable<MarkupNode>) message.Nodes)
    {
      if (node.Name == null && !string.IsNullOrWhiteSpace(((MarkupParameter) ref node.Value).StringValue ?? ""))
      {
        RichTextLabel richTextLabel1 = new RichTextLabel();
        ((Control) richTextLabel1).Margin = new Thickness(4f, 4f, 0.0f, 4f);
        RichTextLabel richTextLabel2 = richTextLabel1;
        richTextLabel2.SetMessage(message, new Color?());
        child.AddChild((Control) richTextLabel2);
        break;
      }
    }
    SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(target, player, typeof (ExamineVerb));
    if (!getVerbs)
    {
      this._verbList.AddRange((IEnumerable<Verb>) localVerbs);
      foreach (Verb verb in this._verbList)
      {
        if (!verb.ClientExclusive)
          localVerbs.Remove(verb);
      }
      this._verbList.Clear();
    }
    if (verbs != null)
      localVerbs.UnionWith((IEnumerable<Verb>) verbs);
    this.AddVerbsToTooltip((IEnumerable<Verb>) localVerbs);
  }

  private void AddVerbsToTooltip(IEnumerable<Verb> verbs)
  {
    if (this._examineTooltipOpen == null)
      return;
    BoxContainer boxContainer1 = new BoxContainer();
    ((Control) boxContainer1).Name = "ExamineButtonsHBox";
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 0;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 3;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    ((Control) boxContainer3).Name = "HoverExamineHBox";
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) boxContainer3).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer();
    ((Control) boxContainer5).Name = "ClickExamineHBox";
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer5).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) boxContainer5).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer5).HorizontalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    foreach (Verb verb1 in verbs)
    {
      if (verb1 is ExamineVerb verb2 && verb2.Icon != null && verb2.ShowOnExamineTooltip)
      {
        ExamineButton examineButton = new ExamineButton(verb2, this._sprite);
        if (verb2.HoverVerb)
        {
          ((Control) boxContainer4).AddChild((Control) examineButton);
        }
        else
        {
          ((BaseButton) examineButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.VerbButtonPressed);
          ((Control) boxContainer6).AddChild((Control) examineButton);
        }
      }
    }
    Control child = ((Control) this._examineTooltipOpen)?.GetChild(0).GetChild(0);
    if (child == null)
    {
      ((Control) boxContainer2).Orphan();
    }
    else
    {
      Control[] array = ((IEnumerable<Control>) child.Children).Where<Control>((Func<Control, bool>) (c => c.Name == "ExamineButtonsHBox")).ToArray<Control>();
      if (((IEnumerable<Control>) array).Any<Control>())
        child.Children.Remove(((IEnumerable<Control>) array).First<Control>());
      ((Control) boxContainer2).AddChild((Control) boxContainer4);
      ((Control) boxContainer2).AddChild((Control) boxContainer6);
      child.AddChild((Control) boxContainer2);
    }
  }

  public void VerbButtonPressed(BaseButton.ButtonEventArgs obj)
  {
    if (!(obj.Button is ExamineButton button))
      return;
    this._verbSystem.ExecuteVerb(this._examinedEntity, (Verb) button.Verb);
    if (((int) button.Verb.CloseMenu ?? (button.Verb.CloseMenuDefault ? 1 : 0)) == 0)
      return;
    this.CloseTooltip();
  }

  public void DoExamine(EntityUid entity, bool centeredOnCursor = true, EntityUid? userOverride = null)
  {
    EntityUid? examiner = userOverride ?? ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!examiner.HasValue)
      return;
    this.OpenTooltip(examiner.Value, entity, centeredOnCursor, false);
    FormattedMessage examineText = this.GetExamineText(entity, examiner);
    this.UpdateTooltipInfo(examiner.Value, entity, examineText);
    if (!this.IsClientSide(entity, (MetaDataComponent) null))
    {
      ++this._idCounter;
      this.RaiseNetworkEvent((EntityEventArgs) new ExamineSystemMessages.RequestExamineInfoMessage(this.GetNetEntity(entity, (MetaDataComponent) null), this._idCounter, true));
    }
    this.RaiseLocalEvent<ClientExaminedEvent>(entity, new ClientExaminedEvent(entity, examiner.Value), false);
  }

  private void CloseTooltip()
  {
    if (this._examineTooltipOpen != null)
    {
      foreach (Control child in ((Control) this._examineTooltipOpen).Children)
      {
        if (child is ExamineButton examineButton)
          ((BaseButton) examineButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.VerbButtonPressed);
      }
      ((Control) this._examineTooltipOpen).Orphan();
      this._examineTooltipOpen = (Popup) null;
    }
    if (this._requestCancelTokenSource == null)
      return;
    this._requestCancelTokenSource.Cancel();
    this._requestCancelTokenSource = (CancellationTokenSource) null;
  }
}
