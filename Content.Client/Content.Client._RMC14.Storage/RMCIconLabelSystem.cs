using System;
using System.Numerics;
using System.Text;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.IconLabel;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Storage;

public sealed class RMCIconLabelSystem : SharedRMCIconLabelSystem
{
	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private IConfigurationManager _config;

	private Font _font;

	private bool _drawStorageIconLabels;

	public override void Initialize()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		_font = (Font)new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDrawStorageIconLabels, (Action<bool>)delegate(bool v)
		{
			_drawStorageIconLabels = v;
		}, true);
	}

	public void DrawStorage(EntityUid entity, float uiScale, Vector2 iconPosition, DrawingHandleScreen handle)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		IconLabelComponent iconLabelComponent = default(IconLabelComponent);
		if (!_drawStorageIconLabels || !((EntitySystem)this).TryComp<IconLabelComponent>(entity, ref iconLabelComponent))
		{
			return;
		}
		float num = 2f * uiScale;
		LocId? labelTextLocId = iconLabelComponent.LabelTextLocId;
		if (!labelTextLocId.HasValue)
		{
			return;
		}
		ILocalizationManager loc = ((EntitySystem)this).Loc;
		labelTextLocId = iconLabelComponent.LabelTextLocId;
		string text = default(string);
		if (loc.TryGetString(labelTextLocId.HasValue ? LocId.op_Implicit(labelTextLocId.GetValueOrDefault()) : null, ref text, iconLabelComponent.LabelTextParams.ToArray()))
		{
			if (text.Length > iconLabelComponent.LabelMaxSize)
			{
				text = text.Substring(0, iconLabelComponent.LabelMaxSize);
			}
			Color val = default(Color);
			Color.TryFromName(iconLabelComponent.TextColor, ref val);
			char[] array = text.ToCharArray();
			Vector2 vector = new Vector2(iconPosition.X + num * (float)iconLabelComponent.StoredOffset.X, iconPosition.Y + num * (float)iconLabelComponent.StoredOffset.Y);
			int textSize = iconLabelComponent.TextSize;
			float num2 = 0f;
			char[] array2 = array;
			foreach (char ch in array2)
			{
				vector.X += num2;
				num2 = _font.DrawChar((DrawingHandleBase)(object)handle, new Rune(ch), vector, (float)textSize * num, val, true);
			}
		}
	}
}
