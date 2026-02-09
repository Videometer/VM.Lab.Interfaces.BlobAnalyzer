namespace VM.Lab.Interfaces.BlobAnalyzer;

/// <summary>Interface for controlling the autofeeder</summary>
public interface IAutofeederControlListener
{
	/// <summary>
	/// Loads a recipe. If <paramref name="recipeName"/> is an absolute path (local or cloud), 
	/// it is loaded from that path. Otherwise, it is loaded from the current active workspace.
	/// </summary>
	/// <param name="recipeName">The path or name of the recipe.</param>
	void LoadRecipe(string recipeName);
	
	/// <summary>Start a new measurement</summary>
	/// <param name="id">ID of the sample</param>
	/// <param name="initials">Operator initials</param>
	/// <param name="comments">Operator comments</param>
	/// <param name="predictionResultName">Result filename (excel / xml file)</param>
	/// <param name="blobCollectionName">
	///		Blob Collection name,
	///		will be saved as {blobCollectionPath}\blobCollectionName\blobCollectionName.blobs
	/// </param>
	void Start(string id, string initials, string comments, string predictionResultName, string blobCollectionName);

	/// <summary>Prepare for the new measurement by filling in required field</summary>
	/// <param name="id">ID of the sample</param>
	/// <param name="initials">Operator initials</param>
	/// <param name="comments">Operator comments</param>
	/// <param name="predictionResultName">Result filename (excel / xml file)</param>
	/// <param name="blobCollectionName">
	///		Blob Collection name,
	///		will be saved as {blobCollectionPath}\blobCollectionName\blobCollectionName.blobs
	/// </param>
	void PrepareForStart(string id, string initials, string comments, string predictionResultName, string blobCollectionName);
		
	/// <summary>Stop/Pause the current measurement</summary>
	void Stop();

	/// <summary>Flush the conveyor</summary>
	void Flush();

	/// <summary>Finish the actual measurement</summary>
	void Finish();
}