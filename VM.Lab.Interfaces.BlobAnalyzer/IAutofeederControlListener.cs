using System.Threading.Tasks;

namespace VM.Lab.Interfaces.BlobAnalyzer
{
	/// <summary>Interface for controlling the autofeeder</summary>
	public interface IAutofeederControlListener
	{
		/// <summary>
		/// Loads a recipe
		/// if an absolute path (local or cloud), the recipe is loaded given the absolute path
		/// if not an absolute path, the recipe is loaded from the current active workspace
		/// </summary>
		/// <param name="recipeName"></param>
		/// <returns></returns>
		Task LoadRecipe(string recipeName);
		
		/// <summary>Start a new measurement</summary>
		/// <param name="id">ID of the sample</param>
		/// <param name="initials">Operator initials</param>
		/// <param name="comments">Operator comments</param>
		Task Start(string id, string initials, string comments);

		/// <summary>Stop/Pause the current measurement</summary>
		/// This is wanted e.g. in the case of stopping due to low coverage.</param>
		Task Stop();

		/// <summary>Flush the conveyor</summary>
		Task Flush();

		/// <summary>Finish the actual measurement</summary>
		Task Finish();
	}
}