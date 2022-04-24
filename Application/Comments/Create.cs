using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<CommentDTO>
        {
            public Guid ActivityId { get; set; }
            public string Body { get; set; }
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommentDTO>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this._mapper = mapper;
                this._context = context;
            }

            public async Task<CommentDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);

                if (activity == null)
                {
                    throw new RestException(
                        System.Net.HttpStatusCode.NotFound,
                        new { Activity = "Not found" }
                    );
                }

                var user = await _context.Users.SingleOrDefaultAsync(
                    x => x.UserName == request.Username
                );

                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body,
                    CreatedAt = DateTime.Now,
                };

                activity.Comments.Add(comment);

                var success = await _context.SaveChangesAsync() > 0;

                return success
                    ? _mapper.Map<CommentDTO>(comment)
                    : throw new Exception("Problem saving changes");
            }
        }
    }
}