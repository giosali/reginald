using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.Base;
using System;
using Xunit;

namespace Reginald.Tests
{
    public class ApplicationsTests
    {
        [Fact]
        public void GetKnownFolder_WhenGivenGuid_ShouldReturnIKnownFolder()
        {
            Guid guid = Constants.ApplicationsGuid;
            IKnownFolder folder = Applications.GetKnownFolder(guid);
            Assert.Equal(typeof(NonFileSystemKnownFolder), folder.GetType());
        }
    }
}
