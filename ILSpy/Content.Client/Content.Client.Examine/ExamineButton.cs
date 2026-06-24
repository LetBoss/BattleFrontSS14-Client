using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Examine;

public sealed class ExamineButton : ContainerButton
{
	public const string StyleClassExamineButton = "examine-button";

	public const int ElementHeight = 32;

	public const int ElementWidth = 32;

	private const int Thickness = 4;

	public TextureRect Icon;

	public ExamineVerb Verb;

	private SpriteSystem _sprite;

	public ExamineButton(ExamineVerb verb, SpriteSystem spriteSystem)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		((Control)this).Margin = new Thickness(4f, 4f, 4f, 4f);
		((Control)this).SetOnlyStyleClass("examine-button");
		Verb = verb;
		_sprite = spriteSystem;
		if (verb.Disabled)
		{
			((BaseButton)this).Disabled = true;
		}
		((Control)this).TooltipSupplier = (TooltipSupplier)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			RichTextLabel val = new RichTextLabel();
			val.SetMessage(FormattedMessage.FromMarkupOrThrow(verb.Message ?? verb.Text), (Color?)null);
			Tooltip val2 = new Tooltip();
			((Control)val2).GetChild(0).Children.Clear();
			((Control)val2).GetChild(0).Children.Add((Control)(object)val);
			return (Control?)val2;
		};
		Icon = new TextureRect
		{
			SetWidth = 32f,
			SetHeight = 32f
		};
		if (verb.Icon != null)
		{
			Icon.Texture = _sprite.Frame0(verb.Icon);
			Icon.Stretch = (StretchMode)7;
			((Control)this).AddChild((Control)(object)Icon);
		}
	}
}
