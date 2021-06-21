using ReferenceDataApi.V1.Boundary.Response;

namespace ReferenceDataApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
