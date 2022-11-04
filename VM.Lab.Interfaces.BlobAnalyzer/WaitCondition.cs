namespace VM.Lab.Interfaces.Autofeeder
{
	/// <summary>Wait conditions used when stopping the autofeeder</summary>
	public enum WaitCondition
	{
		/// <summary>Don't wait for anything</summary>
		DontWait,
		/// <summary>Wait for all queues to be empty</summary>
		Wait_All_Queues_Empty,
		/// <summary>Wait only for the capture thread to end</summary>
		Wait_CaptureThread_Done
	}
}
