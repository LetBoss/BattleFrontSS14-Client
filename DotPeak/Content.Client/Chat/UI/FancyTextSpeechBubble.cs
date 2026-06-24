// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.UI.FancyTextSpeechBubble
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Chat.UI;

public sealed class FancyTextSpeechBubble(
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
    IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
    EntityUid entity = ientityManager.GetEntity(message.SenderEntity);
    if (speechStyleClass == "sayBox")
    {
      if (message.SpeechStyleClass != null)
        speechStyleClass = message.SpeechStyleClass;
      else if (ientityManager.HasComponent<SquadLeaderComponent>(entity) || ientityManager.HasComponent<HiveLeaderComponent>(entity) || ientityManager.HasComponent<InnateCommandSpeechComponent>(entity))
        speechStyleClass = "commanderSpeech";
    }
    if (!this.ConfigManager.GetCVar<bool>(CCVars.ChatEnableFancyBubbles))
    {
      RichTextLabel richTextLabel1 = new RichTextLabel();
      ((Control) richTextLabel1).MaxWidth = 256f;
      ((Control) richTextLabel1).StyleClasses.Add("bubbleContent");
      RichTextLabel richTextLabel2 = richTextLabel1;
      richTextLabel2.SetMessage(this.ExtractAndFormatSpeechSubstring(message, "BubbleContent", fontColor), new Color?());
      PanelContainer panelContainer = new PanelContainer();
      ((Control) panelContainer).StyleClasses.Add("speechBox");
      ((Control) panelContainer).StyleClasses.Add(speechStyleClass);
      ((Control) panelContainer).Children.Add((Control) richTextLabel2);
      Color white = Color.White;
      ((Control) panelContainer).ModulateSelfOverride = new Color?(((Color) ref white).WithAlpha(this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity)));
      return (Control) panelContainer;
    }
    RichTextLabel richTextLabel3 = new RichTextLabel();
    Color white1 = Color.White;
    ((Control) richTextLabel3).ModulateSelfOverride = new Color?(((Color) ref white1).WithAlpha(this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleSpeakerOpacity)));
    ((Control) richTextLabel3).Margin = new Thickness(1f, 1f, 1f, 1f);
    RichTextLabel richTextLabel4 = richTextLabel3;
    RichTextLabel richTextLabel5 = new RichTextLabel();
    Color white2 = Color.White;
    ((Control) richTextLabel5).ModulateSelfOverride = new Color?(((Color) ref white2).WithAlpha(this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleTextOpacity)));
    ((Control) richTextLabel5).MaxWidth = 256f;
    ((Control) richTextLabel5).Margin = new Thickness(2f, 6f, 2f, 2f);
    ((Control) richTextLabel5).StyleClasses.Add("bubbleContent");
    RichTextLabel richTextLabel6 = richTextLabel5;
    richTextLabel4.SetMessage(this.ExtractAndFormatSpeechSubstring(message, "BubbleHeader", fontColor), new Color?());
    richTextLabel6.SetMessage(this.ExtractAndFormatSpeechSubstring(message, "BubbleContent", fontColor), new Color?());
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).StyleClasses.Add("speechBox");
    ((Control) panelContainer1).StyleClasses.Add(speechStyleClass);
    ((Control) panelContainer1).Children.Add((Control) richTextLabel6);
    Color white3 = Color.White;
    ((Control) panelContainer1).ModulateSelfOverride = new Color?(((Color) ref white3).WithAlpha(this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity)));
    ((Control) panelContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer1).VerticalAlignment = (Control.VAlignment) 3;
    ((Control) panelContainer1).Margin = new Thickness(4f, 14f, 4f, 2f);
    PanelContainer panelContainer2 = panelContainer1;
    PanelContainer panelContainer3 = new PanelContainer();
    ((Control) panelContainer3).StyleClasses.Add("speechBox");
    ((Control) panelContainer3).StyleClasses.Add(speechStyleClass);
    ((Control) panelContainer3).Children.Add((Control) richTextLabel4);
    Color white4 = Color.White;
    ((Control) panelContainer3).ModulateSelfOverride = new Color?(((Color) ref white4).WithAlpha(this.ConfigManager.GetCVar<bool>(CCVars.ChatFancyNameBackground) ? this.ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity) : 0.0f));
    ((Control) panelContainer3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer3).VerticalAlignment = (Control.VAlignment) 1;
    PanelContainer panelContainer4 = panelContainer3;
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).Children.Add((Control) panelContainer2);
    ((Control) panelContainer5).Children.Add((Control) panelContainer4);
    return (Control) panelContainer5;
  }
}
