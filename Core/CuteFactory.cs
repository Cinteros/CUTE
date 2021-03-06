﻿namespace Cinteros.Unit.Testing.Extensions.Core
{
    using Microsoft.Xrm.Sdk;
    using System;

    public class CuteFactory : IOrganizationServiceFactory
    {
        #region Public Constructors

        public CuteFactory(CuteProvider provider)
        {
            Provider = provider;
        }

        #endregion Public Constructors

        #region Public Properties

        public CuteProvider Provider
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        IOrganizationService IOrganizationServiceFactory.CreateOrganizationService(Guid? userId)
        {
            return new CuteService(Provider, userId);
        }

        public override string ToString()
        {
            return string.Format("{0}Factory", Provider.Type.ToString());
        }

        #endregion Public Methods
    }
}