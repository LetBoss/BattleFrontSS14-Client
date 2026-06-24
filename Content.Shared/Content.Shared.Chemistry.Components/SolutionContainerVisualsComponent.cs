using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
public sealed class SolutionContainerVisualsComponent : Component, ISerializationGenerated<SolutionContainerVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int MaxFillLevels;

	[DataField(null, false, 1, false, false, null)]
	public string? FillBaseName;

	[DataField(null, false, 1, false, false, null)]
	public SolutionContainerLayers Layer;

	[DataField(null, false, 1, false, false, null)]
	public SolutionContainerLayers BaseLayer = SolutionContainerLayers.Base;

	[DataField(null, false, 1, false, false, null)]
	public SolutionContainerLayers OverlayLayer = SolutionContainerLayers.Overlay;

	[DataField(null, false, 1, false, false, null)]
	public bool ChangeColor = true;

	[DataField(null, false, 1, false, false, null)]
	public string? EmptySpriteName;

	[DataField(null, false, 1, false, false, null)]
	public Color EmptySpriteColor = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public bool Metamorphic;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? MetamorphicDefaultSprite;

	[DataField(null, false, 1, false, false, null)]
	public LocId MetamorphicNameFull = LocId.op_Implicit("transformable-container-component-glass");

	[DataField(null, false, 1, false, false, null)]
	public string? SolutionName;

	[DataField(null, false, 1, false, false, null)]
	public string InitialDescription = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string? InHandsFillBaseName;

	[DataField(null, false, 1, false, false, null)]
	public int InHandsMaxFillLevels;

	[DataField(null, false, 1, false, false, null)]
	public string? EquippedFillBaseName;

	[DataField(null, false, 1, false, false, null)]
	public int EquippedMaxFillLevels;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SolutionContainerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SolutionContainerVisualsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SolutionContainerVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			int MaxFillLevelsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxFillLevels, ref MaxFillLevelsTemp, hookCtx, false, context))
			{
				MaxFillLevelsTemp = MaxFillLevels;
			}
			target.MaxFillLevels = MaxFillLevelsTemp;
			string FillBaseNameTemp = null;
			if (!serialization.TryCustomCopy<string>(FillBaseName, ref FillBaseNameTemp, hookCtx, false, context))
			{
				FillBaseNameTemp = FillBaseName;
			}
			target.FillBaseName = FillBaseNameTemp;
			SolutionContainerLayers LayerTemp = SolutionContainerLayers.Fill;
			if (!serialization.TryCustomCopy<SolutionContainerLayers>(Layer, ref LayerTemp, hookCtx, false, context))
			{
				LayerTemp = Layer;
			}
			target.Layer = LayerTemp;
			SolutionContainerLayers BaseLayerTemp = SolutionContainerLayers.Fill;
			if (!serialization.TryCustomCopy<SolutionContainerLayers>(BaseLayer, ref BaseLayerTemp, hookCtx, false, context))
			{
				BaseLayerTemp = BaseLayer;
			}
			target.BaseLayer = BaseLayerTemp;
			SolutionContainerLayers OverlayLayerTemp = SolutionContainerLayers.Fill;
			if (!serialization.TryCustomCopy<SolutionContainerLayers>(OverlayLayer, ref OverlayLayerTemp, hookCtx, false, context))
			{
				OverlayLayerTemp = OverlayLayer;
			}
			target.OverlayLayer = OverlayLayerTemp;
			bool ChangeColorTemp = false;
			if (!serialization.TryCustomCopy<bool>(ChangeColor, ref ChangeColorTemp, hookCtx, false, context))
			{
				ChangeColorTemp = ChangeColor;
			}
			target.ChangeColor = ChangeColorTemp;
			string EmptySpriteNameTemp = null;
			if (!serialization.TryCustomCopy<string>(EmptySpriteName, ref EmptySpriteNameTemp, hookCtx, false, context))
			{
				EmptySpriteNameTemp = EmptySpriteName;
			}
			target.EmptySpriteName = EmptySpriteNameTemp;
			Color EmptySpriteColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(EmptySpriteColor, ref EmptySpriteColorTemp, hookCtx, false, context))
			{
				EmptySpriteColorTemp = serialization.CreateCopy<Color>(EmptySpriteColor, hookCtx, context, false);
			}
			target.EmptySpriteColor = EmptySpriteColorTemp;
			bool MetamorphicTemp = false;
			if (!serialization.TryCustomCopy<bool>(Metamorphic, ref MetamorphicTemp, hookCtx, false, context))
			{
				MetamorphicTemp = Metamorphic;
			}
			target.Metamorphic = MetamorphicTemp;
			SpriteSpecifier MetamorphicDefaultSpriteTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(MetamorphicDefaultSprite, ref MetamorphicDefaultSpriteTemp, hookCtx, true, context))
			{
				MetamorphicDefaultSpriteTemp = serialization.CreateCopy<SpriteSpecifier>(MetamorphicDefaultSprite, hookCtx, context, false);
			}
			target.MetamorphicDefaultSprite = MetamorphicDefaultSpriteTemp;
			LocId MetamorphicNameFullTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(MetamorphicNameFull, ref MetamorphicNameFullTemp, hookCtx, false, context))
			{
				MetamorphicNameFullTemp = serialization.CreateCopy<LocId>(MetamorphicNameFull, hookCtx, context, false);
			}
			target.MetamorphicNameFull = MetamorphicNameFullTemp;
			string SolutionNameTemp = null;
			if (!serialization.TryCustomCopy<string>(SolutionName, ref SolutionNameTemp, hookCtx, false, context))
			{
				SolutionNameTemp = SolutionName;
			}
			target.SolutionName = SolutionNameTemp;
			string InitialDescriptionTemp = null;
			if (InitialDescription == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(InitialDescription, ref InitialDescriptionTemp, hookCtx, false, context))
			{
				InitialDescriptionTemp = InitialDescription;
			}
			target.InitialDescription = InitialDescriptionTemp;
			string InHandsFillBaseNameTemp = null;
			if (!serialization.TryCustomCopy<string>(InHandsFillBaseName, ref InHandsFillBaseNameTemp, hookCtx, false, context))
			{
				InHandsFillBaseNameTemp = InHandsFillBaseName;
			}
			target.InHandsFillBaseName = InHandsFillBaseNameTemp;
			int InHandsMaxFillLevelsTemp = 0;
			if (!serialization.TryCustomCopy<int>(InHandsMaxFillLevels, ref InHandsMaxFillLevelsTemp, hookCtx, false, context))
			{
				InHandsMaxFillLevelsTemp = InHandsMaxFillLevels;
			}
			target.InHandsMaxFillLevels = InHandsMaxFillLevelsTemp;
			string EquippedFillBaseNameTemp = null;
			if (!serialization.TryCustomCopy<string>(EquippedFillBaseName, ref EquippedFillBaseNameTemp, hookCtx, false, context))
			{
				EquippedFillBaseNameTemp = EquippedFillBaseName;
			}
			target.EquippedFillBaseName = EquippedFillBaseNameTemp;
			int EquippedMaxFillLevelsTemp = 0;
			if (!serialization.TryCustomCopy<int>(EquippedMaxFillLevels, ref EquippedMaxFillLevelsTemp, hookCtx, false, context))
			{
				EquippedMaxFillLevelsTemp = EquippedMaxFillLevels;
			}
			target.EquippedMaxFillLevels = EquippedMaxFillLevelsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SolutionContainerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionContainerVisualsComponent cast = (SolutionContainerVisualsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionContainerVisualsComponent cast = (SolutionContainerVisualsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionContainerVisualsComponent def = (SolutionContainerVisualsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SolutionContainerVisualsComponent Instantiate()
	{
		return new SolutionContainerVisualsComponent();
	}
}
