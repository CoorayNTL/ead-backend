﻿using MongoDB.Bson.Serialization.Attributes;

namespace HealthyBites.Dtos
{
    public class VendorCreateDto
    {
        //public string VendorName { get; set; }
        public bool IsActive { get; set; }
        
        public float AverageRating { get; set; }
       
        public string Comments { get; set; }

    }
}
