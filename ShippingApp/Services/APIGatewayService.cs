using ShippingApp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class APIGatewayService : IAPIGatewayService
    {
        Response response = new Response();
        public List<CheckpointModel> GetCheckpoints(Guid shipmentId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.180.2.128:4000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringBuilder appendUrl = new StringBuilder("api/shipment/getShipmentRoute?");
                if (shipmentId != null)
                {
                    appendUrl.Append("shipmentId=" + shipmentId + "");
                }
                var res = client.GetAsync(appendUrl.ToString()).Result;
                var data = res.Content.ReadAsStringAsync().Result;
                APIResponseModel resp = JsonSerializer.Deserialize<APIResponseModel>(data)!;
                var obj = JsonSerializer.Serialize(resp.data);
                var list = JsonSerializer.Deserialize<List<CheckpointModel>>(obj);
                response.IsSuccess = true;
                response.StatusCode = 200;
                response.Message = "shipment Route";
                response.Data =list!;
                return list;
            }
        }
    }
}
