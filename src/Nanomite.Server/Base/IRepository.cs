namespace Nanomite.Server.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IRepository"/>
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets or sets the ModelType
        /// </summary>
        Type ModelType { get; set; }

        /// <summary>
        /// Gets or sets the proto type URL.
        /// </summary>
        string ProtoTypeUrl { get; set; }

        /// <summary>
        /// The Create
        /// </summary>
        /// <param name="data">The <see cref="object"/></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        Task<object> Create(object data);

        /// <summary>
        /// Delete an entity by identifier
        /// </summary>
        /// <param name="id">The <see cref="Guid"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task Delete(Guid id);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="data">The <see cref="object"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task Delete(object data);

        /// <summary>
        /// Gets the a specific entity
        /// </summary>
        /// <param name="id">The <see cref="Guid"/></param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="object"/></returns>
        object GetById(Guid id, bool includeAll);

        /// <summary>
        /// Gets the a specific entity by a entity, the id need to be set
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="object"/></returns>
        object Get(object data, bool includeAll);

        /// <summary>
        /// Gets a specific entity
        /// </summary>
        /// <param name="func">a expression to filter results</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="object"/></returns>
        object GetFirst(string func, bool includeAll = false);

        /// <summary>
        /// Gets the by identifier list.
        /// </summary>
        /// <param name="idList">The identifier list.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns></returns>
        IList GetByIdList(List<Guid> idList, bool includeAll);

        /// <summary>
        /// Gets the data by SQL.
        /// </summary>
        /// <param name="sql">The SQL query</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns></returns>
        IList GetBySql(string sql, bool includeAll);

        /// <summary>
        /// Gets the a list of specific entities
        /// </summary>
        /// <param name="func">a expression to filter results</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="IList"/></returns>
        IList GetListByQuery(string func, bool includeAll = false);

        /// <summary>
        /// Shrinks the database
        /// </summary>
        void Shrink();

        /// <summary>
        /// The Update
        /// </summary>
        /// <param name="data">The <see cref="object"/></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        Task<object> Update(object data);
    }
}