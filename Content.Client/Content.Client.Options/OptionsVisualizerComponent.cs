using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Options;

[RegisterComponent]
public sealed class OptionsVisualizerComponent : Component, ISerializationGenerated<OptionsVisualizerComponent>, ISerializationGenerated
{
	[DataDefinition]
	public sealed class LayerDatum : ISerializationGenerated<LayerDatum>, ISerializationGenerated
	{
		[DataField(null, false, 1, false, false, null)]
		public OptionVisualizerOptions Options { get; set; }

		[DataField(null, false, 1, false, false, null)]
		public PrototypeLayerData Data { get; set; }

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref LayerDatum target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (serialization.TryCustomCopy<LayerDatum>(this, ref target, hookCtx, false, context))
			{
				return;
			}
			OptionVisualizerOptions options = OptionVisualizerOptions.Default;
			if (!serialization.TryCustomCopy<OptionVisualizerOptions>(Options, ref options, hookCtx, false, context))
			{
				options = Options;
			}
			target.Options = options;
			PrototypeLayerData data = null;
			if (Data == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<PrototypeLayerData>(Data, ref data, hookCtx, false, context))
			{
				if (Data == null)
				{
					data = null;
				}
				else
				{
					serialization.CopyTo<PrototypeLayerData>(Data, ref data, hookCtx, context, true);
				}
			}
			target.Data = data;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref LayerDatum target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			LayerDatum target2 = (LayerDatum)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public LayerDatum Instantiate()
		{
			return new LayerDatum();
		}
	}

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<string, LayerDatum[]> Visuals;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OptionsVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (OptionsVisualizerComponent)(object)val;
		if (!serialization.TryCustomCopy<OptionsVisualizerComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, LayerDatum[]> visuals = null;
			if (Visuals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, LayerDatum[]>>(Visuals, ref visuals, hookCtx, true, context))
			{
				visuals = serialization.CreateCopy<Dictionary<string, LayerDatum[]>>(Visuals, hookCtx, context, false);
			}
			target.Visuals = visuals;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OptionsVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OptionsVisualizerComponent target2 = (OptionsVisualizerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OptionsVisualizerComponent target2 = (OptionsVisualizerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OptionsVisualizerComponent target2 = (OptionsVisualizerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OptionsVisualizerComponent Instantiate()
	{
		return new OptionsVisualizerComponent();
	}
}
