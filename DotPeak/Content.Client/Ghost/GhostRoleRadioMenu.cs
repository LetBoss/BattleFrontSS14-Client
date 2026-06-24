// Decompiled with JetBrains decompiler
// Type: Content.Client.Ghost.GhostRoleRadioMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using CompiledRobustXaml;
using Content.Client.UserInterface.Controls;
using Content.Shared.Ghost.Roles;
using Content.Shared.Ghost.Roles.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML.Proxy;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Client.Ghost;

[XamlMetadata("resm:Content.Client.Ghost.GhostRoleRadioMenu.xaml?assembly=Content.Client", "Content.Client.Ghost.GhostRoleRadioMenu.xaml", "<ui:RadialMenu\r\n    xmlns:ui=\"clr-namespace:Content.Client.UserInterface.Controls\"\r\n    CloseButtonStyleClass=\"RadialMenuCloseButton\"\r\n    VerticalExpand=\"True\"\r\n    HorizontalExpand=\"True\">\r\n    <ui:RadialContainer Name=\"Main\">\r\n    </ui:RadialContainer>\r\n</ui:RadialMenu>\r\n")]
public sealed class GhostRoleRadioMenu : RadialMenu
{
  [Dependency]
  private EntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public event Action<ProtoId<GhostRolePrototype>>? SendGhostRoleRadioMessageAction;

  public EntityUid Entity { get; set; }

  public GhostRoleRadioMenu()
  {
    IoCManager.InjectDependencies<GhostRoleRadioMenu>(this);
    GhostRoleRadioMenu.\u0021XamlIlPopulateTrampoline(this);
  }

  public void SetEntity(EntityUid uid)
  {
    this.Entity = uid;
    this.RefreshUI();
  }

  private void RefreshUI()
  {
    RadialContainer control = ((Control) this).FindControl<RadialContainer>("Main");
    GhostRoleMobSpawnerComponent spawnerComponent;
    if (!this._entityManager.TryGetComponent<GhostRoleMobSpawnerComponent>(this.Entity, ref spawnerComponent))
      return;
    foreach (string selectablePrototype in spawnerComponent.SelectablePrototypes)
    {
      GhostRolePrototype ghostRolePrototype;
      if (this._prototypeManager.TryIndex<GhostRolePrototype>(selectablePrototype, ref ghostRolePrototype))
      {
        GhostRoleRadioMenuButton roleRadioMenuButton1 = new GhostRoleRadioMenuButton();
        ((Control) roleRadioMenuButton1).SetSize = new Vector2(64f, 64f);
        ((Control) roleRadioMenuButton1).ToolTip = Loc.GetString(ghostRolePrototype.Name);
        roleRadioMenuButton1.ProtoId = ProtoId<GhostRolePrototype>.op_Implicit(ghostRolePrototype.ID);
        GhostRoleRadioMenuButton roleRadioMenuButton2 = roleRadioMenuButton1;
        EntityPrototypeView entityPrototypeView1 = new EntityPrototypeView();
        ((Control) entityPrototypeView1).SetSize = new Vector2(48f, 48f);
        ((Control) entityPrototypeView1).VerticalAlignment = (Control.VAlignment) 2;
        ((Control) entityPrototypeView1).HorizontalAlignment = (Control.HAlignment) 2;
        ((SpriteView) entityPrototypeView1).Stretch = (SpriteView.StretchMode) 2;
        EntityPrototypeView entityPrototypeView2 = entityPrototypeView1;
        EntityPrototype entityPrototype;
        if (this._prototypeManager.TryIndex(ghostRolePrototype.IconPrototype, ref entityPrototype))
          entityPrototypeView2.SetPrototype(new EntProtoId?(EntProtoId.op_Implicit(entityPrototype)));
        else
          entityPrototypeView2.SetPrototype(new EntProtoId?(ghostRolePrototype.EntityPrototype));
        ((Control) roleRadioMenuButton2).AddChild((Control) entityPrototypeView2);
        ((Control) control).AddChild((Control) roleRadioMenuButton2);
        this.AddGhostRoleRadioMenuButtonOnClickActions((Control) control);
      }
    }
  }

  private void AddGhostRoleRadioMenuButtonOnClickActions(Control control)
  {
    if (!(control is RadialContainer radialContainer))
      return;
    foreach (Control child in ((Control) radialContainer).Children)
    {
      GhostRoleRadioMenuButton castChild = child as GhostRoleRadioMenuButton;
      if (castChild != null)
        ((BaseButton) castChild).OnButtonUp += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          Action<ProtoId<GhostRolePrototype>> radioMessageAction = this.SendGhostRoleRadioMessageAction;
          if (radioMessageAction != null)
            radioMessageAction(castChild.ProtoId);
          this.Close();
        });
    }
  }

  public static void Populate\u003AContent\u002EClient\u002EGhost\u002EGhostRoleRadioMenu\u002Examl(
    [In] IServiceProvider obj0,
    [In] RadialMenu obj1)
  {
    XamlIlContext.Context<RadialMenu> context = new XamlIlContext.Context<RadialMenu>(obj0, (object[]) null, "resm:Content.Client.Ghost.GhostRoleRadioMenu.xaml?assembly=Content.Client");
    context.RootObject = obj1;
    context.IntermediateRoot = (object) obj1;
    RadialMenu radialMenu = obj1;
    radialMenu.CloseButtonStyleClass = "RadialMenuCloseButton";
    ((Control) radialMenu).VerticalExpand = true;
    ((Control) radialMenu).HorizontalExpand = true;
    RadialContainer radialContainer = new RadialContainer();
    ((Control) radialContainer).Name = "Main";
    Control control1 = (Control) radialContainer;
    context.RobustNameScope.Register("Main", control1);
    ((Control) radialMenu).XamlChildren.Add((Control) radialContainer);
    if (radialMenu is Control control2)
    {
      context.RobustNameScope.Absorb(control2.NameScope);
      control2.NameScope = context.RobustNameScope;
    }
    context.RobustNameScope.Complete();
  }

  private static void \u0021XamlIlPopulateTrampoline([In] GhostRoleRadioMenu obj0)
  {
    if (IoCManager.Resolve<IXamlProxyHelper>().Populate(typeof (GhostRoleRadioMenu), (object) obj0))
      return;
    GhostRoleRadioMenu.Populate\u003AContent\u002EClient\u002EGhost\u002EGhostRoleRadioMenu\u002Examl((IServiceProvider) null, (RadialMenu) obj0);
  }
}
