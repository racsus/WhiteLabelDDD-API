using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;
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
        private IConfiguration Configuration { get; }
        private AuthConfiguration AuthConfiguration { get; }

        public UserService(IGenericRepository<Guid> genericRepository, IUnitOfWork unitOfWork, 
            IMapper mapper, IConfiguration configuration)
        {
            this.genericRepository = genericRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.AuthConfiguration = configuration.GetSection(AuthConfiguration.Section).Get<AuthConfiguration>();
        }

        public async Task<UserInfoDTO> GetUserInfo(string token, ClaimsPrincipal principal)
        {
            UserInfoDTO userInfo = null;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            HttpResponseMessage response = await client.GetAsync($"{this.AuthConfiguration.Authority}/userinfo");
            if (response.IsSuccessStatusCode)
            {
                userInfo = await response.Content.ReadAsAsync<UserInfoDTO>();
            }

            // Add permissions and roles (Auth0)
            if ((userInfo != null) && (this.AuthConfiguration.AuthType == AuthConstants.Auth0))
            {
                // Configure a new rule in Auth0 in Auth Pipeline / Rules
                // https://oscarchelo.blogspot.com/2021/09/get-permissions-and-roles-with-auth0.html
                userInfo.Roles = principal.Claims.Where(x => x.Type == $"{this.AuthConfiguration.Namespace}/roles")?.Select(x => x.Value).ToList();
                userInfo.Permissions = principal.Claims.Where(x => x.Type == $"{this.AuthConfiguration.Namespace}/permissions")?.Select(x => x.Value).ToList();
            }

            return userInfo;
        }

        public async Task<Response<UserDTO>> Add(UserDTO userDto)
        {
            ISpecification<User> alreadyRegisteredSpec =
                new UserAlreadyRegisteredSpec(userDto.Email);

            User existingUser = await this.genericRepository.FindOneAsync(alreadyRegisteredSpec);

            if (existingUser != null)
            {
                return new Response<UserDTO>("User with this email already exists");
            }                

            User user =
                User.Create(userDto.Name, userDto.Email);

            this.unitOfWork.BeginTransaction();
            this.genericRepository.Add(user);
            this.unitOfWork.Commit();

            return new Response<UserDTO>(this.mapper.Map<User, UserDTO>(user));
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

        public async Task<Response<UserDTO>> Get(Guid userId)
        {
            ISpecification<User> registeredSpec =
                new UserRegisteredSpec(userId);

            User user = await this.genericRepository.FindOneAsync(registeredSpec);

            return new Response<UserDTO>(this.mapper.Map<User, UserDTO>(user));
        }

        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            var result = await this.genericRepository.FindAllAsync<User>();

            return new Response<IEnumerable<UserDTO>>(this.mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(result));
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            ISpecification<User> alreadyRegisteredSpec =
                new UserAlreadyRegisteredSpec(email);

            User existingUser = await this.genericRepository.FindOneAsync(alreadyRegisteredSpec);

            return (existingUser != null);
        }

        public async Task<Response<PagedQueryResultDTO<UserDTO>>> GetPaginated(IPageOption pageOption)
        {
            var result = await this.genericRepository.FindPagedAsync<User>(pageOption, null);

            return new Response<PagedQueryResultDTO<UserDTO>>(new PagedQueryResultDTO<UserDTO>(result.Take, result.Skip, result.Total, 
                this.mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(result.Result)));
        }
    }

}
