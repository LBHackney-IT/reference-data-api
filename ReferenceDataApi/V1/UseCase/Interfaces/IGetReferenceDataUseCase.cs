using ReferenceDataApi.V1.Boundary.Request;
using ReferenceDataApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace ReferenceDataApi.V1.UseCase.Interfaces
{
    public interface IGetReferenceDataUseCase
    {
        Task<ResponseObjectList> ExecuteAsync(GetReferenceDataQuery query);
    }
}
