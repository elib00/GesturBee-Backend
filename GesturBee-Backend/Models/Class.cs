﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class Class
    {
        [Key]
        public int Id {  get; set; }

        public string? ClassName { get; set; }
        public string? ClassDescription { get; set; }


        [ForeignKey("TeacherId")]
        public int TeacherId { get; set; }

            
        [JsonIgnore]
        public Teacher? Teacher { get; set; }

    }
}
