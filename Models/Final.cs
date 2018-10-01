using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models
{
    public class Auction
    {
        [Key]
        public int auction_id { get; set; }

        [RequiredAttribute]
        [MinLength(4)]
        [Display(Name="Product Name")]
        public string product_name { get; set; }

        [RequiredAttribute]
        [Display(Name="Starting Bid")]
        [Range(1, Int32.MaxValue)]
        public int starting_bid { get; set; }

        [RequiredAttribute]
        [MinLength(10)]
        [Display(Name="Product Description")]
        public string product_description { get; set; }

        [RequiredAttribute]
        [FutureDate]
        [Display(Name="End Date")]
        public DateTime end_date { get; set; }

        public int user_id { get; set; }
        public User creator {get;set;}
        public int highest_bid { get; set; }
        public int highest_bidder_id { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
    }
    public class Bid
    {
        [RequiredAttribute]
        [Range(1, Int32.MaxValue)]
        public int amount { get; set; }
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!(value is DateTime))
            {
                return new ValidationResult("Not a valid date.");
            }
                
            DateTime date = Convert.ToDateTime(value);

            if(date < DateTime.Now)
            {
                return new ValidationResult("Date must be a future date.");
            }

            return ValidationResult.Success;

        }
    }
}