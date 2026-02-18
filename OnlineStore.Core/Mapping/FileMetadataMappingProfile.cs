using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Mapping
{
    public class FileMetadataMappingProfile : Profile
    {
        public FileMetadataMappingProfile()
        {
            // FileMetadata -> FileMetadataDTO
            CreateMap<FileMetadata, FileMetadataDTO>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.OriginalFileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.UploadedBy))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.UploadedAt))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => $"/api/files/{src.Id}"))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
                .ForMember(dest => dest.DownloadCount, opt => opt.MapFrom(src => src.DownloadCount))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.DateTaken, opt => opt.MapFrom(src => src.DateTaken))
                .ForMember(dest => dest.CameraModel, opt => opt.MapFrom(src => src.CameraModel))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => src.Orientation));
        }
    }
}