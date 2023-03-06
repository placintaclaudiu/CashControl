﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CashControl.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Column(TypeName = "nvarchar(60)")] // Tipul de date
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Emoji { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Cheltuiala";

    }
}
