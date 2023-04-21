﻿namespace ShippingApp.Models
{
    public class APIResponseModel
    {
        public int statusCode { get; set; } 
        public string message { get; set; } 
        public object? data { get; set; } 
        public bool isSuccess { get; set; } 
    }
}
