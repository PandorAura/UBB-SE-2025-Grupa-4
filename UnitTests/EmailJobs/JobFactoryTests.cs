using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Quartz;
using Quartz.Spi;
using Xunit;

namespace YourNamespace.Tests
{
    public class JobFactoryTests
    {
        [Fact]
        public void NewJob_WhenServiceProviderReturnsJob_ReturnsJob()
        {
            // Arrange
            var mockJob = new Mock<IJob>();
            var mockJobType = typeof(IJob);
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mockJob.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var jobFactory = new JobFactory(serviceProvider);
            
            var mockJobDetail = new Mock<IJobDetail>();
            mockJobDetail.Setup(jd => jd.JobType).Returns(mockJobType);
            
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, DateTimeOffset.Now, 
                DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now);
            mockTriggerFiredBundle.Setup(tfb => tfb.JobDetail).Returns(mockJobDetail.Object);
            
            var mockScheduler = new Mock<IScheduler>();

            // Act
            var result = jobFactory.NewJob(mockTriggerFiredBundle.Object, mockScheduler.Object);

            // Assert
            Assert.Same(mockJob.Object, result);
        }

        [Fact]
        public void NewJob_WhenServiceProviderDoesNotReturnJob_ThrowsException()
        {
            // Arrange
            var mockJobType = typeof(IJob);
            
            // Creating an empty service provider that doesn't have IJob registered
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            
            var jobFactory = new JobFactory(serviceProvider);
            
            var mockJobDetail = new Mock<IJobDetail>();
            mockJobDetail.Setup(jd => jd.JobType).Returns(mockJobType);
            
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, DateTimeOffset.Now, 
                DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now);
            mockTriggerFiredBundle.Setup(tfb => tfb.JobDetail).Returns(mockJobDetail.Object);
            
            var mockScheduler = new Mock<IScheduler>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                jobFactory.NewJob(mockTriggerFiredBundle.Object, mockScheduler.Object));
        }

        [Fact]
        public void NewJob_WhenServiceProviderReturnsNonIJobType_ThrowsException()
        {
            // Arrange
            var nonJob = new NonJobClass(); // This doesn't implement IJob
            var nonJobType = typeof(NonJobClass);
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(nonJob);
            serviceCollection.AddSingleton(nonJobType, nonJob);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var jobFactory = new JobFactory(serviceProvider);
            
            var mockJobDetail = new Mock<IJobDetail>();
            mockJobDetail.Setup(jd => jd.JobType).Returns(nonJobType);
            
            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, DateTimeOffset.Now, 
                DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now);
            mockTriggerFiredBundle.Setup(tfb => tfb.JobDetail).Returns(mockJobDetail.Object);
            
            var mockScheduler = new Mock<IScheduler>();

            // Act & Assert
            Assert.Throws<Exception>(() => 
                jobFactory.NewJob(mockTriggerFiredBundle.Object, mockScheduler.Object));
        }


        [Fact]
        public void ReturnJob_WhenJobIsNotDisposable_DoesNothing()
        {
            // Arrange
            var mockJob = new Mock<IJob>();
            var jobFactory = new JobFactory(Mock.Of<IServiceProvider>());

            // Act & Assert (no exception should be thrown)
            jobFactory.ReturnJob(mockJob.Object);
        }

        [Fact]
        public void ReturnJob_WhenJobIsDisposable_CallsDispose()
        {
            // Arrange
            var mockDisposableJob = new Mock<IDisposableJob>();
            var jobFactory = new JobFactory(Mock.Of<IServiceProvider>());

            // Act
            jobFactory.ReturnJob(mockDisposableJob.Object);

            // Assert
            mockDisposableJob.Verify(j => j.Dispose(), Times.AtLeastOnce);
        }

        [Fact]
        public void NewJob_WhenServiceProviderReturnsNull_ThrowsException()
        {
            // Arrange
            var mockJobType = typeof(IJob);

            // Setup service provider to return null for the job type
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == mockJobType)))
                .Returns(null);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mockServiceProvider.Object);
            var serviceProvider = new DelegatingServiceProvider(mockServiceProvider.Object);

            var jobFactory = new JobFactory(serviceProvider);

            var mockJobDetail = new Mock<IJobDetail>();
            mockJobDetail.Setup(jd => jd.JobType).Returns(mockJobType);

            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, DateTimeOffset.Now,
                DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now);
            mockTriggerFiredBundle.Setup(tfb => tfb.JobDetail).Returns(mockJobDetail.Object);

            var mockScheduler = new Mock<IScheduler>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                jobFactory.NewJob(mockTriggerFiredBundle.Object, mockScheduler.Object));
            Assert.IsType(typeof(string), exception.Message);
        }

        [Fact]
        public void NewJob_WhenGetRequiredServiceThrowsInvalidOperationException_RethrowsException()
        {
            // Arrange
            var mockJobType = typeof(IJob);

            // Setup service provider to throw InvalidOperationException
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(It.Is<Type>(t => t == mockJobType)))
                .Throws(new InvalidOperationException("Service not registered"));

            var serviceProvider = new DelegatingServiceProvider(mockServiceProvider.Object);

            var jobFactory = new JobFactory(serviceProvider);

            var mockJobDetail = new Mock<IJobDetail>();
            mockJobDetail.Setup(jd => jd.JobType).Returns(mockJobType);

            var mockTriggerFiredBundle = new Mock<TriggerFiredBundle>(
                null, null, null, false, DateTimeOffset.Now,
                DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now);
            mockTriggerFiredBundle.Setup(tfb => tfb.JobDetail).Returns(mockJobDetail.Object);

            var mockScheduler = new Mock<IScheduler>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                jobFactory.NewJob(mockTriggerFiredBundle.Object, mockScheduler.Object));
        }

        // Helper classes for testing
        private class NonJobClass { }

        public interface IDisposableJob : IJob, IDisposable { }

        // Helper class to intercept GetService calls and delegate to mock
        private class DelegatingServiceProvider : IServiceProvider
        {
            private readonly IServiceProvider _innerProvider;

            public DelegatingServiceProvider(IServiceProvider innerProvider)
            {
                _innerProvider = innerProvider;
            }

            public object GetService(Type serviceType)
            {
                return _innerProvider.GetService(serviceType);
            }
        }
    }
}