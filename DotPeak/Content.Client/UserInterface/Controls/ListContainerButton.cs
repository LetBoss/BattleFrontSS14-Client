// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.ListContainerButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class ListContainerButton : ContainerButton, IEntityControl
{
  public readonly ListData Data;
  public readonly int Index;

  public ListContainerButton(ListData data, int index)
  {
    ((Control) this).AddStyleClass("button");
    this.Data = data;
    this.Index = index;
  }

  public EntityUid? UiEntity => (this.Data as EntityListData)?.Uid;
}
