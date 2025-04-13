using Moq;
using Quartz;
using Quartz.Spi;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
namespace UnitTests.EmailJobs
{
    public class JobFactoryTests
    {
        [Fact]
        public void NewJob_WhenServiceIsAvailable_ReturnsJob()
        {
            var mockJob = new Mock<IJob>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockJobDetail = new Mock<IJobDetail>();
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, null, null, null, null);
            var jobType = typeof(IJob);
            mockJobDetail.Setup(x => x.JobType).Returns(jobType);
            mockTriggerFiredBundle.Setup(x => x.JobDetail).Returns(mockJobDetail.Object);
            mockServiceProvider.Setup(x => x.GetService(jobType)).Returns(mockJob.Object);

            var jobFactory = new JobFactory(mockServiceProvider.Object);
            var result = jobFactory.NewJob(mockTriggerFiredBundle.Object, null);
            Assert.NotNull(result);
            Assert.Same(mockJob.Object, result);
        }

        [Fact]
        public void NewJob_WhenServiceIsNotAvailable_ThrowsException()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockJobDetail = new Mock<IJobDetail>();
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, null, null, null, null);

            var jobType = typeof(IJob);
            mockJobDetail.Setup(x => x.JobType).Returns(jobType);
            mockTriggerFiredBundle.Setup(x => x.JobDetail).Returns(mockJobDetail.Object);

            // Return null from GetService to simulate the service not being available
            mockServiceProvider.Setup(x => x.GetService(jobType)).Returns(null);

            var jobFactory = new JobFactory(mockServiceProvider.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                jobFactory.NewJob(mockTriggerFiredBundle.Object, null));

            Assert.Equal(exception.GetType(),typeof(InvalidOperationException));
        }

        [Fact]
        public void NewJob_WhenServiceProviderThrowsException_RethrowsException()
        {
            var expectedException = new InvalidOperationException("Service not registered");
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockJobDetail = new Mock<IJobDetail>();
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, null, null, null, null);
            var jobType = typeof(IJob);
            mockJobDetail.Setup(x => x.JobType).Returns(jobType);
            mockTriggerFiredBundle.Setup(x => x.JobDetail).Returns(mockJobDetail.Object);
            mockServiceProvider.Setup(x => x.GetService(jobType)).Throws(expectedException);
            var jobFactory = new JobFactory(mockServiceProvider.Object);
            var exception = Assert.Throws<InvalidOperationException>(() =>
                jobFactory.NewJob(mockTriggerFiredBundle.Object, null));
            Assert.Same(expectedException, exception);
        }

        [Fact]
        public void ReturnJob_WhenJobIsDisposable_DisposesJob()
        {
            var mockDisposableJob = new Mock<IDisposableJob>();
            var jobFactory = new JobFactory(Mock.Of<IServiceProvider>());
            jobFactory.ReturnJob(mockDisposableJob.Object);
            mockDisposableJob.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void ReturnJob_WhenJobIsNotDisposable_DoesNothing()
        {
            var mockJob = new Mock<IJob>();
            var jobFactory = new JobFactory(Mock.Of<IServiceProvider>());
            jobFactory.ReturnJob(mockJob.Object);
        }
        public interface IDisposableJob : IJob, IDisposable { }
    }
}