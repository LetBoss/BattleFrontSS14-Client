using System.Numerics;
using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Verbs.UI;

public sealed class VerbMenuElement : ContextMenuElement
{
	public const string StyleClassVerbMenuConfirmationTexture = "verbMenuConfirmationTexture";

	public readonly Verb? Verb;

	public bool IconVisible
	{
		set
		{
			((Control)base.Icon).Visible = value;
		}
	}

	public bool TextVisible
	{
		set
		{
			((Control)base.Label).Visible = value;
		}
	}

	public VerbMenuElement(Verb verb)
		: base(verb.Text)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		((Control)this).TooltipSupplier = (TooltipSupplier)delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			RichTextLabel val3 = new RichTextLabel();
			val3.SetMessage(FormattedMessage.FromMarkupOrThrow(verb.Message ?? verb.Text), (Color?)null);
			Tooltip val4 = new Tooltip();
			((Control)val4).GetChild(0).Children.Clear();
			((Control)val4).GetChild(0).Children.Add((Control)(object)val3);
			return (Control?)val4;
		};
		((BaseButton)this).Disabled = verb.Disabled;
		Verb = verb;
		((Control)base.Label).SetOnlyStyleClass(verb.TextStyleClass);
		if (verb.ConfirmationPopup)
		{
			((Control)base.ExpansionIndicator).SetOnlyStyleClass("verbMenuConfirmationTexture");
			((Control)base.ExpansionIndicator).Visible = true;
		}
		IEntityManager val = IoCManager.Resolve<IEntityManager>();
		if (verb.Icon == null && verb.IconEntity.HasValue)
		{
			SpriteView val2 = new SpriteView
			{
				OverrideDirection = (Direction)0,
				SetSize = new Vector2(32f, 32f)
			};
			val2.SetEntity((EntityUid?)val.GetEntity(verb.IconEntity.Value));
			((Control)base.Icon).AddChild((Control)(object)val2);
		}
		else
		{
			((Control)base.Icon).AddChild((Control)new TextureRect
			{
				Texture = ((verb.Icon != null) ? val.System<SpriteSystem>().Frame0(verb.Icon) : null),
				Stretch = (StretchMode)7
			});
		}
	}

	public VerbMenuElement(VerbCategory category, string styleClass)
		: base(category.Text)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		((Control)base.Label).SetOnlyStyleClass(styleClass);
		((Control)base.Icon).AddChild((Control)new TextureRect
		{
			Texture = ((category.Icon != null) ? IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SpriteSystem>().Frame0(category.Icon) : null),
			Stretch = (StretchMode)7
		});
	}
}
