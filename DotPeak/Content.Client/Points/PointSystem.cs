// Decompiled with JetBrains decompiler
// Type: Content.Client.Points.PointSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CharacterInfo;
using Content.Client.Message;
using Content.Shared.Points;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Points;

public sealed class PointSystem : SharedPointSystem
{
  [Dependency]
  private CharacterInfoSystem _characterInfo;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PointManagerComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<PointManagerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CharacterInfoSystem.GetCharacterInfoControlsEvent>(new EntityEventRefHandler<CharacterInfoSystem.GetCharacterInfoControlsEvent>((object) this, __methodptr(OnGetCharacterInfoControls)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    PointManagerComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this._characterInfo.RequestCharacterInfo();
  }

  private void OnGetCharacterInfoControls(
    ref CharacterInfoSystem.GetCharacterInfoControlsEvent ev)
  {
    foreach (PointManagerComponent managerComponent in this.EntityQuery<PointManagerComponent>(false))
    {
      BoxContainer boxContainer1 = new BoxContainer();
      ((Control) boxContainer1).Margin = new Thickness(5f);
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      BoxContainer boxContainer2 = boxContainer1;
      RichTextLabel richTextLabel1 = new RichTextLabel();
      ((Control) richTextLabel1).HorizontalAlignment = (Control.HAlignment) 2;
      RichTextLabel label = richTextLabel1;
      label.SetMarkup(this.Loc.GetString("point-scoreboard-header"));
      RichTextLabel richTextLabel2 = new RichTextLabel();
      richTextLabel2.SetMessage(managerComponent.Scoreboard, new Color?());
      ((Control) boxContainer2).AddChild((Control) label);
      ((Control) boxContainer2).AddChild((Control) richTextLabel2);
      ev.Controls.Add((Control) boxContainer2);
    }
  }
}
