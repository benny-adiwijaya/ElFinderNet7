
using elFinder.Net.Core;
using elFinder.Net.Core.Services;
using elFinder.Net.Core.Services.Drawing;
using elFinder.Net.Drivers.FileSystem;
using elFinder.Net.Drivers.FileSystem.Services;

namespace ElFinderNet7.Services
{
    public class ApplicationFileSystemDriver : FileSystemDriver
    {
        public ApplicationFileSystemDriver(IPathParser pathParser,
            IPictureEditor pictureEditor,
            IVideoEditor videoEditor,
            IZipDownloadPathProvider zipDownloadPathProvider,
            IZipFileArchiver zipFileArchiver,
            IThumbnailBackgroundGenerator thumbnailBackgroundGenerator,
            ICryptographyProvider cryptographyProvider,
            IConnector connector,
            IConnectorManager connectorManager,
            ITempFileCleaner tempFileCleaner) : base(pathParser, pictureEditor, videoEditor, zipDownloadPathProvider, zipFileArchiver, thumbnailBackgroundGenerator, cryptographyProvider, connector, connectorManager, tempFileCleaner)
        {
        }

    }
}