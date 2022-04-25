using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class Delete
    {
        public class Command : IRequest
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this._userAccessor = userAccessor;
                this._context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var observer = await _context.Users.SingleOrDefaultAsync(
                    x => x.UserName == _userAccessor.GetCurrentUsername()
                );

                var target = await _context.Users.SingleOrDefaultAsync(
                    x => x.UserName == request.Username
                );

                if (target == null)
                {
                    throw new RestException(
                        System.Net.HttpStatusCode.NotFound,
                        new { User = "Not found" }
                    );
                }

                var following = await _context.Followings.SingleOrDefaultAsync(
                    x => x.ObserverId == observer.Id && x.TargetId == target.Id
                );

                if (following == null)
                {
                    throw new RestException(
                        System.Net.HttpStatusCode.BadRequest,
                        new { User = "You are not following this user" }
                    );
                }

                if (following != null)  _context.Followings.Remove(following);

                var success = await _context.SaveChangesAsync() > 0;

                return success ? Unit.Value : throw new Exception("Problem saving changes");
            }
        }
    }
}