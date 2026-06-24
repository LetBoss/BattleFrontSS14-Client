// Decompiled with JetBrains decompiler
// Type: Content.Client.Tips.TippyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Client.Paper.UI;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Content.Shared.Tips;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Tips;

public sealed class TippyUIController : UIController
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IResourceCache _resCache;
  [UISystemDependency]
  private readonly AudioSystem _audio;
  [UISystemDependency]
  private readonly SpriteSystem _sprite;
  public const float Padding = 50f;
  public static Angle WaddleRotation = Angle.FromDegrees(10.0);
  private EntityUid _entity;
  private float _secondsUntilNextState;
  private int _previousStep;
  private TippyEvent? _currentMessage;
  private readonly Queue<TippyEvent> _queuedMessages = new Queue<TippyEvent>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.UIManager.OnScreenChanged += new Action<(UIScreen, UIScreen)>(this.OnScreenChanged);
    this.SubscribeNetworkEvent<TippyEvent>(new EntitySessionEventHandler<TippyEvent>(this.OnTippyEvent), (Type[]) null, (Type[]) null);
  }

  private void OnTippyEvent(TippyEvent msg, EntitySessionEventArgs args)
  {
    this._queuedMessages.Enqueue(msg);
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
    {
      this._queuedMessages.Clear();
    }
    else
    {
      TippyUI orAddWidget = activeScreen.GetOrAddWidget<TippyUI>();
      this._secondsUntilNextState -= ((FrameEventArgs) ref args).DeltaSeconds;
      if ((double) this._secondsUntilNextState <= 0.0)
      {
        this.NextState(orAddWidget);
      }
      else
      {
        Vector2 vector2 = this.UpdatePosition(orAddWidget, ((Control) activeScreen).Size, args);
        LayoutContainer.SetPosition((Control) orAddWidget, vector2);
      }
    }
  }

  private Vector2 UpdatePosition(TippyUI tippy, Vector2 screenSize, FrameEventArgs args)
  {
    if (this._currentMessage == null)
      return new Vector2();
    float slideTime = this._currentMessage.SlideTime;
    float num1;
    switch (tippy.State)
    {
      case TippyUI.TippyState.Hidden:
        num1 = 0.0f;
        break;
      case TippyUI.TippyState.Revealing:
        num1 = Math.Clamp((float) (1.0 - (double) this._secondsUntilNextState / (double) slideTime), 0.0f, 1f);
        break;
      case TippyUI.TippyState.Hiding:
        num1 = Math.Clamp(this._secondsUntilNextState / slideTime, 0.0f, 1f);
        break;
      default:
        num1 = 1f;
        break;
    }
    float num2 = num1;
    float waddleInterval = this._currentMessage.WaddleInterval;
    SpriteComponent spriteComponent;
    if (this._currentMessage == null || (double) waddleInterval <= 0.0 || tippy.State == TippyUI.TippyState.Hidden || tippy.State == TippyUI.TippyState.Speaking || !this.EntityManager.TryGetComponent<SpriteComponent>(this._entity, ref spriteComponent))
      return new Vector2(screenSize.X - num2 * (((Control) tippy).DesiredSize.X + 50f), (float) (((double) screenSize.Y - (double) ((Control) tippy).DesiredSize.Y) / 2.0));
    int num3 = (int) Math.Ceiling((double) slideTime / (double) waddleInterval);
    int num4 = (int) Math.Floor((double) num3 * (double) num2);
    float num5 = (((Control) tippy).DesiredSize.X + 50f) / (float) num3;
    if (num4 != this._previousStep)
    {
      this._previousStep = num4;
      this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent)), Angle.op_Implicit(spriteComponent.Rotation) > 0.0 ? Angle.op_UnaryNegation(TippyUIController.WaddleRotation) : TippyUIController.WaddleRotation);
      FootstepModifierComponent modifierComponent;
      if (this.EntityManager.TryGetComponent<FootstepModifierComponent>(this._entity, ref modifierComponent) && modifierComponent.FootstepSoundCollection != null)
      {
        AudioParams audioParams1 = modifierComponent.FootstepSoundCollection.Params;
        audioParams1 = ((AudioParams) ref audioParams1).AddVolume(-7f);
        AudioParams audioParams2 = ((AudioParams) ref audioParams1).WithVariation(new float?(0.1f));
        ((SharedAudioSystem) this._audio).PlayGlobal(modifierComponent.FootstepSoundCollection, EntityUid.Invalid, new AudioParams?(audioParams2));
      }
    }
    return new Vector2(screenSize.X - num5 * (float) num4, (float) (((double) screenSize.Y - (double) ((Control) tippy).DesiredSize.Y) / 2.0));
  }

  private void NextState(TippyUI tippy)
  {
    switch (tippy.State)
    {
      case TippyUI.TippyState.Hidden:
        TippyEvent result;
        if (!this._queuedMessages.TryDequeue(out result))
          break;
        if (result.Proto != null)
        {
          this._entity = this.EntityManager.SpawnEntity(result.Proto, MapCoordinates.Nullspace, (ComponentRegistry) null);
          tippy.ModifyLayers = false;
        }
        else
        {
          this._entity = this.EntityManager.SpawnEntity(this._cfg.GetCVar<string>(CCVars.TippyEntity), MapCoordinates.Nullspace, (ComponentRegistry) null);
          tippy.ModifyLayers = true;
        }
        SpriteComponent spriteComponent1;
        if (!this.EntityManager.TryGetComponent<SpriteComponent>(this._entity, ref spriteComponent1))
          break;
        if (!this.EntityManager.HasComponent<PaperVisualsComponent>(this._entity))
        {
          PaperVisualsComponent visualsComponent = this.EntityManager.AddComponent<PaperVisualsComponent>(this._entity);
          visualsComponent.BackgroundImagePath = "/Textures/Interface/Paper/paper_background_default.svg.96dpi.png";
          visualsComponent.BackgroundPatchMargin = new Box2(16f, 16f, 16f, 16f);
          visualsComponent.BackgroundModulate = new Color(byte.MaxValue, byte.MaxValue, (byte) 204, byte.MaxValue);
          visualsComponent.FontAccentColor = new Color((byte) 0, (byte) 0, (byte) 0, byte.MaxValue);
        }
        tippy.InitLabel(EntityManagerExt.GetComponentOrNull<PaperVisualsComponent>(this.EntityManager, this._entity), this._resCache);
        Vector2 scale = spriteComponent1.Scale;
        if (tippy.ModifyLayers)
          this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), Vector2.One);
        else
          this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), new Vector2(3f, 3f));
        tippy.Entity.SetEntity(new EntityUid?(this._entity));
        tippy.Entity.Scale = scale;
        this._currentMessage = result;
        this._secondsUntilNextState = result.SlideTime;
        tippy.State = TippyUI.TippyState.Revealing;
        this._previousStep = 0;
        if (tippy.ModifyLayers)
        {
          this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), "revealing", 0.0f);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), "revealing", true);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), "speaking", false);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), "hiding", false);
        }
        this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), Angle.op_Implicit(0.0f));
        tippy.Label.SetMarkupPermissive(this._currentMessage.Msg);
        ((Control) tippy.Label).Visible = false;
        ((Control) tippy.LabelPanel).Visible = false;
        ((Control) tippy).Visible = true;
        this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent1)), true);
        break;
      case TippyUI.TippyState.Revealing:
        tippy.State = TippyUI.TippyState.Speaking;
        SpriteComponent spriteComponent2;
        if (!this.EntityManager.TryGetComponent<SpriteComponent>(this._entity, ref spriteComponent2))
          break;
        this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent2)), Angle.op_Implicit(0.0f));
        this._previousStep = 0;
        if (tippy.ModifyLayers)
        {
          this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent2)), "speaking", 0.0f);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent2)), "revealing", false);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent2)), "speaking", true);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent2)), "hiding", false);
        }
        ((Control) tippy.Label).Visible = true;
        ((Control) tippy.LabelPanel).Visible = true;
        ((Control) tippy).InvalidateArrange();
        ((Control) tippy).InvalidateMeasure();
        if (this._currentMessage == null)
          break;
        this._secondsUntilNextState = this._currentMessage.SpeakTime;
        break;
      case TippyUI.TippyState.Speaking:
        tippy.State = TippyUI.TippyState.Hiding;
        SpriteComponent spriteComponent3;
        if (!this.EntityManager.TryGetComponent<SpriteComponent>(this._entity, ref spriteComponent3))
          break;
        if (tippy.ModifyLayers)
        {
          this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent3)), "hiding", 0.0f);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent3)), "revealing", false);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent3)), "speaking", false);
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((this._entity, spriteComponent3)), "hiding", true);
        }
        ((Control) tippy.LabelPanel).Visible = false;
        if (this._currentMessage == null)
          break;
        this._secondsUntilNextState = this._currentMessage.SlideTime;
        break;
      default:
        this.EntityManager.DeleteEntity(new EntityUid?(this._entity));
        this._entity = new EntityUid();
        ((Control) tippy).Visible = false;
        this._currentMessage = (TippyEvent) null;
        this._secondsUntilNextState = 0.0f;
        tippy.State = TippyUI.TippyState.Hidden;
        break;
    }
  }

  private void OnScreenChanged((UIScreen? Old, UIScreen? New) ev)
  {
    ev.Old?.RemoveWidget<TippyUI>();
    this._currentMessage = (TippyEvent) null;
    this.EntityManager.DeleteEntity(new EntityUid?(this._entity));
  }
}
