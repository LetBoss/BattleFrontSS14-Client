// Decompiled with JetBrains decompiler
// Type: Content.Client.Wires.UI.WiresMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Wires.UI;

public sealed class WiresMenu : BaseWindow
{
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
    IoCManager.InjectDependencies<WiresMenu>(this);
    LayoutContainer layoutContainer1 = new LayoutContainer();
    ((Control) layoutContainer1).Name = "WireRoot";
    LayoutContainer layoutContainer2 = layoutContainer1;
    ((Control) this).AddChild((Control) layoutContainer2);
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    Texture texture = this._resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?())
    };
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer1).MouseFilter = (Control.MouseFilterMode) 1;
    PanelContainer panelContainer2 = panelContainer1;
    LayoutContainer layoutContainer3 = new LayoutContainer();
    ((Control) layoutContainer3).Name = "BottomWrap";
    LayoutContainer layoutContainer4 = layoutContainer3;
    PanelContainer panelContainer3 = new PanelContainer();
    panelContainer3.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer3).MouseFilter = (Control.MouseFilterMode) 1;
    PanelContainer panelContainer4 = panelContainer3;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).MinSize = new Vector2(2f, 0.0f);
    panelContainer5.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    children1.Add((Control) panelContainer5);
    Control.OrderedChildCollection children2 = ((Control) boxContainer1).Children;
    PanelContainer panelContainer6 = new PanelContainer();
    ((Control) panelContainer6).HorizontalExpand = true;
    ((Control) panelContainer6).MouseFilter = (Control.MouseFilterMode) 0;
    ((Control) panelContainer6).Name = "Shadow";
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    Color black = Color.Black;
    styleBoxFlat.BackgroundColor = ((Color) ref black).WithAlpha(0.5f);
    panelContainer6.PanelOverride = (StyleBox) styleBoxFlat;
    children2.Add((Control) panelContainer6);
    Control.OrderedChildCollection children3 = ((Control) boxContainer1).Children;
    PanelContainer panelContainer7 = new PanelContainer();
    ((Control) panelContainer7).MinSize = new Vector2(2f, 0.0f);
    panelContainer7.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    children3.Add((Control) panelContainer7);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer4.SeparationOverride = new int?(4);
    ((Control) boxContainer4).VerticalAlignment = (Control.VAlignment) 3;
    this._wiresHBox = (Control) boxContainer4;
    ((Control) boxContainer3).AddChild(new Control()
    {
      MinSize = new Vector2(20f, 0.0f)
    });
    ((Control) boxContainer3).AddChild(this._wiresHBox);
    ((Control) boxContainer3).AddChild(new Control()
    {
      MinSize = new Vector2(20f, 0.0f)
    });
    ((Control) layoutContainer4).AddChild((Control) panelContainer4);
    LayoutContainer.SetAnchorPreset((Control) panelContainer4, (LayoutContainer.LayoutPreset) 12, false);
    LayoutContainer.SetMarginTop((Control) panelContainer4, -55f);
    ((Control) layoutContainer4).AddChild((Control) boxContainer2);
    LayoutContainer.SetAnchorPreset((Control) boxContainer2, (LayoutContainer.LayoutPreset) 12, false);
    LayoutContainer.SetMarginBottom((Control) boxContainer2, -55f);
    LayoutContainer.SetMarginTop((Control) boxContainer2, -80f);
    LayoutContainer.SetMarginLeft((Control) boxContainer2, 12f);
    LayoutContainer.SetMarginRight((Control) boxContainer2, -12f);
    ((Control) layoutContainer4).AddChild((Control) boxContainer3);
    LayoutContainer.SetAnchorPreset((Control) boxContainer3, (LayoutContainer.LayoutPreset) 15, false);
    LayoutContainer.SetMarginBottom((Control) boxContainer3, -4f);
    ((Control) layoutContainer2).AddChild((Control) panelContainer2);
    ((Control) layoutContainer2).AddChild((Control) layoutContainer4);
    LayoutContainer.SetAnchorPreset((Control) panelContainer2, (LayoutContainer.LayoutPreset) 15, false);
    LayoutContainer.SetMarginBottom((Control) panelContainer2, -80f);
    LayoutContainer.SetAnchorPreset((Control) layoutContainer4, (LayoutContainer.LayoutPreset) 13, false);
    LayoutContainer.SetGrowHorizontal((Control) layoutContainer4, (LayoutContainer.GrowDirection) 2);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    Control.OrderedChildCollection children4 = ((Control) boxContainer5).Children;
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 1;
    Control control1 = (Control) boxContainer6;
    this._topContainer = (Control) boxContainer6;
    Control control2 = control1;
    children4.Add(control2);
    ((Control) boxContainer5).Children.Add(new Control()
    {
      MinSize = new Vector2(0.0f, 110f)
    });
    BoxContainer boxContainer7 = boxContainer5;
    ((Control) layoutContainer2).AddChild((Control) boxContainer7);
    LayoutContainer.SetAnchorPreset((Control) boxContainer7, (LayoutContainer.LayoutPreset) 15, false);
    Font font1 = this._resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
    Font font2 = this._resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 10);
    BoxContainer boxContainer8 = new BoxContainer();
    boxContainer8.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer8).Margin = new Thickness(4f, 2f, 12f, 2f);
    Control.OrderedChildCollection children5 = ((Control) boxContainer8).Children;
    Label label1 = new Label();
    label1.Text = Loc.GetString("wires-menu-name-label");
    label1.FontOverride = font1;
    label1.FontColorOverride = new Color?(StyleNano.NanoGold);
    ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
    Label label2 = label1;
    this._nameLabel = label1;
    Label label3 = label2;
    children5.Add((Control) label3);
    Control.OrderedChildCollection children6 = ((Control) boxContainer8).Children;
    Label label4 = new Label();
    label4.Text = Loc.GetString("wires-menu-dead-beef-text");
    label4.FontOverride = font2;
    label4.FontColorOverride = new Color?(Color.Gray);
    ((Control) label4).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label4).Margin = new Thickness(8f, 0.0f, 20f, 0.0f);
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label4).HorizontalExpand = true;
    Label label5 = label4;
    this._serialLabel = label4;
    Label label6 = label5;
    children6.Add((Control) label6);
    Control.OrderedChildCollection children7 = ((Control) boxContainer8).Children;
    Button button1 = new Button();
    button1.Text = "?";
    ((Control) button1).Margin = new Thickness(0.0f, 0.0f, 2f, 0.0f);
    Button button2 = button1;
    children7.Add((Control) button1);
    Control.OrderedChildCollection children8 = ((Control) boxContainer8).Children;
    TextureButton textureButton1 = new TextureButton();
    ((Control) textureButton1).StyleClasses.Add("windowCloseButton");
    ((Control) textureButton1).VerticalAlignment = (Control.VAlignment) 2;
    TextureButton textureButton2 = textureButton1;
    this.CloseButton = textureButton1;
    TextureButton textureButton3 = textureButton2;
    children8.Add((Control) textureButton3);
    BoxContainer boxContainer9 = boxContainer8;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (a =>
    {
      WiresMenu.HelpPopup helpPopup = new WiresMenu.HelpPopup();
      ((Control) ((Control) this).UserInterfaceManager.ModalRoot).AddChild((Control) helpPopup);
      helpPopup.Open(new UIBox2?(UIBox2.FromDimensions(((BoundKeyEventArgs) a.Event).PointerLocation.Position, new Vector2(400f, 200f))), new Vector2?(), new Vector2?());
    });
    PanelContainer panelContainer8 = new PanelContainer();
    panelContainer8.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#202025", new Color?())
    };
    Control.OrderedChildCollection children9 = ((Control) panelContainer8).Children;
    BoxContainer boxContainer10 = new BoxContainer();
    boxContainer10.Orientation = (BoxContainer.LayoutOrientation) 0;
    Control.OrderedChildCollection children10 = ((Control) boxContainer10).Children;
    GridContainer gridContainer = new GridContainer();
    ((Control) gridContainer).Margin = new Thickness(8f, 4f);
    gridContainer.Rows = 2;
    Control control3 = (Control) gridContainer;
    this._statusContainer = (Control) gridContainer;
    Control control4 = control3;
    children10.Add(control4);
    children9.Add((Control) boxContainer10);
    PanelContainer panelContainer9 = panelContainer8;
    this._topContainer.AddChild((Control) boxContainer9);
    Control topContainer1 = this._topContainer;
    PanelContainer panelContainer10 = new PanelContainer();
    ((Control) panelContainer10).MinSize = new Vector2(0.0f, 2f);
    panelContainer10.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    topContainer1.AddChild((Control) panelContainer10);
    this._topContainer.AddChild((Control) panelContainer9);
    Control topContainer2 = this._topContainer;
    PanelContainer panelContainer11 = new PanelContainer();
    ((Control) panelContainer11).MinSize = new Vector2(0.0f, 2f);
    panelContainer11.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    topContainer2.AddChild((Control) panelContainer11);
    ((BaseButton) this.CloseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
    ((Control) this).SetHeight = 200f;
    ((Control) this).MinWidth = 320f;
  }

  public void Populate(WiresBoundUserInterfaceState state)
  {
    this._nameLabel.Text = state.BoardName;
    this._serialLabel.Text = state.SerialNumber;
    this._wiresHBox.RemoveAllChildren();
    Random random = new Random(state.WireSeed);
    foreach (ClientWire wires in state.WiresList)
    {
      ClientWire wire = wires;
      bool mirror = random.Next(2) == 0;
      bool flip = random.Next(2) == 0;
      int type = random.Next(2);
      WiresMenu.WireControl wireControl1 = new WiresMenu.WireControl(wire.Color, wire.Letter, wire.IsCut, flip, mirror, type, this._resourceCache);
      wireControl1.VerticalAlignment = (Control.VAlignment) 3;
      WiresMenu.WireControl wireControl2 = wireControl1;
      this._wiresHBox.AddChild((Control) wireControl2);
      wireControl2.WireClicked += (Action) (() =>
      {
        Action<int, WiresAction> onAction = this.OnAction;
        if (onAction == null)
          return;
        onAction(wire.Id, wire.IsCut ? WiresAction.Mend : WiresAction.Cut);
      });
      wireControl2.ContactsClicked += (Action) (() =>
      {
        Action<int, WiresAction> onAction = this.OnAction;
        if (onAction == null)
          return;
        onAction(wire.Id, WiresAction.Pulse);
      });
    }
    this._statusContainer.RemoveAllChildren();
    foreach (StatusEntry statuse in state.Statuses)
    {
      if (statuse.Value is StatusLightData data)
        this._statusContainer.AddChild((Control) new WiresMenu.StatusLight(data, this._resourceCache));
      else
        this._statusContainer.AddChild((Control) new Label()
        {
          Text = statuse.ToString()
        });
    }
  }

  protected virtual BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
  {
    return (BaseWindow.DragMode) 1;
  }

  protected virtual bool HasPoint(Vector2 point) => false;

  private sealed class WireControl : Control
  {
    private IResourceCache _resourceCache;
    private const string TextureContact = "/Textures/Interface/WireHacking/contact.svg.96dpi.png";

    public event Action? WireClicked;

    public event Action? ContactsClicked;

    public WireControl(
      WireColor color,
      WireLetter letter,
      bool isCut,
      bool flip,
      bool mirror,
      int type,
      IResourceCache resourceCache)
    {
      this._resourceCache = resourceCache;
      this.HorizontalAlignment = (Control.HAlignment) 2;
      this.MouseFilter = (Control.MouseFilterMode) 0;
      LayoutContainer layoutContainer = new LayoutContainer();
      this.AddChild((Control) layoutContainer);
      Label label1 = new Label();
      label1.Text = letter.Letter().ToString();
      ((Control) label1).VerticalAlignment = (Control.VAlignment) 3;
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
      label1.Align = (Label.AlignMode) 1;
      label1.FontOverride = this._resourceCache.GetFont("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 12);
      label1.FontColorOverride = new Color?(Color.Gray);
      ((Control) label1).ToolTip = letter.Name();
      ((Control) label1).MouseFilter = (Control.MouseFilterMode) 0;
      Label label2 = label1;
      ((Control) layoutContainer).AddChild((Control) label2);
      LayoutContainer.SetAnchorPreset((Control) label2, (LayoutContainer.LayoutPreset) 12, false);
      LayoutContainer.SetGrowVertical((Control) label2, (LayoutContainer.GrowDirection) 1);
      LayoutContainer.SetGrowHorizontal((Control) label2, (LayoutContainer.GrowDirection) 2);
      Texture texture = this._resourceCache.GetTexture("/Textures/Interface/WireHacking/contact.svg.96dpi.png");
      TextureRect textureRect1 = new TextureRect();
      textureRect1.Texture = texture;
      ((Control) textureRect1).Modulate = Color.FromHex((ReadOnlySpan<char>) "#E1CA76", new Color?());
      TextureRect textureRect2 = textureRect1;
      ((Control) layoutContainer).AddChild((Control) textureRect2);
      LayoutContainer.SetPosition((Control) textureRect2, new Vector2(0.0f, 0.0f));
      TextureRect textureRect3 = new TextureRect();
      textureRect3.Texture = texture;
      ((Control) textureRect3).Modulate = Color.FromHex((ReadOnlySpan<char>) "#E1CA76", new Color?());
      TextureRect textureRect4 = textureRect3;
      ((Control) layoutContainer).AddChild((Control) textureRect4);
      LayoutContainer.SetPosition((Control) textureRect4, new Vector2(0.0f, 60f));
      WiresMenu.WireControl.WireRender wireRender = new WiresMenu.WireControl.WireRender(color, isCut, flip, mirror, type, this._resourceCache);
      ((Control) layoutContainer).AddChild((Control) wireRender);
      LayoutContainer.SetPosition((Control) wireRender, new Vector2(2f, 16f));
      this.ToolTip = color.Name();
      this.MinSize = new Vector2(20f, 102f);
    }

    protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
    {
      base.KeyBindDown(args);
      if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
        return;
      if ((double) args.RelativePosition.Y > 20.0 && (double) args.RelativePosition.Y < 60.0)
      {
        Action wireClicked = this.WireClicked;
        if (wireClicked == null)
          return;
        wireClicked();
      }
      else
      {
        Action contactsClicked = this.ContactsClicked;
        if (contactsClicked == null)
          return;
        contactsClicked();
      }
    }

    protected virtual bool HasPoint(Vector2 point)
    {
      return base.HasPoint(point) && (double) point.Y <= 80.0;
    }

    private sealed class WireRender : Control
    {
      private readonly WireColor _color;
      private readonly bool _isCut;
      private readonly bool _flip;
      private readonly bool _mirror;
      private readonly int _type;
      private static readonly string[] TextureNormal = new string[2]
      {
        "/Textures/Interface/WireHacking/wire_1.svg.96dpi.png",
        "/Textures/Interface/WireHacking/wire_2.svg.96dpi.png"
      };
      private static readonly string[] TextureCut = new string[2]
      {
        "/Textures/Interface/WireHacking/wire_1_cut.svg.96dpi.png",
        "/Textures/Interface/WireHacking/wire_2_cut.svg.96dpi.png"
      };
      private static readonly string[] TextureCopper = new string[2]
      {
        "/Textures/Interface/WireHacking/wire_1_copper.svg.96dpi.png",
        "/Textures/Interface/WireHacking/wire_2_copper.svg.96dpi.png"
      };
      private readonly IResourceCache _resourceCache;

      public WireRender(
        WireColor color,
        bool isCut,
        bool flip,
        bool mirror,
        int type,
        IResourceCache resourceCache)
      {
        this._resourceCache = resourceCache;
        this._color = color;
        this._isCut = isCut;
        this._flip = flip;
        this._mirror = mirror;
        this._type = type;
        this.SetSize = new Vector2(16f, 50f);
      }

      protected virtual void Draw(DrawingHandleScreen handle)
      {
        Color color = this._color.ColorValue();
        Texture texture1 = this._resourceCache.GetTexture(this._isCut ? WiresMenu.WireControl.WireRender.TextureCut[this._type] : WiresMenu.WireControl.WireRender.TextureNormal[this._type]);
        float num1 = 0.0f;
        float num2 = (float) texture1.Width + num1;
        float num3 = 0.0f;
        float num4 = (float) texture1.Height + num3;
        if (this._flip)
        {
          double num5 = (double) num4;
          num4 = num3;
          num3 = (float) num5;
        }
        if (this._mirror)
        {
          double num6 = (double) num2;
          num2 = num1;
          num1 = (float) num6;
        }
        float num7 = num1 * this.UIScale;
        float num8 = num2 * this.UIScale;
        float num9 = num3 * this.UIScale;
        float num10 = num4 * this.UIScale;
        UIBox2 uiBox2;
        // ISSUE: explicit constructor call
        ((UIBox2) ref uiBox2).\u002Ector(num7, num9, num8, num10);
        if (this._isCut)
        {
          Color orange = Color.Orange;
          Texture texture2 = this._resourceCache.GetTexture(WiresMenu.WireControl.WireRender.TextureCopper[this._type]);
          handle.DrawTextureRect(texture2, uiBox2, new Color?(orange));
        }
        handle.DrawTextureRect(texture1, uiBox2, new Color?(color));
      }
    }
  }

  private sealed class StatusLight : Control
  {
    private static readonly Animation _blinkingFast;
    private static readonly Animation _blinkingSlow;

    public StatusLight(StatusLightData data, IResourceCache resourceCache)
    {
      this.HorizontalAlignment = (Control.HAlignment) 3;
      Vector4 hsv = Color.ToHsv(data.Color);
      hsv.Z /= 2f;
      Color color1 = Color.FromHsv(hsv);
      Control control1 = new Control();
      control1.SetSize = new Vector2(20f, 20f);
      Control.OrderedChildCollection children1 = control1.Children;
      TextureRect textureRect1 = new TextureRect();
      textureRect1.Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_off_base.svg.96dpi.png");
      textureRect1.Stretch = (TextureRect.StretchMode) 4;
      ((Control) textureRect1).ModulateSelfOverride = new Color?(color1);
      children1.Add((Control) textureRect1);
      Control.OrderedChildCollection children2 = control1.Children;
      TextureRect textureRect2 = new TextureRect();
      Color color2 = data.Color;
      ((Control) textureRect2).ModulateSelfOverride = new Color?(((Color) ref color2).WithAlpha(0.4f));
      textureRect2.Stretch = (TextureRect.StretchMode) 4;
      textureRect2.Texture = resourceCache.GetTexture("/Textures/Interface/WireHacking/light_on_base.svg.96dpi.png");
      TextureRect textureRect3 = textureRect2;
      TextureRect activeLight = textureRect2;
      TextureRect textureRect4 = textureRect3;
      children2.Add((Control) textureRect4);
      Control control2 = control1;
      Animation animation = (Animation) null;
      switch (data.State)
      {
        case StatusLightState.Off:
          ((Control) activeLight).Visible = false;
          goto case StatusLightState.On;
        case StatusLightState.On:
          if (animation != null)
          {
            ((Control) activeLight).PlayAnimation(animation, "blink");
            TextureRect textureRect5 = activeLight;
            ((Control) textureRect5).AnimationCompleted = ((Control) textureRect5).AnimationCompleted + (Action<string>) (s =>
            {
              if (!(s == "blink"))
                return;
              ((Control) activeLight).PlayAnimation(animation, s);
            });
          }
          Font font = resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 12);
          BoxContainer boxContainer1 = new BoxContainer()
          {
            Orientation = (BoxContainer.LayoutOrientation) 0,
            SeparationOverride = new int?(4)
          };
          BoxContainer boxContainer2 = boxContainer1;
          Label label = new Label();
          label.Text = data.Text;
          label.FontOverride = font;
          label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#A1A6AE", new Color?()));
          ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
          ((Control) boxContainer2).AddChild((Control) label);
          ((Control) boxContainer1).AddChild(control2);
          ((Control) boxContainer1).AddChild(new Control()
          {
            MinSize = new Vector2(6f, 0.0f)
          });
          this.AddChild((Control) boxContainer1);
          break;
        case StatusLightState.BlinkingFast:
          animation = WiresMenu.StatusLight._blinkingFast;
          goto case StatusLightState.On;
        case StatusLightState.BlinkingSlow:
          animation = WiresMenu.StatusLight._blinkingSlow;
          goto case StatusLightState.On;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    static StatusLight()
    {
      Animation animation1 = new Animation();
      animation1.Length = TimeSpan.FromSeconds(0.2);
      List<AnimationTrack> animationTracks1 = animation1.AnimationTracks;
      AnimationTrackControlProperty trackControlProperty1 = new AnimationTrackControlProperty();
      trackControlProperty1.Property = "Modulate";
      ((AnimationTrackProperty) trackControlProperty1).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) trackControlProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.White, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.Transparent, 0.1f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.White, 0.1f, (Func<float, float>) null));
      animationTracks1.Add((AnimationTrack) trackControlProperty1);
      WiresMenu.StatusLight._blinkingFast = animation1;
      Animation animation2 = new Animation();
      animation2.Length = TimeSpan.FromSeconds(0.8);
      List<AnimationTrack> animationTracks2 = animation2.AnimationTracks;
      AnimationTrackControlProperty trackControlProperty2 = new AnimationTrackControlProperty();
      trackControlProperty2.Property = "Modulate";
      ((AnimationTrackProperty) trackControlProperty2).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) trackControlProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.White, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.White, 0.3f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.Transparent, 0.1f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.Transparent, 0.3f, (Func<float, float>) null));
      ((AnimationTrackProperty) trackControlProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Color.White, 0.1f, (Func<float, float>) null));
      animationTracks2.Add((AnimationTrack) trackControlProperty2);
      WiresMenu.StatusLight._blinkingSlow = animation2;
    }
  }

  private sealed class HelpPopup : Popup
  {
    public HelpPopup()
    {
      RichTextLabel richTextLabel = new RichTextLabel();
      richTextLabel.SetMessage(Loc.GetString("wires-menu-help-popup"), new Color?());
      PanelContainer panelContainer = new PanelContainer();
      ((Control) panelContainer).StyleClasses.Add("entity-tooltip");
      ((Control) panelContainer).Children.Add((Control) richTextLabel);
      ((Control) this).AddChild((Control) panelContainer);
    }
  }
}
