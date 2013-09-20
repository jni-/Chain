using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace Chain.Test
{
    [TestFixture]
    public class ChainTest
    {
        private ChainTestHelper _helper;

        [TestFixtureSetUp]
        public void CreateHelper()
        {
            _helper = new ChainTestHelper();
        }

        [Test]
        public void ChainWithOnlyAStartingObjectReturnsThat()
        {
            var mock = _helper.CreateTestObject();
            TestObject obj = Chain.Do(mock.Object);
            obj.Should().Be(mock.Object);
        }

        [Test]
        public void ChainWithOnlyAStartingFunctionReturnsItsResult()
        {
            var mock = _helper.CreateTestObject();
            TestObject obj = Chain.Do(() => mock.Object);
            obj.Should().Be(mock.Object);
        }

        [Test]
        public void ChainWithOnlyANullStartingObjectReturnsNull()
        {
            TestObject obj = Chain.Do(_helper.CreateNullTestObject());
            obj.Should().BeNull();
        }
        [Test]
        public void ChainWithOnlyANullStartingObjectReturnsTheFallback()
        {
            var fallbackMock = _helper.CreateTestObject();

            TestObject obj = Chain.Do(_helper.CreateNullTestObject).OrElse(fallbackMock.Object);
            obj.Should().Be(fallbackMock.Object);
        }

        [Test]
        public void ChainCallsSecondObjectIfFirstIsNotNull()
        {
            var firstMock = _helper.CreateTestObject();
            var secondMock = _helper.CreateTestObject();
            var lastMock = _helper.CreateTestObject();
            secondMock.Setup(x => x.ChainCreateObject(firstMock.Object)).Returns(lastMock.Object);

            TestObject finalObject = Chain.Do(firstMock.Object)
                                          .Then(secondMock.Object.ChainCreateObject);

            secondMock.Verify(x => x.ChainCreateObject(It.IsAny<TestObject>()));
            finalObject.Should().Be(lastMock.Object);
        }

        [Test]
        public void ChainDoesntCallsSecondObjectIfFirstIsNullAndReturnsNull()
        {
            var firstMock = _helper.CreateTestObject();
            var secondMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject)
                                          .Then(secondMock.Object.ChainCreateObject);

            secondMock.Verify(x => x.ChainCreateObject(firstMock.Object), Times.Never());
            finalObject.Should().BeNull();
        }

        [Test]
        public void ChainDoesntCallsSecondObjectIfFirstIsNullAndReturnsFallback()
        {
            var firstMock = _helper.CreateTestObject();
            var secondMock = _helper.CreateTestObject();
            var lastMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject)
                                          .Then(secondMock.Object.ChainCreateObject).OrElse(lastMock.Object);

            secondMock.Verify(x => x.ChainCreateObject(firstMock.Object), Times.Never());
            finalObject.Should().Be(lastMock.Object);
        }

        [Test]
        public void ChainDoesntCallsSecondObjectIfFirstIsNullButCallsTheThirdWithTheFallback()
        {
            var firstMock = _helper.CreateTestObject();
            var secondMock = _helper.CreateTestObject();
            var fallbackMock = _helper.CreateTestObject();
            var thirdMock = _helper.CreateTestObject();
            var lastMock = _helper.CreateTestObject();
            thirdMock.Setup(x => x.ChainCreateObject(fallbackMock.Object)).Returns(lastMock.Object);

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject)
                                          .Then(secondMock.Object.ChainCreateObject).OrElse(fallbackMock.Object)
                                          .Then(thirdMock.Object.ChainCreateObject);

            secondMock.Verify(x => x.ChainCreateObject(firstMock.Object), Times.Never());
            thirdMock.Verify(x => x.ChainCreateObject(fallbackMock.Object));
            finalObject.Should().Be(lastMock.Object);
        }
    
        [Test]
        public void ChainWithNullStartObjectUsesTheFirstNonNullFallback_FirstPlace()
        {
            var fallbackMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject).OrElse(fallbackMock.Object).OrElse(_helper.CreateNullTestObject());

            finalObject.Should().Be(fallbackMock.Object);
        }
    
        [Test]
        public void ChainWithNullStartObjectUsesTheFirstNonNullFallback_SecondPlace()
        {
            var fallbackMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject).OrElse(_helper.CreateNullTestObject()).OrElse(fallbackMock.Object);

            finalObject.Should().Be(fallbackMock.Object);
        }
    
        [Test]
        public void ChainWithNullStartObjectUsesTheFirstNonNullFallbackFromInvocation_FirstPlace()
        {
            var fallbackMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject).OrElse(_helper.CreateNullTestObject).OrElse(fallbackMock.Object);

            finalObject.Should().Be(fallbackMock.Object);
        }
    
        [Test]
        public void ChainWithNullStartObjectUsesTheFirstNonNullFallbackFromInvocation_SecondPlace()
        {
            var fallbackMock = _helper.CreateTestObject();

            TestObject finalObject = Chain.Do(_helper.CreateNullTestObject).OrElse(fallbackMock.Object).OrElse(_helper.CreateNullTestObject);

            finalObject.Should().Be(fallbackMock.Object);
        }

        [Test]
        public void ChainDoesntCallFallbackIfNotNeededDueToCurrentValueBeingNotNull()
        {
            var mock = _helper.CreateTestObject();
            var fallbackMock = _helper.CreateTestObject();

             Chain.Do(mock.Object).OrElse(fallbackMock.Object.ChainCreateNull);

            fallbackMock.Verify(x => x.ChainCreateNull(), Times.Never());
        }

        [Test]
        public void ChainDoesntCallFallbackIfNotNeededDueToCurrentValueBeingNotNull_WithPreviousFallback()
        {
            var mock = _helper.CreateTestObject();
            var fallbackMock = _helper.CreateTestObject();

            Chain.Do(mock.Object).OrElse(fallbackMock.Object.ChainCreateNull).OrElse(fallbackMock.Object.ChainCreateNull);

            fallbackMock.Verify(x => x.ChainCreateNull(), Times.Never());
        }

        [Test]
        public void ChainWithNullFirstObjectDoesntCallFallbackIfPreviousCallbackWasNotNull()
        {
            var fallbackMock = _helper.CreateTestObject();
            var lastMock = _helper.CreateTestObject();
            fallbackMock.Setup(x => x.ChainCreateObject()).Returns(lastMock.Object);

            TestObject result = Chain.Do(_helper.CreateNullTestObject).OrElse(fallbackMock.Object.ChainCreateObject).OrElse(fallbackMock.Object.ChainCreateNull);

            fallbackMock.Verify(x => x.ChainCreateObject(), Times.Once());
            fallbackMock.Verify(x => x.ChainCreateNull(), Times.Never());
            result.Should().Be(lastMock.Object);
        }
    }

    public class ChainTestHelper
    {

        public Mock<TestObject> CreateTestObject()
        {
            return new Mock<TestObject>();
        }

        public TestObject CreateNullTestObject()
        {
            return null;
        }

    }

    public class TestObject
    {
        public virtual TestObject ChainCreateObject(TestObject obj)
        {
            throw new NotImplementedException("This method has to be stubbed");
        }

        public virtual TestObject ChainCreateObject()
        {
            throw new NotImplementedException("This method has to be stubbed");
        }

        public virtual TestObject ChainCreateNull()
        {
            return null;
        }
    }
}
