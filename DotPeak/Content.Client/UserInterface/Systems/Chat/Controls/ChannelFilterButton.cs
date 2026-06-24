// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChannelFilterButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Content.Shared.Chat;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelFilterButton : ChatPopupButton<ChannelFilterPopup>
{
  private static readonly Color ColorNormal = Color.FromHex((ReadOnlySpan<char>) "#7b7e9e", new Color?());
  private static readonly Color ColorHovered = Color.FromHex((ReadOnlySpan<char>) "#9699bb", new Color?());
  private static readonly Color ColorPressed = Color.FromHex((ReadOnlySpan<char>) "#789B8C", new Color?());
  private readonly TextureRect? _textureRect;
  private readonly ChatUIController _chatUIController;
  private const int FilterDropdownOffset = 120;

  public ChannelFilterButton()
  {
    this._chatUIController = ((Control) this).UserInterfaceManager.GetUIController<ChatUIController>();
    Texture texture = IoCManager.Resolve<IResourceCache>().GetTexture("/Textures/Interface/Nano/filter.svg.96dpi.png");
    TextureRect textureRect1 = new TextureRect();
    textureRect1.Texture = texture;
    ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
    TextureRect textureRect2 = textureRect1;
    this._textureRect = textureRect1;
    ((Control) this).AddChild((Control) textureRect2);
    this._chatUIController.FilterableChannelsChanged += new Action<ChatChannel>(this.Popup.SetChannels);
    this._chatUIController.UnreadMessageCountsUpdated += new Action<ChatChannel, int?>(this.Popup.UpdateUnread);
    this.Popup.SetChannels(this._chatUIController.FilterableChannels);
  }

  protected override UIBox2 GetPopupPosition()
  {
    Vector2 globalPosition = ((Control) this).GlobalPosition;
    float num1;
    float num2;
    Vector2Helpers.Deconstruct(((Control) this.Popup).MinSize, ref num1, ref num2);
    float val1 = num1;
    float y = num2;
    Vector2 vector2 = new Vector2(120f, 0.0f);
    return UIBox2.FromDimensions(globalPosition - vector2, new Vector2(Math.Max(val1, ((Control) this.Popup).MinWidth), y));
  }

  private void UpdateChildColors()
  {
    if (this._textureRect == null)
      return;
    switch ((int) ((BaseButton) this).DrawMode)
    {
      case 0:
        ((Control) this._textureRect).ModulateSelfOverride = new Color?(ChannelFilterButton.ColorNormal);
        break;
      case 1:
        ((Control) this._textureRect).ModulateSelfOverride = new Color?(ChannelFilterButton.ColorPressed);
        break;
      case 2:
        ((Control) this._textureRect).ModulateSelfOverride = new Color?(ChannelFilterButton.ColorHovered);
        break;
    }
  }

  protected virtual void DrawModeChanged()
  {
    ((ContainerButton) this).DrawModeChanged();
    this.UpdateChildColors();
  }

  protected virtual void StylePropertiesChanged()
  {
    base.StylePropertiesChanged();
    this.UpdateChildColors();
  }

  [Obsolete]
  protected virtual void Dispose(bool disposing)
  {
    ((BaseButton) this).Dispose(disposing);
    if (!disposing)
      return;
    this._chatUIController.FilterableChannelsChanged -= new Action<ChatChannel>(this.Popup.SetChannels);
    this._chatUIController.UnreadMessageCountsUpdated -= new Action<ChatChannel, int?>(this.Popup.UpdateUnread);
  }
}
