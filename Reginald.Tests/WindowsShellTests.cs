namespace Reginald.Tests
{
    using System;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Core.IO;
    using Xunit;

    public class WindowsShellTests
    {
        [Fact]
        public void GetKnownFolder_WhenGivenGuid_ShouldReturnIKnownFolder()
        {
            Guid guid = WindowsShell.ApplicationsFolderGuid;
            IKnownFolder folder = WindowsShell.GetKnownFolder(guid);
            Assert.Equal(typeof(NonFileSystemKnownFolder), folder.GetType());
        }
    }
}
