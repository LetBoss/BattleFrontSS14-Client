using Content.Shared.CCVar;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chat.UI;

public sealed class TextSpeechBubble : SpeechBubble
{
	public TextSpeechBubble(ChatMessage message, EntityUid senderEntity, string speechStyleClass, Color? fontColor = null)
		: base(message, senderEntity, speechStyleClass, fontColor)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	protected override Control BuildBubble(ChatMessage message, string speechStyleClass, Color? fontColor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		RichTextLabel val = new RichTextLabel
		{
			MaxWidth = 256f
		};
		val.SetMessage(FormatSpeech(message.WrappedMessage, fontColor), (Color?)null);
		PanelContainer val2 = new PanelContainer
		{
			StyleClasses = { "speechBox" },
			StyleClasses = { speechStyleClass }
		};
		((Control)val2).Children.Add((Control)(object)val);
		Color white = Color.White;
		((Control)val2).ModulateSelfOverride = ((Color)(ref white)).WithAlpha(ConfigManager.GetCVar<float>(CCVars.SpeechBubbleBackgroundOpacity));
		return (Control)val2;
	}
}
