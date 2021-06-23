using FluentValidation;
using Hackney.Core.Validation;

namespace ReferenceDataApi.V1.Boundary.Request.Validation
{
    public class GetReferenceDataQueryValidator : AbstractValidator<GetReferenceDataQuery>
    {
        public GetReferenceDataQueryValidator()
        {
            RuleFor(x => x.Category).NotNull()
                                    .NotEmpty()
                                    .NotXssString();
            RuleFor(x => x.SubCategory).NotXssString()
                                       .When(x => !string.IsNullOrEmpty(x.SubCategory));
        }
    }
}
