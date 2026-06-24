using Robust.Shared.GameObjects;

namespace Content.Shared.Instruments;

public abstract class SharedInstrumentSystem : EntitySystem
{
	public abstract bool ResolveInstrument(EntityUid uid, ref SharedInstrumentComponent? component);

	public virtual void SetupRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? instrument = null)
	{
	}

	public virtual void EndRenderer(EntityUid uid, bool fromStateChange, SharedInstrumentComponent? instrument = null)
	{
	}

	public void SetInstrumentProgram(EntityUid uid, SharedInstrumentComponent component, byte program, byte bank)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		component.InstrumentBank = bank;
		component.InstrumentProgram = program;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
