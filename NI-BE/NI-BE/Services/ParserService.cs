using NI_BE.Parser;

namespace NI_BE.Services
{
    public class ParserService
    {
        public ParserService()
        {

        }

        public void ParseAFile(FileSystemEventArgs e)
        {
            var parsingFile = new ParsingFile();


            parsingFile.OnChanged(e);
        }
    }
}