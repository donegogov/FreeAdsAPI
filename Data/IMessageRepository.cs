using FreeAds.API.Dtos;
using FreeAds.API.Helpers;
using FreeAds.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeAds.API.Data
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        Task<bool> DeleteMessageAsync(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser();
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);
        Task<bool> SaveAllAsync();
    }
}
