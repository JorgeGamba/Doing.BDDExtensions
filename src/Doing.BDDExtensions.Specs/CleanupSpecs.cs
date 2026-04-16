using System;
using NUnit.Framework;
using Shouldly;

namespace Doing.BDDExtensions.Specs
{
    [TestFixture]
    public class CleanupSpecs : FeatureSpecifications
    {
        protected FeatureWithCleanup _feature;

        public override void Given() =>
            _feature = new FeatureWithCleanup();

        public override void When() =>
            ((IDisposable)_feature).Dispose();

        public class When_a_feature_with_Cleanup_is_disposed : CleanupSpecs
        {
            [Test]
            public void Should_invoke_Cleanup() =>
                _feature.CleanupWasCalled.ShouldBeTrue();
        }

        public class When_a_feature_without_Cleanup_is_disposed : CleanupSpecs
        {
            protected Exception _exception;

            public override void Given() { }

            public override void When() =>
                _exception = Catch.Exception(() => ((IDisposable)new FeatureWithoutCleanup()).Dispose());

            [Test]
            public void Should_not_throw() =>
                _exception.ShouldBeNull();
        }
    }

    public class FeatureWithCleanup : FeatureSpecifications
    {
        public bool CleanupWasCalled { get; set; }

        public override void Cleanup() =>
            CleanupWasCalled = true;
    }

    public class FeatureWithoutCleanup : FeatureSpecifications;
}
