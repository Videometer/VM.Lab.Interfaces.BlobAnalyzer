namespace VM.Lab.Interfaces.BlobAnalyzer
{
	/// <summary>Enumerator with autofeeder states</summary>
	public enum BlobAnalyzerState
	{
		/// <summary>Not ready / no recipe loaded yet.</summary>
		None,

		/// <summary>A recipe is being loaded</summary>
		LOADING_RECIPE,
		
		/// <summary>Idle / Ready to start</summary>
		IDLE,

		/// <summary>Starting a measurement</summary>
		STARTING_MEASUREMENT,
		
		/// <summary>Measuring</summary>
		MEASURING,
		
		/// <summary>Resuming a measurement after being in stopped state </summary>
		RESUMING_MEASUREMENT,

		/// <summary>Stopping pipeline, waiting for data to be processed </summary>
		STOPPING_PIPELINE,
		
		/// <summary>Stopping pipeline, waiting for data to be processed, when done perform a flush </summary>
		STOPPING_PIPELINE_FLUSH,
		
		/// <summary>Device stopped, we can resume or finish the measurement</summary>
		STOPPED,

		/// <summary>Finising and saving results</summary>
		FINALIZING_MEASUREMENT,

		/// <summary> Flusing, go to stopped after</summary>
		FLUSHING_IDLE,
		FLUSHING_NONE,
		FLUSHING_STOPPED
	}
}
