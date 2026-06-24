using System;
using Content.Shared.DeviceLinking;
using Content.Shared.Materials;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cloning;

[RegisterComponent]
public sealed class CloningPodComponent : Component, ISerializationGenerated<CloningPodComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<SinkPortPrototype> PodPort = ProtoId<SinkPortPrototype>.op_Implicit("CloningPodReceiver");

	[ViewVariables]
	public ContainerSlot BodyContainer;

	[ViewVariables]
	public float CloningProgress;

	[ViewVariables]
	public int UsedBiomass = 70;

	[ViewVariables]
	public bool FailedClone;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<MaterialPrototype> RequiredMaterial = ProtoId<MaterialPrototype>.op_Implicit("Biomass");

	[DataField(null, false, 1, false, false, null)]
	public float CloningTime = 30f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId MobSpawnId = EntProtoId.op_Implicit("MobAbomination");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier ScreamSound = (SoundSpecifier)new SoundCollectionSpecifier("ZombieScreams", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVolume(4f)
	};

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public CloningPodStatus Status;

	[ViewVariables]
	public EntityUid? ConnectedConsole;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CloningPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CloningPodComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CloningPodComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<SinkPortPrototype> PodPortTemp = default(ProtoId<SinkPortPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<SinkPortPrototype>>(PodPort, ref PodPortTemp, hookCtx, false, context))
			{
				PodPortTemp = serialization.CreateCopy<ProtoId<SinkPortPrototype>>(PodPort, hookCtx, context, false);
			}
			target.PodPort = PodPortTemp;
			ProtoId<MaterialPrototype> RequiredMaterialTemp = default(ProtoId<MaterialPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<MaterialPrototype>>(RequiredMaterial, ref RequiredMaterialTemp, hookCtx, false, context))
			{
				RequiredMaterialTemp = serialization.CreateCopy<ProtoId<MaterialPrototype>>(RequiredMaterial, hookCtx, context, false);
			}
			target.RequiredMaterial = RequiredMaterialTemp;
			float CloningTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CloningTime, ref CloningTimeTemp, hookCtx, false, context))
			{
				CloningTimeTemp = CloningTime;
			}
			target.CloningTime = CloningTimeTemp;
			EntProtoId MobSpawnIdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(MobSpawnId, ref MobSpawnIdTemp, hookCtx, false, context))
			{
				MobSpawnIdTemp = serialization.CreateCopy<EntProtoId>(MobSpawnId, hookCtx, context, false);
			}
			target.MobSpawnId = MobSpawnIdTemp;
			SoundSpecifier ScreamSoundTemp = null;
			if (ScreamSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(ScreamSound, ref ScreamSoundTemp, hookCtx, true, context))
			{
				ScreamSoundTemp = serialization.CreateCopy<SoundSpecifier>(ScreamSound, hookCtx, context, false);
			}
			target.ScreamSound = ScreamSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CloningPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CloningPodComponent cast = (CloningPodComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CloningPodComponent cast = (CloningPodComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CloningPodComponent def = (CloningPodComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CloningPodComponent Instantiate()
	{
		return new CloningPodComponent();
	}
}
