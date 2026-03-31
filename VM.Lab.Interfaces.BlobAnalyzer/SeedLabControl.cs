namespace VM.Lab.Interfaces.BlobAnalyzer;

	
/// <summary> 
/// SeedLab communication controller
/// Types that implement this class will be detected and gives the implementer the ability to control of SeedLab
/// </summary>
public abstract class SeedLabControl : IDisposable
{
	/// <summary>Implementer of autofeeder control commands</summary>
	protected ISeedLabControlListener Listener;

	/// <summary> Base class for autofeeder controller </summary>
	/// <param name="listener">Implementer of autofeeder control commands</param>
	public SeedLabControl(ISeedLabControlListener listener)
	{
		Listener = listener;
	}

	/// <summary>Tells the controller that the blob analyzer has changed state</summary>
	public abstract void StateChanged(BlobAnalyzerState newState);

	public abstract void BroadcastError();

	#region IDisposable Support
	private bool _isDisposed = false; // To detect redundant calls

	/// <summary>
	/// Dispose duh
	/// </summary>
	/// <param name="disposing"></param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_isDisposed)
		{
			if (disposing)
			{
				
			}
			_isDisposed = true;
		}
	}


	/// <summary>
	/// This code added to correctly implement the disposable pattern.
	/// </summary>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		Dispose(true);
	}
	#endregion
}