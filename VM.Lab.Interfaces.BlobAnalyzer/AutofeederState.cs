namespace VM.Lab.Interfaces.BlobAnalyzer
{
	/// <summary>Enumerator with autofeeder states</summary>
	public enum AutofeederState
	{
		/// <summary>Blob Analyzer is ready for commands, a recipe can also be loaded</summary>
		Idle = 0,

		/// <summary>Blob Analyzer is working</summary>
		Running,

		/// <summary>Blob analyzer is flushing</summary>
		Flushing,

		/// <summary>Blob analyzer is stopping</summary>
		Stopping,

		/// <summary>Blob analyzer is stopped</summary>
		Stopped,
	}
}
