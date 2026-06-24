using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._CIV14merka.MineTable;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.MineTable;

public sealed class CivMineRevealOverlay : Overlay
{
	[Dependency]
	private readonly IEyeManager _eye;

	[Dependency]
	private readonly IResourceCache _cache;

	private readonly Font _font;

	public List<CivMineRevealEntry> Entries = new List<CivMineRevealEntry>();

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivMineRevealOverlay()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		IoCManager.InjectDependencies<CivMineRevealOverlay>(this);
		_font = (Font)new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 11);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (Entries.Count == 0)
		{
			return;
		}
		MapId mapId = args.MapId;
		if (mapId == MapId.Nullspace)
		{
			return;
		}
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		string text = Loc.GetString("civ-mine-table-label");
		Color orangeRed = Color.OrangeRed;
		UIBox2 val = default(UIBox2);
		foreach (CivMineRevealEntry entry in Entries)
		{
			if (!(entry.MapId != mapId))
			{
				Vector2 vector = _eye.WorldToScreen(entry.Position);
				Vector2 dimensions = screenHandle.GetDimensions(_font, (ReadOnlySpan<char>)text, 1f);
				if (!(dimensions.X <= 0f))
				{
					float num = dimensions.X * 0.5f;
					float num2 = dimensions.Y * 0.5f;
					((UIBox2)(ref val))._002Ector(vector.X - num - 3f, vector.Y - num2 - 3f, vector.X + num + 3f, vector.Y + num2 + 3f);
					UIBox2 val2 = val;
					Color black = Color.Black;
					screenHandle.DrawRect(val2, ((Color)(ref black)).WithAlpha(0.55f), true);
					screenHandle.DrawString(_font, new Vector2(vector.X - num, vector.Y - num2), (ReadOnlySpan<char>)text, 1f, orangeRed);
				}
			}
		}
	}
}
