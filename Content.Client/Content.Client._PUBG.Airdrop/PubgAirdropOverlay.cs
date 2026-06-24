using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Airdrop;

public sealed class PubgAirdropOverlay : Overlay
{
	private const string TextFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";

	private const int TextFontSize = 12;

	private readonly IPlayerManager _player;

	private readonly SharedTransformSystem _transform;

	private readonly Font _font;

	public bool Active;

	public Vector2 Target;

	public int RemainingSeconds;

	public MapId MapId;

	private int _cachedSeconds = int.MinValue;

	private string _cachedText = string.Empty;

	public override OverlaySpace Space => (OverlaySpace)2;

	public PubgAirdropOverlay(IResourceCache cache, IPlayerManager player, SharedTransformSystem transform)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		_player = player;
		_transform = transform;
		_font = (Font)new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!Active || args.ViewportControl == null)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(localEntity.Value)) != MapId))
		{
			Vector2 vector = args.ViewportControl.WorldToScreen(Target);
			if (RemainingSeconds != _cachedSeconds)
			{
				_cachedSeconds = RemainingSeconds;
				_cachedText = Loc.GetString("pubg-airdrop-countdown", new(string, object)[1] { ("time", FormatTime(RemainingSeconds)) });
			}
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, vector + new Vector2(0f, -20f), _cachedText, Color.White);
		}
	}

	private static string FormatTime(int seconds)
	{
		if (seconds < 0)
		{
			seconds = 0;
		}
		int value = seconds / 60;
		int value2 = seconds % 60;
		return $"{value:D2}:{value2:D2}";
	}
}
