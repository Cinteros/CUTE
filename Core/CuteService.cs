﻿namespace Cinteros.Unit.Testing.Extensions.Core
{
    using Cinteros.Unit.Testing.Extensions.Core.Background;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Linq;

    /// <summary>
    /// Service that replaces requests with their cached versions
    /// </summary>
    public class CuteService : IOrganizationService
    {
        #region Public Constructors

        public CuteService(CuteProvider provider, Guid? userId)
        {
            Provider = provider;
            UserId = userId;

            if (Provider.Original != null)
            {
                var factory = (IOrganizationServiceFactory)provider.Original.GetService(typeof(IOrganizationServiceFactory));

                Original = factory.CreateOrganizationService(userId);
            }

            if (Provider.Proxy != null)
            {
                Original = this.Provider.Proxy;
            }
        }

        public CuteService(IOrganizationService service)
        {
            Provider = new CuteProvider
            {
                Type = InstanceType.StandaloneInput
            };

            Original = service;
        }

        #endregion Public Constructors

        #region Public Properties

        public IOrganizationService Original
        {
            get;
            private set;
        }

        public CuteProvider Provider
        {
            get;
            private set;
        }

        public Guid? UserId
        {
            get;
            private set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="relationship"></param>
        /// <param name="relatedEntities"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            if (this.Original != null)
            {
                this.Original.Associate(entityName, entityId, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Guid Create(Entity entity)
        {
            var call = new CuteCall(MessageName.Create, new object[] { entity });

            if (this.Original != null)
            {
                try
                {
                    call.Output = this.Original.Create(entity);
                }
                catch (Exception ex)
                {
                    call.Output = ex;
                    this.Provider.Calls.Add(call);

                    throw;
                }

                this.Provider.Calls.Add(call);

                return (Guid)call.Output;
            }
            else
            {
                var result = this.Provider.Calls.Where(x => x.Equals(call)).FirstOrDefault();

                if (result.Output.GetType().BaseType == typeof(Exception))
                {
                    throw (Exception)result.Output;
                }

                return (Guid)result.Output;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Delete(string entityName, Guid id)
        {
            var call = new CuteCall(MessageName.Delete, new object[] { entityName, id });

            if (Original != null)
            {
                try
                {
                    Original.Delete(entityName, id);
                }
                catch (Exception ex)
                {
                    call.Output = ex;
                    Provider.Calls.Add(call);

                    throw;
                }

                Provider.Calls.Add(call);
            }
            else
            {
                var result = Provider.Calls.Where(x => x.Equals(call)).FirstOrDefault();

                if (result != null && result.Output != null)
                {
                    if (result.Output.GetType().BaseType == typeof(Exception))
                    {
                        throw (Exception)result.Output;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="relationship"></param>
        /// <param name="relatedEntities"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            if (Original != null)
            {
                Original.Disassociate(entityName, entityId, relationship, relatedEntities);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public OrganizationResponse Execute(OrganizationRequest request)
        {
            if (Original != null)
            {
                var result = this.Original.Execute(request);

                Provider.Calls.Add(new CuteCall(MessageName.Execute, new[] { request }, result));

                return result;
            }
            else
            {
                var call = new CuteCall(MessageName.Execute, new[] { request });

                return Provider.Calls.Where(x => x.Equals(call)).Select(x => (OrganizationResponse)x.Output).FirstOrDefault();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <param name="columnSet"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var call = new CuteCall(MessageName.Retrieve, new object[] { entityName, id, columnSet });

            if (Original != null)
            {
                call.Output = Original.Retrieve(entityName, id, columnSet);

                Provider.Calls.Add(call);

                return (Entity)call.Output;
            }
            else
            {
                return Provider.Calls.Where(x => x.Equals(call)).Select(x => (Entity)x.Output).FirstOrDefault();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var call = new CuteCall(MessageName.RetrieveMultiple, new[] { query });

            if (Original != null)
            {
                call.Output = Original.RetrieveMultiple(query);

                Provider.Calls.Add(call);

                return (EntityCollection)call.Output;
            }
            else
            {
                return this.Provider.Calls.Where(x => x.Equals(call)).Select(x => (EntityCollection)x.Output).FirstOrDefault();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}Service", Provider.Type.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Update(Entity entity)
        {
            if (Original != null)
            {
                Original.Update(entity);
            }
        }

        #endregion Public Methods
    }
}