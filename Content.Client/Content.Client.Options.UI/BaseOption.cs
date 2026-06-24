namespace Content.Client.Options.UI;

public abstract class BaseOption(OptionsTabControlRow controller)
{
	protected virtual void ValueChanged()
	{
		controller.ValueChanged();
	}

	public abstract void LoadValue();

	public abstract void SaveValue();

	public abstract void ResetToDefault();

	public abstract bool IsModified();

	public abstract bool IsModifiedFromDefault();
}
