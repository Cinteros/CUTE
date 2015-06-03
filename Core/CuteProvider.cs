﻿namespace Cinteros.Unit.Test.Extensions.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Xml;
    using Cinteros.Unit.Test.Extensions.Core.Background;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    [DataContract]
    public class CuteProvider : IServiceProvider
    {
        #region Public Constructors

        public CuteProvider(string data)
            : this()
        {
            var saved = Serialization.Inflate<CuteProvider>(data, CuteProvider.Types);
            this.Context = saved.Context;
            this.Calls = saved.Calls;
        }

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

        public CuteProvider()
        {
            this.Context = new CuteContext();
            this.Calls = new Collection<CuteCall>();
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