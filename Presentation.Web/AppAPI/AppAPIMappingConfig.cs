using Core.DomainModel;
using Presentation.Web.AppAPI.ViewModels;

namespace Presentation.Web.AppAPI
{
    public class AppAPIMappingConfig : AutoMapper.Profile
    {
        public AppAPIMappingConfig()
        {
            CreateMap<EmploymentViewModel, Employment>().ReverseMap();
            CreateMap<RateViewModel, Rate>().ReverseMap();
            CreateMap<DriveReportViewModel, DriveReport>();
            CreateMap<RouteViewModel, Route>();
            CreateMap<GPSCoordinateModel, GPSCoordinate>();
            CreateMap<Core.DomainModel.Profile, ProfileViewModel>();
            CreateMap<OrgUnit, OrgUnitViewModel>();
        }
    }
}