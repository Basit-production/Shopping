using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1.GenericRepository;
using Ahmed_mart.Repository.v1.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace Ahmed_mart.Filter.v1
{
    public class UserTrackingFilterAsync : IAsyncActionFilter
    {
        private ILogger<UserTrackingFilterAsync> _logger;
        private IGenericRepository<Admin> _adminRepo;
        private IGenericRepository<User> _userRepo;
     
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                _logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<UserTrackingFilterAsync>)) as ILogger<UserTrackingFilterAsync>;
                // execute any code before the action executes
                var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();
                var token = !string.IsNullOrEmpty(authorization) ? authorization.Split(' ')[1] != "null" ? authorization.Split(' ')[1] : null : null;

                if (!string.IsNullOrEmpty(token))
                {
                    var role = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                    int userId = int.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    string errMsg = string.Empty;

                    if (role == "Admin")
                    {
                        _adminRepo = context.HttpContext.RequestServices.GetService(typeof(IGenericRepository<Admin>)) as IGenericRepository<Admin>;
                        var data = await _adminRepo.GetByIdAsync(userId);
                        if (!string.Equals(token, data.Token))
                        {
                            errMsg = "Multiple concurrent logins per user is not possible.";
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            await context.HttpContext.Response.WriteAsync(errMsg);
                            return;
                        }
                    }
                    else
                    {
                        _userRepo = context.HttpContext.RequestServices.GetService(typeof(IGenericRepository<User>)) as IGenericRepository<User>;
                        var data = await _userRepo.GetByIdAsync(userId);
                        //if (!string.Equals(token, data.Token))
                        //{
                        //    errMsg = "Multiple concurrent logins per user is not possible.";
                        //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        //    await context.HttpContext.Response.WriteAsync(errMsg);
                        //    return;
                        //}
                    }
                }

                var result = await next();
                // execute any code after the action executes
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(OnActionExecutionAsync)}");
            }
        }
    }
}
