// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.UniformAccessories.UniformAccessoryBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.UniformAccessories;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.UniformAccessories;

public sealed class UniformAccessoryBui : BoundUserInterface
{
  [Dependency]
  private IClyde _displayManager;
  [Dependency]
  private IEyeManager _eye;
  private readonly TransformSystem _transform;
  private readonly SharedContainerSystem _container;
  private UniformAccessoryMenu? _menu;

  public UniformAccessoryBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<UniformAccessoryBui>(this);
    this._transform = this.EntMan.System<TransformSystem>();
    this._container = this.EntMan.System<SharedContainerSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<UniformAccessoryMenu>((BoundUserInterface) this);
    if (this.EntMan.Deleted(this.Owner))
      return;
    this.Refresh();
  }

  public void Refresh()
  {
    UniformAccessoryHolderComponent accessoryHolderComponent;
    BaseContainer baseContainer;
    if (this._menu == null || !this.EntMan.TryGetComponent<UniformAccessoryHolderComponent>(this.Owner, ref accessoryHolderComponent) || !this._container.TryGetContainer(this.Owner, accessoryHolderComponent.ContainerId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    ((Control) this._menu?.Accessories).Children.Clear();
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      MetaDataComponent metaDataComponent;
      if (this.EntMan.TryGetComponent<MetaDataComponent>(containedEntity, ref metaDataComponent))
      {
        RadialMenuTextureButton menuTextureButton1 = new RadialMenuTextureButton();
        ((Control) menuTextureButton1).StyleClasses.Add("RadialMenuButton");
        ((Control) menuTextureButton1).SetSize = new Vector2(64f, 64f);
        ((Control) menuTextureButton1).ToolTip = metaDataComponent.EntityName;
        RadialMenuTextureButton menuTextureButton2 = menuTextureButton1;
        SpriteView spriteView1 = new SpriteView();
        spriteView1.OverrideDirection = new Direction?((Direction) 0);
        spriteView1.Scale = new Vector2(2f, 2f);
        ((Control) spriteView1).MaxSize = new Vector2(112f, 112f);
        spriteView1.Stretch = (SpriteView.StretchMode) 2;
        SpriteView spriteView2 = spriteView1;
        spriteView2.SetEntity(new EntityUid?(containedEntity));
        NetEntity netEnt = this.EntMan.GetNetEntity(containedEntity, (MetaDataComponent) null);
        ((BaseButton) menuTextureButton2).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new UniformAccessoriesBuiMsg(netEnt)));
        ((Control) menuTextureButton2).AddChild((Control) spriteView2);
        ((Control) this._menu?.Accessories).AddChild((Control) menuTextureButton2);
      }
    }
    Vector2i screenSize = this._displayManager.ScreenSize;
    this._menu?.OpenCenteredAt(this._eye.WorldToScreen(((SharedTransformSystem) this._transform).GetMapCoordinates(this.Owner, (TransformComponent) null).Position) / Vector2i.op_Implicit(screenSize));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(this.State is UniformAccessoriesBuiState))
      return;
    this.Refresh();
  }
}
