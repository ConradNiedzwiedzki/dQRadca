using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace dQRadca.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string OwnerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Data utworzenia")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Nazwa produktu i model")]
        [Display(Name = "Nazwa produktu i model")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Wpisz adres strony www produktu")]
        [RegularExpression(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www8\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})", ErrorMessage = "Popraw format adresu (np. http://www.abc.de)")]
        [Display(Name = "Adres strony WWW opisu produktu")]
        public string UrlString { get; set; }

        [Display(Name = "Kod QR produktu")]
        public string QRCode { get; set; }

        public bool IsSelected { get; set; }
    }
}