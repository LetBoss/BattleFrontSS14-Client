using System;
using System.Numerics;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivMapTitleOverlay : Overlay
{
	[Dependency]
	private IResourceCache _resources;

	[Dependency]
	private IGameTiming _timing;

	private readonly Font _font;

	private string? _title;

	private int _index;

	private bool _reverse;

	private TimeSpan _nextUpdate;

	private Vector2? _position;

	private TimeSpan _charInterval = TimeSpan.Zero;

	public override OverlaySpace Space => (OverlaySpace)2;

	public CivMapTitleOverlay()
	{
		IoCManager.InjectDependencies<CivMapTitleOverlay>(this);
		_font = _resources.GetFont("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 64);
		((Overlay)this).ZIndex = 300;
	}

	public void Show(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			Clear();
			return;
		}
		_title = title.Trim();
		_index = 0;
		_reverse = false;
		_nextUpdate = TimeSpan.Zero;
		_position = null;
		_charInterval = TimeSpan.FromSeconds(2f / (float)Math.Max(1, _title.Length));
	}

	private void Clear()
	{
		_title = null;
		_index = 0;
		_reverse = false;
		_nextUpdate = TimeSpan.Zero;
		_position = null;
		_charInterval = TimeSpan.Zero;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(_title))
		{
			return;
		}
		Vector2 valueOrDefault = _position.GetValueOrDefault();
		if (!_position.HasValue)
		{
			valueOrDefault = CalculatePosition(in args);
			_position = valueOrDefault;
		}
		int num = Math.Clamp(_index, 0, _title.Length);
		if (num > 0)
		{
			string text = _title.Substring(0, num);
			Vector2 value = _position.Value;
			DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
			Font font = _font;
			Vector2 vector = value + new Vector2(2f, 2f);
			ReadOnlySpan<char> readOnlySpan = text;
			Color black = Color.Black;
			screenHandle.DrawString(font, vector, readOnlySpan, 1f, ((Color)(ref black)).WithAlpha(0.85f));
			((OverlayDrawArgs)(ref args)).ScreenHandle.DrawString(_font, value, (ReadOnlySpan<char>)text, 1f, Color.White);
		}
		if (!(_nextUpdate > _timing.CurTime))
		{
			if (!_reverse && _index >= _title.Length)
			{
				_reverse = true;
				_nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(2L);
			}
			else if (_reverse && _index <= 0)
			{
				Clear();
			}
			else
			{
				_index = (_reverse ? (_index - 1) : (_index + 1));
				_nextUpdate = _timing.CurTime + _charInterval;
			}
		}
	}

	private Vector2 CalculatePosition(in OverlayDrawArgs args)
	{
		Vector2 dimensions = ((OverlayDrawArgs)(ref args)).ScreenHandle.GetDimensions(_font, (ReadOnlySpan<char>)_title, 1f);
		return new Vector2(((float)((UIBox2i)(ref args.ViewportBounds)).Width - dimensions.X) / 2f, 110f);
	}
}
