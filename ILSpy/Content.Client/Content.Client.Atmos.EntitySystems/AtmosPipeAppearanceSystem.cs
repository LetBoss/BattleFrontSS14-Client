using System;
using Content.Client.SubFloor;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosPipeAppearanceSystem : SharedAtmosPipeAppearanceSystem
{
	private enum PipeConnectionLayer : byte
	{
		NorthConnection = 1,
		SouthConnection = 2,
		EastConnection = 8,
		WestConnection = 4
	}

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PipeAppearanceComponent, ComponentInit>((ComponentEventHandler<PipeAppearanceComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PipeAppearanceComponent, AppearanceChangeEvent>((ComponentEventRefHandler<PipeAppearanceComponent, AppearanceChangeEvent>)OnAppearanceChanged, (Type[])null, new Type[1] { typeof(SubFloorHideSystem) });
	}

	private void OnInit(EntityUid uid, PipeAppearanceComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			return;
		}
		AtmosPipeLayersComponent atmosPipeLayers;
		int numberOfPipeLayers = GetNumberOfPipeLayers(uid, out atmosPipeLayers);
		PipeConnectionLayer[] values = Enum.GetValues<PipeConnectionLayer>();
		for (int i = 0; i < values.Length; i++)
		{
			PipeConnectionLayer layer = values[i];
			for (byte b = 0; b < numberOfPipeLayers; b++)
			{
				string text = layer.ToString() + b;
				int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, item)), text);
				_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, item)), num, component.Sprite[b].RsiPath, (StateId?)null);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), num, StateId.op_Implicit(component.Sprite[b].RsiState));
				_sprite.LayerSetDirOffset(Entity<SpriteComponent>.op_Implicit((uid, item)), num, ToOffset(layer));
			}
		}
	}

	private void HideAllPipeConnection(Entity<SpriteComponent> entity, AtmosPipeLayersComponent? atmosPipeLayers, int numberOfPipeLayers)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent comp = entity.Comp;
		PipeConnectionLayer[] values = Enum.GetValues<PipeConnectionLayer>();
		int num = default(int);
		for (int i = 0; i < values.Length; i++)
		{
			PipeConnectionLayer pipeConnectionLayer = values[i];
			for (byte b = 0; b < numberOfPipeLayers; b++)
			{
				string text = pipeConnectionLayer.ToString() + b;
				if (_sprite.LayerMapTryGet(entity.AsNullable(), text, ref num, false))
				{
					comp[num].Visible = false;
				}
			}
		}
	}

	private void OnAppearanceChanged(EntityUid uid, PipeAppearanceComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite == null || !args.Sprite.Visible)
		{
			return;
		}
		AtmosPipeLayersComponent atmosPipeLayers;
		int numberOfPipeLayers = GetNumberOfPipeLayers(uid, out atmosPipeLayers);
		int num = default(int);
		if (!_appearance.TryGetData<int>(uid, (Enum)PipeVisuals.VisualState, ref num, args.Component))
		{
			HideAllPipeConnection(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), atmosPipeLayers, numberOfPipeLayers);
			return;
		}
		Color white = default(Color);
		if (!_appearance.TryGetData<Color>(uid, (Enum)PipeColorVisuals.Color, ref white, args.Component))
		{
			white = Color.White;
		}
		int num2 = default(int);
		for (byte b = 0; b < numberOfPipeLayers; b++)
		{
			PipeDirection pipeDirection = ((PipeDirection)(0xF & (num >> 4 * b))).RotatePipeDirection(Angle.op_Implicit(-((EntitySystem)this).Transform(uid).LocalRotation));
			PipeConnectionLayer[] values = Enum.GetValues<PipeConnectionLayer>();
			for (int i = 0; i < values.Length; i++)
			{
				PipeConnectionLayer pipeConnectionLayer = values[i];
				string text = pipeConnectionLayer.ToString() + b;
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), text, ref num2, false))
				{
					ISpriteLayer val = args.Sprite[num2];
					PipeDirection other = (PipeDirection)pipeConnectionLayer;
					bool flag = pipeDirection.HasDirection(other);
					val.Visible = val.Visible && flag;
					if (flag)
					{
						val.Color = white;
					}
				}
			}
		}
	}

	private DirectionOffset ToOffset(PipeConnectionLayer layer)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return (DirectionOffset)(layer switch
		{
			PipeConnectionLayer.NorthConnection => 3, 
			PipeConnectionLayer.EastConnection => 2, 
			PipeConnectionLayer.WestConnection => 1, 
			_ => 0, 
		});
	}
}
