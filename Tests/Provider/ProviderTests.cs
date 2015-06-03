﻿namespace Cinteros.Unit.Test.Extensions.Tests.Provider
{
    using System;
    using Cinteros.Unit.Test.Extensions.Core;
    using Microsoft.Xrm.Sdk;
    using NSubstitute;
    using Xunit;

    public class ProviderTests
    {
        #region Public Constructors

        public ProviderTests()
        {
            // Arrange
            this.Provider = new CuteProvider(Substitute.For<IServiceProvider>());
            this.Provider.GetService(typeof(IPluginExecutionContext)).Returns(Substitute.For<IPluginExecutionContext>());
            this.Provider.GetService(typeof(ITracingService)).Returns(Substitute.For<ITracingService>());
        }

        #endregion Public Constructors

        #region Public Properties

        public CuteProvider Provider
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Public Methods

        public virtual void Check_Online_Status()
        {
        }

        public void Get_Context()
        {
            // Act
            var context = this.Provider.GetService(typeof(IPluginExecutionContext));

            // Assert
            Assert.IsAssignableFrom<IPluginExecutionContext>(context);
            Assert.IsNotType<CuteContext>(context);
        }

        public virtual void Get_OriginalProvider()
        {
        }

        public virtual void Get_TracingService()
        {
        }

        public void Get_WrappedFactory()
        {
            // Act
            var factory = Provider.GetService(typeof(IOrganizationServiceFactory));

            // Assert
            Assert.IsAssignableFrom<IOrganizationServiceFactory>(factory);
            Assert.IsType<CuteFactory>(factory);
        }

        #endregion Public Methods
    }
}