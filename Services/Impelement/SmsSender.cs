using KaraYadak.Data;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly ApplicationDbContext _context;

        public SmsSender(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> SendWithPattern(string phone, string patternCode, string data)
        {
            // call sms api

            var apiResult = CallApi(phone, patternCode, data);

            //await _context.Sms.AddAsync(new Sms
            //{
            //    Body = data,
            //    Phone = phone,
            //    PatternCode = patternCode,
            //    ResCode = apiResult
            //});
            //await _context.SaveChangesAsync();

            return apiResult;
        }

        private string CallApi(string phone, string patternCode, string inputData)
        {
            // {\"code\": \" " + content + " \"}
            var client = new RestClient("http://188.0.240.110/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"pattern\"" +
                ",\"user\" : \"09111111921\"" +
                ",\"pass\":  \"2064721401\"" +
                ",\"fromNum\" : \"+981000100087\"" +
                ",\"toNum\": \"" + phone + "\"" +
                ",\"patternCode\": \"" + patternCode + "\"" +
                ",\"inputData\" : [" + inputData + "]}"
                , ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
