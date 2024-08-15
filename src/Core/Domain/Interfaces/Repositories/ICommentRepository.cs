﻿using Domain.Contracts;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommentRepository
    {
        Task Update(Comment comment);
        Task<Comment> GetById(string commentId);

        Task<List<Comment>> GetAllByLiveIdAndPerfilId(Guid liveId, Guid perfilId);
    }
}
