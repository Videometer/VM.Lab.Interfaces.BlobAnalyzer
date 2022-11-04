namespace VM.Lab.Interfaces.Autofeeder
{
	/// <summary>Enumerator with autofeeder states</summary>
	public enum AutofeederState
	{
		/// <summary></summary>
		Idle = 0,

		/// <summary></summary>
		Running,

		/// <summary></summary>
		Flushing,

		/// <summary></summary>
		Stopping,

		/// <summary></summary>
		Stopped,
	}
}
