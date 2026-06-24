// Decompiled with JetBrains decompiler
// Type: Content.Client.TextScreen.TextScreenSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.TextScreen;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.TextScreen;

public sealed class TextScreenSystem : VisualizerSystem<TextScreenVisualsComponent>
{
  [Dependency]
  private IGameTiming _gameTiming;
  private static readonly Dictionary<char, string> CharStatePairs = new Dictionary<char, string>()
  {
    {
      ':',
      "colon"
    },
    {
      '!',
      "exclamation"
    },
    {
      '?',
      "question"
    },
    {
      '*',
      "star"
    },
    {
      '+',
      "plus"
    },
    {
      '-',
      "dash"
    },
    {
      ' ',
      "blank"
    }
  };
  private const string DefaultState = "blank";
  private const string TextMapKey = "textMapKey";
  private const string TimerMapKey = "timerMapKey";
  private const string TextPath = "Effects/text.rsi";
  private const int CharWidth = 4;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<TextScreenVisualsComponent, ComponentInit>(new ComponentEventHandler<TextScreenVisualsComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<TextScreenTimerComponent, ComponentInit>(new ComponentEventHandler<TextScreenTimerComponent, ComponentInit>((object) this, __methodptr(OnTimerInit)), (Type[]) null, (Type[]) null);
    ((EntitySystem) this).UpdatesOutsidePrediction = true;
  }

  private void OnInit(EntityUid uid, TextScreenVisualsComponent component, ComponentInit args)
  {
    SpriteComponent sprite;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref sprite))
      return;
    component.TextOffset = Vector2.Multiply(1f / 32f, component.TextOffset);
    component.TimerOffset = Vector2.Multiply(1f / 32f, component.TimerOffset);
    this.ResetText(uid, component, sprite);
    this.BuildTextLayers(uid, component, sprite);
  }

  private void OnTimerInit(EntityUid uid, TextScreenTimerComponent timer, ComponentInit args)
  {
    SpriteComponent spriteComponent;
    TextScreenVisualsComponent visualsComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent) || !((EntitySystem) this).TryComp<TextScreenVisualsComponent>(uid, ref visualsComponent))
      return;
    for (int index = 0; index < visualsComponent.RowLength; ++index)
    {
      this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), "timerMapKey" + index.ToString());
      timer.LayerStatesToDraw.Add("timerMapKey" + index.ToString(), (string) null);
      this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), "timerMapKey" + index.ToString(), new ResPath("Effects/text.rsi"), new RSI.StateId?());
      this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), "timerMapKey" + index.ToString(), visualsComponent.Color);
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), "timerMapKey" + index.ToString(), RSI.StateId.op_Implicit("blank"));
    }
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    TextScreenVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (!((EntitySystem) this).Resolve<SpriteComponent>(uid, ref args.Sprite, true))
      return;
    object obj1;
    if (args.AppearanceData.TryGetValue((Enum) TextScreenVisuals.Color, out obj1) && obj1 is Color)
      component.Color = (Color) obj1;
    object text1;
    if (args.AppearanceData.TryGetValue((Enum) TextScreenVisuals.DefaultText, out text1) && text1 is string)
      component.Text = this.SegmentText((string) text1, component);
    object text2;
    if (args.AppearanceData.TryGetValue((Enum) TextScreenVisuals.ScreenText, out text2) && text2 is string)
    {
      component.TextToDraw = this.SegmentText((string) text2, component);
      this.ResetText(uid, component);
      this.BuildTextLayers(uid, component, args.Sprite);
      this.DrawLayers(uid, component.LayerStatesToDraw);
    }
    object obj2;
    if (!args.AppearanceData.TryGetValue((Enum) TextScreenVisuals.TargetTime, out obj2) || !(obj2 is TimeSpan timeSpan))
      return;
    if (timeSpan > this._gameTiming.CurTime)
    {
      TextScreenTimerComponent timer = ((EntitySystem) this).EnsureComp<TextScreenTimerComponent>(uid);
      timer.Target = timeSpan;
      this.BuildTimerLayers(uid, timer, component);
      this.DrawLayers(uid, timer.LayerStatesToDraw);
    }
    else
      this.OnTimerFinish(uid, component);
  }

  private void OnTimerFinish(EntityUid uid, TextScreenVisualsComponent screen)
  {
    screen.TextToDraw = screen.Text;
    TextScreenTimerComponent screenTimerComponent;
    SpriteComponent sprite;
    if (!((EntitySystem) this).TryComp<TextScreenTimerComponent>(uid, ref screenTimerComponent) || !((EntitySystem) this).TryComp<SpriteComponent>(uid, ref sprite))
      return;
    foreach (string key in screenTimerComponent.LayerStatesToDraw.Keys)
      this.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, true);
    ((EntitySystem) this).RemComp<TextScreenTimerComponent>(uid);
    this.ResetText(uid, screen);
    this.BuildTextLayers(uid, screen, sprite);
    this.DrawLayers(uid, screen.LayerStatesToDraw);
  }

  private string?[] SegmentText(string text, TextScreenVisualsComponent component)
  {
    int rowLength = component.RowLength;
    string[] strArray = new string[Math.Min(component.Rows, (text.Length - 1) / rowLength + 1)];
    for (int startIndex = 0; startIndex < Math.Min(text.Length, rowLength * component.Rows); startIndex += rowLength)
      strArray[startIndex / rowLength] = text.Substring(startIndex, Math.Min(text.Length - startIndex, rowLength)).Trim();
    return strArray;
  }

  private void ResetText(
    EntityUid uid,
    TextScreenVisualsComponent component,
    SpriteComponent? sprite = null)
  {
    if (!((EntitySystem) this).Resolve<SpriteComponent>(uid, ref sprite, true))
      return;
    foreach (string key in component.LayerStatesToDraw.Keys)
      this.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, true);
    component.LayerStatesToDraw.Clear();
    for (int index1 = 0; index1 < component.Rows; ++index1)
    {
      for (int index2 = 0; index2 < component.RowLength; ++index2)
      {
        string key = $"textMapKey{index1.ToString()}{index2.ToString()}";
        this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key);
        component.LayerStatesToDraw.Add(key, (string) null);
        this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, new ResPath("Effects/text.rsi"), new RSI.StateId?());
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, component.Color);
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, RSI.StateId.op_Implicit("blank"));
      }
    }
  }

  private void BuildTextLayers(
    EntityUid uid,
    TextScreenVisualsComponent component,
    SpriteComponent? sprite = null)
  {
    if (!((EntitySystem) this).Resolve<SpriteComponent>(uid, ref sprite, true))
      return;
    for (int index1 = 0; index1 < Math.Min(component.TextToDraw.Length, component.Rows); ++index1)
    {
      string str = component.TextToDraw[index1];
      if (str != null)
      {
        int num = Math.Min(str.Length, component.RowLength);
        for (int index2 = 0; index2 < num; ++index2)
        {
          component.LayerStatesToDraw[$"textMapKey{index1.ToString()}{index2.ToString()}"] = TextScreenSystem.GetStateFromChar(new char?(str[index2]));
          this.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, sprite)), $"textMapKey{index1.ToString()}{index2.ToString()}", Vector2.Multiply(new Vector2((float) (((double) index2 - (double) num / 2.0 + 0.5) * 4.0), (float) (-index1 * component.RowOffset)), 1f / 32f) + component.TextOffset);
        }
      }
    }
  }

  private void BuildTimerLayers(
    EntityUid uid,
    TextScreenTimerComponent timer,
    TextScreenVisualsComponent screen)
  {
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    string str = TextScreenSystem.TimeToString((this._gameTiming.CurTime - timer.Target).Duration(), false, screen.HourFormat, screen.MinuteFormat, screen.SecondFormat);
    int num = Math.Min(str.Length, screen.RowLength);
    for (int index = 0; index < num; ++index)
    {
      timer.LayerStatesToDraw["timerMapKey" + index.ToString()] = TextScreenSystem.GetStateFromChar(new char?(str[index]));
      this.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), "timerMapKey" + index.ToString(), Vector2.Multiply(new Vector2((float) (((double) index - (double) num / 2.0 + 0.5) * 4.0), 0.0f), 1f / 32f) + screen.TimerOffset);
    }
  }

  private void DrawLayers(
    EntityUid uid,
    Dictionary<string, string?> layerStates,
    SpriteComponent? sprite = null)
  {
    if (!((EntitySystem) this).Resolve<SpriteComponent>(uid, ref sprite, true))
      return;
    foreach ((string key, string str) in layerStates.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pairs => pairs.Value != null)))
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, RSI.StateId.op_Implicit(str));
  }

  public virtual void Update(float frameTime)
  {
    ((EntitySystem) this).Update(frameTime);
    EntityQueryEnumerator<TextScreenTimerComponent, TextScreenVisualsComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<TextScreenTimerComponent, TextScreenVisualsComponent>();
    EntityUid uid;
    TextScreenTimerComponent timer;
    TextScreenVisualsComponent screen;
    while (entityQueryEnumerator.MoveNext(ref uid, ref timer, ref screen))
    {
      if (timer.Target < this._gameTiming.CurTime)
      {
        this.OnTimerFinish(uid, screen);
      }
      else
      {
        this.BuildTimerLayers(uid, timer, screen);
        this.DrawLayers(uid, timer.LayerStatesToDraw);
      }
    }
  }

  public static string TimeToString(
    TimeSpan timeSpan,
    bool getMilliseconds = true,
    string hours = "D2",
    string minutes = "D2",
    string seconds = "D2",
    string cs = "D2")
  {
    string str1;
    string str2;
    if (timeSpan.TotalHours >= 1.0)
    {
      int num = timeSpan.Hours;
      str1 = num.ToString(hours);
      num = timeSpan.Minutes;
      str2 = num.ToString(minutes);
    }
    else if (timeSpan.TotalMinutes >= 1.0 || !getMilliseconds)
    {
      int num = timeSpan.Minutes;
      str1 = num.ToString(minutes);
      num = timeSpan.Seconds;
      str2 = num.ToString(seconds);
    }
    else
    {
      str1 = timeSpan.Seconds.ToString(seconds);
      str2 = (timeSpan.Milliseconds / 10).ToString(cs);
    }
    return $"{str1}:{str2}";
  }

  public static string? GetStateFromChar(char? character)
  {
    if (!character.HasValue)
      return (string) null;
    string stateFromChar;
    if (TextScreenSystem.CharStatePairs.TryGetValue(character.Value, out stateFromChar))
      return stateFromChar;
    return char.IsLetterOrDigit(character.Value) ? character.Value.ToString().ToLower() : (string) null;
  }
}
