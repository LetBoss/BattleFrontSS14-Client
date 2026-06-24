using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client._CIV14merka.Particles;
using Content.Client._PUBG.Vision;
using Content.Client._RMC14.ItemPickup;
using Content.Client._RMC14.Movement;
using Content.Client._RMC14.Weapons.Ranged.Prediction;
using Content.Client.Animations;
using Content.Client.Gameplay;
using Content.Client.IoC;
using Content.Client.Items;
using Content.Client.Resources;
using Content.Client.Stack;
using Content.Client.Weapons.Ranged.Components;
using Content.Client.Weapons.Ranged.ItemStatus;
using Content.Shared._CIV14merka.Particles;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared.CombatMode;
using Content.Shared.Rounding;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Animations;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Utility;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed class GunSystem : SharedGunSystem
{
	public sealed class AmmoCounterControlEvent : EntityEventArgs
	{
		public Control? Control;
	}

	public sealed class UpdateAmmoCounterEvent : HandledEntityEventArgs
	{
		public Control Control;

		public int ArtificialIncrease;
	}

	private sealed class DefaultStatusControl : Control
	{
		private readonly BulletRender _bulletRender;

		public DefaultStatusControl()
		{
			((Control)this).MinHeight = 15f;
			((Control)this).HorizontalExpand = true;
			((Control)this).VerticalAlignment = (VAlignment)2;
			BulletRender bulletRender = new BulletRender();
			((Control)bulletRender).HorizontalAlignment = (HAlignment)3;
			((Control)bulletRender).VerticalAlignment = (VAlignment)3;
			BulletRender bulletRender2 = bulletRender;
			_bulletRender = bulletRender;
			((Control)this).AddChild((Control)(object)bulletRender2);
		}

		public void Update(int count, int capacity)
		{
			_bulletRender.Count = count;
			_bulletRender.Capacity = capacity;
			_bulletRender.Type = ((capacity > 50) ? BulletRender.BulletType.Tiny : BulletRender.BulletType.Normal);
		}
	}

	public sealed class BoxesStatusControl : Control
	{
		private readonly BatteryBulletRenderer _bullets;

		private readonly Label _ammoCount;

		public BoxesStatusControl()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_009c: Expected O, but got Unknown
			//IL_00a7: Expected O, but got Unknown
			((Control)this).MinHeight = 15f;
			((Control)this).HorizontalExpand = true;
			((Control)this).VerticalAlignment = (VAlignment)2;
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0
			};
			OrderedChildCollection children = ((Control)val).Children;
			BatteryBulletRenderer batteryBulletRenderer = new BatteryBulletRenderer();
			((Control)batteryBulletRenderer).Margin = new Thickness(0f, 0f, 5f, 0f);
			((Control)batteryBulletRenderer).HorizontalExpand = true;
			BatteryBulletRenderer batteryBulletRenderer2 = batteryBulletRenderer;
			_bullets = batteryBulletRenderer;
			children.Add((Control)(object)batteryBulletRenderer2);
			OrderedChildCollection children2 = ((Control)val).Children;
			Label val2 = new Label
			{
				StyleClasses = { "ItemStatus" },
				HorizontalAlignment = (HAlignment)3,
				VerticalAlignment = (VAlignment)3
			};
			Label val3 = val2;
			_ammoCount = val2;
			children2.Add((Control)(object)val3);
			((Control)this).AddChild((Control)val);
		}

		public void Update(int count, int max)
		{
			((Control)_ammoCount).Visible = true;
			_ammoCount.Text = $"x{count:00}";
			_bullets.Capacity = max;
			_bullets.Count = count;
		}
	}

	private sealed class ChamberMagazineStatusControl : Control
	{
		private readonly BulletRender _bulletRender;

		private readonly TextureRect _chamberedBullet;

		private readonly Label _noMagazineLabel;

		private readonly Label _ammoCount;

		public ChamberMagazineStatusControl()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			//IL_00b9: Expected O, but got Unknown
			//IL_00c4: Expected O, but got Unknown
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected O, but got Unknown
			//IL_0126: Expected O, but got Unknown
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			//IL_015b: Expected O, but got Unknown
			//IL_0166: Expected O, but got Unknown
			//IL_016b: Expected O, but got Unknown
			((Control)this).MinHeight = 15f;
			((Control)this).HorizontalExpand = true;
			((Control)this).VerticalAlignment = (VAlignment)2;
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true
			};
			OrderedChildCollection children = ((Control)val).Children;
			Control val2 = new Control
			{
				HorizontalExpand = true,
				Margin = new Thickness(0f, 0f, 5f, 0f)
			};
			OrderedChildCollection children2 = val2.Children;
			BulletRender bulletRender = new BulletRender();
			((Control)bulletRender).HorizontalAlignment = (HAlignment)3;
			((Control)bulletRender).VerticalAlignment = (VAlignment)3;
			BulletRender bulletRender2 = bulletRender;
			_bulletRender = bulletRender;
			children2.Add((Control)(object)bulletRender2);
			OrderedChildCollection children3 = val2.Children;
			Label val3 = new Label
			{
				Text = "No Magazine!",
				StyleClasses = { "ItemStatus" }
			};
			Label val4 = val3;
			_noMagazineLabel = val3;
			children3.Add((Control)(object)val4);
			children.Add(val2);
			OrderedChildCollection children4 = ((Control)val).Children;
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				VerticalAlignment = (VAlignment)3,
				Margin = new Thickness(0f, 0f, 0f, 2f)
			};
			OrderedChildCollection children5 = ((Control)val5).Children;
			Label val6 = new Label
			{
				StyleClasses = { "ItemStatus" },
				HorizontalAlignment = (HAlignment)3
			};
			val4 = val6;
			_ammoCount = val6;
			children5.Add((Control)(object)val4);
			OrderedChildCollection children6 = ((Control)val5).Children;
			TextureRect val7 = new TextureRect
			{
				Texture = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/chambered.png"),
				HorizontalAlignment = (HAlignment)1
			};
			TextureRect val8 = val7;
			_chamberedBullet = val7;
			children6.Add((Control)(object)val8);
			children4.Add((Control)val5);
			((Control)this).AddChild((Control)val);
		}

		public void Update(bool chambered, bool magazine, int count, int capacity)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			((Control)_chamberedBullet).ModulateSelfOverride = (chambered ? Color.FromHex((ReadOnlySpan<char>)"#d7df60", (Color?)null) : Color.Black);
			if (!magazine)
			{
				((Control)_bulletRender).Visible = false;
				((Control)_noMagazineLabel).Visible = true;
				((Control)_ammoCount).Visible = false;
				return;
			}
			((Control)_bulletRender).Visible = true;
			((Control)_noMagazineLabel).Visible = false;
			((Control)_ammoCount).Visible = true;
			_bulletRender.Count = count;
			_bulletRender.Capacity = capacity;
			_bulletRender.Type = ((capacity > 50) ? BulletRender.BulletType.Tiny : BulletRender.BulletType.Normal);
			_ammoCount.Text = $"x{count:00}";
		}

		public void PlayAlarmAnimation(Animation animation)
		{
			((Control)_noMagazineLabel).PlayAnimation(animation, "alarm");
		}
	}

	private sealed class RevolverStatusControl : Control
	{
		private readonly BoxContainer _bulletsList;

		public RevolverStatusControl()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_004e: Expected O, but got Unknown
			((Control)this).MinHeight = 15f;
			((Control)this).HorizontalExpand = true;
			((Control)this).VerticalAlignment = (VAlignment)2;
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				HorizontalExpand = true,
				VerticalAlignment = (VAlignment)2,
				SeparationOverride = 0
			};
			BoxContainer val2 = val;
			_bulletsList = val;
			((Control)this).AddChild((Control)(object)val2);
		}

		public void Update(int currentIndex, bool?[] bullets)
		{
			((Control)_bulletsList).RemoveAllChildren();
			int num = bullets.Length;
			string path = ((num <= 20) ? "/Textures/Interface/ItemStatus/Bullets/normal.png" : ((num > 30) ? "/Textures/Interface/ItemStatus/Bullets/tiny.png" : "/Textures/Interface/ItemStatus/Bullets/small.png"));
			Texture texture = StaticIoC.ResC.GetTexture(path);
			Texture texture2 = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/empty.png");
			FillBulletRow(currentIndex, bullets, (Control)(object)_bulletsList, texture, texture2);
		}

		private void FillBulletRow(int currentIndex, bool?[] bullets, Control container, Texture texture, Texture emptyTexture)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Expected O, but got Unknown
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			int num = bullets.Length;
			Color val = Color.FromHex((ReadOnlySpan<char>)"#b68f0e", (Color?)null);
			Color val2 = Color.FromHex((ReadOnlySpan<char>)"#d7df60", (Color?)null);
			Color val3 = Color.FromHex((ReadOnlySpan<char>)"#b50e25", (Color?)null);
			Color val4 = Color.FromHex((ReadOnlySpan<char>)"#d3745f", (Color?)null);
			Color val5 = Color.FromHex((ReadOnlySpan<char>)"#000000", (Color?)null);
			Color val6 = Color.FromHex((ReadOnlySpan<char>)"#222222", (Color?)null);
			bool flag = false;
			float num2 = 1.3f;
			for (int i = 0; i < num; i++)
			{
				bool? flag2 = bullets[i];
				Control val7 = new Control
				{
					MinSize = texture.Size * num2
				};
				if (i == currentIndex)
				{
					val7.AddChild((Control)new TextureRect
					{
						Texture = texture,
						TextureScale = new Vector2(num2, num2),
						ModulateSelfOverride = Color.LimeGreen
					});
				}
				Texture texture2 = texture;
				Color value;
				if (flag2.HasValue)
				{
					if (flag2.Value)
					{
						value = (flag ? val : val2);
					}
					else
					{
						value = (flag ? val3 : val4);
						texture2 = emptyTexture;
					}
				}
				else
				{
					value = (flag ? val5 : val6);
				}
				val7.AddChild((Control)new TextureRect
				{
					Stretch = (StretchMode)4,
					Texture = texture2,
					ModulateSelfOverride = value
				});
				flag = !flag;
				container.AddChild(val7);
			}
		}
	}

	[Dependency]
	private StackSystem _stack;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IStateManager _state;

	[Dependency]
	private AnimationPlayerSystem _animPlayer;

	[Dependency]
	private InputSystem _inputSystem;

	[Dependency]
	private SharedMapSystem _maps;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private PubgFocusViewSystem _pubgFocusView;

	[Dependency]
	private CivLocalParticleSystem _civParticles;

	[Dependency]
	private ItemPickupSystem _itemPickup;

	[Dependency]
	private GunPredictionSystem _gunPrediction;

	[Dependency]
	private RMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private CMGunSystem _cmGun;

	public static readonly EntProtoId HitscanProto = EntProtoId.op_Implicit("HitscanEffect");

	private bool _spreadOverlay;

	public bool SpreadOverlay
	{
		get
		{
			return _spreadOverlay;
		}
		set
		{
			if (_spreadOverlay != value)
			{
				_spreadOverlay = value;
				IOverlayManager val = IoCManager.Resolve<IOverlayManager>();
				if (_spreadOverlay)
				{
					val.AddOverlay((Overlay)(object)new GunSpreadOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager, _eyeManager, Timing, _inputManager, _player, this, TransformSystem));
				}
				else
				{
					val.RemoveOverlay<GunSpreadOverlay>();
				}
			}
		}
	}

	private void OnAmmoCounterCollect(EntityUid uid, AmmoCounterComponent component, ItemStatusCollectMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshControl(uid, component);
		if (component.Control != null)
		{
			args.Controls.Add(component.Control);
		}
	}

	private void RefreshControl(EntityUid uid, AmmoCounterComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AmmoCounterComponent>(uid, ref component, false))
		{
			Control? control = component.Control;
			if (control != null)
			{
				control.Orphan();
			}
			component.Control = null;
			AmmoCounterControlEvent ammoCounterControlEvent = new AmmoCounterControlEvent();
			((EntitySystem)this).RaiseLocalEvent<AmmoCounterControlEvent>(uid, ammoCounterControlEvent, false);
			AmmoCounterControlEvent ammoCounterControlEvent2 = ammoCounterControlEvent;
			if (ammoCounterControlEvent2.Control == null)
			{
				ammoCounterControlEvent2.Control = (Control?)(object)new DefaultStatusControl();
			}
			component.Control = ammoCounterControlEvent.Control;
			UpdateAmmoCount(uid, component);
		}
	}

	private void UpdateAmmoCount(EntityUid uid, AmmoCounterComponent component, int artificialIncrease = 0)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (component.Control != null)
		{
			UpdateAmmoCounterEvent updateAmmoCounterEvent = new UpdateAmmoCounterEvent
			{
				Control = component.Control,
				ArtificialIncrease = artificialIncrease
			};
			((EntitySystem)this).RaiseLocalEvent<UpdateAmmoCounterEvent>(uid, updateAmmoCounterEvent, false);
		}
	}

	public override void UpdateAmmoCount(EntityUid uid, bool prediction = true, int artificialIncrease = 0)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		AmmoCounterComponent component = default(AmmoCounterComponent);
		if ((!prediction || Timing.IsFirstTimePredicted) && ((EntitySystem)this).TryComp<AmmoCounterComponent>(uid, ref component))
		{
			UpdateAmmoCount(uid, component, artificialIncrease);
		}
	}

	protected override void InitializeBallistic()
	{
		base.InitializeBallistic();
		((EntitySystem)this).SubscribeLocalEvent<BallisticAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<BallisticAmmoProviderComponent, UpdateAmmoCounterEvent>)OnBallisticAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnBallisticAmmoCount(EntityUid uid, BallisticAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		if (args.Control is DefaultStatusControl defaultStatusControl)
		{
			defaultStatusControl.Update(GetBallisticShots(component) + args.ArtificialIncrease, component.Capacity);
		}
	}

	protected override void Cycle(EntityUid uid, BallisticAmmoProviderComponent component, MapCoordinates coordinates)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.IsFirstTimePredicted)
		{
			EntityUid? val = null;
			if (component.Entities.Count > 0)
			{
				List<EntityUid> entities = component.Entities;
				EntityUid val2 = entities[entities.Count - 1];
				component.Entities.RemoveAt(component.Entities.Count - 1);
				Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(val2), (BaseContainer)(object)component.Container, true, false, (EntityCoordinates?)null, (Angle?)null);
				EnsureShootable(val2);
			}
			else if (component.UnspawnedCount > 0)
			{
				component.UnspawnedCount--;
				EntProtoId? proto = component.Proto;
				val = ((EntitySystem)this).Spawn(proto.HasValue ? EntProtoId.op_Implicit(proto.GetValueOrDefault()) : null, coordinates, (ComponentRegistry)null, default(Angle));
				_stack.SetCount(val.Value, 1);
				EnsureShootable(val.Value);
			}
			if (val.HasValue && ((EntitySystem)this).IsClientSide(val.Value, (MetaDataComponent)null))
			{
				((EntitySystem)this).Del((EntityUid?)val.Value);
			}
			GunCycledEvent gunCycledEvent = default(GunCycledEvent);
			((EntitySystem)this).RaiseLocalEvent<GunCycledEvent>(uid, ref gunCycledEvent, false);
		}
	}

	protected override void InitializeBasicEntity()
	{
		base.InitializeBasicEntity();
		((EntitySystem)this).SubscribeLocalEvent<BasicEntityAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<BasicEntityAmmoProviderComponent, UpdateAmmoCounterEvent>)OnBasicEntityAmmoCount, (Type[])null, (Type[])null);
	}

	private void OnBasicEntityAmmoCount(EntityUid uid, BasicEntityAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		if (args.Control is DefaultStatusControl defaultStatusControl && component.Count.HasValue && component.Capacity.HasValue)
		{
			defaultStatusControl.Update(component.Count.Value, component.Capacity.Value);
		}
	}

	protected override void InitializeBattery()
	{
		base.InitializeBattery();
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, AmmoCounterControlEvent>((ComponentEventHandler<HitscanBatteryAmmoProviderComponent, AmmoCounterControlEvent>)OnControl, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<HitscanBatteryAmmoProviderComponent, UpdateAmmoCounterEvent>)OnAmmoCountUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, AmmoCounterControlEvent>((ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, AmmoCounterControlEvent>)OnControl, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, UpdateAmmoCounterEvent>)OnAmmoCountUpdate, (Type[])null, (Type[])null);
	}

	private void OnAmmoCountUpdate(EntityUid uid, BatteryAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		if (args.Control is BoxesStatusControl boxesStatusControl)
		{
			boxesStatusControl.Update(component.Shots, component.Capacity);
		}
	}

	private void OnControl(EntityUid uid, BatteryAmmoProviderComponent component, AmmoCounterControlEvent args)
	{
		args.Control = (Control?)(object)new BoxesStatusControl();
	}

	protected override void InitializeChamberMagazine()
	{
		base.InitializeChamberMagazine();
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, AmmoCounterControlEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, AmmoCounterControlEvent>)OnChamberMagazineCounter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<ChamberMagazineAmmoProviderComponent, UpdateAmmoCounterEvent>)OnChamberMagazineAmmoUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, AppearanceChangeEvent>((ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, AppearanceChangeEvent>)OnChamberMagazineAppearance, (Type[])null, (Type[])null);
	}

	private void OnChamberMagazineAppearance(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		bool flag = default(bool);
		if (args.Sprite != null && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)GunVisualLayers.Base, ref num, false) && Appearance.TryGetData<bool>(uid, (Enum)AmmoVisuals.BoltClosed, ref flag, (AppearanceComponent)null))
		{
			if (flag)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit("base"));
			}
			else
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, StateId.op_Implicit("bolt-open"));
			}
		}
	}

	protected override void OnMagazineSlotChange(EntityUid uid, MagazineAmmoProviderComponent component, ContainerModifiedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		base.OnMagazineSlotChange(uid, component, args);
		if (!("gun_chamber" != args.Container.ID))
		{
			EntRemovedFromContainerMessage val = (EntRemovedFromContainerMessage)(object)((args is EntRemovedFromContainerMessage) ? args : null);
			if (val != null && ((EntitySystem)this).IsClientSide(((ContainerModifiedMessage)val).Entity, (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)args.Entity);
			}
		}
	}

	private void OnChamberMagazineCounter(EntityUid uid, ChamberMagazineAmmoProviderComponent component, AmmoCounterControlEvent args)
	{
		args.Control = (Control?)(object)new ChamberMagazineStatusControl();
	}

	private void OnChamberMagazineAmmoUpdate(EntityUid uid, ChamberMagazineAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (args.Control is ChamberMagazineStatusControl chamberMagazineStatusControl)
		{
			EntityUid? chamberEntity = GetChamberEntity(uid);
			EntityUid? magazineEntity = GetMagazineEntity(uid);
			GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
			if (magazineEntity.HasValue)
			{
				((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref getAmmoCountEvent, false);
			}
			chamberMagazineStatusControl.Update(chamberEntity.HasValue, magazineEntity.HasValue, getAmmoCountEvent.Count, getAmmoCountEvent.Capacity);
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<AmmoCounterComponent, ItemStatusCollectMessage>((ComponentEventHandler<AmmoCounterComponent, ItemStatusCollectMessage>)OnAmmoCounterCollect, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AmmoCounterComponent, UpdateClientAmmoEvent>((ComponentEventRefHandler<AmmoCounterComponent, UpdateClientAmmoEvent>)OnUpdateClientAmmo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<MuzzleFlashEvent>((EntityEventHandler<MuzzleFlashEvent>)OnMuzzleFlash, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<HitscanEvent>((EntityEventHandler<HitscanEvent>)OnHitscan, (Type[])null, (Type[])null);
		InitializeMagazineVisuals();
		InitializeSpentAmmo();
	}

	private void OnUpdateClientAmmo(EntityUid uid, AmmoCounterComponent ammoComp, ref UpdateClientAmmoEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAmmoCount(uid, ammoComp, args.AritifialIncrease);
	}

	private void OnMuzzleFlash(MuzzleFlashEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(args.Uid);
		CreateEffect(entity, args, entity, ((ISharedPlayerManager)_player).LocalEntity, args.Offset, args.OriginOffset);
	}

	private void OnHitscan(HitscanEvent ev)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01ab: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent val2 = default(TransformComponent);
		foreach (var sprite in ev.Sprites)
		{
			SpriteSpecifier item = sprite.Sprite;
			Rsi val = (Rsi)(object)((item is Rsi) ? item : null);
			if (val != null)
			{
				EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(sprite.coordinates);
				if (((EntitySystem)this).TryComp(coordinates.EntityId, ref val2))
				{
					EntityUid val3 = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(HitscanProto), coordinates);
					SpriteComponent val4 = ((EntitySystem)this).Comp<SpriteComponent>(val3);
					TransformComponent val5 = ((EntitySystem)this).Transform(val3);
					Angle val6 = sprite.angle + _xform.GetWorldRotation(val2) - _xform.GetWorldRotation(val5);
					_xform.SetLocalRotationNoLerp(val3, val5.LocalRotation + val6, val5);
					val4[(object)EffectLayers.Unshaded].AutoAnimated = false;
					_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((val3, val4)), (Enum)EffectLayers.Unshaded, (SpriteSpecifier)(object)val);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((val3, val4)), (Enum)EffectLayers.Unshaded, StateId.op_Implicit(val.RsiState));
					_sprite.SetScale(Entity<SpriteComponent>.op_Implicit((val3, val4)), new Vector2(sprite.Distance, 1f));
					val4[(object)EffectLayers.Unshaded].Visible = true;
					Animation val7 = new Animation
					{
						Length = TimeSpan.FromSeconds(0.47999998927116394),
						AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
						{
							LayerKey = EffectLayers.Unshaded,
							KeyFrames = 
							{
								new KeyFrame(StateId.op_Implicit(val.RsiState), 0f)
							}
						} }
					};
					_animPlayer.Play(val3, val7, "hitscan-effect");
				}
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		if (!Timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		CombatModeComponent combatModeComponent = default(CombatModeComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<CombatModeComponent>(localEntity, ref combatModeComponent) || !combatModeComponent.IsInCombatMode)
		{
			return;
		}
		EntityUid value = localEntity.Value;
		if (!TryGetGun(value, out EntityUid gunEntity, out GunComponent gunComp))
		{
			return;
		}
		BoundKeyFunction val = (gunComp.UseKey ? EngineKeyFunctions.Use : EngineKeyFunctions.UseSecondary);
		if ((int)_inputSystem.CmdStates.GetState(val) != 1 && !gunComp.BurstActivated)
		{
			if (gunComp.ShotCounter != 0)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent
				{
					Gun = ((EntitySystem)this).GetNetEntity(gunEntity, (MetaDataComponent)null)
				});
			}
		}
		else
		{
			if (gunComp.NextFire > Timing.CurTime)
			{
				return;
			}
			MapCoordinates coordinates = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
			coordinates = _pubgFocusView.AdjustMapCoordinates(gunEntity, coordinates);
			if (coordinates.MapId == MapId.Nullspace)
			{
				if (gunComp.ShotCounter != 0)
				{
					((EntitySystem)this).RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent
					{
						Gun = ((EntitySystem)this).GetNetEntity(gunEntity, (MetaDataComponent)null)
					});
				}
				return;
			}
			EntityCoordinates val2;
			if (((EntitySystem)this).HasComp<VehicleTurretComponent>(gunEntity))
			{
				val2 = TransformSystem.ToCoordinates(coordinates);
			}
			else
			{
				EntityUid val3 = (((EntitySystem)this).HasComp<GunUseGunOriginComponent>(gunEntity) ? gunEntity : value);
				val2 = TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(val3), coordinates);
			}
			NetEntity? target = null;
			if (_state.CurrentState is GameplayStateBase gameplayStateBase)
			{
				target = ((EntitySystem)this).GetNetEntity(gameplayStateBase.GetClickedEntity(coordinates), (MetaDataComponent)null);
			}
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			if (localSession == null || _itemPickup.RecentItemPickUp)
			{
				return;
			}
			((EntitySystem)this).Log.Debug($"Sending shoot request tick {Timing.CurTick} / {Timing.CurTime}");
			List<EntityUid> list = _gunPrediction.ShootRequested(((EntitySystem)this).GetNetEntity(gunEntity, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(val2, (MetaDataComponent)null), target, null, localSession);
			((EntitySystem)this).RaisePredictiveEvent<RequestShootEvent>(new RequestShootEvent
			{
				Target = target,
				Coordinates = ((EntitySystem)this).GetNetCoordinates(val2, (MetaDataComponent)null),
				Gun = ((EntitySystem)this).GetNetEntity(gunEntity, (MetaDataComponent)null),
				Shot = list?.Select((EntityUid e) => e.Id).ToList(),
				LastRealTick = _rmcLagCompensation.GetLastRealTick(null)
			});
			if (_cmGun.TryGetAkimboOffHand(value, Entity<GunComponent>.op_Implicit((gunEntity, gunComp)), out var offHand))
			{
				List<EntityUid> list2 = _gunPrediction.ShootRequested(((EntitySystem)this).GetNetEntity(offHand, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(val2, (MetaDataComponent)null), target, null, localSession);
				((EntitySystem)this).RaisePredictiveEvent<RequestShootEvent>(new RequestShootEvent
				{
					Target = target,
					Coordinates = ((EntitySystem)this).GetNetCoordinates(val2, (MetaDataComponent)null),
					Gun = ((EntitySystem)this).GetNetEntity(offHand, (MetaDataComponent)null),
					Shot = list2?.Select((EntityUid e) => e.Id).ToList(),
					LastRealTick = _rmcLagCompensation.GetLastRealTick(null)
				});
			}
		}
	}

	protected override void Popup(string message, EntityUid? uid, EntityUid? user)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (uid.HasValue && user.HasValue && Timing.IsFirstTimePredicted)
		{
			PopupSystem.PopupEntity(message, uid.Value, user.Value);
		}
	}

	protected override void CreateEffect(EntityUid gunUid, MuzzleFlashEvent message, EntityUid? tracked = null, EntityUid? player = null, Vector2 offset = default(Vector2), Vector2 originOffset = default(Vector2))
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0203: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Expected O, but got Unknown
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Expected O, but got Unknown
		//IL_0390: Expected O, but got Unknown
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		if (!Timing.IsFirstTimePredicted)
		{
			return;
		}
		if (gunUid == EntityUid.Invalid)
		{
			((EntitySystem)this).Log.Debug($"Invalid Entity sent MuzzleFlashEvent (proto: {message.Prototype}, gun: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(gunUid))})");
			return;
		}
		TransformComponent val = ((EntitySystem)this).Transform(gunUid);
		EntityUid? gridUid = val.GridUid;
		MapGridComponent val2 = default(MapGridComponent);
		EntityCoordinates val3 = default(EntityCoordinates);
		if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref val2))
		{
			((EntityCoordinates)(ref val3))._002Ector(gridUid.Value, _maps.LocalToGrid(gridUid.Value, val2, val.Coordinates));
		}
		else
		{
			if (!val.MapUid.HasValue)
			{
				return;
			}
			((EntityCoordinates)(ref val3))._002Ector(val.MapUid.Value, TransformSystem.GetWorldPosition(val));
		}
		EntityUid val4 = ((EntitySystem)this).Spawn(message.Prototype, val3);
		TransformSystem.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(val4), message.Angle);
		if (tracked.HasValue)
		{
			TrackUserComponent trackUserComponent = ((EntitySystem)this).EnsureComp<TrackUserComponent>(val4);
			trackUserComponent.User = tracked;
			trackUserComponent.Offset = offset;
			trackUserComponent.OriginOffset = originOffset;
		}
		float num = 0.4f;
		TimedDespawnComponent val5 = default(TimedDespawnComponent);
		if (((EntitySystem)this).TryComp<TimedDespawnComponent>(gunUid, ref val5))
		{
			num = val5.Lifetime;
		}
		Animation val6 = new Animation
		{
			Length = TimeSpan.FromSeconds(num)
		};
		List<AnimationTrack> animationTracks = val6.AnimationTracks;
		AnimationTrackComponentProperty val7 = new AnimationTrackComponentProperty
		{
			ComponentType = typeof(SpriteComponent),
			Property = "Color",
			InterpolationMode = (AnimationInterpolationMode)0
		};
		List<KeyFrame> keyFrames = ((AnimationTrackProperty)val7).KeyFrames;
		Color white = Color.White;
		keyFrames.Add(new KeyFrame((object)((Color)(ref white)).WithAlpha(1f), 0f, (Func<float, float>)null));
		List<KeyFrame> keyFrames2 = ((AnimationTrackProperty)val7).KeyFrames;
		white = Color.White;
		keyFrames2.Add(new KeyFrame((object)((Color)(ref white)).WithAlpha(0f), num, (Func<float, float>)null));
		animationTracks.Add((AnimationTrack)val7);
		Animation val8 = val6;
		_animPlayer.Play(val4, val8, "muzzle-flash");
		PointLightComponent component = default(PointLightComponent);
		if (!((EntitySystem)this).TryComp<PointLightComponent>(val4, ref component))
		{
			component = ((EntitySystem)this).Factory.GetComponent<PointLightComponent>();
			((Component)component).NetSyncEnabled = false;
			((EntitySystem)this).AddComp<PointLightComponent>(val4, component, false);
		}
		Lights.SetEnabled(val4, true, (SharedPointLightComponent)(object)component, (MetaDataComponent)null);
		Lights.SetRadius(val4, 2f, (SharedPointLightComponent)(object)component, (MetaDataComponent)null);
		Lights.SetColor(val4, Color.FromHex((ReadOnlySpan<char>)"#cc8e2b", (Color?)null), (SharedPointLightComponent)(object)component);
		Lights.SetEnergy(val4, 5f, (SharedPointLightComponent)(object)component);
		Animation val9 = new Animation
		{
			Length = TimeSpan.FromSeconds(num),
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(PointLightComponent),
				Property = "Energy",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)5f, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)0f, num, (Func<float, float>)null)
				}
			} },
			AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
			{
				ComponentType = typeof(PointLightComponent),
				Property = "AnimatedEnable",
				InterpolationMode = (AnimationInterpolationMode)0,
				KeyFrames = 
				{
					new KeyFrame((object)true, 0f, (Func<float, float>)null)
				},
				KeyFrames = 
				{
					new KeyFrame((object)false, num, (Func<float, float>)null)
				}
			} }
		};
		AnimationPlayerComponent val10 = ((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(val4);
		_animPlayer.Stop(val4, val10, "muzzle-flash-light");
		_animPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((val4, val10)), val9, "muzzle-flash-light");
		CivMuzzleSmokeComponent civMuzzleSmokeComponent = default(CivMuzzleSmokeComponent);
		if (((EntitySystem)this).TryComp<CivMuzzleSmokeComponent>(gunUid, ref civMuzzleSmokeComponent))
		{
			_civParticles.EmitBurst(civMuzzleSmokeComponent.Preset, TransformSystem.GetMapCoordinates(gunUid, val), 1f, (float)((Angle)(ref message.Angle)).Degrees);
		}
	}

	public override void ShootProjectile(EntityUid uid, Vector2 direction, Vector2 gunVelocity, EntityUid? gunUid, EntityUid? user = null, float speed = 20f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<PredictedProjectileClientComponent>(uid);
		Physics.UpdateIsPredicted((EntityUid?)uid, (PhysicsComponent)null);
		base.ShootProjectile(uid, direction, gunVelocity, gunUid, user, speed);
	}

	protected override void InitializeMagazine()
	{
		base.InitializeMagazine();
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<MagazineAmmoProviderComponent, UpdateAmmoCounterEvent>)OnMagazineAmmoUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineAmmoProviderComponent, AmmoCounterControlEvent>((ComponentEventHandler<MagazineAmmoProviderComponent, AmmoCounterControlEvent>)OnMagazineControl, (Type[])null, (Type[])null);
	}

	private void OnMagazineAmmoUpdate(EntityUid uid, MagazineAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? magazineEntity = GetMagazineEntity(uid);
		if (!magazineEntity.HasValue)
		{
			if (args.Control is DefaultStatusControl defaultStatusControl)
			{
				defaultStatusControl.Update(0, 0);
			}
		}
		else
		{
			((EntitySystem)this).RaiseLocalEvent<UpdateAmmoCounterEvent>(magazineEntity.Value, args, false);
		}
	}

	private void OnMagazineControl(EntityUid uid, MagazineAmmoProviderComponent component, AmmoCounterControlEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? magazineEntity = GetMagazineEntity(uid);
		if (magazineEntity.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<AmmoCounterControlEvent>(magazineEntity.Value, args, false);
		}
	}

	private void InitializeMagazineVisuals()
	{
		((EntitySystem)this).SubscribeLocalEvent<MagazineVisualsComponent, ComponentInit>((ComponentEventHandler<MagazineVisualsComponent, ComponentInit>)OnMagazineVisualsInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagazineVisualsComponent, AppearanceChangeEvent>((ComponentEventRefHandler<MagazineVisualsComponent, AppearanceChangeEvent>)OnMagazineVisualsChange, (Type[])null, (Type[])null);
	}

	private void OnMagazineVisualsInit(EntityUid uid, MagazineVisualsComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.Mag, ref num, false))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.Mag, StateId.op_Implicit($"{component.MagState}-{component.MagSteps - 1}"));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.Mag, false);
			}
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.MagUnshaded, ref num, false))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.MagUnshaded, StateId.op_Implicit($"{component.MagState}-unshaded-{component.MagSteps - 1}"));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)GunVisualLayers.MagUnshaded, false);
			}
		}
	}

	private void OnMagazineVisualsChange(EntityUid uid, MagazineVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		if (sprite == null)
		{
			return;
		}
		int num2 = default(int);
		if (!args.AppearanceData.TryGetValue(AmmoVisuals.MagLoaded, out var value) || (value is bool && (bool)value))
		{
			if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoMax, out var value2))
			{
				value2 = component.MagSteps;
			}
			if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoCount, out var value3))
			{
				value3 = component.MagSteps;
			}
			int num = ContentHelpers.RoundToLevels((int)value3, (int)value2, component.MagSteps);
			if (component.ZeroOnlyOnEmpty && num == 0 && (int)value3 > 0)
			{
				num = 1;
			}
			if (num == 0 && !component.ZeroVisible)
			{
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, ref num2, false))
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, false);
				}
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, ref num2, false))
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, false);
				}
				return;
			}
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, true);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, StateId.op_Implicit($"{component.MagState}-{num}"));
			}
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, true);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, StateId.op_Implicit($"{component.MagState}-unshaded-{num}"));
			}
		}
		else
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.Mag, false);
			}
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)GunVisualLayers.MagUnshaded, false);
			}
		}
	}

	protected override void InitializeRevolver()
	{
		base.InitializeRevolver();
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, AmmoCounterControlEvent>((ComponentEventHandler<RevolverAmmoProviderComponent, AmmoCounterControlEvent>)OnRevolverCounter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, UpdateAmmoCounterEvent>((ComponentEventHandler<RevolverAmmoProviderComponent, UpdateAmmoCounterEvent>)OnRevolverAmmoUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>)OnRevolverEntRemove, (Type[])null, (Type[])null);
	}

	private void OnRevolverEntRemove(EntityUid uid, RevolverAmmoProviderComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != "revolver-ammo") && ((EntitySystem)this).IsClientSide(((ContainerModifiedMessage)args).Entity, (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel((EntityUid?)((ContainerModifiedMessage)args).Entity);
		}
	}

	private void OnRevolverAmmoUpdate(EntityUid uid, RevolverAmmoProviderComponent component, UpdateAmmoCounterEvent args)
	{
		if (args.Control is RevolverStatusControl revolverStatusControl)
		{
			revolverStatusControl.Update(component.CurrentIndex, component.Chambers);
		}
	}

	private void OnRevolverCounter(EntityUid uid, RevolverAmmoProviderComponent component, AmmoCounterControlEvent args)
	{
		args.Control = (Control?)(object)new RevolverStatusControl();
	}

	private void InitializeSpentAmmo()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpentAmmoVisualsComponent, AppearanceChangeEvent>((ComponentEventRefHandler<SpentAmmoVisualsComponent, AppearanceChangeEvent>)OnSpentAmmoAppearance, (Type[])null, (Type[])null);
	}

	private void OnSpentAmmoAppearance(EntityUid uid, SpentAmmoVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		if (sprite != null && args.AppearanceData.TryGetValue(AmmoVisuals.Spent, out var value))
		{
			string text = ((!(bool)value) ? component.State : (component.Suffix ? (component.State + "-spent") : "spent"));
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)AmmoVisualLayers.Base, StateId.op_Implicit(text));
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)AmmoVisualLayers.Tip, false);
		}
	}
}
