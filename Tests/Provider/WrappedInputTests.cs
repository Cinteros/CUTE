﻿namespace Cinteros.Unit.Testing.Extensions.Tests.Provider
{
    using Cinteros.Unit.Testing.Extensions.Core;
    using NUnit.Framework;

    public class WrappedInputTests : CoreTests
    {
        #region Public Constructors

        public WrappedInputTests()
            : base()
        {
            this.Provider = new CuteProvider(this.Provider);
        }

        #endregion Public Constructors

        #region Public Methods

        [Test]
        [Category("Provider"), Category("Wrapped Input"), Category("Context")]
        public override void Get_Context()
        {
            base.Get_Context();
        }

        [Test]
        [Category("Provider"), Category("Wrapped Input")]
        public override void Get_OriginalProvider()
        {
            base.Get_OriginalProvider();
        }

        [Test]
        [Category("Provider"), Category("Wrapped Input")]
        public override void Get_TracingService()
        {
            base.Get_TracingService();
        }

        [Test]
        [Category("Provider"), Category("Wrapped Input"), Category("Factory")]
        public override void Get_WrappedFactory()
        {
            base.Get_WrappedFactory();
        }

        #endregion Public Methods
    }
}