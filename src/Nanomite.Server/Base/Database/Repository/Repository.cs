///-----------------------------------------------------------------
///   File:         Repository.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 17:25:28
///-----------------------------------------------------------------

namespace Nanomite.Server.Base.Database.Repository
{
    using Community.OData.Linq;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Microsoft.EntityFrameworkCore;
    using Nanomite.Common.Models.Base;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Repository{T}" />
    /// </summary>
    /// <typeparam name="T">any kind of class model</typeparam>
    /// <typeparam name="G"></typeparam>
    public class Repository<T, G> : IRepository where T : class where G : BaseContext
    {
        /// <summary>
        /// Gets or sets the optional context options.
        /// </summary>
        private DbContextOptions<G> contextOptions;

        /// <summary>
        /// The database path
        /// </summary>
        private string databasePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, G}"/> class.
        /// </summary>
        public Repository()
        {
            this.ModelType = typeof(T);
            if (typeof(IMessage).IsAssignableFrom(this.ModelType))
            {
                IMessage proto = Activator.CreateInstance(this.ModelType) as IMessage;
                this.ProtoTypeUrl = Any.GetTypeName(Any.Pack(proto).TypeUrl);
            }
            else
            {
                IMessage proto = (Activator.CreateInstance(this.ModelType) as IBaseModel).Proto;
                this.ProtoTypeUrl = Any.GetTypeName(Any.Pack(proto).TypeUrl);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, G}"/> class.
        /// </summary>
        /// <param name="contextOptions">The context options.</param>
        public Repository(DbContextOptions<G> contextOptions) : base()
        {
            this.contextOptions = contextOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, G}"/> class.
        /// </summary>
        /// <param name="databasePath">The database path.</param>
        public Repository(string databasePath) : base()
        {
            this.databasePath = databasePath;
        }

        /// <summary>
        /// Gets or sets the ModelType
        /// </summary>
        /// <value>
        /// The type of the model.
        /// </value>
        /// <inheritdoc />
        public System.Type ModelType { get; set; }

        /// <summary>
        /// Gets or sets the proto type URL.
        /// </summary>
        public string ProtoTypeUrl { get; set; }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="data">The <see cref="T:System.Object" /></param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task`1" />
        /// </returns>
        /// <exception cref="Exception">Could not create entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public async Task<object> Create(object data)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    List<T> dataList = new List<T>();
                    if (data.GetType().GenericTypeArguments != null
                        && data.GetType().GenericTypeArguments.Count() > 0)
                    {
                        dataList = (data as IEnumerable).Cast<T>().ToList();
                    }
                    else
                    {
                        dataList.Add(data as T);
                    }

                    ctx.Set<T>().AddRange(dataList);
                    await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create entity of type " + typeof(T) + ": " + ex.ToText());
            }

            return data;
        }

        /// <summary>
        /// Delete an entity by identifier
        /// </summary>
        /// <param name="id">The <see cref="T:System.Guid" /></param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        /// <exception cref="Exception">Delete failed: " + ex.Message</exception>
        /// <inheritdoc />
        public async Task Delete(Guid id)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    var entity = this.GetById(id, true);
                    if (entity != null)
                    {
                        ctx.Set<T>().Remove(entity as T);
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Delete failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="data">The <see cref="T:System.Object" /></param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        /// <exception cref="Exception">Could not delete entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public async Task Delete(object data)
        {
            try
            {
                if (data.GetType() == typeof(Guid))
                {
                    await this.Delete((Guid)data);
                    return;
                }

                using (var ctx = NewContext())
                {
                    List<T> dataList = new List<T>();
                    if (data.GetType().GenericTypeArguments != null
                        && data.GetType().GenericTypeArguments.Count() > 0)
                    {
                        if (data.GetType().GenericTypeArguments.FirstOrDefault() == typeof(Guid))
                        {
                            var guids = (data as IEnumerable).Cast<Guid>().ToList();
                            foreach (var guid in guids)
                            {
                                if (ctx.Set<T>().Any(p => (p as IBaseModel).Id == guid))
                                {
                                    var item = Activator.CreateInstance(typeof(T)) as T;
                                    (item as IBaseModel).Id = guid;
                                    dataList.Add(item);
                                }
                            }
                        }
                        else
                        {
                            dataList = (data as IEnumerable).Cast<T>().ToList();
                        }
                    }
                    else
                    {
                        dataList.Add(data as T);
                    }

                    if (dataList.Count > 0)
                    {
                        ctx.Set<T>().RemoveRange(dataList);
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not delete entities of type " + typeof(T) + ": " + ex.ToText());
            }
        }

        /// <summary>
        /// Gets the a specific entity
        /// </summary>
        /// <param name="id">The <see cref="T:System.Guid" /></param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>
        /// The <see cref="T:System.Object" />
        /// </returns>
        /// <exception cref="Exception">Could not get entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public virtual object GetById(Guid id, bool includeAll)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    var list = ctx.Set<T>().Where(p => (p as IBaseModel).Id == id);
                    if (includeAll)
                    {
                        return ctx.Include(list).FirstOrDefault();
                    }
                    else
                    {
                        return list.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entity of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Gets the a specific entity by a entity, the id need to be set
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>
        /// The <see cref="T:System.Object" />
        /// </returns>
        /// <exception cref="Exception">Could not get entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public virtual object Get(object data, bool includeAll)
        {
            try
            {
                if (data.GetType() == typeof(Guid))
                {
                    return this.GetById((Guid)data, includeAll);
                }
                else
                {
                    return this.GetById((data as IBaseModel).Id, includeAll);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entity of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Gets the by identifier list.
        /// </summary>
        /// <param name="idList">The identifier list.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not get entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public virtual IList GetByIdList(List<Guid> idList, bool includeAll)
        {
            try
            {
                IList<T> result = new List<T>();

                using (var ctx = NewContext())
                {
                    var filterParams = idList.Select(id => string.Format("{0}", id));
                    var filter = "contains('" + string.Join(",", filterParams) + "', cast(Id,'Edm.String'))";
                    result = DoLazyLoading(ctx, filter, includeAll);
                }

                return result as IList;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entity of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Gets the data by SQL.
        /// </summary>
        /// <param name="sql">The SQL query</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not get entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public IList GetBySql(string sql, bool includeAll)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    ctx.Database.SetCommandTimeout(1500000);
                    var list = DoLazyLoading(ctx, null, includeAll, sql);
                    return list as IList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entity of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Gets a specific entity
        /// </summary>
        /// <param name="func">a expression to filter results</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="object"/></returns>
        public object GetFirst(string func, bool includeAll = false)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    return DoLazyLoading(ctx, func, includeAll).FirstOrDefault() as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entities of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Gets the a list of specific entities
        /// </summary>
        /// <param name="func">a expression to filter results</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>
        /// The <see cref="T:System.Collections.IList" />
        /// </returns>
        /// <exception cref="Exception">Could not get entities of type " + typeof(T)</exception>
        /// <inheritdoc />
        public IList GetListByQuery(string query, bool includeAll)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    if (!string.IsNullOrEmpty(query))
                    {
                        return DoLazyLoading(ctx, query, includeAll) as IList;
                    }
                    else
                    {
                        return DoLazyLoading(ctx, null, includeAll) as IList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get entities of type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// Shrinks the database
        /// </summary>
        /// <exception cref="Exception">Could not shrink database</exception>
        /// <inheritdoc />
        public void Shrink()
        {
            try
            {
                using (var ctx = NewContext())
                {
                    ctx.Database.ExecuteSqlCommand("VACUUM;");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not shrink database", ex);
            }
        }

        /// <summary>
        /// The Update
        /// </summary>
        /// <param name="data">The <see cref="T:System.Object" /></param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task`1" />
        /// </returns>
        /// <exception cref="Exception">Could not update entity of type " + typeof(T)</exception>
        /// <inheritdoc />
        public async Task<object> Update(object data)
        {
            try
            {
                using (var ctx = NewContext())
                {
                    List<T> dataList = new List<T>();
                    if (data.GetType().GenericTypeArguments != null
                        && data.GetType().GenericTypeArguments.Count() > 0)
                    {
                        dataList = (data as IEnumerable).Cast<T>().ToList();
                    }
                    else
                    {
                        dataList.Add(data as T);
                    }

                    ctx.Set<T>().UpdateRange(data as dynamic);
                    await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update entity of type " + typeof(T) + ": " + ex.ToText());
            }

            return data;
        }

        /// <summary>
        /// Does the lazy loading.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="query">The query.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        private IList<T> DoLazyLoading(BaseContext ctx, string query, bool includeAll, string sql = null)
        {
            IQueryable<T> entities = ctx.Set<T>();
            if (!string.IsNullOrEmpty(sql))
            {
                entities = entities.FromSql(sql);
            }

            if (includeAll)
            {
                return ctx.Include(entities, query).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(query))
                {
                    return entities.OData().Filter(query).ToList();
                }

                return entities.ToList();
            }
        }

        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <returns>The <see cref="G"/></returns>
        private G NewContext()
        {
            if (!string.IsNullOrEmpty(this.databasePath))
            {
                return Activator.CreateInstance(typeof(G), databasePath) as G;
            }
            else if (this.contextOptions != null)
            {
                return Activator.CreateInstance(typeof(G), this.contextOptions) as G;
            }
            else
            {
                return Activator.CreateInstance(typeof(G)) as G;
            }
        }
    }
}
