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

namespace Content.Client.Chat.UI;

public sealed class FancyTextSpeechBubble : SpeechBubble
{
	public FancyTextSpeechBubble(ChatMessage message, EntityUid senderEntity, string speechStyleClass, Color? fontColor = null)
		: base(message, senderEntity, speechStyleClass, fontColor)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	protected override Control BuildBubble(ChatMessage message, string speechStyleClass, Color? fontColor = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		IEntityManager val = IoCManager.Resolve<IEntityManager>();
		EntityUid entity = val.GetEntity(message.SenderEntity);
		if (speechStyleClass == "sayBox")
		{
			if (message.SpeechStyleClass != null)
			{
				speechStyleClass = message.SpeechStyleClass;
			}
			else if (val.HasComponent<SquadLeaderComponent>(entity) || val.HasComponent<HiveLeaderComponent>(entity) || val.HasComponent<InnateCommandSpeechComponent>(entity))
			{
				speechStyleClass = "commanderSpeech";
			}
		}
		Color white;
		if (!ConfigManager.GetCVar<bool>(CCVars.ChatEnableFancyBubbles))
		{
			RichTextLabel val2 = new RichTextLabel
			{
				MaxWidth = 256f,
				StyleClasses = { "bubbleContent" }
			};
			val2.SetMessage(ExtractAndFormatSpeechSubstring(message, "BubbleContent", fontColor), (Color?)null);
			PanelContainer val3 = new PanelContainer
			{
				StyleClasses = { "speechBox" },
				StyleClasses = { speechStyleClass }
			};
			((Control)val3).Children.Add((Control)(object)val2);
			white = Color.White;
			((Control)val3).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity));
			return (Control)val3;
		}
		RichTextLabel val4 = new RichTextLabel();
		white = Color.White;
		((Control)val4).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<float>(CCVars.SpeechBubbleSpeakerOpacity));
		((Control)val4).Margin = new Thickness(1f, 1f, 1f, 1f);
		RichTextLabel val5 = val4;
		RichTextLabel val6 = new RichTextLabel();
		white = Color.White;
		((Control)val6).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<float>(CCVars.SpeechBubbleTextOpacity));
		((Control)val6).MaxWidth = 256f;
		((Control)val6).Margin = new Thickness(2f, 6f, 2f, 2f);
		((Control)val6).StyleClasses.Add("bubbleContent");
		RichTextLabel val7 = val6;
		val5.SetMessage(ExtractAndFormatSpeechSubstring(message, "BubbleHeader", fontColor), (Color?)null);
		val7.SetMessage(ExtractAndFormatSpeechSubstring(message, "BubbleContent", fontColor), (Color?)null);
		PanelContainer val8 = new PanelContainer
		{
			StyleClasses = { "speechBox" },
			StyleClasses = { speechStyleClass }
		};
		((Control)val8).Children.Add((Control)(object)val7);
		white = Color.White;
		((Control)val8).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity));
		((Control)val8).HorizontalAlignment = (HAlignment)2;
		((Control)val8).VerticalAlignment = (VAlignment)3;
		((Control)val8).Margin = new Thickness(4f, 14f, 4f, 2f);
		PanelContainer val9 = val8;
		PanelContainer val10 = new PanelContainer
		{
			StyleClasses = { "speechBox" },
			StyleClasses = { speechStyleClass }
		};
		((Control)val10).Children.Add((Control)(object)val5);
		white = Color.White;
		((Control)val10).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<bool>(CCVars.ChatFancyNameBackground) ? ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity) : 0f);
		((Control)val10).HorizontalAlignment = (HAlignment)2;
		((Control)val10).VerticalAlignment = (VAlignment)1;
		PanelContainer val11 = val10;
		PanelContainer val12 = new PanelContainer();
		((Control)val12).Children.Add((Control)(object)val9);
		((Control)val12).Children.Add((Control)(object)val11);
		return (Control)val12;
	}
}
