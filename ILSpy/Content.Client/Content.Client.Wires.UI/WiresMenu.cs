using System;
using System.Numerics;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Wires;
using Robust.Client.Animations;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Animations;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Wires.UI;

public sealed class WiresMenu : BaseWindow
{
	private sealed class WireControl : Control
	{
		private sealed class WireRender : Control
		{
			private readonly WireColor _color;

			private readonly bool _isCut;

			private readonly bool _flip;

			private readonly bool _mirror;

			private readonly int _type;

			private static readonly string[] TextureNormal = new string[2] { "/Textures/Interface/WireHacking/wire_1.svg.96dpi.png", "/Textures/Interface/WireHacking/wire_2.svg.96dpi.png" };

			private static readonly string[] TextureCut = new string[2] { "/Textures/Interface/WireHacking/wire_1_cut.svg.96dpi.png", "/Textures/Interface/WireHacking/wire_2_cut.svg.96dpi.png" };

			private static readonly string[] TextureCopper = new string[2] { "/Textures/Interface/WireHacking/wire_1_copper.svg.96dpi.png", "/Textures/Interface/WireHacking/wire_2_copper.svg.96dpi.png" };

			private readonly IResourceCache _resourceCache;

			public WireRender(WireColor color, bool isCut, bool flip, bool mirror, int type, IResourceCache resourceCache)
			{
				_resourceCache = resourceCache;
				_color = color;
				_isCut = isCut;
				_flip = flip;
				_mirror = mirror;
				_type = type;
				((Control)this).SetSize = new Vector2(16f, 50f);
			}

			protected override void Draw(DrawingHandleScreen handle)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				Color value = _color.ColorValue();
				Texture texture = _resourceCache.GetTexture(_isCut ? TextureCut[_type] : TextureNormal[_type]);
				float num = 0f;
				float num2 = (float)texture.Width + num;
				float num3 = 0f;
				float num4 = (float)texture.Height + num3;
				if (_flip)
				{
					float num5 = num4;
					num4 = num3;
					num3 = num5;
				}
				if (_mirror)
				{
					float num6 = num2;
					num2 = num;
					num = num6;
				}
				num *= ((Control)this).UIScale;
				num2 *= ((Control)this).UIScale;
				num3 *= ((Control)this).UIScale;
				num4 *= ((Control)this).UIScale;
				UIBox2 val = default(UIBox2);
				((UIBox2)(ref val))._002Ector(num, num3, num2, num4);
				if (_isCut)
				{
					Color orange = Color.Orange;
					Texture texture2 = _resourceCache.GetTexture(TextureCopper[_type]);
					handle.DrawTextureRect(texture2, val, (Color?)orange);
				}
				handle.DrawTextureRect(texture, val, (Color?)value);
			}
		}

		private IResourceCache _resourceCache;

		private const string TextureContact = "/Textures/Interface/WireHacking/contact.svg.96dpi.png";

		public event Action? WireClicked;

		public event Action? ContactsClicked;

		public WireControl(WireColor color, WireLetter letter, bool isCut, bool flip, bool mirror, int type, IResourceCache resourceCache)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			_resourceCache = resourceCache;
			((Control)this).HorizontalAlignment = (HAlignment)2;
			((Control)this).MouseFilter = (MouseFilterMode)0;
			LayoutContainer val = new LayoutContainer();
			((Control)this).AddChild((Control)(object)val);
			Label val2 = new Label
			{
				Text = letter.Letter().ToString(),
				VerticalAlignment = (VAlignment)3,
				HorizontalAlignment = (HAlignment)2,
				Align = (AlignMode)1,
				FontOverride = _resourceCache.GetFont("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 12),
				FontColorOverride = Color.Gray,
				ToolTip = letter.Name(),
				MouseFilter = (MouseFilterMode)0
			};
			((Control)val).AddChild((Control)(object)val2);
			LayoutContainer.SetAnchorPreset((Control)(object)val2, (LayoutPreset)12, false);
			LayoutContainer.SetGrowVertical((Control)(object)val2, (GrowDirection)1);
			LayoutContainer.SetGrowHorizontal((Control)(object)val2, (GrowDirection)2);
			Texture texture = _resourceCache.GetTexture("/Textures/Interface/WireHacking/contact.svg.96dpi.png");
			TextureRect val3 = new TextureRect
			{
				Texture = texture,
				Modulate = Color.FromHex((ReadOnlySpan<char>)"#E1CA76", (Color?)null)
			};
			((Control)val).AddChild((Control)(object)val3);
			LayoutContainer.SetPosition((Control)(object)val3, new Vector2(0f, 0f));
			TextureRect val4 = new TextureRect
			{
				Texture = texture,
				Modulate = Color.FromHex((ReadOnlySpan<char>)"#E1CA76", (Color?)null)
			};
			((Control)val).AddChild((Control)(object)val4);
			LayoutContainer.SetPosition((Control)(object)val4, new Vector2(0f, 60f));
			WireRender wireRender = new WireRender(color, isCut, flip, mirror, type, _resourceCache);
			((Control)val).AddChild((Control)(object)wireRender);
			LayoutContainer.SetPosition((Control)(object)wireRender, new Vector2(2f, 16f));
			((Control)this).ToolTip = color.Name();
			((Control)this).MinSize = new Vector2(20f, 102f);
		}

		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			((Control)this).KeyBindDown(args);
			if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
			{
				if (args.RelativePosition.Y > 20f && args.RelativePosition.Y < 60f)
				{
					this.WireClicked?.Invoke();
				}
				else
				{
					this.ContactsClicked?.Invoke();
				}
			}
		}

		protected override bool HasPoint(Vector2 point)
		{
			if (((Control)this).HasPoint(point))
			{
				return point.Y <= 80f;
			}
			return false;
		}
	}

	private sealed class StatusLight : Control
	{
		private static readonly Animation _blinkingFast = new Animation
		{
			Length = TimeSpan.FromSeconds(0.2),
			AnimationTracks = { (AnimationTrack)new AnimationTrackControlProperty
			{
				Property = "Modulate",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)Color.White, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.Transparent, 0.1f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.White, 0.1f, (Func<float, float>)null)
				}
			} }
		};

		private static readonly Animation _blinkingSlow = new Animation
		{
			Length = TimeSpan.FromSeconds(0.8),
			AnimationTracks = { (AnimationTrack)new AnimationTrackControlProperty
			{
				Property = "Modulate",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)Color.White, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.White, 0.3f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.Transparent, 0.1f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.Transparent, 0.3f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)Color.White, 0.1f, (Func<float, float>)null)
				}
			} }
		};

		public StatusLight(StatusLightData data, IResourceCache resourceCache)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected O, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00d1: Expected O, but got Unknown
			//IL_00d9: Expected O, but got Unknown
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Expected O, but got Unknown
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Expected O, but got Unknown
			((Control)this).HorizontalAlignment = (HAlignment)3;
			Vector4 vector = Color.ToHsv(data.Color);
			vector.Z /= 2f;
			Color value = Color.FromHsv(vector);
			Control val = new Control
			{
				SetSize = new Vector2(20f, 20f)
			};
			val.Children.Add((Control)new TextureRect
			{
				Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_off_base.svg.96dpi.png"),
				Stretch = (StretchMode)4,
				ModulateSelfOverride = value
			});
			OrderedChildCollection children = val.Children;
			TextureRect val2 = new TextureRect();
			Color color = data.Color;
			((Control)val2).ModulateSelfOverride = ((Color)(ref color)).WithAlpha(0.4f);
			val2.Stretch = (StretchMode)4;
			val2.Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_on_base.svg.96dpi.png");
			TextureRect val3 = val2;
			TextureRect activeLight = val2;
			children.Add((Control)(object)val3);
			Control val4 = val;
			Animation animation = null;
			switch (data.State)
			{
			case StatusLightState.Off:
				((Control)activeLight).Visible = false;
				break;
			case StatusLightState.BlinkingFast:
				animation = _blinkingFast;
				break;
			case StatusLightState.BlinkingSlow:
				animation = _blinkingSlow;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case StatusLightState.On:
				break;
			}
			if (animation != null)
			{
				((Control)activeLight).PlayAnimation(animation, "blink");
				TextureRect obj = activeLight;
				((Control)obj).AnimationCompleted = (Action<string>)Delegate.Combine(((Control)obj).AnimationCompleted, (Action<string>)delegate(string s)
				{
					if (s == "blink")
					{
						((Control)activeLight).PlayAnimation(animation, s);
					}
				});
			}
			Font font = resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 12);
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 4
			};
			((Control)val5).AddChild((Control)new Label
			{
				Text = data.Text,
				FontOverride = font,
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#A1A6AE", (Color?)null),
				VerticalAlignment = (VAlignment)2
			});
			((Control)val5).AddChild(val4);
			((Control)val5).AddChild(new Control
			{
				MinSize = new Vector2(6f, 0f)
			});
			((Control)this).AddChild((Control)(object)val5);
		}
	}

	private sealed class HelpPopup : Popup
	{
		public HelpPopup()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			RichTextLabel val = new RichTextLabel();
			val.SetMessage(Loc.GetString("wires-menu-help-popup"), (Color?)null);
			PanelContainer val2 = new PanelContainer
			{
				StyleClasses = { "entity-tooltip" }
			};
			((Control)val2).Children.Add((Control)(object)val);
			((Control)this).AddChild((Control)val2);
		}
	}

	[Dependency]
	private IResourceCache _resourceCache;

	private readonly Control _wiresHBox;

	private readonly Control _topContainer;

	private readonly Control _statusContainer;

	private readonly Label _nameLabel;

	private readonly Label _serialLabel;

	public TextureButton CloseButton { get; set; }

	public event Action<int, WiresAction>? OnAction;

	public WiresMenu()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_010c: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0159: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a8: Expected O, but got Unknown
		//IL_01aa: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Expected O, but got Unknown
		//IL_0309: Expected O, but got Unknown
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Expected O, but got Unknown
		//IL_0337: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		//IL_03df: Expected O, but got Unknown
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Expected O, but got Unknown
		//IL_0456: Expected O, but got Unknown
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Expected O, but got Unknown
		//IL_049a: Expected O, but got Unknown
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Expected O, but got Unknown
		//IL_04c5: Expected O, but got Unknown
		//IL_04ce: Expected O, but got Unknown
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Expected O, but got Unknown
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Expected O, but got Unknown
		//IL_0552: Expected O, but got Unknown
		//IL_055e: Expected O, but got Unknown
		//IL_0560: Expected O, but got Unknown
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Expected O, but got Unknown
		//IL_05bc: Expected O, but got Unknown
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Expected O, but got Unknown
		//IL_0618: Expected O, but got Unknown
		IoCManager.InjectDependencies<WiresMenu>(this);
		LayoutContainer val = new LayoutContainer
		{
			Name = "WireRoot"
		};
		((Control)this).AddChild((Control)(object)val);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		Texture texture = _resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
		StyleBoxTexture val2 = new StyleBoxTexture
		{
			Texture = texture,
			Modulate = Color.FromHex((ReadOnlySpan<char>)"#25252A", (Color?)null)
		};
		val2.SetPatchMargin((Margin)15, 10f);
		PanelContainer val3 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val2,
			MouseFilter = (MouseFilterMode)1
		};
		LayoutContainer val4 = new LayoutContainer
		{
			Name = "BottomWrap"
		};
		PanelContainer val5 = new PanelContainer
		{
			PanelOverride = (StyleBox)(object)val2,
			MouseFilter = (MouseFilterMode)1
		};
		BoxContainer val6 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)val6).Children.Add((Control)new PanelContainer
		{
			MinSize = new Vector2(2f, 0f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		OrderedChildCollection children = ((Control)val6).Children;
		PanelContainer val7 = new PanelContainer
		{
			HorizontalExpand = true,
			MouseFilter = (MouseFilterMode)0,
			Name = "Shadow"
		};
		StyleBoxFlat val8 = new StyleBoxFlat();
		Color black = Color.Black;
		val8.BackgroundColor = ((Color)(ref black)).WithAlpha(0.5f);
		val7.PanelOverride = (StyleBox)val8;
		children.Add((Control)val7);
		((Control)val6).Children.Add((Control)new PanelContainer
		{
			MinSize = new Vector2(2f, 0f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		BoxContainer val9 = val6;
		BoxContainer val10 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		_wiresHBox = (Control)new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 4,
			VerticalAlignment = (VAlignment)3
		};
		((Control)val10).AddChild(new Control
		{
			MinSize = new Vector2(20f, 0f)
		});
		((Control)val10).AddChild(_wiresHBox);
		((Control)val10).AddChild(new Control
		{
			MinSize = new Vector2(20f, 0f)
		});
		((Control)val4).AddChild((Control)(object)val5);
		LayoutContainer.SetAnchorPreset((Control)(object)val5, (LayoutPreset)12, false);
		LayoutContainer.SetMarginTop((Control)(object)val5, -55f);
		((Control)val4).AddChild((Control)(object)val9);
		LayoutContainer.SetAnchorPreset((Control)(object)val9, (LayoutPreset)12, false);
		LayoutContainer.SetMarginBottom((Control)(object)val9, -55f);
		LayoutContainer.SetMarginTop((Control)(object)val9, -80f);
		LayoutContainer.SetMarginLeft((Control)(object)val9, 12f);
		LayoutContainer.SetMarginRight((Control)(object)val9, -12f);
		((Control)val4).AddChild((Control)(object)val10);
		LayoutContainer.SetAnchorPreset((Control)(object)val10, (LayoutPreset)15, false);
		LayoutContainer.SetMarginBottom((Control)(object)val10, -4f);
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val4);
		LayoutContainer.SetAnchorPreset((Control)(object)val3, (LayoutPreset)15, false);
		LayoutContainer.SetMarginBottom((Control)(object)val3, -80f);
		LayoutContainer.SetAnchorPreset((Control)(object)val4, (LayoutPreset)13, false);
		LayoutContainer.SetGrowHorizontal((Control)(object)val4, (GrowDirection)2);
		BoxContainer val11 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		OrderedChildCollection children2 = ((Control)val11).Children;
		BoxContainer val12 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		Control val13 = (Control)val12;
		_topContainer = (Control)val12;
		children2.Add(val13);
		((Control)val11).Children.Add(new Control
		{
			MinSize = new Vector2(0f, 110f)
		});
		BoxContainer val14 = val11;
		((Control)val).AddChild((Control)(object)val14);
		LayoutContainer.SetAnchorPreset((Control)(object)val14, (LayoutPreset)15, false);
		Font font = _resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
		Font font2 = _resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 10);
		BoxContainer val15 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(4f, 2f, 12f, 2f)
		};
		OrderedChildCollection children3 = ((Control)val15).Children;
		Label val16 = new Label
		{
			Text = Loc.GetString("wires-menu-name-label"),
			FontOverride = font,
			FontColorOverride = StyleNano.NanoGold,
			VerticalAlignment = (VAlignment)2
		};
		Label val17 = val16;
		_nameLabel = val16;
		children3.Add((Control)(object)val17);
		OrderedChildCollection children4 = ((Control)val15).Children;
		Label val18 = new Label
		{
			Text = Loc.GetString("wires-menu-dead-beef-text"),
			FontOverride = font2,
			FontColorOverride = Color.Gray,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(8f, 0f, 20f, 0f),
			HorizontalAlignment = (HAlignment)1,
			HorizontalExpand = true
		};
		val17 = val18;
		_serialLabel = val18;
		children4.Add((Control)(object)val17);
		OrderedChildCollection children5 = ((Control)val15).Children;
		Button val19 = new Button
		{
			Text = "?",
			Margin = new Thickness(0f, 0f, 2f, 0f)
		};
		Button val20 = val19;
		children5.Add((Control)val19);
		OrderedChildCollection children6 = ((Control)val15).Children;
		TextureButton val21 = new TextureButton
		{
			StyleClasses = { "windowCloseButton" },
			VerticalAlignment = (VAlignment)2
		};
		TextureButton val22 = val21;
		CloseButton = val21;
		children6.Add((Control)(object)val22);
		BoxContainer val23 = val15;
		((BaseButton)val20).OnPressed += delegate(ButtonEventArgs a)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			HelpPopup helpPopup = new HelpPopup();
			((Control)((Control)this).UserInterfaceManager.ModalRoot).AddChild((Control)(object)helpPopup);
			((Popup)helpPopup).Open((UIBox2?)UIBox2.FromDimensions(((BoundKeyEventArgs)a.Event).PointerLocation.Position, new Vector2(400f, 200f)), (Vector2?)null, (Vector2?)null);
		};
		PanelContainer val24 = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#202025", (Color?)null)
			}
		};
		OrderedChildCollection children7 = ((Control)val24).Children;
		BoxContainer val25 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		OrderedChildCollection children8 = ((Control)val25).Children;
		GridContainer val26 = new GridContainer
		{
			Margin = new Thickness(8f, 4f),
			Rows = 2
		};
		val13 = (Control)val26;
		_statusContainer = (Control)val26;
		children8.Add(val13);
		children7.Add((Control)val25);
		PanelContainer val27 = val24;
		_topContainer.AddChild((Control)(object)val23);
		_topContainer.AddChild((Control)new PanelContainer
		{
			MinSize = new Vector2(0f, 2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		_topContainer.AddChild((Control)(object)val27);
		_topContainer.AddChild((Control)new PanelContainer
		{
			MinSize = new Vector2(0f, 2f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null)
			}
		});
		((BaseButton)CloseButton).OnPressed += delegate
		{
			((BaseWindow)this).Close();
		};
		((Control)this).SetHeight = 200f;
		((Control)this).MinWidth = 320f;
	}

	public void Populate(WiresBoundUserInterfaceState state)
	{
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		_nameLabel.Text = state.BoardName;
		_serialLabel.Text = state.SerialNumber;
		_wiresHBox.RemoveAllChildren();
		Random random = new Random(state.WireSeed);
		ClientWire[] wiresList = state.WiresList;
		foreach (ClientWire wire in wiresList)
		{
			bool mirror = random.Next(2) == 0;
			bool flip = random.Next(2) == 0;
			int type = random.Next(2);
			WireControl wireControl = new WireControl(wire.Color, wire.Letter, wire.IsCut, flip, mirror, type, _resourceCache);
			((Control)wireControl).VerticalAlignment = (VAlignment)3;
			WireControl wireControl2 = wireControl;
			_wiresHBox.AddChild((Control)(object)wireControl2);
			wireControl2.WireClicked += delegate
			{
				this.OnAction?.Invoke(wire.Id, (!wire.IsCut) ? WiresAction.Cut : WiresAction.Mend);
			};
			wireControl2.ContactsClicked += delegate
			{
				this.OnAction?.Invoke(wire.Id, WiresAction.Pulse);
			};
		}
		_statusContainer.RemoveAllChildren();
		StatusEntry[] statuses = state.Statuses;
		for (int i = 0; i < statuses.Length; i++)
		{
			StatusEntry statusEntry = statuses[i];
			if (statusEntry.Value is StatusLightData data)
			{
				_statusContainer.AddChild((Control)(object)new StatusLight(data, _resourceCache));
				continue;
			}
			_statusContainer.AddChild((Control)new Label
			{
				Text = statusEntry.ToString()
			});
		}
	}

	protected override DragMode GetDragModeFor(Vector2 relativeMousePos)
	{
		return (DragMode)1;
	}

	protected override bool HasPoint(Vector2 point)
	{
		return false;
	}
}
