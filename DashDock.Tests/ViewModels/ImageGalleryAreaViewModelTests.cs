using DashDock.Models.Memos;
using DashDock.Services.Memos;
using DashDock.ViewModels;
using Moq;

namespace DashDock.Tests.ViewModels
{
    [TestFixture]
    public class ImageGalleryAreaViewModelTests
    {
        private Mock<IMemosApiClient> memosApiClientMock;
        private Mock<LocalImageCacheService> localImageCacheServiceMock;

        [SetUp]
        public void SetUp()
        {
            memosApiClientMock = new Mock<IMemosApiClient>();
            localImageCacheServiceMock = new Mock<LocalImageCacheService>();
        }

        [Test]
        public void Header_デフォルト値がImageGalleryであること()
        {
            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);
            Assert.That(vm.Header, Is.EqualTo("Image Gallery"));
        }

        [Test]
        public void ImagePath_プロパティ変更通知が行われること()
        {
            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);
            var propertyChangedRaised = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ImageGalleryAreaViewModel.ImagePath))
                {
                    propertyChangedRaised = true;
                }
            };

            vm.ImagePath = @"C:\sample\test.png";

            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(vm.ImagePath, Is.EqualTo(@"C:\sample\test.png"));
        }

        [Test]
        public async Task InitializeAsyncCommand_初回実行時に画像をダウンロードおよびキャッシュしてパスを設定すること()
        {
            // Arrange
            var attachment = new AttachmentDto
            {
                Name = "attachments/img123",
                Filename = "sample.png",
                MimeType = "image/png",
            };
            var response = new ListAttachmentsResponse
            {
                Attachments = new List<AttachmentDto> { attachment },
            };
            var dummyBytes = new byte[] { 1, 2, 3, 4 };
            const string expectedPath = @"C:\cache\img123.png";

            memosApiClientMock
                .Setup(c => c.GetImageAttachmentsAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            localImageCacheServiceMock
                .Setup(s => s.ExistsLocalImage(attachment))
                .Returns(false);

            memosApiClientMock
                .Setup(c => c.DownloadAttachmentBytesAsync(attachment, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dummyBytes);

            localImageCacheServiceMock
                .Setup(s => s.SaveLocalImageAsync(attachment, dummyBytes))
                .Returns(Task.CompletedTask);

            localImageCacheServiceMock
                .Setup(s => s.GetLocalImagePath(attachment))
                .Returns(expectedPath);

            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);

            // Act
            // StartPeriodicTaskAsync を InitializeAsyncCommand 経由で実行（初回フェッチが行われる）
            var task = vm.InitializeAsyncCommand.ExecuteAsync(null);

            // アサーション（タイマー待機に入る前に初回フェッチが完了するのを少し待つ）
            await Task.Delay(200);

            // Assert
            Assert.That(vm.ImagePath, Is.EqualTo(expectedPath));
            memosApiClientMock.Verify(c => c.DownloadAttachmentBytesAsync(attachment, It.IsAny<CancellationToken>()), Times.Once);
            localImageCacheServiceMock.Verify(s => s.SaveLocalImageAsync(attachment, dummyBytes), Times.Once);
            localImageCacheServiceMock.Verify(s => s.GetLocalImagePath(attachment), Times.Once);

            vm.Dispose();
        }

        [Test]
        public async Task InitializeAsyncCommand_キャッシュが存在する場合はダウンロードを行わずにパスを設定すること()
        {
            // Arrange
            var attachment = new AttachmentDto
            {
                Name = "attachments/cached123",
                Filename = "cached.png",
                MimeType = "image/png",
            };
            var response = new ListAttachmentsResponse
            {
                Attachments = new List<AttachmentDto> { attachment },
            };
            const string expectedPath = @"C:\cache\cached123.png";

            memosApiClientMock
                .Setup(c => c.GetImageAttachmentsAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            localImageCacheServiceMock
                .Setup(s => s.ExistsLocalImage(attachment))
                .Returns(true);

            localImageCacheServiceMock
                .Setup(s => s.GetLocalImagePath(attachment))
                .Returns(expectedPath);

            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);

            // Act
            var task = vm.InitializeAsyncCommand.ExecuteAsync(null);
            await Task.Delay(200);

            // Assert
            Assert.That(vm.ImagePath, Is.EqualTo(expectedPath));
            memosApiClientMock.Verify(c => c.DownloadAttachmentBytesAsync(It.IsAny<AttachmentDto>(), It.IsAny<CancellationToken>()), Times.Never);
            localImageCacheServiceMock.Verify(s => s.SaveLocalImageAsync(It.IsAny<AttachmentDto>(), It.IsAny<byte[]>()), Times.Never);
            localImageCacheServiceMock.Verify(s => s.GetLocalImagePath(attachment), Times.Once);

            vm.Dispose();
        }

        [Test]
        public async Task InitializeAsyncCommand_添付ファイル一覧が空の場合はImagePathを更新しないこと()
        {
            // Arrange
            var response = new ListAttachmentsResponse
            {
                Attachments = new List<AttachmentDto>(),
            };

            memosApiClientMock
                .Setup(c => c.GetImageAttachmentsAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);

            // Act
            var task = vm.InitializeAsyncCommand.ExecuteAsync(null);
            await Task.Delay(200);

            // Assert
            Assert.That(vm.ImagePath, Is.Null);
            memosApiClientMock.Verify(c => c.DownloadAttachmentBytesAsync(It.IsAny<AttachmentDto>(), It.IsAny<CancellationToken>()), Times.Never);

            vm.Dispose();
        }

        [Test]
        public async Task InitializeAsyncCommand_例外発生時にクラッシュせずハンドリングされること()
        {
            // Arrange
            memosApiClientMock
                .Setup(c => c.GetImageAttachmentsAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network failure"));

            var vm = new ImageGalleryAreaViewModel(memosApiClientMock.Object, localImageCacheServiceMock.Object);

            // Act & Assert (例外がスローされず正常終了すること)
            Assert.DoesNotThrowAsync(async () =>
            {
                var task = vm.InitializeAsyncCommand.ExecuteAsync(null);
                await Task.Delay(200);
            });

            Assert.That(vm.ImagePath, Is.Null);
            vm.Dispose();
        }
    }
}