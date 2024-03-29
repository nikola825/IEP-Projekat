﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    public class Auction:IValidatableObject
    {
        public Auction()
        {

        }

        public Auction(AuctionViewModel model)
        {
            
            Id=model.Id;
            Name=model.Name;
            Duration=model.Duration;
            StartAmmount=model.StartAmmount;
            EndDate=model.EndDate;
            PictureContent=null;
            StartDate=model.StartDate;
            Status=model.Status;
            User = model.User;
            CurrentAmmount = model.CurrentAmmount;
            Currency = model.Currency;
            CurrencyPrice = model.CurrencyPrice;
            
            if(model.Picture!=null)
            {
                using (BinaryReader br = new BinaryReader(model.Picture.InputStream))
                {
                    PictureContent = br.ReadBytes((int)model.Picture.InputStream.Length);
                }
            }

        }
        [Key]
        public string Id { get; set; }

        [Required]
        [Display(Name ="Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name="Duration")]
        public long Duration { get; set; }

        [Required]
        [Display(Name ="Start ammount")]
        public decimal StartAmmount { get; set; }

        [Required]
        [Display(Name = "Current ammount")]
        public decimal CurrentAmmount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal CurrencyPrice { get; set; }

        public enum AuctionStatus { READY, OPENED, COMPLETED};

        [Required]
        [Display(Name="Status")]
        public AuctionStatus Status { get; set; }

        [Required]
        [Display(Name="Start date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(1024*1024*10)]
        public byte[] PictureContent { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public ApplicationUser LastBidder { get; set; }

        [NotMapped]
        public AuctionStatus RealStatus
        {
            get
            {
                if (Status == AuctionStatus.READY) return AuctionStatus.READY;
                if (Status == AuctionStatus.COMPLETED) return AuctionStatus.COMPLETED;
                if (EndDate <= DateTime.Now) return AuctionStatus.COMPLETED;
                return AuctionStatus.OPENED;
            }
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            if(StartAmmount<0)
            {
                validationErrors.Add(new ValidationResult("Start ammount must be positive"));
            }
            if (Duration < 0)
            {
                validationErrors.Add(new ValidationResult("Duration ammount must be positive"));
            }
            return validationErrors;
        }
    }

    public class AuctionViewModel:IValidatableObject
    {
        public AuctionViewModel()
        {

        }
        public AuctionViewModel(Auction au)       
        {
            this.Id = au.Id;
            this.Name = au.Name;
            this.Duration = au.Duration;
            this.StartAmmount = au.StartAmmount;
            this.StartDate = au.StartDate;
            this.Status = au.Status;
            this.EndDate = au.EndDate;
            this.Picture = null;
            this.LastBidder = au.LastBidder;
            this.User = au.User;
            this.CurrentAmmount = au.CurrentAmmount;
            this.Currency = au.Currency;
            this.CurrencyPrice = au.CurrencyPrice;
        }
        
        public string Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Duration")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Duration must be a positive non zero number")]
        public long Duration { get; set; }

        [Required]
        [Display(Name = "Start ammount")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Start ammount must be a positive non zero number")]
        public decimal StartAmmount { get; set; }

        [Display(Name = "Current ammount")]
        public decimal CurrentAmmount { get; set; }

        [Display(Name = "Status")]
        public Auction.AuctionStatus Status { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public string Currency { get; set; }

        public decimal CurrencyPrice { get; set; }

        public ApplicationUser User { get; set; }

        public Auction.AuctionStatus RealStatus
        {
            get
            {
                if (Status == Auction.AuctionStatus.READY) return Auction.AuctionStatus.READY;
                if (Status == Auction.AuctionStatus.COMPLETED) return Auction.AuctionStatus.COMPLETED;
                if (EndDate <= DateTime.Now) return Auction.AuctionStatus.COMPLETED;
                return Auction.AuctionStatus.OPENED;
            }
        }

        [Required]
        [Display(Name = "Picture (3.5MB max)")]
        public HttpPostedFileBase Picture { get; set; }

        public ApplicationUser LastBidder { get; set; }

        public string CurrencyDescription
        {
            get
            {
                return Currency + " (" + CurrencyPrice.ToString() + " per token)";
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            if(Picture!=null)
            {
                if(!Picture.ContentType.StartsWith("image/"))
                {
                    validationErrors.Add(new ValidationResult("You didn't upload a picture"));
                }
            }
            return validationErrors;
        }
    }
}