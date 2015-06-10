﻿namespace Cinteros.Unit.Testing.Extensions.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.ServiceModel.Description;
    using System.Text;
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
        /// <param name="userCredentials">User credentials that connects to <paramref name="serviceUri"/></param>
        /// <param name="serviceUri">
        /// Address of the IOrganizationService endpoint, should end with
        /// '/XRMServices/2011/Organization.svc' string, otherwise system will try to substitute
        /// correct part of address
        /// </param>
        public CuteProvider(ClientCredentials userCredentials, Uri serviceUri)
            : this()
        {
            var builder = new StringBuilder(serviceUri.ToString());

            if (!builder.ToString().EndsWith("XRMServices/2011/Organization.svc"))
            {
                if (!builder.ToString().EndsWith("/"))
                {
                    builder.Append("/");
                }

                builder.Append("XRMServices/2011/Organization.svc");
            }

            this.Endpoint = builder.ToString();
            this.User = userCredentials.UserName.UserName;
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

        public string Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value shouwing could calls be made to real CRM itself, or not
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return (this.Original != null);
            }
        }

        public IServiceProvider Original
        {
            get;
            private set;
        }

        public object User
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                // Sign that provider was deserialized
                if (!this.IsOnline)
                {
                    return this.Context;
                }
            }

            if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return new CuteFactory(this);
            }

            if (this.IsOnline)
            {
                return this.Original.GetService(serviceType);
            }
            else
            {
                return new object();
            }
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