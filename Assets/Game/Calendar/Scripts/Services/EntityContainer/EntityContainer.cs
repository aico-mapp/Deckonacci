using System;
using System.Collections.Generic;
using UnityEngine;

namespace Calendar.Scripts.Services.EntityContainer
{
    public class EntityContainer : IEntityContainer
    {
        private readonly Dictionary<Type, IFactoryEntity> _entities = new Dictionary<Type, IFactoryEntity>(10);

        public void RegisterEntity<TEntity>(TEntity entity) where TEntity : class, IFactoryEntity
        {
            if (_entities.ContainsKey(typeof(TEntity)))
                ReplaceEntityWithDispose(entity);
            else
                _entities.Add(typeof(TEntity), entity);
            
            Debug.Log($"RegisterEntity {entity.GetType().Name}");
        }

        public TEntity GetEntity<TEntity>() where TEntity : class, IFactoryEntity
        {
            _entities.TryGetValue(typeof(TEntity), out IFactoryEntity entity);
            return (TEntity) entity;
        }

        public void Dispose()
        {
            foreach (IFactoryEntity entity in _entities.Values)
                TryDisposeEntity(entity);
        }

        private void ReplaceEntityWithDispose<TEntity>(TEntity entity) where TEntity : class, IFactoryEntity
        {
            object replaceEntity = _entities[typeof(TEntity)];
            TryDisposeEntity(replaceEntity);
            _entities[typeof(TEntity)] = entity;
        }

        private void TryDisposeEntity(object entity)
        {
            if(entity is IDisposable disposableEntity) 
                disposableEntity.Dispose();
        }
    }
}