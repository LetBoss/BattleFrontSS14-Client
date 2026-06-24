// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.UI.SpeechBubble
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Content.Shared.Speech;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;
using System.Threading;

#nullable enable
namespace Content.Client.Chat.UI;

public abstract class SpeechBubble : Control
{
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

  public static SpeechBubble CreateSpeechBubble(
    SpeechBubble.SpeechType type,
    ChatMessage message,
    EntityUid senderEntity)
  {
    switch (type)
    {
      case SpeechBubble.SpeechType.Emote:
        return (SpeechBubble) new TextSpeechBubble(message, senderEntity, "emoteBox");
      case SpeechBubble.SpeechType.Say:
        return (SpeechBubble) new FancyTextSpeechBubble(message, senderEntity, "sayBox");
      case SpeechBubble.SpeechType.Whisper:
        return (SpeechBubble) new FancyTextSpeechBubble(message, senderEntity, "whisperBox");
      case SpeechBubble.SpeechType.Looc:
        return (SpeechBubble) new TextSpeechBubble(message, senderEntity, "emoteBox", new Color?(Color.FromHex((ReadOnlySpan<char>) "#48d1cc", new Color?())));
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  public SpeechBubble(
    ChatMessage message,
    EntityUid senderEntity,
    string speechStyleClass,
    Color? fontColor = null)
  {
    IoCManager.InjectDependencies<SpeechBubble>(this);
    this._senderEntity = senderEntity;
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
    this.RectClipContent = true;
    Control control = this.BuildBubble(message, speechStyleClass, fontColor);
    this.AddChild(control);
    this.ForceRunStyleUpdate();
    control.Measure(Vector2Helpers.Infinity);
    this.ContentSize = control.DesiredSize;
    this._verticalOffsetAchieved = -this.ContentSize.Y;
    this._deathTime = this._timing.RealTime + SpeechBubble.TotalTime;
  }

  protected abstract Control BuildBubble(
    ChatMessage message,
    string speechStyleClass,
    Color? fontColor = null);

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    float totalSeconds = (float) (this._deathTime - this._timing.RealTime).TotalSeconds;
    if (this._entityManager.Deleted(this._senderEntity) || (double) totalSeconds <= 0.0)
    {
      Timer.Spawn(0, new Action(this.Die), new CancellationToken());
    }
    else
    {
      this._verticalOffsetAchieved = !MathHelper.CloseToPercent(this._verticalOffsetAchieved - this.VerticalOffset, 0.0f, 0.1) ? MathHelper.Lerp(this._verticalOffsetAchieved, this.VerticalOffset, 10f * ((FrameEventArgs) ref args).DeltaSeconds) : this.VerticalOffset;
      TransformComponent transformComponent;
      if (!this._entityManager.TryGetComponent<TransformComponent>(this._senderEntity, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, this._eyeManager.CurrentEye.Position.MapId))
      {
        Color white = Color.White;
        this.Modulate = ((Color) ref white).WithAlpha((byte) 0);
      }
      else
      {
        if ((double) totalSeconds <= SpeechBubble.FadeTime.TotalSeconds)
        {
          Color white = Color.White;
          this.Modulate = ((Color) ref white).WithAlpha(totalSeconds / (float) SpeechBubble.FadeTime.TotalSeconds);
        }
        else
          this.Modulate = Color.White;
        float num = 0.0f;
        SpeechComponent speechComponent;
        if (this._entityManager.TryGetComponent<SpeechComponent>(this._senderEntity, ref speechComponent))
          num = speechComponent.SpeechBubbleOffset;
        Angle angle = Angle.op_UnaryNegation(this._eyeManager.CurrentEye.Rotation);
        Vector2 vector2_1 = ((Angle) ref angle).ToWorldVec() * (float) -(0.5 + (double) num);
        Vector2 vector2_2 = this._eyeManager.WorldToScreen(this._transformSystem.GetWorldPosition(transformComponent) + vector2_1) / this.UIScale;
        Vector2 vector2_3 = Vector2Helpers.Rounded((vector2_2 - new Vector2(this.ContentSize.X / 2f, this.ContentSize.Y + this._verticalOffsetAchieved)) * 2f) / 2f;
        LayoutContainer.SetPosition((Control) this, vector2_3);
        this.SetHeight = MathF.Ceiling(MathHelper.Clamp(vector2_2.Y - vector2_3.Y, 0.0f, this.ContentSize.Y));
      }
    }
  }

  private void Die()
  {
    if (this.Disposed)
      return;
    Action<EntityUid, SpeechBubble> onDied = this.OnDied;
    if (onDied == null)
      return;
    onDied(this._senderEntity, this);
  }

  public void FadeNow()
  {
    if (!(this._deathTime > this._timing.RealTime))
      return;
    this._deathTime = this._timing.RealTime + SpeechBubble.FadeTime;
  }

  protected FormattedMessage FormatSpeech(string message, Color? fontColor = null)
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    if (fontColor.HasValue)
      formattedMessage.PushColor(fontColor.Value);
    formattedMessage.AddMarkupOrThrow(message);
    return formattedMessage;
  }

  protected FormattedMessage ExtractAndFormatSpeechSubstring(
    ChatMessage message,
    string tag,
    Color? fontColor = null)
  {
    return this.FormatSpeech(SharedChatSystem.GetStringInsideTag(message, tag), fontColor);
  }

  public enum SpeechType : byte
  {
    Emote,
    Say,
    Whisper,
    Looc,
  }
}
