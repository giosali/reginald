namespace Reginald.Tests
{
    using System;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Data.ShellItems;
    using Xunit;

    public class ShellTests
    {
        [Fact]
        public void GetKnownFolder_WhenGivenGuid_ShouldReturnIKnownFolder()
        {
            Guid guid = Shell.ApplicationsFolderGuid;
            IKnownFolder folder = Shell.GetKnownFolder(guid);
            Assert.Equal(typeof(NonFileSystemKnownFolder), folder.GetType());
        }
    }
}
