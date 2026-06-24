using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgAnimatedRewardCard : Control
{
	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null);

	private static readonly Color PremiumGlow = Color.FromHex((ReadOnlySpan<char>)"#FF8C00", (Color?)null);

	private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>)"#00FFB3", (Color?)null);

	private bool _canClaim;

	private bool _isClaimed;

	private bool _isPremium;

	private bool _canClaimPremium;

	private bool _hovered;

	private Control? _content;

	[Dependency]
	private IGameTiming _timing;

	public bool CanClaim
	{
		get
		{
			return _canClaim;
		}
		set
		{
			_canClaim = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool IsClaimed
	{
		get
		{
			return _isClaimed;
		}
		set
		{
			_isClaimed = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool IsPremium
	{
		get
		{
			return _isPremium;
		}
		set
		{
			_isPremium = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool CanClaimPremium
	{
		get
		{
			return _canClaimPremium;
		}
		set
		{
			_canClaimPremium = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public event Action? OnClaimPressed;

	public PubgAnimatedRewardCard()
	{
		IoCManager.InjectDependencies<PubgAnimatedRewardCard>(this);
		((Control)this).MouseFilter = (MouseFilterMode)0;
	}

	public void SetContent(Control content)
	{
		Control? content2 = _content;
		if (content2 != null)
		{
			content2.Orphan();
		}
		_content = content;
		((Control)this).AddChild(content);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Draw(handle);
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		if (_canClaim && _canClaimPremium && !_isClaimed)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num = 0.25f + MathF.Sin((float)totalSeconds * 2.5f) * 0.15f;
			Color val2 = (_isPremium ? PremiumGlow : GreenSuccess);
			UIBox2 val3 = default(UIBox2);
			((UIBox2)(ref val3))._002Ector(val.Left - 3f, val.Top - 3f, val.Right + 3f, val.Bottom + 3f);
			handle.DrawRect(val3, ((Color)(ref val2)).WithAlpha(num), true);
			UIBox2 val4 = default(UIBox2);
			((UIBox2)(ref val4))._002Ector(val.Left - 1f, val.Top - 1f, val.Right + 1f, val.Bottom + 1f);
			handle.DrawRect(val4, ((Color)(ref val2)).WithAlpha(num * 1.5f), true);
		}
		if (_hovered && _canClaim && _canClaimPremium && !_isClaimed)
		{
			Color val5 = (_isPremium ? AccentGlow : GreenSuccess);
			UIBox2 val6 = default(UIBox2);
			((UIBox2)(ref val6))._002Ector(val.Left - 2f, val.Top - 2f, val.Right + 2f, val.Bottom + 2f);
			handle.DrawRect(val6, ((Color)(ref val5)).WithAlpha(0.3f), true);
		}
		if (_isClaimed)
		{
			UIBox2 val7 = default(UIBox2);
			((UIBox2)(ref val7))._002Ector(val.Left - 1f, val.Top - 1f, val.Right + 1f, val.Bottom + 1f);
			handle.DrawRect(val7, ((Color)(ref CompletedGlow)).WithAlpha(0.1f), true);
		}
	}

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		if (_canClaim && _canClaimPremium && !_isClaimed)
		{
			_hovered = true;
			((Control)this).UserInterfaceManager.HoverSound();
		}
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_hovered = false;
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindDown(args);
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) && _canClaim && _canClaimPremium && !_isClaimed)
		{
			((Control)this).UserInterfaceManager.ClickSound();
			this.OnClaimPressed?.Invoke();
			((BoundKeyEventArgs)args).Handle();
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_canClaim && _canClaimPremium && !_isClaimed)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
