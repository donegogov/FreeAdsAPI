using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeAds.API.Dtos;
using FreeAds.API.Helpers;
using FreeAds.API.Models;

namespace FreeAds.API.Data
{
    public interface IClassifiedAdsRepository
    {
        void Add<T>(T entity) where T: class;
        Task<ClassifiedAds> Add(ClassifiedAds classifiedAds);
        //void Delete<T>(T entity) where T: class;
        Task<bool> DeleteLike(Like like);
        Task<bool> Delete(Guid id);
        Task<bool> AdminDelete(Guid id);
        Task<bool> DeletePhoto(Photo photo);
        Task<bool> SaveAll();
        PagedList<ClassifiedAds> GetClassifiedAds(ClassifiedAdsParams classifiedAdsParams);
        PagedList<ClassifiedAds> GetRelevantClassifiedAds(string city, ClassifiedAdsParams classifiedAdsParams);
        Task<IEnumerable<ClassifiedAds>> SearchClassifiedAds(SearchQueryParametarsDto searchQueryParametars, string? userId);
        Task<IEnumerable<ClassifiedAds>> GetClassifiedAdsForUser(string userId);
        Task<ClassifiedAds> GetClassifiedAdDetail(Guid id);
        Task<Photo> GetPhoto(Guid id);
        Task<Photo> GetMainPhotoForClassifiedAd(Guid classfiedAdId);
        Task<Like> GetLike(string userId, Guid classifiedAdId);
        Task<PagedList<ClassifiedAds>> GetUserLikedClassifiedAds(ClassifiedAdsParams classifiedAdsParams);
        Task<int> GetNumberOfLikesOfClassifiedAd(Guid classifiedAdId);
        /*String ConvertEngToMkd(String input);*/
        Task<ClassifiedAds> ApproveOrDisapproveClassifiedAds(Guid classifiedAdId, string approvedOrDisapproved);
        IEnumerable<ClassifiedAds> GetAdsForModeration();
        Task<Photo> ApproveOrDisapprovePhoto(Guid photoId, string approvedOrDisapproved);
        Task<ClassifiedAds> ApproveOrDisapproveClassifiedAdsAndPhotos(Guid classifiedAdId, string approvedOrDisapproved);
        Task<ClassifiedAds> GetClassifiedAdDetailAndPhotos(Guid id);
        Task<ClassifiedAds> GetClassifiedAdDetaiForUser(Guid id);
        Task<Photo> GetMainPhotoForClassifiedAdForUserUpdate(Guid classfiedAdId);
        List<ClassifiedAdsForSearchDto> GetClassifiedAdsForSearch();

    }
}