using NUnit.Framework;
using Chartboost.Editor;

namespace Chartboost.Threading
{
    public class VersionValidator
    {
        private const string UnityPackageManagerPackageName = "com.chartboost.unity.threading";
        private const string NuGetPackageName = "Chartboost.CSharp.Threading.Unity";
        
        [Test]
        public void ValidateVersion() 
            => VersionCheck.ValidateVersions(UnityPackageManagerPackageName, NuGetPackageName);
    }
}
