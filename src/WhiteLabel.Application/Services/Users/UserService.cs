using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.Domain.Events;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Domain.Specification;
using WhiteLabel.Domain.Specification.Users;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Application.Services.Users
{
    public class UserService: IUserService
    {
        private readonly IGenericRepository<Guid> genericRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UserService(IGenericRepository<Guid> genericRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.genericRepository = genericRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<UserDTO> Add(UserDTO userDto)
        {
            ISpecification<User> alreadyRegisteredSpec =
                new UserAlreadyRegisteredSpec(userDto.Email);

            User existingUser = await this.genericRepository.FindOneAsync(alreadyRegisteredSpec);

            if (existingUser != null)
            {
                throw new ArgumentException("User with this email already exists");
            }                

            User user =
                User.Create(userDto.Name, userDto.Email);

            this.unitOfWork.BeginTransaction();
            this.genericRepository.Add(user);
            this.unitOfWork.Commit();

            return this.mapper.Map<User, UserDTO>(user);
        }

        public async Task Update(UserDTO userDto)
        {
            if (userDto.Id == Guid.Empty)
                throw new ArgumentException("Id can't be empty");

            ISpecification<User> registeredSpec =
                new UserRegisteredSpec(userDto.Id);

            User user = await this.genericRepository.FindOneAsync(registeredSpec);

            if (user == null)
                throw new ArgumentException("No such user exists");

            user.Update(userDto.Name, userDto.Email);
            this.unitOfWork.Commit();
        }

        public async Task Remove(Guid userId)
        {
            ISpecification<User> registeredSpec =
                new UserRegisteredSpec(userId);

            User user = await this.genericRepository.FindOneAsync(registeredSpec);

            if (user == null)
                throw new ArgumentException("No such customer exists");

            this.genericRepository.Delete(user);
            this.unitOfWork.Commit();
        }

        public async Task<UserDTO> Get(Guid userId)
        {
            ISpecification<User> registeredSpec =
                new UserRegisteredSpec(userId);

            User user = await this.genericRepository.FindOneAsync(registeredSpec);

            return this.mapper.Map<User, UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var result = await this.genericRepository.FindAllAsync<User>();

            return this.mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(result);
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            ISpecification<User> alreadyRegisteredSpec =
                new UserAlreadyRegisteredSpec(email);

            User existingUser = await this.genericRepository.FindOneAsync(alreadyRegisteredSpec);

            return (existingUser != null);
        }

        public async Task<PagedQueryResultDTO<UserDTO>> GetPaginated(IPageOption pageOption)
        {
            var result = await this.genericRepository.FindPagedAsync<User>(pageOption);

            return new PagedQueryResultDTO<UserDTO>(result.Take, result.Skip, result.Total, 
                this.mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(result.Result));
        }
    }

}
