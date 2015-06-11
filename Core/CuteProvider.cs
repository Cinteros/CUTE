﻿namespace Cinteros.Unit.Testing.Extensions.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Xml;
    using Cinteros.Unit.Testing.Extensions.Core.Background;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    [DataContract]
    public class CuteProvider : IServiceProvider
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CuteProvider"/> class.
        /// </summary>
        /// <param name="data">Serialized data that used to re-create provider</param>
        public CuteProvider(string data)
            : this()
        {
            var saved = Serialization.Inflate<CuteProvider>(data, CuteProvider.Types);
            this.Context = saved.Context;
            this.Calls = saved.Calls;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuteProvider"/> class.
        /// </summary>
        /// <param name="provider">Provider that will be acting as a back-end for current one</param>
        public CuteProvider(IServiceProvider provider)
            : this()
        {
            if (provider.GetType() == typeof(CuteProvider))
            {
                this.Original = ((CuteProvider)provider).Original;
            }
            else
            {
                this.Original = provider;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuteProvider"/> class.
        /// </summary>
        public CuteProvider()
        {
            this.Context = new CuteContext();
            this.Calls = new Collection<CuteCall>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuteProvider"/> class.
        /// </summary>
        /// <param name="proxy"></param>
        public CuteProvider(IOrganizationService proxy)
            : this()
        {
            this.Proxy = proxy;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Types used by <see cref="CuteProvider"/> and all subsidiary objects
        /// </summary>
        public static Type[] Types
        {
            get
            {
                return new[]
                {
                    typeof(object),
                    typeof(Entity),
                    typeof(EntityCollection),
                    typeof(QueryExpression),
                    typeof(ColumnSet),
                    typeof(OrganizationRequest),
                    typeof(OrganizationResponse)
                };
            }
        }

        [DataMember]
        public Collection<CuteCall> Calls
        {
            get;
            private set;
        }

        [DataMember]
        public CuteContext Context
        {
            get;
            private set;
        }

        public IServiceProvider Original
        {
            get;
            private set;
        }

        public IOrganizationService Proxy
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates instance of service of type set by <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Type of the service to spawn</param>
        /// <returns>Service instance</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                if (this.Original == null)
                {
                    // Original IServiceProvider is not available, so use cached version of the object
                    return this.Context;
                }
            }

            if (serviceType == typeof(IOrganizationServiceFactory))
            {
                // Always returning wrapped version of the IOrganizationServiceFactory
                return new CuteFactory(this);
            }

            if (serviceType == typeof(ITracingService))
            {
                if (this.Original == null)
                {
                    // Original IServiceProvider is not available, so use cached version of the object
                    return new CuteTracing();
                }
            }

            if (this.Original != null)
            {
                return this.Original.GetService(serviceType);
            }

            throw new NotImplementedException(string.Format("Behavior for service of type '{0}' is not defined yet.", serviceType.ToString()));
        }

        /// <summary>
        /// Serializes object into deflated and compressed string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serialization.Deflate<CuteProvider>(this, CuteProvider.Types);
        }

        /// <summary>
        /// Serializes object to XML representation
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXml()
        {
            var document = new XmlDocument();
            document.LoadXml(Serialization.Serialize<CuteProvider>(this, CuteProvider.Types));

            return document;
        }

        #endregion Public Methods
    }
}