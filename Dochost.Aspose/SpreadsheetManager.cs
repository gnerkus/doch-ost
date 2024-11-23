using Aspose.Cells;

namespace Dochost.Aspose;

public class SpreadsheetManager
{
    public SpreadsheetManager()
    {
        var license = new License();

        try
        {
            license.SetLicense("Aspose.Totalfor.NET.lic");
        }
        catch (Exception e)
        {
            Console.WriteLine($"There was an error setting the license: {e.Message}");
        }
    }
}