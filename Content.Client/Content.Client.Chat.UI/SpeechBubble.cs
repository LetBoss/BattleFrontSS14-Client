using System;
using System.Numerics;
using System.Threading;
using Content.Shared.Chat;
using Content.Shared.Speech;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Chat.UI;

public abstract class SpeechBubble : Control
{
	public enum SpeechType : byte
	{
		Emote,
		Say,
		Whisper,
		Looc
	}

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	protected IConfigurationManager ConfigManager;

	private readonly SharedTransformSystem _transformSystem;

	private static readonly TimeSpan TotalTime = TimeSpan.FromSeconds(4L);

	private static readonly TimeSpan FadeTime = TimeSpan.FromSeconds(0.25);

	private const float EntityVerticalOffset = 0.5f;

	public const float SpeechMaxWidth = 256f;

	private readonly EntityUid _senderEntity;

	private TimeSpan _deathTime;

	private float _verticalOffsetAchieved;

	public float VerticalOffset { get; set; }

	public Vector2 ContentSize { get; private set; }

	public event Action<EntityUid, SpeechBubble>? OnDied;

	public static SpeechBubble CreateSpeechBubble(SpeechType type, ChatMessage message, EntityUid senderEntity)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		return type switch
		{
			SpeechType.Emote => new TextSpeechBubble(message, senderEntity, "emoteBox"), 
			SpeechType.Say => new FancyTextSpeechBubble(message, senderEntity, "sayBox"), 
			SpeechType.Whisper => new FancyTextSpeechBubble(message, senderEntity, "whisperBox"), 
			SpeechType.Looc => new TextSpeechBubble(message, senderEntity, "emoteBox", Color.FromHex((ReadOnlySpan<char>)"#48d1cc", (Color?)null)), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public SpeechBubble(ChatMessage message, EntityUid senderEntity, string speechStyleClass, Color? fontColor = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<SpeechBubble>(this);
		_senderEntity = senderEntity;
		_transformSystem = _entityManager.System<SharedTransformSystem>();
		((Control)this).RectClipContent = true;
		Control val = BuildBubble(message, speechStyleClass, fontColor);
		((Control)this).AddChild(val);
		((Control)this).ForceRunStyleUpdate();
		val.Measure(Vector2Helpers.Infinity);
		ContentSize = val.DesiredSize;
		_verticalOffsetAchieved = 0f - ContentSize.Y;
		_deathTime = _timing.RealTime + TotalTime;
	}

	protected abstract Control BuildBubble(ChatMessage message, string speechStyleClass, Color? fontColor = null);

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		float num = (float)(_deathTime - _timing.RealTime).TotalSeconds;
		if (_entityManager.Deleted(_senderEntity) || num <= 0f)
		{
			Timer.Spawn(0, (Action)Die, default(CancellationToken));
			return;
		}
		if (MathHelper.CloseToPercent(_verticalOffsetAchieved - VerticalOffset, 0f, 0.1))
		{
			_verticalOffsetAchieved = VerticalOffset;
		}
		else
		{
			_verticalOffsetAchieved = MathHelper.Lerp(_verticalOffsetAchieved, VerticalOffset, 10f * ((FrameEventArgs)(ref args)).DeltaSeconds);
		}
		TransformComponent val = default(TransformComponent);
		Color white;
		if (!_entityManager.TryGetComponent<TransformComponent>(_senderEntity, ref val) || val.MapID != _eyeManager.CurrentEye.Position.MapId)
		{
			white = Color.White;
			((Control)this).Modulate = ((Color)(ref white)).WithAlpha((byte)0);
			return;
		}
		if ((double)num <= FadeTime.TotalSeconds)
		{
			white = Color.White;
			((Control)this).Modulate = ((Color)(ref white)).WithAlpha(num / (float)FadeTime.TotalSeconds);
		}
		else
		{
			((Control)this).Modulate = Color.White;
		}
		float num2 = 0f;
		SpeechComponent speechComponent = default(SpeechComponent);
		if (_entityManager.TryGetComponent<SpeechComponent>(_senderEntity, ref speechComponent))
		{
			num2 = speechComponent.SpeechBubbleOffset;
		}
		Angle val2 = -_eyeManager.CurrentEye.Rotation;
		Vector2 vector = ((Angle)(ref val2)).ToWorldVec() * (0f - (0.5f + num2));
		Vector2 vector2 = _transformSystem.GetWorldPosition(val) + vector;
		Vector2 vector3 = _eyeManager.WorldToScreen(vector2) / ((Control)this).UIScale;
		Vector2 vector4 = vector3 - new Vector2(ContentSize.X / 2f, ContentSize.Y + _verticalOffsetAchieved);
		vector4 = Vector2Helpers.Rounded(vector4 * 2f) / 2f;
		LayoutContainer.SetPosition((Control)(object)this, vector4);
		float setHeight = MathF.Ceiling(MathHelper.Clamp(vector3.Y - vector4.Y, 0f, ContentSize.Y));
		((Control)this).SetHeight = setHeight;
	}

	private void Die()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!((Control)this).Disposed)
		{
			this.OnDied?.Invoke(_senderEntity, this);
		}
	}

	public void FadeNow()
	{
		if (_deathTime > _timing.RealTime)
		{
			_deathTime = _timing.RealTime + FadeTime;
		}
	}

	protected FormattedMessage FormatSpeech(string message, Color? fontColor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage val = new FormattedMessage();
		if (fontColor.HasValue)
		{
			val.PushColor(fontColor.Value);
		}
		val.AddMarkupOrThrow(message);
		return val;
	}

	protected FormattedMessage ExtractAndFormatSpeechSubstring(ChatMessage message, string tag, Color? fontColor = null)
	{
		return FormatSpeech(SharedChatSystem.GetStringInsideTag(message, tag), fontColor);
	}
}
