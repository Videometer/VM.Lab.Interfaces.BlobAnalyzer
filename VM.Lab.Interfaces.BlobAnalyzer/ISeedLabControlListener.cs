namespace VM.Lab.Interfaces.BlobAnalyzer;

/// <summary>Interface for controlling the autofeeder</summary>
public interface ISeedLabControlListener : IAutofeederControlListener
{
	/// <summary>
	/// Configure the naming of delivery bins. Applicable to SeedLab, SeedSorter etc.
	///
	/// The mapping is between a generic bin tag (ex. M96_1 for the first 96-well MTP plate) and a bin ID (barcode, name, accession etc.)
	///
	/// Mappings with invalid tags are ignored.
	///
	/// If a non-empty mapping is provided, SeedLab and SeedSorter will not prompt about which bin mapping to use. 
	/// </summary>
	/// <param name="mapping">Dictionary of bin tag and bin ID</param>
	void SetBinIds(Dictionary<string, string> mapping);
}