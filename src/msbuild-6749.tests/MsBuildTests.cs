using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Build.Evaluation;
using Xunit;

namespace msbuild_6749.tests
{
    public class MsBuildTests
    {
        public MsBuildTests()
        {
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
        }

        [Fact]
        public void TestPackageVersionIsImported()
        {
            var projectPath = Path.GetFullPath("./MockData/Dummy.csproj");
            var propsPath = Path.GetFullPath("./MockData/Directory.Build.props");
            
            var project = new Project(projectPath);
            project.Should().NotBeNull();

            var packageVersionProperty = project.GetProperty("PackageVersion");
            packageVersionProperty.Should().NotBeNull();

            packageVersionProperty.IsImported.Should().BeFalse();
            packageVersionProperty.EvaluatedValue.Should().Be("10.5.1");
            packageVersionProperty.UnevaluatedValue.Should().Be("$(MyPackageVersion)");
            
            var myPackageVersionProperty = project.GetProperty("MyPackageVersion");
            myPackageVersionProperty.Should().NotBeNull();
            myPackageVersionProperty.Xml.ContainingProject.FullPath.Should().Be(propsPath);

            myPackageVersionProperty.IsImported.Should().BeTrue();
            myPackageVersionProperty.EvaluatedValue.Should().Be("10.5.1");
            myPackageVersionProperty.UnevaluatedValue.Should().Be("10.5.1");
        }
        
        [Fact]
        public void TestPackageReferenceVersionIsImported()
        {
            var projectPath = Path.GetFullPath("./MockData/Dummy.csproj");
            var propsPath = Path.GetFullPath("./MockData/Directory.Build.props");
            
            var project = new Project(projectPath);
            project.Should().NotBeNull();

            var packageReferenceItem = 
                project.GetItems("PackageReference").First();

            var packageVersionItem = 
                packageReferenceItem.GetMetadata("Version");

            packageVersionItem.Should().NotBeNull();
            packageVersionItem.IsImported.Should().BeFalse();
            packageVersionItem.EvaluatedValue.Should().Be("10.5.1");
            packageVersionItem.UnevaluatedValue.Should().Be("$(MyPackageVersion)");
        }
    }
}