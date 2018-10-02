///-----------------------------------------------------------------
///   File:         BaseContext.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 17:30:52
///-----------------------------------------------------------------

namespace Nanomite.Server.Base
{
    using Community.OData.Linq;
    using Community.OData.Linq.OData.Query.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Nanomite.Server.Base.Database.Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="BaseContext"/>
    /// </summary>
    public abstract class BaseContext : DbContext
    {
        /// <summary>
        /// Migrates the database schema and register the repositories
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Setup<T>() where T : BaseContext
        {
            try
            {
                using (var ctx = Activator.CreateInstance(typeof(T)) as BaseContext)
                {
                    ctx.Database.Migrate();
                }
                RepositoryHelper.RegisterRepositories<T>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets or sets the database options.
        /// </summary>
        public static DbContextOptions DatabaseOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseContext"/> class.
        /// </summary>
        public BaseContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public BaseContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Gets the DatabaseVersion
        /// Gets or sets the database version.
        /// </summary>
        public abstract Version DatabaseVersion { get; }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        public abstract string DatabasePath { get; }

        /// <summary>
        /// Queries the specified entities.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="temp">The temporary.</param>
        /// <param name="authQuery">The authentication query.</param>
        /// <param name="filterQuery">The filter query.</param>
        /// <returns></returns>
        public static List<T> Query<T>(IQueryable<T> temp, Func<T, bool> authQuery = null, string filterQuery = null)
        {
            if ((temp as IQueryable<T>) == null)
            {
                temp = temp.AsQueryable();
            }

            // odata validation settings
            var action = new Action<ODataSettings>((s) =>
            {
                s.ParserSettings.MaximumExpansionCount = 99999;
                s.ParserSettings.MaximumExpansionDepth = 99999;
                s.ValidationSettings.MaxExpansionDepth = 99999;
                s.ValidationSettings.MaxNodeCount = 99999;
            });

            var odataQuery = temp.OData(action);

            // the default odata datetime conversion is shit because it always casts the date queries to local timezone
            // no matter what timezone is saved inside the database. So we hack the library and
            // inject our own filter binder which only skips the datetime.
            var customFilterBinder = new CustomFilterBinder(odataQuery.ServiceProvider);
            (odataQuery.ServiceProvider as ServiceContainer).RemoveService(typeof(FilterBinder));
            (odataQuery.ServiceProvider as ServiceContainer).AddService(typeof(FilterBinder), customFilterBinder);

            if (authQuery != null && filterQuery != null)
            {
                return odataQuery.Filter(filterQuery).Where(authQuery).ToList() as List<T>;
            }
            else if (authQuery != null)
            {
                return odataQuery.Where(authQuery).ToList() as List<T>;
            }
            else if (filterQuery != null)
            {
                return odataQuery.Filter(filterQuery).ToList() as List<T>;
            }
            else
            {
                return temp.ToList() as List<T>;
            }
        }

        /// <summary>
        /// Includes all child objects and executes the query.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="query">The query.</param>
        /// <param name="authQuery">The authentication query.</param>
        /// <returns>The <see cref="IEnumerable{T}"/></returns>
        public abstract IEnumerable<T> Include<T>(IQueryable<T> entities, string query = null, Func<T, bool> authQuery = null);
    }
}
