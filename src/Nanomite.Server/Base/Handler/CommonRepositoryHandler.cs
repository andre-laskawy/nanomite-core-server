///-----------------------------------------------------------------
///   File:         CommonRepositoryHandler.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 17:17:33
///-----------------------------------------------------------------

namespace Nanomite.Server
{
    using Nanomite.Server.Base;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CommonRepositoryHandler" />
    /// </summary>
    public class CommonRepositoryHandler
    {
        /// <summary>
        /// Defines the repositories
        /// </summary>
        private static List<IRepository> repositories = new List<IRepository>();

        /// <summary>
        /// The CreateData
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="data">The <see cref="object" /></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        public static async Task<object> CreateData(Type type, object data)
        {
            return await GetRepository(type)?.Create(data);
        }

        /// <summary>
        /// Delete by id
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="id">The <see cref="Guid" /></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task DeleteData(Type type, Guid id)
        {
            await GetRepository(type)?.Delete(id);
        }

        /// <summary>
        /// Delete by model
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="data">The <see cref="object" /></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task DeleteData(Type type, object data)
        {
            await GetRepository(type)?.Delete(data);
        }

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns></returns>
        public static object GetById(Type type, Guid id, bool includeAll)
        {
            return GetRepository(type)?.GetById(id, includeAll);
        }

        /// <summary>
        /// Gets all repositories.
        /// </summary>
        /// <returns></returns>
        public static List<IRepository> GetAllRepositories()
        {
            return repositories;
        }

        /// <summary>
        /// Gets a list by identifier list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="idList">The identifier list.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="IList"/></returns>
        public static IList GetByIdList(Type type, List<Guid> idList, bool includeAll)
        {
            return GetRepository(type)?.GetByIdList(idList, includeAll);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <param name="includeAll">if set to <c>true</c> [include all].</param>
        /// <returns>The <see cref="object"/></returns>
        public static object GetData(Type type, object data, bool includeAll)
        {
            return GetRepository(type)?.Get(data, includeAll);
        }

        /// <summary>
        /// Gets the first item of a type with the help of a query
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="query">The query"/&gt;</param>
        /// <param name="includeAll">The <see cref="bool" /></param>
        /// <returns>The <see cref="object"/></returns>
        public static object GetFirst(Type type, string query, bool includeAll)
        {
            return GetRepository(type)?.GetFirst(query, includeAll);
        }

        /// <summary>
        /// Gets a list of entities
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="query">The query</param>
        /// <param name="includeAll">The <see cref="bool" /></param>
        /// <returns>The <see cref="IList"/></returns>
        public static IList GetListByQuery(Type type, string query, bool includeAll)
        {
            return GetRepository(type)?.GetListByQuery(query, includeAll);
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="IRepository"/></returns>
        public static IRepository GetRepository(Type type)
        {
            try
            {
                if (type.GenericTypeArguments != null && type.GenericTypeArguments.Count() > 0)
                {
                    type = type.GenericTypeArguments.FirstOrDefault();
                }

                if (repositories == null)
                {
                    throw new Exception("No Repositories has been registered.");
                }

                return repositories.FirstOrDefault(p => p.ModelType == type);
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// The Register
        /// </summary>
        /// <param name="repos">The <see cref="IRepository[]" /></param>
        public static void Register(params IRepository[] repos)
        {
            try
            {
                if (repos != null && repos.Length > 0)
                {
                    if (repositories == null)
                    {
                        repositories = new List<IRepository>();
                    }

                    repositories.AddRange(repos.ToList());
                }
            }
            catch { }
        }

        /// <summary>
        /// The SaveData
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="data">The <see cref="object" /></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        public static async Task<object> SaveData(Type type, object data)
        {
            object result = GetRepository(type).Get(data, false);
            if (result == null)
            {
                result = await CreateData(type, data);
            }
            else
            {
                result = await UpdateData(type, data);
            }

            return result;
        }

        /// <summary>
        /// The UpdateData
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <param name="data">The <see cref="object" /></param>
        /// <returns>The <see cref="Task{object}"/></returns>
        public static async Task<object> UpdateData(Type type, object data)
        {
            return await GetRepository(type)?.Update(data);
        }
    }
}
