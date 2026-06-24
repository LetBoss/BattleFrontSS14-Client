// Decompiled with JetBrains decompiler
// Type: Content.Client.Info.ServerInfo
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Info;

public sealed class ServerInfo : BoxContainer
{
  private readonly RichTextLabel _richTextLabel;

  public ServerInfo()
  {
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).VerticalExpand = true;
    this._richTextLabel = richTextLabel;
    ((Control) this).AddChild((Control) this._richTextLabel);
  }

  public void SetInfoBlob(string markup)
  {
    this._richTextLabel.SetMessage(FormattedMessage.FromMarkupOrThrow(markup), new Color?());
  }
}
