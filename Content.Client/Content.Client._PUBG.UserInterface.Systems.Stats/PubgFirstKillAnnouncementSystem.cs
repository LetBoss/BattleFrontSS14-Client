using System;
using Content.Shared._PUBG;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgFirstKillAnnouncementSystem : EntitySystem
{
	private static readonly TimeSpan BannerLifetime = TimeSpan.FromSeconds(3L);

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototype;

	private PanelContainer? _banner;

	private TimeSpan _hideAt;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgFirstKillAnnouncementEvent>((EntitySessionEventHandler<PubgFirstKillAnnouncementEvent>)OnAnnouncement, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		PanelContainer? banner = _banner;
		if (banner != null)
		{
			((Control)banner).Orphan();
		}
		_banner = null;
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		PanelContainer? banner = _banner;
		if (((banner != null) ? ((Control)banner).Parent : null) != null && !(_timing.CurTime < _hideAt))
		{
			((Control)_banner).Orphan();
		}
	}

	private void OnAnnouncement(PubgFirstKillAnnouncementEvent msg, EntitySessionEventArgs args)
	{
		UIScreen activeScreen = _ui.ActiveScreen;
		PubgFirstKillBannerPrototype prototype = default(PubgFirstKillBannerPrototype);
		if (activeScreen != null && _prototype.TryIndex<PubgFirstKillBannerPrototype>(msg.BannerPrototypeId, ref prototype))
		{
			PanelContainer? banner = _banner;
			if (banner != null)
			{
				((Control)banner).Orphan();
			}
			_banner = BuildBanner(prototype);
			((Control)activeScreen).AddChild((Control)(object)_banner);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_banner, (LayoutPreset)10, (LayoutPresetMode)0, 0);
			LayoutContainer.SetMarginTop((Control)(object)_banner, 16f);
			_hideAt = _timing.CurTime + BannerLifetime;
		}
	}

	private PanelContainer BuildBanner(PubgFirstKillBannerPrototype prototype)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		int num = Math.Max(1, prototype.Width);
		int num2 = Math.Max(1, prototype.Height);
		PanelContainer val = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)2,
			MinWidth = num,
			MinHeight = num2,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#00000000", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#00000000", (Color?)null),
				BorderThickness = new Thickness(0f)
			}
		};
		LayoutContainer val2 = new LayoutContainer
		{
			MinWidth = num,
			MinHeight = num2
		};
		AnimatedTextureRect val3 = new AnimatedTextureRect
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		val3.SetFromSpriteSpecifier((SpriteSpecifier)new Rsi(prototype.BackgroundRsi, prototype.BackgroundState));
		val3.DisplayRect.Stretch = (StretchMode)7;
		LayoutContainer.SetAnchorAndMarginPreset((Control)(object)val3, (LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}
}
