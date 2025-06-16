using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.Interfaces;
using TreeCount.Domain.Interfaces.Repository;

namespace TreeCount.Application.Services
{
    public abstract class ServiceBase<TModel> : IServiceBase where TModel : class
    {
        protected readonly IRepositoryBase<TModel> _entityRepository;

        public ServiceBase(IRepositoryBase<TModel> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public virtual async Task<VM> CreateAsync<D, VM>(D model)
        {
            var entity = MapToEntity<D>(model);
            var result = await _entityRepository.CreateAsync(entity);
            return MapToViewModel<VM>(result);
        }

        public virtual async Task<VM> UpdateAsync<D, VM>(D model)
        {
            var entity = MapToEntity<D>(model);
            var result = await _entityRepository.UpdateAsync(entity);
            return MapToViewModel<VM>(result);
        }

        public virtual async Task<VM> DeleteAsync<D, VM>(D model)
        {
            var entity = MapToEntity<D>(model);

            if (entity == null)
                throw new ArgumentException("Entidade inválida para exclusão.");

            var success = await _entityRepository.DeleteAsync(entity);

            if (!success)
                throw new InvalidOperationException("A exclusão falhou. A entidade pode não existir ou já ter sido removida.");

            return MapToViewModel<VM>(entity);
        }

        public virtual async Task<IEnumerable<VM>> ListPaginatedAsync<D, VM>(D model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            dynamic dynModel = model; // assumindo que D tem Page e PerPage
            int page = dynModel.Page < 1 ? 1 : dynModel.Page;
            int perPage = dynModel.PerPage switch
            {
                < 1 => 1,
                > 50 => 50,
                _ => dynModel.PerPage
            };

            var entities = await _entityRepository.GetPaginatedAsync(page, perPage);
            return entities.Select(MapToViewModel<VM>);
        }

        public virtual async Task<VM> GetByIdAsync<D, VM>(D model)
        {
            var id = ExtractId(model);
            var entity = await _entityRepository.GetByIdAsync(Convert.ToInt64(id));
            return MapToViewModel<VM>(entity);
        }

        public virtual async Task<IEnumerable<VM>> ListAllAsync<D, VM>(D model)
        {
            var entities = await _entityRepository.GetAllAsync();
            return entities.Select(MapToViewModel<VM>);
        }

        protected abstract TModel MapToEntity<D>(D dto);
        protected abstract VM MapToViewModel<VM>(TModel entity);
        protected abstract object ExtractId<D>(D dto);
    }

}
