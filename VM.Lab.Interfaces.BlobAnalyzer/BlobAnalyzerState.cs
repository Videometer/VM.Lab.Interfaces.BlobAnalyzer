namespace VM.Lab.Interfaces.BlobAnalyzer;

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

	/// <summary>Finishing and saving results</summary>
	FINALIZING_MEASUREMENT,

	/// <summary> Flushing, go to IDLE after</summary>
	FLUSHING_IDLE,
		
	/// <summary> Flushing, go to NONE after</summary>
	FLUSHING_NONE,
		
	/// <summary> Flushing, go to STOPPED after</summary>
	FLUSHING_STOPPED
}