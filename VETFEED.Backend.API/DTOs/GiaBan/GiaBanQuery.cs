using System;

namespace VETFEED.Backend.API.DTOs.GiaBan
{
    public class GiaBanQuery
    {
        public Guid? MaSP { get; set; }
        public DateTime? From { get; set; } // ngày
        public DateTime? To { get; set; }   // ngày
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
