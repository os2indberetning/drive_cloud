using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IPersonalRouteService
    {
        PersonalRoute Create(PersonalRoute route);
    }
}
