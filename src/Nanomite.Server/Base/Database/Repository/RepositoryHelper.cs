///-----------------------------------------------------------------
///   File:         RepositoryHelper.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 17:25:38
///-----------------------------------------------------------------

namespace Nanomite.Server.Base.Database.Repository
{
    using System;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="RepositoryHelper" />
    /// </summary>
    public class RepositoryHelper
    {
        /// <summary>
        /// Register all repositories dynamicaly by the given context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterRepositories<T>() where T : BaseContext
        {
            try
            {
                var entityTypes = ((Activator.CreateInstance(typeof(T))) as BaseContext).Model.GetEntityTypes();
                foreach (var type in entityTypes)
                {
                    if (!CommonRepositoryHandler.GetAllRepositories().Any(p => p.ModelType == type.ClrType))
                    {
                        Type repositoryType = typeof(Repository<,>).MakeGenericType(type.ClrType, typeof(T));
                        var repository = (IRepository)Activator.CreateInstance(repositoryType);
                        CommonRepositoryHandler.Register(repository);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register repositories - see inner exception for details.", ex);
            }
        }
    }
}
