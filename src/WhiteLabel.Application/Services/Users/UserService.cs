using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Domain.Specification.Users;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Guid> userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private AuthConfiguration AuthConfiguration { get; }

        public UserService(
            IGenericRepository<Guid> userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            AuthConfiguration = configuration
                .GetSection(AuthConfiguration.Section)
                .Get<AuthConfiguration>();
        }

        public async Task<UserInfoDto> GetUserInfo(string token, ClaimsPrincipal principal)
        {
            UserInfoDto userInfo = null;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token.Replace("Bearer ", "")
                );
            var response = await client.GetAsync($"{AuthConfiguration.Authority}/userinfo");
            if (response.IsSuccessStatusCode)
            {
                userInfo = await response.Content.ReadAsAsync<UserInfoDto>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                // Retry if TooManyRequests
                // https://auth0.com/docs/support/policies/rate-limit-policy
                await Task.Delay(2000);
                userInfo = await response.Content.ReadAsAsync<UserInfoDto>();
            }

            // Add permissions and roles (Auth0)
            if (userInfo != null && AuthConfiguration.AuthType == AuthConstants.Auth0)
            {
                // Configure a new rule in Auth0 in Auth Pipeline / Rules
                // https://oscarchelo.blogspot.com/2021/09/get-permissions-and-roles-with-auth0.html
                userInfo.Roles = principal
                    .Claims.Where(x => x.Type == $"{AuthConfiguration.Namespace}/roles")
                    .Select(x => x.Value)
                    .ToList();
                userInfo.Permissions = principal
                    .Claims.Where(x => x.Type == $"{AuthConfiguration.Namespace}/permissions")
                    .Select(x => x.Value)
                    .ToList();
            }

            return userInfo;
        }

        public async Task<Response<UserDto>> Add(UserDto userDto)
        {
            var alreadyRegisteredSpec = new UserByEmailSpec(userDto.Email);

            var existingUser = await userRepository.FindOneAsync(alreadyRegisteredSpec);

            if (existingUser != null)
                return new Response<UserDto>("User with this email already exists");

            var user = User.Create(userDto.Name, userDto.Email);

            unitOfWork.BeginTransaction();

            userRepository.Add(user);

            await unitOfWork.CommitAsync();

            return new Response<UserDto>(mapper.Map<User, UserDto>(user));
        }

        public async Task Update(UserDto userDto)
        {
            if (userDto.Id == Guid.Empty)
                throw new ArgumentException("Id can't be empty");

            var registeredSpec = new UserByIdSpec(userDto.Id);

            var user = await userRepository.FindOneAsync(registeredSpec);

            if (user == null)
                throw new ArgumentException("No such user exists");

            user.Name = userDto.Name;
            user.Email = userDto.Email;

            await unitOfWork.CommitAsync();
        }

        public async Task Remove(Guid userId)
        {
            var registeredSpec = new UserByIdSpec(userId);

            var user = await userRepository.FindOneAsync(registeredSpec);

            if (user == null)
                throw new ArgumentException("No such customer exists");

            userRepository.Delete(user);

            await unitOfWork.CommitAsync();
        }

        public async Task<Response<UserDto>> Get(Guid userId)
        {
            ISpecification<User> registeredSpec = new UserByIdSpec(userId);

            var user = await userRepository.FindOneAsync(registeredSpec);

            return new Response<UserDto>(mapper.Map<User, UserDto>(user));
        }

        public async Task<Response<IEnumerable<UserDto>>> GetAll()
        {
            var result = await userRepository.FindAllAsync<User>();

            return new Response<IEnumerable<UserDto>>(
                mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(result)
            );
        }

        public async Task<bool> EmailAvailable(string email)
        {
            ISpecification<User> alreadyRegisteredSpec = new UserByEmailSpec(email);

            var existingUser = await userRepository.FindOneAsync(alreadyRegisteredSpec);

            return existingUser != null;
        }

        public async Task<Response<PagedQueryResultDto<UserDto>>> GetPaginated(
            IPageOption pageOption
        )
        {
            var result = await userRepository.FindPagedAsync<User>(pageOption, null);

            return new Response<PagedQueryResultDto<UserDto>>(
                new PagedQueryResultDto<UserDto>(
                    result.Take,
                    result.Skip,
                    result.Total,
                    mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(result.Result)
                )
            );
        }

        public async Task<Response<IEnumerable<GroupDto>>> GetGrouped(string fieldToGroup)
        {
            var result = await userRepository.FindGroupAsync<User>(fieldToGroup, null);

            return new Response<IEnumerable<GroupDto>>(
                mapper.Map<IEnumerable<string>, IEnumerable<GroupDto>>(result)
            );
        }
    }
}
