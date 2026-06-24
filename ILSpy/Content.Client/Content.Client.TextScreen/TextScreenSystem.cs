using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.TextScreen;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.TextScreen;

public sealed class TextScreenSystem : VisualizerSystem<TextScreenVisualsComponent>
{
	[Dependency]
	private IGameTiming _gameTiming;

	private static readonly Dictionary<char, string> CharStatePairs = new Dictionary<char, string>
	{
		{ ':', "colon" },
		{ '!', "exclamation" },
		{ '?', "question" },
		{ '*', "star" },
		{ '+', "plus" },
		{ '-', "dash" },
		{ ' ', "blank" }
	};

	private const string DefaultState = "blank";

	private const string TextMapKey = "textMapKey";

	private const string TimerMapKey = "timerMapKey";

	private const string TextPath = "Effects/text.rsi";

	private const int CharWidth = 4;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TextScreenVisualsComponent, ComponentInit>((ComponentEventHandler<TextScreenVisualsComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TextScreenTimerComponent, ComponentInit>((ComponentEventHandler<TextScreenTimerComponent, ComponentInit>)OnTimerInit, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesOutsidePrediction = true;
	}

	private void OnInit(EntityUid uid, TextScreenVisualsComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			component.TextOffset = Vector2.Multiply(1f / 32f, component.TextOffset);
			component.TimerOffset = Vector2.Multiply(1f / 32f, component.TimerOffset);
			ResetText(uid, component, sprite);
			BuildTextLayers(uid, component, sprite);
		}
	}

	private void OnTimerInit(EntityUid uid, TextScreenTimerComponent timer, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		TextScreenVisualsComponent textScreenVisualsComponent = default(TextScreenVisualsComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && ((EntitySystem)this).TryComp<TextScreenVisualsComponent>(uid, ref textScreenVisualsComponent))
		{
			for (int i = 0; i < textScreenVisualsComponent.RowLength; i++)
			{
				base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, item)), "timerMapKey" + i);
				timer.LayerStatesToDraw.Add("timerMapKey" + i, null);
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, item)), "timerMapKey" + i, new ResPath("Effects/text.rsi"), (StateId?)null);
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), "timerMapKey" + i, textScreenVisualsComponent.Color);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), "timerMapKey" + i, StateId.op_Implicit("blank"));
			}
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, TextScreenVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref args.Sprite, true))
		{
			return;
		}
		if (args.AppearanceData.TryGetValue(TextScreenVisuals.Color, out var value) && value is Color)
		{
			component.Color = (Color)value;
		}
		if (args.AppearanceData.TryGetValue(TextScreenVisuals.DefaultText, out var value2) && value2 is string)
		{
			component.Text = SegmentText((string)value2, component);
		}
		if (args.AppearanceData.TryGetValue(TextScreenVisuals.ScreenText, out var value3) && value3 is string)
		{
			component.TextToDraw = SegmentText((string)value3, component);
			ResetText(uid, component);
			BuildTextLayers(uid, component, args.Sprite);
			DrawLayers(uid, component.LayerStatesToDraw);
		}
		if (args.AppearanceData.TryGetValue(TextScreenVisuals.TargetTime, out var value4) && value4 is TimeSpan timeSpan)
		{
			if (timeSpan > _gameTiming.CurTime)
			{
				TextScreenTimerComponent textScreenTimerComponent = ((EntitySystem)this).EnsureComp<TextScreenTimerComponent>(uid);
				textScreenTimerComponent.Target = timeSpan;
				BuildTimerLayers(uid, textScreenTimerComponent, component);
				DrawLayers(uid, textScreenTimerComponent.LayerStatesToDraw);
			}
			else
			{
				OnTimerFinish(uid, component);
			}
		}
	}

	private void OnTimerFinish(EntityUid uid, TextScreenVisualsComponent screen)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		screen.TextToDraw = screen.Text;
		TextScreenTimerComponent textScreenTimerComponent = default(TextScreenTimerComponent);
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<TextScreenTimerComponent>(uid, ref textScreenTimerComponent) || !((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			return;
		}
		foreach (string key in textScreenTimerComponent.LayerStatesToDraw.Keys)
		{
			base.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, val)), key, true);
		}
		((EntitySystem)this).RemComp<TextScreenTimerComponent>(uid);
		ResetText(uid, screen);
		BuildTextLayers(uid, screen, val);
		DrawLayers(uid, screen.LayerStatesToDraw);
	}

	private string?[] SegmentText(string text, TextScreenVisualsComponent component)
	{
		int rowLength = component.RowLength;
		string[] array = new string[Math.Min(component.Rows, (text.Length - 1) / rowLength + 1)];
		for (int i = 0; i < Math.Min(text.Length, rowLength * component.Rows); i += rowLength)
		{
			array[i / rowLength] = text.Substring(i, Math.Min(text.Length - i, rowLength)).Trim();
		}
		return array;
	}

	private void ResetText(EntityUid uid, TextScreenVisualsComponent component, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true))
		{
			return;
		}
		foreach (string key in component.LayerStatesToDraw.Keys)
		{
			base.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), key, true);
		}
		component.LayerStatesToDraw.Clear();
		for (int i = 0; i < component.Rows; i++)
		{
			for (int j = 0; j < component.RowLength; j++)
			{
				string text = "textMapKey" + i + j;
				base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text);
				component.LayerStatesToDraw.Add(text, null);
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text, new ResPath("Effects/text.rsi"), (StateId?)null);
				base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text, component.Color);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text, StateId.op_Implicit("blank"));
			}
		}
	}

	private void BuildTextLayers(EntityUid uid, TextScreenVisualsComponent component, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true))
		{
			return;
		}
		for (int i = 0; i < Math.Min(component.TextToDraw.Length, component.Rows); i++)
		{
			string text = component.TextToDraw[i];
			if (text != null)
			{
				int num = Math.Min(text.Length, component.RowLength);
				for (int j = 0; j < num; j++)
				{
					component.LayerStatesToDraw["textMapKey" + i + j] = GetStateFromChar(text[j]);
					base.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "textMapKey" + i + j, Vector2.Multiply(new Vector2(((float)j - (float)num / 2f + 0.5f) * 4f, -i * component.RowOffset), 1f / 32f) + component.TextOffset);
				}
			}
		}
	}

	private void BuildTimerLayers(EntityUid uid, TextScreenTimerComponent timer, TextScreenVisualsComponent screen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			string text = TimeToString((_gameTiming.CurTime - timer.Target).Duration(), getMilliseconds: false, screen.HourFormat, screen.MinuteFormat, screen.SecondFormat);
			int num = Math.Min(text.Length, screen.RowLength);
			for (int i = 0; i < num; i++)
			{
				timer.LayerStatesToDraw["timerMapKey" + i] = GetStateFromChar(text[i]);
				base.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), "timerMapKey" + i, Vector2.Multiply(new Vector2(((float)i - (float)num / 2f + 0.5f) * 4f, 0f), 1f / 32f) + screen.TimerOffset);
			}
		}
	}

	private void DrawLayers(EntityUid uid, Dictionary<string, string?> layerStates, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpriteComponent>(uid, ref sprite, true))
		{
			return;
		}
		foreach (var (text3, text4) in layerStates.Where<KeyValuePair<string, string>>((KeyValuePair<string, string> pairs) => pairs.Value != null))
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), text3, StateId.op_Implicit(text4));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<TextScreenTimerComponent, TextScreenVisualsComponent> val = ((EntitySystem)this).EntityQueryEnumerator<TextScreenTimerComponent, TextScreenVisualsComponent>();
		EntityUid uid = default(EntityUid);
		TextScreenTimerComponent textScreenTimerComponent = default(TextScreenTimerComponent);
		TextScreenVisualsComponent screen = default(TextScreenVisualsComponent);
		while (val.MoveNext(ref uid, ref textScreenTimerComponent, ref screen))
		{
			if (textScreenTimerComponent.Target < _gameTiming.CurTime)
			{
				OnTimerFinish(uid, screen);
				continue;
			}
			BuildTimerLayers(uid, textScreenTimerComponent, screen);
			DrawLayers(uid, textScreenTimerComponent.LayerStatesToDraw);
		}
	}

	public static string TimeToString(TimeSpan timeSpan, bool getMilliseconds = true, string hours = "D2", string minutes = "D2", string seconds = "D2", string cs = "D2")
	{
		string text;
		string text2;
		if (timeSpan.TotalHours >= 1.0)
		{
			text = timeSpan.Hours.ToString(hours);
			text2 = timeSpan.Minutes.ToString(minutes);
		}
		else if (timeSpan.TotalMinutes >= 1.0 || !getMilliseconds)
		{
			text = timeSpan.Minutes.ToString(minutes);
			text2 = timeSpan.Seconds.ToString(seconds);
		}
		else
		{
			text = timeSpan.Seconds.ToString(seconds);
			text2 = (timeSpan.Milliseconds / 10).ToString(cs);
		}
		return text + ":" + text2;
	}

	public static string? GetStateFromChar(char? character)
	{
		if (!character.HasValue)
		{
			return null;
		}
		if (CharStatePairs.TryGetValue(character.Value, out string value))
		{
			return value;
		}
		if (char.IsLetterOrDigit(character.Value))
		{
			return character.Value.ToString().ToLower();
		}
		return null;
	}
}
