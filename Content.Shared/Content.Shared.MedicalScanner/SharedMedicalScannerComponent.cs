using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.MedicalScanner;

public abstract class SharedMedicalScannerComponent : Component, ISerializationGenerated<SharedMedicalScannerComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum MedicalScannerVisuals : byte
	{
		Status
	}

	[Serializable]
	[NetSerializable]
	public enum MedicalScannerStatus : byte
	{
		Off,
		Open,
		Red,
		Death,
		Green,
		Yellow
	}

	public SharedMedicalScannerComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedMedicalScannerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedMedicalScannerComponent)(object)definitionCast;
		serialization.TryCustomCopy<SharedMedicalScannerComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedMedicalScannerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMedicalScannerComponent cast = (SharedMedicalScannerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMedicalScannerComponent cast = (SharedMedicalScannerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedMedicalScannerComponent def = (SharedMedicalScannerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedMedicalScannerComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
