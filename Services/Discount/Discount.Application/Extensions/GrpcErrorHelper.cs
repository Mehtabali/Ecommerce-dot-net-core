using Grpc.Core;
using Google.Rpc;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace Discount.Application.Extensions
{
    public static class GrpcErrorHelper
    {
        public static RpcException CreateValidationException(Dictionary<string, string> fieldErrors)
        {
            var fieldValidations = new List<BadRequest.Types.FieldViolation>();
            foreach (var error in fieldErrors)
            {
                fieldValidations.Add(new BadRequest.Types.FieldViolation
                {
                    Field = error.Key,
                    Description = error.Value
                });
            }
            //Now Add bad request
            var badRequest = new BadRequest();
            badRequest.FieldViolations.AddRange(fieldValidations);
            var status = new Google.Rpc.Status
            {
                Code = (int)StatusCode.InvalidArgument,
                Message = "Validation Field",
                Details = { Any.Pack(badRequest) }
            };
            var trailors = new Metadata
                {
                    {"grpc-status-details-bin", status.ToByteArray() }
                };
            return new RpcException(new Grpc.Core.Status(StatusCode.InvalidArgument, "Validation Errors"), trailors);
        }
    }
}
