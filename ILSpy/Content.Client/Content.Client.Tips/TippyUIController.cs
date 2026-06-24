using System;
using System.Collections.Generic;
using System.Numerics;
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

	public override void Initialize()
	{
		((UIController)this).Initialize();
		base.UIManager.OnScreenChanged += OnScreenChanged;
		((UIController)this).SubscribeNetworkEvent<TippyEvent>((EntitySessionEventHandler<TippyEvent>)OnTippyEvent, (Type[])null, (Type[])null);
	}

	private void OnTippyEvent(TippyEvent msg, EntitySessionEventArgs args)
	{
		_queuedMessages.Enqueue(msg);
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen == null)
		{
			_queuedMessages.Clear();
			return;
		}
		TippyUI orAddWidget = activeScreen.GetOrAddWidget<TippyUI>();
		_secondsUntilNextState -= ((FrameEventArgs)(ref args)).DeltaSeconds;
		if (_secondsUntilNextState <= 0f)
		{
			NextState(orAddWidget);
			return;
		}
		Vector2 vector = UpdatePosition(orAddWidget, ((Control)activeScreen).Size, args);
		LayoutContainer.SetPosition((Control)(object)orAddWidget, vector);
	}

	private Vector2 UpdatePosition(TippyUI tippy, Vector2 screenSize, FrameEventArgs args)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		if (_currentMessage == null)
		{
			return default(Vector2);
		}
		float slideTime = _currentMessage.SlideTime;
		float num = tippy.State switch
		{
			TippyUI.TippyState.Hidden => 0f, 
			TippyUI.TippyState.Revealing => Math.Clamp(1f - _secondsUntilNextState / slideTime, 0f, 1f), 
			TippyUI.TippyState.Hiding => Math.Clamp(_secondsUntilNextState / slideTime, 0f, 1f), 
			_ => 1f, 
		};
		float waddleInterval = _currentMessage.WaddleInterval;
		SpriteComponent val = default(SpriteComponent);
		if (_currentMessage == null || waddleInterval <= 0f || tippy.State == TippyUI.TippyState.Hidden || tippy.State == TippyUI.TippyState.Speaking || !base.EntityManager.TryGetComponent<SpriteComponent>(_entity, ref val))
		{
			return new Vector2(screenSize.X - num * (((Control)tippy).DesiredSize.X + 50f), (screenSize.Y - ((Control)tippy).DesiredSize.Y) / 2f);
		}
		int num2 = (int)Math.Ceiling(slideTime / waddleInterval);
		int num3 = (int)Math.Floor((float)num2 * num);
		float num4 = (((Control)tippy).DesiredSize.X + 50f) / (float)num2;
		if (num3 != _previousStep)
		{
			_previousStep = num3;
			_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((_entity, val)), (Angle.op_Implicit(val.Rotation) > 0.0) ? (-WaddleRotation) : WaddleRotation);
			FootstepModifierComponent footstepModifierComponent = default(FootstepModifierComponent);
			if (base.EntityManager.TryGetComponent<FootstepModifierComponent>(_entity, ref footstepModifierComponent) && footstepModifierComponent.FootstepSoundCollection != null)
			{
				AudioParams val2 = footstepModifierComponent.FootstepSoundCollection.Params;
				val2 = ((AudioParams)(ref val2)).AddVolume(-7f);
				AudioParams value = ((AudioParams)(ref val2)).WithVariation((float?)0.1f);
				((SharedAudioSystem)_audio).PlayGlobal(footstepModifierComponent.FootstepSoundCollection, EntityUid.Invalid, (AudioParams?)value);
			}
		}
		return new Vector2(screenSize.X - num4 * (float)num3, (screenSize.Y - ((Control)tippy).DesiredSize.Y) / 2f);
	}

	private void NextState(TippyUI tippy)
	{
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		switch (tippy.State)
		{
		case TippyUI.TippyState.Hidden:
		{
			if (!_queuedMessages.TryDequeue(out TippyEvent result))
			{
				break;
			}
			if (result.Proto != null)
			{
				_entity = base.EntityManager.SpawnEntity(result.Proto, MapCoordinates.Nullspace, (ComponentRegistry)null);
				tippy.ModifyLayers = false;
			}
			else
			{
				_entity = base.EntityManager.SpawnEntity(_cfg.GetCVar<string>(CCVars.TippyEntity), MapCoordinates.Nullspace, (ComponentRegistry)null);
				tippy.ModifyLayers = true;
			}
			if (base.EntityManager.TryGetComponent<SpriteComponent>(_entity, ref val))
			{
				if (!base.EntityManager.HasComponent<PaperVisualsComponent>(_entity))
				{
					PaperVisualsComponent paperVisualsComponent = base.EntityManager.AddComponent<PaperVisualsComponent>(_entity);
					paperVisualsComponent.BackgroundImagePath = "/Textures/Interface/Paper/paper_background_default.svg.96dpi.png";
					paperVisualsComponent.BackgroundPatchMargin = new Box2(16f, 16f, 16f, 16f);
					paperVisualsComponent.BackgroundModulate = new Color(byte.MaxValue, byte.MaxValue, (byte)204, byte.MaxValue);
					paperVisualsComponent.FontAccentColor = new Color((byte)0, (byte)0, (byte)0, byte.MaxValue);
				}
				tippy.InitLabel(EntityManagerExt.GetComponentOrNull<PaperVisualsComponent>(base.EntityManager, _entity), _resCache);
				Vector2 scale = val.Scale;
				if (tippy.ModifyLayers)
				{
					_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((_entity, val)), Vector2.One);
				}
				else
				{
					_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((_entity, val)), new Vector2(3f, 3f));
				}
				tippy.Entity.SetEntity((EntityUid?)_entity);
				tippy.Entity.Scale = scale;
				_currentMessage = result;
				_secondsUntilNextState = result.SlideTime;
				tippy.State = TippyUI.TippyState.Revealing;
				_previousStep = 0;
				if (tippy.ModifyLayers)
				{
					_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((_entity, val)), "revealing", 0f);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "revealing", true);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "speaking", false);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "hiding", false);
				}
				_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((_entity, val)), Angle.op_Implicit(0f));
				tippy.Label.SetMarkupPermissive(_currentMessage.Msg);
				((Control)tippy.Label).Visible = false;
				((Control)tippy.LabelPanel).Visible = false;
				((Control)tippy).Visible = true;
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), true);
			}
			break;
		}
		case TippyUI.TippyState.Revealing:
			tippy.State = TippyUI.TippyState.Speaking;
			if (base.EntityManager.TryGetComponent<SpriteComponent>(_entity, ref val))
			{
				_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((_entity, val)), Angle.op_Implicit(0f));
				_previousStep = 0;
				if (tippy.ModifyLayers)
				{
					_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((_entity, val)), "speaking", 0f);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "revealing", false);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "speaking", true);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "hiding", false);
				}
				((Control)tippy.Label).Visible = true;
				((Control)tippy.LabelPanel).Visible = true;
				((Control)tippy).InvalidateArrange();
				((Control)tippy).InvalidateMeasure();
				if (_currentMessage != null)
				{
					_secondsUntilNextState = _currentMessage.SpeakTime;
				}
			}
			break;
		case TippyUI.TippyState.Speaking:
			tippy.State = TippyUI.TippyState.Hiding;
			if (base.EntityManager.TryGetComponent<SpriteComponent>(_entity, ref val))
			{
				if (tippy.ModifyLayers)
				{
					_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((_entity, val)), "hiding", 0f);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "revealing", false);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "speaking", false);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((_entity, val)), "hiding", true);
				}
				((Control)tippy.LabelPanel).Visible = false;
				if (_currentMessage != null)
				{
					_secondsUntilNextState = _currentMessage.SlideTime;
				}
			}
			break;
		default:
			base.EntityManager.DeleteEntity((EntityUid?)_entity);
			_entity = default(EntityUid);
			((Control)tippy).Visible = false;
			_currentMessage = null;
			_secondsUntilNextState = 0f;
			tippy.State = TippyUI.TippyState.Hidden;
			break;
		}
	}

	private void OnScreenChanged((UIScreen? Old, UIScreen? New) ev)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		var (val, _) = ev;
		if (val != null)
		{
			val.RemoveWidget<TippyUI>();
		}
		_currentMessage = null;
		base.EntityManager.DeleteEntity((EntityUid?)_entity);
	}
}
