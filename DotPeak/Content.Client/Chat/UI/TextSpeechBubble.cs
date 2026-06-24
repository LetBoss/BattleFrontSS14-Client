// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.UI.TextSpeechBubble
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Chat.UI;

public sealed class TextSpeechBubble(
  ChatMessage message,
  EntityUid senderEntity,
  string speechStyleClass,
  Color? fontColor = null) : SpeechBubble(message, senderEntity, speechStyleClass, fontColor)
{
  protected override Control BuildBubble(
    ChatMessage message,
    string speechStyleClass,
    Color? fontColor = null)
  {
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).MaxWidth = 256f;
    RichTextLabel richTextLabel2 = richTextLabel1;
    richTextLabel2.SetMessage(this.FormatSpeech(message.WrappedMessage, fontColor), new Color?());
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).StyleClasses.Add("speechBox");
    ((Control) panelContainer).StyleClasses.Add(speechStyleClass);
    ((Control) panelContainer).Children.Add((Control) richTextLabel2);
    Color white = Color.White;
    ((Control) panelContainer).ModulateSelfOverride = new Color?(((Color) ref white).WithAlpha(this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity)));
    return (Control) panelContainer;
  }
}
